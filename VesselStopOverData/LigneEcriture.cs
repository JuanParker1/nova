using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VesselStopOverData
{
    public class LigneEcriture
    {
        public short CodeArticle { get; set; }
        public string AccountCode { get; set; }
        public double Qte { get; set; }
        public double PrixUnitaire { get; set; }
        public string CodeTVA { get; set; }
    }
}
