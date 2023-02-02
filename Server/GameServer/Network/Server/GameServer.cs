using GameServer.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameServer.Network.Server
{
	class GameServer
	{
		public Tuple<GameClient, GameClient> PlayerPair = new Tuple<GameClient, GameClient>(null, null);
		public List<GameCard> CardList = new List<GameCard>();
		public static GameServer Instance;

		public GameServer()
		{
			InitCardList();
		}

		public static GameServer GetInstance()
		{
			if (Instance == null)
			{
				Instance = new GameServer();
			}

			return Instance;
		}

		public void InitCardList()
		{
			for (int color = 0; color < 3; color++)
			{
				for (int number = 1; number <= 13; number++)
				{
					var card = new GameCard
					{
						color = color,
						number = number
					};

					CardList.Add(card);
				}
			}
		}

		public GameClient GetPlayer(GameClient client)
		{
			if (PlayerPair.Item1 == client)
			{
				return PlayerPair.Item1;
			}
			else if (PlayerPair.Item2 == client)
			{
				return PlayerPair.Item2;
			}

			return null;
		}

		public bool AddPlayer(GameClient client)
		{
			if (PlayerPair.Item1 == null)
			{
				PlayerPair = new Tuple<GameClient, GameClient>(client, null);
				return true;
			}
			else if (PlayerPair.Item2 == null)
			{
				PlayerPair = new Tuple<GameClient, GameClient>(PlayerPair.Item1, client);
				return true;
			}

			return false;
		}

		public GameClient RemovePlayer(GameClient client)
		{
			if (PlayerPair.Item1 == client)
			{
				PlayerPair = new Tuple<GameClient, GameClient>(null, PlayerPair.Item2);
				return PlayerPair.Item2;
			}
			else if (PlayerPair.Item2 == client)
			{
				PlayerPair = new Tuple<GameClient, GameClient>(PlayerPair.Item1, null);
				return PlayerPair.Item1;
			}

			return null;
		}

		public void Broadcast(PacketUtil pUtil)
		{
			if (PlayerPair.Item1 != null)
			{
				PlayerPair.Item1.Send(pUtil);
			}

			if (PlayerPair.Item2 != null)
			{
				PlayerPair.Item2.Send(pUtil);
			}
		}

		public void RequestPlayerExist()
		{
			var pUtil = new PacketUtil(SendHandler.ResultPlayerExist);

			bool bGameStart = !AddPlayer(null);
			pUtil.SetBool(bGameStart);
			Broadcast(pUtil);

			if (bGameStart)
			{
				for (int i = 0; i < 4; i++)
				{
					//플레이어에게 카드 2개씩 분배

					var card = CardList[new Random().Next(CardList.Count)];

					if (i < 2)
					{
						PlayerPair.Item1.CardList.Add(card);
					}
					else
					{
						PlayerPair.Item2.CardList.Add(card);
					}

					//남은 카드 갯수에서 제외
					CardList.Remove(card);
				}

				GameClient[] player_list = { PlayerPair .Item1, PlayerPair.Item2 };

				foreach (GameClient player in player_list)
				{
					//자신의 카드 정보만 보낸다.
					pUtil = new PacketUtil(SendHandler.ResultGameInit);

					var list = player.CardList;
					pUtil.SetInt(list.Count);
					for (int i = 0; i < list.Count; i++)
					{
						pUtil.SetInt(list[i].color);
						pUtil.SetInt(list[i].number);
					}

					player.Send(pUtil);
				}
			}
		}

		public void RequestHit(GameClient client)
		{
			var player = GetPlayer(client);
			if (player != null)
			{
				var card = CardList[new Random().Next(CardList.Count)];

				player.CardList.Add(card);

				CheckBlend(player);

				var pUtil = new PacketUtil(SendHandler.ResultHit);

				pUtil.SetBool(player.bBlend);

				var list = player.CardList;
				pUtil.SetInt(list.Count);
				for (int i = 0; i < list.Count; i++)
				{
					pUtil.SetInt(list[i].color);
					pUtil.SetInt(list[i].number);
				}

				player.Send(pUtil);
			}
		}

		public void CheckBlend(GameClient client)
		{
			int cardVal = client.CardList.Select(card => card.number).Sum();

			client.bBlend = cardVal >= 22;
		}
	}
}
