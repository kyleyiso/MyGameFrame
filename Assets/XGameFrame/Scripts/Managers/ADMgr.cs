using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class ADMgr : MonoBehaviour
{

    [HideInInspector]
    public AndroidJavaObject jo;
    public static ADMgr Instance;
    private Action _successAction;
#if UNITY_IPHONE
    [DllImport("__Internal")]
    private static extern string GetPlayerUseID();
    [DllImport("__Internal")]
    private static extern string GetPlayerCountry();
    [DllImport("__Internal")]
    private static extern string GetPlayerADVersion();

    [DllImport("__Internal")]
    private static extern void ShowInterAD();
    [DllImport("__Internal")]
    private static extern void ShowRewardAD();
    [DllImport("__Internal")]
    private static extern bool IsRewardADReady();
    [DllImport("__Internal")]
    private static extern bool IsInterADReady();
    [DllImport("__Internal")]
    private static extern void SendGameEven_IOS(string evenName, string jsonvalue);
    [DllImport("__Internal")]
    private static extern void SendGamePropret_IOS(string proName, int addValue);
   
#endif


    void Awake()
    {
        Instance = this;
#if UNITY_ANDROID
        try
        {
            AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
#endif
    }

    private void U_SendGameEven_IOS(string evenName, string jsonvalue)
    {

#if UNITY_IPHONE
       SendGameEven_IOS(evenName, jsonvalue);
#endif

    }

    //调用安卓邮件
    public void SendEmail()
    {
#if UNITY_ANDROID
        try
        {
            jo.Call("SendEmail", Application.identifier, Application.version);
        }
        catch (Exception)
        {

        }
#elif UNITY_IPHONE
        string subject = "Player Feedback";
        string toMail = "monniemaxwell@gmail.com";
        string ID=GetPlayerUseID();
        string AppVersion = Application.version;
        string PackageName = Application.identifier;
        string DeviceModel = SystemInfo.deviceModel;
        string DeviceVersion = SystemInfo.operatingSystem;
        string Region = GetPlayerCountry();

        string body= "My suggestion/feedback is " + '\n' + '\n' + '\n' + "Please don't delete the information below." + '\n' +
                     "######################################" + '\n' +
                     "UserId:" + ID + '\n' +
                     "Version:" + AppVersion + '\n' +
                     "PackageName:" + PackageName + '\n' +
                     "ADVersion:" + GetPlayerADVersion() + '\n' +
                     "DeviceModel:" + DeviceModel + '\n' +
                     "OSVersion:" + DeviceVersion + '\n' +
                     "Region:" + Region + '\n' +
                     "######################################" + '\n' +
                     "Please don't delete the information above.";

        Uri uri = new Uri(string.Format("mailto:{0}?subject={1}&cc={2}&bcc={3}&body={4}", toMail, subject,"","", body));
        Application.OpenURL(uri.AbsoluteUri);
#endif
    }

    #region 打点
    public void SendGameEven_New_Device()
    {
        string str = string.Format("{{\"unity_device\":{0}}}", 1);
#if UNITY_ANDROID
        try
        {
            jo.Call("MySendGameEven", "unity_new_device", str);
        }
        catch (Exception)
        {
           
        }
#elif UNITY_IPHONE
        U_SendGameEven_IOS("unity_new_device", str);
#endif

    }


//    public void SendGameEven_Offline_Collect(int coin, bool rv_ready, int multiple)
//    {
//        string str = string.Format("{{\"coin\":{0},\"rv_ready\":{1},\"multiple\":{2},\"coin_spend\":{3},\"level_game\":{4},\"level_restaurant\":{5},\"palyday\":{6},\"playtime\":{7}}}",
//            coin, rv_ready.ToString().ToLower(), multiple, GameManager._DataMgr._AddPlayerData.CoinSpend,
//            GameManager._DataMgr._PlayerData.PlayerLevel, GameManager._DataMgr._PlayerData.HouseLevel,
//            GameManager._DataMgr._AddPlayerData.PlayDay, GameManager._DataMgr._AddPlayerData.PlayTime);
//#if UNITY_ANDROID
//        try
//        {
//            jo.Call("MySendGameEven", "offline_collect", str);
//        }
//        catch (Exception e)
//        {
//            DebugMgr.LogMessage("发送打点事件出错：" + e.ToString());
//        }
//#elif UNITY_IPHONE
//         U_SendGameEven_IOS("offline_collect",str);
//#endif
//    }


    public void SendAddedProperty(string Key, float Number)
    {
#if UNITY_ANDROID
        try
        {
            jo.Call("MySetAddedProperty", Key, Number);
        }
        catch (Exception)
        {
           // Debug.Log("SendAddedProperty_Faile--" + e);
        }
#else
       SendGamePropret_IOS(Key, (int)Number);
#endif

    }


    //public void SendUserProperty(string Key, String value)
    //{
    //    try
    //    {
    //        jo.Call("MySetUserProperty", Key, value);
    //    }
    //    catch (Exception e)
    //    {
    //       Debug.Log("SendUserProperty_Faile--"+e);
    //    }

    //}

    //public void SendOnceProperty(string Key, String value)
    //{
    //    try
    //    {
    //        jo.Call("MySetOnceProperty", Key, value);
    //    }
    //    catch (Exception e)
    //    {
    //        Debug.Log("SendOnceProperty_Faile--"+e);
    //    }

    //}

#endregion

    /// <summary>
    /// 插屏是否加载完毕
    /// </summary>
    public bool IsInterReadly()
    {
        bool IsReadly = false;
#if UNITY_ANDROID
        try
        {
            IsReadly = jo.Call<bool>("IsInterstitialADSuc");
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
#elif UNITY_IPHONE
        IsReadly = IsInterADReady();
#endif

        return IsReadly;
    }

    /// <summary>
    /// 激励视频是否加载完毕
    /// </summary>
    public bool IsReawardReadly()
    {
        bool IsReadly = false;
#if UNITY_ANDROID
        try
        {
            IsReadly = jo.Call<bool>("IsRewardVideoSuc");
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
#elif UNITY_IPHONE
        IsReadly = IsRewardADReady();
#endif

        return IsReadly;
    }
    /// <summary>
    /// 播放插屏
    /// </summary>
    public void ShowInterstitialAD()
    {
#if UNITY_ANDROID
        try
        {
            jo.Call("ShowInterstitialAD");
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
#elif UNITY_IPHONE
      ShowInterAD();
#endif
    }

    /// <summary>
    /// 播放激励视频
    /// </summary>
    public void ShowRewardAD(Action SuccessAction = null)
    {
        _successAction = SuccessAction;
#if UNITY_ANDROID
        try
        {
            jo.Call("ShowRewardVideo");
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
#elif UNITY_IPHONE
        ShowRewardAD();
#endif


    }

    /// <summary>
    /// 激励视频播放完成后的回调
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public void RewardCallbackSuccess(string value)
    {
        //处理逻辑
        if (_successAction != null)
        {
            _successAction();
        }
        _successAction = null;
    }

}
