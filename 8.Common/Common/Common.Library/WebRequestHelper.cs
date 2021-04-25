using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;
using System.IO.Compression;
using System.Security.Cryptography.X509Certificates;

namespace Common.Library
{
    /// <summary>
    /// WebRequest帮助类
    /// </summary>
    public class WebRequestHelper
    {
        /// <summary>
        /// Get请求
        /// </summary>
        /// <param name="url">url地址</param>
        /// <param name="headerDic">header参数</param>
        /// <returns></returns>
        public string HttpGet(string url, Dictionary<string, string> headerDic = null)
        {
            string result = string.Empty;
            HttpWebRequest wbRequest = null;
            try
            {
                //如果是发送HTTPS请求
                if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
                {
                    ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Ssl3;
                    wbRequest = (HttpWebRequest)WebRequest.Create(url);
                    wbRequest.ProtocolVersion = HttpVersion.Version10;
                }
                else
                {
                    wbRequest = (HttpWebRequest)WebRequest.Create(url);
                }
                wbRequest.Method = "GET";
                if (headerDic != null && headerDic.Count > 0)
                {
                    foreach (var item in headerDic)
                    {
                        wbRequest.Headers.Add(item.Key, item.Value);
                    }
                }
                HttpWebResponse wbResponse = (HttpWebResponse)wbRequest.GetResponse();
                Stream responseStream = wbResponse.GetResponseStream();

                if (wbResponse.ContentEncoding.ToLower().Contains("gzip"))
                    responseStream = new GZipStream(responseStream, CompressionMode.Decompress);
                else if (wbResponse.ContentEncoding.ToLower().Contains("deflate"))
                    responseStream = new DeflateStream(responseStream, CompressionMode.Decompress);
                using (responseStream)
                {
                    using (StreamReader sReader = new StreamReader(responseStream, Encoding.UTF8))
                    {
                        result = sReader.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        /// <summary>
        /// Post请求
        /// </summary>
        /// <param name="url">url地址</param>
        /// <param name="paramData">内容</param>
        /// <param name="headerDic">header参数</param>
        /// <returns></returns>
        public string HttpPost(string url, string paramData, Dictionary<string, string> headerDic = null)
        {
            string result = string.Empty;
            HttpWebRequest wbRequest = null;
            try
            {
                //如果是发送HTTPS请求
                if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
                {
                    ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Ssl3;
                    wbRequest = (HttpWebRequest)WebRequest.Create(url);
                    wbRequest.ProtocolVersion = HttpVersion.Version10;
                }
                else
                {
                    wbRequest = (HttpWebRequest)WebRequest.Create(url);
                }
                wbRequest.Method = "POST";
                wbRequest.ContentType = "application/x-www-form-urlencoded";
                wbRequest.ContentLength = Encoding.UTF8.GetByteCount(paramData);
                if (headerDic != null && headerDic.Count > 0)
                {
                    foreach (var item in headerDic)
                    {
                        wbRequest.Headers.Add(item.Key, item.Value);
                    }
                }
                using (Stream requestStream = wbRequest.GetRequestStream())
                {
                    using (StreamWriter swrite = new StreamWriter(requestStream))
                    {
                        swrite.Write(paramData);
                    }
                }
                HttpWebResponse wbResponse = (HttpWebResponse)wbRequest.GetResponse();
                using (Stream responseStream = wbResponse.GetResponseStream())
                {
                    using (StreamReader sread = new StreamReader(responseStream))
                    {
                        result = sread.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        /// <summary>
        /// Http请求
        /// </summary>
        /// <param name="url">url地址</param>
        /// <param name="paramData">内容</param>
        /// <param name="headerDic">header参数</param>
        /// <param name="method">请求方式</param>
        /// <param name="contentType">类型</param>
        /// <returns></returns>
        public string Http(string url, string paramData, Dictionary<string, string> headerDic = null, EMethod method = EMethod.Post, EContentType contentType = EContentType.Form)
        {
            string result = string.Empty;
            HttpWebRequest wbRequest = null;
            try
            {
                //如果是发送HTTPS请求
                if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
                {
                    ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Ssl3;
                    wbRequest = (HttpWebRequest)WebRequest.Create(url);
                    wbRequest.ProtocolVersion = HttpVersion.Version10;
                }
                else
                {
                    wbRequest = (HttpWebRequest)WebRequest.Create(url);
                }
                wbRequest.Method = method.GetDescription();
                wbRequest.ContentType = contentType.GetDescription();
                wbRequest.ContentLength = Encoding.UTF8.GetByteCount(paramData);
                if (headerDic != null && headerDic.Count > 0)
                {
                    foreach (var item in headerDic)
                    {
                        wbRequest.Headers.Add(item.Key, item.Value);
                    }
                }
                using (Stream requestStream = wbRequest.GetRequestStream())
                {
                    using (StreamWriter swrite = new StreamWriter(requestStream))
                    {
                        swrite.Write(paramData);
                    }
                }
                HttpWebResponse wbResponse = (HttpWebResponse)wbRequest.GetResponse();
                using (Stream responseStream = wbResponse.GetResponseStream())
                {
                    using (StreamReader sread = new StreamReader(responseStream))
                    {
                        result = sread.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true; //总是接受
        }
    }
    public enum EContentType
    {
        [Description("application/json")]
        Json = 1,
        [Description("application/xml")]
        Xml = 2,
        [Description("application/x-www-form-urlencoded")]
        Form = 3,
        [Description("image/png")]
        Image = 4
    }
    public enum EMethod
    {
        [Description("GET")]
        Get = 1,
        [Description("POST")]
        Post = 2,
        [Description("PUT")]
        Put = 3,
        [Description("DELETE")]
        Delete = 4
    }
}
