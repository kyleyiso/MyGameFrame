using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class UIEditorHelp : Editor
{

    [MenuItem("GameObject/生成UI代码", false, -33)]
    static void GenCharpFile()
    {
        Transform UIRoot = GameObject.Find("UIRoot").transform;
        UIEditorWirteFile.WriteUICsharpClass(UIRoot);
        UIEditorWirteFile.WriteUIID(UIRoot);
    }

    [MenuItem("GameObject/添加UI脚本", false, -33)]
    static void AddUIComponent()
    {
        Transform UIRoot = GameObject.Find("UIRoot").transform;
        UIEditorWirteFile.AddComponentToUI(UIRoot);
    }
}
