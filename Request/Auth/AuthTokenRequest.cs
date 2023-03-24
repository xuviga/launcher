using iMine.Launcher.Client;
using iMine.Launcher.Serialize;

namespace iMine.Launcher.Request.Auth
{
    public class AuthTokenRequest : AbstractRequest<AuthRequest.Result>
    {
        private readonly string username;
        private readonly string token;

        public AuthTokenRequest(string username, string token)
        {
            this.username = username;
            this.token = token;
        }

        protected override AuthRequest.Result HandleResponse(HInput input, HOutput output)
        {
            output.WriteString(username, 255);
            output.WriteString(token, 255);
            output.Flush();

            ReadError(input);
            var pp = new PlayerProfile(input);
            var accessToken = input.ReadAscii(-RequestHelper.TokenStringLength);
            return new AuthRequest.Result(pp, accessToken);
        }

        protected override RequestType GetRequestType()
        {
            return RequestType.AuthToken;
        }
    }
}