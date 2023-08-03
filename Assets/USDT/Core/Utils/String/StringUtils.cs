using System;
using System.Linq;
using System.Text;

namespace USDT.Utils {
    public static class StringUtils
    {
        #region 字符串首字符大小写

        /// <summary>
        /// 首字母小写
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string FirstCharToLower(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;
            string str = input.First().ToString().ToLower() + input.Substring(1);
            return str;
        }

        /// <summary>
        /// 首字母大写
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string FirstCharToUpper(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;
            string str = input.First().ToString().ToUpper() + input.Substring(1);
            return str;
        }
        
        #endregion
        
        #region 字符串直接采用StringBuilder拼接
        
        /// <summary>
        /// 缓存字符串, ThreadStatic会使不同线程都会new一个新的_cachedStringBuilder
        /// 如果不这样设置，新线程会使用同一个
        /// 下面取自GameFramework 并添加了Concat拼接及注释
        /// </summary>
        [ThreadStatic]
        private static StringBuilder _cachedStringBuilder = null;
        
        public static StringBuilder GetStaticBuilder()
        {
            if (_cachedStringBuilder == null)
            {
                //最长1024
                _cachedStringBuilder = new StringBuilder(1024);
            }

            _cachedStringBuilder.Clear();
            return _cachedStringBuilder;
        }

        #region 格式化字符串
        
        /// <summary>
        /// 获取格式化字符串。
        /// </summary>
        /// <param name="format">字符串格式。</param>
        /// <param name="arg0">字符串参数 0。</param>
        /// <returns>格式化后的字符串。</returns>
        public static string Format(string format, object arg0)
        {
            if (format == null)
            {
                return string.Empty;
            }

            GetStaticBuilder();
            // _cachedStringBuilder.Length = 0;
            _cachedStringBuilder.AppendFormat(format, arg0);
            return _cachedStringBuilder.ToString();
        }

        /// <summary>
        /// 获取格式化字符串。
        /// </summary>
        /// <param name="format">字符串格式。</param>
        /// <param name="arg0">字符串参数 0。</param>
        /// <param name="arg1">字符串参数 1。</param>
        /// <returns>格式化后的字符串。</returns>
        public static string Format(string format, object arg0, object arg1)
        {
            if (format == null)
            {
                return string.Empty;
            }

            GetStaticBuilder();
            // _cachedStringBuilder.Length = 0;
            _cachedStringBuilder.AppendFormat(format, arg0, arg1);
            return _cachedStringBuilder.ToString();
        }

        /// <summary>
        /// 获取格式化字符串。
        /// </summary>
        /// <param name="format">字符串格式。</param>
        /// <param name="arg0">字符串参数 0。</param>
        /// <param name="arg1">字符串参数 1。</param>
        /// <param name="arg2">字符串参数 2。</param>
        /// <returns>格式化后的字符串。</returns>
        public static string Format(string format, object arg0, object arg1, object arg2)
        {
            if (format == null)
            {
                return string.Empty;
            }

            GetStaticBuilder();
            // _cachedStringBuilder.Length = 0;
            _cachedStringBuilder.AppendFormat(format, arg0, arg1, arg2);
            return _cachedStringBuilder.ToString();
        }

        /// <summary>
        /// 获取格式化字符串。
        /// </summary>
        /// <param name="format">字符串格式。</param>
        /// <param name="args">字符串参数。</param>
        /// <returns>格式化后的字符串。</returns>
        public static string Format(string format, params object[] args)
        {
            if (format == null)
            {
                return string.Empty;
            }

            if (args == null)
            {
                return string.Empty;
            }

            GetStaticBuilder();
            // _cachedStringBuilder.Length = 0;
            _cachedStringBuilder.AppendFormat(format, args);
            return _cachedStringBuilder.ToString();
        }
        
        #endregion

        #region 拼接字符串

            public static string Concat(string s1, string s2)
            {
                GetStaticBuilder();
                _cachedStringBuilder.Append(s1);
                _cachedStringBuilder.Append(s2);
                return _cachedStringBuilder.ToString();
            }
            public static string Concat(string s1, string s2, string s3)
            {
                GetStaticBuilder();
                _cachedStringBuilder.Append(s1);
                _cachedStringBuilder.Append(s2);
                _cachedStringBuilder.Append(s3);
                return _cachedStringBuilder.ToString();
            }

        #endregion

        #endregion

        #region 将字节大小自动转换成 B，GB，MB，KB 等单位输出
        private static readonly double[] byteUnits =
{
            1073741824.0, 1048576.0, 1024.0, 1
        };

        private static readonly string[] byteUnitsNames =
        {
            "GB", "MB", "KB", "B"
        };
        /// <summary>
        ///     将字节大小自动转换成 B，GB，MB，KB 等单位输出
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string FormatBytes(ulong bytes) {
            var size = "0 B";
            if (bytes == 0) {
                return size;
            }

            for (var index = 0; index < byteUnits.Length; index++) {
                var unit = byteUnits[index];
                if (bytes >= unit) {
                    size = $"{bytes / unit:##.##} {byteUnitsNames[index]}";
                    break;
                }
            }

            return size;
        }
        #endregion
    }
}
