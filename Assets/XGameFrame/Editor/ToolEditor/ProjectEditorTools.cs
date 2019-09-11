using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PathologicalGames;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ProjectEditorTools : Editor
{
    
    [MenuItem("ProjectEditorHelp/创建汽车克隆体并命名")]
    public static void SetCarName()
    {

    }
    
    [MenuItem("ProjectEditorHelp/汽车设置Mesh")]
    public static void SetCarMeterial()
    {
        Transform parent = GameObject.Find("CarSpawnPool/Cars").transform;

        for (int i = 0; i < Selection.gameObjects.Length; i++)
        {
            for (int j = 0; j < parent.childCount; j++)
            {
                string CarName = parent.GetChild(j).name;
                if (Selection.gameObjects[i].name.Equals(CarName))
                {
                    parent.GetChild(j).GetChild(0).GetChild(0).GetComponent<MeshFilter>().mesh =
                       Selection.gameObjects[i].GetComponent<MeshFilter>().sharedMesh ;
                }
            }
        }
    }
    
    
    [MenuItem("ProjectEditorHelp/一键放置对象池-汽车")]
    public static void FileSpwanPool()
    {
        SpawnPool sp = GameObject.Find("CarSpawnPool").GetComponent<SpawnPool>();

        sp._perPrefabPoolOptions = new List<PrefabPool>(Selection.gameObjects.Length);

        for (int i = 0; i < Selection.gameObjects.Length; i++)
        {
            PrefabPool prefabPool = new PrefabPool(Selection.gameObjects[i].transform);
            prefabPool.preloadAmount = 3;
            prefabPool.preloadTime = true;
            prefabPool.preloadFrames = 2;
            sp._perPrefabPoolOptions.Add(prefabPool);
        }
    }

    private static string FontPath = "Font/ERASBD";//ERASBD为字体名称

    [MenuItem("ProjectEditorHelp/替换字体")]
    public static void ChangFont()
    {
        RecordSeletList.Clear();
        Font font = Resources.Load<Font>(FontPath);
        if (font == null)
        {
            Debug.LogError(FontPath + "目录下没有字体文件");
            return;
        }
        for (int i = 0; i < Selection.gameObjects.Length; i++)
        {
            FindText(Selection.gameObjects[i].transform);
        }

        for (int i = 0; i < RecordSeletList.Count; i++)
        {
            RecordSeletList[i].font = font;
            RecordSeletList[i].fontStyle = FontStyle.Normal;
        }
        Debug.Log("更换字体成功");
    }

    private static List<Text> RecordSeletList = new List<Text>();
    private static void FindText(Transform trans)
    {
        Text text = trans.GetComponent<Text>();

        if (text != null)
        {
            if (!RecordSeletList.Contains(text))
            {
                RecordSeletList.Add(text);
            }
        }

        foreach (Transform child in trans)
        {
            FindText(child);
            Text childText = child.GetComponent<Text>();
            if (childText != null)
            {
                if (!RecordSeletList.Contains(childText))
                {
                    RecordSeletList.Add(childText);
                }
            }
        }
    }

    private static List<Transform> RecordButtonScaleList = new List<Transform>();
    [MenuItem("ProjectEditorHelp/添加ButtonScale")]
    public static void AddButtonScale()
    {
        RecordButtonScaleList.Clear();

        for (int i = 0; i < Selection.gameObjects.Length; i++)
        {
            FindButton(Selection.gameObjects[i].transform);
        }

        for (int i = 0; i < RecordButtonScaleList.Count; i++)
        {
            if (!RecordButtonScaleList[i].gameObject.GetComponent<ButtonScale>())
            {
                RecordButtonScaleList[i].gameObject.AddComponent<ButtonScale>();
            }
        }
        Debug.Log("添加ButtonScale成功");
    }

    private static void FindButton(Transform trans)
    {
        string[] transName = trans.name.Split('_');
        if (transName.Length > 1 && transName[1].Equals("Button"))
        {
            if (!RecordButtonScaleList.Contains(trans))
            {
                RecordButtonScaleList.Add(trans);
            }
        }

        foreach (Transform child in trans)
        {
            FindButton(child);
            string[] transName1 = child.name.Split('_');
            if (transName1.Length > 1 && transName1[1].Equals("Button"))
            {
                if (!RecordButtonScaleList.Contains(child))
                {
                    RecordButtonScaleList.Add(child);
                }
            }
        }
    }
}
