using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VesselStopOverData
{
   public class Psion : SuperClass
    {
       VSOMClassesDataContext dcPsion = new VSOMClassesDataContext();
       public Psion()
           : base()
       { 
       
       }

       public string Sortie(string txt)
       {
           using (var transaction = new System.Transactions.TransactionScope())
           {
           PSION_SORTIES ps = new PSION_SORTIES();
           ps.CHASSIS = "123456789";
           ps.SCAN = DateTime.Now;
               dcPsion.GetTable<PSION_SORTIES>().InsertOnSubmit(ps);
               dcPsion.SubmitChanges();
               transaction.Complete();
               return "ok";
           }
       }
    }

}
