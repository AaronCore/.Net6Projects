using System;
using System.IO;

namespace FileSystemWatcherDemo
{
    internal class Program
    {
        static void Main(string[] args)
        {
            FileListenerServer f1 = new FileListenerServer(@"C:\Temp");
            f1.Start();
            Console.ReadKey();
        }
        public class FileListenerServer
        {
            private FileSystemWatcher _watcher;
            public FileListenerServer()
            {

            }
            public FileListenerServer(string path)
            {
                try
                {
                    this._watcher = new FileSystemWatcher();
                    _watcher.Path = path;
                    _watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.Size | NotifyFilters.DirectoryName;
                    _watcher.IncludeSubdirectories = true;
                    _watcher.Created += new FileSystemEventHandler(FileWatcher_Created);
                    _watcher.Changed += new FileSystemEventHandler(FileWatcher_Changed);
                    _watcher.Deleted += new FileSystemEventHandler(FileWatcher_Deleted);
                    _watcher.Renamed += new RenamedEventHandler(FileWatcher_Renamed);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error:" + ex.Message);
                }
            }

            public void Start()
            {
                this._watcher.EnableRaisingEvents = true;
                Console.WriteLine("文件监控已经启动...");
            }

            public void Stop()
            {
                this._watcher.EnableRaisingEvents = false;
                this._watcher.Dispose();
                this._watcher = null;
            }

            protected void FileWatcher_Created(object sender, FileSystemEventArgs e)
            {
                Console.WriteLine("新增:" + e.ChangeType + ";" + e.FullPath + ";" + e.Name);
            }

            protected void FileWatcher_Changed(object sender, FileSystemEventArgs e)
            {
                Console.WriteLine("变更:" + e.ChangeType + ";" + e.FullPath + ";" + e.Name);
            }

            protected void FileWatcher_Deleted(object sender, FileSystemEventArgs e)
            {
                Console.WriteLine("删除:" + e.ChangeType + ";" + e.FullPath + ";" + e.Name);
            }

            protected void FileWatcher_Renamed(object sender, RenamedEventArgs e)
            {
                Console.WriteLine("重命名: OldPath:{0} NewPath:{1} OldFileName{2} NewFileName:{3}", e.OldFullPath, e.FullPath, e.OldName, e.Name);
            }
        }
    }
}
