using System;
using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;

public class Excel2Json2Cs : EditorWindow
{
	//默认路径;
    static string excelPath ;
    static string jsonPath;
    static string csharpPath ;
    static bool isGenJson;

    static string ExclePathKey ;
    static string CsharpPathKey ;
    static string JsonPathKey ;
    static string IsGenJsonKey ;

    private bool IsFinish = false;


    private static string excleLocalPath;
    private static string csharpLocalPath;
    private static string jsonLocalPath; 

    public const int Float_Accurate = 3;
    public const int Double_Accurate = 5;
    private static Excel2Json2Cs ExcleWindow; 

    [MenuItem("XGameTools/Excle转Charp&Json")]
    static void Init()
    {
        ExclePathKey = Application.dataPath + "ExclePathKey";
        CsharpPathKey = Application.dataPath + "CsharpPathKey";
        JsonPathKey = Application.dataPath + "JsonPathKey";
        IsGenJsonKey = Application.dataPath + "IsGenJsonKey";

        excleLocalPath = Application.dataPath + "/XGameFrame/OutPutData/A_Excle";
        csharpLocalPath = Application.dataPath + "/XGameFrame/OutPutData/B_Csharp";
        jsonLocalPath = Application.dataPath + "/XGameFrame/OutPutData/C_Json";

        #region 读取保存的路径
        if (!string.IsNullOrEmpty(EditorPrefs.GetString(ExclePathKey)))
        {
            excelPath = EditorPrefs.GetString(ExclePathKey);
            if (!Directory.Exists(excelPath))
            {
                excelPath = excleLocalPath;
            }
        }
        else
        {
            excelPath = excleLocalPath;
        }

        if (!string.IsNullOrEmpty(EditorPrefs.GetString(CsharpPathKey)))
        {
            csharpPath = EditorPrefs.GetString(CsharpPathKey);
            if (!Directory.Exists(csharpPath))
            {
                csharpPath = csharpLocalPath;
            }
        }
        else
        {
            csharpPath = csharpLocalPath;
        }

        if (!string.IsNullOrEmpty(EditorPrefs.GetString(JsonPathKey)))
        {
            jsonPath = EditorPrefs.GetString(JsonPathKey);
            if (!Directory.Exists(jsonPath))
            {
                jsonPath = jsonLocalPath;
            }
        }
        else
        {
            jsonPath = jsonLocalPath;
        }

        isGenJson = EditorPrefs.GetBool(IsGenJsonKey);
        #endregion
        

        ExcleWindow = (Excel2Json2Cs)GetWindow(typeof(Excel2Json2Cs));//创建窗口
        ExcleWindow.Show();//展示
    }
    

    void OnGUI()
    {
        
        if (GUI.Button(new Rect(10, 10, 100, 50), "请选择Excel路径"))
        {
            string FilePathValue = EditorUtility.OpenFolderPanel("请选择Excel路径", "", "");
            if (!string.IsNullOrEmpty(FilePathValue))
            {
                excelPath = FilePathValue;
                EditorPrefs.SetString(ExclePathKey, excelPath);
            }
        }
        GUI.Label(new Rect(120, 30, 600, 30), excelPath);

        
        if (GUI.Button(new Rect(10, 70, 100, 50), "请选择C#路径"))
        {
            string FilePathValue = EditorUtility.OpenFolderPanel("请选择C#路径", "", "");
            if (!string.IsNullOrEmpty(FilePathValue))
            {
                csharpPath = FilePathValue;
                EditorPrefs.SetString(CsharpPathKey, csharpPath);
            }
        }
        GUI.Label(new Rect(120, 90, 600, 30), csharpPath);

        if (!string.IsNullOrEmpty(csharpPath))
        {
            if (GUI.Button(new Rect(10, 260, 120, 50), "删除所有Charp文件"))
            {
                DestorFile(csharpPath);
            }
        }
        

        if (GUI.Button(new Rect(10, 130, 100, 50), "请选择Json路径"))
        {
			string FilePathValue = EditorUtility.OpenFolderPanel("请选择Json路径", "", "");
            if (!string.IsNullOrEmpty(FilePathValue))
            {
                jsonPath = FilePathValue;
                EditorPrefs.SetString(JsonPathKey, jsonPath);
            }
        }
        GUI.Label(new Rect(120, 150, 600, 30), jsonPath);
        if (!string.IsNullOrEmpty(jsonPath))
        {
            if (GUI.Button(new Rect(150, 260, 120, 50), "删除所有Json文件"))
            {
                DestorFile(jsonPath);
            }
        }

        if (GUI.Button(new Rect(285, 200, 120, 50), "重置Excle路径"))
        {
            excelPath = excleLocalPath;
            EditorPrefs.SetString(ExclePathKey, excelPath);
        }
        if (GUI.Button(new Rect(435, 200, 120, 50), "重置Csharp路径"))
        {
            csharpPath = csharpLocalPath;
            EditorPrefs.SetString(CsharpPathKey, csharpPath);
        }
        if (GUI.Button(new Rect(575, 200, 120, 50), "重置Json路径"))
        {
            jsonPath = jsonLocalPath;
            EditorPrefs.SetString(JsonPathKey, jsonPath);
        }


        EditorGUI.BeginChangeCheck();

        GUI.Toggle(new Rect(10, 220, 130, 50), isGenJson, "是否生成Json文件");
       
        if (EditorGUI.EndChangeCheck())
        {
            isGenJson = !isGenJson;
            if (!isGenJson)
            {
                EditorPrefs.SetBool(IsGenJsonKey, false);
            }
            else
            {
                EditorPrefs.SetBool(IsGenJsonKey, true);
            }
        }

        
        if (GUI.Button(new Rect(150,350,200,50),"确定"))
		{
		    CheckFilePath(FilePathType.Excle);
		    CheckFilePath(FilePathType.Cshap);
            if (isGenJson)
		    {
		        CheckFilePath(FilePathType.Json);
                IsFinish =  ExcleGenerator.Excel2Json(excelPath, csharpPath, jsonPath);
            }
		    else
		    {
                IsFinish =  ExcleGenerator.Excel2Json(excelPath, csharpPath, String.Empty);
            }

		    if (IsFinish)
		    {
		        if (EditorUtility.DisplayDialog("", "完成", "确定"))
		        {
		            ExcleWindow.Close();
                }
            }
		    else
		    {
		        EditorUtility.DisplayDialog("", "出错了", "确定");
            }
            AssetDatabase.Refresh();
		}
    }

    private void DestorFile(string path)
    {
        DirectoryInfo folder = new DirectoryInfo(path);
        FileSystemInfo[] files = folder.GetFileSystemInfos();
        for (int i = 0; i < files.Length; i++)
        {
            files[i].Delete();
        }
        AssetDatabase.Refresh();
    }

    /// <summary>
    /// 检查文件路径是否正确
    /// </summary>
    /// <param name="filePathType"></param>
    private void CheckFilePath(FilePathType filePathType)
    {
        switch (filePathType)
        {
            case FilePathType.Excle:
               if (!Directory.Exists(excelPath))
               {
                   excelPath = excleLocalPath;
                   EditorUtility.DisplayDialog("", "Excle文件路径错误,重设置为默认路径", "确定");
                }
               
                break;
            case FilePathType.Cshap:
                if (!Directory.Exists(csharpPath))
                {
                    csharpPath = csharpLocalPath;
                    EditorUtility.DisplayDialog("", "Cshap文件路径错误,重设置为默认路径", "确定");
                }
                
                break;
            case FilePathType.Json:
                if (!Directory.Exists(jsonPath))
                {
                    jsonPath = jsonLocalPath;
                    EditorUtility.DisplayDialog("", "Json文件路径错误,重设置为默认路径", "确定");
                }

                break;
        }
        //默认路径如果没有文件则创建文件
        if (!Directory.Exists(excleLocalPath))
        {
            Directory.CreateDirectory(excleLocalPath);
        }
        if (!Directory.Exists(csharpLocalPath))
        {
            Directory.CreateDirectory(csharpLocalPath);
        }
        if (!Directory.Exists(jsonLocalPath))
        {
            Directory.CreateDirectory(jsonLocalPath);
        }
    }
}

public enum FilePathType
{
    Excle,
    Cshap,
    Json
}