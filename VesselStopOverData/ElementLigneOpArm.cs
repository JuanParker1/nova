using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VesselStopOverData
{
    public class ElementLigneOpArm
    {
        public string StatutOp { get; set; }
        public int IdTypeOp { get; set; }
        public string Operation { get; set; }
        public double Qte { get; set; }
        public double Poids { get; set; }
        public double Volume { get; set; }
        public int PrixUnitaire { get; set; }
        public int PrixTotal { get; set; }
        public string Remarques { get; set; }
    }
}
