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
    
    public partial class FAMILLE_ARTICLE
    {
        public FAMILLE_ARTICLE()
        {
            this.ARTICLE = new HashSet<ARTICLE>();
        }
    
        public short CodeFamArt { get; set; }
        public string LibFamArt { get; set; }
        public string Niveau { get; set; }
    
        public virtual ICollection<ARTICLE> ARTICLE { get; set; }
    }
}