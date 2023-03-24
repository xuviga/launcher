using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace iMine.Launcher.Helper
{
    public static class IoHelper
    {
        public static byte[] EncryptPassword(byte[] key, string password)
        {
            var passBytes = Encoding.UTF8.GetBytes(password);
            var bytes = new byte[256];
            Array.Copy(passBytes,bytes,passBytes.Length);
            using (var aes = Aes.Create())
            {
                aes.BlockSize = 128;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.None;

                var encryptor = aes.CreateEncryptor(key, new byte[16]);
                using(var target = new MemoryStream())
                using (var cs = new CryptoStream(target, encryptor, CryptoStreamMode.Write))
                {
                    cs.Write(bytes, 0, bytes.Length);
                    return target.ToArray();
                }
            }
        }

        public static string VerifyFileName(string fileName)
        {
            return VerifyHelper.Verify(fileName, IsValidFileName, $"Invalid file name: '{fileName}'");
        }

        public static bool IsValidFileName(string fileName)
        {
            return !fileName.Equals(".") && !fileName.Equals("..") &&
                   !fileName.ToCharArray().Any(ch => ch == '/' || ch == '\\') && IsValidPath(fileName);
        }

        public static bool IsValidPath(string path)
        {
            try
            {
                path=Path.GetFullPath(path);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}