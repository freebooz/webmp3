using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MP3Download.MusicSource
{
    /// <summary>
    /// QQ音乐源
    /// </summary>
    public class Music_Source_QQ : Music_Source_Base
    {
        public override void SearchPress()
        {
            string searchUrl = "https://c.y.qq.com/soso/fcgi-bin/client_search_cp?aggr=1&cr=1&flag_qc=0&p=1&n=100&w=<name>";
            List<MusicSourceInfo> resultList = new List<MusicSourceInfo>();
            string codeName = HttpUtility.UrlEncode(this.KeyWord);
            string url = searchUrl.Replace("<name>", codeName);

            try
            {
                string result = HttpOpera.Get(url);
                result = result.Substring(9);
                int len = result.Length - 1;
                result = result.Substring(0, len);

                JObject json = JObject.Parse(result);
                if (json["code"].ToString() == "0")
                {
                    JArray list = (JArray)json["data"]["song"]["list"];
                    foreach (JObject item in list)
                    {
                        MusicSourceInfo info = new MusicSourceInfo();
                        info.SoureName = "QQ音乐";
                        info.SongName = (string)item["songname"];
                        info.Albumname = (string)item["albumname"];
                        info.QQ_Songmid = (string)item["songmid"];
                        info.QQ_Songid = (string)item["songid"];
                        JArray singerlist = (JArray)item["singer"];
                        info.SingerName = (string)item["name"];

                        //info.ExtName = (string)item["ExtName"];
                        //info.FileHash = (string)item["FileHash"];
                        //info.SongName = (string)item["SongName"];
                        //info.SQFileHash = (string)item["SQFileHash"];
                        //info.HQFileHash = (string)item["HQFileHash"];
                        //info.HiFiQuality = (int)item["HiFiQuality"];
                        //info.OwnerCount = (int)item["OwnerCount"];
                        resultList.Add(info);
                    }
                }

                this.OnMusicFindedPress(resultList);
            }
            catch(Exception ex)
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
            string url = "http://ws.stream.qqmusic.qq.com/C100<songmid>.m4a?fromtag=0";
            url = url.Replace("<songmid>", info.QQ_Songmid);
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
