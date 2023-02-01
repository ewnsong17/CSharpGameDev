using GameServer.Network;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Util
{
	class Start
	{
		public static void Main(string[] args)
		{
			try
			{
				ServerSocket.GetInstance();

				Console.ReadLine();
			}
			catch (Exception e)
			{
				Logger.Log(LoggerFlag.Debug, e.Message);
			}
		}
	}
}
