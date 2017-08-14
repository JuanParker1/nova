using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VesselStopOverData
{
    public class ElementCompteArmRegroup
    {
        public int Id { get; set; }
        public string Regroup { get; set; }
        public string Rubrique { get; set; }
        public double MontantHT { get; set; }
        public double MontantTVA { get; set; }
        public double MontantTTC { get; set; }
    }
}
