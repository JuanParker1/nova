//------------------------------------------------------------------------------
// <auto-generated>
//    Ce code a été généré à partir d'un modèle.
//
//    Des modifications manuelles apportées à ce fichier peuvent conduire à un comportement inattendu de votre application.
//    Les modifications manuelles apportées à ce fichier sont remplacées si le code est régénéré.
// </auto-generated>
//------------------------------------------------------------------------------

namespace VesselStopOverDataEF
{
    using System;
    using System.Collections.Generic;
    
    public partial class BON_SORTIE
    {
        public int IdBS { get; set; }
        public Nullable<System.DateTime> DateBS { get; set; }
        public string AIBS { get; set; }
        public Nullable<int> IdBL { get; set; }
        public Nullable<int> IdVeh { get; set; }
        public Nullable<int> IdCtr { get; set; }
        public Nullable<int> IdGC { get; set; }
        public Nullable<int> IdU { get; set; }
    }
}