using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ApplyMaterialEdltor : EditorWindow
{
    private static string SeletMatPath;
    private static ApplyMaterialEdltor MatWindow;

    private static Material SelectMat;
    [SerializeField]
    private static List<GameObject> RecordSeletList=new List<GameObject>();

    [MenuItem("Assets/批量替换材质",false)]
    static void Init()
    {
        if (Selection.objects.Length > 0 )
        {
            SeletMatPath = AssetDatabase.GetAssetPath(Selection.objects[0]);
            SelectMat = Selection.objects[0] as Material;
        }

        if (SelectMat != null )
        {
            MatWindow = (ApplyMaterialEdltor)GetWindow(typeof(ApplyMaterialEdltor));//创建窗口
            MatWindow.Show();//展示
        }
    }
    
    private void OnSelectionChange()
    {
        MatWindow.Repaint();
    }
    
    void OnGUI()
    {
        if (GUI.Button(new Rect(20, 30, 100, 50), "设置选中的材质"))
        {
            Object[] SelectObjMatArray = Selection.GetFiltered(typeof(Material), SelectionMode.Assets);
            if (SelectObjMatArray.Length > 0)
            {
                SelectMat = SelectObjMatArray[0] as Material;
                SeletMatPath = AssetDatabase.GetAssetPath(SelectObjMatArray[0]);
            }
        }

        if (SelectMat != null)
        {
            GUI.Label(new Rect(140, 40, 600, 30),string.Format("选中的材质为:{0}", SelectMat.name));
            GUI.Label(new Rect(140, 60, 600, 30),string.Format("材质的路径为:{0}", SeletMatPath));
            if (GUI.Button(new Rect(20, 100, 100, 50), "替换"))
            {
                if (SelectMat != null)
                {
                    if (RecordSeletList.Count > 0)
                    {
                        for (int i = 0; i < RecordSeletList.Count; i++)
                        {
                            ReplaceMat(RecordSeletList[i]);
                        }
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("", "请选择需要替换的物体", "确定");
                    }
                }
                else
                {
                    EditorUtility.DisplayDialog("", "请选择一个材质", "确定");
                }
            }

            GUI.Label(new Rect(140, 120, 600, 30), "需要替换的物体：");
            if (GUI.Button(new Rect(150, 140, 70, 40), "清除未选中"))
            {
                RecordSeletList.Clear();
            }
            for (int i = 0; i < Selection.gameObjects.Length; i++)
            {
                FindMater(Selection.gameObjects[i]);
            }

            for (int i = 0; i < RecordSeletList.Count; i++)
            {
                GUI.Label(new Rect(240, 120 + i * 15, 600, 30), RecordSeletList[i].name);
            }
        }
    }


    /// <summary>
    /// 递归遍历子节点
    /// </summary>
    /// <param name="go"></param>
    /// <param name="m"></param>
    public static void FindMater(GameObject go)
    {
        MeshRenderer parentMeshRenderer = go.GetComponent<MeshRenderer>();

        if (parentMeshRenderer != null)
        {
            if (!RecordSeletList.Contains(parentMeshRenderer.gameObject))
            {
                RecordSeletList.Add(parentMeshRenderer.gameObject);
            }
        }
        
        foreach (Transform child in go.transform)
        {
            FindMater(child.gameObject);
            MeshRenderer childMeshRenderer = child.GetComponent<MeshRenderer>();
            if (childMeshRenderer != null)
            {
                if (!RecordSeletList.Contains(child.gameObject))
                {
                    RecordSeletList.Add(child.gameObject);
                }
            }
        }
    }

    private static void ReplaceMat(GameObject go)
    {
        MeshRenderer meshRenderer = go.GetComponent<MeshRenderer>();
        Undo.RecordObject(meshRenderer, "Record Mat");
        meshRenderer.materials=new Material[1];
        meshRenderer.material = SelectMat;
    }
}
