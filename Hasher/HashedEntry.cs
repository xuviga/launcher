using iMine.Launcher.Serialize.Streaming;

namespace iMine.Launcher.Hasher
{
    public abstract class HashedEntry : StreamObject
    {
        public bool Flag;

        public abstract HashedType GetHashedType();

        public abstract long GetSize();

    }
}