using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Threading;

namespace ZdezClientForWP.Helper
{
    /// <summary>
    /// HTTP助手类，包含异步统一回调、GZIP支持、下载内容的自定义处理方案（集成JSON支持）
    /// </summary>
    class HttpHelper
    {

        static HttpHelper()
        {
            // 使用第三方库支持gzip，后续使用 (HttpWebRequest)WebRequest.Create(uri) 创建request即可自动完成解码工作
            WebRequest.RegisterPrefix("http://", SharpGIS.WebRequestCreator.GZip);
            WebRequest.RegisterPrefix("https://", SharpGIS.WebRequestCreator.GZip);
        }

        public enum RequestTypeEnum { POST, GET }

        public class ReturnType
        {
            public bool Status { get; protected set; }
            public string Message { get; protected set; }
            public object Data { get; protected set; }

            public ReturnType(bool status, object MessageOrData)
            {
                if (status)
                {
                    this.Status = true;
                    this.Data = MessageOrData;
                }
                else
                {
                    this.Status = false;
                    this.Message = (string)MessageOrData;
                }
            }
        }

        protected Uri Target { get; private set; }
        protected RequestTypeEnum RequestType { get; private set; }
        protected IDictionary<string, object> Params { get; private set; }
        protected object Caller { get; private set; }
        protected Action<object, ReturnType> CallerCallback;

        protected HttpHelper(Uri target, RequestTypeEnum requestType, IDictionary<string, object> param, object caller, Action<object, ReturnType> callback)
        {
            this.Target = target;
            this.RequestType = requestType;
            this.Params = param;
            this.Caller = caller;
            this.CallerCallback = callback;
        }

        public static ReturnType RequestOnSync(Uri target, RequestTypeEnum request, IDictionary<string, object> param)
        {
            var waiter = new AutoResetEvent(false);
            ReturnType r = null;
            new HttpHelper(target, request, param, null, (caller, returnValue) =>
            {
                r = returnValue;
                waiter.Set();
            }).DoItAsync();
            waiter.WaitOne();
            return r;
        }

        public static void RequestOnAsync(Uri target, RequestTypeEnum request, IDictionary<string, object> param, object caller, Action<object, ReturnType> callback)
        {
            new HttpHelper(target, request, param, caller, callback).DoItAsync();
        }

        public void DoItAsync()
        {
            Step1();
        }

        private void Step1()
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(Target);
                request.Accept = "Accept:text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
                request.Headers["Accept-Encoding"] = "gzip";
                request.Headers["Accept-Language"] = "zh-CN,zh,en";
                request.Headers["Accept-Charset"] = "utf-8;q=0.7,*;q=0.3";
                request.Method = RequestType.ToString();
                if (RequestType == RequestTypeEnum.POST)
                {
                    request.ContentType = "application/x-www-form-urlencoded";
                    request.BeginGetRequestStream(new AsyncCallback(Step2), request);
                }
                else
                {
                    request.BeginGetResponse(new AsyncCallback(Step3), request);
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                CallerCallback(Caller, new ReturnType(false, e.Message));
            }
        }

        private void Step2(IAsyncResult asynchronousResult)
        {
            try
            {
                var request = (HttpWebRequest)asynchronousResult.AsyncState;
                using (var stream = request.EndGetRequestStream(asynchronousResult))
                {
                    if (Params != null)
                    {
                        var paramBytes = BuildParams();
                        stream.Write(paramBytes, 0, paramBytes.Length);
                    }
                }

                request.BeginGetResponse(new AsyncCallback(Step3), request);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                CallerCallback(Caller, new ReturnType(false, e.Message));
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
                        Step4(reader.ReadToEnd());
                    }
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                CallerCallback(Caller, new ReturnType(false, e.Message));
            }
        }

        private void Step4(string download)
        {
            try
            {
                CallerCallback(Caller, new ReturnType(true, Newtonsoft.Json.JsonConvert.DeserializeObject(download)));
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                CallerCallback(Caller, new ReturnType(true, download));
            }
        }

        private byte[] BuildParams()
        {
            var list = new List<string>(Params.Count);
            foreach (var item in Params)
            {
                list.Add(item.Key + "=" + item.Value);
            }
            return Encoding.UTF8.GetBytes(string.Join("&", list));
        }

    }
}