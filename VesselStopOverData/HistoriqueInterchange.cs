using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VesselStopOverData
{
    public class HistoriqueInterchange
    {
        public int Code { get; set; }
        public string Type { get; set; }
        public string Identification { get; set; }
        public string Sortie { get; set; }
        public string Retour { get; set; }
        public string Parquing { get; set; }
        public string SortieVide { get; set; }
        public string RetourPlein { get; set; }
        public string Embarquement { get; set; }
        public string RetourArmateur { get; set; }
    }
}
