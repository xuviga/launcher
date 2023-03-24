using System.IO;

namespace iMine.Launcher.FileVisitor
{
    public static class FileWatchDoge
    {
        public static void Watch(string path)
        {
            /*FileSystemWatcher fileSystemWatcher = new FileSystemWatcher();
            fileSystemWatcher.Path = path;
            fileSystemWatcher.Created += OnChange;
            fileSystemWatcher.EnableRaisingEvents = true;
            fileSystemWatcher.IncludeSubdirectories = true;    */
        }

        private static void OnChange(object sender, FileSystemEventArgs e)
        {
            App.WriteLog("changed: "+e.FullPath);
            //DataProvider.GameProcess.Kill();
        }
    }
}