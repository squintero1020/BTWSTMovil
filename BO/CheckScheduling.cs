using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Erp.Adapters;
using Erp.Proxy.BO;
using Erp.BO;

namespace BTWSTMovil.BO
{
    public class CheckScheduling
    {
        readonly Globales.Functions fn = new Globales.Functions();
        readonly Models.CNNSql sql = new Models.CNNSql();
        readonly CO.Connect cn = new CO.Connect();
        protected Entidades.EntidadesCheckSchedule checkSchedule(Entidades.EntidadesCheckSchedule enT)
        {
            Entidades.EntidadesCheckSchedule entidadesCheckSchedule = new Entidades.EntidadesCheckSchedule();
            try
            {
                string ConfigFile = string.Empty;
                fn.ReadConfig(out ConfigFile);
                /*INICIO EPICOR*/
                using (Ice.Core.Session sess = new Ice.Core.Session(
                    enT.DcdUserID,
                    enT.Pwd,
                    "",
                    Ice.Core.Session.LicenseType.Default,
                    ConfigFile
                    )
                    )
                {
                    /*OBJETOS E INSTANCIAS DE EPICOR*/
                    SchedulingBoardAdapter schedulingBoardAdapter = new SchedulingBoardAdapter(sess);
                    schedulingBoardAdapter.BOConnect();
                    SchedulingBoardImpl schedulingBoardImpl = schedulingBoardAdapter.BusinessObject as SchedulingBoardImpl;
                    SchedulingBoardDataSet schedulingBoardDataSet = new SchedulingBoardDataSet();
                    /*PREGUNTO LA PROGRAMACIÓN DEL EMPLEADO, PARA EL RANGO DE FECHAS*/
                    EmpBasicAdapter empBasicAdapter = new EmpBasicAdapter(sess);
                    empBasicAdapter.BOConnect();
                    EmpBasicImpl empBasicImpl = empBasicAdapter.BusinessObject as EmpBasicImpl;
                    EmpBasicDataSet empBasicDataSet = new EmpBasicDataSet();
                    empBasicDataSet = empBasicImpl.GetByID(sess.EmployeeID);
                    var dRowEmp = (EmpBasicDataSet.EmpBasicRow)empBasicDataSet.EmpBasic.Rows[(empBasicDataSet.EmpBasic.Rows.Count -1)];
                    schedulingBoardDataSet = schedulingBoardImpl.BuildResourceLine("", dRowEmp.ResourceID, "", Convert.ToDateTime(enT.StartDate.ToString("MM/dd/yyyy")), Convert.ToDateTime(enT.EndDate.ToString("MM/dd/yyyy")));
                    //entidadesCheckSchedule.DcdUserID = sess.EmployeeID + enT.StartDate.ToString("MM/dd/yyyy") + enT.EndDate.ToString("MM/dd/yyyy");
                    entidadesCheckSchedule.DcdUserID = dRowEmp.ResourceID;
                    entidadesCheckSchedule.Pwd = "********";
                    entidadesCheckSchedule.StartDate = enT.StartDate;
                    entidadesCheckSchedule.EndDate = enT.EndDate;
                    entidadesCheckSchedule.dt = new DataSet();
                    if (schedulingBoardDataSet.SchedulingBoard.Rows.Count == 0)
                    {
                        entidadesCheckSchedule.dt = null;
                        entidadesCheckSchedule.Msj = "No tiene trabajos pendientes a la fecha";
                    }
                    else
                    {
                        /*COPIO EL RESULTADO DE LA TABLA, A LA ENTIDAD DE PROGRAMACIÓN, CON EL CRITERIO ROWNUM > 0*/
                        DataTable dTable = new DataTable();
                        dTable.TableName = "Operaciones";
                        dTable.Columns.Add("JobNum",        typeof(string));
                        dTable.Columns.Add("oprSeq",        typeof(int));
                        dTable.Columns.Add("OSDate",        typeof(DateTime));
                        dTable.Columns.Add("ODDate",        typeof(DateTime));
                        dTable.Columns.Add("OSHour",        typeof(decimal));
                        dTable.Columns.Add("ODHour",        typeof(decimal));
                        dTable.Columns.Add("IDServicio",    typeof(int));
                        dTable.Columns.Add("CustNum",       typeof(int));
                        dTable.Columns.Add("CustName",      typeof(string));
                        dTable.Columns.Add("ShipToNum",     typeof(string));
                        dTable.Columns.Add("ShipToName",    typeof(string));
                        dTable.Columns.Add("Address",       typeof(string));
                        dTable.Columns.Add("Phone",         typeof(string));
                        dTable.Columns.Add("Latitud",       typeof(decimal));
                        dTable.Columns.Add("Longitud",      typeof(decimal));
                        dTable.Columns.Add("Observation",   typeof(string));

                        DataRow dRow;
                        foreach(SchedulingBoardDataSet.SchedulingBoardRow dr in schedulingBoardDataSet.SchedulingBoard.Select("RowNum > 0"))
                        {
                            dRow = dTable.NewRow();
                            dRow["JobNum"] = dr.JobNum;
                            dRow["oprSeq"] = dr.OprNum;
                            dRow["OSDate"] = dr.OSDate;
                            dRow["ODDate"] = dr.ODDate;
                            dRow["OSHour"] = dr.OSHour;
                            dRow["ODHour"] = dr.ODHour;
                            string Error = string.Empty;
                            var dsCustomer = cn.cmdConsultar(sql.GetCustomerShipTo(sess.CompanyID, dr.JobNum), out Error);
                            if(!string.IsNullOrEmpty(Error))
                            {
                                entidadesCheckSchedule.Msj = Error;
                                return entidadesCheckSchedule;
                            }
                            if(dsCustomer.Tables["Tabla"].Rows.Count > 0)
                            {
                                foreach(DataRow dRowCustomer in dsCustomer.Tables["Tabla"].Rows)
                                {
                                    dRow["IDServicio"] = dRowCustomer["IDServicio"].ToString();
                                    dRow["CustNum"] = dRowCustomer["CustNum"].ToString();
                                    dRow["CustName"] = dRowCustomer["CustName"].ToString();
                                    dRow["ShipToNum"] = dRowCustomer["ShipToNum"].ToString();
                                    dRow["ShipToName"] = dRowCustomer["ShipToName"].ToString();
                                    dRow["Address"] = dRowCustomer["Address"].ToString();
                                    dRow["Phone"] = dRowCustomer["PhoneNum"].ToString();
                                    dRow["Latitud"] = Convert.ToDecimal(dRowCustomer["Latitud"]);
                                    dRow["Longitud"] = Convert.ToDecimal(dRowCustomer["Longitud"]);
                                    dRow["Observation"] = dRowCustomer["Observation"].ToString();
                                }
                            }
                            //dr.
                            dTable.Rows.Add(dRow);
                        }

                        entidadesCheckSchedule.dt.Tables.Add(dTable);
                        entidadesCheckSchedule.Msj = "Trabajos pendientes cargados";
                    }
                }
            }
            catch (Exception ex)
            {
                entidadesCheckSchedule.dt = null;
                entidadesCheckSchedule.Msj = ex.Message + "\nExc: " + ex.InnerException;
            }
            return entidadesCheckSchedule;
        }
    }
}