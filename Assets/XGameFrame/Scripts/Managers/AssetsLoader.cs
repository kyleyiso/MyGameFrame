using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetsLoader : MonoBehaviour
{
    /// <summary>
    /// 超级明星小图标
    /// </summary>
    private static Dictionary<int,Sprite> SuperStarSmallIconDic=new Dictionary<int, Sprite>(22);
    /// <summary>
    /// 超级明星大图标
    /// </summary>
    private static Dictionary<int, Sprite> SuperStarBigIconDic = new Dictionary<int, Sprite>(22);
    /// <summary>
    /// 食物图标
    /// </summary>
    private static Dictionary<int,Sprite> FoodProductIconDic=new Dictionary<int, Sprite>();
    /// <summary>
    /// 冒泡食物图标和套餐图标
    /// </summary>
    private static Dictionary<int,Sprite> FoodProductPopIconDic=new Dictionary<int, Sprite>(30);
    /// <summary>
    /// 汽车图标
    /// </summary>
    private static Dictionary<int, Sprite> SpecCarIconDic = new Dictionary<int, Sprite>(25);
    /// <summary>
    /// 微笑表情
    /// </summary>
    private static Dictionary<int,Sprite> SmileIconDic=new Dictionary<int, Sprite>(4);
    /// <summary>
    /// 房子大图标
    /// </summary>
    public static Dictionary<int, Sprite> HouseIconDic = new Dictionary<int, Sprite>(15);
    /// <summary>
    /// 房子小图标
    /// </summary>
    private static Dictionary<int, Sprite> HouseSmallIconDic = new Dictionary<int, Sprite>(15);
    /// <summary>
    /// 套餐Icon
    /// </summary>
    public static Dictionary<int, Sprite> ComboIconDic = new Dictionary<int, Sprite>(45);
    /// <summary>
    /// 套餐Icon
    /// </summary>
    public static Dictionary<int, Sprite> ExtraAdditonIconDic = new Dictionary<int, Sprite>(12);
    /// <summary>
    /// 幸运转盘
    /// </summary>
    public static Dictionary<int, Sprite> LuckRoundIconDic = new Dictionary<int, Sprite>(8);
    /// <summary>
    /// 库存车
    /// </summary>
    public static Dictionary<int, Sprite> TransPoreCarIconDic = new Dictionary<int, Sprite>(10);
    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    public static void LoadAllSprite()
    {
        //加载明星界面小图标
        Sprite[] tempSpritesArray= Resources.LoadAll<Sprite>("SuperStar_SmallIcon");
        for (int i = 0; i < tempSpritesArray.Length; i++)
        {
            SuperStarSmallIconDic.Add(int.Parse(tempSpritesArray[i].name), tempSpritesArray[i]);
        }
        //加载明星界面大图标
        tempSpritesArray= Resources.LoadAll<Sprite>("SuperStar_BigIcon");
        for (int i = 0; i < tempSpritesArray.Length; i++)
        {
            SuperStarBigIconDic.Add(int.Parse(tempSpritesArray[i].name), tempSpritesArray[i]);
        }
        //加载食物图标
        tempSpritesArray = Resources.LoadAll<Sprite>("FoodIcon");
        for (int i = 0; i < tempSpritesArray.Length; i++)
        {
            FoodProductIconDic.Add(int.Parse(tempSpritesArray[i].name), tempSpritesArray[i]);
        }
        tempSpritesArray = Resources.LoadAll<Sprite>("PopIcon");
        for (int i = 0; i < tempSpritesArray.Length; i++)
        {
            FoodProductPopIconDic.Add(int.Parse(tempSpritesArray[i].name), tempSpritesArray[i]);
        }
        tempSpritesArray = Resources.LoadAll<Sprite>("SmileIcon");
        for (int i = 0; i < tempSpritesArray.Length; i++)
        {
            SmileIconDic.Add(int.Parse(tempSpritesArray[i].name), tempSpritesArray[i]);
        }
        tempSpritesArray = Resources.LoadAll<Sprite>("CarIcon");
        for (int i = 0; i < tempSpritesArray.Length; i++)
        {
            SpecCarIconDic.Add(int.Parse(tempSpritesArray[i].name), tempSpritesArray[i]);
        }

        tempSpritesArray = Resources.LoadAll<Sprite>("HouseIcon");
        for (int i = 0; i < tempSpritesArray.Length; i++)
        {
            HouseIconDic.Add(int.Parse(tempSpritesArray[i].name), tempSpritesArray[i]);
        }

        tempSpritesArray = Resources.LoadAll<Sprite>("HouseSmallIcon");
        for (int i = 0; i < tempSpritesArray.Length; i++)
        {
            HouseSmallIconDic.Add(int.Parse(tempSpritesArray[i].name), tempSpritesArray[i]);
        }

        tempSpritesArray = Resources.LoadAll<Sprite>("ComboIcon");
        for (int i = 0; i < tempSpritesArray.Length; i++)
        {
            ComboIconDic.Add(int.Parse(tempSpritesArray[i].name), tempSpritesArray[i]);
        }
        tempSpritesArray = Resources.LoadAll<Sprite>("AdditonIcon");
        for (int i = 0; i < tempSpritesArray.Length; i++)
        {
            ExtraAdditonIconDic.Add(int.Parse(tempSpritesArray[i].name), tempSpritesArray[i]);
        }
        tempSpritesArray = Resources.LoadAll<Sprite>("LuckRound");
        for (int i = 0; i < tempSpritesArray.Length; i++)
        {
            LuckRoundIconDic.Add(int.Parse(tempSpritesArray[i].name), tempSpritesArray[i]);
        }
        tempSpritesArray = Resources.LoadAll<Sprite>("TransPortCar");
        for (int i = 0; i < tempSpritesArray.Length; i++)
        {
            TransPoreCarIconDic.Add(int.Parse(tempSpritesArray[i].name), tempSpritesArray[i]);
        }
    }

    private static Sprite TempSprite;
    /// <summary>
    /// 获取超级明星小图标
    /// </summary>
    /// <param name="SuperStarID"></param>
    /// <returns></returns>
    public static Sprite GetSuperStarSmallIcon(int SuperStarID)
    {
        if (SuperStarSmallIconDic.TryGetValue(SuperStarID, out TempSprite))
        {
            return TempSprite;
        }
        Debug.LogError("找不到ID为"+SuperStarID+"的明星图标小");
        return null;
    }
    /// <summary>
    /// 获取超级明星大图标
    /// </summary>
    /// <param name="SuperStarID"></param>
    /// <returns></returns>
    public static Sprite GetSuperStarBigIcon(int SuperStarID)
    {
        if (SuperStarBigIconDic.TryGetValue(SuperStarID, out TempSprite))
        {
            return TempSprite;
        }
        Debug.LogError("找不到ID为" + SuperStarID + "的明星图标大");
        return null;
    }

    public static Sprite GetFoodProductIcon(int FoodID)
    {
        if (FoodProductIconDic.TryGetValue(FoodID, out TempSprite))
        {
            return TempSprite;
        }
        Debug.LogError("找不到ID为" + FoodID + "的食物图标");
        return null;
    }

    public static Sprite GetFoodPopIcon(int PopID)
    {
        if (FoodProductPopIconDic.TryGetValue(PopID, out TempSprite))
        {
            return TempSprite;
        }
        Debug.LogError("找不到ID为" + PopID + "的气泡图标");
        return null;
    }

    public static Sprite GetSmileIcon(int Type)
    {
        if (SmileIconDic.TryGetValue(Type, out TempSprite))
        {
            return TempSprite;
        }
        Debug.LogError("找不到ID为" + Type + "的笑脸图标");
        return null;
    }

    public static Sprite GetSpecCarIcon(int CarID)
    {
        if (SpecCarIconDic.TryGetValue(CarID, out TempSprite))
        {
            return TempSprite;
        }
        Debug.LogError("找不到ID为" + CarID + "的汽车图标");
        return null;
    }
    public static Sprite GetHouseIcon(int HouseID)
    {
        if (HouseIconDic.TryGetValue(HouseID, out TempSprite))
        {
            return TempSprite;
        }
        Debug.LogError("找不到ID为" + HouseID + "的房子图标");
        return null;
    }
    public static Sprite GetHouseSmallIcon(int HouseID)
    {
        if (HouseSmallIconDic.TryGetValue(HouseID, out TempSprite))
        {
            return TempSprite;
        }
        Debug.LogError("找不到ID为" + HouseID + "的房子图标");
        return null;
    }

    public static Sprite GetCombaIcon(int ComboID)
    {
        if (ComboIconDic.TryGetValue(ComboID, out TempSprite))
        {
            return TempSprite;
        }
        Debug.LogError("找不到ID为" + ComboID + "的套餐图标");
        return null;
    }

    public static Sprite GetExtraAddtionIcon(int ID)
    {
        if (ExtraAdditonIconDic.TryGetValue(ID, out TempSprite))
        {
            return TempSprite;
        }
        Debug.LogError("找不到ID为" + ID + "的额外属性图片");
        return null;
    }
    public static Sprite GetLuckRoundIcon(int Order)
    {
        if (LuckRoundIconDic.TryGetValue(Order, out TempSprite))
        {
            return TempSprite;
        }
        Debug.LogError("找不到ID为" + Order + "的幸运转盘图标");
        return null;
    }
    public static Sprite GetTransPortCarIcon(int CarID)
    {
        if (TransPoreCarIconDic.TryGetValue(CarID, out TempSprite))
        {
            return TempSprite;
        }
        Debug.LogError("找不到ID为" + CarID + "的汽车图标");
        return null;
    }
}
