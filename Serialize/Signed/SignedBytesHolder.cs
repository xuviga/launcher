using iMine.Launcher.Request;

namespace iMine.Launcher.Serialize.Signed
{
    public class SignedBytesHolder
    {
        protected readonly byte[] Bytes;
        protected readonly byte[] Sign;

        public SignedBytesHolder(HInput input)
            : this(input.ReadByteArray(0), input.ReadByteArray(-RequestHelper.RsaKeyLength))
        {
        }

        public SignedBytesHolder(byte[] bytes, byte[] sign)
        {
            Bytes = (byte[]) bytes.Clone();
            Sign = (byte[]) sign.Clone();
        }

        public void Write(HOutput output)
        {
            output.WriteByteArray(Bytes, 0);
            output.WriteByteArray(Sign, -RequestHelper.RsaKeyLength);
        }

        public byte[] GetBytes()
        {
            return (byte[]) Bytes.Clone();
        }

        public byte[] GetSign()
        {
            return (byte[]) Sign.Clone();
        }
    }
}
