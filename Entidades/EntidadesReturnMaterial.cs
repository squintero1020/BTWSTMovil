using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BTWSTMovil.Entidades
{
    public class EntidadesReturnMaterial
    {
        public string DcdUserID { get; set; }
        public string Pwd { get; set; }
        public string JobNum { get; set; }
        public string PartNum { get; set; }
        public decimal Qty { get; set; }
        public int FromJobSeq { get; set; }
        public string FromWarehouseCode { get; set; }
        public string FromBinNum { get; set; }
        public string FromJobSeqPartNum { get; set; }
        public string ToWarehouseCode { get; set; }
        public string ToBinNum { get; set; }
        public string Msj { get; set; }
    }
}