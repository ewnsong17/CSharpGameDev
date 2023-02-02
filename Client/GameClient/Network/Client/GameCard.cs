using System;
using System.Collections.Generic;
using System.Text;

namespace GameClient.Network.Client
{
	public class GameCard
	{
		public int color;
		public int number;

		public string GetMark()
		{
			switch (color)
			{
				case 0:
					return "Clover";
				case 1:
					return "Diamond";
				case 2:
					return "Heart";
				case 3:
					return "Spade";
			}
			return null;
		}

		public string GetURL()
		{
			StringBuilder URL = new StringBuilder("pack://application:,,,/GameClient;component/image/Card");

			URL.Append("/" + GetMark());
			
			switch (number)
			{
				case 1:
					URL.Append("/A.jpg");
					break;
				case 11:
					URL.Append("/J.jpg");
					break;
				case 12:
					URL.Append("/Q.jpg");
					break;
				case 13:
					URL.Append("/K.jpg");
					break;
				default:
					URL.Append("/" + number + ".jpg");
					break;
			}

			return URL.ToString();
		}
	}
}
