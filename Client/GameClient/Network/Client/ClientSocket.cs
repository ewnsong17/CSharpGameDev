using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;

namespace GameClient.network
{
	public class ClientSocket
	{
		public const string IP = "127.0.0.1";
		public const short Port = 4040;
		public const short PACKET_MAX_SIZE = 0x4000;

		public static GameClient Instance;

		public static GameClient GetInstance(MainWindow window = null)
		{
			if (Instance == null)
			{
				Instance = new GameClient(window);
			}

			return Instance;
		}

        public static void HandlePacket(GameClient client, PacketUtil pUtil)
        {
            Trace.WriteLine("새 패킷을 받았습니다 : " + pUtil.PrintPacket());

            int h = pUtil.GetInt();
            var header = GetHeader(h);

            switch (header)
            {
                case ReceiveHandler.ClientConnected:
                    client.ServerConnected(pUtil);
                    break;
                case ReceiveHandler.ResultPlayerExist:
                    client.ResultPlayerExist(pUtil);
                    break;
                default:
                    Trace.WriteLine(string.Format("아직 지정되지 않은 헤더입니다 : [{0} | {1}]", h, header));
                    break;
            }
        }

        public static ReceiveHandler GetHeader(int header)
        {
            foreach (ReceiveHandler h in Enum.GetValues(typeof(ReceiveHandler)))
            {
                if (((int)h) == header)
                {
                    return h;
                }
            }
            return ReceiveHandler.Null;
        }
    }
}
