using Edu.Abstraction.ComplexModels;
using Edu.Abstraction.Models;
using Edu.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Edu.Service.Services
{
    /// <summary>
    /// Email sender
    /// </summary>
    public partial class EmailSender : IEmailSender
    {
        #region Fields

        private readonly IEmailAccountService _emailAccountService;
        private readonly ISmtpBuilder _smtpBuilder;

        #endregion

        #region Ctor

        public EmailSender(
            IEmailAccountService emailAccountService,
            ISmtpBuilder smtpBuilder)
        {
            _emailAccountService = emailAccountService;
            _smtpBuilder = smtpBuilder;
        }

        #endregion

        #region Methods

        public async Task<ResponseModel> SendSmtpEmailAsync(EmailModel email, IDictionary<string, string> headers = null)
        {
            var emailAccount = (await _emailAccountService.GetDefaultEmailAccount()).Data as EmailAccount;

            using (var message = new MailMessage
            {
                From = new MailAddress(emailAccount.Email, emailAccount.DisplayName),
                IsBodyHtml = true,
                Subject = email.Subject,
                Body = email.Body
            })
            {
                //To
                if (!string.IsNullOrEmpty(email.To))
                {
                    foreach (var address in email.To.Split(',').Where(to => !string.IsNullOrWhiteSpace(to)))
                    {
                        message.To.Add(address.Trim());
                    }
                }

                //Reply To
                if (!string.IsNullOrEmpty(email.ReplyTo))
                {
                    foreach (var address in email.ReplyTo.Split(',').Where(reply => !string.IsNullOrWhiteSpace(reply)))
                    {
                        message.ReplyTo = new MailAddress(address.Trim());
                    }
                }

                //BCC
                if (!string.IsNullOrEmpty(email.Bcc))
                {
                    foreach (var address in email.Bcc.Split(',').Where(bcc => !string.IsNullOrWhiteSpace(bcc)))
                    {
                        message.Bcc.Add(address.Trim());
                    }
                }

                //CC
                if (!string.IsNullOrEmpty(email.CC))
                {
                    foreach (var address in email.CC.Split(',').Where(cc => !string.IsNullOrWhiteSpace(cc)))
                    {
                        message.CC.Add(address.Trim());
                    }
                }

                //headers
                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        message.Headers.Add(header.Key, header.Value);
                    }
                }

                //attachments
                foreach (var attachment in email.Attachments)
                {
                    message.Attachments.Add(new Attachment(new MemoryStream(attachment.FileBytes), attachment.FileName));
                }

                try
                {
                    //send email
                    using (var smtpClient = await _smtpBuilder.BuildSmtpAsync(emailAccount))
                    {
                        await smtpClient.SendMailAsync(message);
                    }
                    return new ResponseModel { Success = true, StatusCode = StatusCodes.Status200OK, Message = "Mail sent successfully." };
                }
                catch (Exception ex)
                {
                    return new ResponseModel { Success = false, StatusCode = StatusCodes.Status500InternalServerError, Message = "Mail not sent.", Data = ex };
                }
                finally
                {
                    // TODO: Save Email Details in Database
                }
            }
        }

        #endregion
    }
}
