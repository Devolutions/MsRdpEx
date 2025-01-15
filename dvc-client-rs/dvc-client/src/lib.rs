mod ffi;
mod client_message;

use now_proto_pdu::{NowExecAbortMsg, NowExecBatchMsg, NowExecCancelReqMsg, NowExecCapsetFlags, NowExecCapsetMsg, NowExecDataFlags, NowExecDataMsg, NowExecMessage, NowExecProcessMsg, NowExecPwshMsg, NowExecRunMsg, NowExecWinPsMsg, NowMessage, NowSeverity, NowStatus, NowStatusCode, NowVarBuf, NowVarStr};
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
                let message: NowMessage = match exec_run.exec_kind.as_str() {
                    "run" => {
                        NowExecRunMsg::new(
                            exec_run.session_id,
                            NowVarStr::new(exec_run.file)?,
                        )?.into()
                    }
                    "process" => {
                        NowExecProcessMsg::new(
                            exec_run.session_id,
                            NowVarStr::new(exec_run.file)?,
                            NowVarStr::new(exec_run.args)?,
                            NowVarStr::new(exec_run.directory)?,
                        )?.into()
                    }
                    "cmd" => {
                        NowExecBatchMsg::new(
                            exec_run.session_id,
                            NowVarStr::new(exec_run.file)?,
                        ).into()
                    }
                    "powershell" => {
                        NowExecWinPsMsg::new(
                            exec_run.session_id,
                            NowVarStr::new(exec_run.file)?,
                        )?.into()
                    }
                    "pwsh" => {
                        NowExecPwshMsg::new(
                            exec_run.session_id,
                            NowVarStr::new(exec_run.file)?,
                        )?.into()
                    }
                    _ => {
                        return Err(anyhow::anyhow!("Unknown exec kind: {}", exec_run.exec_kind)).into();
                    }
                };
                Ok(DvcClientResponse::SendMessage(message))
            }
            ui_interaction::Request::ExecAbort(abort) => {
                let message: NowMessage = NowExecAbortMsg::new(
                    abort.session_id,
                    NowStatus::new(NowSeverity::Fatal, NowStatusCode::FAILURE)
                ).into();

                Ok(DvcClientResponse::SendMessage(message))
            }
            ui_interaction::Request::ExecCancel(cancel) => {
                let message: NowMessage = NowExecCancelReqMsg::new(
                    cancel.session_id
                ).into();

                Ok(DvcClientResponse::SendMessage(message))
            }
            ui_interaction::Request::ExecStdin(data) => {
                let message: NowMessage = NowExecDataMsg::new(
                    NowExecDataFlags::STDIN,
                    data.session_id,
                    NowVarBuf::new(data.data.bytes().collect::<Vec<u8>>())?
                ).into();

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
            NowMessage::Exec(NowExecMessage::Abort(abort)) => {
                let ui_update = ui_update::Request::ExecLog(ui_update::ExecLog {
                    session_id: abort.session_id(),
                    info: format!("Execution aborted with status: {}", abort.status().code().0),
                });

                Ok(DvcClientResponse::UpdateUi(ui_update))
            }
            NowMessage::Exec(NowExecMessage::CancelRsp(cancel)) => {
                let ui_update = ui_update::Request::ExecLog(ui_update::ExecLog {
                    session_id: cancel.session_id(),
                    info: format!("Execution cancelled with status: {}", cancel.status().code().0),
                });

                Ok(DvcClientResponse::UpdateUi(ui_update))
            }
            NowMessage::Exec(NowExecMessage::Data(data)) => {
                let ui_update = ui_update::Request::ExecDataOut(ui_update::ExecDataOut {
                    session_id: data.session_id(),
                    stderr: data.flags().contains(NowExecDataFlags::STDERR),
                    data: String::from_utf8_lossy(data.data().value()).into_owned(),
                });

                Ok(DvcClientResponse::UpdateUi(ui_update))
            }
            NowMessage::Exec(NowExecMessage::Result(result)) => {
                let ui_update = ui_update::Request::ExecLog(ui_update::ExecLog {
                    session_id: result.session_id(),
                    info: format!("Execution finished with status: {}", result.status().code().0),
                });

                Ok(DvcClientResponse::UpdateUi(ui_update))
            }
            _ => {
                Ok(DvcClientResponse::None)
            }
        }
    }
}
