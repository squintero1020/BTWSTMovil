using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BTWSTMovil.Models
{
    public class Login:BO.Login
    {
        public bool ValidateUserPassword(string uid,string pass,out string email,out string Error)
        {
            return this.ValidatePassword(uid, pass, out email,out Error);
        }
    }
}