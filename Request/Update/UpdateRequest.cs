using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using iMine.Launcher.Hasher;
using iMine.Launcher.Helper;
using iMine.Launcher.Serialize;
using iMine.Launcher.Serialize.Signed;
using iMine.Launcher.Serialize.Streaming;

namespace iMine.Launcher.Request.Update
{
    public class UpdateRequest : AbstractRequest<SignedObjectHolder<HashedDir>>
    {
        public const int MaxQueueSize = 128;
        public const int BufferSize = 4096;

        // Instance
        private readonly string dirName;
        private readonly DirectoryInfo dir;
        private readonly FileNameMatcher matcher;
        private readonly bool digest;
        private Callback stateCallback;

        // State
        private HashedDir localDir;

        private DateTime startTime;

        public long DownloadedSize { get; protected set; }
        public long TotalSize { get; protected set; }
        public string CurrentFile { get; protected set; }
        public long CurrentDownloadedSize { get; protected set; }
        public long CurrentTotalSize { get; protected set; }

        public class HashConverted : StreamObject.IAdapter<HashedDir>
        {
            public HashedDir Convert(HInput input)
            {
                return new HashedDir(input);
            }
        }

        public UpdateRequest(string dirName, DirectoryInfo dir, FileNameMatcher matcher, bool digest)
        {
            this.dirName = IoHelper.VerifyFileName(dirName);
            this.dir = dir ?? throw new NullReferenceException();
            this.matcher = matcher;
            this.digest = digest;
        }

        protected override RequestType GetRequestType()
        {
            return RequestType.UpdateWin;
        }

        public override SignedObjectHolder<HashedDir> SendRequest()
        {
            dir.Create();
            localDir = new HashedDir(dir, matcher, false, digest);

            var result = base.SendRequest();
            return result;
        }

        protected override SignedObjectHolder<HashedDir> HandleResponse(HInput input, HOutput output)
        {
            // Write update dir name
            output.WriteString(dirName, 255);
            output.Flush();
            ReadError(input);

            // Get diff between local and remote dir
            var remoteHDirHolder = new SignedObjectHolder<HashedDir>(input, new HashConverted());
            var diff = remoteHDirHolder.Obj.diff(localDir, matcher);
            TotalSize = diff.Mismatch.GetSize();
            var compress = input.ReadBoolean();

            // Build actions queue
            var queue = new LinkedList<Action>();
            FillActionsQueue(queue, diff.Mismatch);
            queue.AddFirst(Action.Finish);

            // Download missing first
            // (otherwise it will cause mustdie indexing bug)
            startTime = DateTime.Now;
            var currentDir = dir;
            var actionsSlice = new Action[MaxQueueSize];
            while (queue.Count>0)
            {
                var length = Math.Min(queue.Count, MaxQueueSize);

                // Write actions slice
                output.WriteLength(length, MaxQueueSize);
                for (var i = 0; i < length; i++)
                {
                    var action = queue.Last.Value;
                    queue.RemoveLast();
                    actionsSlice[i] = action;
                    action.Write(output);
                }
                App.WriteLog($"Разбираем данные для {dirName}");
                output.Flush();

                var fileInput = compress
                    ? new DeflateStream(input.Stream, CompressionMode.Decompress, false)
                    : input.Stream;

                // Perform actions
                for (var i = 0; i < length; i++)
                {
                    var action = actionsSlice[i];
                    switch (action.actionType)
                    {
                        case Action.ActionType.Cd:
                            currentDir = new DirectoryInfo(currentDir.FullName+"/"+action.name);
                            currentDir.Create();
                            break;
                        case Action.ActionType.Get:
                            var targetFile = new FileInfo(currentDir.FullName + "/" + action.name);

                            if ( fileInput.ReadByte() != 0xFF)
                            {
                                throw new IOException("Serverside cached size mismath for file " + action.name);
                            }
                            App.WriteLog($"Качаем файл из набора {dirName} - {targetFile}: [{i}/{length}] [?/{TotalSize}]");
                            CurrentFile = targetFile.FullName.Substring(Config.WorkingDir.ToString().Length);
                            remoteHDirHolder.Obj.anyMismatches = remoteHDirHolder.Obj.anyMismatches | DownloadFile(targetFile, (HashedFile) action.entry, fileInput);
                            break;
                        case Action.ActionType.CdBack:
                            currentDir = currentDir.Parent;
                            break;
                        case Action.ActionType.Finish:
                            break;
                        default:
                            throw new InvalidOperationException("Unsupported action type: '"+action.actionType+"'");
                    }
                }
            }

            // Write update completed packet
            DeleteExtraDir(dir, diff.Extra, diff.Extra.Flag);
            return remoteHDirHolder;
        }

        public void SetStateCallback(Callback callback)
        {
            stateCallback = callback;
        }

        private void DeleteExtraDir(DirectoryInfo subDir, HashedDir subHDir, bool flag)
        {
            foreach (var mapEntry in subHDir.map())
            {
                var name = mapEntry.Key;

                // Delete files and dirs based on type
                var entry = mapEntry.Value;
                var entryType = entry.GetHashedType();
                switch (entryType)
                {
                    case HashedType.File:
                        var filePath = new FileInfo(subDir.FullName+"/"+name);
                        UpdateState(filePath.Name, 0, 0);
                        filePath.Delete();
                        break;
                    case HashedType.Dir:
                        var dirPath = new DirectoryInfo(subDir.FullName+"/"+name);
                        DeleteExtraDir(dirPath, (HashedDir) entry, flag || entry.Flag);
                        break;
                    default:
                        throw new InvalidOperationException("Unsupported hashed entry type: " + entryType);
                }
            }

            // Delete!
            if (flag)
            {
                UpdateState(subDir.Name, 0, 0);
                subDir.Delete();
            }
        }

        private bool DownloadFile(FileSystemInfo file, HashedFile hFile, Stream input)
        {
            var filePath = file.FullName;
            UpdateState(filePath, 0L, hFile.Size);
            // Start file update

            //var hash = new byte[] { 212,29,140,217,143,0,178,4,233,128,9,152,236,248,66,126};
            using (var memory = new MemoryStream())
            {
                using (var fileOutput = new FileStream(filePath, FileMode.Create))
                {
                    CurrentDownloadedSize = 0;
                    CurrentTotalSize = hFile.Size;

                    // Download with digest update
                    var bytes = new byte[BufferSize];
                    while (CurrentDownloadedSize < hFile.Size)
                    {
                        var remaining = (int) Math.Min(hFile.Size - CurrentDownloadedSize, bytes.Length);
                        var length = input.Read(bytes, 0, remaining);

                        if (length < 0)
                        {
                            throw new EndOfStreamException((hFile.Size - CurrentDownloadedSize) + " bytes remaining");
                        }

                        fileOutput.Write(bytes, 0, length);
                        if (digest)
                            memory.Write(bytes,0,length);

                        // Update state
                        CurrentDownloadedSize += length;
                        DownloadedSize += length;
                        UpdateState(filePath, CurrentDownloadedSize, hFile.Size);
                    }
                }

                // Verify digest
                if (digest)
                {
                    if (!hFile.IsSameDigest(Config.ComputeHash(memory.ToArray())))
                    {
                        App.WriteLog("Ошибка скачки файла: '" + filePath + "'");
                        return true;
                        //throw new SecurityException("File digest mismatch: '" + filePath + "'");
                    }
                }
            }
            return false;
        }

        private static void FillActionsQueue(LinkedList<Action> queue, HashedDir mismatch)
        {
            foreach (var mapEntry in mismatch.map())
            {
                var name = mapEntry.Key;
                var entry = mapEntry.Value;
                var entryType = entry.GetHashedType();
                switch (entryType)
                {
                    case HashedType.Dir: // cd - get - cd ..
                        queue.AddFirst(new Action(Action.ActionType.Cd, name, entry));
                        FillActionsQueue(queue, (HashedDir) entry);
                        queue.AddFirst(Action.CdBack);
                        break;
                    case HashedType.File: // get
                        queue.AddFirst(new Action(Action.ActionType.Get, name, entry));
                        break;
                    default:
                        throw new InvalidOperationException("Unsupported hashed entry type: " + entryType);
                }
            }
        }

        private void UpdateState(string filePath, long fileDownloaded, long fileSize)
        {
            stateCallback?.Call(new State(filePath, fileDownloaded, fileSize,
                DownloadedSize, TotalSize, startTime.Subtract(DateTime.Now)));
        }

        public class Action : StreamObject
        {
            public static Action CdBack = new Action(ActionType.CdBack, null, null);
            public static Action Finish = new Action(ActionType.Finish, null, null);

            // Instance
            public ActionType actionType;

            public string name;
            public HashedEntry entry;

            public Action(ActionType actionType, string name, HashedEntry entry)
            {
                this.actionType = actionType;
                this.name = name;
                this.entry = entry;
            }

            public Action(HInput input)
            {
                actionType = (ActionType) input.ReadVarInt();
                name = actionType == ActionType.Cd || actionType == ActionType.Get ? IoHelper.VerifyFileName(input.ReadString(255)) : null;
                entry = null;
            }

            public override void Write(HOutput output)
            {
                output.WriteVarInt((int) actionType);
                if (actionType == ActionType.Cd || actionType == ActionType.Get)
                    output.WriteString(name, 255);
            }

            public enum ActionType
            {
                Cd = 1,
                CdBack = 2,
                Get = 3,
                Finish = 255
            }
        }

        public class State
        {
            public long fileDownloaded;
            public long fileSize;
            public long totalDownloaded;
            public long totalSize;
            public string filePath;
            public TimeSpan duration;

            public State(string filePath, long fileDownloaded, long fileSize, long totalDownloaded, long totalSize,
                TimeSpan duration)
            {
                this.filePath = filePath;
                this.fileDownloaded = fileDownloaded;
                this.fileSize = fileSize;
                this.totalDownloaded = totalDownloaded;
                this.totalSize = totalSize;

                // Also store time of creation
                this.duration = duration;
            }

            public double GetBps()
            {
                long seconds = duration.Seconds;
                if (seconds == 0)
                {
                    return -1.0D; // Otherwise will throw /0 exception
                }
                return totalDownloaded / (double) seconds;
            }

            public TimeSpan GetEstimatedTime()
            {
                var bps = GetBps();
                return bps <= 0.0D ? TimeSpan.Zero : TimeSpan.FromSeconds((long) (GetTotalRemaining() / bps));
            }

            public double GetFileDownloadedKiB()
            {
                return fileDownloaded / 1024.0D;
            }

            public double GetFileDownloadedMiB()
            {
                return GetFileDownloadedKiB() / 1024.0D;
            }

            public double GetFileDownloadedPart()
            {
                if (fileSize == 0)
                {
                    return 0.0D;
                }
                return (double) fileDownloaded / fileSize;
            }

            public long GetFileRemaining()
            {
                return fileSize - fileDownloaded;
            }

            public double GetFileRemainingKiB()
            {
                return GetFileRemaining() / 1024.0D;
            }

            public double GetFileRemainingMiB()
            {
                return GetFileRemainingKiB() / 1024.0D;
            }

            public double GetFileSizeKiB()
            {
                return fileSize / 1024.0D;
            }

            public double GetFileSizeMiB()
            {
                return GetFileSizeKiB() / 1024.0D;
            }

            public double GetTotalDownloadedKiB()
            {
                return totalDownloaded / 1024.0D;
            }

            public double GetTotalDownloadedMiB()
            {
                return GetTotalDownloadedKiB() / 1024.0D;
            }

            public double GetTotalDownloadedPart()
            {
                if (totalSize == 0)
                {
                    return 0.0D;
                }
                return (double) totalDownloaded / totalSize;
            }

            public long GetTotalRemaining()
            {
                return totalSize - totalDownloaded;
            }

            public double GetTotalRemainingKiB()
            {
                return GetTotalRemaining() / 1024.0D;
            }

            public double GetTotalRemainingMiB()
            {
                return GetTotalRemainingKiB() / 1024.0D;
            }

            public double GetTotalSizeKiB()
            {
                return totalSize / 1024.0D;
            }

            public double GetTotalSizeMiB()
            {
                return GetTotalSizeKiB() / 1024.0D;
            }
        }

        public interface Callback
        {
            void Call(State state);
        }
    }
}