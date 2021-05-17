using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ZookeeperLocker.Sample
{
    /// <summary>
    /// 基于Zookeeper的分布式锁
    /// </summary>
    public class ZookeeperLocker : IDisposable
    {
        /// <summary>
        /// 单点锁
        /// </summary>
        static object locker = new object();
        /// <summary>
        /// Zookeeper集群地址
        /// </summary>
        string[] address;
        /// <summary>
        /// Zookeeper操作辅助类
        /// </summary>
        ZookeeperHelper zookeeperHelper;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="lockerPath">分布式锁的根路径</param>
        /// <param name="address">集群地址</param>
        public ZookeeperLocker(string lockerPath, params string[] address) : this(lockerPath, 0, address)
        {
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="lockerPath">分布式锁的根路径</param>
        /// <param name="sessionTimeout">回话过期时间</param>
        /// <param name="address">集群地址</param>
        public ZookeeperLocker(string lockerPath, int sessionTimeout, params string[] address)
        {
            this.address = address.ToArray();

            zookeeperHelper = new ZookeeperHelper(address, lockerPath);
            if (sessionTimeout > 0)
            {
                zookeeperHelper.SessionTimeout = sessionTimeout;
            }
            if (!zookeeperHelper.Connect())
            {
                throw new Exception("connect failed:" + string.Join(",", address));
            }
            lock (locker)
            {
                if (!zookeeperHelper.Exists())//根节点不存在则创建
                {
                    zookeeperHelper.SetData("", "", true);
                }
            }
        }
        /// <summary>
        /// 生成一个锁
        /// </summary>
        /// <returns>返回锁名</returns>
        public string CreateLock()
        {
            var path = Guid.NewGuid().ToString().Replace("-", "");
            while (zookeeperHelper.Exists(path))
            {
                path = Guid.NewGuid().ToString().Replace("-", "");
            }
            return CreateLock(path);
        }
        /// <summary>
        /// 使用指定的路径名称设置锁
        /// </summary>
        /// <param name="path">锁名，不能包含路径分隔符（/）</param>
        /// <returns>返回锁名</returns>
        public string CreateLock(string path)
        {
            if (path.Contains("/"))
            {
                throw new ArgumentException("invalid path");
            }
            return zookeeperHelper.SetData(path, "", false, true);
        }
        /// <summary>
        /// 获取锁
        /// </summary>
        /// <param name="path">锁名</param>
        /// <returns>如果获得锁返回true，否则一直等待</returns>
        public bool Lock(string path)
        {
            return LockAsync(path).GetAwaiter().GetResult();
        }
        /// <summary>
        /// 获取锁
        /// </summary>
        /// <param name="path">锁名</param>
        /// <param name="millisecondsTimeout">超时时间，单位：毫秒</param>
        /// <returns>如果获得锁返回true，否则等待指定时间后返回false</returns>
        public bool Lock(string path, int millisecondsTimeout)
        {
            return LockAsync(path, millisecondsTimeout).GetAwaiter().GetResult();
        }
        /// <summary>
        /// 异步获取锁等等
        /// </summary>
        /// <param name="path">锁名</param>
        /// <returns>如果获得锁返回true，否则一直等待</returns>
        public async Task<bool> LockAsync(string path)
        {
            return await LockAsync(path, System.Threading.Timeout.Infinite);
        }
        /// <summary>
        /// 异步获取锁等等
        /// </summary>
        /// <param name="path">锁名</param>
        /// <param name="millisecondsTimeout">超时时间，单位：毫秒</param>
        /// <returns>如果获得锁返回true，否则等待指定时间后返回false</returns>
        public async Task<bool> LockAsync(string path, int millisecondsTimeout)
        {
            var array = await zookeeperHelper.GetChildrenAsync("", true);
            if (array != null && array.Length > 0)
            {
                var first = array.FirstOrDefault();
                if (first == path)//正好是优先级最高的，则获得锁
                {
                    return true;
                }

                var index = array.ToList().IndexOf(path);
                if (index > 0)
                {
                    //否则添加监听
                    var are = new AutoResetEvent(false);
                    var watcher = new NodeWatcher();
                    watcher.NodeDeleted += (ze) =>
                    {
                        are.Set();
                    };
                    if (await zookeeperHelper.WatchAsync(array[index - 1], watcher))//监听顺序节点中的前一个节点
                    {
                        if (!are.WaitOne(millisecondsTimeout))
                        {
                            return false;
                        }
                    }

                    are.Dispose();
                }
                else
                {
                    throw new InvalidOperationException($"no locker found in path:{zookeeperHelper.CurrentPath}");
                }
            }
            return true;
        }
        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            zookeeperHelper.Dispose();
        }
    }
}
