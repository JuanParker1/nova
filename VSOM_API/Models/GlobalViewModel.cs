using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace VSOM_API.Models
{
    public class GlobalViewModel
    {
        
    }
    public class QuotationViewModel
    {
        public string NumBl { get; set; }
        public DateTime Date { get; set; }
        public ICollection<Cars> Vehicules { get; set; }
    }
     
    public class BL
    {
        public string NumBl { get; set; }
        public string Consignee { get; set; }
        public string Notify { get; set; }
        public string Adresse { get; set; }
        public string CodeTva { get; set; }
        public string Statut { get; set; }
        public string FinFranchise { get; set; }
        public string NumMan { get; set; }
        public string Navire { get; set; }
        public ICollection<Cars> Vehicules { get; set; }
        public ICollection<InvoiceDetails> Lignes { get; set; }
        public string Lib { get; set; }
        public ICollection<Invoices> Invoices { get; set; }
        public ICollection<CreditNotes> CN { get; set; }
        public ICollection<Payments> Payments { get; set; }
    }
    

    public class Cars   
    {
        public string Chassis { get; set; }
        public string Description { get; set; }
        public string Statut { get; set; }
        //public string Type { get; set; }
        public double Vol { get; set; }
        public Single Longueur { get; set; } 
        public Single Largueur { get; set; }
        public string Porte { get; set; }
        public string Attelle { get; set; }
        public string Charge { get; set; }
        public string ChassisAP { get; set; }
        public DateTime FinFranchise { get; set; }
        public DateTime FinSejour { get; set; }
        public DateTime SortiePrevu { get; set; }
        //public DateTime SortieReal { get; set; }
        public ICollection<InvoiceDetails> Lignes { get; set; }
    }

    public class InvoiceDetails
    {
        public string Libelle { get; set; }
        public double Prix { get; set; }
        public double Qte { get; set; }
        public double Qty { get; set; }
        public string Chassis { get; set; }
        public string TvaCode { get; set; }
        public Single TvaTaux { get; set; }
        public string Statut { get; set; }
        public double Ht { get; set; }
        public double Tva { get; set; }
        public double MT { get; set; }
        public string Unit { get; set; }
        public string Ref { get; set; } //represente la reference du element_facturation 
        public string Code { get; set; }
        public string CodeCmpt { get; set; } // code comptable
        public int LP { get; set; } //id ligne de prix
        public int VEH { get; set; } //id veh
        public int ESC { get; set; } //id esc
        public int MAN { get; set; } //id man
        public double Coef { get; set; }//poid utiliser pour determinier le poid
    }

    public class QuotationInvoice
    {
        public string Ref { get; set; }
        public string HT { get; set; }
        public string TVA { get; set; }
        public string MT { get; set; }
        public List<InvoiceDetails> Lignes { get; set; }
        public List<InvoiceDetails> OLignes { get; set; }
        /// <summary>
        /// ligne de facture frais de dossier, manutention, prestation gestionnaire parc
        /// </summary>
        public List<InvoiceDetails> FMLignes { get; set; }
         public string FMHT { get; set; }
        public string FMTVA { get; set; }
        public string FMMT { get; set; }

        /// <summary>
        /// ligne sejour uniquement
        /// </summary>
        public List<InvoiceDetails> SLignes { get; set; }
        public string SLHT { get; set; }
        public string SLTVA { get; set; }
        public string SLMT { get; set; }
        /// <summary>
        /// reste des autres element qui par hazard non pas pu etre integre dans les deux categories precedent.
        /// </summary>
        public List<InvoiceDetails> DTLignes { get; set; }
        public string DTHT { get; set; }
        public string DTTVA { get; set; }
        public string DTMT { get; set; }

        public string ToXML()
        {
            var stringwriter = new System.IO.StringWriter();
            var serializer = new XmlSerializer(typeof(QuotationInvoice));
            serializer.Serialize(stringwriter, this);
            return stringwriter.ToString();
        }

    }

    public class Invoices 
    {
        public string Num { get; set; }
        public double HT { get; set; }
        public double TVA { get; set; }
        public double MTTC { get; set; }
        public string Statut { get; set; }
        public string Date { get; set; }
    }
    public class CreditNotes {
        public string Num { get; set; }
        public double HT { get; set; }
        public double TVA { get; set; }
        public double MTTC { get; set; }
        public string Statut { get; set; }
        public string Date { get; set; }
    }
    public class Payments {
        public string Num { get; set; } 
        public int MTTC { get; set; }
        public string Statut { get; set; }
        public string Date { get; set; }
    }

    public class Requests
    {
        public string date { get; set; }
        public string num { get; set; }
        public string description { get; set; }
        public string statut { get; set; }
    }
}