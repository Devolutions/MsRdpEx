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

using MsRdpEx;

namespace MsRdpEx_App
{
    public partial class RdpView : Form
    {
        public string axName = "latest";

        private string outputPath;
        private int captureIndex = 0;
        private string captureOutputPath;

        public RdpView(string axName)
        {
            this.axName = axName;

            outputPath = Environment.ExpandEnvironmentVariables("%LocalAppData%\\MsRdpEx");
            Directory.CreateDirectory(outputPath);

            captureOutputPath = Path.Combine(outputPath, "capture");
            Directory.CreateDirectory(captureOutputPath);

            InitializeComponent();

            if (false)
            {
                Timer CaptureTimer = new Timer();
                CaptureTimer.Interval = (1000); // 1 second
                CaptureTimer.Tick += new EventHandler(CaptureTimer_Tick);
                CaptureTimer.Start();
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

            //Debug.WriteLine("UIMainClass: {0}", hUIMainClassWnd);
            //Debug.WriteLine("OPContainerClass: {0}", hOPContainerClassWnd);
            //Debug.WriteLine("UIContainerClass: {0}", hUIContainerClassWnd);
            //Debug.WriteLine("OPWindowClass: {0}", hOPWindowClassWnd);

            return hOPWindowClassWnd;
        }

        private Bitmap ShadowToBitmap(IntPtr hWnd, IntPtr hShadowDC, IntPtr hShadowBitmap, int shadowWidth, int shadowHeight)
        {
            Graphics g = Graphics.FromHwnd(hWnd);

            if (shadowWidth == 0 || shadowHeight == 0)
            {
                return null;
            }

            int width = shadowWidth;
            int height = shadowHeight;

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
            StretchBlt(dc, dstX, dstY, dstWidth, dstHeight,
                hShadowDC, srcX, srcY, srcWidth, srcHeight, TernaryRasterOperations.SRCCOPY);

            memoryGraphics.ReleaseHdc(dc);
            memoryGraphics.Dispose();
            g.Dispose();

            return bmp;
        }

        private void CaptureTimer_Tick(object sender, EventArgs e)
        {
            IntPtr hWnd = GetOutputPresenterHwnd();

            IntPtr hShadowDC = IntPtr.Zero;
            IntPtr hShadowBitmap = IntPtr.Zero;
            UInt32 shadowWidth = 0;
            UInt32 shadowHeight = 0;

            if (Bindings.MsRdpEx_GetShadowBitmap(hWnd,
                ref hShadowDC, ref hShadowBitmap, ref shadowWidth, ref shadowHeight))
            {
                Bitmap bitmap = ShadowToBitmap(hWnd, hShadowDC, hShadowBitmap, (int)shadowWidth, (int)shadowHeight);
                string bitmapName = String.Format("capture_{0:0000}.bmp", captureIndex++);
                string filename = Path.Combine(captureOutputPath, bitmapName);
                bitmap.Save(filename);
            }
        }

        protected void OnConnected(object sender, EventArgs e)
        {
            IMsRdpExtendedSettings extendedSettings = (IMsRdpExtendedSettings)this.rdpClient.GetOcx();
            object propVal = extendedSettings.get_Property("CorePropertySet");
            IMsRdpExtendedSettings props = (IMsRdpExtendedSettings)propVal;

            object myTestProperty = true;
            props.set_Property("MyTestProperty", ref myTestProperty);
        }

        protected void OnConnecting(object sender, EventArgs e)
        {
            
        }

        private System.ComponentModel.IContainer components = null;
        public AxMSTSCLib.AxMsRdpClient9NotSafeForScripting rdpClient;

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RdpView));
            this.rdpClient = new AxMSTSCLib.AxMsRdpClient9NotSafeForScripting();
            this.rdpClient.axName = this.axName;
            this.rdpClient.OnConnected += OnConnected;
            this.rdpClient.OnConnecting += OnConnecting;
            ((System.ComponentModel.ISupportInitialize)(this.rdpClient)).BeginInit();
            this.SuspendLayout();
            // 
            // rdpClient
            // 
            this.rdpClient.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rdpClient.Enabled = true;
            this.rdpClient.Location = new System.Drawing.Point(0, 0);
            this.rdpClient.Name = "rdpClient";
            this.rdpClient.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("rdpClient.OcxState")));
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
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
