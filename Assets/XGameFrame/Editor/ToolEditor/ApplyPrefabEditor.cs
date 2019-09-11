using UnityEngine;

using System.Collections;

using UnityEditor;

public class ApplyPrefabEditor : Editor
{

    [MenuItem("GameObject/批量Apple预制体", false, -32)]
    public static void PastePrefeb()
    {
        GameObject[] objs = Selection.gameObjects;

        if (null == objs || objs.Length < 1)
        {
            Debug.LogError("没有选中prefab");
            return;
        }

        for (int i = 0; i < objs.Length; i++)
        {
            Prefebs(objs[i]);
        }
    }

    private static void Prefebs(GameObject temp)
    {
        if (PrefabUtility.GetCorrespondingObjectFromSource(temp.gameObject) != null)
        {
            PrefabUtility.ReplacePrefab(temp.gameObject, PrefabUtility.GetCorrespondingObjectFromSource(temp),
                ReplacePrefabOptions.ConnectToPrefab);
        }
    }


    //public static void BatchApplyPrefab()
    //{
    //    GameObject[] objs = Selection.gameObjects;

    //    if (null == objs || objs.Length < 1)
    //    {
    //        Debug.LogError("没有选中prefab");
    //        return;
    //    }

    //    for (int i = 0; i < objs.Length; i++)
    //    {
    //        ApplyPrefab(objs[i]);
    //    }
    //}

    //public static void ApplyPrefab(GameObject obj)
    //{
    //    if (null == obj)
    //    {
    //        Debug.LogError("选中的obj 是 null");
    //        return;
    //    }

    //    PrefabType type = PrefabUtility.GetPrefabType(obj);
    //    if (type != PrefabType.PrefabInstance)
    //    {
    //        Debug.LogError("选中的obj " + obj.name + "  不是 PrefabInstance ");
    //        return;
    //    }

    //    //这里必须获取到prefab实例的根节点，否则ReplacePrefab保存不了
    //    GameObject prefabObj = GetPrefabInstanceParent(obj);
    //    UnityEngine.Object prefabAsset = null;
    //    if (prefabObj != null)
    //    {
    //        prefabAsset = PrefabUtility.GetCorrespondingObjectFromSource(prefabObj);
    //        if (prefabAsset != null)
    //        {
    //            PrefabUtility.ReplacePrefab(prefabObj, prefabAsset, ReplacePrefabOptions.ConnectToPrefab);
    //            Debug.Log("PrefabInstance ：" + prefabObj.name + "  Apply 成功");
    //        }
    //    }
    //    AssetDatabase.SaveAssets();
    //}

    ////遍历获取prefab节点所在的根prefab节点
    //static GameObject GetPrefabInstanceParent(GameObject obj)
    //{
    //    if (obj == null)
    //    {
    //        return null;
    //    }

    //    PrefabType pType = PrefabUtility.GetPrefabType(obj);
    //    if (pType != PrefabType.PrefabInstance)
    //    {
    //        return null;
    //    }

    //    if (obj.transform.parent == null)
    //    {
    //        return obj;
    //    }

    //    pType = PrefabUtility.GetPrefabType(obj.transform.parent.gameObject);
    //    if (pType != PrefabType.PrefabInstance)
    //    {
    //        return obj;
    //    }

    //    return GetPrefabInstanceParent(obj.transform.parent.gameObject);
    //}

}
