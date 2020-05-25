using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetASPCore.Services
{
    public class MyComponent 
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public  MyComponent(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        public string GetDataFromSession()
        {
            return _contextAccessor.HttpContext.Session.GetString("userId");
        }
        public string GetRoleFromSession()
        {
            return _contextAccessor.HttpContext.Session.GetString("role");
        }
    }
}
