using System;
using Microsoft.Extensions.DependencyInjection;
using Hangfire;
using VueAdmin.Jobs.Jobs.Hangfire;

namespace VueAdmin.Jobs
{
    public static class VueAdminJobsExtensions
    {
        public static void UseHangfireTest(this IServiceProvider service)
        {
            var job = service.GetService<HangfireTestJob>();

            RecurringJob.AddOrUpdate("定时任务测试", () => job.ExecuteAsync(), CronType.Minute());
        }
    }
}
