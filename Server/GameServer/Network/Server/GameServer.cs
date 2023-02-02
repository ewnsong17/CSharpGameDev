using GameServer.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Network.Server
{
	class GameServer
	{
		public Tuple<GameClient, GameClient> playerPair = new Tuple<GameClient, GameClient>(null, null);

		public static GameServer Instance;

		public static GameServer GetInstance()
		{
			if (Instance == null)
			{
				Instance = new GameServer();
			}

			return Instance;
		}

		public bool AddPlayer(GameClient client)
		{
			if (playerPair.Item1 == null)
			{
				playerPair = new Tuple<GameClient, GameClient>(client, null);
				return true;
			}
			else if (playerPair.Item2 == null)
			{
				playerPair = new Tuple<GameClient, GameClient>(playerPair.Item1, client);
				return true;
			}

			return false;
		}

		public void RemovePlayer(GameClient client)
		{
			if (playerPair.Item1 == client)
			{
				playerPair = new Tuple<GameClient, GameClient>(null, playerPair.Item2);
			}
			else if (playerPair.Item2 == client)
			{
				playerPair = new Tuple<GameClient, GameClient>(playerPair.Item1, null);
			}
		}

		public void Broadcast(PacketUtil pUtil)
		{
			if (playerPair.Item1 != null)
			{
				playerPair.Item1.Send(pUtil);
			}

			if (playerPair.Item2 != null)
			{
				playerPair.Item2.Send(pUtil);
			}
		}

		public void RequestPlayerExist()
		{
			var pUtil = new PacketUtil(SendHandler.ResultPlayerExist);
			pUtil.SetBool(!AddPlayer(null));
			Broadcast(pUtil);
		}
	}
}
