using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VesselStopOverData
{
    public class EcritureJournal
    {
        public double batchNum { get; set; }
        public string refDate { get; set; }
        public string taxDate { get; set; }
        public string dueDate { get; set; }
        public string memo { get; set; }
        public string ref1 { get; set; }
        public string ref2 { get; set; }
        public string ref3 { get; set; }
        public List<SocSAPWS.LigneJournal> lignes { get; set; }
    }
}
