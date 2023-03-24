using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using iMine.Launcher.Helper;
using iMine.Launcher.Serialize;

namespace iMine.Launcher.Hasher
{
    public class HashedDir : HashedEntry
    {
        private readonly Utils.Collections.NDictionary<string, HashedEntry> Map = new Utils.Collections.NDictionary<string, HashedEntry>();
        public bool anyMismatches;

        public HashedDir()
        {
        }

        public HashedDir(DirectoryInfo dir, FileNameMatcher matcher, bool allowSymlinks, bool digest)
        {
            //todo matcher и allowSymlinks не юзается
            foreach (var fileInfo in dir.GetFiles())
            {
                try
                {
                    Map.Add(fileInfo.Name, new HashedFile(fileInfo, digest));
                }
                catch
                {
                }
            }
            foreach (var dirInfo in dir.GetDirectories())
            {
                Map.Add(dirInfo.Name, new HashedDir(dirInfo, matcher, allowSymlinks, digest));
            }
        }

        public HashedDir(HInput input)
        {
            var entriesCount = input.ReadLength(0);
            for (var i = 0; i < entriesCount; i++)
            {
                var name = input.ReadString(255);

                // Read entry
                HashedEntry entry;
                var type = (HashedType) input.ReadVarInt();
                switch (type)
                {
                    case HashedType.File:
                        entry = new HashedFile(input);
                        break;
                    case HashedType.Dir:
                        entry = new HashedDir(input);
                        break;
                    default:
                        throw new InvalidDataException("Unsupported hashed entry type: " + type);
                }

                VerifyHelper.PutIfAbsent(Map, name, entry, "Такой элемент уже есть");
            }
        }

        public override HashedType GetHashedType()
        {
            return HashedType.Dir;
        }

        public override long GetSize()
        {
            return Map.Sum(it => it.Value.GetSize());
        }

        public override void Write(HOutput output)
        {
            output.WriteLength(Map.Count, 0);
            foreach (var mapEntry in Map)
            {
                output.WriteString(mapEntry.Key, 255);

                var entry = mapEntry.Value;
                output.WriteVarInt((int) entry.GetHashedType());
                entry.Write(output);
            }
        }

        public Diff diff(HashedDir other, FileNameMatcher matcher)
        {
            var mismatch = sideDiff(other, matcher, new LinkedList<string>(), true);
            var extra = other.sideDiff(this, matcher, new LinkedList<string>(), false);
            return new Diff(mismatch, extra);
        }

        public HashedEntry getEntry(string name)
        {
            return Map.ContainsKey(name) ? Map[name] : null;
        }

        public bool isEmpty()
        {
            return Map.Count == 0;
        }

        public Dictionary<string, HashedEntry> map()
        {
            return new Dictionary<string, HashedEntry>(Map);
        }

        public HashedEntry resolve(IEnumerable<string> path)
        {
            HashedEntry current = this;
            foreach (var pathEntry in path)
            {
                if (current is HashedDir)
                {
                    current = ((HashedDir) current).Map[pathEntry];
                    continue;
                }
                return null;
            }
            return current;
        }

        private HashedDir sideDiff(HashedDir other, FileNameMatcher matcher, LinkedList<string> path, bool mismatchList)
        {
            var diff = new HashedDir();
            foreach (var mapEntry in Map)
            {
                var name = mapEntry.Key;
                var entry = mapEntry.Value;
                path.AddLast(name);

                // Should update?
                var shouldUpdate = matcher == null || matcher.ShouldUpdate(path);

                // Not found or of different type
                var type = entry.GetHashedType();
                var otherEntry = other.Map[name];
                if (otherEntry == null || otherEntry.GetHashedType() != type)
                {
                    if (shouldUpdate || mismatchList && otherEntry == null)
                    {
                        diff.Map[name] = entry;

                        // Should be deleted!
                        if (!mismatchList)
                        {
                            entry.Flag = true;
                        }
                    }
                    path.RemoveLast();
                    continue;
                }

                // Compare entries based on type
                switch (type)
                {
                    case HashedType.File:
                        var file = (HashedFile) entry;
                        var otherFile = (HashedFile) otherEntry;
                        if (mismatchList && shouldUpdate && !file.IsSame(otherFile))
                        {
                            diff.Map[name] = entry;
                        }
                        break;
                    case HashedType.Dir:
                        var dir = (HashedDir) entry;
                        var otherDir = (HashedDir) otherEntry;
                        if (mismatchList || shouldUpdate)
                        {
                            // Maybe isn't need to go deeper?
                            var mismatch = dir.sideDiff(otherDir, matcher, path, mismatchList);
                            if (!mismatch.isEmpty())
                            {
                                diff.Map[name] = mismatch;
                            }
                        }
                        break;
                    default:
                        throw new InvalidOperationException("Unsupported hashed entry type: " + type);
                }

                // Remove this path entry
                path.RemoveLast();
            }
            return diff;
        }

        /*private class HashFileVisitor : SimpleFileVisitor<FileInfo>
        {
            private FileInfo dir;
            private FileNameMatcher matcher;
            private bool allowSymlinks;
            private bool digest;

            // State
            private HashedDir current;

            private LinkedList<string> path = new LinkedList<string>();
            private LinkedList<HashedDir> stack = new LinkedList<HashedDir>();

            private HashFileVisitor(HashedDir hashedDir, FileInfo dir, FileNameMatcher matcher, bool allowSymlinks,
                bool digest)
            {
                current = hashedDir;
                this.dir = dir;
                this.matcher = matcher;
                this.allowSymlinks = allowSymlinks;
                this.digest = digest;
            }

            public override FileVisitResult PostVisitDirectory(FileInfo dir, IOException exc)
            {
                var result = base.PostVisitDirectory(dir, exc);
                if (this.dir.Equals(dir))
                {
                    return result;
                }

                // Add directory to parent
                var parent = stack.Last.Value;
                stack.RemoveLast();
                var see = path.Last.Value;
                path.RemoveLast();
                parent.Map[see] = current;
                current = parent;

                // We're done
                return result;
            }

            public override FileVisitResult PreVisitDirectory(FileInfo dir)
            {
                FileVisitResult result = base.PreVisitDirectory(dir);
                if (this.dir.Equals(dir))
                {
                    return result;
                }
                if (!allowSymlinks && dir.Attributes.HasFlag(FileAttributes.ReparsePoint))
                {
                    throw new SecurityException("Symlinks are not allowed");
                }

                // Add child
                stack.AddLast(current);
                current = new HashedDir();
                path.AddLast(dir.Name);

                // We're done
                return result;
            }

            public override FileVisitResult VisitFile(FileInfo file)
            {
                // Verify is not symlink
                if (!allowSymlinks && dir.Attributes.HasFlag(FileAttributes.ReparsePoint))
                {
                    throw new SecurityException("Symlinks are not allowed");
                }

                // Add file (may be unhashed, if exclusion)
                path.AddLast(file.Name);
                bool doDigest = digest && (matcher == null || matcher.ShouldUpdate(path));
                var see = path.Last.Value;
                path.RemoveLast();
                current.Map[see]=new HashedFile(file, file.Length, doDigest);
                return base.VisitFile(file);
            }
        }*/


        public class Diff
        {
            public readonly HashedDir Mismatch;
            public readonly HashedDir Extra;

            public Diff(HashedDir mismatch, HashedDir extra)
            {
                Mismatch = mismatch;
                Extra = extra;
            }
        }
    }
}