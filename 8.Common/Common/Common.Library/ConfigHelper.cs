using System;
using System.Configuration;

namespace Common.Library
{
    /// <summary>
    /// 读取配置文件
    /// </summary>
    public static class ConfigHelper
    {
        /// <summary>
        /// 获取AppSettings配置
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetConfigAppSetting(string key)
        {
            try
            {
                return ConfigurationManager.AppSettings[key];
            }
            catch (Exception)
            { }
            return null;
        }

        /// <summary>
        /// 获取ConnectionStrings配置
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetConfigConnectionString(string key)
        {
            try
            {
                return ConfigurationManager.ConnectionStrings[key].ToString();
            }
            catch (Exception)
            { }
            return null;
        }
    }
}
