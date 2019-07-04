using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml;

namespace BTWSTMovil.Globales
{
    public class Functions
    {
        public void WriteErrorLog(string FileName, string Message)
        {
            try
            {
                StreamWriter streamWriter = new StreamWriter(FileName, true);
                streamWriter.AutoFlush = true;
                streamWriter.WriteLine("[" + DateTime.Now.ToString() + "]:" + Message);
                streamWriter.Flush();
                streamWriter.Close();
            }
            catch
            {
            }
        }
        public void ReadConfig(out string ConfigFile)
        {
            ConfigFile = System.AppDomain.CurrentDomain.BaseDirectory;
            XmlDocument objDocumento = new XmlDocument();
            XmlNode objNode;
            string strArchivoXml = System.AppDomain.CurrentDomain.BaseDirectory + @"\\Conexion.xml";
            objDocumento.Load(strArchivoXml);
            objNode = objDocumento.SelectSingleNode("//ConfigFile");
            ConfigFile += @"\" + objNode.InnerText;
        }
        public bool IsDate(string fecha)
        {
            try
            {
                DateTime.Parse(fecha);
                return true;
            }
            catch 
            {
                return false;
            }
        }
        public string BuildWhere(string Table, string Campos, string Where, string OrderBy)
        {
            string Strn = string.Empty;
            try
            {
                Strn = "SELECT ";
                if (!string.IsNullOrEmpty(Campos))
                    Strn += Campos + " ";
                else
                    Strn += " * ";
                Strn += " FROM " + Table + " ";
                if (!string.IsNullOrEmpty(Where))
                    Strn += "WHERE " + Where;
                if (!string.IsNullOrEmpty(OrderBy))
                    Strn += " ORDER BY " + OrderBy;
            }
            catch
            {

            }
            return Strn;
        }
    }
}