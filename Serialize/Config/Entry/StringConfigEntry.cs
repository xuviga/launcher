
namespace iMine.Launcher.Serialize.Config.Entry
{
    public class StringConfigEntry : ConfigEntry
    {
        public StringConfigEntry(string value, bool ro, int cc) : base(value, ro, cc)
        {
        }

        public StringConfigEntry(HInput input, bool ro) : this(input.ReadString(0), ro, 0)
        {
        }

        public override int GetEntryType()
        {
            return 4;
        }

        /*@Override
        protected void uncheckedSetValue(String value) {
            super.uncheckedSetValue(value);
        }*/

        public override void Write(HOutput output)
        {
            output.WriteString((string) GetValue(), 0);
        }
    }
}