using System;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace Common.Library.Email
{
    /// <summary>
    /// C#邮件发送类
    /// </summary>
    public class EmailHelper
    {
        public bool SendMail(EmailSender sender, out string errorMsg)
        {
            //声明一个Mail对象
            MailMessage mymail = new MailMessage();
            //发件人地址
            //如是自己，在此输入自己的邮箱
            mymail.From = new MailAddress(sender.SendAddress, sender.SendNickName, Encoding.UTF8);
            //收件人地址
            mymail.To.Add(new MailAddress(sender.ReceiveAddress));
            //邮件主题
            mymail.Subject = sender.Title;
            //邮件标题编码
            mymail.SubjectEncoding = Encoding.UTF8;
            //发送邮件的内容
            mymail.Body = sender.Content;
            //邮件内容编码
            mymail.BodyEncoding = Encoding.UTF8;
            //添加附件
            foreach (var attachment in sender.Attachments)
            {
                mymail.Attachments.Add(attachment);
            }

            //抄送到其他邮箱
            foreach (var str in sender.Cc)
            {
                mymail.CC.Add(new MailAddress(str));
            }

            //是否是HTML邮件
            mymail.IsBodyHtml = sender.IsBodyHtml;
            //邮件优先级
            mymail.Priority = sender.MailPriority;
            //创建一个邮件服务器类
            SmtpClient myclient = new SmtpClient();
            myclient.Host = sender.Host;
            //SMTP服务端口
            myclient.Port = sender.Port;
            //验证登录
            myclient.Credentials = new NetworkCredential(sender.Username, sender.Password);//"@"输入有效的邮件名, "*"输入有效的密码
            try
            {
                myclient.Send(mymail);
                errorMsg = "";
                return true;
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                return false;
            }

        }
    }
}