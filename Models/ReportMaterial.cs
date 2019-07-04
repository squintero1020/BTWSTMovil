using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BTWSTMovil.Models
{
    public class ReportMaterial:BO.ReportMaterial
    {
        public Entidades.EntidadesReportMaterial material(Entidades.EntidadesReportMaterial enT)
        {
            return this.reportMaterial(enT);
        }
    }
}