//------------------------------------------------------------------------------
// <auto-generated>
//    Ce code a été généré à partir d'un modèle.
//
//    Des modifications manuelles apportées à ce fichier peuvent conduire à un comportement inattendu de votre application.
//    Les modifications manuelles apportées à ce fichier sont remplacées si le code est régénéré.
// </auto-generated>
//------------------------------------------------------------------------------

namespace VSOM_API
{
    using System;
    using System.Collections.Generic;
    
    public partial class CONNAISSEMENT
    {
        public CONNAISSEMENT()
        {
            this.ELEMENT_FACTURATION = new HashSet<ELEMENT_FACTURATION>();
            this.VEHICULEs = new HashSet<VEHICULE>();
        }
    
        public int IdBL { get; set; }
        public string NumBL { get; set; }
        public string NumBooking { get; set; }
        public Nullable<System.DateTime> DCBL { get; set; }
        public string EtatBL { get; set; }
        public string LPBL { get; set; }
        public string DPBL { get; set; }
        public string LDBL { get; set; }
        public string BLGN { get; set; }
        public string BLIL { get; set; }
        public string BLSocar { get; set; }
        public string NumSocar { get; set; }
        public string BLFO { get; set; }
        public string BLLT { get; set; }
        public string CCBL { get; set; }
        public Nullable<int> CCBLMontant { get; set; }
        public string LPFret { get; set; }
        public string BlBloque { get; set; }
        public string BLER { get; set; }
        public string BLERNote { get; set; }
        public string TypeBL { get; set; }
        public string DescBL { get; set; }
        public string ConsigneeBL { get; set; }
        public string AdresseBL { get; set; }
        public string ConsigneeBooking { get; set; }
        public string AdresseConsignee { get; set; }
        public string AdresseNotify { get; set; }
        public string EmailNotify { get; set; }
        public string TelNotify { get; set; }
        public string NotifyBL { get; set; }
        public string NotifyBL2 { get; set; }
        public string AdresseNotify2 { get; set; }
        public string EmailNotify2 { get; set; }
        public string TelNotify2 { get; set; }
        public string NomCharger { get; set; }
        public string AdresseCharger { get; set; }
        public string StatutBL { get; set; }
        public Nullable<int> IdClient { get; set; }
        public Nullable<int> IdMan { get; set; }
        public Nullable<int> IdEsc { get; set; }
        public Nullable<int> IdAcc { get; set; }
        public string CodeTVA { get; set; }
        public string NomManBL { get; set; }
        public string CNIManBL { get; set; }
        public Nullable<System.DateTime> DDCNIManBL { get; set; }
        public string LDCNIManBL { get; set; }
        public string PhoneManBL { get; set; }
        public string EmailBL { get; set; }
        public string NContribBL { get; set; }
        public Nullable<System.DateTime> DateAccBL { get; set; }
        public string AIBL { get; set; }
        public string SensBL { get; set; }
        public Nullable<double> PoidsBL { get; set; }
        public Nullable<double> VolBL { get; set; }
        public string NumCtrBL { get; set; }
        public string NumDEBL { get; set; }
        public string NumBESCBL { get; set; }
        public string NumHSCode { get; set; }
        public Nullable<System.DateTime> DCBLI { get; set; }
        public Nullable<System.DateTime> DVCBLI { get; set; }
        public string AIVCBLI { get; set; }
        public Nullable<System.DateTime> DCLBL { get; set; }
        public string AIDCLBL { get; set; }
        public string BLDette { get; set; }
        public Nullable<int> DetteMontant { get; set; }
        public string BLBloqueNote { get; set; }
        public Nullable<System.DateTime> DVBL { get; set; }
        public string AIBLVal { get; set; }
        public string DestBL { get; set; }
        public Nullable<System.DateTime> LastModif { get; set; }
        public Nullable<int> IdU { get; set; }
        public string Payor { get; set; }
        public string ClearAgent { get; set; }
        public string NoSEPBC { get; set; }
    
        public virtual ICollection<ELEMENT_FACTURATION> ELEMENT_FACTURATION { get; set; }
        public virtual ICollection<VEHICULE> VEHICULEs { get; set; }
        public virtual CLIENT CLIENT { get; set; }
    }
}
