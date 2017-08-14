using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VesselStopOverData
{
    public class FacturePaiementClient
    {
        public string NumElmt { get; set; }
        public string BL { get; set; }
        public int idBL { get; set; }
        public string Escale { get; set; }
        public FACTURE Facture { get; set; }
        public AVOIR Avoir { get; set; }
        public int MHT { get; set; }
        public int TVA { get; set; }
        public int MTTC { get; set; }
        public DateTime Date { get; set; }
    }
}
