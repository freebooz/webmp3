using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace MP3Download.MusicSource
{
    /// <summary>
    /// 酷狗音乐
    /// </summary>
    public class Music_Source_KG : Music_Source_Base
    {
        /// <summary>
        /// 音乐网络搜索
        /// </summary>
        public override void SearchPress()
        {
            string searchUrl = "http://songsearch.kugou.com/song_search_v2?keyword=<name>&page=1&pagesize=100";
            List<MusicSourceInfo> resultList = new List<MusicSourceInfo>();
            string codeName = HttpUtility.UrlEncode(this.KeyWord);
            string url = searchUrl.Replace("<name>", codeName);

            try
            {
                string result = HttpOpera.Get(url);
                JObject json = JObject.Parse(result);
                if ((int)json["data"]["total"] == 0) return;

                if (json["error_code"].ToString() == "0")
                {
                    JArray list = (JArray)json["data"]["lists"];
                    foreach (JObject item in list)
                    {
                        MusicSourceInfo info = new MusicSourceInfo();
                        info.SoureName = "酷狗音乐";
                        info.SongName = (string)item["SongName"];
                        info.SingerName = (string)item["SingerName"];
                        info.FileName = (string)item["FileName"];
                        info.ExtName = (string)item["ExtName"];                        
                        info.SongName = (string)item["SongName"];
                        info.QualityLevel = (string)item["QualityLevel"];
                        info.FileHash = (string)item["FileHash"];
                        info.SQFileHash = (string)item["SQFileHash"];
                        info.HQFileHash = (string)item["HQFileHash"];
                        info.HiFiQuality = (int)item["HiFiQuality"];
                        info.OwnerCount = (int)item["OwnerCount"];
                        resultList.Add(info);
                    }
                }

                this.OnMusicFindedPress(resultList);
            }
            catch (Exception ex)
            {
                this.OnErrorPress(ex.Message);
            }
        }

        /// <summary>
        /// 获取音乐文件下载信息
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public override MusicDownloadInfo GetDownloadInfo(MusicSourceInfo info)
        {
            string url = "http://www.kugou.com/yy/index.php?r=play/getdata&hash=<hash>";
            url = url.Replace("<hash>", info.FileHash);
            MusicDownloadInfo downloadInfo = new MusicDownloadInfo();

            try
            {
                string result = HttpOpera.Get(url);
                JObject json = JObject.Parse(result);
                if (json["err_code"].ToString() == "0")
                {                    
                    downloadInfo.audio_name = (string)json["data"]["audio_name"];
                    downloadInfo.play_url = (string)json["data"]["play_url"];
                    downloadInfo.extname = "mp3";
                }

                return downloadInfo;
            }
            catch (Exception ex)
            {
                this.OnErrorPress(ex.Message);
                return null;
            }
        }
    }
}
