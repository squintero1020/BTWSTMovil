using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace BTWSTMovil.Entidades
{
    public class EntidadesGetInventory
    {
        public string DcdUserID { get; set; }
        public string Pwd { get; set; }
        public DataSet ds { get; set; }
        public string Msj { get; set; }
  
    }
}