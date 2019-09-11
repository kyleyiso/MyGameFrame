using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace XGameTools
{
    /// <summary>
    /// 工具类
    /// </summary>
    public class UITools
    {

        /// <summary>
        ///  把对象放入到指定的父物体下面 
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="child"></param>
        public static void addChildToParent(Transform parent, Transform child)
        {
            child.SetParent(parent);
            child.localPosition = Vector3.zero;
            child.localScale = Vector3.one;
            child.localEulerAngles = Vector3.zero;

            if (child.GetComponent<RectTransform>())
            {
                child.GetComponent<RectTransform>().offsetMin = Vector2.zero;
                child.GetComponent<RectTransform>().offsetMax = Vector2.zero;
            }
        }

        /// <summary>
        ///  查找父物体下面的子物体 
        /// </summary>
        /// <param name="goParent"></param>
        /// <param name="childName"></param>
        /// <returns></returns>
        public static Transform Find_Child(GameObject goParent, string childName)
        {
            Transform transChild = goParent.transform.Find(childName);
            if (transChild == null)//如果没有找到
            {
                foreach (Transform t in goParent.transform)//父物体下面的所有子物体
                {
                    transChild = Find_Child(t.gameObject, childName);
                    if (transChild != null)
                    {
                        return transChild;
                    }
                }
            }
            return null;
        }
        /// <summary>
        /// 将MyVector3转换为Vector3
        /// </summary>
        /// <param name="_MyVector3"></param>
        /// <returns></returns>
        public static Vector3 ConverToVector3(MyVector3 _MyVector3)
        {
            return new Vector3(_MyVector3.x, _MyVector3.y, _MyVector3.z);
        }

        /// <summary>
        /// 清除内存 
        /// </summary>
        public static void ClearMemory()
        {
            GC.Collect();
            Resources.UnloadUnusedAssets();
        }
    }

}

