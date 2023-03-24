using System.IO;

namespace iMine.Launcher.Utils
{
    public static class DirStuff
    {
        public static DirectoryInfo ResolveDirectory(this DirectoryInfo root, string subPath)
        {
            return new DirectoryInfo(ResolveString(root,subPath));
        }

        public static FileInfo ResolveFile(this DirectoryInfo root, string subPath)
        {
            return new FileInfo(ResolveString(root,subPath));
        }

        private static string ResolveString(DirectoryInfo root, string subPath)
        {
            var rootName = root.FullName;
            if (subPath.EndsWith("/") || subPath.EndsWith("\\"))
                rootName = rootName.Substring(0,rootName.Length-1);
            if (subPath.StartsWith("/") || subPath.StartsWith("\\"))
                subPath = subPath.Substring(1);
            return rootName+"/"+subPath;
        }
    }
}