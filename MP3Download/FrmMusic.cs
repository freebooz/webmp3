using MP3Download.MusicSource;
using MP3Download.MusicPlayer;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace MP3Download
{
    public partial class FrmMusic : Form
    {
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
        [DllImport("user32.dll")]
        public static extern bool SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);
        private const int VM_NCLBUTTONDOWN = 0XA1;//定义鼠标左键按下
        private const int HTCAPTION = 2;

        private Mp3Player Mp3Player;
        private Music_Source Music_Source;
        private string dir = @"c:\Users\Administrator\Music\";

        public FrmMusic()
        {
            this.DoubleBuffered = true;
            InitializeComponent();
            
            this.label1.Text = dir;
            this.dataGridView1.MouseWheel += DataGridView1_MouseWheel;

            this.Mp3Player = new Mp3Player(this.Handle, this.pictureBox1);
            this.Music_Source = new Music_Source();
            this.Music_Source.OnMuicFinded += Music_Source_OnMuicFinded;
        }

        private void Music_Source_OnMuicFinded(object sender, MusicSourceInfoEvrg e)
        {
            this.Invoke(new Action(()=> {

                foreach (MusicSourceInfo item in e.MusicSourceInfoList)
                {
                    int row = dataGridView1.Rows.Add();                    
                    dataGridView1.Rows[row].Cells["SoureName"].Value = item.SoureName;
                    dataGridView1.Rows[row].Cells["SongName"].Value = item.SongName;
                    dataGridView1.Rows[row].Cells["SingerName"].Value = item.SingerName;
                    dataGridView1.Rows[row].Cells["Quality"].Value = item.QualityLevel;                    
                    dataGridView1.Rows[row].Cells["ExtName"].Value = item.ExtName;
                    dataGridView1.Rows[row].Cells["OwnerCount"].Value = item.OwnerCount.ToString();
                    dataGridView1.Rows[row].Tag = item;
                }

            }));
        }

        private void DataGridView1_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta > 0)
            {
                if (this.dataGridView1.FirstDisplayedScrollingRowIndex - 5 < 0)
                {
                    this.dataGridView1.FirstDisplayedScrollingRowIndex = 0;
                }
                else
                {
                    this.dataGridView1.FirstDisplayedScrollingRowIndex = this.dataGridView1.FirstDisplayedScrollingRowIndex - 5;
                }
            }
            else
            {
                this.dataGridView1.FirstDisplayedScrollingRowIndex = this.dataGridView1.FirstDisplayedScrollingRowIndex + 5;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Music_Source.Search(textBox1.Text);
            dataGridView1.Rows.Clear();
            dataGridView1.AutoGenerateColumns = false;
        }

        private void play(MusicSourceInfo info)
        {
            string filename = this.Music_Source.Download(info, dir);
            this.Mp3Player.play(filename);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.SelectedPath = dir;
            dialog.Description = "请选择文件路径";

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                dir = dialog.SelectedPath;
                this.label1.Text = dir;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Mp3Player.stop();
        }
        private void button3_Click(object sender, EventArgs e)
        {
            this.Mp3Player.Puase();
        }
        private void button5_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                MusicSourceInfo info = dataGridView1.SelectedRows[0].Tag as MusicSourceInfo;
                this.play(info);
            }
        }
        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            //为当前应用程序释放鼠标捕获
            ReleaseCapture();
            //发送消息 让系统误以为在标题栏上按下鼠标
            SendMessage((IntPtr)this.Handle, VM_NCLBUTTONDOWN, HTCAPTION, 0);
        }

        private void FrmPlayer_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Mp3Player.Dispose(this);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
