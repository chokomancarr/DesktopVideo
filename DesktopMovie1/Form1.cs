using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace DesktopMovie1
{
    public partial class Form1 : Form
    {
        [DllImport("user32.dll")]
        static extern IntPtr GetActiveWindow();

        [DllImport("user32.dll")]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        public static extern int SendMessage(int hWnd, IntPtr msg, int wParam, int lParam);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindowEx(IntPtr parent, IntPtr childAfter, string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        private static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);

        [Flags]
        public enum SendMessageTimeoutFlags : uint
        {
            SMTO_NORMAL = 0x0,
            SMTO_BLOCK = 0x1,
            SMTO_ABORTIFHUNG = 0x2,
            SMTO_NOTIMEOUTIFNOTHUNG = 0x8,
            SMTO_ERRORONEXIT = 0x20
        }

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessageTimeout( IntPtr hWnd, uint Msg, UIntPtr wParam, IntPtr lParam, SendMessageTimeoutFlags fuFlags, uint uTimeout, out UIntPtr lpdwResult);

        // Delegate to filter which windows to include 
        public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            mediaPlayer.uiMode = "none";
            mediaPlayer.stretchToFit = true;

            IntPtr w = FindWindow("Progman", null);
            IntPtr workerw = IntPtr.Zero;

            UIntPtr result;

            SendMessageTimeout(w, (uint)0x052C, new UIntPtr(0), IntPtr.Zero, SendMessageTimeoutFlags.SMTO_NORMAL, 1000, out result);

            EnumWindows(new EnumWindowsProc((tophandle, topparamhandle) =>
            {
                IntPtr p = FindWindowEx(tophandle, IntPtr.Zero, "SHELLDLL_DefView", "");
                if (p != IntPtr.Zero)
                {
                    workerw = FindWindowEx(IntPtr.Zero, tophandle, "WorkerW", "");
                }
                return true;
            }), IntPtr.Zero);

            if (w != IntPtr.Zero)
            {
                WindowState = FormWindowState.Maximized;
                SetWindowLong(Handle, -16, GetWindowLong(Handle, -16) | 0x40000000);
                SetParent(Handle, workerw);

                DesktopVideo editor = new DesktopVideo();
                editor.f1 = this;
                editor.Show();
                
                //ryuu ga waga teki wo kurau
                //StartYTVideo("p1SJ5vBXLI8");
                //gab kawaii
                //StartYTVideo("doJSO9bQkng");
            }
            else
            {
                Close();
            }
        }

        public void StartLocalVideo(string path)
        {
            Debug.WriteLine("Starting Local @ " + path);
            StopYTVideo();
            webBrowser.Visible = false;
            mediaPlayer.Visible = true;
            mediaPlayer.URL = path;
            mediaPlayer.Ctlcontrols.play();
        }

        public void StopLocalVideo()
        {
            if (mediaPlayer.playState == WMPLib.WMPPlayState.wmppsPlaying)
                mediaPlayer.Ctlcontrols.pause();
        }

        public void StartYTVideo(string id)
        {
            Debug.WriteLine("Starting YT @ " + id);
            StopLocalVideo();
            webBrowser.Visible = true;
            mediaPlayer.Visible = false;
            webBrowser.Navigate(@"https://www.youtube.com/v/" + id + @"?%20controls=0%20&loop=1%20&autoplay=1&rel=0&playlist=" + id);
        }

        public void StopYTVideo()
        {
            webBrowser.Dispose();
            webBrowser = new WebBrowser();
            webBrowser.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            webBrowser.Dock = DockStyle.Fill;
            webBrowser.ScrollBarsEnabled = false;
            webBrowser.AllowNavigation = false;
            Controls.Add(webBrowser);
        }
    }
}
