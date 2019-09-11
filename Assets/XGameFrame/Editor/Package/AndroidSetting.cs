using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.Collections;
using System.IO;
using System.Xml;

public class AndroidSetting : MonoBehaviour
{
    //把要替换的文件都放到Setting/Android里面，要换什么就往下面加
    static string androidSettingPath = Application.dataPath.Substring(0, Application.dataPath.Length - 7) + "/Setting/Android";

    [PostProcessBuild(999)]
    public static void OnPostprocessBuild(BuildTarget BuildTarget, string path)
    {
        if (BuildTarget != BuildTarget.Android) return;

        string ap = path + "/" + Application.productName;

        //manifest
        string manifestPath = androidSettingPath + "/AndroidManifest.xml";
        string manifestPathNew = ap + "/src/main/AndroidManifest.xml";
        Copy(manifestPath, manifestPathNew);

        //gradle
        string gPath = androidSettingPath + "/build.gradle";
        string gPathNew = ap + "/build.gradle";
        Copy(gPath, gPathNew);

        //UnityPlayerActivity
        string uPath = androidSettingPath + "/UnityPlayerActivity.java";
        string uPathNew = ap + "/src/main/java/" + Application.productName + "/UnityPlayerActivity";
        if(!File.Exists(uPathNew))
        {
            uPathNew = path + "/" + Application.productName + "/src/main/java/";
            string[] flood = Application.identifier.Split('.');
            for(int i=0;i<flood.Length;i++)
                uPathNew += flood[i] + "/";
            uPathNew += "UnityPlayerActivity.java";
        }
        Copy(uPath, uPathNew);
    }

    public static void Copy(string from,string to)
    {
        if (File.Exists(from))
        {
            File.Copy(from, to, true);
        }
        else
        {
            Debug.Log("有文件不存在 " + from + " " + to);
        }
    }
}
