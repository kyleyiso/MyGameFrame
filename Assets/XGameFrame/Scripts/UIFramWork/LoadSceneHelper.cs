using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

class LoadSceneHelper : MonoBehaviour
{
    private AsyncOperation asyn; //异步对象
    private Action actionAfterLoadScene; //加载完场景之后需要做的事情
    private float curLoadedValue; //当前加载的进度
    [SerializeField]
    private Image LeftNumberImage;
    [SerializeField]
    private Image MiddleNumberImage;
    [SerializeField]
    private Image RightNumberImage;
    [SerializeField]
    private Image ProgramImage;
    private Dictionary<int,Sprite> LoadNumberDic= new Dictionary<int, Sprite>(10);
    void Start()
    {
        LaodNumSprites();
        LoadScene("Main", null);
    }
    /// <summary>
    /// 加载数字图片
    /// </summary>
    private void LaodNumSprites()
    {
        Sprite[] NumberSpArray = Resources.LoadAll<Sprite>("LoadingNumber");
        for (int i = 0; i < NumberSpArray.Length; i++)
        {
            LoadNumberDic.Add(int.Parse(NumberSpArray[i].name),NumberSpArray[i]);
        }
    }

    private Sprite sprite;
    private Sprite GetNumberSprite(int Number)
    {
        if (LoadNumberDic.TryGetValue(Number, out sprite))
        {
            return sprite;
        }
        Debug.Log("找不到对应的图片");
        return null;
    }

    public void LoadScene(string name, Action action)
    {
        curLoadedValue = 0;
        //记录传进来的这个Action1
        actionAfterLoadScene = action;
        //开启一个协程
        StartCoroutine(taskLoadScene(name));
    }

    IEnumerator taskLoadScene(string name)
    {
        asyn = SceneManager.LoadSceneAsync(name); //异步加载场景的方法
        asyn.allowSceneActivation = false; //避免加载到90%就进入场景！！永远只会到90%
        yield return asyn;
    }

    private float LoadingTimer;//3秒之后进入
    private int LeftNum;
    private int MiddleNum;
    private int RightNum;
    void FixedUpdate()
    {
        if (asyn == null)
        {
            return;
        }

        if (curLoadedValue < 30)
        {
            curLoadedValue = Mathf.Lerp(curLoadedValue, 30, Time.fixedTime);
        }
        else if (curLoadedValue>=30 && curLoadedValue<90)
        {
            curLoadedValue = Mathf.Lerp(curLoadedValue, 90, Time.fixedTime*0.2f);
        }
        else
        {
            curLoadedValue = Mathf.Lerp(curLoadedValue, 100, Time.fixedTime*0.05f);
        }
        
        
        
        if (curLoadedValue < 10 )
        {
            LeftNumberImage.gameObject.SetActive(true);
            LeftNumberImage.sprite = GetNumberSprite((int) curLoadedValue);
        }
        else if (curLoadedValue >= 99.99f)
        {
            RightNumberImage.gameObject.SetActive(true);
            curLoadedValue = 100;
            
            LeftNumberImage.sprite = GetNumberSprite(1);
            MiddleNumberImage.sprite = GetNumberSprite(0);
            RightNumberImage.sprite = GetNumberSprite(0);

            asyn.allowSceneActivation = true; //直接就进去场景了
        }
        else
        {
            MiddleNumberImage.gameObject.SetActive(true);

            LeftNum = (int) curLoadedValue / 10;
            MiddleNum= (int)curLoadedValue % 10;

            LeftNumberImage.sprite = GetNumberSprite(LeftNum);
            MiddleNumberImage.sprite =GetNumberSprite(MiddleNum);
        }

        ProgramImage.fillAmount = curLoadedValue / 100f; //把百分比赋值给图片

        if (asyn.isDone)
        {
            if (actionAfterLoadScene != null)
            {
                actionAfterLoadScene();
            }

            asyn = null;
        }
    }
}
