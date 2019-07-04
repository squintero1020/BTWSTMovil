using Erp.Adapters;
using Erp.BO;
using Erp.Proxy.BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BTWSTMovil.BO
{
    public class ReturnMaterial
    {
        readonly Globales.Functions fn = new Globales.Functions();
        protected Entidades.EntidadesReturnMaterial retunrMaterial(Entidades.EntidadesReturnMaterial enT)
        {
            Entidades.EntidadesReturnMaterial entidadesReturnMaterial = new Entidades.EntidadesReturnMaterial();
            try
            {
                entidadesReturnMaterial.DcdUserID = enT.DcdUserID;
                entidadesReturnMaterial.Pwd = "******";
                entidadesReturnMaterial.JobNum = enT.JobNum;
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
                    string vMes = string.Empty;
                    IssueReturnAdapter issueReturn = new IssueReturnAdapter(sess);
                    issueReturn.BOConnect();
                    IssueReturnImpl issue = issueReturn.BusinessObject as IssueReturnImpl;
                    IssueReturnDataSet dataSet = new IssueReturnDataSet();
                    Guid guid = new Guid("00000000-0000-0000-0000-000000000000");
                    /*OBJETOS E INSTANCIAS DE EPICOR*/
                    SelectedJobAsmblDataSet selectedJobAsmblDataSet = new SelectedJobAsmblDataSet();
                    SelectedJobAsmblDataSet.SelectedJobAsmblRow selectedJobAsmblRow = (SelectedJobAsmblDataSet.SelectedJobAsmblRow)selectedJobAsmblDataSet.SelectedJobAsmbl.NewRow();
                    selectedJobAsmblRow.Company = sess.CompanyID;
                    selectedJobAsmblRow.JobNum = enT.JobNum;
                    selectedJobAsmblRow.AssemblySeq = 0;
                    selectedJobAsmblDataSet.SelectedJobAsmbl.Rows.Add(selectedJobAsmblRow);
                    dataSet = issue.GetNewJobAsmblMultiple("MTL-STK", guid, "ReturnMaterial", selectedJobAsmblDataSet, out vMes);
                    issue.GetAvailTranDocTypes(out vMes);
                    var dRow = (IssueReturnDataSet.IssueReturnRow)dataSet.IssueReturn.Rows[(dataSet.IssueReturn.Rows.Count - 1)];
                    dRow.FromJobNum = enT.JobNum;
                    dRow.PartNum = enT.PartNum;
                    dRow.FromJobSeq = enT.FromJobSeq;
                    dRow.FromWarehouseCode = enT.FromWarehouseCode;
                    dRow.FromBinNum = enT.FromBinNum;
                    dRow.FromJobSeqPartNum = enT.FromJobSeqPartNum;
                    dRow.ToWarehouseCode = enT.ToWarehouseCode;
                    dRow.ToBinNum = enT.ToBinNum;
                    dRow.RowMod = "U";
                    issue.OnChangingJobSeq(enT.FromJobSeq, "From", "ReturnMaterial", dataSet);
                    dRow = (IssueReturnDataSet.IssueReturnRow)dataSet.IssueReturn.Rows[(dataSet.IssueReturn.Rows.Count - 1)];
                    dRow.FromJobNum = enT.JobNum;
                    dRow.PartNum = enT.PartNum;
                    dRow.FromJobSeq = enT.FromJobSeq;
                    dRow.FromWarehouseCode = enT.FromWarehouseCode;
                    dRow.FromBinNum = enT.FromBinNum;
                    dRow.FromJobSeqPartNum = enT.FromJobSeqPartNum;
                    dRow.ToWarehouseCode = enT.ToWarehouseCode;
                    dRow.ToBinNum = enT.ToBinNum;
                    dRow.RowMod = "U";
                    issue.OnChangeFromJobSeq(dataSet, "ReturnMaterial", out vMes);
                    dRow = (IssueReturnDataSet.IssueReturnRow)dataSet.IssueReturn.Rows[(dataSet.IssueReturn.Rows.Count - 1)];
                    dRow.FromJobNum = enT.JobNum;
                    dRow.PartNum = enT.PartNum;
                    dRow.FromJobSeq = enT.FromJobSeq;
                    dRow.FromWarehouseCode = enT.FromWarehouseCode;
                    dRow.FromBinNum = enT.FromBinNum;
                    dRow.FromJobSeqPartNum = enT.FromJobSeqPartNum;
                    dRow.ToWarehouseCode = enT.ToWarehouseCode;
                    dRow.ToBinNum = enT.ToBinNum;
                    dRow.RowMod = "U";
                    issue.OnChangeTranQty(enT.Qty, dataSet);
                    bool flag;
                    issue.PrePerformMaterialMovement(dataSet, out flag);
                    dRow = (IssueReturnDataSet.IssueReturnRow)dataSet.IssueReturn.Rows[(dataSet.IssueReturn.Rows.Count - 1)];
                    dRow.FromJobNum = enT.JobNum;
                    dRow.PartNum = enT.PartNum;
                    dRow.FromJobSeq = enT.FromJobSeq;
                    dRow.FromWarehouseCode = enT.FromWarehouseCode;
                    dRow.FromBinNum = enT.FromBinNum;
                    dRow.FromJobSeqPartNum = enT.FromJobSeqPartNum;
                    dRow.ToWarehouseCode = enT.ToWarehouseCode;
                    dRow.ToBinNum = enT.ToBinNum;
                    dRow.RowMod = "U";
                    issue.MasterInventoryBinTests(dataSet, out vMes, out vMes, out vMes, out vMes, out vMes, out vMes);
                    issue.PerformMaterialMovement(false, dataSet, out vMes, out vMes);
                    entidadesReturnMaterial.Msj = "Terminó la devolución del material " + entidadesReturnMaterial.PartNum + " para el trabajo: " + entidadesReturnMaterial.JobNum;
                }

            }
            catch (Exception ex)
            {
                entidadesReturnMaterial.Msj = ex.Message + "\nExc: " + ex.InnerException;
            }
            return entidadesReturnMaterial;
        }
    }
}