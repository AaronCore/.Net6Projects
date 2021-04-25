using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RarOrZip.Sample
{
    public class RarOrZipHelper
    {
        public void UnRarOrZipUnit()
        {
            try
            {
                Console.WriteLine("start...");

                var unPath = @"F:\.NetProjects\DotNetProjects\RarOrZipApp.Sample\UnitFiles\Save";
                var rarPathName = @"F:\.NetProjects\DotNetProjects\RarOrZipApp.Sample\UnitFiles\1003.rar";
                var isCover = true;
                string passWord = null;

                UnRarOrZip(unPath, rarPathName, isCover, passWord);

                Console.WriteLine("end...");
                Console.ReadKey();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        /// <summary>
        /// 解压RAR和ZIP文件(需存在Winrar.exe(只要自己电脑上可以解压或压缩文件就存在Winrar.exe))
        /// </summary>
        /// <param name="UnPath">解压后文件保存目录</param>
        /// <param name="rarPathName">待解压文件存放绝对路径（包括文件名称）</param>
        /// <param name="IsCover">所解压的文件是否会覆盖已存在的文件(如果不覆盖,所解压出的文件和已存在的相同名称文件不会共同存在,只保留原已存在文件)</param>
        /// <param name="PassWord">解压密码(如果不需要密码则为空)</param>
        /// <returns>true(解压成功);false(解压失败)</returns>
        public static bool UnRarOrZip(string UnPath, string rarPathName, bool IsCover, string PassWord)
        {
            if (!Directory.Exists(UnPath))
                Directory.CreateDirectory(UnPath);
            Process process = new Process();
            process.StartInfo.FileName = @"F:\.NetProjects\DotNetProjects\RarOrZipApp.Sample\UnitFiles\Winrar.exe";
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = true;

            string cmd = "";
            if (!string.IsNullOrEmpty(PassWord) && IsCover)
                //解压加密文件且覆盖已存在文件( -p密码 )
                cmd = string.Format(" x -p{0} -o+ {1} {2} -y", PassWord, rarPathName, UnPath);
            else if (!string.IsNullOrEmpty(PassWord) && !IsCover)
                //解压加密文件且不覆盖已存在文件( -p密码 )
                cmd = string.Format(" x -p{0} -o- {1} {2} -y", PassWord, rarPathName, UnPath);
            else if (IsCover)
                //覆盖命令( x -o+ 代表覆盖已存在的文件)
                cmd = string.Format(" x -o+ {0} {1} -y", rarPathName, UnPath);
            else
                //不覆盖命令( x -o- 代表不覆盖已存在的文件)
                cmd = string.Format(" x -o- {0} {1} -y", rarPathName, UnPath);
            //命令
            process.StartInfo.Arguments = cmd;
            process.Start();
            process.WaitForExit();//无限期等待进程 winrar.exe 退出
            //Process1.ExitCode==0指正常执行，Process1.ExitCode==1则指不正常执行
            if (process.ExitCode == 0)
            {
                process.Close();
                return true;
            }
            else
            {
                process.Close();
                return false;
            }
        }

        /// <summary>
        /// 压缩文件成RAR或ZIP文件(需存在Winrar.exe(只要自己电脑上可以解压或压缩文件就存在Winrar.exe))
        /// </summary>
        /// <param name="filesPath">将要压缩的文件夹或文件的绝对路径</param>
        /// <param name="rarPathName">压缩后的压缩文件保存绝对路径（包括文件名称）</param>
        /// <param name="IsCover">所压缩文件是否会覆盖已有的压缩文件(如果不覆盖,所压缩文件和已存在的相同名称的压缩文件不会共同存在,只保留原已存在压缩文件)</param>
        /// <param name="PassWord">压缩密码(如果不需要密码则为空)</param>
        /// <returns>true(压缩成功);false(压缩失败)</returns>
        public static bool CondenseRarOrZip(string filesPath, string rarPathName, bool IsCover, string PassWord)
        {
            string rarPath = Path.GetDirectoryName(rarPathName);
            if (!Directory.Exists(rarPath))
                Directory.CreateDirectory(rarPath);
            Process process = new Process();
            process.StartInfo.FileName = @"F:\.NetProjects\DotNetProjects\RarOrZipApp.Sample\UnitFiles\Winrar.exe";
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = true;

            string cmd = "";
            if (!string.IsNullOrEmpty(PassWord) && IsCover)
                //压缩加密文件且覆盖已存在压缩文件( -p密码 -o+覆盖 )
                cmd = string.Format(" a -ep1 -p{0} -o+ {1} {2} -r", PassWord, rarPathName, filesPath);
            else if (!string.IsNullOrEmpty(PassWord) && !IsCover)
                //压缩加密文件且不覆盖已存在压缩文件( -p密码 -o-不覆盖 )
                cmd = string.Format(" a -ep1 -p{0} -o- {1} {2} -r", PassWord, rarPathName, filesPath);
            else if (string.IsNullOrEmpty(PassWord) && IsCover)
                //压缩且覆盖已存在压缩文件( -o+覆盖 )
                cmd = string.Format(" a -ep1 -o+ {0} {1} -r", rarPathName, filesPath);
            else
                //压缩且不覆盖已存在压缩文件( -o-不覆盖 )
                cmd = string.Format(" a -ep1 -o- {0} {1} -r", rarPathName, filesPath);
            //命令
            process.StartInfo.Arguments = cmd;
            process.Start();
            process.WaitForExit();//无限期等待进程 winrar.exe 退出
            //Process1.ExitCode==0指正常执行，Process1.ExitCode==1则指不正常执行
            if (process.ExitCode == 0)
            {
                process.Close();
                return true;
            }
            else
            {
                process.Close();
                return false;
            }
        }
    }
}
