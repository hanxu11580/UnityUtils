using System;

namespace USDT.Utils
{
	public static class EnumUtils
	{
		public static T FromString<T>(string str)
		{
            if (!Enum.IsDefined(typeof(T), str))
            {
                return default(T);
            }
            return (T)Enum.Parse(typeof(T), str);
        }
    }
}