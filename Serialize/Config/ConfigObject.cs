using iMine.Launcher.Serialize.Config.Entry;
using iMine.Launcher.Serialize.Streaming;

namespace iMine.Launcher.Serialize.Config
{
    public abstract class ConfigObject : StreamObject
    {
        public BlockConfigEntry Block;

        protected ConfigObject(BlockConfigEntry block)
        {
            Block = block;
        }

        public override void Write(HOutput output)
        {
            Block.Write(output);
        }
    }
}