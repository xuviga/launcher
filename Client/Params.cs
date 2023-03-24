using System.IO;
using iMine.Launcher.Request;
using iMine.Launcher.Serialize;
using iMine.Launcher.Serialize.Streaming;

namespace iMine.Launcher.Client
{
    public class Params : StreamObject
    {
        // Client paths
        public DirectoryInfo assetDir;
        public DirectoryInfo librariesDir;
        public DirectoryInfo clientDir;

        // Client params
        public PlayerProfile pp;

        public string accessToken;
        public bool autoEnter;
        public bool fullScreen;
        public int ram;
        public int width;
        public int height;
        private byte[] launcherSign;

        public Params(byte[] launcherSign, DirectoryInfo assetDir, DirectoryInfo librariesDir, DirectoryInfo clientDir, PlayerProfile pp,
            string accessToken,
            bool autoEnter, bool fullScreen, int ram, int width, int height)
        {
            this.launcherSign = (byte[]) launcherSign.Clone();

            // Client paths
            this.assetDir = assetDir;
            this.librariesDir = librariesDir;
            this.clientDir = clientDir;

            // Client params
            this.pp = pp;
            //this.accessToken = SecurityHelper.verifyToken(accessToken);
            this.accessToken = accessToken; //todo verifyToken
            this.autoEnter = autoEnter;
            this.fullScreen = fullScreen;
            this.ram = ram;
            this.width = width;
            this.height = height;
        }

        public override void Write(HOutput output)
        {
            output.WriteByteArray(launcherSign, -RequestHelper.RsaKeyLength);

            // Client paths
            output.WriteString(assetDir.ToString(), 0);
            output.WriteString(clientDir.ToString(), 0);

            // Client params
            pp.Write(output);
            output.WriteASCII(accessToken, -RequestHelper.TokenStringLength);
            output.WriteBoolean(autoEnter);
            output.WriteBoolean(fullScreen);
            output.WriteVarInt(ram);
            output.WriteVarInt(width);
            output.WriteVarInt(height);
        }
    }
}