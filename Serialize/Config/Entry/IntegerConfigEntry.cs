namespace iMine.Launcher.Serialize.Config.Entry
{
    public class IntegerConfigEntry : ConfigEntry
    {
        public IntegerConfigEntry(int value, bool ro, int cc) : base(value, ro, cc)
        {
        }

        public IntegerConfigEntry(HInput input, bool ro) : this(input.ReadVarInt(), ro, 0)
        {
        }

        public override int GetEntryType()
        {
            return 3;
        }

        public override void Write(HOutput output)
        {
            output.WriteVarInt((int) GetValue());
        }
    }
}
