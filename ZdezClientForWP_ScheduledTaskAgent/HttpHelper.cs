using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;

namespace ZdezClientForWP_ScheduledTaskAgent
{

    static class HttpHelper
    {

        public static bool Request(Uri target, IDictionary<string, object> param, out string response)
        {
            var syncer = new AutoResetEvent(false);
            bool result_status = false;
            string result_response = null;
            new InnerRealize(target, param, (callback_status, callback_response) =>
                {
                    result_response = callback_response;
                    result_status = callback_status;
                    syncer.Set();
                }).DoRequest();
            syncer.WaitOne();
            response = result_response;
            return result_status;
        }


        private class InnerRealize
        {

            private Uri target;
            private IDictionary<string, object> param;
            protected Action<bool, string> callback;

            public InnerRealize(Uri target, IDictionary<string, object> param, Action<bool, string> callback)
            {
                this.target = target;
                this.param = param;
                this.callback = callback;
            }

            public void DoRequest()
            {
                Step1();
            }

            private void Step1()
            {
                try
                {
                    var request = (HttpWebRequest)WebRequest.Create(target);
                    request.Accept = "Accept:text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
                    request.Headers["Accept-Language"] = "zh-CN,zh,en";
                    request.Headers["Accept-Charset"] = "utf-8;q=0.7,*;q=0.3";
                    request.Method = "POST";
                    request.ContentType = "application/x-www-form-urlencoded";
                    request.BeginGetRequestStream(new AsyncCallback(Step2), request);
                }
                catch (Exception)
                {
                    callback(false, null);
                }
            }

            private void Step2(IAsyncResult asynchronousResult)
            {
                try
                {
                    var request = (HttpWebRequest)asynchronousResult.AsyncState;
                    using (var stream = request.EndGetRequestStream(asynchronousResult))
                    {
                        if (param != null && param.Count != 0)
                        {
                            var sbParam = new StringBuilder(param.Count * 4);
                            foreach (var item in param)
                            {
                                sbParam.Append(item.Key);
                                sbParam.Append("=");
                                sbParam.Append(item.Value);
                                sbParam.Append("&");
                            }
                            sbParam.Remove(sbParam.Length - 1, 1);
                            var bytesParam = Encoding.UTF8.GetBytes(sbParam.ToString());
                            stream.Write(bytesParam, 0, bytesParam.Length);
                        }
                    }
                    request.BeginGetResponse(new AsyncCallback(Step3), request);
                }
                catch (Exception)
                {
                    callback(false, null);
                }
            }

            private void Step3(IAsyncResult asynchronousResult)
            {
                try
                {
                    var request = (HttpWebRequest)asynchronousResult.AsyncState;
                    var response = (HttpWebResponse)request.EndGetResponse(asynchronousResult);
                    using (var stream = response.GetResponseStream())
                    {
                        using (var reader = new StreamReader(stream))
                        {
                            callback(true, reader.ReadToEnd());
                        }
                    }
                }
                catch (Exception)
                {
                    callback(false, null);
                }
            }

        }

    }
}