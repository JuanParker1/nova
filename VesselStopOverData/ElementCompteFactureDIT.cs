using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VesselStopOverData
{
    public class ElementCompteFactureDIT
    {
        public string NumFacture { get; set; }
        public DateTime DateFacturation { get; set; }
        public string NumBL { get; set; }
        public string ConsigneeBL { get; set; }
        public double MontantFacture { get; set; }
    }
}
