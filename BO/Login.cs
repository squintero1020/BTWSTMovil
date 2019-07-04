using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ice.Adapters;
using Ice.Proxy.BO;
using Ice.BO;
using System.Configuration;
using System.Xml;

namespace BTWSTMovil.BO
{
    public class Login
    {
        readonly Globales.Functions fn = new Globales.Functions();
        protected bool ValidatePassword(string Usuario,string Password,out string email,out string Error)
        {
            Error = string.Empty;
            email = string.Empty;
            bool returnObj = false;
            try
            {
                string ConfigFile = string.Empty;
                fn.ReadConfig(out ConfigFile);
                /*INICIO EPICOR*/
                using (Ice.Core.Session sess = new Ice.Core.Session(
                    ConfigurationManager.AppSettings["UserEpicor"].ToString(),
                    ConfigurationManager.AppSettings["PassEpicor"].ToString(), "", Ice.Core.Session.LicenseType.Default, ConfigFile))
                {
                    UserFileAdapter userFileAdapter = new UserFileAdapter(sess);
                    userFileAdapter.BOConnect();
                    /*VALIDO LOS DATOS DE ACCESO*/
                    returnObj = userFileAdapter.ValidatePassword(Usuario, Password);
                    if (!returnObj)
                    {
                        Error = "Nombre de usuario ó contraseña inválido";
                        return false;
                    }
                    /*INICIO EPICOR CON LOS DATOS DEL EMPLEADO*/
                    using (Ice.Core.Session sessuser = new Ice.Core.Session(
                    Usuario,
                    Password, "", Ice.Core.Session.LicenseType.Default, ConfigFile))
                    {
                        /*OBTENGO EL CORREO DEL EMPLEADO*/
                        email = sessuser.UserEmail;
                        sessuser.Dispose();
                    }
                    sess.Dispose();
                }
            }
            catch (Exception ex)
            {
                Error = ex.Message + "\nExc: " +ex.InnerException;
            }
            return returnObj;
        }

       
    }
}