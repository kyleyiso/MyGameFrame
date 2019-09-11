using UnityEngine;
using Excel;
using System.Data;
using System.IO;
using System.Text;
using System;

public class ExcleGenerator:MonoBehaviour
{
    public static bool Excel2Json(string _excelPath,string _csharpPath, string _jsonPath)
    {
        Encoding encoding = Encoding.UTF8;
        
        DirectoryInfo folder = new DirectoryInfo(_excelPath);
        FileSystemInfo[] files = folder.GetFileSystemInfos();
        int length = files.Length;
        
        for (int index = 0; index < length; index++)
        {
            if (files[index].Name.EndsWith(".xlsx"))
            {
                StringBuilder JsonBuilder=new StringBuilder();
                StringBuilder CSharpBuilder = new StringBuilder();
                string childPath = files[index].FullName;
                childPath = childPath.Replace('\\', '/');
                FileStream mStream;
                try
                {
                    mStream = File.Open(childPath, FileMode.Open, FileAccess.ReadWrite);
                }
                catch
                {
                    return false;
                }
                
                IExcelDataReader mExcelReader = ExcelReaderFactory.CreateOpenXmlReader(mStream);
               // mExcelReader.IsFirstRowAsColumnNames = false;
                DataSet mResultSet = mExcelReader.AsDataSet();
                
                if (mResultSet==null)
                {
                    return false;
                }
				//重新构建一个DataSet
				DataSet m_newDataSet = new DataSet();
				DataTable m_newTable = new DataTable();
				m_newDataSet.Tables.Add(m_newTable);

                //判断Excel文件中是否存在数据表
                if (mResultSet.Tables.Count < 1)
                    return false;



                //默认读取第一个数据表
                for (int i = 0; i < mResultSet.Tables.Count; i++)
                {
                    DataTable mSheet = mResultSet.Tables[i];

                    string fileName;
                    //如果Excle的sheet数只有一个
                    if (mResultSet.Tables.Count==1)
                    {
                        fileName = files[index].Name.Substring(0, files[index].Name.LastIndexOf('.'));
                    }
                    else if(mResultSet.Tables.Count>1 && !mSheet.TableName.ToLower().Contains("sheet"))//多个sheet且名字不为默认sheet
                    {
                        fileName = mSheet.TableName;
                    }
                    else
                    {
                        continue;
                    }

                    //新构建的DataSet设置table名字
                    m_newTable.TableName = fileName;

                    //判断数据表内是否存在数据
                    if (mSheet.Rows.Count < 1)
                        return false;

                    if (!string.IsNullOrEmpty(_jsonPath))
                    {
                        if (!WriteJson(mSheet, JsonBuilder))
                        {
                            return false;
                        }

                        using (FileStream fileStream = new FileStream(_jsonPath + "/" + fileName + "_Json" + ".json", FileMode.Create, FileAccess.Write))
                        {
                            using (TextWriter textWriter = new StreamWriter(fileStream, encoding))
                            {
                                textWriter.Write(JsonBuilder);
                              
                                textWriter.Close();
                                JsonBuilder = new StringBuilder();
                            }
                        }
                    }


                    if (!WriteCharpCode(mSheet, CSharpBuilder, fileName))
                    {
                        return false;
                    }
                    using (FileStream fileStream = new FileStream(_csharpPath + "/" + fileName + "_Data" + ".cs", FileMode.Create, FileAccess.Write))
                    {
                        using (TextWriter textWriter = new StreamWriter(fileStream, encoding))
                        {
                            Debug.Log(CSharpBuilder.ToString());
                            textWriter.Write(CSharpBuilder);
                            
                            textWriter.Close();
                            CSharpBuilder=new StringBuilder();
                        }
                    }
                }
            }
        }

        return true;
    }
    
    private static bool WriteJson(DataTable mSheet,StringBuilder stringBuilder)
    {
        int rowCount = mSheet.Rows.Count;
        int colCount = mSheet.Columns.Count;

        if (rowCount != 3)
        {
            stringBuilder.Append('[');
            stringBuilder.Append('\n');
        }
       
        for (int i = 2; i < rowCount; i++)
        {
            if (mSheet.Rows[i].IsNull(mSheet.Columns[0]))
            {
                stringBuilder.Remove(stringBuilder.Length - 2, 1);
                continue;
            }
            stringBuilder.Append('\t');
            stringBuilder.Append('{');
            for (int k = 0; k < colCount; k++)
            {
               
                if (k == 0)
                {
                    stringBuilder.Append('\n');
                }
                stringBuilder.Append('\t');
                stringBuilder.Append('\t');
                string temp = mSheet.Rows[1][k].ToString();//属性名字_类型
                string[] tempArry = temp.Split('_');

                string pName = tempArry[0];//属性名字
                string typeName = tempArry[1];//类型
                
                mSheet.Columns[k].ColumnName = pName;
                //stringBuilder.Append(pName);
                //需要什么类型自己扩展
                switch (typeName)
                {
                    case "i":
                    case "l":
                        stringBuilder.Append(String.Format("\"{0}\"", pName));
                        stringBuilder.Append(':');
                        stringBuilder.Append(mSheet.Rows[i][k]);
                        if (k != colCount - 1)
                        {
                            stringBuilder.Append(',');
                        }

                        stringBuilder.Append('\n');
                        break;
                    case "f":
                        stringBuilder.Append(String.Format("\"{0}\"", pName));
                        stringBuilder.Append(':');
                        string value = mSheet.Rows[i][k].ToString();
                        stringBuilder.Append(Math.Round(float.Parse(value),Excel2Json2Cs.Float_Accurate));
                        if (value.IndexOf('.') < 0) //判断是否有小数点,没有的话手动添加
                        {
                            stringBuilder.Append('.');
                            stringBuilder.Append('0');
                        }

                        if (k != colCount - 1)
                        {
                            stringBuilder.Append(',');
                        }

                        stringBuilder.Append('\n');
                        break;
                    case "s":
                        stringBuilder.Append(String.Format("\"{0}\"", pName));
                        stringBuilder.Append(':');
                        stringBuilder.Append(String.Format("\"{0}\"", mSheet.Rows[i][k]));
                        if (k != colCount - 1)
                        {
                            stringBuilder.Append(',');
                        }

                        stringBuilder.Append('\n');
                        break;
                    case "b":
                        stringBuilder.Append(String.Format("\"{0}\"", pName));
                        stringBuilder.Append(':');
                        bool b;
                        if (!bool.TryParse(mSheet.Rows[i][k].ToString().ToLower(), out b))
                        {
                            return false;
                        }
                        if (b)
                        {
                            stringBuilder.Append('t');
                            stringBuilder.Append('r');
                            stringBuilder.Append('u');
                            stringBuilder.Append('e');
                        }
                        else
                        {
                            stringBuilder.Append('f');
                            stringBuilder.Append('a');
                            stringBuilder.Append('l');
                            stringBuilder.Append('s');
                            stringBuilder.Append('e');
                        }

                        if (k != colCount - 1)
                        {
                            stringBuilder.Append(',');
                        }

                        stringBuilder.Append('\n');
                        break;
                    case "ari":
                        {
                            stringBuilder.Append(String.Format("\"{0}\"", pName));
                            stringBuilder.Append(':');
                            stringBuilder.Append('[');
                            string RemoveSpec = mSheet.Rows[i][k].ToString()
                                .Substring(1, mSheet.Rows[i][k].ToString().Length-2);
                            string[] arryValue = RemoveSpec.Split(new Char[] { ',', '，' }, StringSplitOptions.RemoveEmptyEntries);
                            for (int j = 0; j < arryValue.Length; j++)
                            {
                                int tempInt;
                                if (int.TryParse(arryValue[j], out tempInt))
                                {
                                    stringBuilder.Append(tempInt);
                                 }
                                else
                                {
                                    return false;
                                }
                                if (j != arryValue.Length - 1)
                                {
                                    stringBuilder.Append(',');
                                }
                            }

                            stringBuilder.Append(']');
                            if (k != colCount - 1)
                            {
                                stringBuilder.Append(',');
                            }

                            stringBuilder.Append('\n');
                        }
                        break;
                    case "arf":
                    {
                        stringBuilder.Append(String.Format("\"{0}\"", pName));
                        stringBuilder.Append(':');
                        stringBuilder.Append('[');
                        string RemoveSpec = mSheet.Rows[i][k].ToString()
                            .Substring(1, mSheet.Rows[i][k].ToString().Length - 2);

                            string[] arryValue = RemoveSpec.Split(new Char[] { ',', '，' }, StringSplitOptions.RemoveEmptyEntries);
                            for (int j = 0; j < arryValue.Length; j++)
                        {
                            float tempF;
                            if (!float.TryParse(arryValue[j], out tempF))
                            {
                                return false;
                            }
                            string[] valueLast = arryValue[j].Split('.');
                            int lastValue = int.Parse(valueLast[valueLast.Length - 1]);
                            stringBuilder.Append(Math.Round(tempF,Excel2Json2Cs.Float_Accurate));
                            if (arryValue[j].IndexOf('.') < 0 || lastValue==0) //判断是否有小数点,没有的话人工添加
                            {
                                stringBuilder.Append('.');
                                stringBuilder.Append('0');
                            }
                                
                            if (j != arryValue.Length - 1)
                            {
                                stringBuilder.Append(',');
                            }
                        }

                        stringBuilder.Append(']');
                        if (k != colCount - 1)
                        {
                            stringBuilder.Append(',');
                        }

                        stringBuilder.Append('\n');
                    }
                        break;
                    case "ars":
                        {
                            stringBuilder.Append(String.Format("\"{0}\"", pName));
                            stringBuilder.Append(':');
                            stringBuilder.Append('[');
                            string RemoveSpec = mSheet.Rows[i][k].ToString()
                                .Substring(1, mSheet.Rows[i][k].ToString().Length - 2);

                            string[] arryValue = RemoveSpec.Split(new Char[] { ',','，' }, StringSplitOptions.RemoveEmptyEntries);

                            for (int j = 0; j < arryValue.Length; j++)
                            {
                                string asrValue = String.Format("\"{0}\"", arryValue[j]);
                               
                                stringBuilder.Append(asrValue);
                                if (j != arryValue.Length - 1)
                                {
                                    stringBuilder.Append(',');
                                }
                            }
                            stringBuilder.Append(']');
                            if (k != colCount - 1)
                            {
                                stringBuilder.Append(',');
                            }
                            stringBuilder.Append('\n');
                        }
                        break;
                }

            }
            stringBuilder.Append('\t');
            stringBuilder.Append('}');
            if (i != rowCount - 1)
            {
                stringBuilder.Append(',');
            }
            stringBuilder.Append('\n');
        }

        if (rowCount != 3)
        {
            stringBuilder.Append(']');
        }
        return true;
    }
   
    private static bool WriteCharpCode(DataTable mSheet, StringBuilder stringBuilder, string ClassName)
    {
        int rowCount = mSheet.Rows.Count;

        int colCount = mSheet.Columns.Count;

        stringBuilder.AppendLine("//Generator by Tools");
        stringBuilder.AppendLine("//Editor by YS");
        stringBuilder.AppendLine("using System;");
        stringBuilder.AppendLine("using System.Collections.Generic;\n");
        stringBuilder.AppendLine("public class " + ClassName + "_Data");
        stringBuilder.AppendLine("{");

        for (int i = 0; i < colCount; i++)
        {
            string temp = mSheet.Rows[1][i].ToString();//属性名字_类型
            string[] tempArry = temp.Split('_');
            string pName = tempArry[0];//属性名字
            string typeName = tempArry[1];//类型
            mSheet.Columns[i].ColumnName = pName;

            string[] sv1 = mSheet.Rows[0][i].ToString().Split(new Char[] { '\t', ' ', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            string fin = string.Empty;
            for (int j = 0; j < sv1.Length; j++)
            {
                fin += sv1[j];
            }
            stringBuilder.AppendLine("\t/// <summary>");
            stringBuilder.AppendLine("\t/// " + fin);
            stringBuilder.AppendLine("\t/// </summary>");

            stringBuilder.Append('\t');
            stringBuilder.Append("public static ");
            switch (typeName)
            {
                case "i":
                    if (rowCount > 3)
                    {
                        stringBuilder.Append("int []");
                        stringBuilder.Append(pName);
                        stringBuilder.Append(" = new []{");
                        for (int j = 2; j < rowCount; j++)
                        {
                            if (mSheet.Rows[j].IsNull(i))
                            {
                                break;
                            }
                            stringBuilder.Append(mSheet.Rows[j][i]);
                            if (j != rowCount - 1)
                            {
                                stringBuilder.Append(',');
                            }
                        }
                    }
                    else
                    {
                        stringBuilder.Append("int ");
                        stringBuilder.Append(pName);
                        stringBuilder.Append(" = ");
                        stringBuilder.Append(mSheet.Rows[2][i]); 
                    }
                   
                    break;
                case "l":
                    if (rowCount > 3)
                    {
                        stringBuilder.Append("long []");
                        stringBuilder.Append(pName);
                        stringBuilder.Append(" = new []{");
                        for (int j = 2; j < rowCount; j++)
                        {
                            if (mSheet.Rows[j].IsNull(i))
                            {
                                break;
                            }
                            stringBuilder.Append(mSheet.Rows[j][i]+"L"); 
                            if (j != rowCount - 1)
                            {
                                stringBuilder.Append(',');
                            }
                        }
                    }
                    else
                    {
                        stringBuilder.Append("long ");
                        stringBuilder.Append(pName);
                        stringBuilder.Append(" = ");
                        stringBuilder.Append(mSheet.Rows[2][i]+"L");
                    }
                    break;
                case "f":
                    if (rowCount > 3)
                    {
                        stringBuilder.Append("float []");
                        stringBuilder.Append(pName);
                        stringBuilder.Append(" = new []{");
                        for (int j = 2; j < rowCount; j++)
                        {
                            if (mSheet.Rows[j].IsNull(i))
                            {
                                break;
                            }
                            stringBuilder.Append(Math.Round(float.Parse(mSheet.Rows[j][i].ToString()),Excel2Json2Cs.Float_Accurate) + "f");
                            if (j != rowCount - 1)
                            {
                                stringBuilder.Append(',');
                            }
                        }
                    }
                    else
                    {
                        stringBuilder.Append("float ");
                        stringBuilder.Append(pName);
                        stringBuilder.Append(" = ");
                        stringBuilder.Append(Math.Round(float.Parse(mSheet.Rows[2][i].ToString()), Excel2Json2Cs.Float_Accurate) + "f");
                    }

                    break;
                case "s":
                    if (rowCount > 3)
                    {
                        stringBuilder.Append("string []");
                        stringBuilder.Append(pName);
                        stringBuilder.Append(" = new []{");
                        for (int j = 2; j < rowCount; j++)
                        {
                            if (mSheet.Rows[j].IsNull(i))
                            {
                                break;
                            }

                            string tempS = string.Format("\"{0}\"", mSheet.Rows[j][i]);
                            stringBuilder.Append(tempS);
                            if (j != rowCount - 1)
                            {
                                stringBuilder.Append(',');
                            }
                        }
                    }
                    else
                    {
                        stringBuilder.Append("string ");
                        stringBuilder.Append(pName);
                        stringBuilder.Append(" = ");
                        stringBuilder.Append(string.Format("\"{0}\"", mSheet.Rows[2][i]));
                    }
                    break;
                case "b":
                    if (rowCount > 3)
                    {
                        stringBuilder.Append("bool []");
                        stringBuilder.Append(pName);
                        stringBuilder.Append(" = new []{");
                        for (int j = 2; j < rowCount; j++)
                        {
                            if (mSheet.Rows[j].IsNull(i))
                            {
                                break;
                            }
                            stringBuilder.Append(mSheet.Rows[j][i].ToString().ToLower());
                            if (j != rowCount - 1)
                            {
                                stringBuilder.Append(',');
                            }
                        }
                    }
                    else
                    {
                        stringBuilder.Append("string ");
                        stringBuilder.Append(pName);
                        stringBuilder.Append(" = ");
                        stringBuilder.Append(mSheet.Rows[2][i].ToString().ToLower());
                    }
                    break;
                case "ari":
                    if (rowCount > 3)
                    {
                        stringBuilder.Append("int [,]");
                        stringBuilder.Append(pName);
                        stringBuilder.Append(" = new int[,]{");
                        stringBuilder.Append('\n');
                        for (int j = 2; j < rowCount; j++)
                        {
                            if (mSheet.Rows[j].IsNull(i))
                            {
                                stringBuilder.Append(InsetSpace(pName, "int"));
                                break;
                            }

                            string s = mSheet.Rows[j][i].ToString()
                                .Substring(1, mSheet.Rows[j][i].ToString().Length - 2);
                            string[] sa = s.Split(new Char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                            stringBuilder.Append('\t');
                            stringBuilder.Append('\t');
                            stringBuilder.Append("{ ");
                            for (int k = 0; k < sa.Length; k++)
                            {
                                stringBuilder.Append(sa[k]);
                                if (k != sa.Length - 1)
                                    stringBuilder.Append(',');
                            }
                            stringBuilder.Append(" }");
                         
                            if (j != rowCount - 1)
                            {
                                stringBuilder.Append(',');
                                stringBuilder.Append('\n');
                            }
                            else
                            {
                                stringBuilder.Append('\n');
                                stringBuilder.Append(InsetSpace(pName, "int"));
                            }
                        }
                    }
                    else
                    {
                        stringBuilder.Append("int []");
                        stringBuilder.Append(pName);
                        stringBuilder.Append(" = new []{");
                        string s = mSheet.Rows[2][i].ToString()
                            .Substring(1, mSheet.Rows[2][i].ToString().Length - 2);
                        string[] sa = s.Split(new Char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        for (int k = 0; k < sa.Length; k++)
                        {
                            stringBuilder.Append(sa[k]);
                            if (k != sa.Length - 1)
                                stringBuilder.Append(',');
                        }
                        
                        stringBuilder.Append('}');
                    }

                    break;
                case "arl":
                    if (rowCount > 3)
                    {
                        stringBuilder.Append("long [,]");
                        stringBuilder.Append(pName);
                        stringBuilder.Append(" = new long[,]{");
                        stringBuilder.Append('\n');
                        for (int j = 2; j < rowCount; j++)
                        {
                            if (mSheet.Rows[j].IsNull(i))
                            {
                                stringBuilder.Append(InsetSpace(pName, "long"));
                                break;
                            }
                            string s = mSheet.Rows[j][i].ToString()
                                .Substring(1, mSheet.Rows[j][i].ToString().Length - 2);
                            string[] sa = s.Split(new Char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                            stringBuilder.Append('\t');
                            stringBuilder.Append('\t');
                            stringBuilder.Append("{ ");
                            for (int k = 0; k < sa.Length; k++)
                            {
                                stringBuilder.Append(sa[k]+"L");
                                if (k != sa.Length - 1)
                                    stringBuilder.Append(',');
                            }
                            stringBuilder.Append(" }");

                            if (j != rowCount - 1)
                            {
                                stringBuilder.Append(',');
                                stringBuilder.Append('\n');
                            }
                            else
                            {
                                stringBuilder.Append('\n');
                                stringBuilder.Append(InsetSpace(pName, "long"));
                            }
                        }
                    }
                    else
                    {
                        stringBuilder.Append("long []");
                        stringBuilder.Append(pName);
                        stringBuilder.Append(" = new []{");

                        string s = mSheet.Rows[2][i].ToString()
                            .Substring(1, mSheet.Rows[2][i].ToString().Length - 2);
                        string[] sa = s.Split(new Char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        for (int k = 0; k < sa.Length; k++)
                        {
                            stringBuilder.Append(sa[k]+"L");
                            if (k != sa.Length - 1)
                                stringBuilder.Append(',');
                        }

                        stringBuilder.Append('}');
                    }


                    break;
                case "arf":
                    if (rowCount > 3)
                    {
                        stringBuilder.Append("float [,]");
                        stringBuilder.Append(pName);
                        stringBuilder.Append(" = new float[,]{");
                        stringBuilder.Append('\n');
                        for (int j = 2; j < rowCount; j++)
                        {
                            string s = mSheet.Rows[j][i].ToString()
                                .Substring(1, mSheet.Rows[j][i].ToString().Length - 2);
                            string[] sa = s.Split(new Char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                            stringBuilder.Append('\t');
                            stringBuilder.Append('\t');
                            stringBuilder.Append("{ ");
                            for (int k = 0; k < sa.Length; k++)
                            {
                                if (mSheet.Rows[j].IsNull(i))
                                {
                                    stringBuilder.Append(InsetSpace(pName, "float"));
                                    break;
                                }
                                stringBuilder.Append(Math.Round(float.Parse(sa[k]), Excel2Json2Cs.Float_Accurate) + "f");
                                if (k != sa.Length - 1)
                                    stringBuilder.Append(',');
                            }

                            stringBuilder.Append(" }");

                            if (j != rowCount - 1)
                            {
                                stringBuilder.Append(',');
                                stringBuilder.Append('\n');
                            }
                            else
                            {
                                stringBuilder.Append('\n');
                                stringBuilder.Append(InsetSpace(pName, "float"));
                            }
                        }
                    }
                    else
                    {
                        stringBuilder.Append("float []");
                        stringBuilder.Append(pName);
                        stringBuilder.Append(" = new []{");

                        string s = mSheet.Rows[2][i].ToString()
                            .Substring(1, mSheet.Rows[2][i].ToString().Length - 2);
                        string[] sa = s.Split(new Char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        for (int k = 0; k < sa.Length; k++)
                        {
                            stringBuilder.Append(Math.Round(float.Parse(sa[k]), Excel2Json2Cs.Float_Accurate) + "f");
                            if (k != sa.Length - 1)
                                stringBuilder.Append(',');
                        }

                        stringBuilder.Append('}');
                    }

                    break;
                case "ars":
                    if (rowCount > 3)
                    {
                        stringBuilder.Append("string [,]");
                        stringBuilder.Append(pName);
                        stringBuilder.Append(" = new string[,]{");
                        stringBuilder.Append('\n');
                        for (int j = 2; j < rowCount; j++)
                        {
                            string s = mSheet.Rows[j][i].ToString()
                                .Substring(1, mSheet.Rows[j][i].ToString().Length - 2);
                            string[] sa = s.Split(new Char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                            stringBuilder.Append('\t');
                            stringBuilder.Append('\t');
                            stringBuilder.Append("{ ");
                            for (int k = 0; k < sa.Length; k++)
                            {
                                if (mSheet.Rows[j].IsNull(i))
                                {
                                    stringBuilder.Append(InsetSpace(pName, "string"));
                                    break;
                                }
                                stringBuilder.Append(String.Format("\"{0}\"", sa[k]));
                                if (k != sa.Length - 1)
                                    stringBuilder.Append(',');
                            }

                            stringBuilder.Append(" }");
                            if (j != rowCount - 1)
                            {
                                stringBuilder.Append(',');
                                stringBuilder.Append('\n');
                            }
                            else
                            {
                                stringBuilder.Append('\n');
                                stringBuilder.Append(InsetSpace(pName, "string"));
                            }
                        }
                    }
                    else
                    {
                        stringBuilder.Append("string []");
                        stringBuilder.Append(pName);
                        stringBuilder.Append(" = new []{");

                        string s = mSheet.Rows[2][i].ToString()
                            .Substring(1, mSheet.Rows[2][i].ToString().Length - 2);
                        string[] sa = s.Split(new Char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        for (int k = 0; k < sa.Length; k++)
                        {
                            stringBuilder.Append(String.Format("\"{0}\"", sa[k]));
                            if (k != sa.Length - 1)
                                stringBuilder.Append(',');
                        }

                        stringBuilder.Append('}');
                    }

                    break;
            }

            if (rowCount > 3)
            {
                stringBuilder.Append('}');
            }
            
            stringBuilder.Append(';');
            stringBuilder.Append('\n');
        }
        stringBuilder.Append('}');

        return true;
    }

    private static string InsetSpace(string PamLeng, string type)
    {
        int lenth = PamLeng.Length + type.Length + 36;
        string value="";
        for (int i = 0; i < lenth; i++)
        {
            value = value.Insert(value.Length, " ");
        }
        return value;
    }

}

