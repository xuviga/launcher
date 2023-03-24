using System;
using iMine.Launcher.Helper;

namespace iMine.Launcher.Client
{
    public class McVersion : IComparable<McVersion>
    {
        private static readonly Utils.Collections.NDictionary<string, McVersion> Versions=new Utils.Collections.NDictionary<string, McVersion>();

        public static readonly McVersion Mc1_5_2 = new McVersion("1.5.2", 61);
        public static readonly McVersion Mc1_6_4 = new McVersion("1.6.4", 78);
        public static readonly McVersion Mc1_7_2 = new McVersion("1.7.2", 4);
        public static readonly McVersion Mc1_7_10 = new McVersion("1.7.10", 5);
        public static readonly McVersion Mc1_8_9 = new McVersion("1.8.9", 47);
        public static readonly McVersion Mc1_9_4 = new McVersion("1.9.4", 110);
        public static readonly McVersion Mc1_10_2 = new McVersion("1.10.2", 210);
        public static readonly McVersion Mc1_11_2 = new McVersion("1.11.2", 316);
        public static readonly McVersion Mc1_12_1 = new McVersion("1.12.1", 338);
        public static readonly McVersion Mc1_12_2 = new McVersion("1.12.2", 340);

        public readonly string Name;
        public readonly int Protocol;

        private McVersion(string name, int protocol)
        {
            Name = name;
            Protocol = protocol;
            Versions[name] = this;
        }

        public override string ToString()
        {
            return Name;
        }

        public static McVersion ByName(string name)
        {
            return VerifyHelper.GetMapValue(Versions, name, $"Unknown client version: '{name}'");
        }

        public int CompareTo(McVersion other)
        {
            return Protocol.CompareTo(other.Protocol);
        }
    }
}