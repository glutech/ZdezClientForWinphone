using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace ZdezClientForWP.Model
{
    public abstract class MsgItem : INotifyPropertyChanged, INotifyPropertyChanging
    {

        public abstract int Id { get; set; }

        public abstract string Title { get; set; }

        public abstract string Content { get; set; }

        public abstract DateTime Time { get; set; }

        public abstract bool HasRead { get; set; }

        public string Date
        {
            get { return Time.ToString("yyyy年MM月dd日"); }
        }

        public string WeekOrTime
        {
            get
            {
                if (Time.Date.Equals(DateTime.Now.Date))
                {
                    return Time.ToString("HH:mm");
                }
                else
                {
                    var w = Time.DayOfWeek;
                    switch (w)
                    {
                        case DayOfWeek.Monday: return "周一";
                        case DayOfWeek.Tuesday: return "周二";
                        case DayOfWeek.Wednesday: return "周三";
                        case DayOfWeek.Thursday: return "周四";
                        case DayOfWeek.Friday: return "周五";
                        case DayOfWeek.Saturday: return "周六";
                        case DayOfWeek.Sunday: return "周日";
                    }
                    throw new Exception();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public event PropertyChangingEventHandler PropertyChanging;
        protected void NotifyPropertyChanging(string propertyName)
        {
            if (PropertyChanging != null)
            {
                PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
            }
        }

    }
}