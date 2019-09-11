using System;
using System.Collections.Generic;
using System.IO;
using XGameTools;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// 玩家需要保存的数据
/// </summary>
[Serializable]
public class PlayerData
{


}



/// <summary>
/// 玩家管理类
/// </summary>
public class PlayerManager
{
    public static PlayerData _PlayerData;

    //玩家数据路径
    private const string PlayerConfigFileName = @"/PlayerData.dat";

#if UNITY_EDITOR
    public static string PlayeyAssetConfigPath = Application.dataPath + PlayerConfigFileName;
#else
    public static string PlayeyAssetConfigPath = Application.persistentDataPath + PlayerConfigFileName;
#endif

    public static void InitPlayer()
    {
        LoadPlayerConfig();
    }

    //读取玩家存储的数据，如果没有数据则初始化一个数据。
	private static void LoadPlayerConfig()
    {
        if (File.Exists(PlayeyAssetConfigPath))
        {
            _PlayerData = SerializHelp.DeserializeFileToObj<PlayerData>(PlayeyAssetConfigPath);
            //todo 版本号不一致时是否需要做特殊处理
            //if (_PlayerData.Version != Application.version)
            //{
            //}
        }
        else
        {
            _PlayerData = CreateNewPlayer();

            SavePlayer();
        }
    }

    /// <summary>
    /// 初始化玩家数据
    /// </summary>
    /// <returns></returns>
    private static PlayerData CreateNewPlayer()
    {
        PlayerData playerData = new PlayerData
        {

        };
        return playerData;
    }
    
    public static void SavePlayer()
    {
        SerializHelp.SerializeFile(_PlayerData, PlayeyAssetConfigPath);
    }


}


/// <summary>
/// 解决Vector3无法序列化的问题
/// </summary>
[Serializable]
public class MyVector3
{
    public float x;
    public float y;
    public float z;

    public MyVector3()
    {
        x = 0f;
        y = 0f;
        z = 0f;
    }

    public MyVector3(float _x,float _y,float _z)
    {
        x = _x;
        y = _y;
        z = _z;
    }
}
