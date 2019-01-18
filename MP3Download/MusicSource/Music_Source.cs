using System;
using System.Collections.Generic;

namespace MP3Download.MusicSource
{
    public class Music_Source
    {
        public event EventHandler<MusicSourceInfoEvrg> OnMuicFinded;
        public event EventHandler<ErrorEvrg> OnError;

        private Music_Source_Base MSource_QQ;
        private Music_Source_Base MSource_KG;
        private Music_Source_Base MSource_WY;
        private Music_Source_Base MSource_KW;

        public Music_Source()
        {
            //// QQ音乐源
            //MSource_QQ = new Music_Source_QQ();
            //MSource_QQ.OnMusicFinded += Ms_OnMusicFinded;
            //MSource_QQ.OnError += Ms_OnError;

            // 酷狗音乐源
            MSource_KW = new Music_Source_KW();
            MSource_KW.OnMusicFinded += Ms_OnMusicFinded;
            MSource_KW.OnError += Ms_OnError;

            // 酷狗音乐源
            MSource_KG = new Music_Source_KG();
            MSource_KG.OnMusicFinded += Ms_OnMusicFinded;
            MSource_KG.OnError += Ms_OnError;

            //// 网易音乐源
            //MSource_WY = new Music_Source_WY();
            //MSource_WY.OnMusicFinded += Ms_OnMusicFinded;
            //MSource_WY.OnError += Ms_OnError;
        }

        /// <summary>
        /// 关键字搜索歌曲
        /// </summary>
        /// <param name="keyWord"></param>
        public void Search(string keyWord)
        {
            MSource_KW.Search(keyWord);
            //MSource_KG.Search(keyWord);
            //MSource_QQ.Search(keyWord);
            //MSource_WY.Search(keyWord);
        }

        /// <summary>
        /// 下载歌曲
        /// </summary>
        /// <param name="info"></param>
        public string Download(MusicSourceInfo info, string path)
        {
            string filename = string.Empty;

            switch (info.SoureName)
            {
                case "酷狗音乐":
                    filename = MSource_KG.Download(info, path);
                    break;
                case "酷我音乐":
                    filename = MSource_KW.Download(info, path);
                    break;
                case "QQ音乐":
                    filename = MSource_QQ.Download(info, path);
                    break;
                case "网页音乐":
                    filename = MSource_WY.Download(info, path);
                    break;
            }

            return filename;
        }

        /// <summary>
        /// 发送错误事件通知
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Ms_OnError(object sender, ErrorEvrg e)
        {
            OnError?.Invoke(this, new ErrorEvrg(e.ErrorMsg));
        }

        /// <summary>
        /// 发送音乐查找结果事件通知
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Ms_OnMusicFinded(object sender, MusicSourceInfoEvrg e)
        {
            OnMuicFinded?.Invoke(this, new MusicSourceInfoEvrg(e.MusicSourceInfoList));
        }
    }
}
