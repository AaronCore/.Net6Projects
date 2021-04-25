using System;
using System.Collections.Generic;
using System.Text;
using Grpc.Core;
using Grpc.Core.Interceptors;

namespace Grpc.Client
{
    public class ClientLoggerInterceptor : Interceptor
    {
        /*
         * BlockingUnaryCall	            拦截阻塞调用
         * AsyncUnaryCall	                拦截异步调用
         * AsyncServerStreamingCall	        拦截异步服务端流调用
         * AsyncClientStreamingCall	        拦截异步客户端流调用
         * AsyncDuplexStreamingCall	        拦截异步双向流调用
         * UnaryServerHandler	            用于拦截和传入普通调用服务器端处理程序
         * ClientStreamingServerHandler	    用于拦截客户端流调用的服务器端处理程序
         * ServerStreamingServerHandler	    用于拦截服务端流调用的服务器端处理程序
         * DuplexStreamingServerHandler	    用于拦截双向流调用的服务器端处理程序
         */


        public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(
            TRequest request,
            ClientInterceptorContext<TRequest, TResponse> context,
            AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
        {
            LogCall(context.Method);

            return continuation(request, context);
        }

        private void LogCall<TRequest, TResponse>(Method<TRequest, TResponse> method)
            where TRequest : class
            where TResponse : class
        {
            var initialColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"客户端拦截器：Starting call. Type: {method.Type}. Request: {typeof(TRequest)}. Response: {typeof(TResponse)}");
            Console.ForegroundColor = initialColor;
        }

    }
}
