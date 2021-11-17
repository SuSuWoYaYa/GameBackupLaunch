using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace TheHunter_BackupLaunch
{
    public partial class Form1 : Form
    {

        static string[] txtString;
        static string sourceDirectory;
        static string destinationDirectory;

        public Form1()
        {
            InitializeComponent();

            //添加图片后缀选择框,默认选择第一个
            this.comboBox1.Items.Add(".bmp");
            this.comboBox1.Items.Add(".jpg");
            this.comboBox1.Items.Add(".png");
            this.comboBox1.SelectedIndex = 0;
        }



        //文本文件选择
        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "文本文件(*.txt)|*.txt|所有文件|*.*";
            ofd.ValidateNames = true;
            ofd.CheckPathExists = true;
            ofd.CheckFileExists = true;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                string strFileName = ofd.FileName;
                //其他代码
                textBox1.Text = strFileName;
            }

        }

        //选择一个文件夹
        private string OpenFileFolder()
        {

            FolderBrowserDialog folder = new FolderBrowserDialog();
            folder.Description = "选择所有文件存放目录";
            string sPath = "";
            if (folder.ShowDialog() == DialogResult.OK)
            {

                sPath = folder.SelectedPath;
            }
            return sPath;
        }


        private void button2_Click(object sender, EventArgs e)
        {
            sourceDirectory = OpenFileFolder();
            textBox2.Text = sourceDirectory;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            destinationDirectory = OpenFileFolder();
            textBox3.Text = destinationDirectory;
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
                addFileExtension();

                CopyFile();
            }
            catch (Exception exception)
            {
                ShowInfo("遇到错误!请检查输入！！！");
            }





        }


        //给获取的工号按照选择添加后缀
        private void addFileExtension()
        {

            String extension;
            int index = comboBox1.SelectedIndex;

            switch (index)
            {
                case 0:
                    extension = ".bmp";
                    break;

                case 1:
                    extension = ".jpg";
                    break;

                case 2:
                    extension = ".png";
                    break;

                default:
                    extension = ".bmp";
                    break;
            }

            for (int i = 0; i < txtString.Length; i++)
            {
                txtString[i] += extension;
            }


            //foreach (string s in txtString)
            //{
            //    ShowInfo(s);
            //}
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
                ShowInfo("先选择工号文本文件");
                return false;
            }

            if (textBox2.Text.Equals(""))
            {
                ShowInfo("先选择原始照片路径");
                return false;
            }
            sourceDirectory = textBox2.Text;


            if (textBox3.Text.Equals(textBox2.Text))
            {
                ShowInfo("请选择一个不同的路径保存复制的图片");
                return false;
            }


            if (textBox3.Text.Equals(""))
            {
                ShowInfo("再选择照片保存路径");
                return false;
            }
            destinationDirectory = textBox3.Text;
            return true;
        }





    }
}
