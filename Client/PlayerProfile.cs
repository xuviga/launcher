using System;
using System.Text;
using iMine.Launcher.Serialize;
using iMine.Launcher.Serialize.Streaming;

namespace iMine.Launcher.Client
{
    public class PlayerProfile : StreamObject
    {
        public Guid uuid;
        public string username;
        public Texture skin;
        public Texture cloak;

        public PlayerProfile(HInput input)
        {
            uuid = input.ReadGuid();
            username = input.ReadAscii(16);
            skin = input.ReadBoolean() ? new Texture(input) : null; //todo тут баг
            cloak = input.ReadBoolean() ? new Texture(input) : null;
        }

        public PlayerProfile(Guid uuid, string username, Texture skin, Texture cloak)
        {
            this.uuid = uuid;
            this.username = username; //todo проверка на юзернейм
            this.skin = skin;
            this.cloak = cloak;
        }

        public override void Write(HOutput output)
        {
            output.WriteGuid(uuid);
            output.WriteASCII(username, 16);

            output.WriteBoolean(skin != null);
            skin?.Write(output);
            output.WriteBoolean(cloak != null);
            cloak?.Write(output);
        }


        public override string ToString()
        {
            return $"{{{uuid} {username} {skin} {cloak}}}";
        }

        public class Texture : StreamObject
        {
            //private static DigestAlgorithm DIGEST_ALGO = DigestAlgorithm.SHA256;

            public string url;
            public byte[] digest;

            public Texture(string url, byte[] digest)
            {
                this.url = url; //todo verify
                this.digest = digest; //todo verify
            }

            public Texture(string url, bool cloak)
            {
                this.url = url; //todo verify

                // Fetch texture
                byte[] texture;
                /*try (InputStream input = IOHelper.newInput(new URL(url))) {
                    texture = IOHelper.read(input);
                }
                try (ByteArrayInputStream input = new ByteArrayInputStream(texture)) {
                    IOHelper.readTexture(input, cloak); // Verify texture
                }

                // Get digest of texture
                digest = SecurityHelper.digest(DIGEST_ALGO, new URL(url));*/
            }

            public Texture(HInput input)
            {
                url = input.ReadAscii(2048);
                digest = input.ReadByteArray(-32);
            }

            public override void Write(HOutput output)
            {
                output.WriteASCII(url, 2048);
                output.WriteByteArray(digest, -32);
            }

            public override string ToString()
            {
                return $"{{{url}}}";
            }
        }
    }
}