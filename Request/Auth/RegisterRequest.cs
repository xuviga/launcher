using iMine.Launcher.Client;
using iMine.Launcher.Helper;
using iMine.Launcher.Serialize;

namespace iMine.Launcher.Request.Auth
{
    public class RegisterRequest : AbstractRequest<AuthRequest.Result>
    {
        private readonly string username;
        private readonly string password;
        private readonly string email;

        public RegisterRequest(string username, string password, string email)
        {
            this.username = username;
            this.password = password;
            this.email = email;
        }

        protected override AuthRequest.Result HandleResponse(HInput input, HOutput output)
        {
            var key = input.ReadByteArray(16);
            output.WriteString(username, 255);
            output.WriteByteArray(IoHelper.EncryptPassword(key,password), RequestHelper.CryproMaxLength);
            output.WriteString(email, 255);
            output.Flush();

            ReadError(input);
            var pp = new PlayerProfile(input);
            var accessToken = input.ReadAscii(-RequestHelper.TokenStringLength);
            return new AuthRequest.Result(pp, accessToken);
        }

        protected override RequestType GetRequestType()
        {
            return RequestType.Register;
        }
    }
}