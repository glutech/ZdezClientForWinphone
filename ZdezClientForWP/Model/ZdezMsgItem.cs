using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Linq.Mapping;
using Microsoft.Phone.Data.Linq.Mapping;

namespace ZdezClientForWP.Model
{
    /*
    // 服务器端结构
    // cn.com.zdez.vo.ZdezMsgVo
    public class ZdezMsgVo
    {
        private int zdezMsgId;
        private String coverPath;
        private String title;
        private String content;
        private String date;
        private int receivedNum;
        private int receiverNum;
    }
    // 通信结构
    //      ZdezMsgId    =>     Id
    //      CoverPath    =>     Cover
    //      Title        =>     Title
    //      Content      =>     Content
    //      Date         =>     Time<datetime>
    // 自定义结构
    //      HasRead<bool>
    */

    [Index(Columns = "Time", IsUnique = false, Name = "TimeIndex"), Index(Columns = "HasRead", IsUnique = false, Name = "HasReadIndex")]
    [Table(Name = "ZdezMsg")]
    public class ZdezMsgItem : MsgItem
    {

        private int _id;
        [Column(IsPrimaryKey = true)]
        public override int Id
        {
            get
            {
                return _id;
            }
            set
            {
                if (_id != value)
                {
                    NotifyPropertyChanging("Id");
                    _id = value;
                    NotifyPropertyChanged("Id");
                }
            }
        }

        private string _cover;
        [Column]
        public string Cover
        {
            get
            {
                return _cover;
            }
            set
            {
                if (_cover != value)
                {
                    NotifyPropertyChanging("Cover");
                    _cover = value;
                    NotifyPropertyChanged("Cover");
                }
            }
        }

        private string _title;
        [Column]
        public override string Title
        {
            get
            {
                return _title;
            }
            set
            {
                if (_title != value)
                {
                    NotifyPropertyChanging("Title");
                    _title = value;
                    NotifyPropertyChanged("Title");
                }
            }
        }

        private string _content;
        [Column(DbType = "ntext", UpdateCheck = UpdateCheck.Never)]
        public override string Content
        {
            get
            {
                return _content;
            }
            set
            {
                if (_content != value)
                {
                    NotifyPropertyChanging("Content");
                    _content = value;
                    NotifyPropertyChanged("Content");
                }
            }
        }

        private DateTime _time;
        [Column]
        public override DateTime Time
        {
            get { return _time; }
            set
            {
                if (_time != value)
                {
                    NotifyPropertyChanging("Time");
                    _time = value;
                    NotifyPropertyChanged("Time");
                }
            }
        }

        private bool _hasRead;
        [Column]
        public override bool HasRead
        {
            get
            {
                return _hasRead;
            }
            set
            {
                if (_hasRead != value)
                {
                    NotifyPropertyChanging("HasRead");
                    _hasRead = value;
                    NotifyPropertyChanged("HasRead");
                }
            }
        }

        public ZdezMsgItem() { }

        public static ZdezMsgItem Parse(int zdezMsgId, string coverPath, string title, string content, string date)
        {
            return new ZdezMsgItem()
            {
                Id = zdezMsgId,
                Cover = coverPath,
                Title = title,
                Content = content,
                Time = DateTime.Parse(date),
                HasRead = false
            };
        }
    }
}