use crate::{client_message::ui_interaction, DvcClientCtx, DvcClientResponse};
use now_proto_pdu::NowMessage;


struct FfiDvcClientResponse {
    pub kind: &'static [u8],
    pub data: Vec<u8>,
}

impl FfiDvcClientResponse {
    pub fn from_result(value: Result<DvcClientResponse, anyhow::Error>) -> Option<Self> {
        const KIND_UPDATE_UI: &[u8] = b"update_ui\0";
        const KIND_SEND_MESSAGE: &[u8] = b"send_message\0";
        const KIND_ERROR: &[u8] = b"error\0";

        let (kind, data) = match value {
            Ok(DvcClientResponse::None) => return None,
            Ok(DvcClientResponse::UpdateUi(data)) => match serde_json::to_vec(&data) {
                Ok(data) => (KIND_UPDATE_UI, data),
                Err(error) => (KIND_ERROR, error.to_string().as_bytes().to_vec()),
            }
            Ok(DvcClientResponse::SendMessage(message)) => match ironrdp::core::encode_vec(&message) {
                Ok(data) => (KIND_SEND_MESSAGE, data),
                Err(error) => (KIND_ERROR, error.to_string().as_bytes().to_vec()),
            }
            Err(error) => (KIND_ERROR, error.to_string().as_bytes().to_vec()),
        };

        Some(Self { kind, data })
    }
}

fn dvcc_handle_ui_impl(ctx: *mut DvcClientCtx, req: *const u8, req_len: usize) -> Result<DvcClientResponse, anyhow::Error> {
    if ctx.is_null() {
        return Err(anyhow::anyhow!("Invalid context pointer")).into();
    }

    if req.is_null() {
        return Err(anyhow::anyhow!("Invalid request pointer")).into();
    }

    // SAFETY: As long as the client code is passing a valid pointer to this function, this
    // operation is safe.
    let ctx = unsafe { &mut *ctx };

    // SAFETY: As long as req is a valid pointer to a valid UTF-8 string with req_len bytes, this
    // operation is safe.
    let request_str = std::str::from_utf8( unsafe {
        std::slice::from_raw_parts(req, req_len)
    })?;

    let request: ui_interaction::Request = serde_json::from_str(request_str)?;

    ctx.handle_ui(request)
}

fn dvcc_handle_data_impl(ctx: *mut DvcClientCtx, data: *const u8, data_len: usize) -> Result<DvcClientResponse, anyhow::Error> {
    if ctx.is_null() {
        return Err(anyhow::anyhow!("Invalid context pointer")).into();
    }

    if data.is_null() {
        return Err(anyhow::anyhow!("Invalid data pointer")).into();
    }

    // SAFETY: As long as the client code is passing a valid pointer to this function, this
    // operation is safe.
    let ctx = unsafe { &mut *ctx };

    // SAFETY: As long as data is a valid pointer to a valid byte array with data_len bytes, this
    // operation is safe.
    let message = ironrdp::core::decode::<NowMessage>(unsafe { std::slice::from_raw_parts(data, data_len) })?;

    ctx.handle_message(message)
}

#[no_mangle]
extern "C" fn dvcc_init() -> *mut DvcClientCtx {
    Box::into_raw(Box::new(DvcClientCtx::default()))
}

#[no_mangle]
extern "C" fn dvcc_destroy(ctx: *mut DvcClientCtx) {
    if !ctx.is_null() {
        // SAFETY: As long as the client code is passing a valid pointer to this function, this
        // operation is safe.
        unsafe {
            let _ = Box::from_raw(ctx);
        }
    }
}

/// Handle incoming UI requests.
///
/// UI requests are passed as serialized JSON objects.
#[no_mangle]
extern "C" fn dvcc_handle_ui(ctx: *mut DvcClientCtx, req: *const u8, req_len: usize) -> *mut FfiDvcClientResponse {
    let response = match FfiDvcClientResponse::from_result(dvcc_handle_ui_impl(ctx, req, req_len)) {
        Some(response) => response,
        None => return std::ptr::null_mut(),
    };

    Box::into_raw(response.into())
}


/// Handle incoming UI requests.
///
/// UI requests are passed as serialized JSON objects.
#[no_mangle]
extern "C" fn dvcc_handle_data(ctx: *mut DvcClientCtx, data: *const u8, data_len: usize) -> *mut FfiDvcClientResponse {
    let response = match FfiDvcClientResponse::from_result(dvcc_handle_data_impl(ctx, data, data_len)) {
        Some(response) => response,
        None => return std::ptr::null_mut(),
    };

    Box::into_raw(response.into())
}

/// Destroy the result object returned by `dvcc_handle_ui` or `dvcc_handle_data`.
#[no_mangle]
extern "C" fn dvcc_response_destroy(result: *mut FfiDvcClientResponse) {
    if !result.is_null() {
        // SAFETY: As long as the client code is passing a valid pointer to this function, this
        // operation is safe.
        unsafe {
            let _ = Box::from_raw(result);
        }
    }
}

/// Return null-terminated const string representing the kind of the response.
#[no_mangle]
extern "C" fn dvcc_response_get_kind(result: *const FfiDvcClientResponse) -> *const u8 {
    if result.is_null() {
        return std::ptr::null();
    }

    let result = unsafe { &*result };

    result.kind.as_ptr()
}

#[no_mangle]
extern "C" fn dvcc_response_get_data(result: *const FfiDvcClientResponse) -> *const u8 {
    if result.is_null() {
        return std::ptr::null();
    }

    let result = unsafe { &*result };

    result.data.as_ptr()
}

#[no_mangle]
extern "C" fn dvcc_response_get_data_len(result: *const FfiDvcClientResponse) -> usize {
    if result.is_null() {
        return 0;
    }

    let result = unsafe { &*result };

    result.data.len()
}