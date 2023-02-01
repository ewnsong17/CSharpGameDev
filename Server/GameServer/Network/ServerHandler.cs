using GameServer.Client;
using GameServer.Util;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace GameServer.Network
{
    class ServerHandler
    {
        /// <summary>
        /// 요청 받은 패킷을 헤더에 따라 적절한 핸들러로 분류하여 처리
        /// </summary>
        /// <param name="client"></param>
        /// <param name="pUtil"></param>
        public static void HandlePacket(GameClient client, PacketUtil pUtil)
        {

            int h = pUtil.GetInt();
            var header = GetHeader(h);

            StringBuilder str = new StringBuilder("새로운 패킷 데이터를 받았습니다.\n");
            str.AppendLine("#### header : " + header);
            str.AppendLine("##### bytes : " + pUtil.PrintPacket());
            str.AppendLine("##### ascii : " + Encoding.UTF8.GetString(pUtil.GetBytes()));
            Logger.Log(LoggerFlag.Debug, str.ToString());

            switch (header)
            {
                case ReceiveHandler.ClientClosed:
                    client.Disconnect();
                    break;
                default:
                    Logger.Log(LoggerFlag.Debug, string.Format("아직 지정되지 않은 헤더입니다 : [{0} | {1}]", h, header));
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

        public static int GetInt(byte[] byteCode)
        {
            int a1 = (byteCode[0] << 0);
            int a2 = (byteCode[1] << 8);
            int a3 = (byteCode[2] << 16);
            int a4 = (byteCode[3] << 24);
            return a1 + a2 + a3 + a4;
        }
    }
}
