use windows::{
    core::*,
    Win32::Foundation::*,
    Win32::Storage::FileSystem::*,
    Win32::System::Threading::*,
    Win32::System::RemoteDesktop::*,
    Win32::System::IO::*,
};

fn open_virtual_channel(channel_name: &str) -> Result<HANDLE> {
    unsafe {
        let channel_name_wide = PCSTR::from_raw(channel_name.as_ptr());
        let h_wts_handle = WTSVirtualChannelOpenEx(WTS_CURRENT_SESSION, channel_name_wide, WTS_CHANNEL_OPTION_DYNAMIC)
            .map_err(|e| std::io::Error::from_raw_os_error(e.code().0))?;

        let mut vc_file_handle_ptr: *mut HANDLE = std::ptr::null_mut();
        let mut len: u32 = 0;
        let wts_virtual_class: WTS_VIRTUAL_CLASS = WTSVirtualFileHandle;
        WTSVirtualChannelQuery(h_wts_handle, wts_virtual_class, &mut vc_file_handle_ptr as *mut _ as *mut _, &mut len)
            .map_err(|e| std::io::Error::from_raw_os_error(e.code().0))?;

        let mut new_handle: HANDLE = HANDLE(0);
        let duplicate_result = DuplicateHandle(GetCurrentProcess(),
            *vc_file_handle_ptr, GetCurrentProcess(), &mut new_handle, 0, false, DUPLICATE_SAME_ACCESS);
        
        WTSFreeMemory(vc_file_handle_ptr as *mut core::ffi::c_void);
        let _ = WTSVirtualChannelClose(h_wts_handle);

        Ok(new_handle)
    }
}

fn write_virtual_channel_message(h_file: HANDLE, cb_size: u32, buffer: *const u8) -> windows::core::Result<()> {
    unsafe {
        let buffer_slice = std::slice::from_raw_parts(buffer, cb_size as usize);
        let mut dw_written: u32 = 0;
        WriteFile(h_file, Some(buffer_slice), Some(&mut dw_written), None)
    }
}

const CHANNEL_PDU_LENGTH: usize = 1024;

fn handle_virtual_channel(h_file: HANDLE) -> windows::core::Result<()> {
    unsafe {
        let mut read_buffer = [0u8; CHANNEL_PDU_LENGTH];
        let mut overlapped = OVERLAPPED::default();
        let mut dw_read: u32 = 0;

        let cmd = "whoami\0";
        let cb_size = cmd.len() as u32;
        write_virtual_channel_message(h_file, cb_size, cmd.as_ptr())?;

        let h_event = CreateEventW(None, false, false, None)?;
        overlapped.hEvent = h_event;

        loop {
            // Notice the wrapping of parameters in Some()
            let result = ReadFile(h_file, Some(&mut read_buffer), Some(&mut dw_read), Some(&mut overlapped));

            if let Err(e) = result {
                if GetLastError() == WIN32_ERROR(ERROR_IO_PENDING.0) {
                    let dw_status = WaitForSingleObject(h_event, INFINITE);
                    if !GetOverlappedResult(h_file, &mut overlapped, &mut dw_read, false).is_ok() {
                        return Err(windows::core::Error::from_win32());
                    }
                } else {
                    return Err(e);
                }
            }

            println!("read {} bytes", dw_read);

            let packet_size = dw_read as usize - std::mem::size_of::<CHANNEL_PDU_HEADER>();
            let pData = read_buffer.as_ptr().offset(std::mem::size_of::<CHANNEL_PDU_HEADER>() as isize) as *const u8;

            println!(">> {}", std::str::from_utf8(std::slice::from_raw_parts(pData, packet_size)).unwrap_or("Invalid UTF-8"));

            if dw_read == 0 || ((*(pData.offset(-(std::mem::size_of::<CHANNEL_PDU_HEADER>() as isize)) as *const CHANNEL_PDU_HEADER)).flags & CHANNEL_FLAG_LAST) != 0 {
                break;
            }
        }

        Ok(())
    }
}

fn main() -> Result<()> {
    let channel_name = "DvcSample";
    let h_file = open_virtual_channel(channel_name)?;

    handle_virtual_channel(h_file);

    Ok(())
}
