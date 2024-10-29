mod ffi;
mod client_message;

use now_proto_pdu::{NowExecCapsetFlags, NowExecCapsetMsg, NowExecMessage, NowExecRunMsg, NowMessage, NowVarStr};
use client_message::{ui_interaction, ui_update};

/// This type specifies the action, which the frontend (e.g C# GUI code) should take after backend
/// (`DvcClientCtx`) finishes processing the request.
enum DvcClientResponse {
    SendMessage(NowMessage),
    UpdateUi(ui_update::Request),
    None,
}

/// Root DvcClient backend object.
#[derive(Debug, Default)]
struct DvcClientCtx {
    caps: Option<NowExecCapsetFlags>,
}

impl DvcClientCtx {
    /// Handles UI interaction from the frontend.
    fn handle_ui(&mut self, request: ui_interaction::Request) -> Result<DvcClientResponse, anyhow::Error> {
        match request {
            ui_interaction::Request::Init => {
                let message: NowMessage = NowExecCapsetMsg::new(
                    NowExecCapsetFlags::STYLE_RUN
                ).into();

                Ok(DvcClientResponse::SendMessage(message))
            }
            ui_interaction::Request::ExecRun(exec_run) => {
                let message: NowMessage = NowExecRunMsg::new(
                    exec_run.session_id,
                    NowVarStr::new(exec_run.cmd)?,
                )?.into();

                Ok(DvcClientResponse::SendMessage(message))
            }
        }
    }

    /// Handles messages received from the DVC channel.
    fn handle_message(&mut self, request: NowMessage) -> Result<DvcClientResponse, anyhow::Error> {
        match request {
            NowMessage::Exec(NowExecMessage::Capset(capset)) => {
                let flags = capset.flags();

                self.caps = Some(flags);

                let ui_update = ui_update::Request::ShowCapabilities(ui_update::ShowCapabilities {
                    run: flags.contains(NowExecCapsetFlags::STYLE_RUN),
                    cmd: flags.contains(NowExecCapsetFlags::STYLE_CMD),
                    process: flags.contains(NowExecCapsetFlags::STYLE_PROCESS),
                    shell: flags.contains(NowExecCapsetFlags::STYLE_SHELL),
                    batch: flags.contains(NowExecCapsetFlags::STYLE_BATCH),
                    winps: flags.contains(NowExecCapsetFlags::STYLE_WINPS),
                    pwsh: flags.contains(NowExecCapsetFlags::STYLE_PWSH),
                    applescript: flags.contains(NowExecCapsetFlags::STYLE_APPLESCRIPT),
                });

                Ok(DvcClientResponse::UpdateUi(ui_update))
            }
            _ => {
                Ok(DvcClientResponse::None)
            }
        }
    }
}
