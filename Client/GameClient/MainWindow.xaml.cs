using System;
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
			GameStart.Visibility = Visibility.Collapsed;
			Instance = ClientSocket.GetInstance(this);
		}

		private void GameEndClick(object sender, RoutedEventArgs e)
		{
			Application.Current.Shutdown();
		}
	}
}
