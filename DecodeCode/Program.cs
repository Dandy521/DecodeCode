using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecodeCode
{
    class Program
    {
        static void Main(string[] args)
        {
            string dir = @"G:\miui";
            if (args.Length > 0)
                dir = args[0].ToString();
            else
            {
                Console.WriteLine("请输入路径");
                dir = Console.ReadLine();
            }
            if (!Directory.Exists(dir)&& !File.Exists(dir))
            {
                Console.WriteLine("路径不存在！");
            }
            else
            {
                var dirInfo = new DirectoryInfo(dir);
                if (dirInfo.Root.Name==dirInfo.Name)
                {
                    Console.WriteLine("不支持根目录！");
                }else if(Directory.Exists(dir))
                {
                    DecodeDir(Path.GetFullPath(dir));
                }else if (File.Exists(dir))
                {
                    DecodeFile(Path.GetFullPath(dir));
                }
            }
            Console.ReadKey();
        }
        static void DecodeDir(string dirStr)
        {
            string newDir = dirStr + "_new";
            string[] files = Directory.GetFiles(dirStr, "*.*", SearchOption.AllDirectories);
            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.UseShellExecute = false;    //是否使用操作系统shell启动
            p.StartInfo.RedirectStandardInput = true;//接受来自调用程序的输入信息
            p.StartInfo.RedirectStandardOutput = false;//不需要获取输出信息
            p.StartInfo.RedirectStandardError = true;//不需要重定向标准错误输出,但false时下面的指令未执行
            p.StartInfo.CreateNoWindow = true;//不显示程序窗口
            p.Start();//启动程序
            StreamWriter mySR = p.StandardInput;
            string newFilePath = null;
            string newFileDir = null;
            Console.WriteLine("开始复制处理。。。");
            int j = 0;
            for (int nowIdx = 0; nowIdx < files.Length; nowIdx++)
            {
                if(((1+nowIdx)*10.0 / files.Length) >j)
                {
                    j++;
                    Console.WriteLine(string.Format("{0:P1}", (nowIdx + 1.0f) / files.Length));
                }
                newFilePath = newDir+ files[nowIdx].Replace(dirStr, "")+"_dandy";
                newFileDir = Path.GetDirectoryName(newFilePath);
                if (!Directory.Exists(newFileDir))
                    Directory.CreateDirectory(newFileDir);
                mySR.WriteLine($"copy \"{files[nowIdx]}\" \"{newFilePath}\" /y");
                //str表示一行命令
                //mySR.WriteLine(str[nowIdx]);
                //这里的Flush（）操作和我理解的不一样，尽管flush了但是命令行并没有立即执行
                //而且当需要执行的命令行数量十分多的时候，在使用了WriteLine后会在随机行数之后值执行，可能与缓冲区大小有关
                mySR.Flush();
            }
            //这个地方我对于原文所提供的exit和&exit的区别没有完全弄清，以后可能进行补充
            p.StandardInput.WriteLine("&exit");
            //关闭StreamWriter后，之前输入的命令行会立即执行
            mySR.Close();
            p.WaitForExit();
            p.Close();
            Console.WriteLine("开始重命名处理。。。");
            string[] newfiles = Directory.GetFiles(newDir,"*.*",SearchOption.AllDirectories);
            foreach(var nf in newfiles)
            {
                 File.Move(nf,nf.Substring(0,nf.LastIndexOf("_dandy")));
            }
            Console.WriteLine("处理完成！");
        }


        static void DecodeFile(string pathStr)
        {
            string newFile = Path.GetDirectoryName(pathStr)+"\\"+"new_"+ Path.GetFileName(pathStr) + "_dandy"; 
            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.UseShellExecute = false;    //是否使用操作系统shell启动
            p.StartInfo.RedirectStandardInput = true;//接受来自调用程序的输入信息
            p.StartInfo.RedirectStandardOutput = false;//不需要获取输出信息
            p.StartInfo.RedirectStandardError = true;//不需要重定向标准错误输出,但false时下面的指令未执行
            p.StartInfo.CreateNoWindow = true;//不显示程序窗口
            p.Start();//启动程序
            StreamWriter mySR = p.StandardInput;
            Console.WriteLine("开始复制处理。。。");
            mySR.WriteLine($"copy \"{pathStr}\" \"{newFile}\" /y");
            //str表示一行命令
            //mySR.WriteLine(str[nowIdx]);
            //这里的Flush（）操作和我理解的不一样，尽管flush了但是命令行并没有立即执行
            //而且当需要执行的命令行数量十分多的时候，在使用了WriteLine后会在随机行数之后值执行，可能与缓冲区大小有关
            mySR.Flush();
            //这个地方我对于原文所提供的exit和&exit的区别没有完全弄清，以后可能进行补充
            p.StandardInput.WriteLine("&exit");
            //关闭StreamWriter后，之前输入的命令行会立即执行
            mySR.Close();
            p.WaitForExit();
            p.Close();
            Console.WriteLine("开始重命名处理。。。");
            File.Move(newFile, newFile.Substring(0, newFile.LastIndexOf("_dandy")));
            Console.WriteLine("处理完成！");
        }
    }
}
