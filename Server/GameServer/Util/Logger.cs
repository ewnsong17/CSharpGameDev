using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace GameServer.Util
{
	enum LoggerFlag
	{
		Error, Warning, Info, Debug
	}

	class Logger
	{
		public static string GetName(LoggerFlag flag)
		{
			switch (flag)
			{
				case LoggerFlag.Error:
					return "오류";
				case LoggerFlag.Warning:
					return "경고";
				case LoggerFlag.Info:
					return "정보";
				case LoggerFlag.Debug:
					return "디버그";
			}
			return "";
		}
		public static void Log(LoggerFlag flag, string str)
		{
			if (flag == LoggerFlag.Debug && !GameUtil.IsDebugging)
			{
				return;
			}

			var sb = new StringBuilder(DateTime.UtcNow.ToString());
			var st = new StackTrace(true);

			if (st.FrameCount > 0)
			{
				sb.Append(" [");
				sb.Append(st.GetFrame(1).GetMethod().Name);
				sb.Append("] ");
				sb.Append(": ");
				sb.Append(str);

				Console.WriteLine(sb.ToString());
			}
		}
	}
}
