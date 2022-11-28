using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Quartz;

namespace QuartzNet.Sample.MoreTask
{
    /// <summary>
    /// 任务集合
    /// </summary>
    public class TaskCollections
    {
        /// <summary>
        /// 获取执行的任务集合
        /// </summary>
        /// <returns></returns>
        public static List<TaskDetail> GetJobs()
        {
            var list = new List<TaskDetail>();
            var types = GetIJobTypes();
            if (types.Count <= 0) return list;
            for (var i = 0; i < types.Count; i++)
            {
                var item = types[i];
                var key = "Job" + i;
                var job = JobBuilder.Create(item.TaskType).WithIdentity("Job" + i).Build();

                var task = new TaskDetail
                {
                    Job = job,
                    Key = key,
                    Seconds = item.Seconds
                };
                list.Add(task);
            }
            return list;
        }

        /// <summary>
        /// 获取所有继承IJob的类
        /// </summary>
        /// <returns></returns>
        public static List<ClassDetail> GetIJobTypes()
        {
            var res = new List<ClassDetail>();
            //根据反射获取所有继承了IJobBase接口的类
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes().Where(t => t.GetInterfaces().Contains(typeof(IJobBase))))
                .ToArray();
            if (types.Length <= 0) return res;
            {
                foreach (var t in types)
                {
                    //类对象
                    var obj = GetClassObj(t.Assembly, t.FullName);
                    //获取指定名称的属性，执行间隔时间
                    var propertyInfo = t.GetProperty("Seconds");
                    //获取属性值
                    if (propertyInfo == null) continue;
                    var value = (int)propertyInfo.GetValue(obj, null);
                    var entity = new ClassDetail
                    {
                        TaskType = t,
                        Seconds = value
                    };
                    res.Add(entity);
                }
            }
            return res;
        }

        /// <summary>
        /// 获取类对象
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="className"></param>
        /// <returns></returns>
        public static object GetClassObj(Assembly assembly, string className)
        {
            //从程序集中获取指定对象类型;
            var type = assembly.GetType(className);
            var obj = type.Assembly.CreateInstance(type.ToString());
            return obj;
        }
    }
}
