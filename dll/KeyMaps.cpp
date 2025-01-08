#include <MsRdpEx/KeyMaps.h>

typedef struct {
    const char* keyName;
    uint32_t vkCode;
} KeyCodeMapping;

#define VK_UNKNOWN  0

#define VK_KEY_0 0x30 /* '0' key */
#define VK_KEY_1 0x31 /* '1' key */
#define VK_KEY_2 0x32 /* '2' key */
#define VK_KEY_3 0x33 /* '3' key */
#define VK_KEY_4 0x34 /* '4' key */
#define VK_KEY_5 0x35 /* '5' key */
#define VK_KEY_6 0x36 /* '6' key */
#define VK_KEY_7 0x37 /* '7' key */
#define VK_KEY_8 0x38 /* '8' key */
#define VK_KEY_9 0x39 /* '9' key */

#define VK_KEY_A 0x41 /* 'A' key */
#define VK_KEY_B 0x42 /* 'B' key */
#define VK_KEY_C 0x43 /* 'C' key */
#define VK_KEY_D 0x44 /* 'D' key */
#define VK_KEY_E 0x45 /* 'E' key */
#define VK_KEY_F 0x46 /* 'F' key */
#define VK_KEY_G 0x47 /* 'G' key */
#define VK_KEY_H 0x48 /* 'H' key */
#define VK_KEY_I 0x49 /* 'I' key */
#define VK_KEY_J 0x4A /* 'J' key */
#define VK_KEY_K 0x4B /* 'K' key */
#define VK_KEY_L 0x4C /* 'L' key */
#define VK_KEY_M 0x4D /* 'M' key */
#define VK_KEY_N 0x4E /* 'N' key */
#define VK_KEY_O 0x4F /* 'O' key */
#define VK_KEY_P 0x50 /* 'P' key */
#define VK_KEY_Q 0x51 /* 'Q' key */
#define VK_KEY_R 0x52 /* 'R' key */
#define VK_KEY_S 0x53 /* 'S' key */
#define VK_KEY_T 0x54 /* 'T' key */
#define VK_KEY_U 0x55 /* 'U' key */
#define VK_KEY_V 0x56 /* 'V' key */
#define VK_KEY_W 0x57 /* 'W' key */
#define VK_KEY_X 0x58 /* 'X' key */
#define VK_KEY_Y 0x59 /* 'Y' key */
#define VK_KEY_Z 0x5A /* 'Z' key */

#define VK_ABNT_C1 0xC1 /* Brazilian (ABNT) Keyboard */
#define VK_ABNT_C2 0xC2 /* Brazilian (ABNT) Keyboard */

// https://github.com/microsoft/vscode/blob/main/src/vs/base/common/keyCodes.ts

static const KeyCodeMapping keyCodeTable[] =
{
    { "None", VK_UNKNOWN },
    { "Hyper", VK_UNKNOWN },
    { "Super", VK_UNKNOWN },
    { "Fn", VK_UNKNOWN },
    { "FnLock", VK_UNKNOWN },
    { "Suspend", VK_UNKNOWN },
    { "Resume", VK_UNKNOWN },
    { "Turbo", VK_UNKNOWN },
    { "Sleep", VK_SLEEP },
    { "WakeUp", VK_UNKNOWN },

    { "Ctrl", VK_CONTROL },
    { "Shift", VK_SHIFT },
    { "Alt", VK_MENU },

    { "A", VK_KEY_A },
    { "B", VK_KEY_B },
    { "C", VK_KEY_C },
    { "D", VK_KEY_D },
    { "E", VK_KEY_E },
    { "F", VK_KEY_F },
    { "G", VK_KEY_G },
    { "H", VK_KEY_H },
    { "I", VK_KEY_I },
    { "J", VK_KEY_J },
    { "K", VK_KEY_K },
    { "L", VK_KEY_L },
    { "M", VK_KEY_M },
    { "N", VK_KEY_N },
    { "O", VK_KEY_O },
    { "P", VK_KEY_P },
    { "Q", VK_KEY_Q },
    { "R", VK_KEY_R },
    { "S", VK_KEY_S },
    { "T", VK_KEY_T },
    { "U", VK_KEY_U },
    { "V", VK_KEY_V },
    { "W", VK_KEY_W },
    { "X", VK_KEY_X },
    { "Y", VK_KEY_Y },
    { "Z", VK_KEY_Z },

    { "0", VK_KEY_0 },
    { "1", VK_KEY_1 },
    { "2", VK_KEY_2 },
    { "3", VK_KEY_3 },
    { "4", VK_KEY_4 },
    { "5", VK_KEY_5 },
    { "6", VK_KEY_6 },
    { "7", VK_KEY_7 },
    { "8", VK_KEY_8 },
    { "9", VK_KEY_9 },

    { "Enter", VK_RETURN },
    { "Escape", VK_ESCAPE },
    { "Backspace", VK_BACK },
    { "Tab", VK_TAB },
    { "Space", VK_SPACE },
    { "-", VK_OEM_MINUS },
    { "=", VK_OEM_PLUS },
    { "[", VK_OEM_4 },
    { "]", VK_OEM_6 },
    { "\\", VK_OEM_5 },
    { ";", VK_OEM_1 },
    { "\'", VK_OEM_7 },
    { "`", VK_OEM_3 },
    { ",", VK_OEM_COMMA },
    { ".", VK_OEM_PERIOD },
    { "/", VK_OEM_2 },
    { "CapsLock", VK_CAPITAL },

    { "F1", VK_F1 },
    { "F2", VK_F2 },
    { "F3", VK_F3 },
    { "F4", VK_F4 },
    { "F5", VK_F5 },
    { "F6", VK_F6 },
    { "F7", VK_F7 },
    { "F8", VK_F8 },
    { "F9", VK_F9 },
    { "F10", VK_F10 },
    { "F11", VK_F11 },
    { "F12", VK_F12 },

    { "PrintScreen", VK_UNKNOWN },
    { "ScrollLock", VK_SCROLL },
    { "PauseBreak", VK_PAUSE },
    { "Insert", VK_INSERT },
    { "Home", VK_HOME },
    { "PageUp", VK_PRIOR },
    { "Delete", VK_DELETE },
    { "End", VK_END },
    { "PageDown", VK_NEXT },
    { "RightArrow", VK_RIGHT },
    { "LeftArrow", VK_LEFT },
    { "DownArrow", VK_DOWN },
    { "UpArrow", VK_UP },

    { "NumLock", VK_NUMLOCK },
    { "NumPad_Divide", VK_DIVIDE },
    { "NumPad_Subtract", VK_SUBTRACT },
    { "NumPad_Add", VK_ADD },
    { "NumPad_Enter", VK_UNKNOWN },

    { "Numpad0", VK_NUMPAD0 },
    { "Numpad1", VK_NUMPAD1 },
    { "Numpad2", VK_NUMPAD2 },
    { "Numpad3", VK_NUMPAD3 },
    { "Numpad4", VK_NUMPAD4 },
    { "Numpad5", VK_NUMPAD5 },
    { "Numpad6", VK_NUMPAD6 },
    { "Numpad7", VK_NUMPAD7 },
    { "Numpad8", VK_NUMPAD8 },
    { "Numpad9", VK_NUMPAD9 },

    { "NumPad_Decimal", VK_DECIMAL },
    { "Numpad_Equal", VK_UNKNOWN },
    { "Numpad_Separator", VK_SEPARATOR },
    { "Numpad_Clear", VK_CLEAR },
};

uint32_t MsRdpEx_KeyNameToVKCode(const char* keyName)
{
    int numMappings = sizeof(keyCodeTable) / sizeof(keyCodeTable[0]);
    
    for (int i = 0; i < numMappings; i++) {
        if (_stricmp(keyCodeTable[i].keyName, keyName) == 0) {
            return keyCodeTable[i].vkCode;
        }
    }
    
    return VK_UNKNOWN;
}
