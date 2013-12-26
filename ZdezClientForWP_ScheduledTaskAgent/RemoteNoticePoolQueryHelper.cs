using System.IO;
using System.Xml.Linq;

namespace ZdezClientForWP_ScheduledTaskAgent
{
    static class RemoteNoticePoolQueryHelper
    {
        public class DataStruct
        {
            public int ToReceiveNum { get; private set; }
            public int LastItemId { get; private set; }
            public string LastItemTitle { get; private set; }

            public DataStruct(int toReceiveNum, int lastItemId, string lastItemTitle)
            {
                this.ToReceiveNum = toReceiveNum;
                this.LastItemId = lastItemId;
                this.LastItemTitle = lastItemTitle;
            }
        }

        public static DataStruct Parse(string xmlContent)
        {
            int toReceiveNum;
            int lastItemid;
            string lastItemTitle;
            using (var xmlContentTextReader = new StringReader(xmlContent))
            {
                var doc = XDocument.Load(xmlContentTextReader, LoadOptions.None);
                var root = doc.Element("NoticePool");
                toReceiveNum = int.Parse(root.Element("ToReceiveNum").Value);
                lastItemid = int.Parse(root.Element("LastItemId").Value);
                lastItemTitle = root.Element("LastItemTitle").Value;
            }
            return new DataStruct(toReceiveNum, lastItemid, lastItemTitle);
        }

    }
}
