using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdCreator.Sample
{
    /// <summary>
    /// 64位ID生成器,最高位为符号位,始终为0,可用位数63.
    /// 实例编号占10位,范围为0-1023
    /// 时间戳和索引共占53位
    /// </summary>
    public sealed class IdCreator
    {
        private static readonly Random r = new Random();
        private static readonly IdCreator _default = new IdCreator();

        private readonly long instanceID;//实例编号
        private readonly int indexBitLength;//索引可用位数
        private readonly long tsMax = 0;//时间戳最大值
        private readonly long indexMax = 0;
        private readonly object m_lock = new object();

        private long timestamp = 0;//当前时间戳
        private long index = 0;//索引/计数器

        /// <summary>
        ///
        /// </summary>
        /// <param name="instanceID">实例编号(0-1023)</param>
        /// <param name="indexBitLength">索引可用位数(1-32).每秒可生成ID数等于2的indexBitLength次方.大并发情况下,当前秒内ID数达到最大值时,将使用下一秒的时间戳,不影响获取ID.</param>
        /// <param name="initTimestamp">初始化时间戳,精确到秒.当之前同一实例生成ID的timestamp值大于当前时间的时间戳时,
        /// 有可能会产生重复ID(如持续一段时间的大并发请求).设置initTimestamp比最后的时间戳大一些,可避免这种问题</param>
        public IdCreator(int instanceID, int indexBitLength, long? initTimestamp = null)
        {
            if (instanceID < 0)
            {
                //这里给每个实例随机生成个实例编号
                this.instanceID = r.Next(0, 1024);
            }
            else
            {
                this.instanceID = instanceID % 1024;
            }

            if (indexBitLength < 1)
            {
                this.indexBitLength = 1;
            }
            else if (indexBitLength > 32)
            {
                this.indexBitLength = 32;
            }
            else
            {
                this.indexBitLength = indexBitLength;
            }
            tsMax = Convert.ToInt64(new string('1', 53 - indexBitLength), 2);
            indexMax = Convert.ToInt64(new string('1', indexBitLength), 2);

            if (initTimestamp != null)
            {
                this.timestamp = initTimestamp.Value;
            }
        }

        /// <summary>
        /// 默认每实例每秒生成65536个ID,从1970年1月1日起,累计可使用4358年
        /// </summary>
        /// <param name="instanceID">实例编号(0-1023)</param>
        public IdCreator(int instanceID) : this(instanceID, 16)
        {

        }

        /// <summary>
        /// 默认每秒生成65536个ID,从1970年1月1日起,累计可使用4358年
        /// </summary>
        public IdCreator() : this(-1)
        {

        }

        /// <summary>
        /// 生成64位ID
        /// </summary>
        /// <returns></returns>
        public long Create()
        {
            long id = 0;

            lock (m_lock)
            {
                //增加时间戳部分
                long ts = GetTimeStamp();

                ts = ts % tsMax;  //如果超过时间戳允许的最大值,从0开始
                id = ts << (10 + indexBitLength);//腾出后面部分,给实例编号和索引编号使用

                //增加实例部分
                id = id | (instanceID << indexBitLength);

                //获取计数
                if (timestamp < ts)
                {
                    timestamp = ts;
                    index = 0;
                }
                else
                {
                    if (index > indexMax)
                    {
                        timestamp++;
                        index = 0;
                    }
                }

                id = id | index;

                index++;
            }

            return id;
        }

        /// <summary>
        /// 获取当前实例的时间戳
        /// </summary>
        public long CurrentTimestamp
        {
            get
            {
                return this.timestamp;
            }
        }

        /// <summary>
        /// 默认每实例每秒生成65536个ID,从1970年1月1日起,累计可使用4358年
        /// </summary>
        public static IdCreator Default
        {
            get
            {
                return _default;
            }
        }

        /// <summary>
        /// 获取时间戳
        /// </summary>
        /// <returns></returns>
        public static long GetTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds);
        }
    }
}
