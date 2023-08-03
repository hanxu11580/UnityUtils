using System;
using System.Collections;
using System.Collections.Generic;

namespace USDT.Utils {

    public static  class TimeUtils
    {
        #region 计算时间差

        /// <summary>
        /// 获取间隔秒数
        /// </summary>
        /// <param name="startTimer"></param>
        /// <param name="endTimer"></param>
        /// <param name="onlySecond">仅计算秒数差忽略时分等</param>
        /// <returns></returns>
        public static int GetSubSeconds(DateTime startTimer, DateTime endTimer, bool onlySecond = false)
        {
            TimeSpan startSpan = new TimeSpan(startTimer.Ticks);

            TimeSpan nowSpan = new TimeSpan(endTimer.Ticks);

            TimeSpan subTimer = nowSpan.Subtract(startSpan).Duration();

            return onlySecond ? subTimer.Seconds : (int)subTimer.TotalSeconds;
        }

        /// <summary>
        /// 获取两个时间的相差多少分钟
        /// </summary>
        /// <param name="startTimer"></param>
        /// <param name="endTimer"></param>
        /// <param name="onlyMinute">仅计算分钟差</param>
        /// <returns></returns>
        public static int GetSubMinutes(DateTime startTimer, DateTime endTimer, bool onlyMinute = false)
        {
            TimeSpan startSpan = new TimeSpan(startTimer.Ticks);

            TimeSpan nowSpan = new TimeSpan(endTimer.Ticks);

            TimeSpan subTimer = nowSpan.Subtract(startSpan).Duration();

            return onlyMinute ? subTimer.Minutes : (int)subTimer.TotalMinutes;
        }


        /// <summary>
        /// 获取两个时间的相差多少小时
        /// </summary>
        /// <param name="startTimer"></param>
        /// <param name="endTimer"></param>
        /// <param name="onlyHour">仅计算小时差</param>
        /// <returns></returns>
        public static int GetSubHours(DateTime startTimer, DateTime endTimer, bool onlyHour = false)
        {
            TimeSpan startSpan = new TimeSpan(startTimer.Ticks);

            TimeSpan nowSpan = new TimeSpan(endTimer.Ticks);

            TimeSpan subTimer = nowSpan.Subtract(startSpan).Duration();

            return onlyHour ? subTimer.Hours : (int)subTimer.TotalHours;
        }

        /// <summary>
        /// 获取两个时间的相差多少天
        /// </summary>
        /// <param name="startTimer"></param>
        /// <param name="endTimer"></param>
        /// <param name="onlyDay">仅计算天数差</param>
        /// <returns></returns>
        public static int GetSubDays(DateTime startTimer, DateTime endTimer, bool onlyDay = false)
        {
            TimeSpan startSpan = new TimeSpan(startTimer.Ticks);

            TimeSpan nowSpan = new TimeSpan(endTimer.Ticks);

            TimeSpan subTimer = nowSpan.Subtract(startSpan).Duration();

            return onlyDay ? subTimer.Days : (int)subTimer.TotalDays;
        }

        #endregion

        /// <summary>
        /// 将时间格式化成标准年/月/日 时:分:秒格式
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static string ToDefaultDateString(DateTime time)
        {
            return time.ToString("yyyy/MM/dd HH:mm:ss");
        }

        public static string ToYYYY_MM_DD(DateTime time) {
            return time.ToString("yyyy-MM-dd");
        }

        public static string Toyyyy_MM_dd_HH_mm_ss(DateTime time) {
            return time.ToString("yy_MM_dd HH:mm:ss");
        }

        public static (Int32 h, Int32 m, Int32 s) GetHmsFromSeconds(float seconds)
        {
            Int32 h = (Int32)Math.Floor(seconds / 3600f);
            Int32 m = (Int32)Math.Floor(seconds / 60f - h * 60f);
            Int32 s = (Int32)Math.Floor(seconds - m * 60f - h * 3600f);
            return (h, m, s);
        }
    }
}