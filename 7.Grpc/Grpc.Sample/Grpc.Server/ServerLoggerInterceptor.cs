using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Grpc.Server
{
    public class ServerLoggerInterceptor : Interceptor
    {
        private readonly ILogger<ServerLoggerInterceptor> _logger;

        public ServerLoggerInterceptor(ILogger<ServerLoggerInterceptor> logger)
        {
            _logger = logger;
        }

        public override Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
            TRequest request,
            ServerCallContext context,
            UnaryServerMethod<TRequest, TResponse> continuation)
        {
            LogCall<TRequest, TResponse>(MethodType.Unary, context);
            return continuation(request, context);
        }

        private void LogCall<TRequest, TResponse>(MethodType methodType, ServerCallContext context)
            where TRequest : class
            where TResponse : class
        {
            _logger.LogWarning($"服务端拦截器：Starting call. Type: {methodType}. Request: {typeof(TRequest)}. Response: {typeof(TResponse)}");
        }
    }
}
