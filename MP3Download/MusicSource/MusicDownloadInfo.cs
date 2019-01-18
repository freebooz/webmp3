using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MP3Download.MusicSource
{
    public class MusicDownloadInfo
    {
        public string audio_name { get; set; }
        public string play_url { get; set; }
        public string img { get; set; }
        public string hash { get; set; }
        public string author_name { get; set; }
        public string album_name { get; set; }
        public long timelength { get; set; }
        public long filesize { get; set; }
        public string extname { get; set; }
    }
}
