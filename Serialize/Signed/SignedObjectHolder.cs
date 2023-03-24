using iMine.Launcher.Serialize.Streaming;

namespace iMine.Launcher.Serialize.Signed
{
    public class SignedObjectHolder<T> : SignedBytesHolder where T : StreamObject
    {
        public readonly T Obj;

        public SignedObjectHolder(HInput input, StreamObject.IAdapter<T> adapter)
            : base(input)
        {
            Obj = NewInstance(adapter);
        }

        public T NewInstance(StreamObject.IAdapter<T> adapter)
        {
            var input = new HInput(Bytes);
            return adapter.Convert(input);
        }
    }
}
