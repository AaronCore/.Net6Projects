using Quartz;

namespace QuartzNet.Sample.MoreTask
{
    public interface IJobBase : IJob
    {
        /// <summary>
        /// 执行时间间隔(秒)
        /// </summary>
        int Seconds { get; set; }
    }
}
