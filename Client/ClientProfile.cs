using System;
using iMine.Launcher.Hasher;
using iMine.Launcher.Serialize;
using iMine.Launcher.Serialize.Config;
using iMine.Launcher.Serialize.Config.Entry;

namespace iMine.Launcher.Client
{
    public class ClientProfile : ConfigObject
    {
        public static IAdapter<ClientProfile> RoAdapter = new ClientConverted();
        public static readonly FileNameMatcher AssetMatcher = new FileNameMatcher(
            new string[0], new[]{ "indexes", "objects" }, new string[0]);
        public static readonly FileNameMatcher LibrariesMatcher = new StrongFileNameMatcher();

        // Version
        private readonly StringConfigEntry version;

        private readonly StringConfigEntry assetIndex;
        private readonly StringConfigEntry assetDir;
        private readonly StringConfigEntry librariesDir;
        private readonly BooleanConfigEntry isPrivate;

        // Client
        private readonly IntegerConfigEntry sortIndex;
        private readonly StringConfigEntry dir;
        private readonly StringConfigEntry title;


        private readonly StringConfigEntry serverAddress;
        private readonly IntegerConfigEntry serverPort;

        public readonly IntegerConfigEntry ramMin;
        public readonly IntegerConfigEntry systemRamMin;
        public readonly IntegerConfigEntry systemRamRec;
        public readonly IntegerConfigEntry ramRec;

        //  Updater and client watch service
        private readonly ListConfigEntry update;

        private readonly ListConfigEntry updateExclusions;
        private readonly ListConfigEntry updateVerify;
        private readonly BooleanConfigEntry updateFastCheck;

        // Client launcher
        private readonly StringConfigEntry mainClass;

        private readonly ListConfigEntry jvmArgs;
        private readonly ListConfigEntry classPath;
        private readonly ListConfigEntry clientArgs;

        public ClientProfile(BlockConfigEntry block) : base(block)
        {
            // Version
            version = block.GetEntry<StringConfigEntry>("version");
            assetIndex = block.GetEntry<StringConfigEntry>("assetIndex");
            assetDir = block.GetEntry<StringConfigEntry>("assetDir");
            librariesDir = block.GetEntry<StringConfigEntry>("librariesDir");
            isPrivate = block.GetEntry<BooleanConfigEntry>("isPrivate");

            // Client
            sortIndex = block.GetEntry<IntegerConfigEntry>("sortIndex");
            dir = block.GetEntry<StringConfigEntry>("dir");
            title = block.GetEntry<StringConfigEntry>("title");
            serverAddress = block.GetEntry<StringConfigEntry>("serverAddress");
            serverPort = block.GetEntry<IntegerConfigEntry>("serverPort");
            ramMin = block.GetEntry<IntegerConfigEntry>("ramMin");
            systemRamMin = block.GetEntry<IntegerConfigEntry>("systemRamMin");
            systemRamRec = block.GetEntry<IntegerConfigEntry>("systemRamRec");
            ramRec = block.GetEntry<IntegerConfigEntry>("ramRec");

            //  Updater and client watch service
            update = block.GetEntry<ListConfigEntry>("update");
            updateVerify = block.GetEntry<ListConfigEntry>("updateVerify");
            updateExclusions = block.GetEntry<ListConfigEntry>("updateExclusions");
            updateFastCheck = block.GetEntry<BooleanConfigEntry>("updateFastCheck");

            // Client launcher
            mainClass = block.GetEntry<StringConfigEntry>("mainClass");
            classPath = block.GetEntry<ListConfigEntry>("winClassPath");
            jvmArgs = block.GetEntry<ListConfigEntry>("jvmArgs");
            clientArgs = block.GetEntry<ListConfigEntry>("clientArgs");
        }

        public ClientProfile(HInput input, bool ro) : this(new BlockConfigEntry(input, ro))
        {
        }

        public override string ToString()
        {
            return title.GetValue().ToString();
        }

        public string GetAssetIndex()
        {
            return assetIndex.GetValue().ToString();
        }

        public string GetAssetDir()
        {
            return assetDir.GetValue().ToString();
        }

        public string GetLibrariesDir()
        {
            return librariesDir.GetValue().ToString();
        }

        public bool IsPrivate()
        {
            return isPrivate.GetValue().Equals(true);
        }

        public string[] GetClassPath()
        {
            return classPath.GetEntries<string>().ToArray();
        }

        public string[] GetClientArgs()
        {
            return clientArgs.GetEntries<string>().ToArray();
        }

        public FileNameMatcher GetClientUpdateMatcher()
        {
            var updateArray = update.GetEntries<string>().ToArray();
            var verifyArray = updateVerify.GetEntries<string>().ToArray();
            var exclusionsArray = updateExclusions.GetEntries<string>().ToArray();
            return new FileNameMatcher(updateArray, verifyArray, exclusionsArray);
        }

        public string[] GetJvmArgs()
        {
            return jvmArgs.GetEntries<string>().ToArray();
        }

        public string GetMainClass()
        {
            return (string) mainClass.GetValue();
        }

        public string GetServerAddress()
        {
            return (string) serverAddress.GetValue();
        }

        public int GetServerPort()
        {
            return (int) serverPort.GetValue();
        }

        /*public InetSocketAddress getServerSocketAddress() {
            return InetSocketAddress.createUnresolved(getServerAddress(), getServerPort());
        }*/

        public int GetSortIndex()
        {
            return (int) sortIndex.GetValue();
        }

        public string GetDir()
        {
            return dir.GetValue().ToString();
        }

        public string GetTitle()
        {
            return (string) title.GetValue();
        }

        public void SetTitle(string newTitle)
        {
            title.SetValue(newTitle);
        }

        public McVersion GetVersion()
        {
            return McVersion.ByName((string) version.GetValue());
        }

        public void SetVersion(McVersion mcVersion)
        {
            version.SetValue(mcVersion.Name);
        }

        public bool IsUpdateFastCheck()
        {
            return (bool) updateFastCheck.GetValue();
        }

        /*public void verify() {
            // Version
            getVersion();
            IOHelper.verifyFileName(getAssetIndex());

            // Client
            VerifyHelper.verify(getTitle(), VerifyHelper.NOT_EMPTY, "Profile title can't be empty");
            VerifyHelper.verify(getServerAddress(), VerifyHelper.NOT_EMPTY, "Server address can't be empty");
            VerifyHelper.verifyInt(getServerPort(), VerifyHelper.range(0, 65535), "Illegal server port: " + getServerPort());

            //  Updater and client watch service
            update.verifyOfType(Type.STRING);
            updateVerify.verifyOfType(Type.STRING);
            updateExclusions.verifyOfType(Type.STRING);

            // Client launcher
            jvmArgs.verifyOfType(Type.STRING);
            classPath.verifyOfType(Type.STRING);
            clientArgs.verifyOfType(Type.STRING);
            VerifyHelper.verify(getTitle(), VerifyHelper.NOT_EMPTY, "Main class can't be empty");
        }*/ //todo repair

        public override void Write(HOutput output)
        {
            throw new NotImplementedException();
        }

        public class ClientConverted : IAdapter<ClientProfile>
        {
            public ClientProfile Convert(HInput input)
            {
                return new ClientProfile(input, true);
            }
        }
    }
}