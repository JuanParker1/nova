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
        public string Chassis { get; set; }
        public string TvaCode { get; set; }
        public Single TvaTaux { get; set; }
        public string Statut { get; set; }
        public double Ht { get; set; }
        public double Tva { get; set; }
        public double MT { get; set; }
        public string Unit { get; set; }
        public string Ref { get; set; } //represente la reference du element_facturation 
    }

    public class QuotationInvoice
    {
        public string Ref { get; set; }
        public double HT { get; set; }
        public double TVA { get; set; }
        public double MT { get; set; }
        public List<InvoiceDetails> Lignes { get; set; }
        public List<InvoiceDetails> OLignes { get; set; }
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