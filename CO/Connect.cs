using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace BTWSTMovil.CO
{
    public class Connect:CNN.cnn
    {
        public string cmdEjecutar(string sql, out string Error)
        {
            return this.Ejecutar(sql, out Error);
        }
        public DataSet cmdConsultar(string sql,out string Error)
        {
            return this.consultar(sql, out Error);
        }
    }
}