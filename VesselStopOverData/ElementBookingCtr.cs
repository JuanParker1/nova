using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VesselStopOverData
{
    public class ElementBookingCtr
    {
        public int IdCtr { get; set; }
        public string NumCtr { get; set; }
        public string Description { get; set; }
        public string UNCode { get; set; }
        public string DescMses { get; set; }
        public string TypeCtr { get; set; }
        public string StatutCtr { get; set; }
        public float Volume { get; set; }
        public int Poids { get; set; }
        public string TypeMsesCtr { get; set; }
        public string Seal1 { get; set; }
        public string Seal2 { get; set; }
        public string StatCtr { get; set; }
        //AH 7juin16 
        public int VGM { get; set; }
    }
}
