using System;
using System.IO;

namespace iMine.Launcher.Serialize
{
    public class HInput : IDisposable
    {
        public readonly Stream Stream;

        public HInput(Stream streamReader)
        {
            Stream = streamReader;
        }

        public HInput(byte[] bytes)
        {
            Stream = new MemoryStream(bytes);
        }

        public void Dispose()
        {
            Stream.Close();
        }

        public void Close()
        {
            Stream.Close();
        }

        public string ReadAscii(int maxBytes)
        {
            return System.Text.Encoding.ASCII.GetString(ReadByteArray(maxBytes));
        }

        public bool ReadBoolean()
        {
            var b = ReadUnsignedByte();
            switch (b)
            {
                case 0b0:
                    return false;
                case 0b1:
                    return true;
                default:
                    throw new IOException("Invalid boolean state: " + b);
            }
        }

        public byte[] ReadByteArray(int max)
        {
            var len = ReadLength(max);
            var bytes = new byte[len];
            for (var i = 0; i < len; i++)
                bytes[i] = (byte) Stream.ReadByte();
            return bytes;
        }

        private int ReadInt()
        {
            return (ReadUnsignedByte() << 24)
                   + (ReadUnsignedByte() << 16)
                   + (ReadUnsignedByte() << 8)
                   + ReadUnsignedByte();
        }

        public int ReadLength(int max)
        {
            if (max < 0)
                return -max;
            return VerifyLength(ReadVarInt(), max);
        }

        public long ReadLong()
        {
            return (long) ReadInt() << 32 | ReadInt() & 0xFFFFFFFFL;
        }

        public short ReadShort()
        {
            return (short) ((ReadUnsignedByte() << 8) + ReadUnsignedByte());
        }

        public string ReadString(int maxBytes)
        {
            return System.Text.Encoding.UTF8.GetString(ReadByteArray(maxBytes));
        }

        public Guid ReadGuid()
        {
            return new Guid(ReadAscii(512));
        }

        public int ReadUnsignedByte()
        {
            return Stream.ReadByte();
        }

        public int ReadUnsignedShort()
        {
            return (ushort) ReadShort();
        }

        public int ReadVarInt()
        {
            var shift = 0;
            var result = 0;
            while (shift < 32)
            {
                var b = ReadUnsignedByte();
                result |= (b & 0x7F) << shift;
                if ((b & 0x80) == 0)
                {
                    return result;
                }
                shift += 7;
            }
            throw new IOException("VarInt too big");
        }

        public long ReadVarLong()
        {
            var shift = 0;
            long result = 0;
            while (shift < 64)
            {
                var b = ReadUnsignedByte();
                result |= (long) (b & 0x7F) << shift;
                if ((b & 0x80) == 0)
                {
                    return result;
                }
                shift += 7;
            }
            throw new IOException("VarLong too big");
        }

        public int VerifyLength(int length, int max)
        {
            if (length < 0 || max < 0 && length != -max || max > 0 && length > max)
                throw new IOException("Illegal length: " + length);
            return length;
        }
    }
}