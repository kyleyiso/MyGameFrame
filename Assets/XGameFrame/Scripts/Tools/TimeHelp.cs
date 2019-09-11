using System;
using System.IO;
using System.Net;
using System.Text;
using UnityEngine;

namespace XGameTools
{
    /// <summary>
    /// 时间戳
    /// </summary>
    public class TimeHelp : MonoBehaviour
    {
        /// <summary>  
        /// 将c# DateTime时间格式转换为Unix时间戳格式  
        /// </summary>  
        /// <param name="time">时间</param>  
        /// <returns>long</returns>  
        public static long ConvertDateTimeToTimeStamp(DateTime time)
        {
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1, 0, 0, 0));
            long timeStamp = (time.Ticks - startTime.Ticks) / 10000; //除10000调整为13位
            return timeStamp;
        }

        /// <summary>        
        /// 时间戳转为C#格式时间        
        /// </summary>        
        /// <param name=”timeStamp”></param>        
        /// <returns></returns>        
        public static DateTime ConvertTimeStampToDateTime(long timeStamp)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = long.Parse(string.Format("{0}0000", timeStamp));
            TimeSpan toNow = new TimeSpan(lTime);
            return dtStart.Add(toNow);
        }

        /// <summary>
        /// 获取时间,如果玩家有网，检验时间戳
        /// </summary>
        /// <returns></returns>
        public static DateTime GetNowTime()
        {
            if (IsHaveNet())
            {
                DateTime dt;
                WebRequest wrt = null;
                WebResponse wrp = null;
                try
                {
                    wrt = WebRequest.Create("http://www.hko.gov.hk/cgi-bin/gts/time5a.pr?a=1"); //香港服务器
                    wrp = wrt.GetResponse();
                    string timeStampStr = string.Empty;

                    using (Stream stream = wrp.GetResponseStream())
                    {
                        using (StreamReader sr = new StreamReader(stream, Encoding.UTF8))
                        {
                            timeStampStr = sr.ReadToEnd();
                        }
                    }

                    if (!string.IsNullOrEmpty(timeStampStr))
                    {
                        string[] tempArray = timeStampStr.Split('=');
                        long timeStamp;
                        if (tempArray.Length == 2 && long.TryParse(tempArray[1], out timeStamp))
                        {
                            if (IsRealTime(timeStamp, 10)) //如果玩家本地时间和网络时间误差在2分钟内
                            {
#if UNITY_EDITOR
                               // Debug.Log("允许的时间误差");
#endif
                                return DateTime.Now;
                            }
                            else //否则视为玩家调整时间来作弊
                            {
#if UNITY_EDITOR
                                Debug.Log("更改系统时间");
#endif
                                dt = ConvertTimeStampToDateTime(timeStamp);
                                //todo 处理玩家更改系统时间，将玩家的最后登录时间设置为网络时间
                            }
                        }
                        else
                        {
                            return DateTime.Now;
                        }
                    }
                    else
                    {
                        return DateTime.Now;
                    }
                }

                catch (WebException)
                {
                    return DateTime.Now;

                }
                catch (Exception)
                {
                    return DateTime.Now;
                }
                finally
                {
                    if (wrp != null)
                    {
                        wrp.Close();
                    }

                    if (wrt != null)
                    {
                        wrt.Abort();
                    }
                }

                return dt;
            }

            return DateTime.Now;
        }

        /// <summary>
        /// 验证时间戳,如果玩家有网，需要验证一下玩家是否更改本地时间.
        /// </summary>
        /// <param name="time"></param>
        /// <param name="interval">差值（分钟）</param>
        /// <returns></returns>
        private static bool IsRealTime(long time, double interval)
        {
            //取现在时间
            DateTime dt = ConvertTimeStampToDateTime(time);
            DateTime dt1 = DateTime.Now.AddMinutes(interval);
            DateTime dt2 = DateTime.Now.AddMinutes(interval * -1);
            if (dt > dt2 && dt < dt1)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// 获取两个时间间隔的秒数
        /// </summary>
        /// <param name="dateTime1"></param>
        /// <param name="dateTime2"></param>
        /// <param name="MaxSecond">允许最大的间隔秒数</param>
        /// <returns></returns>
        public static int DiffSecondByTwoDateTime(DateTime dateTime1, DateTime dateTime2, int MaxSecond = int.MaxValue)
        {
            //间隔的时间
            TimeSpan ts = dateTime1.Subtract(dateTime2).Duration();
            //间隔的秒数
            int diffSceon = ts.Days * 86400 + ts.Hours * 3600 + ts.Minutes * 60 + ts.Seconds;
            return diffSceon <= MaxSecond ? diffSceon : MaxSecond;
        }

        public static bool IsHaveNet()
        {
            return Application.internetReachability != NetworkReachability.NotReachable;
        }

        public static double CoverTimerToSecond(DateTime dateTime)
        {
            TimeSpan ts=new TimeSpan(dateTime.Ticks);
            return ts.TotalSeconds;
        }
    }
    
}
