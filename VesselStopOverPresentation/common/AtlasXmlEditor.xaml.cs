using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml; 
using System.Xml.Linq;
namespace VesselStopOverPresentation.common
{

    #region class de manipulation XML GATLAS
    public class Bol_segment
    {
        public string Bol_reference { get; set; }
        public string Line_number { get; set; }
        public string Place_of_loading_code { get; set; }
        public string Place_of_unloading_code { get; set; }
        public string Bol_nature { get; set; }
        public string Location_code { get; set; }
    }
    public class ctn_segment
    {
         

        public string Ctn_reference { get; set; }
        public string Number_of_packages { get; set; }
       public string Type_of_container{ get; set; }
        public string    Empty_Full{ get; set; }
        public string Marks1 { get; set; }
        public string Marks2 { get; set; }
       public string Sealing_Party{ get; set; }
        public string Gross_mass { get; set; }
       public string Fridge_indicator{ get; set; }
        public string Dangerous_indicator { get; set; }
    }
    public class Goods_detail_segment
    {
        public string Package_type_code { get; set; }
        public string Goods_description { get; set; }
        public string Shipping_marks { get; set; }
        public string Gross_mass { get; set; }
        public string Number_of_packages { get; set; }
        public string Volume_in_cubic_meters { get; set; }
        public string Ctn_reference { get; set; }
        public string Dangerous_indicator { get; set; }
        public string Seals_segment { get; set; }
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
    #endregion

    /// <summary>
    /// Logique d'interaction pour AtlasXmlEditor.xaml
    /// </summary>
    public partial class AtlasXmlEditor : Window
    {
        private string filename;
        private Bol_segment bs; private Exporter exporter;
        private Consignee consignee;
        private Notify notify;
        private XElement xdocument;
        private List<Goods_detail_segment> lst_good; List<ctn_segment> lst_ctn;
        public AtlasXmlEditor()
        {
            InitializeComponent();
            bs = new Bol_segment(); exporter = new Exporter(); consignee = new Consignee();
            notify = new Notify();
            
        }

        private void dtgBL_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            if (dtgBL.SelectedIndex != -1)
            {
                Bol_segment bs = (Bol_segment)dtgBL.SelectedItem;
                LoadBL(bs.Line_number);
            }
        }

        /// <summary>
        /// charge un noed Bol_Segment pour edition
        /// </summary>
        /// <param name="linenum"></param>
        private void LoadBL(string linenum)
        {
            var lst = (from el in xdocument.Descendants("Bol_segment") select el).ToList();
            foreach (var el in lst)
            {
                if ((string)(from item in el.Descendants("Line_number") select item).First() == linenum)
                {
                    exporter = new Exporter(); consignee = new Consignee();
                    notify = new Notify(); bs = new Bol_segment();
                    bs.Bol_reference = (string)(from item in el.Descendants("Bol_reference") select item).First();
                    bs.Line_number = (string)(from item in el.Descendants("Line_number") select item).First();
                    bs.Place_of_loading_code = (string)(from item in el.Descendants("Place_of_loading_code") select item).First();
                    bs.Place_of_unloading_code = (string)(from item in el.Descendants("Place_of_unloading_code") select item).First();
                    bs.Bol_nature = (string)(from item in el.Descendants("Bol_nature") select item).First();
                    bs.Location_code = (string)(from item in el.Descendants("Location_code") select item).First();
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

                    txtBLNbrPackages.Text = (string)(from item in el.Descendants("Number_of_packages") select item).First();
                    txtBLPoids.Text = (string)(from item in el.Descendants("Gross_mass") select item).First();

                    var ctn = (from item in el.Descendants("ctn_segment") select item).ToList();
                     lst_ctn = new List<ctn_segment>();

                    foreach (var _ctn in ctn)
                    {
                        lst_ctn.Add(new ctn_segment
                        {
                            Ctn_reference = (string)(from item in _ctn.Descendants("Ctn_reference") select item).First(),
                            Gross_mass = (string)(from item in _ctn.Descendants("Gross_mass") select item).First(),
                            Marks1 = (string)(from item in _ctn.Descendants("Marks1") select item).First(),
                            Marks2 = (string)(from item in _ctn.Descendants("Marks2") select item).First(),
                            Number_of_packages = (string)(from item in _ctn.Descendants("Number_of_packages") select item).First(),
                            Type_of_container = (string)(from item in _ctn.Descendants("Type_of_container") select item).First(),
                            Dangerous_indicator = (string)(from item in _ctn.Descendants("Dangerous_indicator") select item).First(),
                            Empty_Full = (string)(from item in _ctn.Descendants("Empty_Full") select item).First(),
                            Fridge_indicator = (string)(from item in _ctn.Descendants("Fridge_indicator") select item).First(),
                            Sealing_Party = (string)(from item in _ctn.Descendants("Sealing_Party") select item).First()

                        });
                    }
                    dtgCtr.ItemsSource = null; dtgCtr.ItemsSource = lst_ctn;

                    var good_detail = (from m in el.Descendants("Goods_segment").Descendants("Goods_detail_segment") select m).ToList();
                    lst_good = new List<Goods_detail_segment>();
                    foreach (var gds in good_detail)
                    {
                        lst_good.Add(new Goods_detail_segment
                        {
                            Ctn_reference = (string)(from item in gds.Descendants("Ctn_reference") select item).First(),
                            Goods_description = (string)(from item in gds.Descendants("Goods_description") select item).First(),
                            Number_of_packages = (string)(from item in gds.Descendants("Number_of_packages") select item).First(),
                            Package_type_code = (string)(from item in gds.Descendants("Package_type_code") select item).First(),
                            Shipping_marks = (string)(from item in gds.Descendants("Shipping_marks") select item).First(),
                            Dangerous_indicator = (string)(from item in gds.Descendants("Dangerous_indicator") select item).First(),
                            Gross_mass = (string)(from item in gds.Descendants("Gross_mass") select item).First(),
                            Seals_segment = (string)(from item in gds.Descendants("Seals_segment") select item).First(),
                            Volume_in_cubic_meters = (string)(from item in gds.Descendants("Volume_in_cubic_meters") select item).First()
                        });
                    }
                    dtgMchd.ItemsSource = null; dtgMchd.ItemsSource = lst_good;

                    //blrecapgrid.Datacontext = null; grid_shipper.DataContext = null;
                    blrecapgrid.DataContext = bs; grid_notify.DataContext = notify;
                    grid_shipper.DataContext = exporter; grid_consignee.DataContext = consignee;
                }

            }
        }
        private void dtgMchd_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
             Goods_detail_segment gds = (Goods_detail_segment)dtgMchd.SelectedItem;
             if (gds != null)
             {
                 rtb.Document.Blocks.Clear(); 
                 rtb.Document.Blocks.Add(new Paragraph(new Run(gds.Goods_description)));
             }
        }

        /// <summary>
        /// lit le fichier xml et prepare les requete linq to xml
        /// </summary>
        private void readxml()
        {
            try
            {
                xdocument = XElement.Load(filename);
                txtnbrbl.Text = (string)(from el in xdocument.Descendants("Total_number_of_bols") select el).First();
                txtnbrcolis.Text = (string)(from el in xdocument.Descendants("Total_number_of_packages") select el).First();
                txtnbrctr.Text = (string)(from el in xdocument.Descendants("Total_number_of_containers") select el).First();
                txtgrossmass.Text = (string)(from el in xdocument.Descendants("Total_gross_mass") select el).First();

                //lire tous les BL
                var lst = (from el in xdocument.Descendants("Bol_segment") select el).ToList();
                List<Bol_segment> lbol = new List<Bol_segment>();

                foreach (var el in lst)
                {
                    lbol.Add(new Bol_segment
                    {
                        Bol_reference = (string)(from item in el.Descendants("Bol_reference") select item).First(),
                        Line_number = (string)(from item in el.Descendants("Line_number") select item).First(),
                        Place_of_loading_code = (string)(from item in el.Descendants("Place_of_loading_code") select item).First(),
                        Place_of_unloading_code = (string)(from item in el.Descendants("Place_of_unloading_code") select item).First()
                    });

                }

                dtgBL.ItemsSource = null; dtgBL.ItemsSource = lbol;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        private void btnNouveau_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {


                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

                dlg.DefaultExt = ".xml";
                dlg.Filter = "Fichiers (*.xml)|*.xml";
                Nullable<bool> result = dlg.ShowDialog();

                if (result == true)
                {
                    filename = dlg.FileName;
                    txtfilename.Text = dlg.SafeFileName;
                    readxml();
                }
                else
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Echec de la selection du fichier \n" + ex.Message, "Appointement");
            }
        }

        private void btnUpdatBOL_Click_1(object sender, RoutedEventArgs e)
        {
            XElement upd = (from bol in xdocument.Elements("Bol_segment")
                            where (string)bol.Element("Bol_id").Element("Bol_reference").Value == bs.Bol_reference
                            select bol).SingleOrDefault();
            if(upd==null)
            {
                MessageBox.Show("Auccune modification n'est effectuée.", "ATLAS XML", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                try
                {
                    upd.Element("Traders_segment").Element("Exporter").Element("Exporter_name").Value = exporter.Exporter_name;
                    upd.Element("Traders_segment").Element("Exporter").Element("Exporter_address1").Value = exporter.Exporter_address1;
                    upd.Element("Traders_segment").Element("Exporter").Element("Exporter_address2").Value = exporter.Exporter_address2;
                    upd.Element("Traders_segment").Element("Exporter").Element("Exporter_address3").Value = exporter.Exporter_address3;
                    upd.Element("Traders_segment").Element("Exporter").Element("Exporter_address4").Value = exporter.Exporter_address4;

                    upd.Element("Traders_segment").Element("Notify").Element("Notify_name").Value = notify.Notify_name;
                    upd.Element("Traders_segment").Element("Notify").Element("Notify_address1").Value = notify.Notify_address1;
                    upd.Element("Traders_segment").Element("Notify").Element("Notify_address2").Value = notify.Notify_address2;
                    upd.Element("Traders_segment").Element("Notify").Element("Notify_address3").Value = notify.Notify_address3;
                    upd.Element("Traders_segment").Element("Notify").Element("Notify_address4").Value = notify.Notify_address4;

                    upd.Element("Traders_segment").Element("Consignee").Element("Consignee_name").Value = consignee.Consignee_name;
                    upd.Element("Traders_segment").Element("Consignee").Element("Consignee_address1").Value = consignee.Consignee_address1;
                    upd.Element("Traders_segment").Element("Consignee").Element("Consignee_address2").Value = consignee.Consignee_address2;
                    upd.Element("Traders_segment").Element("Consignee").Element("Consignee_address3").Value = consignee.Consignee_address3;
                    upd.Element("Traders_segment").Element("Consignee").Element("Consignee_address4").Value = consignee.Consignee_address4;

                    upd.Element("Bol_id").Element("Bol_nature").Value = bs.Bol_nature;
                    upd.Element("Location").Element("Location_code").Value = bs.Location_code;
                    upd.Element("Goods_segment").Element("Gross_mass").Value = txtBLPoids.Text.Trim();
                    upd.Element("Goods_segment").Element("Number_of_packages").Value = txtBLNbrPackages.Text.Trim();

                    (from m in upd.Descendants("Goods_segment").Descendants("Goods_detail_segment") select m).Remove();
                    // XElement xgds = (from m in upd.Descendants("Goods_segment") select m).SingleOrDefault();
                    foreach (Goods_detail_segment gds in lst_good)
                    {

                        upd.Element("Goods_segment")
                           .Add(new XElement("Goods_detail_segment",
                            new XElement("Package_type_code", gds.Package_type_code),
                            new XElement("Goods_description", gds.Goods_description),
                            new XElement("Shipping_marks", gds.Shipping_marks),
                            new XElement("Gross_mass", gds.Gross_mass),
                            new XElement("Number_of_packages", gds.Number_of_packages),
                            new XElement("Volume_in_cubic_meters", gds.Volume_in_cubic_meters),
                            new XElement("Ctn_reference", gds.Ctn_reference),
                            new XElement("Dangerous_indicator", gds.Dangerous_indicator),
                            new XElement("Seals_segment", gds.Seals_segment)
                            ));
                    }

                    (from m in upd.Descendants("ctn_segment") select m).Remove();
                    foreach (ctn_segment ctn in lst_ctn)
                    {
                        upd.Element("Goods_segment").AddBeforeSelf(new XElement("ctn_segment",
                          new XElement("Ctn_reference", ctn.Ctn_reference),
                          new XElement("Number_of_packages", ctn.Number_of_packages),
                          new XElement("Type_of_container", ctn.Type_of_container),
                          new XElement("Empty_Full", ctn.Empty_Full),
                          new XElement("Marks1", ctn.Marks1),
                          new XElement("Marks2", ctn.Marks2),
                          new XElement("Sealing_Party", ctn.Sealing_Party),
                          new XElement("Gross_mass", ctn.Gross_mass),
                          new XElement("Fridge_indicator", ctn.Fridge_indicator),
                          new XElement("Dangerous_indicator", ctn.Dangerous_indicator)
                          ));


                    }

                    xdocument.Save(filename);
                    MessageBox.Show("BL modifiée");
                    //recharge le fichier
                    xdocument = XElement.Load(filename);
                    readxml();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Une erreur est survenue durant l'opération", "", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void btnGoodUpd_Click_1(object sender, RoutedEventArgs e)
        {

        }
    }
}
