using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// 全局变量
/// </summary>
public class AppVariable : MonoBehaviour
{
    private static AppVariable instance;
    public static AppVariable Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.Find("GameManager").GetComponent<AppVariable>();
            }

            return instance;
        }
    }
    
    void Awake ()
	{

    }
}
