using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Enyim.Caching;
using Enyim.Caching.Memcached;

namespace Memcached.Sample
{
    public class MemcachedHelper
    {
        private static MemcachedClient MemClient;
        static readonly object padlock = new object();

        //线程安全的单例模式
        public static MemcachedClient getInstance()
        {
            if (MemClient == null)
            {
                lock (padlock)
                {
                    if (MemClient == null)
                    {
                        MemClientInit();
                    }
                }
            }
            return MemClient;
        }

        static void MemClientInit()
        {
            try
            {
                MemClient = new MemcachedClient();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 插入指定值
        /// </summary>
        /// <param name="key">缓存名称 </param>
        /// <param name="value">缓存值</param>
        /// <param name="minutes">过期时间(分钟)，默认一个礼拜</param>
        /// <returns>返回是否成功</returns>
        public bool Insert(string key, string value, int minutes = 10080)
        {
            MemcachedClient mc = getInstance();
            var data = mc.Get(key);

            DateTime dateTime = DateTime.Now.AddMinutes(1);
            if (data == null)
                return mc.Store(StoreMode.Add, key, value, dateTime);
            else
                return mc.Store(StoreMode.Replace, key, value, dateTime);
        }

        /// <summary>
        /// 获取缓存值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object Get(string key)
        {
            MemcachedClient mc = getInstance();
            return mc.Get(key);
        }

        /// <summary>
        /// 删除指定缓存
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Remove(string key)
        {
            MemcachedClient mc = getInstance();
            return mc.Remove(key);
        }

        /// <summary>
        /// 清空缓存服务器上的缓存
        /// </summary>
        public void FlushCache()
        {
            MemcachedClient mc = getInstance();
            mc.FlushAll();
        }
    }
}
