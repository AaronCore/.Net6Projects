using System;
using Quartz;

namespace QuartzNet.Sample.MoreTask
{
    /// <summary>
    /// 类明细
    /// </summary>
    public class ClassDetail
    {
        public Type TaskType { get; set; }
        public int Seconds { get; set; }
    }
    /// <summary>
    /// 任务明细
    /// </summary>
    public class TaskDetail
    {
        public IJobDetail Job { get; set; }
        public string Key { get; set; }
        public int Seconds { get; set; }
    }
}
