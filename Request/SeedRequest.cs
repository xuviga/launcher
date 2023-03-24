using System;
using System.IO;
using iMine.Launcher.Serialize;
using iMine.Launcher.Utils;

namespace iMine.Launcher.Request
{
    public class SeedRequest : AbstractRequest<object>
    {
        private readonly string username;
        private readonly string token;
        private readonly string guid;
        private readonly string nope;

        public SeedRequest(string username, string token, string nope, string guid)
        {
            this.username = username;
            this.token = token;
            this.nope = nope;
            this.guid = guid;
        }

        protected override object HandleResponse(HInput input, HOutput output)
        {
            try
            {
                output.WriteString(username, 255);
                output.WriteString(token, 255);
                output.WriteString(guid, 64);
            }
            catch
            {
                App.WriteLog($"{username == null}, {token == null}, {guid == null}, {nope}");
                throw;
            }

            var hwInfo = HardwareInfo.GetRawData();
            hwInfo.Remove("free_ram");
            hwInfo.Remove("username");
            hwInfo.Remove("ver");
            output.WriteString(hwInfo.ToString(), 16384);
            output.Flush();

            if (!input.ReadBoolean())
                throw new IOException("Failed to auth");
            return null;
        }

        protected override RequestType GetRequestType()
        {
            return RequestType.Seed;
        }
    }
}