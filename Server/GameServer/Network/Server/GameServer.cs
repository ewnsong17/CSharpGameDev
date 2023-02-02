using GameServer.Client;
using GameServer.Util;
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
			CardList.Clear();
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

		public Tuple<GameClient, GameClient> GetPlayers(GameClient client)
		{
			if (PlayerPair.Item1 == client)
			{
				return PlayerPair;
			}
			else if (PlayerPair.Item2 == client)
			{
				return new Tuple<GameClient, GameClient>(PlayerPair.Item2, PlayerPair.Item1);
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
				InitCardList();
				PlayerPair.Item1.CardList.Clear();
				PlayerPair.Item2.CardList.Clear();

				PlayerPair.Item1.bStand = false;
				PlayerPair.Item2.bStand = false;

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
			var players = GetPlayers(client);
			if (players != null)
			{
				//무조건 내가 앞에 옴
				var player = players.Item1;

				var card = CardList[new Random().Next(CardList.Count)];

				player.CardList.Add(card);

				var pUtil = new PacketUtil(SendHandler.ResultHit);

				pUtil.SetInt(card.color);
				pUtil.SetInt(card.number);

				player.Send(pUtil);

				//적은 무조건 뒤쪽에 옴
				var opposite = players.Item2;

				pUtil = new PacketUtil(SendHandler.ResultOppositeHit);

				pUtil.SetInt(player.CardList.Count);

				opposite.Send(pUtil);

				CalcGameEnd(client);
			}
		}

		public void RequestStand(GameClient client)
		{
			var players = GetPlayers(client);
			if (players != null)
			{
				if (!players.Item1.bStand)
				{
					players.Item1.bStand = true;

					if (players.Item1.bStand && players.Item2.bStand)
					{
						CalcGameEnd(client);
					}
					else
					{
						var pUtil = new PacketUtil(SendHandler.ResultStand);
						players.Item1.Send(pUtil);

						pUtil = new PacketUtil(SendHandler.ResultOppositeStand);
						players.Item2.Send(pUtil);
					}
				}
			}
		}

		public void RequestGameEnd(GameClient client)
		{
			//무승부가 아닐 경우
			if (client != null)
			{
				var players = GetPlayers(client);
				if (players != null)
				{
					var winner = players.Item1;
					var loser = players.Item2;

					var pUtil = new PacketUtil(SendHandler.ResultGameEnd);
					pUtil.SetBool(false);
					pUtil.SetBool(true);

					winner.Send(pUtil);

					pUtil = new PacketUtil(SendHandler.ResultGameEnd);
					pUtil.SetBool(false);
					pUtil.SetBool(false);

					loser.Send(pUtil);
				}
			}
			else
			{
				var pUtil = new PacketUtil(SendHandler.ResultGameEnd);
				pUtil.SetBool(true);

				Broadcast(pUtil);
			}
		}

		public void CalcGameEnd(GameClient client)
		{
			var players = GetPlayers(client);
			if (players != null)
			{
				var player = players.Item1;

				int cardVal = 0;
				int aceCnt = 0;

				foreach (GameCard card in player.CardList)
				{
					if (card.number >= 10)
					{
						cardVal += 10;
					}
					else if (card.number == 1)
					{
						//TODO:: 1 또는 11 중 유리한 숫자로 계산
						aceCnt++;
					}
					else
					{
						cardVal += card.number;
					}
				}

				Logger.Log(LoggerFlag.Info, string.Format("1 플레이어 일반 카드 합 : {0}", cardVal));

				var player_2 = players.Item2;

				int cardVal_2 = 0;
				int aceCnt_2 = 0;

				foreach (GameCard card in player_2.CardList)
				{
					if (card.number >= 10)
					{
						cardVal_2 += 10;
					}
					else if (card.number == 1)
					{
						//TODO:: 1 또는 11 중 유리한 숫자로 계산
						aceCnt_2++;
					}
					else
					{
						cardVal_2 += card.number;
					}
				}

				Logger.Log(LoggerFlag.Info, string.Format("2 플레이어 일반 카드 합 : {0}", cardVal_2));

				if (cardVal >= 22)
				{
					//블렌드 : 2번 플레이어의 승리
					RequestGameEnd(player_2);
				}
				else if (cardVal_2 >= 22)
				{
					//블렌드 : 1번 플레이어의 승리
					RequestGameEnd(player);
				}
				else
				{
					int remain = 22 - cardVal;

					for (int i = 0; i < aceCnt; i++)
					{
						if (remain > 10)
						{
							remain -= 10;
						}
						else if (remain > 2)
						{
							remain--;
						}
						else
						{
							//블렌드 : 2번 플레이어의 승리
							RequestGameEnd(player_2);
							return;
						}
					}

					Logger.Log(LoggerFlag.Info, string.Format("1 플레이어 남은 카드 값 : {0}", remain));

					int remain_2 = 22 - cardVal_2;

					for (int i = 0; i < aceCnt_2; i++)
					{
						if (remain_2 > 10)
						{
							remain_2 -= 10;
						}
						else if (remain_2 > 2)
						{
							remain_2--;
						}
						else
						{
							//블렌드 : 1번 플레이어의 승리
							RequestGameEnd(player);
							return;
						}
					}

					Logger.Log(LoggerFlag.Info, string.Format("2 플레이어 남은 카드 값 : {0}", remain_2));

					if (remain > remain_2)
					{
						//2번 플레이어의 승리
						RequestGameEnd(player_2);
					}
					else if (remain < remain_2)
					{
						//1번 플레이어의 승리
						RequestGameEnd(player);
					}
					else
					{
						//무승부
						RequestGameEnd(null);
					}
				}
			}
		}

		public void RequestRetry(GameClient client)
		{
			var players = GetPlayers(client);
			if (players != null)
			{
				var player = players.Item2;

				var pUtil = new PacketUtil(SendHandler.ResultRetry);
				player.Send(pUtil);
			}
		}

		public void RequestAskRetry(GameClient client, PacketUtil pUtil)
		{
			bool bRetry = pUtil.GetBool();

			var players = GetPlayers(client);
			if (players != null)
			{
				var player = players.Item2;

				pUtil = new PacketUtil(SendHandler.ResultAskRetry);
				pUtil.SetBool(bRetry);

				player.Send(pUtil);
			}
		}
	}
}
