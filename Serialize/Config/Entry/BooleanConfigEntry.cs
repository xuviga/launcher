namespace iMine.Launcher.Serialize.Config.Entry
{
    public class BooleanConfigEntry : ConfigEntry
    {
        public BooleanConfigEntry(bool value, bool ro, int cc) : base(value, ro, cc)
        {
        }

        public BooleanConfigEntry(HInput input, bool ro) : this(input.ReadBoolean(), ro, 0)
        {
        }

        public override int GetEntryType()
        {
            return 2;
        }

        public override void Write(HOutput output)
        {
            output.WriteBoolean((bool) GetValue());
        }
    }
}