using System;
using System.IO;
using System.Net;
using System.Text;

namespace MP3Download
{
    public class HttpOpera
    {
        /// <summary>
        /// Get方法Http请求
        /// </summary>
        /// <param name="Url"></param>
        /// <param name="cookie"></param>
        /// <returns></returns>
        public static string Get(string Url)
        {
            string result = string.Empty;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
                request.Method = "GET";
                request.ContentType = "application/json; charset=UTF-8";
                request.KeepAlive = false;

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (Stream responseStm = response.GetResponseStream())
                    {
                        StreamReader redStm = new StreamReader(responseStm, Encoding.UTF8);
                        result = redStm.ReadToEnd();
                        redStm.Close();
                    }
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }

            return result;
        }

        public static string Get(string Url, CookieContainer cookie)
        {
            string result = string.Empty;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
                request.Method = "GET";
                request.ContentType = "application/json; charset=UTF-8";
                request.CookieContainer = cookie;
                request.KeepAlive = true;

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (Stream responseStm = response.GetResponseStream())
                    {
                        StreamReader redStm = new StreamReader(responseStm, Encoding.UTF8);
                        result = redStm.ReadToEnd();
                        redStm.Close();
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
        /// Post方法Http请求
        /// </summary>
        /// <param name="Url"></param>
        /// <param name="postData"></param>
        /// <param name="cookie"></param>
        /// <returns></returns>
        public static string Post(string Url, string postData)
        {
            string result = string.Empty;
            byte[] data = Encoding.UTF8.GetBytes(postData);

            try
            {
                //string param = HttpUtility.UrlEncode("参数一", myEncoding);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
                request.Method = "POST";
                request.ContentType = "application/json; charset=UTF-8";
                request.Timeout = 10000;
                //request.CookieContainer = cookie;
                request.KeepAlive = false;
                request.ContentLength = data.LongLength;

                // 发送请求数据
                using (Stream requestStm = request.GetRequestStream())
                {
                    requestStm.Write(data, 0, data.Length);
                }

                // 接收响应数据
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    //response.Cookies = cookie.GetCookies(response.ResponseUri);
                    Stream responseStream = response.GetResponseStream();
                    StreamReader streamReader = new StreamReader(responseStream, Encoding.UTF8);
                    result = streamReader.ReadToEnd();
                    streamReader.Close();
                    responseStream.Close();
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }

            return result;
        }
    }
}
