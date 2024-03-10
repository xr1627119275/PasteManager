using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;

namespace Project2
{

    public partial class MainForm : Form
    {

        public WebView2 webView;
    
        public MainForm()
        {
            
            webView = new WebView2();
            webView.Dock = DockStyle.Fill;
            this.Name = "剪贴板管理";
            this.Text = "剪贴板管理";
            webView.Source = new Uri("https://test/dist/index.html");

            //string currentPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            webView.CoreWebView2InitializationCompleted += (sender, args) =>
            {
                webView.CoreWebView2.AddHostObjectToScript("bridge", new Bridge());
                // //本地html文件，可以把css和js文件导入
                webView.CoreWebView2.SetVirtualHostNameToFolderMapping("test",
                    "", CoreWebView2HostResourceAccessKind.DenyCors);

                //string filepath = currentPath + @"\dist\index.html";
                //webView.Source = new Uri("https://test/dist/index.html");
            };
            this.Controls.Add(webView);
            // 最小化
            this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;
            this.SizeChanged += new System.EventHandler(this.MainForm_OnSizeChanged);
            FormClosing += (sender, e) =>
            {
                e.Cancel = true;
                WindowState = FormWindowState.Minimized;
                this.Visible = false;
            };
            //Form childForm = new Form{ Name = "窗口 " };
            //childForm.Text = childForm.Name;
            //childForm.Show();
        }
        
        // protected override void WndProc(ref Message m)
        // {
        //     const int WM_SYSCOMMAND = 0x0112;
        //     const int SC_CLOSE = 0xF060;
        //     if (m.Msg == WM_SYSCOMMAND && (int)m.WParam == SC_CLOSE)
        //     {
        //         return;
        //     }
        //     base.WndProc(ref m);
        // }

     
        private static byte[] ImageToByte(Image Picture)
        {
            MemoryStream ms = new MemoryStream();
            if (Picture == null)
                return new byte[ms.Length];
            Picture.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            byte[] BPicture = new byte[ms.Length];
            BPicture = ms.GetBuffer();
            return BPicture;
        }



        private void MainForm_Load(object sender, EventArgs e)
        {
        }

        private void MainForm_OnSizeChanged(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                this.Visible = false;
            }
            else
            {
                this.Visible = true;
            }

            Console.WriteLine(WindowState);
        }




        internal static class NativeMethods
        {
            //Reference https://docs.microsoft.com/en-us/windows/desktop/dataxchg/wm-clipboardupdate
            public const int WM_CLIPBOARDUPDATE = 0x031D;

            //Reference https://www.pinvoke.net/default.aspx/Constants.HWND
            public static IntPtr HWND_MESSAGE = new IntPtr(-3);

            //Reference https://www.pinvoke.net/default.aspx/user32/AddClipboardFormatListener.html
            [DllImport("user32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool AddClipboardFormatListener(IntPtr hwnd);

            //Reference https://www.pinvoke.net/default.aspx/user32.setparent
            [DllImport("user32.dll", SetLastError = true)]
            public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.SuspendLayout();
            // 
            // MainForm
            // 
            this.ClientSize = new System.Drawing.Size(282, 253);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.Load += new System.EventHandler(this.MainForm_Load_1);
            this.ResumeLayout(false);

        }

        private void MainForm_Load_1(object sender, EventArgs e)
        {

        }
    }
}