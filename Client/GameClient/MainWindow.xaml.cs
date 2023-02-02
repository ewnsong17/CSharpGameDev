using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using GameClient.network;
using GameClient.Network.Client;
using System.Threading;

namespace GameClient
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public network.GameClient Instance;

		public List<Image> CardImageList = new List<Image>();
		public List<Image> CardImageOppositeList = new List<Image>();

		public MainWindow()
		{
			InitializeComponent();
		}

		private void GameStartClick(object sender, RoutedEventArgs e)
		{
			Instance = ClientSocket.GetInstance(this);
		}

		private void GameEndClick(object sender, RoutedEventArgs e)
		{
			if (Instance != null)
			{
				Instance.GameClosed();
			}
			else
			{
				Application.Current.Shutdown();
			}
		}

		private void GameCloseClick(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (Instance != null)
			{
				Instance.GameClosed();
			}
			else
			{
				Application.Current.Shutdown();
			}
		}

		public void ChangeScene()
		{
			Dispatcher.Invoke(
				DispatcherPriority.Normal,
				new Action(
					delegate
					{
						GameStart.Visibility = Visibility.Collapsed;
						GameEnd.Visibility = Visibility.Collapsed;

						if (MyGrid.Background is ImageBrush myBrush)
						{
							myBrush.ImageSource = new BitmapImage(new Uri("pack://application:,,,/GameClient;component/image/game_card_back.jpg"));
						}

						GameWait.Visibility = Visibility.Visible;

						Instance.RequestPlayerExist();
					}
				)
			);
		}

		public void RemoveWaitLabel()
		{
			Dispatcher.Invoke(
				DispatcherPriority.Normal,
				new Action(
					delegate
					{
						GameWait.Visibility = Visibility.Collapsed;
					}
				)
			);
		}

		public void GameInit()
		{

			Dispatcher.Invoke(
				DispatcherPriority.Normal,
				new Action(
					delegate
					{

						foreach (Image image in CardImageList)
						{
							MyGrid.Children.Remove(image);
						}

						CardImageList.Clear();

						foreach (Image image in CardImageOppositeList)
						{
							MyGrid.Children.Remove(image);
						}

						CardImageOppositeList.Clear();
						Instance.bStand = false;

						GameNotice.Visibility = Visibility.Visible;
						GameNotice.Content = "원하시는 행동을 선택해주세요.";

						GameHit.Visibility = Visibility.Visible;
						GameStand.Visibility = Visibility.Visible;

						GameRetry.Visibility = Visibility.Collapsed;

						var cardList = Instance.CardList;

						//중심 : 460, 460

						int x = 50;
						foreach (GameCard card in cardList)
						{
							//내꺼 그리기
							Image cardImage = new Image
							{

								Source = new BitmapImage(new Uri(card.GetURL())),
								Margin = new Thickness(460 - x, 600, 460 + x, 22),
								Stretch = Stretch.Fill,

								Name = card.GetMark() + "" + card.number
							};

							MyGrid.Children.Add(cardImage);
							CardImageList.Add(cardImage);

							//상대꺼 그리기
							Image cardImage_o = new Image
							{

								Source = new BitmapImage(new Uri("pack://application:,,,/GameClient;component/image/Card/back.png")),
								Margin = new Thickness(460 - x, 22, 460 + x, 600),
								Stretch = Stretch.Fill
							};

							MyGrid.Children.Add(cardImage_o);
							CardImageOppositeList.Add(cardImage_o);

							x -= 50;
						}
					}
				)
			);
		}

		public void GameHitClick(object sender, RoutedEventArgs e)
		{
			Instance.RequestHit();
		}

		public void ReDrawCard(GameCard card)
		{
			Dispatcher.Invoke(
				DispatcherPriority.Normal,
				new Action(
					delegate
					{
						var cardList = Instance.CardList;

						//중심 : 460, 460

						int x = 100 - (50 * cardList.Count);

						Image cardImage = new Image
						{

							Source = new BitmapImage(new Uri(card.GetURL())),
							Margin = new Thickness(460 - x, 600, 460 + x, 22),
							Stretch = Stretch.Fill,

							Name = card.GetMark() + "" + card.number
						};

						GameNotice.Content = string.Format("새 카드를 뽑았습니다 : {0}", cardImage.Name);

						MyGrid.Children.Add(cardImage);
						CardImageList.Add(cardImage);
					}
				)
			);
		}

		public void ReDrawOppositeCard(int cardCount)
		{
			Dispatcher.Invoke(
				DispatcherPriority.Normal,
				new Action(
					delegate
					{
						foreach (Image image in CardImageOppositeList)
						{
							MyGrid.Children.Remove(image);
						}

						CardImageOppositeList.Clear();

						GameNotice.Content = "적이 새 카드를 뽑았습니다.";

						int x = 50;

						for (int i = 0; i < cardCount; i++)
						{
							Image cardImage_o = new Image
							{

								Source = new BitmapImage(new Uri("pack://application:,,,/GameClient;component/image/Card/back.png")),
								Margin = new Thickness(460 - x, 22, 460 + x, 600),
								Stretch = Stretch.Fill
							};

							MyGrid.Children.Add(cardImage_o);
							CardImageOppositeList.Add(cardImage_o);

							x -= 50;
						}
					}
				)
			);
		}

		public void GameStandClick(object sender, RoutedEventArgs e)
		{
			Instance.RequestStand();
		}

		public void ContentStand()
		{
			Dispatcher.Invoke(
				DispatcherPriority.Normal,
				new Action(
					delegate
					{
						GameNotice.Content = string.Format("상대방이 마칠 때까지 기다리는 중입니다..");
					}
				)
			);
		}

		public void ContentOppositeStand()
		{
			Dispatcher.Invoke(
				DispatcherPriority.Normal,
				new Action(
					delegate
					{
						GameNotice.Content = string.Format("상대방이 스탠드를 신청했습니다.");
					}
				)
			);
		}

		public void ContentGameEnd(bool bDraw, bool bWin)
		{
			Dispatcher.Invoke(
				DispatcherPriority.Normal,
				new Action(
					delegate
					{
						if (bDraw)
						{
							GameNotice.Content = "무승부입니다.";
						}
						else if (bWin)
						{
							GameNotice.Content = "승리했습니다!";
						}
						else
						{
							GameNotice.Content = "패배했습니다.";

							GameRetry.Visibility = Visibility.Visible;
						}
					}
				)
			);
		}

		public void GameRetryClick(object sender, RoutedEventArgs e)
		{
			GameRetry.Visibility = Visibility.Collapsed;

			Instance.RequestRetryGame();
		}

		public void ContentRetry(bool bRetry)
		{
			if (bRetry)
			{
				//TODO::게임 다시 세팅
				Instance.RequestPlayerExist();
			}
			else
			{
				Dispatcher.Invoke(
					DispatcherPriority.Normal,
					new Action(
						delegate
						{

							GameNotice.Content = "상대방이 다시하기를 거절했습니다.";
							GameRetry.Visibility = Visibility.Visible;
						}
					)
				);
			}
		}
	}
}
