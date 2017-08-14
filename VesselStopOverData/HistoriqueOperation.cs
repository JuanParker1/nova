using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VesselStopOverData
{
    public class HistoriqueOperation
    {
        public string Operation { get; set; }
        public DateTime DateOperation { get; set; }
        public string LieuOperation { get; set; }
        public string ExecutePar { get; set; }
        public string Commentaires { get; set; }
    }
}
