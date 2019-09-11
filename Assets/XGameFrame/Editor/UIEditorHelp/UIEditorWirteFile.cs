using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using XGameTools;
using UnityEditor;
using UnityEngine;

[Serializable]
public class UIInstanceProperty//通过InstanceID判断该UI控件是新生成还是改名
{
    public List<string> ItemList;
}


public class UIEditorWirteFile : MonoBehaviour
{
    //编辑器数据路径
    public static string EditorConfigFileName =Application.dataPath+ @"/XGameFrame/Editor/Excel2Json2CSharp/DoNotDeleteThisFile/EditorData.txt";

    private const string UIProtertyPrefix= "@UIProtertyStar";
    private const string UIProtertySuffix = "@UIProtertyEnd";
    private const string UIProtertyWarningText = "UI控件字段 ";

    private const string UIBandPrefix = "@UIBandStar";
    private const string UIBandSuffix = "@UIBandEnd";
    private const string UIBandWarningText = "UI事件绑定";

    private const string UIEvenPrefix = "@UIEvenStar";
    private const string UIEvenSuffix = "@UIEvenEnd";
    private const string UIEvenWarningText = "UI点击事件";

    private const string Region = "#region";
    private const string Endregion = "#endregion";

    /// <summary>
    /// 写入UI脚本文件
    /// </summary>
    /// <param name="UIRoot"></param>
    public static void WriteUICsharpClass(Transform UIRoot)
    {
        
        for (int i = 0; i < UIRoot.childCount; i++)
        {
            StringBuilder _stringBuilder = new StringBuilder();
            string FileName = UIRoot.GetChild(i).name;
            if (FileName.Equals("Other"))
            {
                continue;
            }
            string CsharpFilePath = Application.dataPath + "/XGameFrame/Scripts/A_UIScrpts/" + FileName + ".cs";
            if (File.Exists(CsharpFilePath))//如果已经存在文件，仅重写UI控件部分，避免误操作
            {
                //重写UI控件区域
                WriteUIControlProperty(UIRoot.GetChild(i), _stringBuilder, CsharpFilePath,true);
                continue;
            }
            //前部分
            
            _stringBuilder.AppendLine("using System.Collections;");
            _stringBuilder.AppendLine("using System.Collections.Generic;");
            _stringBuilder.AppendLine("using UnityEngine;");
            _stringBuilder.AppendLine("using UnityEngine.UI;");
            _stringBuilder.Append('\n');
            _stringBuilder.AppendLine(string.Format("public class {0} : UIBase", FileName));
            _stringBuilder.AppendLine("{");
            _stringBuilder.AppendLine("    "+ Region +" "+ UIProtertyWarningText+ UIProtertyPrefix);
            //写入UI控件属性
            //_stringBuilder.Append("\n");
            WriteUIControlProperty(UIRoot.GetChild(i), _stringBuilder,CsharpFilePath,false);
            //后部分
            _stringBuilder.Append("\n");
            _stringBuilder.AppendLine("    "+Endregion+" "+ UIProtertySuffix);
            _stringBuilder.Append('\n');
            _stringBuilder.AppendLine("    public override void OnInit()    ");
            _stringBuilder.AppendLine("    {");
            //绑定UI控件
            WriteUIControllerBand(UIRoot.GetChild(i), _stringBuilder, CsharpFilePath, false);
            _stringBuilder.Append('\n');
            _stringBuilder.AppendLine("    }");
            _stringBuilder.Append('\n');
            _stringBuilder.AppendLine("    public override void OnShow(object objParm = null)    ");
            _stringBuilder.AppendLine("    {\n\t\t");
            _stringBuilder.AppendLine("    }");
            _stringBuilder.Append('\n');
            _stringBuilder.AppendLine("    public override void OnHide()    ");
            _stringBuilder.AppendLine("    {\n\t\t");
            _stringBuilder.AppendLine("    }");
            _stringBuilder.AppendLine();
            //写入UI事件
            WriteUIControllerEven(UIRoot.GetChild(i), _stringBuilder, CsharpFilePath, false);
            _stringBuilder.AppendLine("}");

            using (FileStream fileStream = new FileStream(CsharpFilePath, FileMode.Create, FileAccess.Write))
            {
                using (TextWriter textWriter = new StreamWriter(fileStream, Encoding.UTF8))
                {
                    textWriter.Write(_stringBuilder);
                    textWriter.Flush();
                }
            }
        }
    }

    /// <summary>
    /// 写入UIID文件
    /// </summary>
    /// <param name="UIRoot"></param>
    public static void WriteUIID(Transform UIRoot)
    {
        string UiidFilePath = Application.dataPath + "/XGameFrame/Scripts/UIFramWork" + "/UIDefine.cs";
        if (File.Exists(UiidFilePath))
        {
            //读取UIID文件后整个截断，取头和尾，中间内容根据UIRoot下的层级重写，
            //（强迫症，一定要一一对应，所以每次新生成一个文件就要点一下挂脚本）
            string fileText = File.ReadAllText(UiidFilePath);

            string[] FileTextSpArray = fileText.Split(new String[] { "UiId", "Null" }, StringSplitOptions.RemoveEmptyEntries);
            if (FileTextSpArray.Length < 3)
            {
                AssetDatabase.Refresh();
                return;
            }
            string nameValues = string.Empty;
            for (int i = 0; i < UIRoot.childCount; i++)
            {
                if (!UIRoot.GetChild(i).name.Equals("Other"))
                {
                    nameValues +="    "+ UIRoot.GetChild(i).name + ",";
                    if (i != UIRoot.childCount - 1)
                    {
                        nameValues += "\n";
                    }
                }
            }
            
            StringBuilder _stringBuilder=new StringBuilder();
            _stringBuilder.Append(FileTextSpArray[0]);
            _stringBuilder.Append("UiId\n");
            _stringBuilder.Append("{\n    Null,\n");
            _stringBuilder.Append(nameValues+"\n");
            _stringBuilder.Append("}");
            using (FileStream fileStream = new FileStream(UiidFilePath, FileMode.Create, FileAccess.Write))
            {
                using (TextWriter textWriter = new StreamWriter(fileStream, Encoding.UTF8))
                {
                    textWriter.Write(_stringBuilder);
                    textWriter.Flush();
                }
            }
        }
        AssetDatabase.Refresh();
    }

    /// <summary>
    /// 给对应的UI面板添加脚本并且为组件赋值
    /// </summary>
    /// <param name="UIRoot"></param>
    public static void AddComponentToUI(Transform UIRoot)
    {
        Type type = null;
        //移除多余不需要的脚本,避免误操作
        for (int i = 0; i < UIRoot.childCount; i++)
        {
            string SelectName = UIRoot.GetChild(i).name;
            if (SelectName.Equals("Other"))
            {
                continue;
            }
            UIBase[] components = UIRoot.GetChild(i).GetComponents<UIBase>();
            

            //给写入UIID的物体添加对应的脚本
            if (UIRoot.GetChild(i).name.Equals(SelectName))
            {
                type = GetTypeByName(SelectName);
                if (type != null)
                {
                    if (!UIRoot.GetChild(i).gameObject.GetComponent(type))
                    {
                        Undo.AddComponent(UIRoot.GetChild(i).gameObject, type);
                    }
                }
            }

            //判定是否添加上对应的脚本
            components = UIRoot.GetChild(i).GetComponents<UIBase>();
            if (components.Length == 0)
            {
                Debug.LogError(string.Format("添加{0}脚本失败，请先生成文件", SelectName));
            }
        }

        //通过反射设置对应脚本的ID和UI控件的值
        for (int i = 0; i < UIRoot.childCount; i++)
        {
            string SelectName = UIRoot.GetChild(i).name;
            type = GetTypeByName(SelectName);
            if (type != null)
            {
                if (UIRoot.GetChild(i).GetComponent(type))
                {
                    //设置UUID
                    var Field = type.GetField("UiId");
                    if (Field != null)
                    {
                        UiId _uiId = (UiId) Enum.Parse(typeof(UiId), SelectName);
                        Undo.RecordObject(UIRoot.GetChild(i).GetComponent(type), "Record UIID");
                        Field.SetValue(UIRoot.GetChild(i).GetComponent(type), _uiId);
                    }
                    //UI控件赋值
                    Transform[] AllChild = UIRoot.GetChild(i).GetComponentsInChildren<Transform>();
                    for (int j = 0; j < AllChild.Length; j++)
                    {
                        string[] NameAry = AllChild[j].name.Trim().Split('_');
                        if (NameAry.Length > 1)
                        {
                            MemberInfo[] memberInfos =
                                type.GetMembers(BindingFlags.Instance | BindingFlags.NonPublic);
                           
                            for (int k = 0; k < memberInfos.Length; k++)
                            {
                                if (AllChild[j].name.Equals(memberInfos[k].Name))
                                {
                                    var fieldInfo = type.GetField(AllChild[j].name, BindingFlags.Instance| BindingFlags.NonPublic);
                                    if (fieldInfo != null)
                                    {
                                        fieldInfo.SetValue(UIRoot.GetChild(i).GetComponent(type),
                                            AllChild[j].GetComponent(fieldInfo.FieldType));
                                    }
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    
    /// <summary>
    /// 读取UI控件并写入文件
    /// </summary>
    private static void WriteUIControlProperty(Transform PanelRoot, StringBuilder _stringBuilder,string FilePath,bool IsRewrite)
    {
        //如果是重写的，则需要截取#region这一段并重写，这样不会影响其他内容
        UIInstanceProperty _uiInstanceProperty = GetUIInstanceEditor();
        string[] SpliteProperAry=null;
        if (IsRewrite)
        {
            string fileContext = File.ReadAllText(FilePath, Encoding.UTF8);
            SpliteProperAry = fileContext.Split(new string[] { UIProtertyPrefix, UIProtertySuffix }, StringSplitOptions.RemoveEmptyEntries);
            if (SpliteProperAry.Length > 2)//如果分割成功
            {
                _stringBuilder.Append(SpliteProperAry[0]+ UIProtertyPrefix);
            }
        }
        else
        {
            _stringBuilder.Append("\n");
        }
        RectTransform[] ChildAry = PanelRoot.GetComponentsInChildren<RectTransform>();
        string AddedValue = string.Empty;
        for (int i = 0; i < ChildAry.Length; i++)
        {
            string[] NameAry = ChildAry[i].name.Trim().Split('_');
            string ChildName = ChildAry[i].name.Trim();
            if (NameAry.Length > 1 )
            {
                Type UiControllerType = GetTypeByName(NameAry[1].Trim());
                if (UiControllerType != null && UiControllerType.IsClass && IsUIContorllerClass(UiControllerType)!=-1)
                {
                    //记录UI控件InstanceID
                    if (IsRewrite)
                    {
                        //如果该UI控件没有记录
                        if (!IsHaveItem(_uiInstanceProperty, PanelRoot.name+ChildName))
                        {
                            //添加UI控件属性
                            AddedValue += string.Format("    [SerializeField] private {0} {1};\n", NameAry[1].Trim(),ChildName);
                            //插入绑定UI事件  插入写入UI事件
                            string bangValue=String.Empty;
                            string evenValue = String.Empty;
                            if (IsUIContorllerClass(NameAry[1].Trim())==1)
                            {
                                int bandIndx = SpliteProperAry[2].IndexOf(UIBandSuffix) - Endregion.Length - 1;
                                bangValue = ChildName + ".onClick.AddListener(" + "On" + ChildName + "_Click);\n        ";
                                SpliteProperAry[2] = SpliteProperAry[2].Insert(bandIndx, bangValue);

                                evenValue = "private void On" + ChildName + "_Click()\n" + "    {\n" + "        \n    }\n    ";
                                int evenIndx = SpliteProperAry[2].IndexOf(UIEvenSuffix) - Endregion.Length - 1;
                                SpliteProperAry[2] = SpliteProperAry[2].Insert(evenIndx, evenValue);
                            }
                            else if (IsUIContorllerClass(NameAry[1].Trim()) == 2)
                            {
                                int bandIndx = SpliteProperAry[2].IndexOf(UIBandSuffix) - Endregion.Length - 1;
                                bangValue = ChildName + ".onValueChanged.AddListener(" + "On" + ChildName + "_Click);\n        ";
                                SpliteProperAry[2] = SpliteProperAry[2].Insert(bandIndx, bangValue);

                                int evenIndx = SpliteProperAry[2].IndexOf(UIEvenSuffix) - Endregion.Length - 1;
                                evenValue = "private void On" + ChildName + "_Click(bool IsOn)\n" + "    {\n" + "        \n    }\n    ";
                                SpliteProperAry[2] = SpliteProperAry[2].Insert(evenIndx, evenValue);
                            }

                            //记录UI控件InstanceID
                            _uiInstanceProperty.ItemList.Add(PanelRoot.name + ChildName);
                            
                        }
                    }
                    else
                    {
                        _stringBuilder.AppendLine(string.Format("    [SerializeField] private {0} {1};", NameAry[1].Trim(), ChildName));
                        //记录UI控件InstanceID
                        _uiInstanceProperty.ItemList.Add(PanelRoot.name + ChildName);
                    }
                    SerializHelp.SerializeFile(_uiInstanceProperty, EditorConfigFileName);
                }
            }
        }
       
        
        if (IsRewrite)
        {
            if (SpliteProperAry.Length > 2)
            {
                _stringBuilder.Append(SpliteProperAry[1]);
                if (!string.IsNullOrEmpty(AddedValue))
                {
                    _stringBuilder.Insert(_stringBuilder.Length - Endregion.Length-5, AddedValue+"\n");
                }
               
                _stringBuilder.Append(UIProtertySuffix);
                _stringBuilder.Append(SpliteProperAry[2]);
                
                using (FileStream fileStream = new FileStream(FilePath, FileMode.Create, FileAccess.Write))
                {
                    using (TextWriter textWriter = new StreamWriter(fileStream, Encoding.UTF8))
                    {
                        textWriter.Write(_stringBuilder);
                        textWriter.Flush();
                    }
                }
            }
        }
    }
    /// <summary>
    /// 绑定UI
    /// </summary>
    /// <param name="PanelRoot"></param>
    /// <param name="_stringBuilder"></param>
    /// <param name="FilePath"></param>
    /// <param name="IsRewrite"></param>
    private static void WriteUIControllerBand(Transform PanelRoot, StringBuilder _stringBuilder, string FilePath, bool IsRewrite)
    {
        _stringBuilder.Append("        "+Region+" " + UIBandWarningText +" "+ UIBandPrefix+"\n" );
        RectTransform[] ChildAry = PanelRoot.GetComponentsInChildren<RectTransform>();
        for (int i = 0; i < ChildAry.Length; i++)
        {
            string[] NameAry = ChildAry[i].name.Trim().Split('_');

            if (NameAry.Length > 1)
            {
                Type UiControllerType = GetTypeByName(NameAry[1].Trim());
                if (UiControllerType != null && UiControllerType.IsClass)
                {
                    string ChildNameTrim = ChildAry[i].name.Trim();
                    if (IsUIContorllerClass(UiControllerType) == 1)
                    {
                        _stringBuilder.AppendLine("        " + ChildNameTrim + ".onClick.AddListener(" + "On" + ChildNameTrim + "_Click);");
                    }
                    else if (IsUIContorllerClass(UiControllerType) == 2)
                    {
                        _stringBuilder.AppendLine("        " + ChildNameTrim + ".onValueChanged.AddListener(" + "On" +
                                                  ChildNameTrim + "_Click);");
                    }
                }
            }
        }
        _stringBuilder.Append("        " + Endregion+" "+ UIBandSuffix);
    }
    /// <summary>
    /// 写入UI点击事件
    /// </summary>
    /// <param name="PanelRoot"></param>
    /// <param name="_stringBuilder"></param>
    /// <param name="FilePath"></param>
    /// <param name="IsRewrite"></param>
    private static void WriteUIControllerEven(Transform PanelRoot, StringBuilder _stringBuilder, string FilePath, bool IsRewrite)
    {
        _stringBuilder.Append("    " + Region+" " +UIEvenWarningText + " " + UIEvenPrefix+"\n");
        RectTransform[] ChildAry = PanelRoot.GetComponentsInChildren<RectTransform>();
        for (int i = 0; i < ChildAry.Length; i++)
        {
            string[] NameAry = ChildAry[i].name.Trim().Split('_');

            if (NameAry.Length > 1)
            {
                Type UiControllerType = GetTypeByName(NameAry[1].Trim());
                if (UiControllerType != null && UiControllerType.IsClass)
                {
                    if (IsUIContorllerClass(UiControllerType) == 1)
                    {
                        _stringBuilder.AppendLine("    private void On" + ChildAry[i].name.Trim() + "_Click()");
                        _stringBuilder.AppendLine("    {\n");
                        _stringBuilder.AppendLine("    }");
                    }
                    else if (IsUIContorllerClass(UiControllerType) == 2)
                    {
                        _stringBuilder.AppendLine("    private void On" + ChildAry[i].name.Trim() + "_Click(bool IsOn)");
                        _stringBuilder.AppendLine("    {\n");
                        _stringBuilder.AppendLine("    }");
                    }
                }
            }
        }
        _stringBuilder.Append("    "+Endregion+" " + UIEvenSuffix+"\n");
    }


    /// <summary>
    /// 检查是否是指定的UI类型
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private static int IsUIContorllerClass(Type type)
    {
        if (type.ToString().Contains("Button"))
        {
            return 1;
        }
        else if (type.ToString().Contains("Toggle"))
        {
            return 2;
        }
        else if(
            type.ToString().Contains("Image") ||
            type.ToString().Contains("Text") ||
            type.ToString().Contains("Slider") ||
            type.ToString().Contains("Scrollbar") ||
            type.ToString().Contains("ScrollRect") ||
            type.ToString().Contains("RawImage")
            )
        {
            return 3;
        }
        return -1;
    }
    /// <summary>
    /// 检查是否是指定的UI类型
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private static int IsUIContorllerClass(string type)
    {
        if (type.Contains("Button"))
        {
            return 1;
        }
        else if (type.Contains("Toggle"))
        {
            return 2;
        }
        else if (  type.Contains("Image") ||
                    type.Contains("Text") ||
                  type.Contains("Slider") ||
                  type.Contains("Scrollbar") ||
                  type.Contains("ScrollRect") ||
                  type.Contains("RawImage")
                )
        {
            return 3;
        }
        return -1;
    }

    /// <summary>
    /// 通过类名找到对应的类
    /// </summary>
    /// <param name="typeName"></param>
    /// <returns></returns>
    public static Type GetTypeByName(string typeName)
    {
        Type type = null;
        Assembly[] assemblyArray = AppDomain.CurrentDomain.GetAssemblies();
        int assemblyArrayLength = assemblyArray.Length;
        for (int i = 0; i < assemblyArrayLength; ++i)
        {
            type = assemblyArray[i].GetType(typeName);
            if (type != null)
            {
                return type;
            }
        }

        for (int i = 0; (i < assemblyArrayLength); ++i)
        {
            Type[] typeArray = assemblyArray[i].GetTypes();
            int typeArrayLength = typeArray.Length;
            for (int j = 0; j < typeArrayLength; ++j)
            {
                if (typeArray[j].Name.Equals(typeName))
                {
                    return typeArray[j];
                }
            }
        }
        return type;
    }

    private static UIInstanceProperty GetUIInstanceEditor()
    {
        if (File.Exists(EditorConfigFileName))
        {
            return SerializHelp.DeserializeFileToObj<UIInstanceProperty>(EditorConfigFileName);
        }
        else
        {
            UIInstanceProperty uiInstance = new UIInstanceProperty();
            uiInstance.ItemList =new List<string>();
            SerializHelp.SerializeFile(uiInstance, EditorConfigFileName);
            return uiInstance;
        }
    }

    private static bool IsHaveItem(UIInstanceProperty uiInstance, string Name)
    {
        for (int i = 0; i < uiInstance.ItemList.Count; i++)
        {
            if (uiInstance.ItemList[i].Equals(Name))
            {
                return true;
            }
        }
        return false;
    }

}