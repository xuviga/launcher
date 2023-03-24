using System;
using System.IO;
using System.Linq;
using iMine.Launcher.Helper;
using iMine.Launcher.Serialize;

namespace iMine.Launcher.Hasher
{
    public class HashedFile : HashedEntry
    {
        public long Size;
        private readonly byte[] _digest;

        public HashedFile(long size, byte[] digest)
        {
            Size = VerifyHelper.VerifyLong(size, VerifyHelper.LNotNegative, "Illegal size: " + size);
            if (digest != null)
            {
                if (digest.Length != 16)
                    throw new ArgumentException("Invalid digest length: " + digest.Length);
                _digest = new byte[16];
                digest.CopyTo(_digest, 0);
            }
        }

        public HashedFile(FileInfo file, bool digest)
            : this(file.Length, digest ? Config.ComputeHash(file.FullName) : null)
        {
        }

        public HashedFile(HInput input)
            : this(input.ReadVarLong(), input.ReadBoolean() ? input.ReadByteArray(-16) : null)
        {
        }

        public override HashedType GetHashedType()
        {
            return HashedType.File;
        }

        public override long GetSize()
        {
            return Size;
        }

        public override void Write(HOutput output)
        {
            output.WriteVarLong(Size);
            output.WriteBoolean(_digest != null);
            if (_digest != null)
            {
                output.WriteByteArray(_digest, -16);
            }
        }

        public bool IsSame(HashedFile o)
        {
            return Size == o.Size && (_digest == null || o._digest == null || _digest.SequenceEqual(o._digest));
        }

        public bool IsSameDigest(byte[] digest)
        {
            return _digest == null || digest == null || _digest.SequenceEqual(digest);
        }
    }
}