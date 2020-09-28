using System;
using System.Net;

namespace SelfishNet10
{
	public class tools
	{
		public tools()
		{
		}

		public static bool areValuesEqual(byte[] obj1, byte[] obj2)
		{
			if (obj1 == null || obj2 == null)
			{
				return false;
			}
			int length = obj1.Length;
			if (length != obj2.Length)
			{
				return false;
			}
			int num = 0;
			int num1 = length;
			if (0 < num1)
			{
				while (obj1[num] == obj2[num])
				{
					num++;
					if (num < num1)
					{
						continue;
					}
					return true;
				}
				return false;
			}
			return true;
		}

		public static IPAddress getIpAddress(string ip)
		{
			string[] strArrays = new string[4];
			byte[] num = new byte[4];
			char[] chrArray = new char[] { '.', '\u0003' };
			string[] strArrays1 = ip.Split(chrArray);
			int num1 = 0;
			do
			{
				num[num1] = Convert.ToByte(strArrays1[num1]);
				num1++;
			}
			while (num1 < 4);
			return new IPAddress(num);
		}
	}
}