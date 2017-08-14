using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VesselStopOverData
{
    public class ElementFacturation
    {
        public int IdElt { get; set; }
        public short CodeArticle { get; set; }
        public string LibArticle { get; set; }
        public double Qte { get; set; }
        public string Unite { get; set; }
        public double PrixUnitaire { get; set; }
        public double MontantHT { get; set; }
        public double MontantTVA { get; set; }
        public double MontantTTC { get; set; }
        public double MontantDIT { get; set; }
        public string Statut { get; set; }
        public bool IsProforma { get; set; }
        public bool IsFacture { get; set; }
        public bool IsNew { get; set; }
        public string Compte { get; set; }
    }
}
