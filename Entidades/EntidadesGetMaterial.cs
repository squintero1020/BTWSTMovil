using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace BTWSTMovil.Entidades
{
    public class EntidadesGetMaterial
    {
        public string DcdUserID { get; set; }
        public string Pwd { get; set; }
        public string JobNum { get; set; }
        public DataTable dt { get; set; }
        public string Msj { get; set; }
    }
}