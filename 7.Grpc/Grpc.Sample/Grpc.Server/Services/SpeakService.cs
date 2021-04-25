using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using GrpcService;
using Microsoft.Extensions.Logging;

namespace Grpc.Server
{
    public class SpeakService : Speak.SpeakBase
    {
        private readonly ILogger<SpeakService> _logger;
        public SpeakService(ILogger<SpeakService> logger)
        {
            _logger = logger;
        }

        private static readonly List<string> speaks = new List<string>() { "英语", "汉语", "德语", "汉语", "俄语" };
        private static readonly Random Rand = new Random(DateTime.Now.Millisecond);

        public override Task<SpeakResult> SpeakL(Empty request, ServerCallContext context)
        {
            return Task.FromResult(new SpeakResult()
            {
                Message = $"{speaks[Rand.Next(0, speaks.Count)]} 交流开始..."
            });
        }

        public override async Task Chat(IAsyncStreamReader<ChatRequest> requestStream, IServerStreamWriter<ChatResponse> responseStream, ServerCallContext context)
        {
            var bathQueue = new Queue<int>();
            while (await requestStream.MoveNext())
            {
                //加入队列
                bathQueue.Enqueue(requestStream.Current.Id);

                _logger.LogInformation($"Chat {requestStream.Current.Id} Enqueue。");
            }

            //遍历队列
            while (!context.CancellationToken.IsCancellationRequested && bathQueue.TryDequeue(out var catId))
            {
                _logger.LogInformation($"Chat {catId} Dequeue.");
                await responseStream.WriteAsync(new ChatResponse() { Message = $"开始用：{speaks[catId]} 交流了！" });

                await Task.Delay(500);//此处主要是为了方便客户端能看出流调用的效果
            }
        }

        public override Task<CountResult> Count(Empty request, ServerCallContext context)
        {
            return Task.FromResult(new CountResult()
            {
                Count = speaks.Count
            });
        }

    }
}
