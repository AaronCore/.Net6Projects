using System;
using System.Collections.Generic;

namespace IdCreator.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            var ids = new List<long>();
            var idwork = new SnowflakeId(15, 3);
            for (var i = 0; i < 100; i++)
            {
                var idTest = idwork.NextId();
                ids.Add(idTest);
            }
            foreach (var id in ids)
            {
                var idStr = SnowflakeId.AnalyzeId(id);
                Console.WriteLine("雪花ID：" + id + "\t\b解析-> " + idStr);
            }

            //var watch = Stopwatch.StartNew();
            //System.Threading.Tasks.Parallel.For(0, 8, i =>
            //{
            //    for (var j = 0; j < 100000; j++)
            //    {
            //        var id = idwork.NextId();
            //        lock (ids)
            //        {
            //            ids.Add(id);
            //        }
            //    }
            //});
            //Console.WriteLine(ids.Count + "个Id生成完毕，耗时：" + watch.ElapsedMilliseconds);

            ////return;
            //System.Threading.Tasks.Parallel.For(0, ids.Count, (i, state) =>
            //{
            //    for (var j = i + 1; j < ids.Count; j++)
            //    {
            //        if (ids[i] == ids[j])
            //        {
            //            Console.WriteLine("有重复项:" + ids[i]);
            //            state.Stop();
            //        }
            //    }
            //});
            //Console.WriteLine("重复项检查完毕，耗时：" + watch.ElapsedMilliseconds);
            //Console.WriteLine("fineshed");

            //IdCreator c = new IdCreator(0, 16);
            //var id = c.Create();

            //Console.WriteLine(id);
            //Console.ReadKey();
        }
    }
}
