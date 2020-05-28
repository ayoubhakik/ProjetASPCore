using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace ProjetASPCore.Services
{
    public interface IEmailService
    {

        Task SendEmailAsync(string email,string subject, string message);
    }
}
