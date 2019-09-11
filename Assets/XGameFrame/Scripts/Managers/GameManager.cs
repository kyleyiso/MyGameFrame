using System;
using System.Collections;
using UnityEngine;
using XGameTools;
using Random = UnityEngine.Random;

/// <summary>
/// 游戏进程控制器，负责初始化数据、开始游戏、暂停游戏、结束游戏
/// </summary>
public class GameManager : MonoBehaviour
{
    private void Awake()
    {
        InitEnvironment();

        StartGame();
    }

    /// <summary>
    /// 初始化平台环境
    /// </summary>
    private void InitEnvironment()
    {
        //在IOS上文件序列化成二进制，涉及到对List序列化会出问题,因此调用下面这句话
#if UNITY_IPHONE
        System.Environment.SetEnvironmentVariable("MONO_REFLECTION_SERIALIZER", "yes");
#endif
        //设置目标帧数
        Application.targetFrameRate = 60;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

    private void StartGame()
    {
        //读取玩家配置并且初始化玩家
        PlayerManager.InitPlayer();
        //加载所有图片
        AssetsLoader.LoadAllSprite();

        //初始化UI
        UIPanelManager.Init();
        //开始游戏，显示UI
       // UIPanelManager.ShowUI(UiId.MainPanel);
       
    }


    private void OnApplicationFocus(bool focus)
    {
        if (focus)//获取焦点
        {
          
        }
        else//失去焦点
        {
           
        }
    }
    
    
}
