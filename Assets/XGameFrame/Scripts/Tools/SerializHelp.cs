using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace XGameTools
{
    /// <summary>
    /// 序列化和反序列化
    /// </summary>
    public class SerializHelp
    {
        /// <summary>
        /// 将对象序列化成文件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="classObj">需要序列化的对象</param>
        /// <param name="filePath">保存的地址</param>
        public static void SerializeFile<T>(T classObj, string filePath) where T : class
        {
            try
            {
                using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Serialize(fileStream, classObj);
                    fileStream.Close();
                    fileStream.Dispose();
                }
            }
            catch (Exception)
            {

#if UNITY_EDITOR
                Debug.Log(string.Format("保存{0}数据失败", classObj));
#endif
            }
        }


        /// <summary>
        /// 将文件反序列化成对象
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="filePath">文件地址</param>
        /// <returns></returns>
        public static T DeserializeFileToObj<T>(string filePath) where T : class
        {
            try
            {
                using (FileStream fileStream = File.OpenRead(filePath))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    return bf.Deserialize(fileStream) as T;
                }

            }
            catch (Exception)
            {
#if UNITY_EDITOR
                Debug.Log(string.Format("读取{0}数据失败", typeof(T)));
#endif
                return default(T);
            }

        }
    }

}
