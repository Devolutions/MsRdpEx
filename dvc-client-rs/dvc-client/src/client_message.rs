pub mod ui_update {
    use serde::{Deserialize, Serialize};

    #[derive(Debug, Serialize, Deserialize)]
    pub struct ShowCapabilities {
        pub run: bool,
        pub cmd: bool,
        pub process: bool,
        pub shell: bool,
        pub batch: bool,
        pub winps: bool,
        pub pwsh: bool,
        pub applescript: bool,
    }

    #[derive(Debug, Serialize, Deserialize)]
    pub struct MessageBoxResult {
        pub message_box_kind: String,
    }

    #[derive(Debug, Serialize, Deserialize)]
    pub struct ExecLog {
        pub session_id: u32,
        pub info: String,
    }

    #[derive(Debug, Serialize, Deserialize)]
    pub struct ExecDataOut {
        pub session_id: u32,
        pub stderr: bool,
        pub data: String,
    }

    #[derive(Debug, Serialize, Deserialize)]
    #[serde(tag = "kind")]
    pub enum Request {
        ShowCapabilities(ShowCapabilities),
        MessageBoxResult(MessageBoxResult),
        ExecLog(ExecLog),
        ExecDataOut(ExecDataOut),
    }
}

pub mod ui_interaction {
    use serde::{Deserialize, Serialize};

    #[derive(Debug, Serialize, Deserialize)]
    pub struct ExecRun {
        pub session_id: u32,
        pub exec_kind: String,
        pub file: String,
        pub args: String,
        pub directory: String,
    }

    #[derive(Debug, Serialize, Deserialize)]
    pub struct ExecAbort {
        pub session_id: u32,
        pub status: u16,
    }

    #[derive(Debug, Serialize, Deserialize)]
    pub struct ExecCancel {
        pub session_id: u32,
    }

    #[derive(Debug, Serialize, Deserialize)]
    pub struct ExecStdin {
        pub session_id: u32,
        pub data: String,
        pub eof: bool,
    }

    #[derive(Debug, Serialize, Deserialize)]
    #[serde(tag = "kind")]
    pub enum Request {
        Init,
        ExecRun(ExecRun),
        ExecAbort(ExecAbort),
        ExecCancel(ExecCancel),
        ExecStdin(ExecStdin),
    }
}