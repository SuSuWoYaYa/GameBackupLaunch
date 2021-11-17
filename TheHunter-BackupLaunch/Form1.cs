using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace TheHunter_BackupLaunch
{
    public partial class Form1 : Form
    {
        static string RunGameCommand = "stem:\\";
        static string GameFilePath = "";
        static string GameProcessName = "360chrome";
        static string[] txtString;
        static string SourceDirectory = "";
        static string BackupDirectory = "";

        public Form1()
        {
            InitializeComponent();
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            toolTip1.SetToolTip(label4, "Power By cuisanzhang@163.com");

            //默认steam启动
            checkBox2.Checked = true;
            label3.Visible = false;
            textBox3.Visible = false;
            button3.Visible = false;
        }



        //文件夹选择对话框
        private string OpenFileFolder(string openDescription)
        {

            FolderBrowserDialog folder = new FolderBrowserDialog();
            folder.Description = openDescription;
            string sPath = "";
            if (folder.ShowDialog() == DialogResult.OK)
            {

                sPath = folder.SelectedPath;
            }
            return sPath;
        }


        //选择游戏存档文件夹
        private void button1_Click(object sender, EventArgs e)
        {
            

            SourceDirectory = OpenFileFolder("选择游戏存档文件夹");
            textBox1.Text = SourceDirectory;
        }

        //选择存档备份文件夹
        private void button2_Click(object sender, EventArgs e)
        {
            BackupDirectory = OpenFileFolder("选择存档备份文件夹");
            textBox2.Text = BackupDirectory;
        }

        //选择游戏启动文件
        private void button3_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "游戏启动文件(*.exe)|*.exe|所有文件|*.*";
            ofd.ValidateNames = true;
            ofd.CheckPathExists = true;
            ofd.CheckFileExists = true;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
            string strFileName = ofd.FileName;
                 //获取路径
                textBox3.Text = strFileName;
                GameFilePath = strFileName;
           }
        }

        private void button6_Click(object sender, EventArgs e)
        {

            if (checkAllInput() == false)//检查路径
            {
                return;

            }
            else if (isGameRuning())//检测进程
            {
                MessageBox.Show("检测到进程,游戏正在运行中,请先停止游戏");
            }
          
            else
            {
                //启动游戏
                Process.Start(RunGameCommand);
                Application.Exit();
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (BackupDirectory.Equals(""))
            {
                ShowInfo("没有设置存档备份文件夹");
            }
            else
            {
                System.Diagnostics.Process.Start(BackupDirectory);
            }
        }




        //开始拷贝对应的工号图片到目标文件夹
        private void CopyFile()
        {
            ShowInfo("开始拷贝图片");

            int count = 0;
            foreach (string file in txtString)
            {
                //拼接原始文件路径
                string sourcefilepath = Path.Combine(SourceDirectory, file);
                //拼接目标文件路径
                string destinationfilepath = Path.Combine(BackupDirectory, file);


                if (File.Exists(sourcefilepath))
                {
                    //原始文件存在,开始拷贝，已存在直接覆盖
                    File.Copy(sourcefilepath, destinationfilepath, true);
                    count++;
                }
                else
                {
                    //原始文件不存在
                    ShowInfo("错误" + sourcefilepath + " 该文件不存在.");
                }

            }
            ShowInfo("完成,共拷贝图片数量" + count);
        }

        //读取工号txt文件到数组中，每行一个工号
        private int ReadTxtFile(string fileName)
        {
            int count = 0;


            txtString = File.ReadAllLines(fileName, Encoding.ASCII);
            count = txtString.Length;
            return count;
        }



        //显示消息
        private void ShowInfo(string msg)
        {
            msg += "\r\n";
            textBox4.Text = msg + textBox4.Text;
        }

        //检查所有输入
        private Boolean checkAllInput()
        {

            if (textBox1.Text.Equals(""))
            {
                ShowInfo("选择游戏存档文件夹");
                return false;
            }
            SourceDirectory = textBox1.Text;

            if (textBox2.Text.Equals(""))
            {
                ShowInfo("选择存档备份文件夹");
                return false;
            }


            if (textBox1.Text.Equals(textBox2.Text))
            {
                ShowInfo("请选择一个不同的路径保存存档");
                return false;
            }
            BackupDirectory = textBox2.Text;

            return true;
        }





        //检测游戏运行进程
        bool isGameRuning()
        {
            Process[] ps = Process.GetProcesses();
            foreach (Process p in ps)
            {
                string info = "";
                try
                {
                    info =p.ProcessName;
                    if (GameProcessName.Equals(info))
                    {
                        ShowInfo("检测到进程" + info);
                        return true;
                    }
                }
                catch (Exception e)
                {
                    info = e.Message;
                }
              
            }
            return false;
        }

        //选择启动方式
        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
            {
                RunGameCommand = "stem:\\";
                label3.Visible = false;
                textBox3.Visible = false;
                button3.Visible = false; 
            }
            else
            {
                RunGameCommand = GameFilePath;
                label3.Visible = true;
                textBox3.Visible = true;
                button3.Visible = true;
            }
        }



 
    }
}
