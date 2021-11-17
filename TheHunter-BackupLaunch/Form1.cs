using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;


//读写ini
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace TheHunter_BackupLaunch
{
    public partial class Form1 : Form
    {
        //ini文件名
        static string IniConfigFileName = "Launcher-config.ini";
        //ini字段
        static string IniBackConfig = "BackConfig";
        static string IniGameSavePath = "GameSavePath";
        static string IniBackupPath = "BackupPath";
        static string IniGameFilePath = "GameFilePath";
        static string IniConfigFilePath = "";

        static string RunGameCommand = "stem:\\";   //steam命令
        static string GameMainFilePath = "";    //游戏文件路径
        static string GameProcessName = "360chrome";
        static string[] txtString;
        static string SourceDirectoryPath = "";
        static string BackupDirectoryPath = "";
        static string MyMail = "Power By cuisanzhang@163.com";
        

        public Form1()
        {
            InitializeComponent();
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            toolTip1.SetToolTip(label4, MyMail);

            //默认steam启动
            checkBox2.Checked = true;
            label3.Visible = false;
            textBox3.Visible = false;
            button3.Visible = false;

            //创建ini配置文件
            string path = System.Environment.CurrentDirectory;
            IniConfigFilePath = Path.Combine(path, IniConfigFileName);  
            IniHelper.Ini_Create(IniConfigFilePath);
            //ShowInfo(IniConfigFilePath);
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
            

            SourceDirectoryPath = OpenFileFolder("选择游戏存档文件夹");
            textBox1.Text = SourceDirectoryPath;
            IniHelper.Ini_Write(IniBackConfig, IniGameSavePath, SourceDirectoryPath, IniConfigFilePath);

        }

        //选择存档备份文件夹
        private void button2_Click(object sender, EventArgs e)
        {
            BackupDirectoryPath = OpenFileFolder("选择存档备份文件夹");
            textBox2.Text = BackupDirectoryPath;
            IniHelper.Ini_Write(IniBackConfig, IniBackupPath, BackupDirectoryPath, IniConfigFilePath);
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
                GameMainFilePath = strFileName;
                IniHelper.Ini_Write(IniBackConfig, IniGameFilePath, GameMainFilePath, IniConfigFilePath);
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
            if (BackupDirectoryPath.Equals(""))
            {
                ShowInfo("没有设置存档备份文件夹");
            }
            else
            {
                System.Diagnostics.Process.Start(BackupDirectoryPath);
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
                string sourcefilepath = Path.Combine(SourceDirectoryPath, file);
                //拼接目标文件路径
                string destinationfilepath = Path.Combine(BackupDirectoryPath, file);


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
            SourceDirectoryPath = textBox1.Text;

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
            BackupDirectoryPath = textBox2.Text;

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
                RunGameCommand = IniGameFilePath;
                label3.Visible = true;
                textBox3.Visible = true;
                button3.Visible = true;
            }
        }





        //https://blog.csdn.net/baidu_26678247/article/details/78254876?utm_medium=distribute.pc_relevant.none-task-blog-2~default~baidujs_baidulandingword~default-1.no_search_link&spm=1001.2101.3001.4242.2
        /// <summary>
        /// C#类库：ini文件操作类
        /// </summary>
        public class IniHelper
        {
            #region 动态链接库调用
            /// <summary>
            /// 调用动态链接库读取值
            /// </summary>
            /// <param name="lpAppName">ini节名</param>
            /// <param name="lpKeyName">ini键名</param>
            /// <param name="lpDefault">默认值：当无对应键值，则返回该值。</param>
            /// <param name="lpReturnedString">结果缓冲区</param>
            /// <param name="nSize">结果缓冲区大小</param>
            /// <param name="lpFileName">ini文件位置</param>
            /// <returns></returns>
            [DllImport("kernel32")]
            private static extern int GetPrivateProfileString(
                string lpAppName,
                string lpKeyName,
                string lpDefault,
                StringBuilder lpReturnedString,
                int nSize,
                string lpFileName);

            /// <summary>
            /// 调用动态链接库写入值
            /// </summary>
            /// <param name="mpAppName">ini节名</param>
            /// <param name="mpKeyName">ini键名</param>
            /// <param name="mpDefault">写入值</param>
            /// <param name="mpFileName">文件位置</param>
            /// <returns>0：写入失败 1：写入成功</returns>
            [DllImport("kernel32")]
            private static extern long WritePrivateProfileString(
                string mpAppName,
                string mpKeyName,
                string mpDefault,
                string mpFileName);
            #endregion

            /// <summary>
            /// 读ini文件
            /// </summary>
            /// <param name="section">节</param>
            /// <param name="key">键</param>
            /// <returns>返回读取值</returns>
            public static string Ini_Read(string section, string key, string path)
            {
                StringBuilder stringBuilder = new StringBuilder(1024);                  //定义一个最大长度为1024的可变字符串
                GetPrivateProfileString(section, key, "", stringBuilder, 1024, path);   //读取INI文件
                return stringBuilder.ToString();                                        //返回INI文件的内容
            }

            /// <summary>
            /// 写ini文件
            /// </summary>
            /// <param name="section">节</param>
            /// <param name="key">键</param>
            /// <param name="iValue">待写入值</param>
            public static void Ini_Write(string section, string key, string iValue, string path)
            {
                WritePrivateProfileString(section, key, iValue, path);    //写入
            }

            /// <summary>
            /// 根据文件名创建文件
            /// </summary>
            /// <param name="path">文件名称以及路径</param>
            public static void Ini_Create(string path)
            {
                if (!File.Exists(path))                             //判断是否存在相关文件
                {
                    FileStream _fs = File.Create(path);               //不存在则创建ini文件
                    _fs.Close();                                    //关闭文件，解除占用
                }
            }

            /// <summary>
            /// 删除ini文件中键
            /// </summary>
            /// <param name="section">节名称</param>
            /// <param name="key">键名称</param>
            /// <param name="path">ini文件路径</param>
            public static void Ini_Del_Key(string section, string key, string path)
            {
                WritePrivateProfileString(section, key, null, path);                          //写入
            }

            /// <summary>
            /// 删除ini文件中节
            /// </summary>
            /// <param name="section">节名</param>
            /// <param name="path">ini文件路径</param>
            public static void Ini_Del_Section(string section, string path)
            {
                WritePrivateProfileString(section, null, null, path);                          //写入
            }


            //        3.类库使用
            //  #region INI操作类测试
            //string _str_path = Path.Combine(@"G:", "ini_test.ini");             //路径
            //Console.WriteLine("————Create File:");
            //IniHelper.Ini_Create(_str_path);                                    //在G盘创建名为ini_test的ini文件

            //Console.WriteLine("————Write Name:");
            //IniHelper.Ini_Write("INFO", "Name", "Test", _str_path);             //在路径文件中写入节为“INFO”，键名为“Name”，键值为“SWorld”的数据
            //Console.WriteLine("————Write Age:");
            //IniHelper.Ini_Write("INFO", "Age", "0", _str_path);                 //在路径文件中写入节为“INFO”，键名为“Age”，键值为“0”的数据

            //Console.WriteLine("————Read Name:");
            //string _str_Name = IniHelper.Ini_Read("INFO", "Name", _str_path);   //读取Name
            //Console.WriteLine("Name:"+_str_Name);                               //打印Name
            //Console.WriteLine("————Read Age:");
            //string _str_Age = IniHelper.Ini_Read("INFO", "Age", _str_path);     //读取Age
            //Console.WriteLine("Age:" + _str_Age);                               //打印Age

            //Console.WriteLine("————Update Name:");
            //IniHelper.Ini_Write("INFO", "Name", "SWorld", _str_path);           //更新数据
            //Console.WriteLine("————Read New Name:");
            //_str_Name = IniHelper.Ini_Read("INFO", "Name", _str_path);          //读取Name
            //Console.WriteLine("Name:" + _str_Name);                             //打印Name

            //Console.WriteLine("————Delete Key Name:");
            //IniHelper.Ini_Del_Key("INFO", "Name", _str_path);                   //删除键
            //Console.WriteLine("————Read Name And Age:");
            //_str_Name = IniHelper.Ini_Read("INFO", "Name", _str_path);          //读取Name
            //Console.WriteLine("Name:" + _str_Name);                             //打印Name
            //_str_Age = IniHelper.Ini_Read("INFO", "Age", _str_path);            //读取Age
            //Console.WriteLine("Age:" + _str_Age);                               //打印Age

            //Console.WriteLine("————Delete Section Info:");
            //IniHelper.Ini_Del_Section("INFO", _str_path);                       //删除节
            //Console.WriteLine("————Read Name And Age:");
            //_str_Name = IniHelper.Ini_Read("INFO", "Name", _str_path);          //读取Name
            //Console.WriteLine("Name:" + _str_Name);                             //打印Name
            //_str_Age = IniHelper.Ini_Read("INFO", "Age", _str_path);            //读取Age
            //Console.WriteLine("Age:" + _str_Age);                               //打印Name
            //#endregion
        }
 
    }



}
