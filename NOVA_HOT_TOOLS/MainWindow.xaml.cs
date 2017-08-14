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
using System.Windows.Navigation;
using System.Windows.Shapes;
using VesselStopOverData;
using Outlook = Microsoft.Office.Interop.Outlook;
using Office = Microsoft.Office.Core;

namespace NOVA_HOT_TOOLS
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private VsomMarchal vsom;
        public MainWindow()
        {
            InitializeComponent();
            vsom = new VsomMarchal();
            List<string> _Statut = new List<string>();
            _Statut.Add("Booking"); _Statut.Add("Final Booking"); _Statut.Add("BL généré"); _Statut.Add("Clearance");
            cboBookingStatut.ItemsSource = _Statut;
        }

        private void btnVermax_Click_1(object sender, RoutedEventArgs e)
        {
            if (txtBooking.Text.Trim().Length == 0)
            {
                MessageBox.Show("Veuillez saisir le numero du booking");
            }
            else
            {
                try
                {

                    CONNAISSEMENT bl = vsom.GetBookingByIdBooking(int.Parse(txtBooking.Text.Trim()));
                    if (bl != null)
                    {
                        if (bl.StatutBL != "Cloturé")
                        {
                            //verifier que le bl contien des conteneur
                            List<ElementBookingCtr> lstctr = vsom.GetConteneursOfBooking(bl.IdBL);
                            if (lstctr.Count == 0)
                            {
                                MessageBox.Show("Ce booking ne contient pas de conteneur");
                            }
                            else
                            {
                                if (MessageBox.Show(string.Format("Confirmez vous la generation du booking {0} / Escale {1}", bl.NumBL, bl.ESCALE.NumEsc),
                                               "Generation VERMAS", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                                {
                                    //generation vermas pour chaque conteneur du booking
                                    StringBuilder sb;// = new StringBuilder();
                                    string armateur = string.Empty;

                                    if (bl.ESCALE.ARMATEUR.CodeArm == "INARME")
                                    { armateur = "GRIMALDI"; }
                                    else if ((bl.ESCALE.ARMATEUR.CodeArm == "NILEDUTCH" || bl.ESCALE.ARMATEUR.CodeArm == "NILE DUTCH" || bl.ESCALE.ARMATEUR.CodeArm == "NDS"))
                                    {
                                        armateur = "NDS";
                                    }

                                    foreach (ElementBookingCtr ebc in lstctr)
                                    {
                                        sb = new StringBuilder();

                                        sb.Append("UNB+UNOA:2+" + armateur + "+DITCAMDLA+" + DateFormat(DateTime.Today) + ":" + string.Format("{0:Hmm}", DateTime.Now) + "+" + bl.IdBL + "'").Append(Environment.NewLine);
                                        sb.Append("UNH+1+VERMAS:D:16A:UN:SMDG04'").Append(Environment.NewLine);
                                        sb.Append("BGM+XXX+" + ebc.NumCtr + "+9+'").Append(Environment.NewLine);
                                        sb.Append("DTM+137:" + string.Format("{0:yyyyMMddHmm}", DateTime.Now) + ":203'").Append(Environment.NewLine);
                                        sb.Append("RFF+MS:" + armateur + "'").Append(Environment.NewLine);
                                        sb.Append("NAD+CF+" + armateur + "'").Append(Environment.NewLine);
                                        sb.Append("EQD+CN+" + ebc.NumCtr + ":6346:5+45G1+++5'").Append(Environment.NewLine);
                                        sb.Append("RFF+BN:" + bl.IdBL + "'").Append(Environment.NewLine);
                                        sb.Append("LOC+9+" + bl.DPBL + "'").Append(Environment.NewLine);
                                        sb.Append("LOC+85+" + bl.LPBL + "'").Append(Environment.NewLine);
                                        sb.Append("MEA+AAE+VGM+KGM:" + ebc.VGM + "'").Append(Environment.NewLine);
                                        sb.Append("DTM+WAT:" + string.Format("{0:yyyyMMdd}0000", DateTime.Now) + ":208'").Append(Environment.NewLine);
                                        sb.Append("TDT+20+" + bl.ESCALE.NumVoySCR + "+1++" + armateur + ":172:20+++ IBOQ103::" + bl.ESCALE.NAVIRE.NomNav + "'").Append(Environment.NewLine);
                                        sb.Append("RFF+VON:" + bl.ESCALE.NumVoySCR + "'").Append(Environment.NewLine);
                                        sb.Append("DOC+DRF:VGM:306:SHIPPER INFO'").Append(Environment.NewLine);
                                        sb.Append("DTM+WAT:" + string.Format("{0:yyyyMMdd}0000", DateTime.Now) + ":203'").Append(Environment.NewLine);
                                        sb.Append("NAD+" + bl.ConsigneeBL + "+" + bl.AdresseBL + "'").Append(Environment.NewLine);
                                        sb.Append("CTA+RP+:SOCOMAR'").Append(Environment.NewLine);
                                        sb.Append("DOC+SM2:VGM:306:WEIGHING CERTIFICATE+SM2'").Append(Environment.NewLine);
                                        sb.Append("NAD+WC'").Append(Environment.NewLine);
                                        sb.Append("UNT+20+16'").Append(Environment.NewLine);
                                        sb.Append("UNZ+1+15'").Append(Environment.NewLine);

                                        System.IO.File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\VERMAS -" + bl.IdBL + "-" + ebc.NumCtr + ".EDI",
                                                                   sb.ToString(), Encoding.GetEncoding("ISO-8859-1"));

                                        Microsoft.Office.Interop.Outlook.Application app = new Microsoft.Office.Interop.Outlook.Application();
                                        Microsoft.Office.Interop.Outlook.MailItem mailItem = app.CreateItem(Microsoft.Office.Interop.Outlook.OlItemType.olMailItem);
                                        mailItem.Subject = "VERMAS " + ebc.NumCtr + " (Socomar)";
                                        mailItem.To = "edi.tedi@ditcameroun.com";
                                        mailItem.Body = "VERMAS " + ebc.NumCtr + " (Socomar)";
                                        mailItem.Attachments.Add(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\VERMAS -" + bl.IdBL + "-" + ebc.NumCtr + ".EDI");
                                        mailItem.Importance = Microsoft.Office.Interop.Outlook.OlImportance.olImportanceHigh;
                                        mailItem.Display(false);
                                        mailItem.Send();

                                    }

                                    MessageBox.Show("Oppération Terminée");
                                }
                                else
                                    MessageBox.Show("Opération annulée par l'utilisateur");
                            }

                        }
                        else
                        {
                            MessageBox.Show("Le statut de ce booking ne permet pas une génération de VERMAS");
                        }
                    }
                    else
                        MessageBox.Show("Aucun booking ne correspond a ce numero.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erreur lors de l'opération " + ex.Message, "VERMAX");
                }
            }
        }

        private string DateFormat(DateTime date)
        {
            return string.Format("{0:yyMMdd}", DateTime.Parse(date.ToShortDateString()));
        }

        private void btnRecap_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void btnRoutine_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void btnChangerStatut_Click_1(object sender, RoutedEventArgs e)
        {

        }
    }
}
