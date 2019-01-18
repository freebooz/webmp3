using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MP3Download
{
    public class FileDownload
    {
        //public event EventHandler OnDownloading;
        //public event EventHandler OnDownloaded;
        //public event EventHandler OnDownloadError;

        public bool DownLoad(string url, string fileName)
        {
            try
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                using (HttpWebResponse rsp = (HttpWebResponse)req.GetResponse())
                {
                    using (Stream reader = rsp.GetResponseStream())
                    {
                        using (FileStream writer = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Write))
                        {
                            byte[] buff = new byte[1024];

                            int c = 0; //实际读取的字节数
                            while ((c = reader.Read(buff, 0, buff.Length)) > 0)
                            {
                                writer.Write(buff, 0, c);
                            }
                        }
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
