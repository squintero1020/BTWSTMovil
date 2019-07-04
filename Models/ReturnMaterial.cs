using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BTWSTMovil.Models
{
    public class ReturnMaterial:BO.ReturnMaterial
    {
        public Entidades.EntidadesReturnMaterial rMaterial(Entidades.EntidadesReturnMaterial enT)
        {
            return this.retunrMaterial(enT);
        }
    }
}