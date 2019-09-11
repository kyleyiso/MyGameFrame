using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CombinMeshEditor:Editor
{

    [MenuItem("GameObject/合并网格", false, -32)]
    static void CombineMesh()
    {
        GameObject[] objs = Selection.gameObjects;

        for (int j = 0; j < objs.Length; j++)
        {
            MeshFilter[] meshfilters = objs[j].GetComponentsInChildren<MeshFilter>();

            CombineInstance[] combine = new CombineInstance[meshfilters.Length];

            Matrix4x4 matrix = objs[j].transform.worldToLocalMatrix;

            for (int i = 0; i < meshfilters.Length; i++)
            {
                MeshFilter mf = meshfilters[i];

                MeshRenderer mr = mf.GetComponent<MeshRenderer>();

                if (mr == null) continue;

                combine[i].mesh = mf.sharedMesh;

                combine[i].transform = matrix * mf.transform.localToWorldMatrix;
            }
            Mesh mesh = new Mesh();

            mesh.name = objs[j].name;

            mesh.CombineMeshes(combine, true);

            string path = @"Assets/" + mesh.name + ".asset";

            AssetDatabase.CreateAsset(mesh, path);
        }
        AssetDatabase.Refresh();
    }
}
