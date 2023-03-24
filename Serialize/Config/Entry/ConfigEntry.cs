using System;
using iMine.Launcher.Serialize.Streaming;

namespace iMine.Launcher.Serialize.Config.Entry
{
    public abstract class ConfigEntry : StreamObject
    {
        public readonly bool Ro;
        private readonly string[] comments;
        private object value;

        protected ConfigEntry(object value, bool ro, int cc)
        {
            Ro = ro;
            comments = new string[cc];
            UncheckedSetValue(value);
        }

        public abstract int GetEntryType();

        public string GetComment(int i)
        {
            if (i < 0)
                i += comments.Length;
            return i >= comments.Length ? null : comments[i];
        }

        public object GetValue()
        {
            return value;
        }

        public void SetValue(object value)
        {
            EnsureWritable();
            UncheckedSetValue(value);
        }

        public void SetComment(int i, string comment)
        {
            comments[i] = comment;
        }

        protected void EnsureWritable()
        {
            if (Ro)
                throw new InvalidOperationException();
        }

        protected void UncheckedSetValue(object value)
        {
            this.value = value;
        }

        protected static ConfigEntry ReadEntry(HInput input, bool ro)
        {
            var type = input.ReadVarInt();
            switch (type)
            {
                case 2:
                    return new BooleanConfigEntry(input, ro);
                case 3:
                    return new IntegerConfigEntry(input, ro);
                case 4:
                    return new StringConfigEntry(input, ro);
                case 5:
                    return new ListConfigEntry(input, ro);
                case 1:
                    return new BlockConfigEntry(input, ro);
                default:
                    throw new Exception("Unsupported config entry type: " + type);
            }
        }

        protected static void WriteEntry(ConfigEntry entry, HOutput output)
        {
            output.WriteVarInt(entry.GetEntryType());
            entry.Write(output);
        }
    }
}