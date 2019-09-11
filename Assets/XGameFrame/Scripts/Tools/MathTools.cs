using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MathTools
{
    /// <summary>
    /// 保留2位小数点
    /// </summary>
    /// <returns></returns>
    public static float ConvertTwoDecimal<T>(T t) where T :struct
    {
       return (float)Math.Round(Convert.ToDecimal(t), 2);
    }


    private static float TempCovertNum;
    /// <summary>
    /// 转换成带单位的字符串
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="t"></param>
    /// <returns></returns>
    public static string ConvertToUnitl<T>(T t,bool IsMoneySymbo=true) where T : struct
    {
        TempCovertNum = ConvertTwoDecimal(t);
        if (TempCovertNum < 1000)
        {
            if (IsMoneySymbo)
            {
                return string.Format("{0}$", TempCovertNum);
            }
            return string.Format("{0}", TempCovertNum);
        }
        else if (TempCovertNum >= 1000 && TempCovertNum < 1000000)
        {
            return string.Format("{0}K", ConvertTwoDecimal(TempCovertNum / 1000.0f));
        }
        else if (TempCovertNum >= 1000000 && TempCovertNum < 1000000000)
        {
            return string.Format("{0}M", ConvertTwoDecimal(TempCovertNum / 1000000.0f) );
        }
        else if (TempCovertNum >= 1000000000 && TempCovertNum <= 2140000000)
        {
            return string.Format("{0}B", ConvertTwoDecimal (TempCovertNum / 1000000000.0f));
        }
        else
        {
            return string.Format("{0}B", ConvertTwoDecimal(TempCovertNum / 1000000000.0f));
        }
    }
    /// <summary>
    /// 将数字转换成带标点符号的字符串
    /// </summary>
    /// <returns></returns>
    public static string CovertNumToSymbol(int Number)
    {
        if (Number < 10000)
        {
            return Number.ToString();
        }
        return string.Format("{0}K", ConvertTwoDecimal(Number / 1000f));
    }


    private static string FinalHour;
    private static string FinalMinute;
    private static string FinalSecon;
    private static TimeSpan timeSpan=new TimeSpan();
    /// <summary>
    /// 将一个数字转换成01：23：33这种形式
    /// </summary>
    /// <returns>/是否有小时</returns>
    public static string CoverNumberToTimer(float Number,bool IsHaveHour)
    {
        timeSpan=TimeSpan.FromSeconds(Number);
        if (IsHaveHour)
        {
            if (timeSpan.Hours < 10)
            {
                FinalHour = string.Format("0{0}", timeSpan.Hours);
            }
            else
            {
                FinalHour = timeSpan.Hours.ToString();
            }
        }

        if (timeSpan.Minutes < 10)
        {
            FinalMinute = string.Format("0{0}", timeSpan.Minutes);
        }
        else
        {
            FinalMinute = timeSpan.Minutes.ToString();
        }
        if (timeSpan.Seconds < 10)
        {
            FinalSecon = string.Format("0{0}", timeSpan.Seconds);
        }
        else
        {
            FinalSecon = timeSpan.Seconds.ToString();
        }
        if (IsHaveHour)
        {
            return string.Format("{0}:{1}:{2}", FinalHour, FinalMinute, FinalSecon);
        }
       return string.Format("{0}:{1}", FinalMinute, FinalSecon);
    }

    //随机打算int数组
    public static int[] GetRandomNum(int[] num)
    {
        for (int i = 0; i < num.Length; i++)
        {
            int temp = num[i];
            int randomIndex = Random.Range(0, num.Length);
            num[i] = num[randomIndex];
            num[randomIndex] = temp;
        }
        return num;
    }
    //打乱List数组
    public static List<T> RandomSortList<T>(List<T> ListT)
    {
        System.Random random = new System.Random();
        List<T> newList = new List<T>();
        foreach (T item in ListT)
        {
            newList.Insert(random.Next(newList.Count), item);
        }
        return newList;
    }

    public static bool IsEqualsZero(float Number)
    {
        return Number < 0.000001f && Number>-0.000001f;
    }

}
