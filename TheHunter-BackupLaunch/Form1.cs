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
        static string GameProcessName = "360chrome";
        static string[] txtString;
        static string sourceDirectory;
        static string destinationDirectory;

        public Form1()
        {
            InitializeComponent();

            //添加图片后缀选择框,默认选择第一个
            //this.comboBox1.Items.Add(".bmp");
            //this.comboBox1.Items.Add(".jpg");
            //this.comboBox1.Items.Add(".png");
            //this.comboBox1.SelectedIndex = 0;
        }


        private void Form1_Load(object sender, EventArgs e)
        {

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
            //OpenFileDialog ofd = new OpenFileDialog();
            //ofd.Filter = "文本文件(*.txt)|*.txt|所有文件|*.*";
            //ofd.ValidateNames = true;
            //ofd.CheckPathExists = true;
            //ofd.CheckFileExists = true;
            //if (ofd.ShowDialog() == DialogResult.OK)
            //{
            //    string strFileName = ofd.FileName;
            //    //其他代码
            //    textBox1.Text = strFileName;
            //}

            sourceDirectory = OpenFileFolder("选择游戏存档文件夹");
            textBox1.Text = sourceDirectory;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            sourceDirectory = OpenFileFolder("选择存档备份文件夹");
            textBox2.Text = sourceDirectory;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (isGameRuning())
            {
                MessageBox.Show("检测到进程,游戏正在运行中,请先停止游戏");
            }
            else
            {
                Application.Exit();
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (checkAllInput() == false)
            {
                return;

            }

            try
            {
                int count = ReadTxtFile(textBox1.Text);
                //foreach (string s in txtString)
                //{
                //    ShowInfo(s);
                //}
                ShowInfo("从txt共读取数据" + count + "条");
 

                CopyFile();
            }
            catch (Exception exception)
            {
                ShowInfo("遇到错误!请检查输入！！！" + exception);
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
                string sourcefilepath = Path.Combine(sourceDirectory, file);
                //拼接目标文件路径
                string destinationfilepath = Path.Combine(destinationDirectory, file);


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
            sourceDirectory = textBox1.Text;

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
            destinationDirectory = textBox2.Text;

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
                    if(GameProcessName.Equals(info))
                        return true;
                }
                catch (Exception e)
                {
                    info = e.Message;
                }
                ShowInfo("检测进程" + info);
            }
            return false;
        }
    }
}
