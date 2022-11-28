using System;
using Quartz;

namespace QuartzNet.Sample.MoreTask
{
    /// <summary>
    /// 任务1
    /// </summary>
    public class Task1 : IJobBase
    {
        public int Seconds { get; set; } = 5;

        public System.Threading.Tasks.Task Execute(IJobExecutionContext context)
        {
            return Console.Out.WriteLineAsync($"这是任务1，执行时间：{DateTime.Now}");
        }
    }
    /// <summary>
    /// 任务2
    /// </summary>
    public class Task2 : IJobBase
    {
        public int Seconds { get; set; } = 10;

        public System.Threading.Tasks.Task Execute(IJobExecutionContext context)
        {
            return Console.Out.WriteLineAsync($"这是任务2，执行时间：{DateTime.Now}");
        }
    }
}
