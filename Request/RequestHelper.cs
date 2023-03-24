namespace iMine.Launcher.Request
{
    public static class RequestHelper
    {
        public static readonly int ProtocolMagic = 0x724724_00 + 23;
        public static readonly int RsaKeyLengthBits = 2048;
        public static readonly int RsaKeyLength = RsaKeyLengthBits / 8;

        public static int TokenLength = 16;
        public static int TokenStringLength = TokenLength << 1;
        public static int CryproMaxLength = 2048;
    }
}