using System;
using System.Collections.Specialized;
using System.Threading.Tasks;
using Quartz;
using Quartz.Impl;
using Quartz.Logging;

namespace QuartzNet.Sample
{
    class Program
    {
        // https://www.quartz-scheduler.net/documentation/index.html
        // https://www.jianshu.com/c/f3cf4e34014c
        static void Main(string[] args)
        {
            Console.WriteLine("开始...");

            LogProvider.SetCurrentLogProvider(new ConsoleLogProvider());
            RunProgramRunExample().GetAwaiter().GetResult();

            Console.WriteLine("结束...");
            Console.ReadKey();
        }
        private static async Task RunProgramRunExample()
        {
            try
            {
                // 创建作业调度实例
                NameValueCollection props = new NameValueCollection
                {
                    { "quartz.serializer.type", "binary" }
                };
                StdSchedulerFactory factory = new StdSchedulerFactory(props);
                IScheduler scheduler = await factory.GetScheduler();

                // 调度开始
                await scheduler.Start();

                // 创建作业
                var jobDataMap = new JobDataMap();
                jobDataMap.Add("Name", "小伙子");

                IJobDetail job = JobBuilder.Create<HelloJob>()
                    .WithIdentity("job_1", "job_group")
                    .UsingJobData(jobDataMap)
                    .Build();

                // 创建触发器，每10秒执行一次
                ITrigger trigger = TriggerBuilder.Create()
                    .WithIdentity("trigger_1", "tri_group")
                    .StartNow()
                    .WithSimpleSchedule(x => x
                        .WithIntervalInSeconds(10)
                        .RepeatForever())
                    .Build();

                // 加入作业调度池
                await scheduler.ScheduleJob(job, trigger);

                // 休眠60s
                await Task.Delay(TimeSpan.FromSeconds(60));

                // 调度结束
                await scheduler.Shutdown();
            }
            catch (SchedulerException se)
            {
                Console.WriteLine(se);
            }
        }
        private class ConsoleLogProvider : ILogProvider
        {
            public Logger GetLogger(string name)
            {
                return (level, func, exception, parameters) =>
                {
                    if (level >= LogLevel.Info && func != null)
                    {
                        Console.WriteLine("[" + DateTime.Now.ToLongTimeString() + "] [" + level + "] " + func(), parameters);
                    }
                    return true;
                };
            }
            public IDisposable OpenNestedContext(string message)
            {
                throw new NotImplementedException();
            }

            public IDisposable OpenMappedContext(string key, object value, bool destructure = false)
            {
                throw new NotImplementedException();
            }
        }
    }
    public class HelloJob : IJob
    {
        public string Name { get; set; }
        public async Task Execute(IJobExecutionContext context)
        {
            //var name = context.JobDetail.JobDataMap["name"].ToString();
            await Console.Out.WriteLineAsync($"欢迎啊 {Name}....");
        }
    }
}
