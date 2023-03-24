using System;
using System.Collections.Generic;

namespace iMine.Launcher.Serialize.Config.Entry
{
    public class BlockConfigEntry : ConfigEntry
    {
        public BlockConfigEntry(Dictionary<string, ConfigEntry> map, bool ro, int cc) : base(map, ro, cc)
        {
        }

        public BlockConfigEntry(int cc) : base(new Dictionary<string, ConfigEntry>(), false, cc)
        {
        }

        public BlockConfigEntry(HInput input, bool ro) : base(ReadMap(input, ro), ro, 0)
        {
        }

        public override int GetEntryType()
        {
            return 1;
        }

        /*public override Dictionary<string, ConfigEntry> GetValue()
        {
            var value = base.GetValue();
            //return ro ? value : Collections.unmodifiableMap(value); // Already RO
            return value;
        }*/

        /*protected override void UncheckedSetValue(object rawValue)
        {
            var value = (Dictionary<string, ConfigEntry>) rawValue;
            Dictionary<String, ConfigEntry<?>> newValue = new LinkedHashMap<>(value);
            newValue.keySet().stream().forEach(VerifyHelper::verifyIDName);

            // Call super method to actually set new value
            super.uncheckedSetValue(ro ? Collections.unmodifiableMap(newValue) : newValue);
        }*/

        public override void Write(HOutput output)
        {
            var value = (Dictionary<string, ConfigEntry>)GetValue();
            output.WriteLength(value.Count, 0);
            foreach (var entry in value)
            {
                output.WriteString(entry.Key, 255);
                WriteEntry(entry.Value, output);
            }
        }

        public Dictionary<string, ConfigEntry> GetValueAsMap()
        {
            return (Dictionary<string, ConfigEntry>) GetValue();
        }

        public void Clear()
        {
            GetValueAsMap().Clear();
        }

        public T GetEntry<T>(string name) where T : ConfigEntry
        {
            var map = GetValueAsMap();
            var value = map[name];
            if (!(value is T))
                throw new Exception(name);
            return (T) value;
        }

        /*@LauncherAPI
        public <V, E extends ConfigEntry<V>> V getEntryValue(String name, Class<E> clazz) {
            return getEntry(name, clazz).getValue();
        }

        @LauncherAPI
        public boolean hasEntry(String name) {
            return getValue().containsKey(name);
        }

        @LauncherAPI
        public void remove(String name) {
            super.getValue().remove(name);
        }

        @LauncherAPI
        public void setEntry(String name, ConfigEntry<?> entry) {
            super.getValue().put(VerifyHelper.verifyIDName(name), entry);
        }*/

        private static Dictionary<string, ConfigEntry> ReadMap(HInput input, bool ro)
        {
            var entriesCount = input.ReadLength(0);
            var map = new Dictionary<string, ConfigEntry>(entriesCount);
            for (var i = 0; i < entriesCount; i++) {
                //String name = VerifyHelper.verifyIDName(input.readString(255));
                //ConfigEntry<?> entry = readEntry(input, ro);
                var name = input.ReadString(255);
                var entry = ReadEntry(input, ro);

                // Try add entry to map
                map.Add(name, entry);
            }
            return map;
        }
    }
}