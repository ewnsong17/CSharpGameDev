using System;
using System.Collections.Generic;
using System.Text;

namespace GameClient.Network.Client
{
	public class GameCard
	{
		public int color;
		public int number;

		public string GetURL()
		{
			StringBuilder URL = new StringBuilder("pack://application:,,,/GameClient;component/image/Card");

			switch (color)
			{
				case 0:
					URL.Append("/Clover");
					break;
				case 1:
					URL.Append("/Diamond");
					break;
				case 2:
					URL.Append("/Heart");
					break;
				case 3:
					URL.Append("/Spade");
					break;
			}
			
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
