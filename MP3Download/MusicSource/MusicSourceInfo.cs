using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MP3Download.MusicSource
{
    public class MusicSourceInfo
    {
        public string SoureName { get; set; }
        public string Musicrid { get; set; }
        public string SongName { get; set; }
        public string Albumname { get; set; }
        public string Singer { get; set; }
        public string QualityLevel { get; set; }        
        public string FileName { get; set; }
        public string FileHash { get; set; }
        public string SQFileHash { get; set; }
        public string HQFileHash { get; set; }
        public string SingerName { get; set; }
        public string ExtName { get; set; }
        public int OwnerCount { get; set; }
        public int HiFiQuality { get; set; }
        public string QQ_Songmid { get; set; }
        public string QQ_Songid { get; set; }
    }
}
