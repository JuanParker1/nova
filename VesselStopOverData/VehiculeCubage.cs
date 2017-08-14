using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VesselStopOverData
{
    public class VehiculeCubage
    {
        public int IdVeh { get; set; }
        public string NumChassis { get; set; }
        public string NumBL { get; set; }
        public string Description { get; set; }
        public double VolumeManifeste { get; set; }
        public float LongueurManifeste { get; set; }
        public float LargeurManifeste { get; set; }
        public float HauteurManifeste { get; set; }
        public double VolumeCube { get; set; }
        public float LongueurCube { get; set; }
        public float LargeurCube { get; set; }
        public float HauteurCube { get; set; }
        public DateTime? DateVal { get; set; }
        public string Etat { get; set; }
        public bool IsCubed { get; set; }
        public bool IsValidated { get; set; }
    }
}
