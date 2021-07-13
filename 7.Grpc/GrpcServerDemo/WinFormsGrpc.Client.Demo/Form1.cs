using Google.Protobuf;
using Grpc.Core;
using Grpc.Net.Client;
using Grpc.Server.Demo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinFormsGrpc.Client.Demo
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // 允许多线程更新UI 
            Control.CheckForIllegalCrossThreadCalls = false;
        }

        #region 简单模式
        private void btn_sample_Click(object sender, EventArgs e)
        {
            //1、创建grpc客户端
            using var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var grpcClient = new StudentService.StudentServiceClient(channel);
            //2、发起请求
            var resp = grpcClient.GetStudentByUserName(new QueryStudentRequest
            {
                UserName = txt_condition.Text.Trim()
            });
            //3、处理响应结果，将其显示在文本框中
            this.txt_result.Text = $"姓名：{resp.Student.UserName}，年龄：{resp.Student.Age}，地址：{resp.Student.Addr}";
        }
        #endregion

        #region 服务端流模式
        private async void btn_server_Click(object sender, EventArgs e)
        {
            //用于多线程通知
            CancellationTokenSource cts = new CancellationTokenSource();
            //1、创建grpc客户端
            using var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var grpcClient = new StudentService.StudentServiceClient(channel);

            //2、客户端一次请求，多次返回
            using var respStreamingCall = grpcClient.GetAllStudent(new QueryAllStudentRequest());
            //3、获取响应流
            var respStreaming = respStreamingCall.ResponseStream;
            //4、循环读取响应流，直到读完为止
            while (await respStreaming.MoveNext(cts.Token))
            {
                //5、取得每次返回的信息，并显示在文本框中
                var student = respStreaming.Current.Student;
                this.txt_result.Text += $"姓名：{student.UserName}，年龄：{student.Age}，地址：{student.Addr}\r\n";
            }
        } 
        #endregion

        private async void btn_client_Click(object sender, EventArgs e)
        {
            // 用于存放选择的文件路径
            string filePath = string.Empty;
            // 打开文件选择对话框
            if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                filePath = this.openFileDialog1.FileName;
            }
            if(string.IsNullOrEmpty(filePath))
            {
                this.txt_result.Text = "请选择文件";
                return;
            }
            //1、创建grpc客户端
            using var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var grpcClient = new StudentService.StudentServiceClient(channel);
            //2、读取选择的文件
            FileStream fileStream = File.OpenRead(filePath);
            //3、通过客户端请求流将文件流发送的服务端
            using var call = grpcClient.UploadImg();
            var clientStream = call.RequestStream;
            //4、循环发送，指定发送完文件
            while(true)
            {
                // 一次最多发送1024字节
                byte[] buffer = new byte[1024];
                int nRead = await fileStream.ReadAsync(buffer, 0, buffer.Length);
                // 直到读不到数据为止，即文件已经发送完成，即退出发送
                if(nRead==0)
                {
                    break;
                }
                // 5、将每次读取到的文件流通过客户端流发送到服务端
                await clientStream.WriteAsync(new UploadImgRequest { Data = ByteString.CopyFrom(buffer) });
            }
            // 6、发送完成之后，告诉服务端发送完成
            await clientStream.CompleteAsync();
            // 7、接收返回结果，并显示在文本框中
            var res = await call.ResponseAsync;
            this.txt_result.Text = $"上传返回Code:{res.Code},Msg:{res.Msg}";
        }

        private async void btn_double_Click(object sender, EventArgs e)
        {
            //用于多线程通知
            CancellationTokenSource cts = new CancellationTokenSource();
            //模拟通过请求流方式保存多个Student，同时通过响应流的方式返回存储结果
            List<Student> students = new List<Student> { 
                new Student{ UserName="Code综艺圈1", Age=20, Addr="关注我一块学" },
                new Student{ UserName="综艺圈好酷", Age=18, Addr="关注Code综艺圈和我一块学" }
            };
            //1、创建grpc客户端
            using var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var grpcClient = new StudentService.StudentServiceClient(channel);
            //2、分别取得请求流和响应流
            using var call = grpcClient.AddManyStudents();
            var requestStream = call.RequestStream;
            var responseStream = call.ResponseStream;
            //3、开启一个线程专门用来接收响应流
            var taskResp = Task.Run(async()=> { 
                while(await responseStream.MoveNext(cts.Token))
                {
                    var student = responseStream.Current.Student;
                    // 将接收到结果在文本框中显示 ，多线程更新UI简单处理一下：Control.CheckForIllegalCrossThreadCalls = false;
                    this.txt_result.Text += $"保存成功，姓名：{student.UserName}，年龄：{student.Age}，地址：{student.Addr}\r\n";
                }
            });
            //4、通过请求流的方式将多条数据依次传到服务端
            foreach (var student in students)
            {
                // 每次发送一个学生请求
                await requestStream.WriteAsync(new AddStudentRequest
                {
                     Student = student
                });
            }
            //5、传送完毕
            await requestStream.CompleteAsync();
            await taskResp;
        }

        private async Task<string> GetToken()
        {
            //1、创建grpc客户端
            using var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var grpcClient = new StudentService.StudentServiceClient(channel);
            //2、发起请求
            var resp = await grpcClient.GetTokenAsync(new TokenRequest { 
                 UserName= "Code综艺圈",
                 UserPwd= "admin123"
            });
            return resp.Token;
        }

        private async void btn_server_token_Click(object sender, EventArgs e)
        {
            //用于多线程通知
            CancellationTokenSource cts = new CancellationTokenSource();
            //1、创建grpc客户端
            using var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var grpcClient = new StudentService.StudentServiceClient(channel);

            // 获取Token
            var token = await GetToken();
            // 将Token封装为Metadat和请求一起发送过去
            var headers = new Metadata
            {
                {"Authorization",$"Bearer {token}"}
            };

            //2、客户端一次请求，多次返回
            using var respStreamingCall = grpcClient.GetAllStudent(new QueryAllStudentRequest(),headers);
            //3、获取响应流
            var respStreaming = respStreamingCall.ResponseStream;
            //4、循环读取响应流，直到读完为止
            while (await respStreaming.MoveNext(cts.Token))
            {
                //5、取得每次返回的信息，并显示在文本框中
                var student = respStreaming.Current.Student;
                this.txt_result.Text += $"姓名：{student.UserName}，年龄：{student.Age}，地址：{student.Addr}\r\n";
            }
        }
    }
}
