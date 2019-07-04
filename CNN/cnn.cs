using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace BTWSTMovil.CNN
{
    public class cnn
    {
        private SqlConnection con;
        private SqlCommand cmd;

        protected string Ejecutar(string sentencia,out string Error)
        {
            Error = string.Empty;
            string msj = string.Empty;
            try
            {
                con = new SqlConnection(GenerarString());
                con.Open();
                cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = sentencia;
                //log.EscribirLOG("SQL: " + sentencia, false);
                cmd.ExecuteNonQuery();
                msj = "Operación Existosa!";
            }
            catch (Exception ex)
            {
                Error= ex.Message + "\n" + ex.InnerException;
                msj = "";
            }
            finally
            {
                con.Close();
            }
            return msj;
        }
        protected DataSet consultar(string sentencia,out string Error)
        {
            Error = string.Empty;
            DataSet ds = new DataSet();
            try
            {
                con = new SqlConnection(GenerarString());
                con.Open();
                var objRes = new SqlDataAdapter(sentencia, con);
                objRes.SelectCommand.CommandTimeout = 5000000;
                objRes.Fill(ds, "Tabla");
                return ds;
            }
            catch (Exception ex)
            {
                Error = ex.Message + "\n" + ex.InnerException;
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        string GenerarString()
        {
            return  "Data Source=" + ConfigurationManager.AppSettings["Server"].ToString() + ";Initial Catalog=" + ConfigurationManager.AppSettings["DB"].ToString() + ";User Id=" + ConfigurationManager.AppSettings["UserSQL"].ToString() + ";Password=" + ConfigurationManager.AppSettings["PassSQL"].ToString() + ";";
        }
    }
}