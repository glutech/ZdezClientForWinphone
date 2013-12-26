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
    // cn.com.zdez.vo.SchoolMsgVo
    public class SchoolMsgVo
    {
        private int schoolMsgId;
        private String coverPath;
        private String title;
        private String content;
        private String date;
        private String schoolName;
        private String senderName;
        private List<String> destGrade;
        private List<String> destDepartment;
        private List<String> destMajor;
        private int receivedNum;
        private int receiverNum;
        private String remarks;
    }
     // 客户端需要结构
     //     SchoolMsgId  =>     Id
     //     CoverPath    =>     Cover
     //     Title        =>     Title
     //     Content      =>     Content
     //     Date         =>     Time<DateTime>
     //     SchoolName   =>     School
     //     Remarks      =>     Remarks
     // 自定义结构
     //     HasRead<bool>
     */

    [Index(Columns = "Time", IsUnique = false, Name = "TimeIndex"), Index(Columns = "HasRead", IsUnique = false, Name = "HasReadIndex")]
    [Table(Name = "NoticeMsg")]
    public class NoticeMsgItem : MsgItem
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
            get
            {
                return _time;
            }
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

        private string _school;
        [Column]
        public string School
        {
            get
            {
                return _school;
            }
            set
            {
                if (_school != value)
                {
                    NotifyPropertyChanging("School");
                    _school = value;
                    NotifyPropertyChanged("School");
                }
            }
        }

        private string _remarks;
        [Column]
        public string Remarks
        {
            get
            {
                return _remarks;
            }
            set
            {
                if (_remarks != value)
                {
                    NotifyPropertyChanging("Remarks");
                    _remarks = value;
                    NotifyPropertyChanged("Remarks");
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

        public NoticeMsgItem() { }

        public static NoticeMsgItem Parse(int schoolMsgId, string coverPath, string title, string content, string date, string schoolName, string remarks)
        {
            return new NoticeMsgItem()
            {
                Id = schoolMsgId,
                Cover = coverPath,
                Title = title,
                Content = content,
                Time = DateTime.Parse(date),
                School = schoolName,
                Remarks = remarks,
                HasRead = false
            };
        }
    }
}