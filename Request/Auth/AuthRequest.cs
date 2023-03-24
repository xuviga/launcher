using iMine.Launcher.Client;
using iMine.Launcher.Helper;
using iMine.Launcher.Serialize;

namespace iMine.Launcher.Request.Auth
{
    public class AuthRequest : AbstractRequest<AuthRequest.Result>
    {
        private readonly string username;
        private readonly string password;

        public AuthRequest(string username, string password)
        {
            this.username = username;
            this.password = password;
        }

        protected override Result HandleResponse(HInput input, HOutput output)
        {
            var key = input.ReadByteArray(16);
            output.WriteString(username, 255);
            output.WriteByteArray(IoHelper.EncryptPassword(key,password), RequestHelper.CryproMaxLength);
            output.Flush();

            ReadError(input);
            var pp = new PlayerProfile(input);
            var accessToken = input.ReadAscii(-RequestHelper.TokenStringLength);
            return new Result(pp, accessToken);
        }

        protected override RequestType GetRequestType()
        {
            return RequestType.AuthWin;
        }

        public class Result
        {
            public readonly PlayerProfile PlayerProfile;
            public readonly string AccessToken;

            public Result(PlayerProfile playerProfile, string accessToken)
            {
                PlayerProfile = playerProfile;
                AccessToken = accessToken;
            }
        }
    }
}