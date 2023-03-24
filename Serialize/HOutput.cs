using System;
using System.IO;

namespace iMine.Launcher.Serialize
{
    public class HOutput : IDisposable
    {
        public Stream stream;

        public HOutput(Stream stream)
        {
            this.stream = stream;
        }

        public void Dispose()
        {
            stream.Close();
        }

        public void Close()
        {
            stream.Close();
        }

        public void Flush()
        {
            stream.Flush();
        }

        public void WriteASCII(string str, int maxBytes)
        {
            WriteByteArray(System.Text.Encoding.ASCII.GetBytes(str), maxBytes);
        }


        public void WriteBoolean(bool b)
        {
            WriteUnsignedByte(b ? 0b1 : 0b0);
        }

        public void WriteByteArray(byte[] bytes, int max)
        {
            var len = Math.Min(bytes.Length, max);
            WriteLength(bytes.Length, max);
            stream.Write(bytes, 0, len);
        }

        public void WriteRightInt(int i)
        {
            WriteUnsignedByte(i >> 24 & 0xFF);
            WriteUnsignedByte(i >> 16 & 0xFF);
            WriteUnsignedByte(i >> 8 & 0xFF);
            WriteUnsignedByte(i & 0xFF);
        }

        public void WriteLength(int length, int max)
        {
            VerifyLength(length, max);
            if (max >= 0)
                WriteVarInt(length);
        }

        public void WriteLong(long l)
        {
            WriteRightInt((int) (l >> 32));
            WriteRightInt((int) l);
        }

        public void WriteShort(short s)
        {
            WriteUnsignedByte(s >> 8 & 0xFF);
            WriteUnsignedByte(s & 0xFF);
        }

        public void WriteString(string str, int maxBytes)
        {
            WriteByteArray(System.Text.Encoding.UTF8.GetBytes(str), maxBytes);
        }

        public void WriteGuid(Guid uuid)
        {
            WriteASCII(uuid.ToString(), 512);
        }

        public void WriteUnsignedByte(int b)
        {
            stream.WriteByte((byte) b);
        }

        public void WriteVarInt(int i)
        {
            while ((i & ~0x7FL) != 0)
            {
                WriteUnsignedByte(i & 0x7F | 0x80);
                i >>= 7;
            }
            WriteUnsignedByte(i);
        }

        public void WriteVarLong(long l)
        {
            while ((l & ~0x7FL) != 0)
            {
                WriteUnsignedByte((int) l & 0x7F | 0x80);
                l >>= 7;
            }
            WriteUnsignedByte((int) l);
        }

        public int VerifyLength(int length, int max)
        {
            if (length < 0 || max < 0 && length != -max || max > 0 && length > max)
                throw new IOException("Illegal length: " + length);
            return length;
        }
    }
}