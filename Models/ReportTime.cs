using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BTWSTMovil.Models
{
    public class ReportTime : BO.ReportTime
    {
        public Entidades.EntidadesReportTime ReportTimeTechnical(Entidades.EntidadesReportTime entidades)
        {
            return this.reportTime(entidades);
        }
    }
}