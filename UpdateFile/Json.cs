using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using UpdateFile.Log;

namespace UpdateFile
{
    /// <summary>
    /// 解析JSON，仿Javascript风格
    /// </summary>

    public class Json
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        public static T Parse<T>(string jsonString)
        {
            try
            {
                using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonString)))
                {
                    return (T)new DataContractJsonSerializer(typeof(T)).ReadObject(ms);
                }
            }catch(Exception ex)
            {
                Logs.WriteLog($"Message:{ex.Message},StackTrace:{ex.StackTrace}", PathConfig.UpdateLog);
                return default;
            }
        }

        /// <summary>
        /// 对象转json字符串
        /// </summary>
        /// <param name="jsonObject"></param>
        /// <returns></returns>
        public static string Stringify(object jsonObject)
        {
            try
            {
                using (var ms = new MemoryStream())
                {
                    new DataContractJsonSerializer(jsonObject.GetType()).WriteObject(ms, jsonObject);
                    return Encoding.UTF8.GetString(ms.ToArray());
                }
            }
            catch (Exception ex)
            {
                Logs.WriteLog($"Message:{ex.Message},StackTrace:{ex.StackTrace}", PathConfig.UpdateLog);
                return default;
            }
        }

    }
}
