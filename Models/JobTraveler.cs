using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BTWSTMovil.Models
{
    public class JobTraveler:BO.JobTraveler
    {
        public Entidades.EntidadesJobTraveler printJob(Entidades.EntidadesJobTraveler entidades)
        {
            return this.printJobTraveler(entidades);
        }
    }
}