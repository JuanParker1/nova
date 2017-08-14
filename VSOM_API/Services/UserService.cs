using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VSOM_API.Models;
namespace VSOM_API.Services
{
    public class UserService
    {
        public Users GetCredentialsUsers(string login, string password)
        {
            return new Users { id = 1, login = "Login", mdp = null, Nom = "Credentials" };
        }
    }
}