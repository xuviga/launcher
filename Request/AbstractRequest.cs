using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using iMine.Launcher.Serialize;
using iMine.Launcher.Utils;

namespace iMine.Launcher.Request
{
    public abstract class AbstractRequest<T>
    {
        private static readonly IPEndPoint EndPoint;
        private int attempt = 1;

        static AbstractRequest()
        {
            var ipHostInfo = Dns.GetHostEntry(Config.ServerIp);
            var ipAddress = ipHostInfo.AddressList[0];
            EndPoint = new IPEndPoint(ipAddress, Config.ServerPort);
        }

        public virtual T SendRequest()
        {
            Socket socket;
            for (;;)
            {
                try
                {
                    socket = new Socket(EndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp)
                    {
                        SendTimeout = 30000,
                        ReceiveTimeout = 30000
                    };
                    socket.Connect(EndPoint);
                    break;
                }
                catch
                {
                    Console.WriteLine("Не удалось подключиться к серверам iMine.\nПробуем снова... #" + attempt++);
                    Thread.Sleep(500);
                }
            }
            var stream = new NetworkStream(socket);
            var input = new HInput(stream);
            var output = new HOutput(stream);
            WriteHandshake(input, output);
            return HandleResponse(input, output);
        }

        private void WriteHandshake(HInput input, HOutput output)
        {
            output.WriteRightInt(RequestHelper.ProtocolMagic);

            output.WriteByteArray(new byte[1], RequestHelper.RsaKeyLength + 1);

            output.WriteVarInt((int)GetRequestType());
            output.Flush();

            if (!input.ReadBoolean())
            {
                Console.WriteLine("Serverside not accepted this connection");
                App.WriteImportantException(new Exception());
                GoogleAnalytics.Exception("handshake",null,"",true);
            }
        }

        protected string ReadError(HInput input)
        {
            var error = input.ReadString(0);
            if (error.Length!=0)
                RequestError(error);
            return error;
        }

        public static void RequestError(string message)
        {
            throw new Exception(message);
        }

        protected abstract T HandleResponse(HInput input, HOutput output);
        protected abstract RequestType GetRequestType();
    }
}