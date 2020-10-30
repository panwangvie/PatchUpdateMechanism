using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MathHotfix
{
    public class ParseJson
    {
        /// <summary>
        /// String 转josn
        /// </summary>
        /// <param name="json"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object DeserializeObject(string json, Type type)
        {
            if (string.IsNullOrEmpty(json))
            {
                return null;
            }
            object result;
            try
            {
                object obj = Activator.CreateInstance(type);
                Dictionary<string, string> dictionary = new Dictionary<string, string>();
                int num = 0;
                string text = "";
                char[] array = json.ToCharArray();
                for (int i = 0; i < array.Length; i++)
                {
                    if (string.IsNullOrEmpty(text) && array[i] == '"' && array[i - 1] != '\\')
                    {
                        if (num > 0)
                        {
                            int num2 = i;
                            text = json.Substring(num + 1, num2 - num - 1);
                            num = 0;
                            if (array[num2 + 2] != '"')
                            {
                                for (int j = num2 + 2; j < array.Length; j++)
                                {
                                    if (array[j] == ',')
                                    {
                                        string value = json.Substring(num2 + 2, j - num2 - 2);
                                        dictionary.Add(text, value);
                                        i = j;
                                        num = 0;
                                        text = string.Empty;
                                        value = string.Empty;
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                for (int k = num2 + 2; k < array.Length; k++)
                                {
                                    if (num > 0)
                                    {
                                        if (array[k] == '"' && array[k] != '\\')
                                        {
                                            string value = json.Substring(num2 + 3, k - num2 - 3);
                                            dictionary.Add(text, value);
                                            i = k + 1;
                                            num = 0;
                                            text = string.Empty;
                                            value = string.Empty;
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        num = k;
                                    }
                                }
                            }
                        }
                        else
                        {
                            num = i;
                        }
                    }
                }
                foreach (string text2 in dictionary.Keys)
                {
                    FieldInfo field = type.GetField(text2);
                    if (field != null)
                    {
                        if (field.FieldType == typeof(int))
                        {
                            field.SetValue(obj, int.Parse(dictionary[text2]));
                        }
                        else if (field.FieldType == typeof(uint))
                        {
                            field.SetValue(obj, uint.Parse(dictionary[text2]));
                        }
                        else if (field.FieldType == typeof(bool))
                        {
                            field.SetValue(obj, bool.Parse(dictionary[text2]));
                        }
                        else
                        {
                            field.SetValue(obj, dictionary[text2]);
                        }
                    }
                }
                result = obj;
            }
            catch
            {
                result = null;
            }
            return result;
        }
    }
}
