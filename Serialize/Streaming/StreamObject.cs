namespace iMine.Launcher.Serialize.Streaming
{
    public abstract class StreamObject
    {
        public abstract void Write(HOutput output);

        public interface IAdapter<T> where T : StreamObject
        {
            T Convert(HInput input);
        }
    }
}