using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace BTWSTMovil.Controllers
{
    [RoutePrefix("api/ServiceBTW")]
    public class ServiceConnectController: ApiController
    {
        readonly Globales.Functions fn = new Globales.Functions();

        [System.Web.Http.AcceptVerbs("POST")]
        [System.Web.Http.HttpPost]
        [Route("ValidateAccess")]
        public Entidades.EntidadesLogin ValidateAccess(Entidades.EntidadesLogin enT)
        {
            Entidades.EntidadesLogin userExist = new Entidades.EntidadesLogin();
            try
            {
                string Error = string.Empty;
                if (string.IsNullOrEmpty(enT.DcdUserID))
                {
                    userExist.Msj = "Necesita usuario de Epicor";
                    return userExist;
                }
                if (string.IsNullOrEmpty(enT.Pwd))
                {
                    userExist.Msj = "Necesita contraseña de Epicor";
                    return userExist;
                }
                Models.Login lg = new Models.Login();
                userExist.DcdUserID = enT.DcdUserID;
                userExist.Pwd = enT.Pwd;
                string Email = string.Empty;
                userExist.passLoginEpicor = lg.ValidateUserPassword(enT.DcdUserID, enT.Pwd,out Email, out Error);
                userExist.EmailAddress = Email;
                userExist.Msj = Error;
            }
            catch (Exception ex)
            {
                userExist.Msj = ex.Message;
                userExist.passLoginEpicor = false;
            }
            return userExist;
        }

        [System.Web.Http.AcceptVerbs("POST")]
        [System.Web.Http.HttpPost]
        [Route("CheckSchedule")]
        public Entidades.EntidadesCheckSchedule CheckSchedule(Entidades.EntidadesCheckSchedule enT)
        {
            Entidades.EntidadesCheckSchedule entidadesCheck = new Entidades.EntidadesCheckSchedule();
            try
            {
                string Error = string.Empty;
                if(string.IsNullOrEmpty(enT.DcdUserID))
                {
                    entidadesCheck.Msj = "Necesita usuario de Epicor";
                    return entidadesCheck;
                }
                if (string.IsNullOrEmpty(enT.Pwd))
                {
                    entidadesCheck.Msj = "Necesita contraseña de Epicor";
                    return entidadesCheck;
                }
                if ((!fn.IsDate(enT.StartDate.ToString("yyyy-MM-dd"))) || enT.StartDate.ToString("yyyy-MM-dd") == "0001-01-01")
                {
                    entidadesCheck.Msj = "Necesita fecha de inicio";
                    return entidadesCheck;
                }
                if ((!fn.IsDate(enT.EndDate.ToString("yyyy-MM-dd"))) || (enT.EndDate.ToString("yyyy-MM-dd") == "0001-01-01"))
                {
                    entidadesCheck.Msj = "Necesita fecha de fin";
                    return entidadesCheck;
                }

                Models.CheckSchedule checkSchedule = new Models.CheckSchedule();
                entidadesCheck = checkSchedule.CheckJobSchedule(enT);
            }
            catch (Exception ex)
            {
                entidadesCheck.Msj = ex.Message;
            }
            return entidadesCheck;
        }

        [System.Web.Http.AcceptVerbs("POST")]
        [System.Web.Http.HttpPost]
        [Route("GetInventory")]

        public Entidades.EntidadesGetInventory GetInventory(Entidades.EntidadesGetInventory enT)
        {
            Entidades.EntidadesGetInventory getInventory = new Entidades.EntidadesGetInventory();
            try
            {
                string Error = string.Empty;
                if (string.IsNullOrEmpty(enT.DcdUserID))
                {
                    getInventory.Msj = "Necesita usuario de Epicor";
                    return getInventory;
                }
                if (string.IsNullOrEmpty(enT.Pwd))
                {
                    getInventory.Msj = "Necesita contraseña de Epicor";
                    return getInventory;
                }
                Models.GetInventory getInventor = new Models.GetInventory();
                getInventory = getInventor.inventory(enT);
            }
            catch (Exception ex)
            {
                getInventory.Msj = ex.Message;
            }
            return getInventory;
        }
        [System.Web.Http.AcceptVerbs("POST")]
        [System.Web.Http.HttpPost]
        [Route("GetAttributes")]
        public Entidades.EntidadesGetAtributos GetAttributes(Entidades.EntidadesGetAtributos enT)
        {
            Entidades.EntidadesGetAtributos getAtributos = new Entidades.EntidadesGetAtributos();
            try
            {
                string Error = string.Empty;
                if (string.IsNullOrEmpty(enT.DcdUserID))
                {
                    getAtributos.Msj = "Necesita usuario de Epicor";
                    return getAtributos;
                }
                if (string.IsNullOrEmpty(enT.Pwd))
                {
                    getAtributos.Msj = "Necesita contraseña de Epicor";
                    return getAtributos;
                }
                if(string.IsNullOrEmpty(enT.JobNum))
                {
                    getAtributos.Msj = "Necesita un trabajo";
                    return getAtributos;
                }
                Models.GetInventory getInventory= new Models.GetInventory();
                getAtributos = getInventory.atributos(enT);
            }
            catch (Exception ex)
            {
                getAtributos.Msj = ex.Message;
            }
            return getAtributos;
                }
        [System.Web.Http.AcceptVerbs("POST")]
        [System.Web.Http.HttpPost]
        [Route("GetMaterial")]
        public Entidades.EntidadesGetMaterial GetMaterial(Entidades.EntidadesGetMaterial enT)
        {
            Entidades.EntidadesGetMaterial getMaterial = new Entidades.EntidadesGetMaterial();
            try
            {
                string Error = string.Empty;
                if (string.IsNullOrEmpty(enT.DcdUserID))
                {
                    getMaterial.Msj = "Necesita usuario de Epicor";
                    return getMaterial;
                }
                if (string.IsNullOrEmpty(enT.Pwd))
                {
                    getMaterial.Msj = "Necesita contraseña de Epicor";
                    return getMaterial;
                }
                if (string.IsNullOrEmpty(enT.JobNum))
                {
                    getMaterial.Msj = "Necesita un trabajo";
                    return getMaterial;
                }
                Models.GetInventory getInventory = new Models.GetInventory();
                getMaterial = getInventory.material(enT);
            }
            catch (Exception ex)
            {
                getMaterial.Msj = ex.Message;
            }
            return getMaterial;
        }

        [System.Web.Http.AcceptVerbs("POST")]
        [System.Web.Http.HttpPost]
        [Route("ReportTime")]
        public Entidades.EntidadesReportTime ReportTime(Entidades.EntidadesReportTime enT)
        {
            Entidades.EntidadesReportTime entidadesReportTime = new Entidades.EntidadesReportTime();
            entidadesReportTime.ReportCreate = false;
            try
            {
                string Error = string.Empty;
                if (string.IsNullOrEmpty(enT.DcdUserID))
                {
                    entidadesReportTime.Msj = "Necesita usuario de Epicor";
                    return entidadesReportTime;
                }
                if (string.IsNullOrEmpty(enT.Pwd))
                {
                    entidadesReportTime.Msj = "Necesita contraseña de Epicor";
                    return entidadesReportTime;
                }
                if (string.IsNullOrEmpty(enT.JobNum))
                {
                    entidadesReportTime.Msj = "Necesita un trabajo de Epicor";
                    return entidadesReportTime;
                }
                if ((!fn.IsDate(enT.StartDate.ToString("yyyy-MM-dd"))) || enT.StartDate.ToString("yyyy-MM-dd") == "0001-01-01")
                {
                    entidadesReportTime.Msj = "Necesita fecha de inicio";
                    return entidadesReportTime;
                }
                if ((!fn.IsDate(enT.EndDate.ToString("yyyy-MM-dd"))) || (enT.EndDate.ToString("yyyy-MM-dd") == "0001-01-01"))
                {
                    entidadesReportTime.Msj = "Necesita fecha de fin";
                    return entidadesReportTime;
                }
                Models.ReportTime reportTime = new Models.ReportTime();
                entidadesReportTime = reportTime.ReportTimeTechnical(enT);
            }
            catch (Exception ex)
            {
                entidadesReportTime.Msj = ex.Message;
            }
            return entidadesReportTime;
        }

        [System.Web.Http.AcceptVerbs("POST")]
        [System.Web.Http.HttpPost]
        [Route("JobTraveler")]
        public Entidades.EntidadesJobTraveler JobTraveler(Entidades.EntidadesJobTraveler enT)
        {
            Entidades.EntidadesJobTraveler entidadesReportTime = new Entidades.EntidadesJobTraveler();

            try
            {
                string Error = string.Empty;
                if (string.IsNullOrEmpty(enT.DcdUserID))
                {
                    entidadesReportTime.Msj = "Necesita usuario de Epicor";
                    return entidadesReportTime;
                }
                if (string.IsNullOrEmpty(enT.Pwd))
                {
                    entidadesReportTime.Msj = "Necesita contraseña de Epicor";
                    return entidadesReportTime;
                }
                if (string.IsNullOrEmpty(enT.JobNum))
                {
                    entidadesReportTime.Msj = "Necesita un trabajo de Epicor";
                    return entidadesReportTime;
                }

                Models.JobTraveler reportTime = new Models.JobTraveler();
                entidadesReportTime = reportTime.printJob(enT);
            }
            catch (Exception ex)
            {
                entidadesReportTime.Msj = ex.Message;
            }
            return entidadesReportTime;
        }

        [System.Web.Http.AcceptVerbs("POST")]
        [System.Web.Http.HttpPost]
        [Route("ReportMaterial")]
        public Entidades.EntidadesReportMaterial ReportMaterial(Entidades.EntidadesReportMaterial enT)
        {
            Entidades.EntidadesReportMaterial reportMaterial = new Entidades.EntidadesReportMaterial();
            try
            {
                string Error = string.Empty;
                if (string.IsNullOrEmpty(enT.DcdUserID))
                {
                    reportMaterial.Msj = "Necesita usuario de Epicor";
                    return reportMaterial;
                }
                if (string.IsNullOrEmpty(enT.Pwd))
                {
                    reportMaterial.Msj = "Necesita contraseña de Epicor";
                    return reportMaterial;
                }
                Models.ReportMaterial report = new Models.ReportMaterial();
                reportMaterial = report.material(enT);
            }
            catch (Exception ex)
            {
                reportMaterial.Msj = ex.Message;
            }
            return reportMaterial;
        }

        [System.Web.Http.AcceptVerbs("POST")]
        [System.Web.Http.HttpPost]
        [Route("ReturnMaterial")]
        public Entidades.EntidadesReturnMaterial ReturnMaterial(Entidades.EntidadesReturnMaterial enT)
        {
            Entidades.EntidadesReturnMaterial returnMaterial = new Entidades.EntidadesReturnMaterial();
            try
            {
                string Error = string.Empty;
                if (string.IsNullOrEmpty(enT.DcdUserID))
                {
                    returnMaterial.Msj = "Necesita usuario de Epicor";
                    return returnMaterial;
                }
                if (string.IsNullOrEmpty(enT.Pwd))
                {
                    returnMaterial.Msj = "Necesita contraseña de Epicor";
                    return returnMaterial;
                }
                Models.ReturnMaterial report = new Models.ReturnMaterial();
                returnMaterial = report.rMaterial(enT);
            }
            catch (Exception ex)
            {
                returnMaterial.Msj = ex.Message;
            }
            return returnMaterial;
        }
        [System.Web.Http.AcceptVerbs("POST")]
        [System.Web.Http.HttpPost]
        [Route("ReportLegalization")]
        public Entidades.EntidadesReturnMaterial ReportLegalization(Entidades.EntidadesReturnMaterial enT)
        {
            Entidades.EntidadesReturnMaterial returnMaterial = new Entidades.EntidadesReturnMaterial();
            try
            {
                string Error = string.Empty;
                if (string.IsNullOrEmpty(enT.DcdUserID))
                {
                    returnMaterial.Msj = "Necesita usuario de Epicor";
                    return returnMaterial;
                }
                if (string.IsNullOrEmpty(enT.Pwd))
                {
                    returnMaterial.Msj = "Necesita contraseña de Epicor";
                    return returnMaterial;
                }
                Models.ReturnMaterial report = new Models.ReturnMaterial();
                returnMaterial = report.rMaterial(enT);
            }
            catch (Exception ex)
            {
                returnMaterial.Msj = ex.Message;
            }
            return returnMaterial;
        }
    }
}