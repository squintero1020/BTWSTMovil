using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Erp.Adapters;
using Erp.Proxy.BO;
using Erp.BO;
using System.Globalization;

namespace BTWSTMovil.BO
{
    public class ReportTime
    {
        readonly Globales.Functions fn = new Globales.Functions();
        protected Entidades.EntidadesReportTime reportTime(Entidades.EntidadesReportTime enT)
        {
            Entidades.EntidadesReportTime entidadesReportTime = new Entidades.EntidadesReportTime();
            entidadesReportTime.ReportCreate = enT.ReportCreate;
            entidadesReportTime.DcdUserID = enT.DcdUserID;
            entidadesReportTime.Pwd = "********";
            entidadesReportTime.JobNum = enT.JobNum;
            entidadesReportTime.oprSeq = enT.oprSeq;
            entidadesReportTime.StartDate = enT.StartDate;
            entidadesReportTime.EndDate = enT.EndDate;
            entidadesReportTime.IndirectCode = enT.IndirectCode;
            entidadesReportTime.LaborType = enT.LaborType;
            entidadesReportTime.LaborNote = enT.LaborNote;
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
                    EmpBasicAdapter empBasicAdapter = new EmpBasicAdapter(sess);
                    empBasicAdapter.BOConnect();
                    EmpBasicImpl empBasicImpl = empBasicAdapter.BusinessObject as EmpBasicImpl;
                    EmpBasicDataSet empBasicDataSet = new EmpBasicDataSet();
                    empBasicDataSet = empBasicImpl.GetByIDForTE(sess.EmployeeID);
                    EmpBasicDataSet.EmpBasicRow empBasicRow = (EmpBasicDataSet.EmpBasicRow)empBasicDataSet.EmpBasic.Rows[0];
                    //
                    LaborAdapter laborAdapter = new LaborAdapter(sess);
                    laborAdapter.BOConnect();
                    LaborImpl laborImpl = laborAdapter.BusinessObject as LaborImpl;
                    LaborDataSet laborDataSet = new LaborDataSet();
                    string whereClauseLaborHed = string.Concat(new string[]
                    {
                        "EmployeeNum = '",
                        sess.EmployeeID,
                        "' AND PayrollDate >= '",
                        string.Format("{0:MM/dd/yyyy}", enT.StartDate),
                        "' AND PayrollDate <= '",
                        string.Format("{0:MM/dd/yyyy}", enT.EndDate),
                        "'"
                    });
                    string whereClauseLaborDtl = "ActiveTrans = 0";
                    string vMessage = string.Empty;
                    bool MorePages;
                    /*CALENDARIO DEL EMPLEADO*/
                    laborDataSet = laborImpl.GetRowsCalendarView(whereClauseLaborHed, whereClauseLaborDtl, "", "", "", "", "", "", "", "", "", "", 0, 1, sess.EmployeeID, new DateTime?(enT.StartDate), new DateTime?(enT.EndDate), out MorePages);
                    /*CREÓ UN NUEVO REPORTE DE TIEMPO*/
                    laborImpl.GetNewLaborDtlNoHdr(laborDataSet, sess.EmployeeID, false, new DateTime?(DateTime.Now), decimal.Zero, new DateTime?(DateTime.Now), decimal.Zero);
                    /*TIPO DE REPORTE ? DIRECTO O INDIRECTO*/
                    if (!enT.LaborType)
                    {
                        var dr = (LaborDataSet.LaborDtlRow)laborDataSet.LaborDtl.Rows[laborDataSet.LaborDtl.Rows.Count - 1];
                        dr.NewDifDateFlag = 1;
                        dr.SubmittedBy = sess.EmployeeID;
                        dr.IndirectCode = enT.IndirectCode;
                        dr.ExpenseCode = enT.ExpenseCode;
                        laborImpl.DefaultLaborType(laborDataSet,"I");
                        laborImpl.ChangeLaborType(laborDataSet);
                        laborImpl.LaborRateCalc(laborDataSet);
                    }
                    else
                    {
                       
                        laborImpl.DefaultLaborType(laborDataSet, "V");
                        laborImpl.ChangeLaborType(laborDataSet);
                        laborImpl.DefaultJobNum(laborDataSet, enT.JobNum);
                        laborImpl.LaborRateCalc(laborDataSet);
                        laborImpl.DefaultOprSeq(laborDataSet, enT.oprSeq, out vMessage);
                        laborImpl.LaborRateCalc(laborDataSet);
                        laborImpl.DefaultLaborQty(laborDataSet, decimal.One, out vMessage);
                    }
                    /*CONFIGRUACIÓN REGIONAL PARA LA HORA*/
                    CultureInfo provider = new CultureInfo("es-CO");
                    string sInicio = Convert.ToDateTime(enT.StartDate, provider).ToString("HH:mm");
                    string sFinal = Convert.ToDateTime(enT.EndDate, provider).ToString("HH:mm");

                    decimal HoraInicio = Convert.ToDecimal(TimeSpan.Parse(sInicio).TotalHours);
                    decimal HoraFinal = Convert.ToDecimal(TimeSpan.Parse(sFinal).TotalHours);
                    /*CAMBIO LA FECHA DEL REPORTE*/
                    laborImpl.OnChangeClockInDate(laborDataSet, new DateTime?(enT.StartDate));
                    var drU = (LaborDataSet.LaborDtlRow)laborDataSet.LaborDtl.Rows[laborDataSet.LaborDtl.Rows.Count - 1];
                    drU.SubmittedBy = sess.EmployeeID;
                    drU.ClockinTime = HoraInicio;
                    drU.ClockOutTime = HoraFinal;
                    drU.DspClockInTime = HoraInicio.ToString().Replace(",",":");
                    drU.DspClockOutTime = HoraFinal.ToString().Replace(",", ":");
                    laborImpl.GetDspClockTime(HoraInicio, out vMessage);
                    drU = (LaborDataSet.LaborDtlRow)laborDataSet.LaborDtl.Rows[laborDataSet.LaborDtl.Rows.Count - 1];
                    drU.SubmittedBy = sess.EmployeeID;
                    drU.ClockinTime = HoraInicio;
                    drU.ClockOutTime = HoraFinal;
                    drU.DspClockInTime = HoraInicio.ToString().Replace(",", ":");
                    drU.DspClockOutTime = HoraFinal.ToString().Replace(",", ":");
                    laborImpl.DefaultDtlTime(laborDataSet);
                    laborImpl.Update(laborDataSet);
                    drU = (LaborDataSet.LaborDtlRow)laborDataSet.LaborDtl.Rows[laborDataSet.LaborDtl.Rows.Count - 1];
                    drU.SubmittedBy = sess.EmployeeID;
                    drU.LaborQty = 1;
                    laborImpl.DefaultLaborQty(laborDataSet, decimal.One, out vMessage);
                    laborImpl.CheckWarnings(laborDataSet, out vMessage);
                    drU = (LaborDataSet.LaborDtlRow)laborDataSet.LaborDtl.Rows[laborDataSet.LaborDtl.Rows.Count - 1];
                    drU.TimeStatus = "E";
                    drU.RowMod = "U";
                    if (!string.IsNullOrEmpty(enT.LaborNote))
                        drU.LaborNote = enT.LaborNote;
                    laborImpl.Update(laborDataSet);
                    drU = (LaborDataSet.LaborDtlRow)laborDataSet.LaborDtl.Rows[laborDataSet.LaborDtl.Rows.Count - 1];
                    drU.TimeStatus = "E";
                    drU.RowMod = "U";
                    if (!string.IsNullOrEmpty(enT.LaborNote))
                        drU.LaborNote = enT.LaborNote;
                    laborImpl.ValidateChargeRateForTimeType(laborDataSet, out vMessage);
                    drU = (LaborDataSet.LaborDtlRow)laborDataSet.LaborDtl.Rows[laborDataSet.LaborDtl.Rows.Count - 1];
                    drU.ActiveTaskID = "SuprAppr";
                    drU.TimeStatus = "S";
                    drU.RowMod = "U";
                    if (!string.IsNullOrEmpty(enT.LaborNote))
                        drU.LaborNote = enT.LaborNote;
                    laborImpl.Update(laborDataSet);
                    laborImpl.SubmitForApproval(laborDataSet, false, out vMessage);
                    entidadesReportTime.ReportCreate = true;
                    entidadesReportTime.Msj = "Operación creada con éxito";
                }
            }
            catch (Exception ex)
            {
                entidadesReportTime.Msj = ex.Message + "\nExc: " + ex.InnerException;
            }
            return entidadesReportTime;
        }
    }
}