using UnityEngine;
using System.Collections;
using System;
using DG.Tweening;//引入动画控制的命名空间

public enum AnimationType
{
    None,
    Short,
    Long
}

public enum UIShowMode //打开一个UI的时候，其它UI应该怎么办
{
    doNothing,//当前UI显示出来的时候其它UI不做任何操作   
    hideAllExceptAbove,//除了最前面的不隐藏，其它UI全部都隐藏(除了在UIRoot1KeepAbove下的UI，其它的都隐藏)
    hideAll//隐藏所有的UI，包括最前面的  
}

public class UIBase : MonoBehaviour
{
    public UiId UiId;
    // protected UiId idBefore = UiId.Null;//当前界面的上一个界面 暂未用到
    protected Tweener animShow;//UI显示时的动画
    protected Tweener animHide;//UI隐藏时的动画
    private bool isKeepAbove = false;//主要是用在主界面和菜单显示，一直不消失，除非特殊情况
    public UIShowMode showMode = UIShowMode.doNothing;//ui的显示模式（打开什么也不干，隐藏所有其它的UI，除了主界面UI隐藏所有其它UI）
    public AnimationType _AnimationType = AnimationType.Short;
    protected virtual void Awake()
    {
        OnInit();
    }

    public bool IsKeepAbove
    {
        get { return isKeepAbove; }
        set { isKeepAbove = value; }
    }


    //虚函数，子类来实现该方法
    public virtual void OnInit()//1：用来初始化一些数据和UI组件
    {
        
    }
    public virtual void OnShow(object objParm = null)//2：显示UI时候调用的方法，如果你打开该界面需要操作，就写在这里面
    { 
        
    }
    public virtual void OnHide()//3：隐藏UI时候调用的方法，隐藏该界面需要操作什么，就写里面
    { 
    
    }
    
    /// <summary>
    /// 显示UI的方法
    /// </summary>
    public void Show(object objParm)
    {
        this.gameObject.SetActive(true);//即将显示的UI设置为显示
        //重新播放动画
        if (animShow != null)
        {
            animShow.Restart();
        }
        OnShow(objParm);//虚方法，子类实现
    }

    /// <summary>
    /// 隐藏UI的方法
    /// </summary>
    /// <param name="action">隐藏UI之后需要做的事情</param>
    public void Hide(Action action=null)
    {
        OnHide();
        if (animHide != null)//如果有U1I出去的动画
        {
            animHide.Restart();
        }
        else//如果没有1UI出去的动画
        {
            this.gameObject.SetActive(false);//直接隐藏
        }
        if (action != null)
        {
            action.Invoke();
        }
    }

   
}
