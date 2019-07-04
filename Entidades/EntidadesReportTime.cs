using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BTWSTMovil.Entidades
{
    public class EntidadesReportTime
    {
        public string DcdUserID { get; set; }
        public string Pwd { get; set; }
        public string JobNum { get; set; }
        public int oprSeq { get; set; }
        public bool LaborType { get; set; }
        public string IndirectCode { get; set; }
        public string ExpenseCode { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string LaborNote { get; set; }
        public bool ReportCreate { get; set; }
        public string Msj { get; set; }
    }
}