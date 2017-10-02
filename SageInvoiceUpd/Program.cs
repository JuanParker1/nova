using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Xml;
using System.Xml.Linq;
namespace SageInvoiceUpd
{
    class Program
    {
        static void Main(string[] args)
        {
           /* SqlConnection con = new SqlConnection();
            con.ConnectionString = @"Data Source=192.168.0.28\SVR2012;Initial Catalog=SOCOMARTESTING; User id=sa; Password=Passw0rd2012;";
            con.Open();
            SqlCommand cmd = new SqlCommand("select * from F_ECRITUREC where ", con);
            SqlDataReader dr = null;
            dr = cmd.ExecuteReader();
            */
            XElement xdocument = XElement.Load("H:\\GAR0617-36608064880.xml");
            var lst = (from el in xdocument.Descendants("Bol_segment") select el).ToList();
            List<Bol_segment> lbol = new List<Bol_segment>();

            foreach (var el in lst)
            {

                lbol.Add(new Bol_segment { Bol_reference=(string)(from item in el.Descendants("Bol_reference") select item).First(),
                                           Line_number = (string)(from item in el.Descendants("Line_number") select item).First(),
                                           Place_of_loading_code = (string)(from item in el.Descendants("Place_of_loading_code") select item).First(),
                                           Place_of_unloading_code = (string)(from item in el.Descendants("Place_of_unloading_code") select item).First()
                });
            
            }

            //recherche du noeud d'un BL
            Bol_segment bs = new Bol_segment(); Exporter exporter = new Exporter(); Consignee consignee = new Consignee();
            Notify notify = new Notify();
            foreach (var el in lst)
            {
                if ((string)(from item in el.Descendants("Line_number") select item).First() == "100")
                { 
                  bs.Bol_reference=(string)(from item in el.Descendants("Bol_reference") select item).First();
                  bs.Line_number = (string)(from item in el.Descendants("Line_number") select item).First();
                  bs.Place_of_loading_code = (string)(from item in el.Descendants("Place_of_loading_code") select item).First();
                  bs.Place_of_unloading_code = (string)(from item in el.Descendants("Place_of_unloading_code") select item).First();
                  bs.Bol_nature=(string)(from item in el.Descendants("Bol_nature") select item).First();
                  bs.Location_code=(string)(from item in el.Descendants("Location_code") select item).First();
                 //exporter
                  exporter.Exporter_address1 = (string)(from item in el.Descendants("Exporter_address1") select item).First();
                  exporter.Exporter_address2 = (string)(from item in el.Descendants("Exporter_address2") select item).First();
                  exporter.Exporter_address3 = (string)(from item in el.Descendants("Exporter_address3") select item).First();
                  exporter.Exporter_address4 = (string)(from item in el.Descendants("Exporter_address4") select item).First();
                  exporter.Exporter_name = (string)(from item in el.Descendants("Exporter_name") select item).First();
                    //consignee
                  consignee.Consignee_address1 = (string)(from item in el.Descendants("Consignee_address1") select item).First();
                  consignee.Consignee_address2 = (string)(from item in el.Descendants("Consignee_address2") select item).First(); ;
                  consignee.Consignee_address3 = (string)(from item in el.Descendants("Consignee_address3") select item).First();
                  consignee.Consignee_address4 = (string)(from item in el.Descendants("Consignee_address4") select item).First();
                  consignee.Consignee_name = (string)(from item in el.Descendants("Consignee_name") select item).First();
                    //notify
                  notify.Notify_address1 = (string)(from item in el.Descendants("Notify_address1") select item).First();
                  notify.Notify_address2 = (string)(from item in el.Descendants("Notify_address2") select item).First();
                  notify.Notify_address3 = (string)(from item in el.Descendants("Notify_address3") select item).First();
                  notify.Notify_address4 = (string)(from item in el.Descendants("Notify_address4") select item).First();
                  notify.Notify_name = (string)(from item in el.Descendants("Notify_name") select item).First();

                }
                 
            }
        }
    }

    public class Bol_segment
    {
        public string Bol_reference { get; set; }
        public string Line_number { get; set; }
        public string Place_of_loading_code { get; set; }
        public string Place_of_unloading_code { get; set; }
        public string Bol_nature { get; set; }
        public string Location_code { get; set; }
    }
    public class Exporter
    {
        public string Exporter_name { get; set; } public string Exporter_address1 { get; set; }
        public string Exporter_address2 { get; set; }public string Exporter_address3 { get; set; }
        public string Exporter_address4 { get; set; }
    }

    public class Consignee
    {
        public string Consignee_name { get; set; } public string Consignee_address1 { get; set; }
        public string Consignee_address2 { get; set; }public string Consignee_address3 { get; set; }
        public string Consignee_address4 { get; set; }
    }

    public class Notify
    {
        public string Notify_name { get; set; } public string Notify_address1 { get; set; }
        public string Notify_address2 { get; set; }public string Notify_address3 { get; set; }
        public string Notify_address4 { get; set; }
    }
}
