using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BTWSTMovil.BO
{
    public class CNNSql
    {
        protected string getCustShipToServiceID(string Company,string JobNum)
        {
            return @"DECLARE @OrderNum int = 0
            DECLARE @CallNum int = 0
            DECLARE @Company varchar(20)
            DECLARE @Table Table (IDServicio int,CustNum int,CustName varchar(50),ShipToNum varchar(30),ShipToName varchar(50),[Address] varchar(50),PhoneNum varchar(50),Latitud decimal,Longitud decimal,Observation varchar(100))

            select 
            @Company = Company,
            @OrderNum = OrderNum,
            @CallNum = CallNum " +
            "from Erp.JobProd WHERE Company = '"+Company+"' AND JobNum = '"+JobNum+"' " +
            @"if(@CallNum > 0)
            BEGIN
	            INSERT INTO @Table
	            SELECT F.ServicioId_c,F.CustNum,C.Name,F.ShipToNum,S.Name,S.Address1,S.PhoneNum,0,0,'' 
	            FROM FSCallhd F
		            INNER JOIN Customer C ON C.Company = F.Company AND C.CustNum = F.CustNum
			            INNER JOIN ShipTo S ON S.Company = F.Company AND S.ShipToNum = F.ShipToNum AND S.CustNum = F.CustNum
				            WHERE F.Company = @Company AND F.CallNum = @CallNum
            END
            if(@OrderNum > 0)
            BEGIN
	            INSERT INTO @Table
	            SELECT O.IDServicio_c,O.CustNum,C.Name,S.ShipToNum,S.Name,S.Address1,S.PhoneNum,0,0,'' 
	            FROM OrderDtl O
		            INNER JOIN Ice.UD22 U ON U.Company = O.Company AND U.Key1 = O.IDServicio_c  AND U.Key2 = O.QuoteNum
			            INNER JOIN Customer C ON C.Company = O.Company AND C.CustNum = O.CustNum
				            INNER JOIN ShipTo S ON S.Company = O.Company AND S.ShipToNum = U.Key4 AND S.CustNum = O.CustNum
					            WHERE O.Company = @Company AND O.OrderNum = @OrderNum
            END
            SELECT * FROM @Table
            DELETE FROM @Table";
        }
        protected string getAttrShipToNum(string JobNum)
        {
            return @"DECLARE @OrderNum int = 0
                    DECLARE @CallNum int = 0
                    DECLARE @Company varchar(20)
                    DECLARE @Table Table (IDServicio int,Description varchar(50),Valor varchar(50))

                    select 
                    @Company = Company,
                    @OrderNum = OrderNum,
                    @CallNum = CallNum " +
                    "from Erp.JobProd WHERE JobNum = '"+JobNum+"' " +
                    @"if(@CallNum > 0)
                    BEGIN
	                    INSERT INTO @Table
	                    SELECT F.ServicioId_c,U.Character01,U.Character02
	                    FROM FSCallhd F
		                    INNER JOIN Ice.UD17 U ON U.Company = F.Company AND U.Number20 = F.ServicioId_c
			                    AND U.Key4 = F.ShipToNum
				                    WHERE F.Company = @Company AND F.CallNum = @CallNum
                    END
                    if(@OrderNum > 0)
                    BEGIN
	                    INSERT INTO @Table
	                    SELECT O.IDServicio_c,U.Character01,U.Character02
	                    FROM OrderDtl O
		                    INNER JOIN Ice.UD23 U ON U.Company = O.Company AND U.Number20 = O.IDServicio_c  AND U.Key2 = O.QuoteNum
					                    WHERE O.Company = @Company AND O.OrderNum = @OrderNum
                    END
                    SELECT * FROM @Table
                    DELETE FROM @Table";
        }
    }
}