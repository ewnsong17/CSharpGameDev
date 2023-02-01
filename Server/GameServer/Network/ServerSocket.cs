using GameServer.Client;
using GameServer.Util;
using System;
using System.Net;
using System.Net.Sockets;

namespace GameServer.Network
{
    class ServerSocket : Socket
    {
        public static ServerSocket Instance;
        public const short PACKET_MAX_SIZE = 0x400;
        public const short Port = 4040;

        public ServerSocket() : base(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
        {
            base.Bind(new IPEndPoint(IPAddress.Any, Port));
            base.Listen(0);
            BeginAccept(Accept, this);

            Logger.Log(LoggerFlag.Info, string.Format("서버를 {0} 포트에서 연결합니다.", Port));
        }

        private void Accept(IAsyncResult result)
        {
            var client = new GameClient(EndAccept(result));
            BeginAccept(Accept, this);
        }

        public static ServerSocket GetInstance()
        {
            if (Instance == null)
            {
                Instance = new ServerSocket();
            }
            return Instance;
        }
    }
}
