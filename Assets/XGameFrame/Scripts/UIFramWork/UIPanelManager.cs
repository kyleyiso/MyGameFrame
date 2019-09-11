using System;
using System.Collections.Generic;
using UnityEngine;

public class UIPanelManager : MonoBehaviour
{
    private static Dictionary<UiId, UIBase> dicAllUI = new Dictionary<UiId, UIBase>(); //所有打开过的UI,包括显示和不显示
    private static Dictionary<UiId, UIBase> dicShowUI = new Dictionary<UiId, UIBase>(); //所有正在显示的UI

    /// <summary>
    /// 初始化UI
    /// </summary>
    public static void Init()
    {
        if (dicAllUI != null)
        {
            dicAllUI.Clear();
        }
        if (dicShowUI!=null)
        {
            dicShowUI.Clear();
        }

        Transform UIRoot = GameObject.Find("UIRoot").transform;
        if (UIRoot == null)
        {
            Debug.LogError("找不到UIRoot");
            return;
        }

        for (int i = 0; i < UIRoot.childCount; i++)
        {
            UIBase uiBase = UIRoot.GetChild(i).GetComponent<UIBase>();
            if (uiBase != null)
            {
                if (dicAllUI != null)
                {
                    dicAllUI.Add(uiBase.UiId, uiBase);
                    //隐藏起来
                    UIRoot.GetChild(i).gameObject.SetActive(false);
                }
            }
        }
    }

    //从正在显示的UI字典中删除掉该UI
    private void DelUI(UiId id)
    {
        if (dicShowUI.ContainsKey(id))
        {
            dicShowUI.Remove(id);
        }
    }

    /// <summary>
    /// 从容器中获取需要显示的UI
    /// </summary>
    /// <param name="id">传入进来的ID</param>
    /// <returns></returns>
    private static UIBase GetUIByAllUIDic(UiId id)
    {
        if (dicAllUI.ContainsKey(id))
        {
            return dicAllUI[id];
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// 显示UI的方法 需要手动调用
    /// </summary>
    /// <param name="id"></param>
    public static void ShowUI(UiId id, object objParm = null) //1.处理当前UI，2，处理即将显示的UI，3，回调了虚方法
    {
        if (id == UiId.Null)
        {
            return;
        }

        //1：加载当前UI
        if (dicShowUI.ContainsKey(id)) //当前UI已经在显示列表中了，就直接返回
        {
            return;
        }

        UIBase ui = GetUIByAllUIDic(id); //通过ID获取需要显示的UI，从dicAll1UI容器中获取

        if (ui == null) //如果在容器中没有此UI
        {
            return;
        }

        //2:更新显示其它的UI
        UpdateOtherUIState(ui); //当前这个UI可能需要做动画出去，等它出去之后才打开即将显示的UI

        //3:显示当前UI
        dicShowUI[id] = ui;
        //当该UI隐藏时就从该字典删除
        ui.Show(objParm);
    }

    public static void HideUI(UiId id, Action action = null) //隐藏UI，传入ID和需要做的事情
    {
        if (!dicShowUI.ContainsKey(id)) //正在显示的容器中没有此ID
        {
            return;
        }

        if (action == null) //隐藏UI的时候不需要做别的事情
        {
            dicShowUI[id].Hide(); //直接隐藏
            dicShowUI.Remove(id); //从显示列表中删除 
        }
        else //隐藏窗体之后需要做的事情
        {
            action += delegate { dicShowUI.Remove(id); };
            dicShowUI[id].Hide(action);
        }
    }

    private static void UpdateOtherUIState(UIBase ui) //更新其它UI的状态（显示或者隐藏）
    {
        if (ui.showMode == UIShowMode.hideAll)
        {
            HideAllUI(true); //隐藏所有的UI
        }
        else if (ui.showMode == UIShowMode.hideAllExceptAbove) //剔除最前面UI
        {
            HideAllUI(false); //隐藏所有的UI
        }
    }

    public static void HideAllUI(bool isHideAbove) //隐藏所有的UI，参数表示是否隐藏最前面主UI
    {
        //isHideAbove是真，隐藏所有的UI，包括最上层的
        if (isHideAbove) //隐藏最上面的UI
        {
            foreach (KeyValuePair<UiId, UIBase> item in dicShowUI) //遍历所有正在显示的UI
            {
                item.Value.Hide();
            }

            dicShowUI.Clear();
        }
        else
        {
            //不隐藏最上面的主UI，其他的全部隐藏
            List<UiId> listRemove = new List<UiId>();
            foreach (KeyValuePair<UiId, UIBase> item in dicShowUI)
            {
                if (item.Value.IsKeepAbove)
                {
                    continue;
                }

                item.Value.Hide();
                listRemove.Add(item.Key);
            }

            for (int i = 0; i < listRemove.Count; i++)
            {
                dicShowUI.Remove(listRemove[i]);
                //循环结束后，只剩下了主UI（类型为IsKeepAbove）
            }

            listRemove.Clear();
        }
    }

    /// <summary>
    /// 指定的UI是否正在显示
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static bool IsUIShowing(UiId id)
    {
        return dicShowUI.ContainsKey(id);
    }

    public static UIBase GetUInstance<T>(UiId _uiId) where T : UIBase
    {
        if (dicAllUI.ContainsKey(_uiId))
        {
            return dicAllUI[_uiId] as T;
        }
        return null;
    }
    /// <summary>
    /// 当前的显示的UI是否仅仅只有上层UI
    /// </summary>
    /// <returns></returns>
    public static bool IsOnlyKeepAboveShow()
    {
        foreach (var uiBase in dicShowUI)
        {
            if (uiBase.Value.IsKeepAbove == false)
            {
                return false;
            }
        }
        return true;
    }
}
//public static T GetUInstance<T>(UiId _uiId) where T : UIBase
    //{


    //    return bf.Deserialize(fileStream) as T;
    //}
