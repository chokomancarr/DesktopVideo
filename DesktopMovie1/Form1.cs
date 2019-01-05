﻿using System;
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

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool SystemParametersInfo(int uiAction, int uiParam, IntPtr pvParam, int fWinIni);

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

        bool isnico = false;

        HtmlElement nicoplay;
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
                //WindowState = FormWindowState.Maximized;
                SetWindowLong(Handle, -16, GetWindowLong(Handle, -16) | 0x40000000);
                SetParent(Handle, workerw);

                DesktopVideo editor = new DesktopVideo();
                editor.f1 = this;
                editor.Show();
                editor.ReadDefault();

                FormClosed += Form1_FormClosed;
            }
            else
            {
                Close();
            }
        }

        void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            SystemParametersInfo(20, 0, IntPtr.Zero, 2);
        }

        public void StartLocalVideo(string path)
        {
            Debug.WriteLine("Starting Local @ " + path);
            StopYTVideo();
            webBrowser.Visible = false;
            mediaPlayer.Visible = true;
            mediaPlayer.URL = path;
            mediaPlayer.Ctlcontrols.play();
            mediaPlayer.settings.setMode("loop", true);
            timer1.Enabled = true;
            timer1.Interval = 500;
        }

        public void StopLocalVideo()
        {
            if (mediaPlayer.playState == WMPLib.WMPPlayState.wmppsPlaying)
                mediaPlayer.Ctlcontrols.pause();
            timer1.Enabled = false;
        }

        public void StartYTVideo(string id)
        {
            Debug.WriteLine("Starting YT @ " + id);
            StopLocalVideo();
            webBrowser.Visible = true;
            webBrowser.DocumentCompleted += WebBrowser_DocumentCompleted;
            mediaPlayer.Visible = false;
            webBrowser.Navigate(@"https://www.youtube.com/embed/" + id + @"?controls=0&loop=1&autoplay=1&rel=0&playlist=" + id);
            isnico = false;
        }

        public void StartNCVideo(string id)
        {
            Debug.WriteLine("Starting NC @ " + id);
            StopLocalVideo();
            webBrowser.Visible = true;
            webBrowser.DocumentCompleted += WebBrowser_DocumentCompleted;
            mediaPlayer.Visible = false;
            webBrowser.Navigate(@"http://embed.nicovideo.jp/watch/" + id);
            isnico = true;
        }

        private void WebBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (isnico)
            {
                foreach (HtmlElement aa in webBrowser.Document.GetElementsByTagName("button"))
                {
                    if (aa.GetAttribute("className") == "f1l5qaxt")
                    {
                        nicoplay = aa;
                    }
                }
				timer1.Enabled = true;
				timer1.Interval = 500;
            }
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
            timer1.Enabled = false;
        }

        private void webBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e) {
            
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (isnico)
            {
                var tt = nicoplay.GetAttribute("data-title");
                if (tt != "Pause")
                    nicoplay.InvokeMember("click");
            }
            else
            {
                if (timer1.Interval == 500 && (mediaPlayer.currentMedia.duration - mediaPlayer.Ctlcontrols.currentPosition) < 1)
                {
                    timer1.Interval = 100;
                }
                else if ((mediaPlayer.currentMedia.duration - mediaPlayer.Ctlcontrols.currentPosition) < 0.1)
                {
                    mediaPlayer.Ctlcontrols.currentPosition = 0;
                    timer1.Interval = 500;
                }
            }
        }
    }
}
