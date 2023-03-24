using System;
using System.IO;

namespace iMine.Launcher.FileVisitor
{
    public class SimpleFileVisitor<T>
    {

        protected SimpleFileVisitor()
        {
        }

        public virtual FileVisitResult PreVisitDirectory(T dir)
        {
            if (dir == null)
                throw new NullReferenceException();
            return FileVisitResult.Continue;
        }

        public virtual FileVisitResult VisitFile(T file)
        {
            if (file == null)
                throw new NullReferenceException();
            return FileVisitResult.Continue;
        }

        public virtual FileVisitResult VisitFileFailed(T file, IOException exc)
        {
            if (file == null)
                throw new NullReferenceException();
            throw exc;
        }

        public virtual FileVisitResult PostVisitDirectory(T dir, IOException exc)
        {
            if (dir == null)
                throw new NullReferenceException();
            if (exc != null)
                throw exc;
            return FileVisitResult.Continue;
        }
    }

    public enum FileVisitResult
    {
        Continue,
        Terminate,
        SkipSubtree,
        SkipSiblings
    }
}