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
//using System.IO;
using System.Runtime.InteropServices;
//using System.Text;

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
        static string IniStartWithSteam = "StartWithSteam";
        static string IniExitWhenStartGame = "ExitWhenStartGame";
        static string IniConfigFilePath = "";

        static string RunGameCommand = "";   //steam命令
        static string SteamCommand = "stem:\\";   //steam命令
        static string GameMainFilePath = "";    //游戏文件路径
        static string GameProcessName = ""; //游戏进程名称
       //
        static string SourceDirectoryPath = "";
        static string BackupDirectoryPath = "";
        static string MyMail = "Power By cuisanzhang@163.com";
        static string StartWithSteam = "true";
        static string ExitWhenStartGame = "true";

        public Form1()
        {
            InitializeComponent();
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            toolTip1.SetToolTip(label4, MyMail);



            //创建ini配置文件
            string path = System.Environment.CurrentDirectory;
            IniConfigFilePath = Path.Combine(path, IniConfigFileName);
            IniHelper.Ini_Create(IniConfigFilePath);
            //ShowInfo(IniConfigFilePath);

            //读取ini配置
            SourceDirectoryPath = IniHelper.Ini_Read(IniBackConfig, IniGameSavePath, IniConfigFilePath);
            BackupDirectoryPath = IniHelper.Ini_Read(IniBackConfig, IniBackupPath, IniConfigFilePath);
            GameMainFilePath = IniHelper.Ini_Read(IniBackConfig, IniGameFilePath, IniConfigFilePath);
            StartWithSteam = IniHelper.Ini_Read(IniBackConfig, IniExitWhenStartGame, IniConfigFilePath);
            ExitWhenStartGame = IniHelper.Ini_Read(IniBackConfig, IniExitWhenStartGame, IniConfigFilePath);

            //设置显示界面
            textBox1.Text = SourceDirectoryPath;
            textBox2.Text = BackupDirectoryPath;
            textBox3.Text = GameMainFilePath;

            //默认steam启动
            if (StartWithSteam.Equals("") || StartWithSteam.Equals("false"))
            {
                RunGameCommand = GameMainFilePath; // .exe启动
                label3.Visible = true;
                textBox3.Visible = true;
                button3.Visible = true;
            }
            else
            {
                checkBox3.Checked = true;
                RunGameCommand = SteamCommand; // steam::\\启动
                label3.Visible = false;
                textBox3.Visible = false;
                button3.Visible = false; 
            }



            //默认启动游戏后自动退出本程序
            if (ExitWhenStartGame.Equals("") || ExitWhenStartGame.Equals("true"))
            {
                checkBox2.Checked = true;
                IniHelper.Ini_Write(IniBackConfig, IniExitWhenStartGame, ExitWhenStartGame, IniConfigFilePath);
            }
            else
            {
                checkBox3.Checked = false;
                IniHelper.Ini_Write(IniBackConfig, IniExitWhenStartGame, ExitWhenStartGame, IniConfigFilePath);
            }
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


        //显示存档备份文件夹
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


        //开始游戏按钮
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
                ShowInfo("============================================");
                ShowInfo("开始备份");
                BackAllFiles();//开始备份存档
                ShowInfo("备份成功");

                //启动游戏
                ShowInfo("启动游戏中..."); 
                try
                {

                    Process.Start(RunGameCommand);
                    ShowInfo("游戏启动完成.");
                    ShowInfo("============================================");
                    if(ExitWhenStartGame.Equals("true"))
                    {
                        Application.Exit();
                    }
                }
                catch (Exception ex)
                {
                    //启动游戏
                    MessageBox.Show("启动游戏失败");
                    
                    ShowInfo("启动游戏失败,当前游戏启动命令为");
                    ShowInfo("---------------------------------------------");
                    ShowInfo(RunGameCommand);
                    ShowInfo("---------------------------------------------");
                    ShowInfo("如果你没有安装steam正版, 不要勾选'使用steam命令启动游戏'");
                    ShowInfo("你可以不要勾选'使用steam命令启动游戏'.设置游戏启动路径后再试试," + ex.Message);
                    ShowInfo("讲道理, 这个小工具可以设置成备份任何游戏存档...");
                    ShowInfo("毕竟它的设计目标就是复制存档文件夹然后启动游戏...");
                    ShowInfo("这个小工具应该不会有BUG, 如果有那肯定是你电脑坏了,毕竟我测试了好几次,木有问题...");
                }
                
                
            }
        }

      


        //显示消息在文本框中
        private void ShowInfo(string msg)
        {
            msg += "\r\n";
            //textBox4.Text = msg + textBox4.Text;
            textBox4.Text += msg;
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

            if ((checkBox3.Checked == false) && (textBox3.Text.Equals("")))
            {
                ShowInfo("没有设置游戏文件路径");
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



        //生成一个新的文件夹名称,按 '年-月-日-时-分-秒' 添加
        string GetNewFolderName(string sourceFolderPath)
        {
            string newFolderName = "";
            newFolderName = DateTime.Now.ToString("-yyyy-MM-dd-HH-mm-ss");

            string sourceFolder = "";
            sourceFolder = Path.GetFileName(sourceFolderPath);

            return sourceFolder + newFolderName;
        }



        //开始备份存档
        void BackAllFiles()
        {
            //ShowInfo(GetNewFolderName(SourceDirectoryPath));
            string NewFolder = "";
            NewFolder = GetNewFolderName(SourceDirectoryPath);

            //生成备份位置绝对路径
            string NewFolderFullPath = "";
            NewFolderFullPath = Path.Combine(BackupDirectoryPath, NewFolder);

            ShowInfo("生成备份目标路径 " + NewFolderFullPath);


            ShowInfo("开始拷贝文件");
            //复制文件夹
            CopyFolder(SourceDirectoryPath, NewFolderFullPath);
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


        //Ini启动游戏后是否退出
        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
            {
                ExitWhenStartGame = "true";
                IniHelper.Ini_Write(IniBackConfig, IniExitWhenStartGame, ExitWhenStartGame, IniConfigFilePath);
            }
            else
            {
                ExitWhenStartGame = "false";
                IniHelper.Ini_Write(IniBackConfig, IniExitWhenStartGame, ExitWhenStartGame, IniConfigFilePath);
            }
        }



        //Ini选择启动方式
        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox3.Checked)
            {
                RunGameCommand = SteamCommand; // steam::\\启动
                label3.Visible = false;
                textBox3.Visible = false;
                button3.Visible = false;

                StartWithSteam = "true";
                IniHelper.Ini_Write(IniBackConfig, IniStartWithSteam, StartWithSteam, IniConfigFilePath);
            }
            else
            {
                RunGameCommand = GameMainFilePath; // .exe启动
                label3.Visible = true;
                textBox3.Visible = true;
                button3.Visible = true;

                StartWithSteam = "false";
                IniHelper.Ini_Write(IniBackConfig, IniStartWithSteam, StartWithSteam, IniConfigFilePath);
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






        //https://www.cnblogs.com/fps2tao/p/14965561.html
        //C#中复制文件夹及文件的两种方法
        //方法一:
        /// <summary>
        /// 复制文件夹及文件
        /// </summary>
        /// <param name="sourceFolder">原文件路径</param>
        /// <param name="destFolder">目标文件路径</param>
        /// <returns></returns>
        public int CopyFolder(string sourceFolder, string destFolder)
        {
            try
            {
                //如果目标路径不存在,则创建目标路径
                if (!System.IO.Directory.Exists(destFolder))
                {
                    System.IO.Directory.CreateDirectory(destFolder);
                }
                //得到原文件根目录下的所有文件
                string[] files = System.IO.Directory.GetFiles(sourceFolder);
                foreach (string file in files)
                {
                    string name = System.IO.Path.GetFileName(file);
                    string dest = System.IO.Path.Combine(destFolder, name);
                    System.IO.File.Copy(file, dest);//复制文件
                }
                //得到原文件根目录下的所有文件夹
                string[] folders = System.IO.Directory.GetDirectories(sourceFolder);
                foreach (string folder in folders)
                {
                    string name = System.IO.Path.GetFileName(folder);
                    string dest = System.IO.Path.Combine(destFolder, name);
                    CopyFolder(folder, dest);//构建目标路径,递归复制文件
                }
                return 1;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                ShowInfo("备份文件时出错,请尝试更改设置");
                return 0;
            }

        }




        //C#中实现文本框的滚动条自动滚到最底端
        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            textBox4.SelectionStart = textBox4.Text.Length;
            textBox4.ScrollToCaret();

            //————————————————
            //版权声明：本文为CSDN博主「fml927」的原创文章，遵循CC 4.0 BY-SA版权协议，转载请附上原文出处链接及本声明。
            //原文链接：https://blog.csdn.net/fml927/article/details/3940525

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            
            GameMainFilePath = textBox3.Text;
            IniHelper.Ini_Write(IniBackConfig, IniGameFilePath, GameMainFilePath, IniConfigFilePath);
        }

  
    }



}
