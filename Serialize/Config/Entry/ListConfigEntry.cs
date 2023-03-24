using System.Collections.Generic;

namespace iMine.Launcher.Serialize.Config.Entry
{
    public class ListConfigEntry : ConfigEntry
    {
        public ListConfigEntry(List<ConfigEntry> value, bool ro, int cc) : base(value, ro, cc)
        {
        }

        public ListConfigEntry(HInput input, bool ro) : base(ReadList(input, ro), ro, 0)
        {
        }

        public override int GetEntryType()
        {
            return 5;
        }

        /*@Override
        protected void uncheckedSetValue(List<ConfigEntry<?>> value) {
            List<ConfigEntry<?>> list = new ArrayList<>(value);
            super.uncheckedSetValue(ro ? Collections.unmodifiableList(list) : list);
        }*/

        public override void Write(HOutput output)
        {
            var value = (List<ConfigEntry>) GetValue();
            output.WriteLength(value.Count, 0);
            foreach (var element in value)
                WriteEntry(element, output);
        }

        public List<T> GetEntries<T>()
        {
            return ((List<ConfigEntry>) GetValue()).ConvertAll(it=>(T)it.GetValue());
        }

        /*@LauncherAPI
        public void verifyOfType(Type type) {
            if (getValue().stream().anyMatch(e -> e.getType() != type)) {
                throw new IllegalArgumentException("List type mismatch: " + type.name());
            }
        }*/

        private static List<ConfigEntry> ReadList(HInput input, bool ro)
        {
            var elementsCount = input.ReadLength(0);
            var list = new List<ConfigEntry>(elementsCount);
            for (var i = 0; i < elementsCount; i++)
            {
                list.Add(ReadEntry(input, ro));
            }

            return list;
        }
    }
}
