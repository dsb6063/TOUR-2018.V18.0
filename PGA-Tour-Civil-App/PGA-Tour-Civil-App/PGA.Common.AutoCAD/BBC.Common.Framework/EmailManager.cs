using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Mail;

//using Common.Logging;

namespace BBC.Common.Framework
{
    /// <summary>
    /// Class to send email
    /// </summary>
    public static class EmailManager
    {
        #region Private Members

        //private static readonly ILog //_logger = LogManager.GetLogger("EmailManager");
        private const string EMAIL_SERVER = "exchange12.utility.pge.com";

        #endregion

        #region Public Static Methods

        public static void SendMail(string to, string from, string subject, string body)
        {
            try
            {
                MailAddress fromAddr = new MailAddress(from);
                MailAddress toAddr = new MailAddress(to);

                MailMessage msg = new MailMessage(fromAddr, toAddr);
                msg.Subject = subject;
                msg.Body = body;

                string server = EMAIL_SERVER;
                SmtpClient client = new SmtpClient(server);
                client.Credentials = new NetworkCredential("s5mk", "sMmM6201!", server);
                client.UseDefaultCredentials = true;
                client.Send(msg);
            }
            catch (System.Net.Mail.SmtpFailedRecipientException failedRecipent)
            {
                PGA.MessengerManager.MessengerManager.AddLog("Could not resolve Recipient", failedRecipent);
            }
            catch (System.Net.Mail.SmtpException smtpEx)
            {
                PGA.MessengerManager.MessengerManager.AddLog("Error sending email. Please check configuration in file EmailManager.cs", smtpEx);
            }
        } 

        #endregion
    }
}
