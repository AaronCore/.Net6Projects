using System;
using Newtonsoft.Json;

namespace Common.Library
{
    /// <summary>
    /// Json序列化帮助类
    /// </summary>
    public static partial class JsonHelper
    {
        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static String ObjectToJsonString(this Object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        public static T JsonStringToObject<T>(this String jsonString)
        {
            return JsonConvert.DeserializeObject<T>(jsonString);
        }
    }
}
