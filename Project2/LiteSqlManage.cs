using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using SQLite;
using SQLitePCL;

namespace Project2
{
    public class PasteInfo
    {
        [PrimaryKey, AutoIncrement]
        [Column("id")]
        public int Id { get; set; }
        
        [Column("title")]
        public string Title { get; set; }
        [Column("content")]
        public string Content { get; set; }

        [Column("image")] public byte[] Image { get; set; } = null;
        [Column("type")]
        public string Type { get; set; }
        [Column("time")]
        public DateTime Time { get; set; }	
        [Column("time_str")]
        public string TimeStr { get; set; }	
    }

    
    public class LiteSqlManage
    {
        private SQLiteConnection db = null;
        public static LiteSqlManage instance = new LiteSqlManage();
        public LiteSqlManage()
        {
            if (instance == null)
            {
                string dbPath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "store.db");
                Console.WriteLine("dbPath: "+ dbPath);
                var options = new SQLiteConnectionString("./store.db", true);
                db = new SQLiteConnection(options);
                db.CreateTable<PasteInfo>();
            }
  
            // // 插入
            // var dtNow = DateTime.Now;
            // ;
            // var user = new PasteInfo() { Title = "标题",Content = "xr" ,Time = dtNow, Type = DataFormats.Text,TimeStr = dtNow.ToString("yyyy-MM-dd hh:mm:ss") };
            // db.Insert(user);
            // // 查询
            // var users = db.Table<PasteInfo>().Where(u => u.Content.StartsWith("x")).ToList();
            // Console.WriteLine(users[0].Time.Day);
        }
       
        public List<PasteInfo> getListByPages(int page, int pageSize = 10)
        {
            // 分页
            // return db.Table<PasteInfo>().Skip(pageSize * page).Take(pageSize).ToList();
            return db.Query<PasteInfo>("select * from PasteInfo ORDER BY id DESC Limit  ?,?;",
                new Object[] { pageSize * page, pageSize }).ToList();
        }

        public PasteInfo getLastOne()
        {
            var lists = db.Query<PasteInfo>("select * from PasteInfo ORDER BY id DESC Limit 1;").ToList();
            if (lists.Count > 0) return lists[0];
            return new PasteInfo { Title = "" };
        }
        public List<PasteInfo> getListByID(int id)
        {
            // 分页
            // return db.Table<PasteInfo>().Skip(pageSize * page).Take(pageSize).ToList();
            return db.Query<PasteInfo>("select * from PasteInfo where id=?", new object[]{ id }).ToList();
        }
        public List<PasteInfo> getListBySearch(string title, int page = 0, int pageSize = 10)
        {
            // 分页
            return db.Query<PasteInfo>("select * from PasteInfo where title like  ?  ORDER BY id DESC   limit ? offset ? ", new Object[] {"%" + title + "%", pageSize, page * pageSize}).ToList();
        }

        public void addData(PasteInfo pasteInfo)
        {
            pasteInfo.Time = DateTime.Now;
            pasteInfo.TimeStr = pasteInfo.Time.ToString("yyyy-MM-dd HH:mm:ss");
            db.Insert(pasteInfo);
        }
        
        // public static void Main(string []args)
        // {
        //     var manage = new LiteSqlManage();
        //     Console.WriteLine(
        //             manage.getListByPages(0)
        //         );
        // }
        //
        
    }
    
}