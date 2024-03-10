using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace Project2
{
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [ComVisible(true)]
    public class BridgeAnotherClass
    {
        // Sample property.
        public string Prop { get; set; } = "Example";
    }

    [ClassInterface(ClassInterfaceType.AutoDual)]
    [ComVisible(true)]
    public class Bridge
    {
        public BridgeAnotherClass Func(string param)
        {
            return new BridgeAnotherClass { Prop = "Test"};
        }

        public String getList(int page = 0, int pageSize = 10)
        {
            return  JsonConvert.SerializeObject(LiteSqlManage.instance.getListByPages(page, pageSize));
        }

        public String handleSelect(int id)
        {
            var pasteInfos = LiteSqlManage.instance.getListByID(id);                        

            Console.WriteLine(pasteInfos[0]);
            if (pasteInfos.Count > 0)
            {
                var pasteInfo = pasteInfos[0];
                Clipboard.Clear();
                if (pasteInfo.Type == DataFormats.Text)
                {
                    Console.WriteLine("success");
                    Console.WriteLine(pasteInfo.Title);
                    
                    Clipboard.SetText(pasteInfo.Title);
                }
                if (pasteInfo.Type == DataFormats.Bitmap)
                {
                    var bytes = pasteInfo.Image;
                    // bytes to bitmap
                    var bitmap = new Bitmap(new System.IO.MemoryStream(bytes));
                    Clipboard.SetImage(bitmap);
                }
                
                if (pasteInfo.Type == DataFormats.FileDrop)
                {
                    var bytes = pasteInfo.Content;
                    var files = new StringCollection();
                    foreach (var s in pasteInfo.Content.Split('#'))
                    {
                        files.Add(s);
                    }
                    // bytes to bitmap
                    Clipboard.SetFileDropList(files);
                }
                Form2.mainForm.WindowState = FormWindowState.Minimized;
                Form2.mainForm.Visible = true;
                MouseHelper.mouse_event(MouseHelper.MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
                MouseHelper.mouse_event(MouseHelper.MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
                System.Threading.Thread.Sleep(100);
                SendKeys.Send("^{v}");
                Form2.startPaste = false;
            }
            return "success";
        }

        public String ShowToast(string message)
        {
            
            return "success";
        }

        public String handleCopy(int id)
        {
            var pasteInfos = LiteSqlManage.instance.getListByID(id);                        

            Console.WriteLine(pasteInfos[0]);
            if (pasteInfos.Count > 0)
            {
                var pasteInfo = pasteInfos[0];
                Clipboard.Clear();
                if (pasteInfo.Type == DataFormats.Text)
                {
                    NativeMethods.lockPaste = true;
                    System.Threading.Thread.Sleep(200);
                    Clipboard.SetText(pasteInfo.Title);
                    System.Threading.Thread.Sleep(1000);
                    NativeMethods.lockPaste = false;
                }
                
                if (pasteInfo.Type == DataFormats.Bitmap)
                {
                    var bytes = pasteInfo.Image;
                    // bytes to bitmap
                    var bitmap = new Bitmap(new System.IO.MemoryStream(bytes));
                    Clipboard.SetImage(bitmap);
                }
                
                if (pasteInfo.Type == DataFormats.FileDrop)
                {
                    var bytes = pasteInfo.Content;
                    var files = new StringCollection();
                    foreach (var s in pasteInfo.Content.Split('#'))
                    {
                        files.Add(s);
                    }
                    // bytes to bitmap
                    Clipboard.SetFileDropList(files);
                }
            }
            
            return "success";
        }

        public BridgeAnotherClass AnotherObject { get; set; } = new BridgeAnotherClass();

        // Sample indexed property.
        [System.Runtime.CompilerServices.IndexerName("Items")]
        public string this[int index]
        {
            get { return m_dictionary[index]; }
            set { m_dictionary[index] = value; }
        }
        private Dictionary<int, string> m_dictionary = new Dictionary<int, string>();
    }
    
}