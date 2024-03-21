using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace Project2
{

   
    public partial class Form2 : Form
    {
        private KeyEventHandler myKeyEventHandeler = null;//按键钩子
        private KeyboardHook k_hook = new KeyboardHook();
        public static MainForm mainForm;
        public static bool startPaste = false;

        public Form2()
        {
            InitializeComponent();
            this.Icon = null;
            Version version = Assembly.GetExecutingAssembly().GetName().Version;
            Console.WriteLine(version);
            //Control.CheckForIllegalCrossThreadCalls = false;
            //Turn the child window into a message-only window (refer to Microsoft docs)
            NativeMethods.SetParent(Handle, NativeMethods.HWND_MESSAGE);
            //Place window in the system-maintained clipboard format listener list
            NativeMethods.AddClipboardFormatListener(Handle);
            startListen();
            mainForm = new MainForm();
            mainForm.SetBounds(0, 0, 400, 800);
            mainForm.MaximumSize = new Size(Screen.GetBounds(this).Width, 800);
            mainForm.MinimumSize = new Size(400, 800);
            mainForm.TopMost = true;
        }
        private void hook_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.V && Control.ModifierKeys == 0) return;
            if (e.KeyCode == Keys.V && (Control.ModifierKeys & Keys.Shift) == Keys.Shift && (Control.ModifierKeys & Keys.Alt) == Keys.Alt)
            {
                mainForm.webView.ExecuteScriptAsync("window.getData()");
                mainForm.WindowState = FormWindowState.Minimized;
                mainForm.SetDesktopLocation(Cursor.Position.X + 10, Cursor.Position.Y + 10);
                mainForm.Visible = true;
                mainForm.WindowState = FormWindowState.Normal;
            }
            if (e.KeyCode == Keys.C && Control.ModifierKeys == Keys.Control)
            {
                //  这里写具体实现
                // MessageBox.Show("ALT V");
                // Clipboard.SetFileDropList(files);
                Console.WriteLine(
                        Clipboard.GetText(TextDataFormat.Text));
            }
            //  这里写具体实现
            // MessageBox.Show("按下按键" + e.KeyValue + "和控制键"+(int)Control.ModifierKeys);
        }
        public void startListen()
        {
            myKeyEventHandeler = new KeyEventHandler(hook_KeyDown);
            k_hook.KeyDownEvent += myKeyEventHandeler;//钩住键按下
            k_hook.Start();//安装键盘钩子
        }
        public void stopListen()
        {
            if (myKeyEventHandeler != null)
            {
                k_hook.KeyDownEvent -= myKeyEventHandeler;//取消按键事件
                myKeyEventHandeler = null;
                k_hook.Stop();//关闭键盘钩子
            }
        }

        /// <summary>
        /// 显示剪贴板内容
        /// </summary>
        public void DisplayClipboardData()
        {
            IDataObject iData = Clipboard.GetDataObject();
            var last = LiteSqlManage.instance.getLastOne();
           
                if (iData.GetDataPresent(DataFormats.Text))
                {
                    var text = (string)iData.GetData(DataFormats.Text);
                    if (last.Title.Equals(text)) return;
                    Console.WriteLine("text");
                    if (text != null && text.Trim().Length > 0)
                    {
                        LiteSqlManage.instance.addData(new PasteInfo() { Title = text, Content = text, Type = DataFormats.Text });
                    }
                }
                else if (Clipboard.ContainsImage())
                {
                    Image img = Clipboard.GetImage();
                    Console.WriteLine("img");
                    Console.WriteLine(img);
                    if (img != null)
                    {
                        var currImage = ImageToByte(img);
                        if (last.Image != null && currImage.Length == last.Image.Length) return;
                        LiteSqlManage.instance.addData(new PasteInfo()
                        { Title = "图片", Image = currImage, Type = DataFormats.Bitmap });
                    }

                }
                else if (Clipboard.ContainsFileDropList())
                {
                    var fileList = Clipboard.GetFileDropList();
                    if (fileList == null || fileList.Count == 0) return;
                    var files = new string[fileList.Count];
                    fileList.CopyTo(files, 0);
                    Console.WriteLine("fileList");
                    var currFiles = String.Join("#", files);
                    if (currFiles.Length == 0)
                    {
                        return;
                    }
                    if (last.Content.Equals(currFiles)) return;
                    LiteSqlManage.instance.addData(new PasteInfo()
                    { Title = "文件", Content = currFiles, Type = DataFormats.FileDrop });
                }

        }
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
        protected override void WndProc(ref Message m)
        {
            //Listen for operating system messages
            if (m.Msg == NativeMethods.WM_CLIPBOARDUPDATE)
            {
                Console.WriteLine("CLIPBOARD_CHANGE");
                
                if (!NativeMethods.lockPaste) DisplayClipboardData();
                mainForm.webView.ExecuteScriptAsync("window.getData()");
                  
            }
            //Called for any unhandled messages
            base.WndProc(ref m);
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }


        private void 显示ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // SystemInformation.WorkingArea
            int SH = Screen.PrimaryScreen.WorkingArea.Height;
            int SW = Screen.PrimaryScreen.WorkingArea.Width;
            //获取当前活动窗口高度跟宽度
            int self_SH = this.Size.Height;
            int self_SW = this.Size.Width;
            //设置窗口打开的初始位置为下方居中
            mainForm.SetDesktopLocation((SW - self_SW) / 2, (SH - self_SH) / 2);
            mainForm.Visible = false;
            mainForm.WindowState = FormWindowState.Normal;
        }

        private void 隐藏ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mainForm.WindowState = FormWindowState.Minimized;
        }

        private void 关闭ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Dispose();
            this.Close();
        }

        private void 显示ToolStripMenuItem_Click_1(object sender, EventArgs e)
        {

        }
    }
    internal static class NativeMethods
    {
        public static bool lockPaste = false;
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

}
