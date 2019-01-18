using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MP3Download.MusicSource
{
    /// <summary>
    /// 音乐源基类
    /// </summary>
    public class Music_Source_Base
    {
        /// <summary>
        /// http请求
        /// </summary>
        public HttpOpera HttpOpera;
        /// <summary>
        /// 文件下载
        /// </summary>
        public FileDownload FileDownload;
        /// <summary>
        /// 查找到音乐事件
        /// </summary>
        public event EventHandler<MusicSourceInfoEvrg> OnMusicFinded;
        public event EventHandler<ErrorEvrg> OnError;
        public string KeyWord;

        public Music_Source_Base()
        {
            this.HttpOpera = new HttpOpera();
            this.FileDownload = new FileDownload();
        }

        /// <summary>
        /// 搜索歌曲
        /// </summary>
        /// <param name="keyWord"></param>
        public void Search(string keyWord)
        {
            this.KeyWord = keyWord;

            Thread thread = new Thread(new ThreadStart(SearchPress));
            thread.Start();
            //this.SearchPress();
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void SearchPress()
        {            
        }

        public virtual MusicDownloadInfo GetDownloadInfo(MusicSourceInfo info)
        {
            return null;
        }
        public string Download(MusicSourceInfo info, string path)
        {
            string saveFileName = string.Empty;
            MusicDownloadInfo loadinfo = this.GetDownloadInfo(info);
            saveFileName = path + string.Format("{0}.{1}", loadinfo.audio_name, loadinfo.extname);

            DirectoryInfo directory = new DirectoryInfo(path);
            if (!directory.Exists)
            {
                directory.Create();
            }

            if (File.Exists(saveFileName))
            {
                return saveFileName;
            }

            bool result = this.FileDownload.DownLoad(loadinfo.play_url, saveFileName);
            return (result)? saveFileName : "";
        }

        /// <summary>
        /// 查找到音乐事件通知处理
        /// </summary>
        /// <param name="list"></param>
        public void OnMusicFindedPress(List<MusicSourceInfo> list)
        {
            OnMusicFinded?.Invoke(this, new MusicSourceInfoEvrg(list));
        }

        /// <summary>
        /// 错误处理
        /// </summary>
        /// <param name="msg"></param>
        public void OnErrorPress(string msg)
        {
            OnError?.Invoke(this, new ErrorEvrg(msg));
        }
    }
}
