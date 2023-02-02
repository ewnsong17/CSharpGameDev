using GameServer.Network;
using GameServer.Network.Server;
using GameServer.Util;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace GameServer.Client
{
	class GameClient
	{
        //통신 관련 정보
        public Socket Socket;
        public byte[] Buffer = new byte[ServerSocket.PACKET_MAX_SIZE];
        public long LastPingTime;

        //게임 관련 정보
        public List<GameCard> CardList = new List<GameCard>();
        public bool bStand = false;

        public GameClient(Socket socket)
        {
            this.Socket = socket;
            this.Socket.BeginReceive(Buffer, 0, ServerSocket.PACKET_MAX_SIZE, SocketFlags.None, UserPacketReceived, this);

            Initialization();
        }

        private void UserPacketReceived(IAsyncResult result)
        {
            try
            {
                if (Socket.Connected)
                {
                    int size = Socket.EndReceive(result);
                    int packetLen = ServerHandler.GetInt(Buffer);

                    if (size == 4 + packetLen)
                    {
                        //받은 패킷 중 헤더부분 제외하여 다시 배열에 담음
                        byte[] packet = new byte[packetLen];
                        Array.Copy(Buffer, 4, packet, 0, packetLen);

                        var pUtil = new PacketUtil(packet);

                        ServerHandler.HandlePacket(this, pUtil);
                    }

                    if (Socket.Connected)
                    {
                        this.Socket.BeginReceive(Buffer, 0, ServerSocket.PACKET_MAX_SIZE, SocketFlags.None, UserPacketReceived, this);
                    }
                }
                else
                {
                    Disconnect();
                }
            }
            catch (Exception ex)
            {
                Logger.Log(LoggerFlag.Error, ex.ToString());
            }
        }

        public void Send(PacketUtil pUtil)
        {
            try
            {
                SendHandler header = GetHeader(
                    ServerHandler.GetInt(pUtil.GetBytes()));

                byte[] data = pUtil.GetSendData();
                Socket.Send(data, data.Length, SocketFlags.None);

                StringBuilder str = new StringBuilder("새로운 패킷 데이터를 보냈습니다.\n");
                str.AppendLine("##### bytes : " + pUtil.PrintPacket());
                str.AppendLine("##### ascii : " + Encoding.UTF8.GetString(pUtil.GetBytes()));
                Logger.Log(LoggerFlag.Debug, str.ToString());
            }
            catch
            {
                Socket.Close();
            }
        }

        public static SendHandler GetHeader(int header)
        {
            foreach (SendHandler h in Enum.GetValues(typeof(SendHandler)))
            {
                if (((int)h) == header)
                {
                    return h;
                }
            }
            return SendHandler.Null;
        }

        /// <summary>
        /// 유저 서버 접속 시 정상 접속 검증 체크 함수
        /// </summary>
        public void Initialization()
        {
            IPEndPoint remoteAddr = (IPEndPoint)Socket.RemoteEndPoint;
            Logger.Log(LoggerFlag.Info, string.Format("/{0} 에서 새 유저가 서버에 접속을 시도했습니다.", remoteAddr.Address.ToString()));

            var Server = Network.Server.GameServer.GetInstance();

            var pUtil = new PacketUtil(SendHandler.ClientConnected);
            pUtil.SetBool(Server.AddPlayer(this));

            this.LastPingTime = GameUtil.CurrentTimeMillis();
            //나중에 패킷 암호화 정보를 보낸다고 하면, 여기에 보내면 좋을 것 같네요.
            Send(pUtil);
        }

        /// <summary>
        /// 유저 접속 해제
        /// </summary>
        public void Disconnect()
        {
            var server = Network.Server.GameServer.GetInstance();

            var other = server.RemovePlayer(this);

            if (other != null)
            {
                var pUtil = new PacketUtil(SendHandler.ResultOtherDisconnect);
                other.Send(pUtil);
            }

            Socket.Close();
        }
    }
}
