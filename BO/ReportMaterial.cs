using System;
using Erp.Adapters;
using Erp.BO;
using Erp.Proxy.BO;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

namespace BTWSTMovil.BO
{
    public class ReportMaterial
    {
        readonly Globales.Functions fn = new Globales.Functions();
        protected Entidades.EntidadesReportMaterial reportMaterial(Entidades.EntidadesReportMaterial enT)
        {
            Entidades.EntidadesReportMaterial rMaterial = new Entidades.EntidadesReportMaterial();
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
                    rMaterial.DcdUserID = enT.DcdUserID;
                    rMaterial.Pwd = enT.Pwd;
                    rMaterial.JobNum = enT.JobNum;
                    /*OBJETOS E INSTANCIAS DE EPICOR*/
                    JobEntryAdapter jobEntry = new JobEntryAdapter(sess);
                    jobEntry.BOConnect();
                    JobEntryImpl job = jobEntry.BusinessObject as JobEntryImpl;
                    JobEntryDataSet dsJob = new JobEntryDataSet();
                    dsJob = job.GetByID(enT.JobNum);
                    /**/
                    var dRow = (JobEntryDataSet.JobHeadRow)dsJob.JobHead.Rows[(dsJob.JobHead.Rows.Count -1)];
                    dRow.JobReleased = false;
                    dRow.JobEngineered = false;
                    job.Update(dsJob);
                    /**/
                    int i = 0,initialRow =0;
                    foreach (DataRow dr in enT.dMaterial.Rows)
                    {
                        /*SI EL LOTE TIENE ALGÚN DATO*/
                        if (!string.IsNullOrEmpty(dr["LotNum"].ToString()))
                            /*PREGUNTO SI DICHO LOTE Y SU PARTE, TIENEN INTENCIONALIDAD DE INVENTARIO*/
                            if (!intencionalidad(dr["PartNum"].ToString(), dr["LotNum"].ToString()))
                            {
                                /*TRANSFERENCIA DE INVENTARIO SI EL LOTE NO TIENE INTENCIONALIDAD DE INVENTARIO*/
                                string Tran = string.Empty;
                                string pcNeqQtyAction = "";
                                string pcMessage = "";
                                int h = 0;
                                bool PreCommit = false;
                                string StrinOutput = "";

                                InvTransferAdapter aTrans = new InvTransferAdapter(sess); aTrans.BOConnect();
                                InvTransferImpl boTrans = aTrans.BusinessObject as InvTransferImpl;
                                InvTransferDataSet dsTrans = new InvTransferDataSet();
                                Guid gd = new Guid("00000000-0000-0000-0000-000000000000");
                                dsTrans = boTrans.GetTransferRecord(dr["PartNum"].ToString(),"");
                                var drTran = (InvTransferDataSet.InvTransRow)dsTrans.InvTrans.Rows[(dsTrans.InvTrans.Rows.Count -1)];

                                drTran.FromWarehouseCode = "";
                                drTran.ToWarehouseCode = "";
                                drTran.FromBinNum = "";
                                drTran.ToBinNum = "";

                                drTran.TransferQty      = Convert.ToDecimal(dr["Qty"]);
                                drTran.TrackingQty      = Convert.ToDecimal(dr["Qty"]);
                                drTran.ToLotNumber      = dr["LotNum"].ToString();
                                drTran.FromLotNumber    = dr["LotNum"].ToString();
                                drTran.RowMod = "U";


                                try
                                {
                                    boTrans.ChangeUOM(dsTrans);
                                    boTrans.ChangeLot(dsTrans);
                                    boTrans.ChangeToBin(dsTrans);
                                    //bool ret = false;
                                    boTrans.NegativeInventoryTest(dr["PartNum"].ToString(), "", "", "", "", "", 1, 1, out pcNeqQtyAction, out pcMessage);
                                    boTrans.PreCommitTransfer(dsTrans, out PreCommit);
                                    boTrans.NegativeInventoryTest(dr["PartNum"].ToString(), "", "", "", "", "", 1, 1, out pcNeqQtyAction, out pcMessage);
                                    if (!string.IsNullOrEmpty(dr["SerialNo"].ToString()))
                                    {
                                        boTrans.GetSelectSerialNumbersParams(dsTrans);
                                        SelectedSerialNumbersAdapter aSerial = new SelectedSerialNumbersAdapter(sess); aSerial.BOConnect();
                                        SelectedSerialNumbersImpl boSerial = aSerial.BusinessObject as SelectedSerialNumbersImpl;
                                        SelectedSerialNumbersDataSet dsSrial = new SelectedSerialNumbersDataSet();
                                        boSerial.GetSerialNumFormat(dsSrial, dr["PartNum"].ToString(), "", "", 0, sess.PlantID.ToString());
                                        string nextBaseSN = "";
                                        string SNPrefix = "";
                                        string NextFullSn = "";
                                        bool snCounterMax = false;
                                        boSerial.GetNextSN(dsSrial, dr["PartNum"].ToString(), "", "", 0, sess.PlantID.ToString(), out nextBaseSN, out SNPrefix, out NextFullSn, out snCounterMax);

                                        DataRow dRowSerie;
                                        dRowSerie = dsTrans.Tables["SelectedSerialNumbers"].NewRow();
                                        dRowSerie["Company"] = sess.CompanyID.ToString();
                                        dRowSerie["SerialNumber"] = dr["SerialNo"].ToString();
                                        dRowSerie["Scrapped"] = false;
                                        dRowSerie["Voided"] = false;
                                        dRowSerie["PartNum"] = dr["PartNum"].ToString();
                                        dRowSerie["PassedInspection"] = false;
                                        dRowSerie["Deselected"] = false;
                                        dRowSerie["KBLbrAction"] = 0;
                                        dRowSerie["PreventDeselect"] = false;
                                        dRowSerie["PreDeselected"] = false;
                                        dRowSerie["RowMod"] = "A";
                                        dsTrans.Tables["SelectedSerialNumbers"].Rows.Add(dRowSerie);
                                        LotSelectUpdateAdapter aLot = new LotSelectUpdateAdapter(sess); aLot.BOConnect();
                                        LotSelectUpdateImpl boLotUpdate = aLot.BusinessObject as LotSelectUpdateImpl;
                                        LotSelectUpdateDataSet dsLotUpdate = new LotSelectUpdateDataSet();
                                        bool needsLotAttrs = false;
                                        boLotUpdate.ChkForNeedsLotAttrs(dr["PartNum"].ToString(), dr["LotNum"].ToString(),"", out needsLotAttrs);

                                        aLot.Dispose();
                                        boLotUpdate.Dispose();
                                        //
                                        aSerial.Dispose();
                                        boSerial.Dispose();
                                        //
                                    }
                                    boTrans.NegativeInventoryTest(dr["PartNum"].ToString(), "", "", "", "", "", 1, 1, out pcNeqQtyAction, out pcMessage);
                                    boTrans.CommitTransfer(dsTrans, out Tran, out StrinOutput);
                                    continue;
                                }
                                catch { }
                            }
                        /*EMISIÖN DE MATERIAL SI EL LOTE TIENE INTENCIONALIDAD DE INVENTARIO*/

                        bool ExisteMatl = false;
                        foreach(JobEntryDataSet.JobMtlRow drMtl in dsJob.JobMtl)
                        {
                            if(drMtl.PartNum == dr["PartNum"].ToString())
                            {
                                ExisteMatl = true;
                                break;
                            }
                        }
                        if (!ExisteMatl)
                        {
                            job.GetNewJobMtl(dsJob, enT.JobNum, 0);
                            string PartNum = dr["PartNum"].ToString();
                            bool vSubAvail = false;
                            string vMsgType = string.Empty;
                            Guid sysRow = new Guid("00000000-0000-0000-0000-000000000000");
                            /*CAMBIO EL NÚMERO DE LA PARTE EN EPICOR*/
                            job.ChangeJobMtlPartNum(dsJob, true, ref PartNum, sysRow, "", "", out vMsgType, out vSubAvail, out vMsgType, out vSubAvail, out vSubAvail, out vMsgType);
                            /*ULTIMO MATERIAL CREADO*/
                            var dRowMtl = (JobEntryDataSet.JobMtlRow)dsJob.JobMtl.Rows[(dsJob.JobMtl.Rows.Count - 1)];
                            /*CAMBIO LA CANTIDAD Y LA DESCRIPCIÓN DE LA PARTE A EMITIR*/
                            dRowMtl.QtyPer = Convert.ToDecimal(dr["Qty"]);
                            dRowMtl.Description = dr["PartDesc"].ToString();
                            /*SI EL EL PRIMER MATERIAL, TOMO LA SECUENCIA INICIAL, PARA SABER EN QUE MATERIAL DEBO INICIAR LAS EMISIONES*/
                            if (i == 0)
                                initialRow = dRowMtl.MtlSeq;
                            /*MÉTODOS NECESARIOS DE EPICOR*/
                            job.ChangeJobMtlQtyPer(dsJob);
                            job.ChangeJobMtlEstSplitCosts(dsJob);
                            job.Update(dsJob);
                            i++;
                        }
                    }
                    /*CAMBIO EL ESTADO DEL TRABAJO A LIBERADO E INGENIERIA*/
                    dRow.JobReleased = true;
                    dRow.JobEngineered = true;
                    /*AGREGO UNA DESCRIPCIÓN DE CAMBIOS PARA EL TRABAJO*/
                    dRow.ChangeDescription = "Emisión de material por: " + sess.EmployeeID + "\nEl día: " + DateTime.Now.ToString("yyyy-MM-dd") + " a las " + DateTime.Now.ToShortTimeString() +
                        "\nEn la planta: " + sess.PlantName + " por medio de: BTWMovil";
                    job.Update(dsJob);
                    /*PROCESO DE EMISIÓN DE MATERIAL*/
                    string vMes = string.Empty;
                    bool requiresUserInput;
                    IssueReturnAdapter issueReturn = new IssueReturnAdapter(sess);
                    issueReturn.BOConnect();
                    IssueReturnImpl issue = issueReturn.BusinessObject as IssueReturnImpl;
                    IssueReturnDataSet dataSet = new IssueReturnDataSet();
                    Guid guid = new Guid("00000000-0000-0000-0000-000000000000");
                    /*DEFINIR LOS ALMÁCENES Y UBICACIONES QUE SE DEBEN USAR EN LOS MOVIMIENTOS DE INVENTARIO*/
                    string FromWarehouseCode    = "BASE";
                    string FromBinNum           = "A1";
                    string ToWarehouseCode      = "BASE";
                    string ToBinNum             = "A1";
                    int j = 0;
                    /*RECORRO LOS MATERIALES DEL TRABAJO, SIEMPRE Y CUANDO SEA MAYOR O IGUAL A LA SECUENCIA DE MATERIAL QUE ESTOY AGREGANDO*/
                    foreach (JobEntryDataSet.JobMtlRow dr in dsJob.JobMtl.Select("MtlSeq >= " + initialRow))
                    {
                        /*SI EL LOTE TIENE ALGÚN DATO*/
                        if (!string.IsNullOrEmpty(enT.dMaterial.Rows[j]["LotNum"].ToString()))
                            /*PREGUNTO SI DICHO LOTE Y SU PARTE, TIENEN INTENCIONALIDAD DE INVENTARIO*/
                            if (!intencionalidad(dr.PartNum,enT.dMaterial.Rows[j]["LotNum"].ToString()))
                            {
                                /*SI EL LOTE NO TIENE INTENCIONALIDAD DE INVENTARIO, NO REALIZO LA EMISIÓN*/
                                j++;
                                continue;
                            }
                        /*OBJETOS E INSTANCIAS DE EPICOR*/
                        SelectedJobAsmblDataSet selectedJobAsmblDataSet = new SelectedJobAsmblDataSet();
                        SelectedJobAsmblDataSet.SelectedJobAsmblRow selectedJobAsmblRow = (SelectedJobAsmblDataSet.SelectedJobAsmblRow)selectedJobAsmblDataSet.SelectedJobAsmbl.NewRow();
                        selectedJobAsmblRow.Company = sess.CompanyID;
                        selectedJobAsmblRow.JobNum = enT.JobNum;
                        selectedJobAsmblRow.AssemblySeq = 0;
                        selectedJobAsmblDataSet.SelectedJobAsmbl.Rows.Add(selectedJobAsmblRow);
                        dataSet = issue.GetNewJobAsmblMultiple("STK-MTL", guid, "IssueMaterial", selectedJobAsmblDataSet, out vMes);
                        issue.OnChangingToJobSeq(dr.MtlSeq, dataSet);
                        var dRowIssue = (IssueReturnDataSet.IssueReturnRow)dataSet.IssueReturn.Rows[(dataSet.IssueReturn.Rows.Count-1)];
                        dRowIssue.ToJobSeq = dr.MtlSeq;
                        dRowIssue.RowMod = "U";
                        issue.OnChangeToJobSeq(dataSet, "IssueMaterial", out vMes);
                        dRowIssue = (IssueReturnDataSet.IssueReturnRow)dataSet.IssueReturn.Rows[(dataSet.IssueReturn.Rows.Count - 1)];
                        dRowIssue.PartNum           = dr.PartNum;
                        dRowIssue.FromWarehouseCode = FromWarehouseCode;
                        dRowIssue.FromBinNum        = FromBinNum;
                        dRowIssue.ToWarehouseCode   = ToWarehouseCode;
                        dRowIssue.ToBinNum          = ToBinNum;
                        dRowIssue.ToJobSeqPartNum   = dr.PartNum;
                        dRowIssue.ToJobPartDesc     = dr.PartNumPartDescription;
                        dRowIssue.RowMod            = "U";
                        issue.OnChangeTranQty(dr.QtyPer, dataSet);
                        dRowIssue = (IssueReturnDataSet.IssueReturnRow)dataSet.IssueReturn.Rows[(dataSet.IssueReturn.Rows.Count - 1)];
                        dRowIssue.PartNum           = dr.PartNum;
                        dRowIssue.FromWarehouseCode = FromWarehouseCode;
                        dRowIssue.FromBinNum        = FromBinNum;
                        dRowIssue.ToWarehouseCode   = ToWarehouseCode;
                        dRowIssue.ToBinNum          = ToBinNum;
                        dRowIssue.ToJobSeqPartNum   = dr.PartNum;
                        dRowIssue.ToJobPartDesc     = dr.PartNumPartDescription;
                        dRowIssue.RowMod            = "U";
                        issue.OnChangeLotNum(enT.dMaterial.Rows[j]["LotNum"].ToString(), dataSet);
                        /*SI LA SERIE TIENE ALGÚN DATO*/
                        if(!string.IsNullOrEmpty(enT.dMaterial.Rows[j]["SerialNo"].ToString()))
                        {
                            /**SE CREA INSTANCIA DE SERIALES EN EL ADAPTADOR*/
                            issue.GetSelectSerialNumbersParams(dataSet);
                            SelectedSerialNumbersAdapter aSerial = new SelectedSerialNumbersAdapter(sess); aSerial.BOConnect();
                            SelectedSerialNumbersImpl boSerial = aSerial.BusinessObject as SelectedSerialNumbersImpl;
                            SelectedSerialNumbersDataSet dsSrial = new SelectedSerialNumbersDataSet();
                            boSerial.GetSerialNumFormat(dsSrial, dr.PartNum, "", "", 0, sess.PlantID.ToString());
                            /**/
                            string nextBaseSN = "";
                            string SNPrefix = "";
                            string NextFullSn = "";
                            bool snCounterMax = false;
                            boSerial.GetNextSN(dsSrial, dr.PartNum, "", "", 0, sess.PlantID.ToString(), out nextBaseSN, out SNPrefix, out NextFullSn, out snCounterMax);

                            DataRow dRowSerial;
                            dRowSerial = dataSet.Tables["SelectedSerialNumbers"].NewRow();
                            dRowSerial["Company"] = sess.CompanyID.ToString();
                            dRowSerial["SerialNumber"] = enT.dMaterial.Rows[j]["SerialNo"].ToString();
                            dRowSerial["Scrapped"] = false;
                            dRowSerial["Voided"] = false;
                            dRowSerial["PartNum"] = dr.PartNum;
                            dRowSerial["PassedInspection"] = false;
                            dRowSerial["Deselected"] = false;
                            dRowSerial["KBLbrAction"] = 0;
                            dRowSerial["PreventDeselect"] = false;
                            dRowSerial["PreDeselected"] = false;
                            dRowSerial["RowMod"] = "A";
                            /*SE AGREGA EL SERIAL, AL DATASET DE LA EMISIÓN DE MATERIALES*/
                            dataSet.Tables["SelectedSerialNumbers"].Rows.Add(dRowSerial);
                        }
                        issue.PrePerformMaterialMovement(dataSet, out requiresUserInput);
                        dRowIssue = (IssueReturnDataSet.IssueReturnRow)dataSet.IssueReturn.Rows[(dataSet.IssueReturn.Rows.Count - 1)];
                        dRowIssue.PartNum           = dr.PartNum;
                        dRowIssue.FromWarehouseCode = FromWarehouseCode;
                        dRowIssue.FromBinNum        = FromBinNum;
                        dRowIssue.ToWarehouseCode   = ToWarehouseCode;
                        dRowIssue.ToBinNum          = ToBinNum;
                        dRowIssue.ToJobSeqPartNum   = dr.PartNum;
                        dRowIssue.ToJobPartDesc     = dr.PartNumPartDescription;
                        dRowIssue.RowMod            = "U";
                        issue.MasterInventoryBinTests(dataSet, out vMes, out vMes, out vMes, out vMes, out vMes, out vMes);
                        dRowIssue = (IssueReturnDataSet.IssueReturnRow)dataSet.IssueReturn.Rows[(dataSet.IssueReturn.Rows.Count - 1)];
                        dRowIssue.PartNum           = dr.PartNum;
                        dRowIssue.FromWarehouseCode = FromWarehouseCode;
                        dRowIssue.FromBinNum        = FromBinNum;
                        dRowIssue.ToWarehouseCode   = ToWarehouseCode;
                        dRowIssue.ToBinNum          = ToBinNum;
                        dRowIssue.ToJobSeqPartNum   = dr.PartNum;
                        dRowIssue.ToJobPartDesc     = dr.PartNumPartDescription;
                        dRowIssue.RowMod            = "U";
                        issue.PerformMaterialMovement(false,dataSet,out vMes,out vMes);
                        j++;
                    }
                    /**/
                    rMaterial.Msj = "Se emitió el material al trabajo :" + enT.JobNum;
                }
            }
            catch (Exception ex)
            {
                rMaterial.Msj = ex.Message + "\nExc: " + ex.InnerException; 
            }
            return rMaterial;
        }
        bool intencionalidad(string PartNum,string lotNum)
        {
            bool pInten = false;
            try
            {
                /*SENTENCIA PARA PREGUNTAR POR LA INTENCIONALIDAD DE UNA PARTE Y LOTE*/
                string sql = fn.BuildWhere("PartLot", "InventarioIntencionalidad_c", "PartNum = '"+ PartNum + "' AND LotNum = '"+lotNum+"'","");
                CO.Connect cn = new CO.Connect();
                string error = string.Empty;
                var ds = cn.cmdConsultar(sql, out error);
                if (ds.Tables["Tabla"].Rows.Count > 0)
                    pInten = Convert.ToBoolean(ds.Tables[0].Rows[ds.Tables[0].Rows.Count - 1]["InventarioIntencionalidad_c"]);
            }
            catch 
            {
                
            }
            return pInten;
        }
    }
}