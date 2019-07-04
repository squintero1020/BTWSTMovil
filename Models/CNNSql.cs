using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BTWSTMovil.Models
{
    public class CNNSql:BO.CNNSql
    {
        public string GetCustomerShipTo(string Company, string JobNum)
        {
            return this.getCustShipToServiceID(Company, JobNum);
        }
        public string GetAttrShipTo(string JobNum)
        {
            return this.getAttrShipToNum(JobNum);
        }
    }
}