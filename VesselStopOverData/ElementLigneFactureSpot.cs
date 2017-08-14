using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VesselStopOverData
{
    public class ElementLigneFactureSpot
    {
        public string Code { get; set; }
        public string Libelle { get; set; }
        public float Qte { get; set; }
        public string Unite { get; set; }
        public double PrixUnitaire { get; set; }
        public double TVA { get; set; }
        public double HT { get; set; }
        public string CompteComptable { get; set; }
        public double PrixTotal { get; set; }
        public string Remarques { get; set; }
        public string CodeTVA { get; set; }
        public FAMILLE_ARTICLE FamilleArticl { get; set; }
        public ARTICLE Articl { get; set; }
        public ElementFacturation EltCompulsory { get; set; }
    }
}
