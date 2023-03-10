using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using GameClient.Network.Client;

namespace GameClient.network
{
	public class GameClient : Socket
	{
		private MainWindow Window;
		public byte[] Buffer = new byte[ClientSocket.PACKET_MAX_SIZE];

		public List<GameCard> CardList = new List<GameCard>();

		public bool bStand = false;

		public GameClient(MainWindow window) : base(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
		{
			this.Window = window;
			base.BeginConnect(new IPEndPoint(IPAddress.Parse(ClientSocket.IP), ClientSocket.Port), Connect, this);
		}

		private void Connect(IAsyncResult result)
		{
			try
			{
				base.EndConnect(result);
				base.BeginReceive(Buffer, 0, Buffer.Length, SocketFlags.None, UserPacketReceived, this);
			}
			catch
			{
				if (MessageBoxResult.OK == MessageBox.Show("서버에 연결할 수 없습니다. 게임을 종료합니다.", "알림", MessageBoxButton.OK, MessageBoxImage.Error))
				{
					Environment.Exit(0);
				}
			}
		}

		private void UserPacketReceived(IAsyncResult result)
		{
			try
			{
				if (Connected)
				{
					int size = EndReceive(result);
					int packetLen = GetInt(Buffer);

					if (size == 4 + packetLen)
					{
						byte[] packet = new byte[packetLen];
						Array.Copy(Buffer, 4, packet, 0, packetLen);

						PacketUtil pUtil = new PacketUtil(packet);

						ClientSocket.HandlePacket(this, pUtil);
					}

					base.BeginReceive(Buffer, 0, Buffer.Length, SocketFlags.None, UserPacketReceived, this);
				}
				else
				{
					if (MessageBoxResult.OK == MessageBox.Show("서버에 연결할 수 없습니다. 게임을 종료합니다.", "알림", MessageBoxButton.OK, MessageBoxImage.Error))
					{
						Environment.Exit(0);
					}
				}
			}
			catch (Exception ex)
			{
				Trace.WriteLine(ex.ToString());
			}
		}

		public static int GetInt(byte[] byteCode)
		{
			int a1 = (byteCode[0] << 0);
			int a2 = (byteCode[1] << 8);
			int a3 = (byteCode[2] << 16);
			int a4 = (byteCode[3] << 24);
			return a1 + a2 + a3 + a4;
		}

		public void ServerConnected(PacketUtil pUtil)
		{
			bool IsServerConnected = pUtil.GetBool();
			if (!IsServerConnected)
			{
				if (MessageBoxResult.OK == MessageBox.Show("2인용 게임입니다. 2명 이상 참가하실 수 없습니다.", "알림", MessageBoxButton.OK, MessageBoxImage.Error))
				{
					GameClosed();
				}
			}
			else
			{
				Window.ChangeScene();
			}
		}

		public void ResultPlayerExist(PacketUtil pUtil)
		{
			if (pUtil.GetBool())
			{
				Window.RemoveWaitLabel();
			}
		}

		public void ResultGameInit(PacketUtil pUtil)
		{
			CardList.Clear();

			int count = pUtil.GetInt();

			for (int i = 0; i < count; i++)
			{
				var card = new GameCard
				{
					color = pUtil.GetInt(),
					number = pUtil.GetInt()
				};

				CardList.Add(card);
			}

			Window.GameInit();
		}

		public void ResultOtherDisconnect()
		{
			if (MessageBoxResult.OK == MessageBox.Show("상대방의 연결이 끊겼습니다.", "알림", MessageBoxButton.OK, MessageBoxImage.Error))
			{
				GameClosed();

				Environment.Exit(0);
			}
		}

		public void ResultHit(PacketUtil pUtil)
		{
			var card = new GameCard
			{
				color = pUtil.GetInt(),
				number = pUtil.GetInt()
			};

			CardList.Add(card);

			Window.ReDrawCard(card);
		}

		public void ResultOppositeHit(PacketUtil pUtil)
		{
			int cardCount = pUtil.GetInt();

			Window.ReDrawOppositeCard(cardCount);
		}

		public void ResultStand()
		{
			Window.ContentStand();
		}

		public void ResultOppositeStand()
		{
			Window.ContentOppositeStand();
		}

		public void ResultGameEnd(PacketUtil pUtil)
		{
			bool bDraw = pUtil.GetBool();
			bool bWin = pUtil.GetBool();

			int count = pUtil.GetInt();
			List<GameCard> cardList = new List<GameCard>();

			for (int i = 0; i < count; i++)
			{
				cardList.Add(
					new GameCard
					{
						color = pUtil.GetInt(),
						number = pUtil.GetInt()
					}
				);
			}

			Window.ContentGameEnd(bDraw, bWin, cardList);
		}

		public void ResultRetry()
		{
			var pUtil = new PacketUtil(SendHandler.RequestAskRetry);
			if (MessageBoxResult.Yes == MessageBox.Show("상대방이 게임을 다시하고자 합니다. 수락하시겠습니까?", "알림", MessageBoxButton.YesNo, MessageBoxImage.Question))
			{
				pUtil.SetBool(true);
			}
			else
			{
				pUtil.SetBool(false);
			}

			Send(pUtil);
		}

		public void ResultAskRetry(PacketUtil pUtil)
		{
			Window.ContentRetry(pUtil.GetBool());
		}

		public void Send(PacketUtil pUtil)
		{
			try
			{
				byte[] data = pUtil.GetSendData();
				Send(data, data.Length, SocketFlags.None);
			}
			catch
			{
				Close();
			}
		}

		public void GameClosed()
		{
			var pUtil = new PacketUtil(SendHandler.ClientClosed);
			pUtil.SetBool(true);

			Send(pUtil);

		}

		public void RequestPlayerExist()
		{
			var pUtil = new PacketUtil(SendHandler.RequestPlayerExist);

			Send(pUtil);
		}

		public void RequestHit()
		{
			if (!bStand)
			{
				var pUtil = new PacketUtil(SendHandler.RequestHit);
				Send(pUtil);
			}
		}

		public void RequestStand()
		{
			if (!bStand)
			{
				bStand = true;

				var pUtil = new PacketUtil(SendHandler.RequestStand);
				Send(pUtil);
			}
		}

		public void RequestRetryGame()
		{
			var pUtil = new PacketUtil(SendHandler.RequestRetry);
			Send(pUtil);
		}
	}
}
