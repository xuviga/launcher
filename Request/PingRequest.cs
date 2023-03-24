using System.IO;
using iMine.Launcher.Serialize;

namespace iMine.Launcher.Request
{
    public class PingRequest : AbstractRequest<object>
    {
        public static byte ExpectedByte = 0b01010101;

        protected override object HandleResponse(HInput input, HOutput output)
        {
            var pong = input.ReadUnsignedByte();
            if (pong != ExpectedByte)
                throw new IOException("Illegal ping response: " + pong);
            return null;
        }

        protected override RequestType GetRequestType()
        {
            return RequestType.Ping;
        }
    }
}