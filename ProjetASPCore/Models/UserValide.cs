using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjetASPCore.Models
{
    public class UserValide
    {

        private static readonly IHttpContextAccessor _httpContextAccessor;
        private static ISession _session => _httpContextAccessor.HttpContext.Session;
        private static IHttpContextAccessor httpContextAccessor { get; set; }
        static UserValide()
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public static bool IsValid()
        {
            if (_session.GetString("userId") != null)
            {
                return true;
            }
            else
                return false;
        }
        public static bool IsAdmin()
        {

            if (_session.GetString("role").Equals("Departement"))
            {
                return true;
            }
            else
                return false;
        }
        public static bool IsStudent()
        {

            if (_session.GetString("role").Equals("Etudiant"))
            {
                return true;
            }
            else
                return false;
        }
    }
}
