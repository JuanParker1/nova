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
    
    public partial class CLIENT
    {
        public int IdClient { get; set; }
        public string CodeClient { get; set; }
        public string NomClient { get; set; }
        public string AdrClient { get; set; }
        public string EmailClient { get; set; }
        public string TelClient { get; set; }
        public string CCClient { get; set; }
        public string CodeTVA { get; set; }
        public string TypeClient { get; set; }
        public string StatutClient { get; set; }
        public string NumContrib { get; set; }
        public string NumRegCommerce { get; set; }
        public string RetenueIS { get; set; }
        public Nullable<double> PourcentIS { get; set; }
    
        public virtual CODE_TVA CODE_TVA { get; set; }
    }
}