using System;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Edu.Abstraction.Models;
using Edu.Service.Interfaces;

namespace Edu.Service.Services
{
    /// <summary>
    /// SMTP Builder
    /// </summary>
    public class SmtpBuilder : ISmtpBuilder
    {
        #region Fields

        private readonly IEmailAccountService _emailAccountService;

        #endregion

        #region Ctor

        public SmtpBuilder(IEmailAccountService emailAccountService)
        {
            _emailAccountService = emailAccountService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Create a new SMTP client for a specific email account
        /// </summary>
        /// <param name="emailAccount">Email account to use. If null, then would be used EmailAccount by default</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the an SMTP client that can be used to send email messages
        /// </returns>
        public virtual async Task<SmtpClient> BuildSmtpAsync(EmailAccount emailAccount = null)
        {
            if (emailAccount is null)
            {
                emailAccount = (await _emailAccountService.GetDefaultEmailAccount()).Data
                ?? throw new Exception("Email account could not be loaded");
            }

            var client = new SmtpClient
            {
                EnableSsl = emailAccount.EnableSsl,
                Port = emailAccount.Port,
                Host = emailAccount.Host,
                UseDefaultCredentials = emailAccount.UseDefaultCredentials
            };

            if (emailAccount.EnableSsl)
            {
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls |
                                                        SecurityProtocolType.Tls11 |
                                                        SecurityProtocolType.Tls12;
            }

            try
            {
                if (!emailAccount.UseDefaultCredentials && !string.IsNullOrWhiteSpace(emailAccount.Username))
                {
                    client.Credentials = new NetworkCredential(emailAccount.Username, emailAccount.Password);
                }
                else
                {
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    client.Credentials = new NetworkCredential();
                }

                return client;
            }
            catch (Exception ex)
            {
                client.Dispose();
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// Validates the remote Secure Sockets Layer (SSL) certificate used for authentication.
        /// </summary>
        /// <param name="sender">An object that contains state information for this validation.</param>
        /// <param name="certificate">The certificate used to authenticate the remote party.</param>
        /// <param name="chain">The chain of certificate authorities associated with the remote certificate.</param>
        /// <param name="sslPolicyErrors">One or more errors associated with the remote certificate.</param>
        /// <returns>A System.Boolean value that determines whether the specified certificate is accepted for authentication</returns>
        public virtual bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            //By default, server certificate verification is disabled.
            return true;
        }

        #endregion
    }
}
