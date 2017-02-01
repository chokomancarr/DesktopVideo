using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace DesktopMovie1
{
    public partial class DesktopVideo : Form
    {
        public Form1 f1;
        public bool playing;
        public bool isWeb = false;

        public DesktopVideo()
        {
            InitializeComponent();
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Resize += DesktopVideo_Resize;
            int i = 0;
            foreach (Screen sc in Screen.AllScreens)
            {
                comboBox1.Items.Add("Screen" + ++i);
            }
        }

        public void ReadDefault()
        {
            comboBox1.SelectedIndex = 0;

            string path = AppDomain.CurrentDomain.BaseDirectory + "startup.txt";
            Debug.WriteLine("reading " + path);
            if (File.Exists(path))
            {
                string dat = File.ReadAllText(path);
                string[] dat2 = dat.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                int i = 0;
                foreach (string s in dat2)
                {
                    if (s.Length > 5 && s.StartsWith("type="))
                    {
                        if (s[5] == '0')
                            isWeb = false;
                        else if (s[5] == '1')
                            isWeb = true;

                        i++;
                    }
                    else if (s.Length > 4 && s.StartsWith("src="))
                    {
                        textBox1.Text = s.Substring(4, s.Length - 4);
                        i++;
                    }
                }
                if (i > 1)
                {
                    button1_Click(null, new EventArgs());
                }
            }
        }

        void DesktopVideo_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
                Hide();
        }

        private void DesktopVideo_FormClosed(object sender, FormClosedEventArgs e)
        {
            f1.Close();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            isWeb = !((RadioButton)sender).Checked;
            UpdateDesc();
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            isWeb = ((RadioButton)sender).Checked;
            UpdateDesc();
        }

        void UpdateDesc()
        {
            if (isWeb)
            {
                localDesc.Visible = false;
                ytDesc.Visible = true;
            }
            else
            {
                localDesc.Visible = true;
                ytDesc.Visible = false;
            }
            browseButton.Enabled = !isWeb;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Debug.WriteLine("starting...");
            if (isWeb)
            {
                int i = textBox1.Text.IndexOf("watch?v=");
                int i2 = textBox1.Text.IndexOf('&');
                if (i == -1)
                    textBox1.ForeColor = Color.Red;
                else
                {
                    f1.StopLocalVideo();
                    f1.StopYTVideo();
                    f1.StartYTVideo(textBox1.Text.Substring(i + 8, ((i2 > i) ? i2 : textBox1.Text.Length) - i - 8));
                }
            }
            else
            {
                if (File.Exists(textBox1.Text))
                {
                    f1.StopLocalVideo();
                    f1.StopYTVideo();
                    f1.StartLocalVideo(textBox1.Text);
                }
                else
                {
                    textBox1.ForeColor = Color.Red;
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            f1.StopLocalVideo();
            f1.StopYTVideo();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            textBox1.ForeColor = Color.Black;
        }

        private void DesktopVideo_ResizeEnd(object sender, EventArgs e) { }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e) { }

        private void contextMenuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e) { }

        private void restoreToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void DesktopVideo_Load(object sender, EventArgs e) { }

        private void browseButton_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog1.ShowDialog(); // Show the dialog.
            if (result == DialogResult.OK) // Test result.
            {
                textBox1.Text = openFileDialog1.FileName;
                textBox1.ForeColor = Color.Black;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Rectangle r = Screen.AllScreens[comboBox1.SelectedIndex].Bounds;
            f1.Location = new Point(r.X, r.Y);
            f1.Size = new Size(r.Width, r.Height);
        }
    }
}
