using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Util
{
	class GameUtil
	{
		public const bool IsDebugging = true;

		public static readonly DateTime Jan1st1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

		//하트비트 매직 넘버
		public const int MAGIC_NUM = 0x1107;

		/// <summary>
		/// Java에 있는 그것
		/// </summary>
		/// <returns></returns>
		public static long CurrentTimeMillis()
		{
			return (long)(DateTime.UtcNow - Jan1st1970).TotalMilliseconds;
		}
	}
}
