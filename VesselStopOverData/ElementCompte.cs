using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VesselStopOverData
{
    public class ElementCompte
    {
        public int Id { get; set; }
        public string Libelle { get; set; }
        public DateTime DateComptable { get; set; }
        public double Debit { get; set; }
        public double Credit { get; set; }
        public string TypeDoc { get; set; }
    }
}
