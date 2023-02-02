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
							Trace.WriteLine("OLD URI : " + myBrush.ImageSource.ToString());
							myBrush.ImageSource = new BitmapImage(new Uri("pack://application:,,,/GameClient;component/image/game_card_back.jpg"));
						}

						GameWait.Visibility = Visibility.Visible;

						Instance.RequestPlayerExist();
					}
				)
			);
		}

		public void InitGameSet()
		{
			Dispatcher.Invoke(
				DispatcherPriority.Normal,
				new Action(
					delegate
					{
						GameWait.Visibility = Visibility.Collapsed;

						//TODO::게임 시작 서버쪽에 세팅 요청
					}
				)
			);
		}
	}
}
