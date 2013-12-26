using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Linq;

namespace ZdezClientForWP.Model
{
    public class MsgContext : DataContext
    {
        const string CONN_STRING = "isostore:/MsgContext.sdf";

        public static void InitDB()
        {
            if (!Instance.DatabaseExists())
            {
                Instance.CreateDatabase();
            }
        }

        private static MsgContext instance;
        private static object locker = new object();
        public static MsgContext Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (locker)
                    {
                        if (instance == null)
                        {
                            instance = new MsgContext();
                        }
                    }
                }
                return instance;
            }
        }

        protected MsgContext()
            : base(CONN_STRING)
        {
        }

        public Table<NewsMsgItem> NewsMsgItems;
        public Table<NoticeMsgItem> NoticeMsgItems;
        public Table<ZdezMsgItem> ZdezMsgItems;

    }
}