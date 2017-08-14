using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VesselStopOverData
{
    public class ElementLigneOS
    {
        public string Code { get; set; }
        public string Libelle { get; set; }
        public float Qte { get; set; }
        public string Unite { get; set; }
        public int PrixUnitaire { get; set; }
        public float TVA { get; set; }
        public string CompteComptable { get; set; }
        public float PrixTotal { get; set; }
        public string Remarques { get; set; }
        public string CodeTVA { get; set; }
    }
}
