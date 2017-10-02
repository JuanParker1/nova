using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using VesselStopOverData;
namespace VesselStopOverPresentation.Remote
{
    /// <summary>
    /// Logique d'interaction pour QuotationForm.xaml
    /// </summary>
    public partial class QuotationForm : Window
    {
        private FormLoader formLoader;
       private VesselStopOverPresentation.Remote.QuotationsPanel _panel; 
       private RMVSOM_Views _view;
       private UTILISATEUR _usr;
        public QuotationForm(VesselStopOverPresentation.Remote.QuotationsPanel panel, RMVSOM_Views view, UTILISATEUR usr)
        {
            InitializeComponent();
            _panel = panel; _usr = usr; _view = view;
            //form loader en fonction du statut de la requete
            
            load();   
        }
        /// <summary>
        /// applique le statut des bouton d'action en fonction du statut du dossier
        /// </summary>
        private void load()
        {
            _view = new RMVSOM_Marchal().LoadQuotationForm(_view);
            this.DataContext = _view;
            if (_view.Statut == "Pending") { btnEnregistrer.IsEnabled = true; BtnFacturer.IsEnabled = false; btnPrint.IsEnabled = false; btnAnnuler.IsEnabled = true; txtClientFature.IsEnabled = false; }

            if (_view.Statut == "Proforma") { btnEnregistrer.IsEnabled = false; BtnFacturer.IsEnabled = true; btnPrint.IsEnabled = false;
                btnAnnuler.IsEnabled = false; txtClientFature.IsEnabled = true; 
            //TODO: recharger les element de facturation liée a cette quotation
            }
            if (_view.Statut == "Proccessed") { btnEnregistrer.IsEnabled = false; BtnFacturer.IsEnabled = false; btnPrint.IsEnabled = true;
                btnAnnuler.IsEnabled = false; txtClientFature.IsEnabled = false;
                //TODO: recharger les element de facturation liée a cette quotation

            }
        }
        private void btnEnregistrer_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                load();
                if (_view.Statut == "Pending")
                {
                    if (MessageBox.Show("Etes vous sûr de vouloir valider cette demande?", "Validation cotation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        //on appel le calcule de stationnement ici. puis on affiche les element de stationnement en fonction de la demande

                        //using (var ctx = new VSOMClassesDataContext())
                        //{
                            VSOMAccessors vs = new VSOMAccessors();
                            vs.CalculerSejourVehiculeV2(_view.Chassis, _view.NumBL, _view.NumQuotation, _view.DateSejour, _usr);
                            MessageBox.Show("Elements de facturation générés", "Validation quotation");

                            //TODO: chargement element de facturation selon quotation
                            List<ElementFacturation> lef = null;

                            lef = vs.GetElementFacturationBLFree(_view.IdBL);
                            dataGridEltsFact.ItemsSource = null;
                            dataGridEltsFact.ItemsSource = lef;
                        //}
                        new RMVSOM_Marchal().ValideRequetes(_view.NumRequete);
                        load();
                        MessageBox.Show("La demande est Validée. Veuillez effectuer la facturation", "Validation cotation");
                        txtClientFature.IsEnabled = true;

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Echec de l'opération : " + ex.Message, "Validation cotation", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
        }

        private void BtnProforma_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void BtnFacturer_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                if (txtClientFature.Text == "") { MessageBox.Show("Veuillez definir le client a facturer", "Validation cotation"); return; }
                load();
                if (_view.Statut == "Proforma")
                {
                    if (MessageBox.Show("Confirmez vous la facturation ?", "Validation cotation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        FACTURE fac = null;
                        //using (var ctx = new VSOMClassesDataContext())
                        //{
                            VSOMAccessors vs = new VSOMAccessors();
                            fac= vs.FacturerQuotation(_view.IdBL, "", _usr.IdU, txtClientFature.Text,_view.HT);
                        //}

                        if (fac != null)
                        {
                            MessageBox.Show("Facture n° " + fac.IdDocSAP + " créée avec succès.", "Validation cotation");
                            new RMVSOM_Marchal().FactureRequetes(_view.NumRequete,fac.IdDocSAP.Value);
                            load();
                            MessageBox.Show("Requètes mise à jour avec succès.", "Validation cotation");

                            #region envoie facture par mail
                            //il faudra un job pour envoye la facture par mail
                            Microsoft.Reporting.WinForms.ReportViewer repViewer = new Microsoft.Reporting.WinForms.ReportViewer();
                            repViewer.ProcessingMode = Microsoft.Reporting.WinForms.ProcessingMode.Remote;
                            repViewer.ServerReport.ReportServerUrl = new Uri("http://192.168.0.28/ReportServer");
                            repViewer.ShowParameterPrompts = false;
                            repViewer.ServerReport.ReportPath = "/VSOMReports/FactureRM";

                            System.Net.NetworkCredential myCred = new System.Net.NetworkCredential("novareports", "novareports", "siege.local");
                            repViewer.ServerReport.ReportServerCredentials.NetworkCredentials = myCred;

                            Microsoft.Reporting.WinForms.ReportParameter[] parameters = new Microsoft.Reporting.WinForms.ReportParameter[1];
                            parameters[0] = new Microsoft.Reporting.WinForms.ReportParameter("RefFacture",
                                fac.IdDocSAP.ToString());
                            
                            repViewer.ServerReport.SetParameters(parameters);
                            repViewer.ServerReport.Refresh();
                            repViewer.RefreshReport();

                            Microsoft.Reporting.WinForms.Warning[] warnings;
                            string[] streamIds;
                            string mimeType = string.Empty;
                            string encoding = string.Empty;
                            string extension = string.Empty;
                            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

                            byte[] bytes = repViewer.ServerReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamIds, out warnings);

                            System.IO.File.WriteAllBytes(path + "/facture_" + fac.IdDocSAP + "_A.pdf", bytes);

                            MailMessage mail = new MailMessage();
                            SmtpClient SmtpServer = new SmtpClient("webmail.socomar-cameroun.com");

                            mail.From = new MailAddress("automate@socomar-cameroun.com");
                            mail.Bcc.Add("support@socomar-cameroun.com");
                            mail.To.Add(_view.EmailUser); mail.CC.Add("socomar.tembo-alcm@bollore.com");
                            mail.Subject = "Votre Facture basée sur la quotation n° "+_view.NumQuotation;
                            mail.Body = "Merci de voir ci joint votre facture basée sur la demande " + _view.Libelle;
                            mail.Attachments.Add(new Attachment(path + "/facture_" + fac.IdDocSAP + "_A.pdf"));
                            //SmtpServer.Port = 587;
                            SmtpServer.Credentials = new System.Net.NetworkCredential("automate@socomar-cameroun.com", "Socom@r17!");
                            //SmtpServer.EnableSsl = true;

                            SmtpServer.Send(mail);

                            #endregion

                            MessageBox.Show("Facture envoyée avec succès.", "Validation cotation");
                           // System.IO.File.Delete(path + "/facture_" + fac.IdDocSAP + "_A.pdf");
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show("Echec de l'opération : " + ex.Message, "Validation cotation", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnAnnuler_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void btnPrint_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            if(System.IO.File.Exists(path + "/facture_" + _view.Facture + "_B.pdf"))
            {
                System.IO.File.Delete(path + "/facture_" + _view.Facture + "_B.pdf");
            }
            #region envoie facture par mail
            //il faudra un job pour envoye la facture par mail
            Microsoft.Reporting.WinForms.ReportViewer repViewer = new Microsoft.Reporting.WinForms.ReportViewer();
            repViewer.ProcessingMode = Microsoft.Reporting.WinForms.ProcessingMode.Remote;
            repViewer.ServerReport.ReportServerUrl = new Uri("http://192.168.0.28/ReportServer");
            repViewer.ShowParameterPrompts = false;
            repViewer.ServerReport.ReportPath = "/VSOMReports/FactureRM";

            System.Net.NetworkCredential myCred = new System.Net.NetworkCredential("novareports", "novareports", "siege.local");
            repViewer.ServerReport.ReportServerCredentials.NetworkCredentials = myCred;

            Microsoft.Reporting.WinForms.ReportParameter[] parameters = new Microsoft.Reporting.WinForms.ReportParameter[1];
            parameters[0] = new Microsoft.Reporting.WinForms.ReportParameter("RefFacture",
                _view.Facture.ToString());

            repViewer.ServerReport.SetParameters(parameters);
            repViewer.ServerReport.Refresh();
            repViewer.RefreshReport();

            Microsoft.Reporting.WinForms.Warning[] warnings;
            string[] streamIds;
            string mimeType = string.Empty;
            string encoding = string.Empty;
            string extension = string.Empty;
            

            byte[] bytes = repViewer.ServerReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamIds, out warnings);

            System.IO.File.WriteAllBytes(path + "/facture_" + _view.Facture + "_B.pdf", bytes);

            MailMessage mail = new MailMessage();
            SmtpClient SmtpServer = new SmtpClient("webmail.socomar-cameroun.com");

            mail.From = new MailAddress("automate@socomar-cameroun.com");
            mail.Bcc.Add("support@socomar-cameroun.com");
            mail.To.Add(_view.EmailUser); mail.CC.Add("socomar.tembo-alcm@bollore.com");
            mail.Subject = "Votre Facture basée sur la quotation n° " + _view.NumQuotation;
            mail.Body = "Merci de voir ci joint votre facture basée sur la demande " + _view.Libelle;
            mail.Attachments.Add(new Attachment(path + "/facture_" + _view.Facture + "_B.pdf"));
            //SmtpServer.Port = 587;
            SmtpServer.Credentials = new System.Net.NetworkCredential("automate@socomar-cameroun.com", "Socom@r17!");
            //SmtpServer.EnableSsl = true;

            SmtpServer.Send(mail);

            #endregion
            MessageBox.Show("Facture envoyée avec succès.", "Validation cotation");
            }
            catch (Exception ex)
            {

                MessageBox.Show(" L'opération a echoué : " + ex.Message, "Validation cotation");
            }
           
        }
    }
}
