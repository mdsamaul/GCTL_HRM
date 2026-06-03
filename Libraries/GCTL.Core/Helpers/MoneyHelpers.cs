using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.Helpers
{
    public static class MoneyHelpers
    {
        public static void Convert()
        {

   //         numbervar RmVal:= 0;
   //         numbervar Amt:= 0;
   //         numbervar pAmt:= 0;
   //         stringvar InWords := "BDT ";

   //     Amt:= ({ MVCPosApp_Model_CustomerBillReport.NetAmount}) ;


   //         if Amt > 10000000 then RmVal := truncate(Amt / 10000000);
   //         if Amt = 10000000 then RmVal := 1;

   //         if RmVal = 1 then
   //              InWords := InWords + " " + towords(RmVal, 0) + " crore"
   //else
   //     if RmVal > 1 then InWords := InWords + " " + towords(RmVal, 0) + " crores";


   //     Amt:= Amt - (RmVal * 10000000);
   //     RmVal:= 0;

   //         if Amt > 100000 then RmVal := truncate(Amt / 100000);
   //         if Amt = 100000 then RmVal := 1;

   //         if RmVal = 1 then
   //             InWords := InWords + " " + towords(RmVal, 0) + " lac"
   // Else
   //     If RmVal > 1 then InWords := InWords + " " + ToWords(RmVal, 0) + " lacs";

   //     Amt:= Amt - Rmval * 100000;

   //         if Amt > 0 then InWords := InWords + " " + towords(truncate(Amt), 0);

   //     pAmt:= (Amt - truncate(Amt)) * 100;

   //         if pAmt > 0 then
   //             InWords := InWords + " and " + towords(pAmt, 0) + " paisa only."
   //     else
   //         InWords:= InWords + " only.";

        }
    }
}
