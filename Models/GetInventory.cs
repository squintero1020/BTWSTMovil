using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BTWSTMovil.Models
{
    public class GetInventory:BO.GetInventory
    {
        public Entidades.EntidadesGetInventory inventory(Entidades.EntidadesGetInventory enT)
        {
            return this.getInventory(enT);
        }
        public Entidades.EntidadesGetAtributos atributos(Entidades.EntidadesGetAtributos enT)
        {
            return this.getAttributes(enT);
        }
        public Entidades.EntidadesGetMaterial material(Entidades.EntidadesGetMaterial enT)
        {
            return this.getMaterial(enT);
        }
    }
}