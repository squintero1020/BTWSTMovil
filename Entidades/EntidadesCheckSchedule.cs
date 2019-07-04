using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace BTWSTMovil.Entidades
{
    public class EntidadesCheckSchedule
    {
        public string DcdUserID { get; set; }
        public string Pwd { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DataSet dt { get; set; }
        public string Msj { get; set; }
    }
}