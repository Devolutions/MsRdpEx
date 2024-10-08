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
        pub kind: String,
    }

    #[derive(Debug, Serialize, Deserialize)]
    #[serde(tag = "kind")]
    pub enum Request {
        ShowCapabilities(ShowCapabilities),
        MessageBoxResult(MessageBoxResult),
    }
}

pub mod ui_interaction {
    use serde::{Deserialize, Serialize};

    #[derive(Debug, Serialize, Deserialize)]
    pub struct ExecRun {
        pub cmd: String,
        pub session_id: u32,
    }

    #[derive(Debug, Serialize, Deserialize)]
    #[serde(tag = "kind")]
    pub enum Request {
        Init,
        ExecRun(ExecRun),
    }
}