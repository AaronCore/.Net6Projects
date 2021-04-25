using System;
using Polly;

namespace Polly.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                // 1.重试策略(Retry)
                //Policy.Handle<Exception>().Retry(3, (ex, index) =>
                //{
                //    Console.WriteLine(string.Format("执行失败，重试次数 {0}   异常来自：{1}", index, ex.GetType().Name));
                //}).Execute(() =>
                //{
                //    Print();
                //});

                // 2.回退(Fallback)
                //Policy.Handle<Exception>().Fallback(() =>
                //{
                //    Console.WriteLine("执行失败，返回Fallback");
                //}).Execute(() =>
                //{
                //    Print();
                //});

                // 3.断路保护(Circuit-breaker)
                //var policy = Policy.Handle<Exception>().CircuitBreaker(3, TimeSpan.FromSeconds(5));
                //while (true)
                //{
                //    Console.WriteLine("Start...");
                //    try
                //    {
                //        policy.Execute(() =>
                //        {
                //            Console.WriteLine("开始执行....");
                //            throw new Exception("出错了...");
                //            Console.WriteLine("完成执行....");
                //        });
                //    }
                //    catch (Exception ex)
                //    {
                //        Console.WriteLine("断路出错....");
                //    }
                //    Thread.Sleep(500);
                //}

                // 4.超时(Timeout) 一般搭配Wrap使用
                //var fallbackPolicy = Policy.Handle<TimeoutRejectedException>().Fallback(() =>
                //{
                //    Console.WriteLine("Fallback...");
                //});
                //var timeoutPolicy = Policy.Timeout(3, TimeoutStrategy.Pessimistic);
                //var wrapPolicy = Policy.Wrap(timeoutPolicy, fallbackPolicy);
                //wrapPolicy.Execute(() =>
                //{
                //    Console.WriteLine("开始任务...");
                //    Thread.Sleep(5000);
                //    Console.WriteLine("完成任务....");
                //});

                // 5.隔离(Bulkhead Isolation)
                //Policy.Bulkhead(12).Execute(() => { });

                // 6.策略包(Policy Wrap)
                var fallbackPolicy = Policy.Handle<Exception>().Fallback(() =>
                {
                    Console.WriteLine("Fallback...");
                });
                var retryPolicy = Policy.Handle<Exception>().Retry(3, (ex, index) =>
                {
                    Console.WriteLine(string.Format("执行失败，重试次数 {0}   异常来自：{1}", index, ex.GetType().Name));
                });
                var wrapPolicy = Policy.Wrap(fallbackPolicy, retryPolicy);
                wrapPolicy.Execute(() =>
                {
                    Print();
                });

                // 7.缓存(Cache)
                //var memoryCacheProvider = new MemoryCacheProvider(myMemoryCache);
                //var cachePolicy = Policy.Cache(memoryCacheProvider, TimeSpan.FromMinutes(5));
                //var result = cachePolicy.Execute(context => getFoo(), new Context("FooKey"));


            }
            catch (Exception ex)
            {
                Console.WriteLine("Main Exception");
            }
            Console.ReadKey();
        }
        public static void Print()
        {
            Console.WriteLine("Exception Start......");
            throw new Exception("This Exception Error");
        }
    }
}
