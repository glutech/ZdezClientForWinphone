using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ZdezClientForWP.Model;
using ZdezClientForWP.Helper;

namespace ZdezClientForWP.Service
{
    static class LocalService
    {

        /// <summary>
        /// 数据库不支持跨线程，所有数据库操作全部上锁
        /// </summary>
        private static object locker = new object();

        public static IEnumerable<MsgItem> GetNewsMsg(IEnumerable<MsgItem> source)
        {
            lock (locker)
            {
                return (from q in MsgContext.Instance.NewsMsgItems where !(from t in source select t.Id).Contains(q.Id) orderby q.Time descending select (MsgItem)q).Take(20).ToList();
            }
        }

        public static IEnumerable<MsgItem> GetNoticeMsg(IEnumerable<MsgItem> source)
        {
            lock (locker)
            {
                return (from q in MsgContext.Instance.NoticeMsgItems where !(from t in source select t.Id).Contains(q.Id) orderby q.Time descending select (MsgItem)q).Take(20).ToList();
            }
        }

        public static IEnumerable<MsgItem> GetZdezMsg(IEnumerable<MsgItem> source)
        {
            lock (locker)
            {
                return (from q in MsgContext.Instance.ZdezMsgItems where !(from t in source select t.Id).Contains(q.Id) orderby q.Time descending select (MsgItem)q).Take(20).ToList();
            }
        }

        public static IEnumerable<MsgItem> FlushNewsMsg(IEnumerable<MsgItem> query)
        {
            lock (locker)
            {
                var table = MsgContext.Instance.NewsMsgItems;
                var distance_query = (from q in query where !(from d in table select d.Id).Contains(q.Id) select (NewsMsgItem)q).ToList();
                foreach (var item in distance_query)
                {
                    table.InsertOnSubmit(item);
                }
                MsgContext.Instance.SubmitChanges();
                System.Diagnostics.Debug.WriteLine("FlushNewsMsg: DISTANCE " + (query.Count() - distance_query.Count) + " items， CURRENT " + distance_query.Count + " items.");
                return distance_query.Map<NewsMsgItem, MsgItem>(item => (MsgItem)item);
            }
        }

        public static IEnumerable<MsgItem> FlushNoticeMsg(IEnumerable<MsgItem> query)
        {
            lock (locker)
            {
                var table = MsgContext.Instance.NoticeMsgItems;
                var distance_query = (from q in query where !(from d in table select d.Id).Contains(q.Id) select (NoticeMsgItem)q).ToList();
                foreach (var item in distance_query)
                {
                    table.InsertOnSubmit(item);
                }
                MsgContext.Instance.SubmitChanges();
                System.Diagnostics.Debug.WriteLine("FlushNoticeMsg: DISTANCE " + (query.Count() - distance_query.Count) + " items， CURRENT " + distance_query.Count + " items.");
                return distance_query.Map<NoticeMsgItem, MsgItem>(item => (MsgItem)item);
            }
        }

        public static IEnumerable<MsgItem> FlushZdezMsg(IEnumerable<MsgItem> query)
        {
            lock (locker)
            {
                var table = MsgContext.Instance.ZdezMsgItems;
                var distance_query = (from q in query where !(from d in table select d.Id).Contains(q.Id) select (ZdezMsgItem)q).ToList();
                foreach (var item in distance_query)
                {
                    table.InsertOnSubmit(item);
                }
                MsgContext.Instance.SubmitChanges();
                System.Diagnostics.Debug.WriteLine("FlushZdezMsg: DISTANCE " + (query.Count() - distance_query.Count) + " items， CURRENT " + distance_query.Count + " items.");
                return distance_query.Map<ZdezMsgItem, MsgItem>(item => (MsgItem)item);
            }
        }

        public static void MarkMsgReaded(MsgItem msg)
        {
            lock (locker)
            {
                msg.HasRead = true;
                MsgContext.Instance.SubmitChanges();
            }
        }

        public static int GetTopNoticeForShellTile(out int id, out string title)
        {
            lock (locker)
            {
                var query = (from item in MsgContext.Instance.NoticeMsgItems where !item.HasRead orderby item.Time descending select item).ToList();
                var count = query.Count;
                if (count != 0)
                {
                    var first = query[0];
                    id = first.Id;
                    title = first.Title;
                }
                else
                {
                    id = 0;
                    title = null;
                }
                return count;
            }
        }

    }
}
