using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Un4seen.Bass;

namespace MP3Download.MusicPlayer
{
    public class Mp3Player
    {
        private IntPtr handle;
        private int stream;
        public String songName;

        private Graphics gcanvas;
        private Graphics gpicturebox;
        private Bitmap canvas;

        private CancellationTokenSource cts;
        private Thread threadDraw;
        private const int colCount = 256;
        private Control Control;
        public Mp3Player(IntPtr parent, Control dataGridView1)
        {
            this.Control = dataGridView1;
            this.canvas = new Bitmap(dataGridView1.Width, dataGridView1.Height);
            gcanvas = Graphics.FromImage(this.canvas);
            gpicturebox = dataGridView1.CreateGraphics();

            threadDraw = new Thread(new ThreadStart(DrawFFT));
            threadDraw.Start();
            this.cts = new CancellationTokenSource();

            try
            {
                this.handle = parent;
                this.stream = 0;
                Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_CPSPEAKERS, this.handle);
            }
            catch(Exception ex)
            {

            }
        }

        #region 播放控制
        /// <summary>
        /// 开始播放文件
        /// </summary>
        /// <param name="fileName"></param>
        public void play(string fileName)
        {
            this.songName = Path.GetFileNameWithoutExtension(fileName);
            if (stream != 0)
            {
                if (Bass.BASS_ChannelIsActive(stream) == BASSActive.BASS_ACTIVE_PLAYING || Bass.BASS_ChannelIsActive(stream) == BASSActive.BASS_ACTIVE_PAUSED)
                {
                    Bass.BASS_ChannelStop(stream);
                }
                Bass.BASS_StreamFree(stream);
            }

            stream = Bass.BASS_StreamCreateFile(fileName, 0L, 0L, BASSFlag.BASS_MUSIC_FLOAT);
            Bass.BASS_ChannelPlay(stream, false);
        }

        /// <summary>
        /// 暂停
        /// </summary>
        public void Puase()
        {
            if (Bass.BASS_ChannelIsActive(stream) == BASSActive.BASS_ACTIVE_PLAYING)
            {
                Bass.BASS_ChannelPause(stream);
            }
            else if(Bass.BASS_ChannelIsActive(stream) == BASSActive.BASS_ACTIVE_PAUSED)
            {
                Bass.BASS_ChannelPlay(stream, false);
            }

        }

        /// <summary>
        /// 停止播放
        /// </summary>
        public void stop()
        {
            if (stream != 0 && Bass.BASS_ChannelIsActive(stream) != BASSActive.BASS_ACTIVE_STOPPED)
            {
                Bass.BASS_ChannelStop(stream);
            }
        }
        #endregion

        #region 播放相关属性
        /// <summary>
        /// 总时间--前音乐持续时间（单位秒）
        /// </summary>
        public double Duration
        {
            get { return Bass.BASS_ChannelBytes2Seconds(stream, Bass.BASS_ChannelGetLength(stream)); }
        }

        /// <summary>
        /// 当前播放进度
        /// </summary>
        public double CurrentPosition
        {
            get { return Bass.BASS_ChannelBytes2Seconds(stream, Bass.BASS_ChannelGetPosition(stream)); }
            set { Bass.BASS_ChannelSetPosition(stream, value); }
        }

        /// <summary>
        /// 音量调节
        /// </summary>
        public int Volume
        {
            get { return Bass.BASS_GetConfig(BASSConfig.BASS_CONFIG_GVOL_STREAM) / 100; }
            set { Bass.BASS_SetConfig(BASSConfig.BASS_CONFIG_GVOL_STREAM, value * 100); }
        }

        public bool IsPlay()
        {
            //switch (Bass.BASS_ChannelIsActive(stream))
            //{
            //    case Bass.BASS_ACTIVE_PAUSED: return PS_Paused;
            //    case Bass.BASS_ACTIVE_PLAYING: return PS_Playing;
            //    case Bass.BASS_ACTIVE_STOPPED: return PS_Stopped;
            //}
            if (Bass.BASS_ChannelIsActive(stream) == BASSActive.BASS_ACTIVE_PLAYING)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion


        /// <summary>
        /// 获取FFT采样数据，返回512个浮点采样数据
        /// </summary>
        /// <returns></returns>
        public float[] GetFFTData()
        {
            float[] fft = new float[colCount];
            Bass.BASS_ChannelGetData(stream, fft, (int)BASSData.BASS_DATA_FFT256);
            return fft;
        }
        public void DrawFFT()
        {
            int drawWidth = this.canvas.Width;
            int drawHeight = this.canvas.Height;

            int di;
            int w = 14;
            int[] FFTPeacks = new int[colCount];
            int[] FFTFallOff = new int[colCount];

            LinearGradientBrush lgb = new LinearGradientBrush(new Point(0, this.canvas.Height - 5), new Point(0, 0), Color.GreenYellow, Color.FromArgb(255,0,0,0));
            LinearGradientBrush lgb1 = new LinearGradientBrush(new Point(0, this.canvas.Height - 5), new Point(0, 0), Color.Yellow, Color.FromArgb(255, 0, 0, 0));
            SolidBrush sbGreenYellow = new SolidBrush(Color.GreenYellow);
            SolidBrush sbYellow = new SolidBrush(Color.Yellow);
            SolidBrush solidBrushline = new SolidBrush(Color.FromArgb(128, 50, 50, 50));
            SolidBrush solidBrushtitle = new SolidBrush(Color.FromArgb(48, 48, 48));

            Font songNameFont = new Font("微软雅黑", 11, FontStyle.Regular);
            Font myFont = new Font("微软雅黑", 45, FontStyle.Bold);
            Brush bush = new SolidBrush(Color.FromArgb(12, 12, 12));//填充的颜色

            while (true)
            {
                if (cts.Token.IsCancellationRequested)
                {
                    Console.WriteLine("线程被终止！");
                    break;
                }
                else
                {
                    Thread.Sleep(1);

                    gcanvas.Clear(Color.Black);

                    int lineLen = (int)Math.Floor(this.CurrentPosition * drawWidth / this.Duration);
                    gcanvas.FillRectangle(solidBrushline, 0, 0, lineLen, drawHeight);

                    gcanvas.DrawString("MP3播放器--刘光博", myFont, bush, 200, 100);

                    if (this.IsPlay() == false) continue;

                    float[] FFTDatas = this.GetFFTData();
                    for (int i = 0; i < FFTDatas.Length - 1; i++)
                    {
                        di = (int)(Math.Abs(FFTDatas[i]) * drawHeight * 10);
                        if (di > drawHeight) di = drawHeight - 5;

                        FFTPeacks[i] = (di >= FFTPeacks[i]) ? di : FFTPeacks[i] - 2;
                        FFTFallOff[i] = (di >= FFTFallOff[i]) ? di : FFTFallOff[i] - 5;

                        if ((drawHeight - FFTPeacks[i]) > drawHeight) FFTPeacks[i] = 0;
                        if ((drawHeight - FFTFallOff[i]) > drawHeight) FFTFallOff[i] = 0;

                        if (di >= FFTFallOff[i])
                        {
                            gcanvas.FillRectangle(sbYellow, i * (w + 1), drawHeight - FFTPeacks[i] + 5, w, 5);
                            gcanvas.FillRectangle(sbYellow, i * (w + 1), drawHeight - FFTFallOff[i], w, drawHeight);
                        }
                        else
                        {
                            gcanvas.FillRectangle(sbGreenYellow, i * (w + 1), drawHeight - FFTPeacks[i] + 5, w, 5);
                            gcanvas.FillRectangle(sbGreenYellow, i * (w + 1), drawHeight - FFTFallOff[i], w, drawHeight);
                        }
                    }

                    string title = string.Format("{0}  [{1}]", this.songName, ToTimeStr(this.Duration));
                    SizeF sizeF = gcanvas.MeasureString(title, songNameFont);
                    gcanvas.DrawString(title, songNameFont, solidBrushline, 10, 11);
                    gcanvas.DrawString(title, songNameFont, solidBrushtitle, 10, 10);

                    string playtime = ToTimeStr(this.CurrentPosition);
                    SizeF sizeFinfo = gcanvas.MeasureString(playtime, songNameFont);
                    gcanvas.DrawString(playtime, songNameFont, solidBrushline, drawWidth - sizeFinfo.Width - 30, 11);
                    gcanvas.DrawString(playtime, songNameFont, solidBrushtitle, drawWidth - sizeFinfo.Width - 30, 10);

                    gpicturebox.DrawImage(this.canvas, 0, 0);

                    //this.DataGridView.();
                }
            }
        }

        public String ToTimeStr(double second)
        {
            int iminute = 0;
            int isecond = 0;

            if (second > 60)
            {
                iminute = (int)(second / 60.0f);
            }

            isecond = (int)(second % 60.0f);
            return string.Format("{0}:{1}", iminute.ToString("00"), isecond.ToString("00"));
        }

        public void Dispose(object obj)
        {
            cts.Cancel();

            Bass.BASS_ChannelStop(stream);
            Bass.BASS_StreamFree(stream);
            Bass.BASS_Stop();    //停止
            Bass.BASS_Free();    //释放
            GC.SuppressFinalize(obj);
        }
    }
}
