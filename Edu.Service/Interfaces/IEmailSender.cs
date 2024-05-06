using Edu.Abstraction.ComplexModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edu.Service.Interfaces
{
    /// <summary>
    /// Email sender
    /// </summary>
    public partial interface IEmailSender
    {
        Task<ResponseModel> SendSmtpEmailAsync(EmailModel email, IDictionary<string, string> headers = null);
    }
}
