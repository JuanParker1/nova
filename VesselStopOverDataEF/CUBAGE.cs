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
    
    public partial class CUBAGE
    {
        public CUBAGE()
        {
            this.CUBAGE_VEHICULE = new HashSet<CUBAGE_VEHICULE>();
            this.NOTE = new HashSet<NOTE>();
        }
    
        public int IdCubage { get; set; }
        public Nullable<System.DateTime> DateCubage { get; set; }
        public Nullable<System.DateTime> DateExCubage { get; set; }
        public Nullable<System.DateTime> DateCloCubage { get; set; }
        public string AICubage { get; set; }
        public Nullable<int> IdEsc { get; set; }
    
        public virtual ESCALE ESCALE { get; set; }
        public virtual ICollection<CUBAGE_VEHICULE> CUBAGE_VEHICULE { get; set; }
        public virtual ICollection<NOTE> NOTE { get; set; }
    }
}