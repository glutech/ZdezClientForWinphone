using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Linq.Mapping;
using System.Data.Linq;
using Microsoft.Phone.Data.Linq.Mapping;

namespace ZdezClientForWP.Model
{
    /*
    // 服务器端结构
    // cn.com.zdez.vo.NewsVo
    public class NewsVo
    {
        private int id;
        private String title;
        private String content;
        private String date;
        private String coverPath;
        private List<String> destSchools;
        private int receiverNum;
        private int receivedNum;
        private int isTop;
    }
     // 客户端需要结构
     //     Id          =>      Id
     //     CoverPath   =>      Cover
     //     Title       =>      Title
     //     Content     =>      Content
     //     Date        =>      Time<DateTime>
     //     IsTop       =>      IsTop<bool>
     // 自定义结构
     //     HasRead<bool>
     */

    [Index(Columns = "Time", IsUnique = false, Name = "TimeIndex"), Index(Columns = "HasRead", IsUnique = false, Name = "HasReadIndex"), Index(Columns = "IsTop", IsUnique = false, Name = "IsTopIndex")]
    [Table(Name = "NewsMsg")]
    public class NewsMsgItem : MsgItem
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

        private bool _isTop;
        [Column]
        public bool IsTop
        {
            get
            {
                return _isTop;
            }
            set
            {
                if (_isTop != value)
                {
                    NotifyPropertyChanging("IsTop");
                    _isTop = value;
                    NotifyPropertyChanged("IsTop");
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

        public NewsMsgItem() { }

        public static NewsMsgItem Parse(int id, string coverPath, string title, string content, string date, int isTop)
        {
            return new NewsMsgItem()
            {
                Id = id,
                Cover = coverPath,
                Title = title,
                Content = content,
                Time = DateTime.Parse(date),
                IsTop = isTop == 1 ? true : false,
                HasRead = false
            };
        }
    }
}