using System.Collections;
using System.Collections.Generic;
using System.IO;
using XGameTools;
using UnityEditor;
using UnityEngine;

public class PlayerDataEditor : Editor
{
    [MenuItem("XGameTools/删档", false, -32)]
    public static void EditorPlayerData_DeleData()
    {
        if (File.Exists(PlayerManager.PlayeyAssetConfigPath))
        {
            File.Delete(PlayerManager.PlayeyAssetConfigPath);
            AssetDatabase.Refresh();
        }
    }

    [MenuItem("XGameTools/加钱", false, -32)]
    public static void EditorPlayerData_AddGold()
    {
        if (!File.Exists(PlayerManager.PlayeyAssetConfigPath))
        {
            PlayerManager.InitPlayer();
        }

        PlayerManager._PlayerData = SerializHelp.DeserializeFileToObj<PlayerData>(PlayerManager.PlayeyAssetConfigPath);
       // PlayerManager._PlayerData.Gold = 999999999;
        PlayerManager.SavePlayer();
    }
}
