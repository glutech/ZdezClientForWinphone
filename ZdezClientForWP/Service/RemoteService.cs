using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ZdezClientForWP.Model;
using ZdezClientForWP.Helper;

namespace ZdezClientForWP.Service
{
    static class RemoteService
    {

        public class ReturnType
        {
            public bool Status { get; private set; }
            public string Message { get; private set; }
            public object Data { get; private set; }

            public ReturnType(bool status, object msgOrData)
            {
                if (status)
                {
                    this.Status = true;
                    this.Data = msgOrData;
                }
                else
                {
                    this.Status = false;
                    this.Message = (string)msgOrData;
                }
            }
        }

        public static ReturnType Login(string username, string password)
        {
            var result = HttpHelper.RequestOnSync(
                new Uri(SettingService.HOST + "WPClient_StudentLogiCheck"),
                HttpHelper.RequestTypeEnum.POST,
                new Toolkit.DictWrapper().OR("username", username).OR("password", password).OR(VERSION_NAME, VERSION_NAME)
            );
            if (result.Status)
            {
                if (result.Data is string)
                {
                    return new ReturnType(false, result.Data);
                }
                else
                {
                    var jobj = result.Data as JObject;
                    if (jobj != null)
                    {
                        return new ReturnType(true, new AuthUserStruct(
                            Convert.ToInt32(jobj["id"]),
                            (string)jobj["username"],
                            (string)jobj["name"],
                            (string)jobj["gender"],
                            (string)jobj["grade"],
                            (string)jobj["major"],
                            (string)jobj["department"]
                        ));
                    }
                    else
                    {
                        return new ReturnType(false, "解析错误");
                    }
                }
            }
            else
            {
                return new ReturnType(false, result.Message);
            }
        }

        public static ReturnType Logout()
        {
            var result = HttpHelper.RequestOnSync(
                new Uri(SettingService.HOST + "WPClient_NoNotificationAnymore"),
                HttpHelper.RequestTypeEnum.POST,
                new Toolkit.DictWrapper().OR("username", AuthService.Instance.AuthUsername)
            );
            if (result.Status)
            {
                if (result.Data is bool && (bool)result.Data == true)
                {
                    return new ReturnType(true, null);
                }
                else
                {
                    return new ReturnType(false, "服务器拒绝请求");
                }
            }
            else
            {
                return new ReturnType(false, result.Message);
            }
        }

        public static ReturnType UpdateNotificationChannelUri(Uri newUri)
        {
            var result = HttpHelper.RequestOnSync(
                new Uri(SettingService.HOST + "WPClient_ModifyStaus"),
                HttpHelper.RequestTypeEnum.POST,
                new Toolkit.DictWrapper().OR("stu_username", AuthService.Instance.AuthUsername).OR("stu_staus", newUri)
            );
            if (result.Status)
            {
                if (result.Data is bool && (bool)result.Data == true)
                {
                    return new ReturnType(true, null);
                }
                else
                {
                    return new ReturnType(false, "服务器拒绝请求");
                }
            }
            else
            {
                return new ReturnType(false, result.Message);
            }
        }

        public static ReturnType GetSchoolMsg()
        {
            var result = HttpHelper.RequestOnSync(
                new Uri(SettingService.HOST + "AndroidClient_GetUpdateSchoolMsg"),
                HttpHelper.RequestTypeEnum.POST,
                new Toolkit.DictWrapper().OR("user_id", AuthService.Instance.AuthId).OR("version_name", VERSION_NAME)
            );
            if (result.Status)
            {
                if (result.Data is string)
                {
                    return new ReturnType(false, result.Data);
                }
                else
                {
                    var data = result.Data as JArray;
                    if (data != null)
                    {
                        if (data.Count == 0)
                        {
                            return new ReturnType(true, null);
                        }
                        else
                        {
                            var r = new List<MsgItem>(data.Count);
                            foreach (var item in data)
                            {
                                var o = item as JObject;
                                r.Add(NoticeMsgItem.Parse(
                                    Convert.ToInt32(o["schoolMsgId"]),
                                    (string)o["coverPath"],
                                    (string)o["title"],
                                    (string)o["content"],
                                    (string)o["date"],
                                    (string)o["schoolName"],
                                    (string)o["remarks"]
                                    ));
                            }
                            return new ReturnType(true, r);
                        }
                    }
                    else
                    {
                        return new ReturnType(false, "解析错误");
                    }
                }
            }
            else
            {
                return new ReturnType(false, result.Message);
            }
        }

        public static ReturnType GetNewsMsg()
        {
            var result = HttpHelper.RequestOnSync(
                new Uri(SettingService.HOST + "AndroidClient_GetUpdateNews"),
                HttpHelper.RequestTypeEnum.POST,
                new Toolkit.DictWrapper().OR("user_id", AuthService.Instance.AuthId).OR("version_name", VERSION_NAME)
            );
            if (result.Status)
            {
                if (result.Data is string)
                {
                    return new ReturnType(false, result.Data);
                }
                else
                {
                    var data = result.Data as JArray;
                    if (data != null)
                    {
                        if (data.Count == 0)
                        {
                            return new ReturnType(true, null);
                        }
                        else
                        {
                            var r = new List<MsgItem>(data.Count);
                            foreach (var item in data)
                            {
                                var o = item as JObject;
                                r.Add(NewsMsgItem.Parse(
                                    Convert.ToInt32(o["id"]),
                                    (string)o["coverPath"],
                                    (string)o["title"],
                                    (string)o["content"],
                                    (string)o["date"],
                                    Convert.ToInt32(o["isTop"])
                                    ));
                            }
                            return new ReturnType(true, r);
                        }
                    }
                    else
                    {
                        return new ReturnType(false, "解析错误");
                    }
                }
            }
            else
            {
                return new ReturnType(false, result.Message);
            }
        }

        public static ReturnType GetZdezMsg()
        {
            var result = HttpHelper.RequestOnSync(
                new Uri(SettingService.HOST + "AndroidClient_GetUpdateZdezMsg"),
                HttpHelper.RequestTypeEnum.POST,
                new Toolkit.DictWrapper().OR("user_id", AuthService.Instance.AuthId).OR("version_name", VERSION_NAME)
            );
            if (result.Status)
            {
                if (result.Data is string)
                {
                    return new ReturnType(false, result.Data);
                }
                else
                {
                    var data = result.Data as JArray;
                    if (data != null)
                    {
                        if (data.Count == 0)
                        {
                            return new ReturnType(true, null);
                        }
                        else
                        {
                            var r = new List<MsgItem>(data.Count);
                            foreach (var item in data)
                            {
                                var o = item as JObject;
                                r.Add(ZdezMsgItem.Parse(
                                    Convert.ToInt32(o["zdezMsgId"]),
                                    (string)o["coverPath"],
                                    (string)o["title"],
                                    (string)o["content"],
                                    (string)o["date"]
                                    ));
                            }
                            return new ReturnType(true, r);
                        }
                    }
                    else
                    {
                        return new ReturnType(false, "解析错误");
                    }
                }
            }
            else
            {
                return new ReturnType(false, result.Message);
            }
        }

        
        

        public static void MarkSchoolMsgReceived(IEnumerable<string> ids)
        {
            HttpHelper.RequestOnSync(
                new Uri(SettingService.HOST + "WPClient_UpdateSchoolMsgReceived"),
                HttpHelper.RequestTypeEnum.POST,
                new Toolkit.DictWrapper().OR("ack", Newtonsoft.Json.JsonConvert.SerializeObject(
                        new MsgReceivedStruct() { userId = AuthService.Instance.AuthId,msgIds = ids}
                        ))
            );
        }

        public static void MarkNewsMsgReceived(IEnumerable<string> ids)
        {
            var x2x = ids.ToArray();
            HttpHelper.RequestOnSync(
                new Uri(SettingService.HOST + "WPClient_UpdateNewsReceived"),
                HttpHelper.RequestTypeEnum.POST,
                new Toolkit.DictWrapper().OR("ack", Newtonsoft.Json.JsonConvert.SerializeObject(
                        new MsgReceivedStruct() { userId = AuthService.Instance.AuthId, msgIds = ids }
                        ))
            );
        }

        public static void MarkZdezMsgReceived(IEnumerable<string> ids)
        {
            HttpHelper.RequestOnSync(
                new Uri(SettingService.HOST + "WPClient_UpdateZdezMsgReceived"),
                HttpHelper.RequestTypeEnum.POST,
                new Toolkit.DictWrapper().OR("ack", Newtonsoft.Json.JsonConvert.SerializeObject(
                        new MsgReceivedStruct() { userId = AuthService.Instance.AuthId, msgIds = ids }
                        ))
            );
        }

        public static ReturnType ModifyPassword(string sourcePassword, string newPassword)
        {
            var result = HttpHelper.RequestOnSync(
                new Uri(SettingService.HOST + "WPClient_ModifyPsw"),
                HttpHelper.RequestTypeEnum.POST,
                new Toolkit.DictWrapper().OR("id", AuthService.Instance.AuthId).OR("oldpsw", sourcePassword).OR("newpsw", newPassword)
            );
            if (result.Status)
            {
                if (result.Data is bool && (bool)result.Data == true)
                {
                    return new ReturnType(true, true);
                }
                else
                {
                    return new ReturnType(true, false);
                }
            }
            else
            {
                return new ReturnType(false, result.Message);
            }
        }

        public static ReturnType PostFeedback(string content)
        {
            var result = HttpHelper.RequestOnSync(
                new Uri(SettingService.HOST + "WPClient_Feedback"),
                HttpHelper.RequestTypeEnum.POST,
                new Toolkit.DictWrapper().OR("user_id", AuthService.Instance.AuthId).OR("feedback", content)
            );
            if (result.Status)
            {
                return new ReturnType(true, null);
            }
            else
            {
                return new ReturnType(false, null);
            }
        }

        /// <summary>
        /// 当前程序标志版本号
        /// </summary>
        private const string VERSION_NAME = "WP-1.0";

    }

    // WindowsPhone7 不支持匿名反射和内部类反射
    // 故做成public实体类
    public class MsgReceivedStruct
    {
        public int userId { get; set; }
        public IEnumerable<string> msgIds { get; set; }
    }

}