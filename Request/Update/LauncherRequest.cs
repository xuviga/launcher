using System.Collections.Generic;
using iMine.Launcher.Client;
using iMine.Launcher.Serialize;
using iMine.Launcher.Serialize.Signed;

namespace iMine.Launcher.Request.Update
{
    public class LauncherRequest : AbstractRequest<List<SignedObjectHolder<ClientProfile>>>
    {
        protected override List<SignedObjectHolder<ClientProfile>> HandleResponse(HInput input, HOutput output)
        {
            output.WriteASCII(Settings.Username ?? "", 255);
            output.WriteASCII(Settings.Token ?? "", 255);
            output.WriteBoolean(false);
            output.Flush();
            ReadError(input);

            var sign = input.ReadByteArray(-RequestHelper.RsaKeyLength); //обратная совместимость

            output.WriteBoolean(false);
            output.Flush();

            var count = input.ReadLength(0);
            var profiles = new List<SignedObjectHolder<ClientProfile>>();
            for (var i = 0; i < count; i++)
                profiles.Add(new SignedObjectHolder<ClientProfile>(input, ClientProfile.RoAdapter));
            return profiles;
        }

        protected override RequestType GetRequestType()
        {
            return RequestType.Launcher;
        }
    }
}