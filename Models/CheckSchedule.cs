using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BTWSTMovil.Models
{
    public class CheckSchedule : BO.CheckScheduling
    {
        public Entidades.EntidadesCheckSchedule CheckJobSchedule(Entidades.EntidadesCheckSchedule ent)
        {
            return this.checkSchedule(ent);
        }
           
    }
}