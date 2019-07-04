using Erp.Adapters;
using Erp.BO;
using Erp.Proxy.BO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace BTWSTMovil.BO
{
    public class GetInventory
    {
        readonly Globales.Functions fn = new Globales.Functions();
        readonly Models.CNNSql sql = new Models.CNNSql();
        readonly CO.Connect cn = new CO.Connect();

        protected Entidades.EntidadesGetInventory getInventory(Entidades.EntidadesGetInventory enT)
        {
            Entidades.EntidadesGetInventory entidadesGetInventory = new Entidades.EntidadesGetInventory();
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
                    EmpBasicAdapter empBasicAdapter = new EmpBasicAdapter(sess);
                    empBasicAdapter.BOConnect();
                    EmpBasicImpl empBasicImpl = empBasicAdapter.BusinessObject as EmpBasicImpl;
                    EmpBasicDataSet empBasicDataSet = new EmpBasicDataSet();
                    empBasicDataSet = empBasicImpl.GetByID(sess.EmployeeID);
                    var dRowEmp = (EmpBasicDataSet.EmpBasicRow)empBasicDataSet.EmpBasic.Rows[(empBasicDataSet.EmpBasic.Rows.Count - 1)];

                    entidadesGetInventory.DcdUserID = enT.DcdUserID;
                    entidadesGetInventory.Pwd = "******";
                    DataSet ds = new DataSet();
                    DataTable dTable = new DataTable();
                    /*SENTENCIA PARA OBTENER EL INVENTARIO ACTUAL DE LA BODEGA ASIGANDA AL EMPLEADO*/
                    string sql = fn.BuildWhere("Erp.PartBin PB INNER JOIN Erp.Part P ON P.Company = PB.Company AND P.PartNum = PB.PartNum INNER JOIN WhseBin WB ON WB.Company = PB.Company AND WB.BinNum = PB.BinNum"
                        , "P.PartNum,P.PartDescription,PB.OnhandQty,PB.LotNum", "WB.mc_Encargado_c = '"+sess.EmployeeID + "' AND WB.WarehouseCode <> ''", "P.PartNum");
                    string Error = string.Empty;
                    dTable = cn.cmdConsultar(sql, out Error).Tables["Tabla"].Copy();
                    if (!string.IsNullOrEmpty(Error))
                    {
                        entidadesGetInventory.Msj = Error;
                        return entidadesGetInventory;
                    }
                    if (dTable.Rows.Count > 0)
                    {
                        dTable.TableName = "partBin";
                        ds.Tables.Add(dTable);
                        /*SENTENCIA PARA OBTENER LOS SERIALES ACTUALES DE LA BODEGA ASIGNADA AL EMPLEADO*/
                        sql = fn.BuildWhere("Erp.SerialNo SN INNER JOIN WhseBin WB ON WB.Company = SN.Company AND WB.WarehouseCode = SN.WareHouseCode AND WB.BinNum = SN.BinNum",
                            "SN.PartNum,SN.LotNum,SN.SerialNumber", "(SNStatus = 'INVENTORY' OR (SNStatus = 'PICKED')) AND WB.mc_Encargado_c = '"+ dRowEmp["mc_Documento_c"].ToString() + "' AND WB.WarehouseCode <> ''", "");
                        Error = string.Empty;
                        dTable = cn.cmdConsultar(sql, out Error).Tables["Tabla"].Copy(); ;
                        if (!string.IsNullOrEmpty(Error))
                        {
                            entidadesGetInventory.Msj = Error;
                            return entidadesGetInventory;
                        }
                        if (dTable.Rows.Count > 0)
                        {
                            dTable.TableName = "serialNo";
                            ds.Tables.Add(dTable);
                        }
                        entidadesGetInventory.ds = ds;
                        entidadesGetInventory.Msj = "Se cargó el inventario";
                }
                sess.Dispose();
            }
            }
            catch (Exception ex)
            {
                entidadesGetInventory.Msj = ex.Message + "\nExc: " + ex.InnerException;
            }
            return entidadesGetInventory;
        }

        protected Entidades.EntidadesGetAtributos getAttributes(Entidades.EntidadesGetAtributos enT)
        {
            Entidades.EntidadesGetAtributos entidadesGetAtributos = new Entidades.EntidadesGetAtributos();
            try
            {
                entidadesGetAtributos.DcdUserID = enT.DcdUserID;
                entidadesGetAtributos.Pwd = "******";
                entidadesGetAtributos.JobNum = enT.JobNum;
                string Error = string.Empty;
                var dsAttr = cn.cmdConsultar(sql.GetAttrShipTo(enT.JobNum), out Error);
                if(!string.IsNullOrEmpty(Error))
                {
                    entidadesGetAtributos.Msj = Error;
                    return entidadesGetAtributos;
                }
                DataTable dTable = new DataTable();
                dTable = dsAttr.Tables["Tabla"].Copy();
                entidadesGetAtributos.dt = dTable;
                entidadesGetAtributos.Msj = "Se cargarón los atributos del servicio";
            }
            catch (Exception ex)
            {
                entidadesGetAtributos.Msj = ex.Message + "\nExc: " + ex.InnerException;
            }
            return entidadesGetAtributos;
        }
        protected Entidades.EntidadesGetMaterial getMaterial(Entidades.EntidadesGetMaterial enT)
        {
            Entidades.EntidadesGetMaterial entidadesGetMaterial = new Entidades.EntidadesGetMaterial();
            try
            {
                entidadesGetMaterial.DcdUserID = enT.DcdUserID;
                entidadesGetMaterial.Pwd = "******";
                entidadesGetMaterial.JobNum = enT.JobNum;
                var sqlStr = fn.BuildWhere("Erp.JobMtl", "JobComplete,IssuedComplete,MtlSeq,PartNum,Description,QtyPer,RequiredQty,IUM,RelatedOperation,IssuedQty", "JobNum = '" + enT.JobNum + "'", "");
                string Error = string.Empty;
                var dsMaterial = cn.cmdConsultar(sqlStr, out Error);
                if(!string.IsNullOrEmpty(Error))
                {
                    entidadesGetMaterial.Msj = Error;
                    return entidadesGetMaterial;
                }
                DataTable dTable = new DataTable();
                dTable = dsMaterial.Tables["Tabla"].Copy();
                entidadesGetMaterial.dt = dTable;
                entidadesGetMaterial.Msj = "Se cargarón los materiales del trabajo";
            }
            catch (Exception ex)
            {
                entidadesGetMaterial.Msj = ex.Message + "\nExc: " + ex.InnerException;
            }
            return entidadesGetMaterial;
        }
    }
}