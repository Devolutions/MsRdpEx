using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;

using MSTSCLib;
using AxMSTSCLib;

using MsRdpEx;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace MsRdpEx_App
{
    public partial class RdpView : Form
    {
        public string axName = "mstsc";
        public string rdpExDll = null;

        private string outputPath;
        private int captureIndex = 0;
        private string captureOutputPath;
        private bool enableCapture = true;
        private Timer captureTimer = null;

        private bool closeOnDisconnect = true;

        private int disconnectReason = 0;

        public int DisconnectReason { get => disconnectReason; }

        private DvcDialog dvcDialog = null;

        public RdpView(string axName, string rdpExDll)
        {
            this.axName = axName;
            this.rdpExDll = rdpExDll;

            outputPath = Environment.ExpandEnvironmentVariables("%LocalAppData%\\MsRdpEx");
            Directory.CreateDirectory(outputPath);

            captureOutputPath = Path.Combine(outputPath, "capture");
            Directory.CreateDirectory(captureOutputPath);

            InitializeComponent();

            if (enableCapture)
            {
                this.captureTimer = new Timer();
                captureTimer.Interval = (1000); // 1 second
                captureTimer.Tick += new EventHandler(CaptureTimer_Tick);
                captureTimer.Start();
            }
        }

        private const int WM_SYSCOMMAND = 0x112;
        private const int MF_STRING = 0x00000000;
        private const int MF_SEPARATOR = 0x00000800;
        private const int MF_CHECKED = 0x00000008;
        private const int MF_UNCHECKED = 0x00000000;
        private const int MF_ENABLED = 0x00000000;
        private const int MF_DISABLED = 0x00000002;

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool AppendMenu(IntPtr hMenu, int uFlags, int uIDNewItem, string lpNewItem);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool InsertMenu(IntPtr hMenu, int uPosition, int uFlags, int uIDNewItem, string lpNewItem);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool CheckMenuItem(IntPtr hMenu, int uIDCheckItem, int uFlags);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool EnableMenuItem(IntPtr hMenu, int uIDEnableItem, int uFlags);

        private const int SYSMENU_RDP_CONNECT_ID = 0x1;
        private const int SYSMENU_RDP_DISCONNECT_ID = 0x2;
        private const int SYSMENU_RDP_CLOSE_ON_DISCONNECT_ID = 0x3;
        private const int SYSMENU_RDP_SEND_CTRL_ALT_DEL_ID = 0x4;
        private const int SYSMENU_RDP_SEND_CTRL_ALT_END_ID = 0x5;

        private IntPtr hSysMenu = IntPtr.Zero;

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            this.hSysMenu = GetSystemMenu(this.Handle, false);
            AppendMenu(this.hSysMenu, MF_SEPARATOR, 0, string.Empty);
            AppendMenu(this.hSysMenu, MF_STRING, SYSMENU_RDP_CONNECT_ID, "Connect");
            AppendMenu(this.hSysMenu, MF_STRING, SYSMENU_RDP_DISCONNECT_ID, "Disconnect");
            AppendMenu(this.hSysMenu, MF_STRING | MF_CHECKED, SYSMENU_RDP_CLOSE_ON_DISCONNECT_ID, "Close on Disconnect");
            AppendMenu(this.hSysMenu, MF_SEPARATOR, 0, string.Empty);
            AppendMenu(this.hSysMenu, MF_STRING, SYSMENU_RDP_SEND_CTRL_ALT_DEL_ID, "Send Ctrl+Alt+Del");
            AppendMenu(this.hSysMenu, MF_STRING, SYSMENU_RDP_SEND_CTRL_ALT_END_ID, "Send Ctrl+Alt+End");
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == WM_SYSCOMMAND)
            {
                switch ((int)m.WParam)
                {
                    case SYSMENU_RDP_CONNECT_ID:
                        this.rdpClient.Connect();
                        EnableMenuItem(this.hSysMenu, SYSMENU_RDP_CONNECT_ID, MF_DISABLED);
                        EnableMenuItem(this.hSysMenu, SYSMENU_RDP_DISCONNECT_ID, MF_ENABLED);
                        break;

                    case SYSMENU_RDP_DISCONNECT_ID:
                        this.rdpClient.Disconnect();
                        EnableMenuItem(this.hSysMenu, SYSMENU_RDP_CONNECT_ID, MF_ENABLED);
                        EnableMenuItem(this.hSysMenu, SYSMENU_RDP_DISCONNECT_ID, MF_DISABLED);
                        break;

                    case SYSMENU_RDP_CLOSE_ON_DISCONNECT_ID:
                        this.closeOnDisconnect = this.closeOnDisconnect ? false : true;
                        CheckMenuItem(this.hSysMenu, SYSMENU_RDP_CLOSE_ON_DISCONNECT_ID, this.closeOnDisconnect ? MF_CHECKED : MF_UNCHECKED);
                        break;

                    case SYSMENU_RDP_SEND_CTRL_ALT_DEL_ID:
                        {
                            IntPtr hIHWindowClassWnd = GetInputHandlerHwnd();
                            SendCtrlAltDel(hIHWindowClassWnd);
                        }
                        break;

                    case SYSMENU_RDP_SEND_CTRL_ALT_END_ID:
                        {
                            IntPtr hIHWindowClassWnd = GetInputHandlerHwnd();
                            SendCtrlAltEnd(hIHWindowClassWnd);
                        }
                        break;
                }
            }
        }

        private enum TernaryRasterOperations : uint
        {
            SRCCOPY = 0x00CC0020,
        }

        [DllImport("gdi32.dll")]
        private static extern bool StretchBlt(
            IntPtr dstContext,
            int dstX,
            int dstY,
            int width,
            int height,
            IntPtr srcContext,
            int srcX,
            int srcY,
            int srcWidth,
            int srcHeight,
            TernaryRasterOperations rop);

        [DllImport("gdi32.dll")]
        public static extern int SetStretchBltMode(IntPtr hDC, int stretchMode);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, IntPtr lpszWindow);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        public IntPtr GetOutputPresenterHwnd()
        {
            IntPtr clientHandle = this.rdpClient.Handle;

            // UIMainClass ""
            // UIContainerClass ""
            // OPContainerClass "Output Painter Window"
            // OPWindowClass "Output Painter Child Window"

            IntPtr hUIMainClassWnd = FindWindowEx(clientHandle, IntPtr.Zero, "UIMainClass", IntPtr.Zero);
            IntPtr hUIContainerClassWnd = FindWindowEx(hUIMainClassWnd, IntPtr.Zero, "UIContainerClass", IntPtr.Zero);
            IntPtr hOPContainerClassWnd = FindWindowEx(hUIContainerClassWnd, IntPtr.Zero, "OPContainerClass", IntPtr.Zero);
            IntPtr hOPWindowClassWnd = FindWindowEx(hOPContainerClassWnd, IntPtr.Zero, "OPWindowClass", IntPtr.Zero);

            if (hOPWindowClassWnd == IntPtr.Zero)
                hOPWindowClassWnd = FindWindowEx(hOPContainerClassWnd, IntPtr.Zero, "OPWindowClass_mstscax", IntPtr.Zero);

            if (hOPWindowClassWnd == IntPtr.Zero)
                hOPWindowClassWnd = FindWindowEx(hOPContainerClassWnd, IntPtr.Zero, "OPWindowClass_rdclientax", IntPtr.Zero);

            //Debug.WriteLine("UIMainClass: {0}", hUIMainClassWnd);
            //Debug.WriteLine("OPContainerClass: {0}", hOPContainerClassWnd);
            //Debug.WriteLine("UIContainerClass: {0}", hUIContainerClassWnd);
            //Debug.WriteLine("OPWindowClass: {0}", hOPWindowClassWnd);

            return hOPWindowClassWnd;
        }

        public IntPtr GetInputHandlerHwnd()
        {
            IntPtr clientHandle = this.rdpClient.Handle;

            // UIMainClass ""
            // UIContainerClass ""
            // IHWindowClass "Input Capture Window"

            IntPtr hUIMainClassWnd = FindWindowEx(clientHandle, IntPtr.Zero, "UIMainClass", IntPtr.Zero);
            IntPtr hUIContainerClassWnd = FindWindowEx(hUIMainClassWnd, IntPtr.Zero, "UIContainerClass", IntPtr.Zero);
            IntPtr hIHWindowClassWnd = FindWindowEx(hUIContainerClassWnd, IntPtr.Zero, "IHWindowClass", IntPtr.Zero);

            if (hIHWindowClassWnd == IntPtr.Zero)
                hIHWindowClassWnd = FindWindowEx(hUIContainerClassWnd, IntPtr.Zero, "IHWindowClass_mstscax", IntPtr.Zero);

            if (hIHWindowClassWnd == IntPtr.Zero)
                hIHWindowClassWnd = FindWindowEx(hUIContainerClassWnd, IntPtr.Zero, "IHWindowClass_rdclientax", IntPtr.Zero);

            Debug.WriteLine("UIMainClass: {0}", hUIMainClassWnd);
            Debug.WriteLine("UIContainerClass: {0}", hUIContainerClassWnd);
            Debug.WriteLine("IHWindowClass: {0}", hIHWindowClassWnd);

            return hIHWindowClassWnd;
        }

        const int WM_KEYDOWN = 0x0100;
        const int WM_KEYUP = 0x0101;
        const uint VK_CONTROL = 0x11;
        const uint VK_MENU = 0x12;
        const uint VK_DELETE = 0x2E;
        const uint VK_END = 0x23;
        const uint EXTENDED_KEY_FLAG = 0x01000000;

        public static void SendCtrlAltDel(IntPtr hWnd)
        {
            SendMessage(hWnd, WM_KEYDOWN, (IntPtr)VK_CONTROL, IntPtr.Zero);
            SendMessage(hWnd, WM_KEYDOWN, (IntPtr)VK_MENU, IntPtr.Zero);
            SendMessage(hWnd, WM_KEYDOWN, (IntPtr)VK_DELETE, IntPtr.Zero);
            SendMessage(hWnd, WM_KEYUP, (IntPtr)VK_DELETE, IntPtr.Zero);
            SendMessage(hWnd, WM_KEYUP, (IntPtr)VK_MENU, IntPtr.Zero);
            SendMessage(hWnd, WM_KEYUP, (IntPtr)VK_CONTROL, IntPtr.Zero);
        }

        public static void SendCtrlAltEnd(IntPtr hWnd)
        {
            SendMessage(hWnd, WM_KEYDOWN, (IntPtr)VK_CONTROL, IntPtr.Zero);
            SendMessage(hWnd, WM_KEYDOWN, (IntPtr)VK_MENU, IntPtr.Zero);
            SendMessage(hWnd, WM_KEYDOWN, (IntPtr)VK_END, (IntPtr)EXTENDED_KEY_FLAG);
            SendMessage(hWnd, WM_KEYUP, (IntPtr)VK_END, (IntPtr)EXTENDED_KEY_FLAG);
            SendMessage(hWnd, WM_KEYUP, (IntPtr)VK_MENU, IntPtr.Zero);
            SendMessage(hWnd, WM_KEYUP, (IntPtr)VK_CONTROL, IntPtr.Zero);
        }

        private Bitmap ShadowToBitmap(IntPtr hWnd, IntPtr hShadowDC, IntPtr hShadowBitmap, int shadowWidth, int shadowHeight, int captureWidth, int captureHeight)
        {
            Graphics g = Graphics.FromHwnd(hWnd);

            if (shadowWidth == 0 || shadowHeight == 0)
            {
                return null;
            }

            int width = captureWidth;
            int height = captureHeight;

            int srcX = 0;
            int srcY = 0;
            int dstX = 0;
            int dstY = 0;
            float percent;
            int srcWidth = shadowWidth;
            int srcHeight = shadowHeight;

            float percentW = width / (float)srcWidth;
            float percentH = height / (float)srcHeight;
            if (percentH < percentW)
            {
                percent = percentH;
                dstX = Convert.ToInt16((width - (srcWidth * percent)) / 2);
            }
            else
            {
                percent = percentW;
                dstY = Convert.ToInt16((height - (srcHeight * percent)) / 2);
            }

            int dstWidth = (int)(srcWidth * percent);
            int dstHeight = (int)(srcHeight * percent);

            Bitmap bmp = new Bitmap(width, height, g);
            Graphics memoryGraphics = Graphics.FromImage(bmp);
            IntPtr dc = memoryGraphics.GetHdc();

            SetStretchBltMode(dc, 4);

            if (!StretchBlt(dc, dstX, dstY, dstWidth, dstHeight,
                hShadowDC, srcX, srcY, srcWidth, srcHeight, TernaryRasterOperations.SRCCOPY))
            {
                bmp = null;
            }

            memoryGraphics.ReleaseHdc(dc);
            memoryGraphics.Dispose();
            g.Dispose();

            return bmp;
        }

        private void CaptureTimer_Tick(object sender, EventArgs e)
        {
            IntPtr hWnd = GetOutputPresenterHwnd();

            IMsRdpExCoreApi coreApi = Bindings.GetCoreApi();

            object instance = null;
            bool success = coreApi.OpenInstanceForWindowHandle(hWnd, out instance);
            IMsRdpExInstance rdpInstance = (IMsRdpExInstance) instance;
            rdpInstance.SetOutputMirrorEnabled(true);

            IntPtr hShadowDC = IntPtr.Zero;
            IntPtr hShadowBitmap = IntPtr.Zero;
            IntPtr shadowData = IntPtr.Zero;
            UInt32 shadowWidth = 0;
            UInt32 shadowHeight = 0;
            UInt32 shadowStep = 0;

            int captureWidth = 1024;
            int captureHeight = 768;

            if (rdpInstance.GetShadowBitmap(ref hShadowDC, ref hShadowBitmap, ref shadowData, ref shadowWidth, ref shadowHeight, ref shadowStep))
            {
                rdpInstance.LockShadowBitmap();
                Bitmap bitmap = ShadowToBitmap(hWnd, hShadowDC, hShadowBitmap, (int)shadowWidth, (int)shadowHeight, captureWidth, captureHeight);
                rdpInstance.UnlockShadowBitmap();

                if (bitmap != null)
                {
                    string bitmapName = String.Format("capture_{0:0000}.bmp", captureIndex++);
                    string filename = Path.Combine(captureOutputPath, bitmapName);
                    bitmap.Save(filename);
                }
            }
        }

        protected void OnConnected(object sender, EventArgs e)
        {
            Debug.WriteLine("RdpOnConnected");
        }

        protected void OnConnecting(object sender, EventArgs e)
        {
            Debug.WriteLine("RdpOnConnecting");
        }

        protected void OnDisconnected(object sender, IMsTscAxEvents_OnDisconnectedEvent e)
        {
            this.disconnectReason = e.discReason;
            Debug.WriteLine("RdpOnDisconnected: reason: {0}", e.discReason);

            if (this.disconnectReason != 0)
            {
                string text = String.Format("Connection Failed: discReason: {0}", this.DisconnectReason);
                MessageBox.Show(text);
            }

            if (this.closeOnDisconnect)
            {
                this.Close();
            }
        }

        protected void OnEnterFullScreenMode(object sender, EventArgs e)
        {

        }

        protected void OnLeaveFullScreenMode(object sender, EventArgs e)
        {

        }

        protected void OnConnectionBarPullDown(object sender, EventArgs e)
        {

        }

        protected void OnConfirmClose(object sender, IMsTscAxEvents_OnConfirmCloseEvent e)
        {
            e.pfAllowClose = true;
        }

        protected void OnFormClosing(object sender, EventArgs e)
        {
            if (captureTimer != null)
            {
                captureTimer.Stop();
            }
        }

        private System.ComponentModel.IContainer components = null;
        public AxMSTSCLib.AxMsRdpClient9NotSafeForScripting rdpClient;

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RdpView));
            this.rdpClient = new AxMSTSCLib.AxMsRdpClient9NotSafeForScripting();
            this.rdpClient.axName = this.axName;
            this.rdpClient.rdpExDll = this.rdpExDll;
            this.rdpClient.OnConnected += OnConnected;
            this.rdpClient.OnConnecting += OnConnecting;
            this.rdpClient.OnDisconnected += OnDisconnected;
            this.rdpClient.OnEnterFullScreenMode += OnEnterFullScreenMode;
            this.rdpClient.OnLeaveFullScreenMode += OnLeaveFullScreenMode;
            this.rdpClient.OnConnectionBarPullDown += OnConnectionBarPullDown;
            this.rdpClient.OnConfirmClose += OnConfirmClose;
            ((System.ComponentModel.ISupportInitialize)(this.rdpClient)).BeginInit();
            this.SuspendLayout();
            // 
            // rdpClient
            // 
            this.rdpClient.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rdpClient.Enabled = true;
            this.rdpClient.Location = new System.Drawing.Point(0, 0);
            this.rdpClient.Name = "rdpClient";
            this.rdpClient.OcxState = null;
            this.rdpClient.Size = new System.Drawing.Size(1107, 503);
            this.rdpClient.TabIndex = 0;
            // 
            // RdpView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1107, 503);
            this.Controls.Add(this.rdpClient);
            this.Name = "RdpView";
            this.Text = "Remote Desktop Client";
            ((System.ComponentModel.ISupportInitialize)(this.rdpClient)).EndInit();
            this.ResumeLayout(false);

            this.FormClosing += OnFormClosing;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        public DvcDialog StartDvcDialog()
        {
            dvcDialog = new DvcDialog();
            dvcDialog.Show(this);

            return dvcDialog;
        }
    }
}
