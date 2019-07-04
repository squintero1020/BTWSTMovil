using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ice.Adapters;
using Ice.Proxy.BO;
using Ice.BO;
using Ice.Lib.Framework;
using Ice.Core;
using System.IO;

namespace BTWSTMovil.BO
{
    public class JobTraveler
    {
        readonly Globales.Functions fn = new Globales.Functions();
        protected Entidades.EntidadesJobTraveler printJobTraveler(Entidades.EntidadesJobTraveler enT)
        {
            Entidades.EntidadesJobTraveler entidadesJobTraveler = new Entidades.EntidadesJobTraveler();
            string agentID = "";

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
                    string workStation = Ice.Lib.Report.EpiReportFunctions.GetWorkStationID(sess);
                    /*OBTENGO EL NOMBRE DEL AGENTE DEL SISTEMA*/
                    using (var aSA = new SysAgentAdapter(sess))
                    {
                        aSA.BOConnect();
                        aSA.GetDefaultTaskAgentID(out agentID);
                        if (!string.IsNullOrEmpty(agentID)) { agentID = "SystemAgent"; }
                    }

                    {
                        var baqR = WCFServiceSupport.CreateImpl<Ice.Proxy.Rpt.BAQReportImpl>(sess, Epicor.ServiceModel.Channels.ImplBase<Ice.Contracts.BAQReportSvcContract>.UriPath);
                        var dynamicReport = WCFServiceSupport.CreateImpl<Ice.Proxy.BO.DynamicReportImpl>(sess, Epicor.ServiceModel.Channels.ImplBase<Ice.Contracts.DynamicReportSvcContract>.UriPath);
                        var rptMonitor = WCFServiceSupport.CreateImpl<Ice.Proxy.BO.ReportMonitorImpl>(sess, Epicor.ServiceModel.Channels.ImplBase<Ice.Contracts.ReportMonitorSvcContract>.UriPath);


                        entidadesJobTraveler.DcdUserID = enT.DcdUserID;
                        entidadesJobTraveler.Pwd = "********";
                        entidadesJobTraveler.JobNum = enT.JobNum;

                        // ENVIÓ PARAMETROS POR DEFECTO DEL REPORTE.
                        var rptDs = dynamicReport.GetByID("RPT_Traveler");
                        rptDs.BAQRptOptionFld[0].FieldValue = enT.JobNum;
                        rptDs.BAQReport[0].Description = enT.JobNum;
                        var baqRptDS = baqR.GetNewBAQReportParam("RPT_Traveler");

                        baqRptDS.BAQReportParam[0].AutoAction = "SSRSGenerate";
                        baqRptDS.BAQReportParam[0].PrinterName = "";
                        baqRptDS.BAQReportParam[0].RptPageSettings = "Color=False,Landscape=True,Margins=[Left=0 Right=0 Top=0 Bottom=0],PaperSize=[Kind='Letter' PaperName='Letter' Height=1100 Width=850],PaperSource=[SourceName='Auto Select' Kind='AutomaticFeed'],PrinterResolution=[Kind='Custom' X=600 Y=600]";
                        baqRptDS.BAQReportParam[0].RptPrinterSettings = "";
                        baqRptDS.BAQReportParam[0].PrintReportParameters = false;
                        baqRptDS.BAQReportParam[0].WorkstationID = workStation;
                        baqRptDS.BAQReportParam[0].SSRSRenderFormat = "PDF";
                        baqRptDS.BAQReportParam[0].Character01 = "PrintPrev";
                        baqRptDS.BAQReportParam[0].Option01 = enT.JobNum;

                        baqRptDS.BAQReportParam[0].BAQRptID = ("BTW_REPORTTraveler");
                        baqRptDS.BAQReportParam[0].ReportID = ("RPT_Traveler");
                        baqRptDS.BAQReportParam[0].Summary = false;
                        baqRptDS.BAQReportParam[0].ReportStyleNum = 1;
                        baqRptDS.BAQReportParam[0].BAQID = ("BTW_REPORTTraveler");
                        baqRptDS.BAQReportParam[0].ReportTitle = enT.JobNum;

                        rptDs.AcceptChanges();
                        StringWriter writer = new StringWriter();
                        rptDs.WriteXml(writer);
                        baqRptDS.BAQReportParam[0].Filter1 = writer.ToString();

                        baqR.SubmitToAgent(baqRptDS, agentID, 0, 0, "Ice.UIRpt.BAQReport;RPT_Traveler");

                        var reportM = WCFServiceSupport.CreateImpl<Ice.Proxy.BO.ReportMonitorImpl>(sess, Epicor.ServiceModel.Channels.ImplBase<Ice.Contracts.ReportMonitorSvcContract>.UriPath);
                        ReportMonitorDataSet dsReport = new ReportMonitorDataSet();
                        bool MorePages;
                        bool print = false;
                        while(!print)
                        {
                            /*CONSULTO CADA 5 SEGUNDOS, SI LA TAREA YA ESTA LISTA PARA SER PROCESADA*/
                            System.Threading.Thread.Sleep(5000);
                            string Where = "PrintDriver='SSRS' AND LastAction = 'None' AND WorkStationID='" + workStation + "'";
                            dsReport = reportM.GetRowsKeepIdleTime(Where, 0,1,out MorePages);
                            if(dsReport.SysRptLst.Count > 0)
                            {
                                /*PREGUNTO POR EL ULTIMO REGISTRO DISPONIBLE PARA PROCESAR*/
                                var dr = (ReportMonitorDataSet.SysRptLstRow)dsReport.SysRptLst.Rows[dsReport.SysRptLst.Rows.Count - 1];
                                var bytes = reportM.GetReportBytes(dr.SysRowID);
                                /*CREO LA CARPETA*/
                                string Carpeta = System.AppDomain.CurrentDomain.BaseDirectory + @"\Reports";
                                string Ruta = Carpeta + @"\" + enT.JobNum + ".pdf";
                                if (!Directory.Exists(Carpeta))
                                    Directory.CreateDirectory(Carpeta);
                                /*SI EL PDF YA EXISTE, LO ELIMINÓ*/
                                if (File.Exists(Ruta))
                                    File.Delete(Ruta);
                                /*GUARDO EL PDF EN LA RUTA*/
                                File.WriteAllBytes(Ruta, bytes);
                                /*CAMBIO EL ESTADO DE LA TAREA EN EPICOR*/
                                dsReport.SysRptLst[0].LastAction = "PRINT";
                                dsReport.SysRptLst[0].LastActionOn = DateTime.Now;
                                reportM.Update(dsReport);
                                print = true;
                                entidadesJobTraveler.Msj = "Se creó el PDF en " + Ruta;
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                entidadesJobTraveler.Msj="Ocurrió un error imprimiendo el reporte" + ex.Message + "\nExc: " + ex.InnerException;
            }
            return entidadesJobTraveler;
        }
    }
}