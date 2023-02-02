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
						GameNotice.Visibility = Visibility.Visible;
						GameNotice.Content = "원하시는 행동을 선택해주세요.";

						GameHit.Visibility = Visibility.Visible;
						GameStand.Visibility = Visibility.Visible;

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

								Name = card.GetMark() + "_" + card.number
							};

							MyGrid.Children.Add(cardImage);

							//상대꺼 그리기
							Image cardImage_o = new Image
							{

								Source = new BitmapImage(new Uri("pack://application:,,,/GameClient;component/image/Card/back.png")),
								Margin = new Thickness(460 - x, 22, 460 + x, 600),
								Stretch = Stretch.Fill
							};

							MyGrid.Children.Add(cardImage_o);

							x = -50;
						}
					}
				)
			);
		}

		public void GameHitClick(object sender, RoutedEventArgs e)
		{
			Instance.RequestHit();
		}

		public void GameStandClick(object sender, RoutedEventArgs e)
		{
			Trace.WriteLine("STAND!!");
		}
	}
}
