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
    public class Music_Source_KW : Music_Source_Base
    {
        /// <summary>
        /// 音乐网络搜索
        /// </summary>
        public override void SearchPress()
        {
            List<MusicSourceInfo> resultList = new List<MusicSourceInfo>();
            string codeName = HttpUtility.UrlEncode(this.KeyWord);
            string url = string.Format("http://search.kuwo.cn/r.s?all={0}&ft=music&itemset=web_2013&client=kt&pn=0&rn=250&rformat=json&encoding=utf8", codeName);

            try
            {
                string result = HttpOpera.Get(url);
                JObject json = JObject.Parse(result);
                if ((int)json["SHOW"] == 0) return;

                JArray list = (JArray)json["abslist"];
                foreach (JObject item in list)
                {
                    MusicSourceInfo info = new MusicSourceInfo();
                    info.SoureName = "酷我音乐";
                    info.Musicrid = (string)item["MUSICRID"];
                    info.SongName = (string)item["SONGNAME"];
                    info.SingerName = (string)item["ARTIST"];

                    info.SongName = info.SongName.Replace("&nbsp;", "");
                    resultList.Add(info);
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
            string url = string.Format("http://antiserver.kuwo.cn/anti.s?type=convert_url&rid={0}&format=aac|mp3&response=url", info.Musicrid);
            MusicDownloadInfo downloadInfo = new MusicDownloadInfo();

            try
            {
                string result = HttpOpera.Get(url);
                //JObject json = JObject.Parse(result);
                //if (json["err_code"].ToString() == "0")
                //{                    
                downloadInfo.audio_name = info.SongName;
                downloadInfo.play_url = result;
                downloadInfo.extname = result.Substring(result.Length - 3);
                //}

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
