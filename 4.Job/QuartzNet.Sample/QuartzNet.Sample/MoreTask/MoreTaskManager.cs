using System.Collections.Generic;
using System.Collections.ObjectModel;
using Quartz;
using Quartz.Impl;

namespace QuartzNet.Sample.MoreTask
{
    /// <summary>
    /// 多任务
    /// </summary>
    public class MoreTaskManager
    {
        /// <summary>
        /// 任务调度的使用过程
        /// </summary>
        /// <returns></returns>
        public static async System.Threading.Tasks.Task Run()
        {
            // 创建scheduler的引用
            ISchedulerFactory schedulerFactory = new StdSchedulerFactory();
            IScheduler scheduler = await schedulerFactory.GetScheduler();

            //所有任务集合
            var jobs = TaskCollections.GetJobs();
            //声明一个任务与触发器映射的字典集合
            var jobAndTriggerMapping = new Dictionary<IJobDetail, IReadOnlyCollection<ITrigger>>();
            //遍历任务列表
            foreach (var job in jobs)
            {
                //生成只读的触发器集合
                var triggers = new ReadOnlyCollection<ITrigger>(
                    new List<ITrigger>()
                    {
                        TriggerBuilder.Create()
                            .WithIdentity("trigger_" + job.Key)
                            .WithSimpleSchedule(x => x.WithIntervalInSeconds(job.Seconds).RepeatForever())
                            .Build()
                    });

                jobAndTriggerMapping[job.Job] = triggers;
            }

            //将映射关系包装成制度字典集合
            var readOnlyJobAndTriggerMapping = new ReadOnlyDictionary<IJobDetail, IReadOnlyCollection<ITrigger>>(jobAndTriggerMapping);

            //使用trigger规划执行任务job,第二个参数replace：如果为true，则指定的触发器或者任务名称已经存在将会替换，否则将抛出异常
            await scheduler.ScheduleJobs(readOnlyJobAndTriggerMapping, true);

            //启动 scheduler
            await scheduler.Start();
        }
    }
}
