using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VesselStopOverData
{
    public class ElementBookingGC
    {
        public int IdGC { get; set; }
        public string NumGC { get; set; }
        public string Description { get; set; }
        public float Longueur { get; set; }
        public float Largeur { get; set; }
        public float Hauteur { get; set; }
        public short Quantite { get; set; }
        public double Volume { get; set; }
        public double Poids { get; set; }
        public string TypeMses { get; set; }
        public string StatGC { get; set; }
    }
}
