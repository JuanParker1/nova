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
    
    public partial class MANIFESTE
    {
        public MANIFESTE()
        {
            this.NOTE = new HashSet<NOTE>();
        }
    
        public int IdMan { get; set; }
        public Nullable<short> NPBLMan { get; set; }
        public Nullable<short> NPVMan { get; set; }
        public Nullable<short> NPCMan { get; set; }
        public Nullable<short> NPMMan { get; set; }
        public Nullable<short> NPGCMan { get; set; }
        public Nullable<System.DateTime> DCMan { get; set; }
        public Nullable<System.DateTime> DVMan { get; set; }
        public string AIMan { get; set; }
        public Nullable<short> FormatMan { get; set; }
        public string CAFMan { get; set; }
        public string StatutMan { get; set; }
        public Nullable<int> IdEsc { get; set; }
        public string CodePort { get; set; }
        public string C70616 { get; set; }
    
        public virtual ESCALE ESCALE { get; set; }
        public virtual ICollection<NOTE> NOTE { get; set; }
    }
}