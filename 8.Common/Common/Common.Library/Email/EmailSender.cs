using System.Collections.Generic;
using System.Net.Mail;

namespace Common.Library.Email
{
    public class EmailSender
    {
        private string sendAddress;
        private string sendNickName;
        private string receiveAddress;
        private string receiveNickName;
        private string title;
        private string content;
        private List<Attachment> attachments;
        private List<string> cc;
        private bool isBodyHtml;
        private MailPriority mailPriority;
        private string host;
        private int port;
        private string username;
        private string password;

        public EmailSender()
        {
            attachments = new List<Attachment>();
            cc = new List<string>();
        }

        /// <summary>
        /// 发送人地址
        /// </summary>
        public string SendAddress
        {
            get { return sendAddress; }
            set { sendAddress = value; }
        }

        /// <summary>
        /// 发送人昵称
        /// </summary>
        public string SendNickName
        {
            get { return sendNickName; }
            set { sendNickName = value; }
        }

        /// <summary>
        /// 接收人地址
        /// </summary>
        public string ReceiveAddress
        {
            get { return receiveAddress; }
            set { receiveAddress = value; }
        }

        /// <summary>
        /// 接收人昵称
        /// </summary>
        public string ReceiveNickName
        {
            get { return receiveNickName; }
            set { receiveNickName = value; }
        }

        /// <summary>
        /// 邮件标题
        /// </summary>
        public string Title
        {
            get { return title; }
            set { title = value; }
        }

        /// <summary>
        /// 邮件内容
        /// </summary>
        public string Content
        {
            get { return content; }
            set { content = value; }
        }

        /// <summary>
        /// 附件列表
        /// </summary>
        public List<Attachment> Attachments
        {
            get { return attachments; }
            set { attachments = value; }
        }

        /// <summary>
        /// 抄送列表
        /// </summary>
        public List<string> Cc
        {
            get { return cc; }
            set { cc = value; }
        }

        /// <summary>
        /// 是否为html邮件
        /// </summary>
        public bool IsBodyHtml
        {
            get { return isBodyHtml; }
            set { isBodyHtml = value; }
        }

        /// <summary>
        /// 邮件优先级
        /// </summary>
        public MailPriority MailPriority
        {
            get { return mailPriority; }
            set { mailPriority = value; }
        }

        /// <summary>
        /// smtp服务器
        /// </summary>
        public string Host
        {
            get { return host; }
            set { host = value; }
        }

        /// <summary>
        /// 端口号
        /// </summary>
        public int Port
        {
            get { return port; }
            set { port = value; }
        }

        /// <summary>
        /// 邮件服务器验证用户名
        /// </summary>
        public string Username
        {
            get { return username; }
            set { username = value; }
        }

        /// <summary>
        /// 邮件服务器验证密码
        /// </summary>
        public string Password
        {
            get { return password; }
            set { password = value; }
        }
    }
}