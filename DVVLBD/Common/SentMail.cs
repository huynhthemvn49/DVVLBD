using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace DVVLBD.Common
{
    public class SentMail
    {
        static String DVVLEmail = "huynhthema3@gmail.com";
        static String DVVLName = "DVVLBD";
        static String DVVLPassword = "19062014";

        public static void Send(String Email, String Subject, String Body)
        {
            SmtpClient smtp = new SmtpClient();
            try
            {
                //ĐỊA CHỈ SMTP Server
                smtp.Host = "smtp.gmail.com";
                //Cổng SMTP
                smtp.Port = 587;
                //SMTP yêu cầu mã hóa dữ liệu theo SSL
                smtp.EnableSsl = true;
                //UserName và Password của mail
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(DVVLEmail, DVVLPassword);
                MailAddress from = new MailAddress(DVVLEmail, DVVLName);
                MailMessage message = new MailMessage(from.ToString(), Email, Subject, Body);
                //var path = System.Web.Hosting.HostingEnvironment.MapPath(@"~/Images/Template_1.pdf");
                //Attachment at = new Attachment(path);
                //message.Attachments.Add(at);
                message.IsBodyHtml = true;
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                //smtp.UseDefaultCredentials = true;
                smtp.Send(message);
            }
            catch (Exception ex)
            {

            }
        }
    }
}