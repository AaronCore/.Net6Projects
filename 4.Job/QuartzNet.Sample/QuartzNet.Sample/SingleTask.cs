using System;
using System.Threading.Tasks;
using Quartz;
using Quartz.Impl;

namespace QuartzNet.Sample
{
    /// <summary>
    /// 单任务
    /// </summary>
    public class SingleTask
    {
        public static async Task RunTask()
        {
            // 1.创建scheduler的引用
            ISchedulerFactory chedulerFactory = new StdSchedulerFactory();
            IScheduler scheduler = await chedulerFactory.GetScheduler();

            //2.启动scheduler
            await scheduler.Start();

            //3.创建job
            IJobDetail job = JobBuilder.Create<WxhJob>()
                .WithIdentity("Wxh.Test.Job", "Wxh.Test.Group")
                .Build();

            //4.创建trigger(创建trigger触发器)
            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("Wxh.Test.Trigger", "Wxh.Test.Group")//触发器 组
                .WithSimpleSchedule(x => x.WithIntervalInMinutes(1).RepeatForever())
                .Build();

            //5.使用trigger规划执行任务job(使用触发器规划执行任务)
            await scheduler.ScheduleJob(job, trigger);

            //await Task.Delay(TimeSpan.FromSeconds(60));

            //await scheduler.Shutdown();
        }

        /// <summary>
        /// 实现IJob,Execute方法里编写要处理的业务逻辑
        /// </summary>
        public class WxhJob : IJob
        {
            public async Task Execute(IJobExecutionContext context)
            {
                await Console.Out.WriteLineAsync($"当前时间：{DateTime.Now}.");
            }
        }
    }
}
