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
using VesselStopOverData;
using System.Text.RegularExpressions;

namespace VesselStopOverPresentation
{
    /// <summary>
    /// Logique d'interaction pour ClearanceForm.xaml
    /// </summary>
    public partial class ClearanceForm : Window
    {
        private BookingForm bookForm;

        private CONNAISSEMENT booking;
        private List<OPERATION> operationsUser;
        private UTILISATEUR utilisateur;

        private FormLoader formLoader;
        //private VsomParameters vsp = new VsomParameters();
        private VSOMAccessors vsomAcc;
        public ClearanceForm(BookingForm form, CONNAISSEMENT con, UTILISATEUR user)
        {
            try
            {
                 vsomAcc = new VSOMAccessors();

                InitializeComponent();
                booking = con;
                txtNumContrat.Text = con.NumCtrBL;
                txtNumBesc.Text = con.NumBESCBL;
                txtDeclExport.Text = con.NumDEBL;
                txtNumHSCode.Text = con.NumHSCode;
                txtDateClr.SelectedDate = con.DCBLI;
                if (con.DCBLI.HasValue)
                {
                    txtDateCreation.SelectedDate = con.DCBLI;
                }
                txtObservations.Document.Blocks.Clear();
                txtObservations.Document.Blocks.Add(new Paragraph(new Run(booking.AIBL)));
                if (booking.StatutBL != "Clearance" && booking.StatutBL != "Booking")
                {
                    btnEnregistrer.IsEnabled = false;
                }
                bookForm = form;

                utilisateur = user;
                operationsUser = vsomAcc.GetOperationsUtilisateur(utilisateur.IdU);

                formLoader = new FormLoader(utilisateur);
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        private void btnEnregistrer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                vsomAcc = new VSOMAccessors();
               // VsomMarchal vsm = new VsomMarchal();

                if (bookForm.cbEscale.SelectedIndex == -1)
                {
                    MessageBox.Show("Vous devez indiquer à quelle escale est associée ce booking", "N° Escale ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (bookForm.txtNatMses.Text.Trim() == "")
                {
                    MessageBox.Show("Vous devez saisir la nature des marchandises associées ce booking", "Nature des marchandises ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (bookForm.cbDestination.SelectedIndex == -1)
                {
                    MessageBox.Show("Vous devez indiquer à le point de livraison des marchandises est associée ce booking", "Place of delivery ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (bookForm.cbVia.SelectedIndex == -1)
                {
                    MessageBox.Show("Vous devez indiquer à le point de déchargement des marchandises est associée ce booking", "Place of discharge ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (bookForm.txtConsignee.Text.Trim() == "")
                {
                    MessageBox.Show("Vous devez le nom du shipper effectuant ce booking", "Consignee ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (bookForm.txtShipper.Text.Trim() == "")
                {
                    MessageBox.Show("VVous devez le nom du consignee pour qui ce booking est effectué", "Shipper ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (bookForm.txtNotify.Text.Trim() == "")
                {
                    MessageBox.Show("Vous devez le nom du notify relatif à ce booking", "Notify ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (bookForm.chkFretPrepaid.IsChecked == true && bookForm.txtFretPrepaid.Text == "")
                {
                    MessageBox.Show("Le montant à payer pour le fret n'a pas été saisis", "Montant fret ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (txtNumContrat.Text.Trim() == "")
                {
                    MessageBox.Show("Vous devez saisir le numéro du contrat relatif à cette transaction.", "N° Contrat ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (txtDeclExport.Text.Trim() == "")
                {
                    MessageBox.Show("Vous devez saisir le numéro de la déclaration d'exportation relative à cette transaction.", "N° déclaration export ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (txtNumBesc.Text.Trim() == "")
                {
                    MessageBox.Show("Vous devez saisir le numéro du BESC relatif à cette transaction.", "N° BESC ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (txtNumHSCode.Text.Trim() == "")
                {
                    MessageBox.Show("Vous devez saisir le HS Code relatif à cette transaction.", "HS Code ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {

                    List<CONVENTIONNEL> convs = new List<CONVENTIONNEL>();

                    if (bookForm.eltsBookingGC != null)
                    {
                        foreach (ElementBookingGC gc in bookForm.eltsBookingGC)
                        {
                            CONVENTIONNEL conv = new CONVENTIONNEL();

                            conv.SensGC = "E";
                            conv.NumGC = gc.NumGC;
                            conv.DescGC = gc.Description;
                            conv.DescGCEmbarq = gc.Description;
                            conv.LongMGC = gc.Longueur;
                            conv.LargMGC = gc.Largeur;
                            conv.HautMGC = gc.Hauteur;
                            conv.LongCGC = gc.Longueur;
                            conv.LargCGC = gc.Largeur;
                            conv.HautCGC = gc.Hauteur;
                            conv.NumItem = gc.Quantite;
                            conv.QteBGC = gc.Quantite;
                            conv.VolMGC = gc.Volume;
                            conv.VolCGC = gc.Volume;
                            conv.PoidsMGC = gc.Poids;
                            conv.PoidsCGC = gc.Poids;
                            conv.TypeMGC = gc.TypeMses;
                            conv.TypeCGC = gc.TypeMses;
                            conv.DCGC = DateTime.Now;
                            conv.StatGC = "Non initié";

                            convs.Add(conv);
                        }
                    }

                    List<CONTENEUR> conteneurs = new List<CONTENEUR>();

                    if (bookForm.eltsBookingCtr != null)
                    {
                        foreach (ElementBookingCtr ct in bookForm.eltsBookingCtr)
                        {
                            CONTENEUR ctr = new CONTENEUR();

                            ctr.SensCtr = "E";
                            ctr.NumCtr = ct.NumCtr;
                            ctr.DescCtr = ct.Description;
                            ctr.IMDGCode = ct.UNCode;
                            ctr.DescMses = ct.DescMses;
                            ctr.StatutCtr = ct.StatutCtr;
                            ctr.TypeMCtr = ct.TypeCtr;
                            ctr.TypeCCtr = ct.TypeCtr;
                            ctr.TypeMses = ct.TypeMsesCtr;
                            ctr.VolMCtr = ct.Volume;
                            ctr.PoidsMCtr = ct.Poids;
                            ctr.PoidsCCtr = ct.Poids;
                            ctr.Seal1Ctr = ct.Seal1;
                            ctr.Seal2Ctr = ct.Seal2;
                            ctr.DCCtr = DateTime.Now;
                            ctr.StatCtr = "Non initié";
                            //AH 19aout2016
                            ctr.VGM = ct.VGM;

                            conteneurs.Add(ctr);
                        }
                    }

                    CONNAISSEMENT booking = vsomAcc.UpdateClearance(Convert.ToInt32(bookForm.txtSysId.Text), bookForm.escales.ElementAt<ESCALE>(bookForm.cbEscale.SelectedIndex).IdEsc, bookForm.clients.ElementAt<CLIENT>(bookForm.cbClient.SelectedIndex).IdClient, bookForm.txtNatMses.Text, bookForm.cbVia.Text, bookForm.cbDestination.Text, bookForm.cbVia.Text, bookForm.chkGN.IsChecked == true ? "Y" : "N", bookForm.chkHinterland.IsChecked == true ? "Y" : "N", bookForm.chkFretPrepaid.IsChecked == true ? "Y" : "N", bookForm.txtFretPrepaid.Text, bookForm.txtShipper.Text, bookForm.txtAdresseShipper.Text, bookForm.txtEmailShipper.Text, bookForm.txtTelephoneShipper.Text, bookForm.txtConsignee.Text, bookForm.txtAdresseConsigee.Text, bookForm.txtNotify.Text, bookForm.txtAdresseNotify.Text, bookForm.txtEmailNotify.Text, bookForm.txtTelephoneNotify.Text, bookForm.txtPayor.Text, bookForm.txtClearAgent.Text, bookForm.chkSEPBC.IsChecked == true ? "Y" : "N", conteneurs, convs, txtNumContrat.Text, txtNumBesc.Text, txtDeclExport.Text, txtNumHSCode.Text, new TextRange(txtObservations.Document.ContentStart, txtObservations.Document.ContentEnd).Text, utilisateur.IdU);

                    //Raffraîchir les informations
                    formLoader.LoadBookingForm(bookForm, booking);

                    bookForm.actionsBorder.Visibility = System.Windows.Visibility.Visible;

                    MessageBox.Show("Les informations de clearance du booking sélectionné ont été mises à jour !", "Clearance enregistré !", MessageBoxButton.OK, MessageBoxImage.Information);

                    if (booking.CONTENEUR.Count > 0)
                    {
                        StringBuilder sb = new StringBuilder();
                        DateTime dte = DateTime.Now;

                        int i = 1;
                        sb.Append("UNB+UNOA:1+").Append(booking.ESCALE.ARMATEUR.CodeArm == "INARME" ? "GRIMALDI" : booking.ESCALE.ARMATEUR.CodeArm).Append("+DITCAMDLA+" + booking.DCBL.Value.Year.ToString().Substring(2, 2) + FormatChiffre(booking.DCBL.Value.Month) + FormatChiffre(booking.DCBL.Value.Day) + ":" + FormatChiffre(booking.DCBL.Value.Hour) + FormatChiffre(booking.DCBL.Value.Minute) + "+" + booking.IdBL + "'").Append(Environment.NewLine);
                        foreach (CONTENEUR ctr in booking.CONTENEUR)
                        {
                            sb.Append("UNH+" + ctr.IdBL + FormatReferenceCOPARN(i) + "+COPARN:D:95B:UN'").Append(Environment.NewLine);
                            sb.Append("BGM+11+" + booking.DCBL.Value.Year + FormatChiffre(booking.DCBL.Value.Month) + FormatChiffre(booking.DCBL.Value.Day) + FormatChiffre(booking.DCBL.Value.Hour) + FormatChiffre(booking.DCBL.Value.Minute) + FormatChiffre(booking.DCBL.Value.Second) + "+5'").Append(Environment.NewLine);
                            sb.Append("RFF+BN:" + booking.NumBooking + "'").Append(Environment.NewLine);
                            //sb.Append("TDT+20+" + (booking.ESCALE.NumVoySCR.Length == 4 ? booking.ESCALE.NAVIRE.CodeNav + booking.ESCALE.NumVoySCR : booking.ESCALE.NumVoySCR) + "+1++" + booking.ESCALE.ARMATEUR.CodeArm + ":172:20+++" + booking.ESCALE.NAVIRE.CodeRadio + ":103::" + booking.ESCALE.NAVIRE.NomNav + "'").Append(Environment.NewLine);
                            if (booking.ESCALE.ARMATEUR.CodeArm == "INARME")
                            {
                                sb.Append("TDT+20+" + booking.ESCALE.NumVoyDIT + "+1++").Append("GRIMALDI").Append(":172:20+++" + booking.ESCALE.NAVIRE.CodeRadio + ":103::" + booking.ESCALE.NAVIRE.NomNav + "'").Append(Environment.NewLine);
                            }
                            else if ((booking.ESCALE.ARMATEUR.CodeArm == "NILEDUTCH" || booking.ESCALE.ARMATEUR.CodeArm == "NILE DUTCH" || booking.ESCALE.ARMATEUR.CodeArm == "NDS"))
                            {
                                sb.Append("TDT+20+" + booking.ESCALE.NumVoyDIT + "+1++").Append("NDS").Append(":172:20+++" + booking.ESCALE.NAVIRE.CodeRadio + ":103::" + booking.ESCALE.NAVIRE.NomNav + "'").Append(Environment.NewLine);
                            }
                            sb.Append("LOC+88+CMDLA:139:6'").Append(Environment.NewLine);
                            sb.Append("LOC+9+CMDLA:139:6'").Append(Environment.NewLine);
                            sb.Append("DTM+133:" + booking.DCBL.Value.Year + FormatChiffre(booking.DCBL.Value.Month) + FormatChiffre(booking.DCBL.Value.Day) + FormatChiffre(booking.DCBL.Value.Hour) + FormatChiffre(booking.DCBL.Value.Minute) + FormatChiffre(booking.DCBL.Value.Second) + ":203'").Append(Environment.NewLine);
                            sb.Append("NAD+CZ++" + booking.ConsigneeBL + "'").Append(Environment.NewLine);
                            sb.Append("NAD+FW++'" + booking.NotifyBL + "'").Append(Environment.NewLine);
                            if (booking.ESCALE.ARMATEUR.CodeArm == "INARME")
                            {
                                sb.Append("NAD+CA+").Append("GRIMALDI").Append(":172:20'").Append(Environment.NewLine);
                            }
                            else if ((booking.ESCALE.ARMATEUR.CodeArm == "NILEDUTCH" || booking.ESCALE.ARMATEUR.CodeArm == "NILE DUTCH" || booking.ESCALE.ARMATEUR.CodeArm == "NDS"))
                            {
                                sb.Append("NAD+CA+").Append("NDS").Append(":172:20'").Append(Environment.NewLine);
                            }
                            sb.Append("GID+1'").Append(Environment.NewLine);
                            sb.Append("FTX+AAA+++" + booking.DescBL + "'").Append(Environment.NewLine);
                            if (ctr.TypeCCtr == "20BX" || ctr.TypeCCtr == "20DV")
                            {
                                sb.Append("EQD+CN+" + ctr.NumCtr + "+22G1:102:5+2+2+5'").Append(Environment.NewLine);
                            }
                            else if (ctr.TypeCCtr == "40BX" || ctr.TypeCCtr == "40DV")
                            {
                                sb.Append("EQD+CN+" + ctr.NumCtr + "+42G1:102:5+2+2+5'").Append(Environment.NewLine);
                            }
                            else if (ctr.TypeCCtr == "40HC")
                            {
                                sb.Append("EQD+CN+" + ctr.NumCtr + "+45G1:102:5+2+2+5'").Append(Environment.NewLine);
                            }
                            else if (ctr.TypeCCtr == "40OT")
                            {
                                sb.Append("EQD+CN+" + ctr.NumCtr + "+42U1:102:5+2+2+5'").Append(Environment.NewLine);
                            }
                            else if (ctr.TypeCCtr == "40FL")
                            {
                                sb.Append("EQD+CN+" + ctr.NumCtr + "+45P3:102:5+2+2+5'").Append(Environment.NewLine);
                            }
                            else if (ctr.TypeCCtr == "20OT")
                            {
                                sb.Append("EQD+CN+" + ctr.NumCtr + "+22U1:102:5+2+2+5'").Append(Environment.NewLine);
                            }
                            else
                            {
                                sb.Append("EQD+CN+" + ctr.NumCtr + "+" + ctr.TypeCCtr + ":102:5+2+2+5'").Append(Environment.NewLine);
                            }
                            sb.Append("RFF+BN:" + booking.NumBooking + "'").Append(Environment.NewLine);
                            sb.Append("EQN+35'").Append(Environment.NewLine);
                            sb.Append("TMD+3++2'").Append(Environment.NewLine);
                            sb.Append("LOC+8+" + booking.DPBL + ":139:6'").Append(Environment.NewLine);
                            sb.Append("LOC+98+" + booking.LPBL + ":139:6'").Append(Environment.NewLine);
                            sb.Append("LOC+11+" + booking.LDBL + ":139:6'").Append(Environment.NewLine);
                            /*
                             * 23juin16 pour ajouter la VGM. la valeur de la vgm 
                             
                            sb.Append("MEA+AAE+G+KGM:" + ctr.PoidsCCtr + "'").Append(Environment.NewLine);
                            if (ctr.TypeCCtr.Substring(0, 2) == "20")
                            {
                                sb.Append("MEA+AAE+T+KGM:2280'").Append(Environment.NewLine);
                            }
                            else if (ctr.TypeCCtr.Substring(0, 2) == "40")
                            {
                                sb.Append("MEA+AAE+T+KGM:4480'").Append(Environment.NewLine);
                            }

                            if (ctr.TypeCCtr.Substring(0, 2) == "20")
                            {
                                sb.Append("MEA+AAE+EGW+KGM:" + (ctr.PoidsCCtr.Value + 2280).ToString() + "'").Append(Environment.NewLine);
                            }
                            else if (ctr.TypeCCtr.Substring(0, 2) == "40")
                            {
                                sb.Append("MEA+AAE+T+KGM:" + (ctr.PoidsCCtr.Value + 4480).ToString() + "'").Append(Environment.NewLine);
                            }
                            */

                            //AH 23JUIN16 ligne de la VGM 
                            sb.Append("MEA+AAE+VGM+KGM:" + ctr.VGM.Value.ToString() + "'").Append(Environment.NewLine);
                           
                            sb.Append("FTX+AAI+++" + ctr.DescMses + "'").Append(Environment.NewLine);

                            sb.Append("CNT+16:1'").Append(Environment.NewLine);
                            sb.Append("UNT+25+" + booking.IdBL + FormatReferenceCOPARN(i) + "'").Append(Environment.NewLine);
                            i++;
                        }
                        sb.Append("UNZ+1+" + booking.IdBL).Append(Environment.NewLine);

                        System.IO.File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\COPARN - " + booking.IdBL + ".EDI", sb.ToString(), Encoding.GetEncoding("ISO-8859-1"));

                        Microsoft.Office.Interop.Outlook.Application app = new Microsoft.Office.Interop.Outlook.Application();
                        Microsoft.Office.Interop.Outlook.MailItem mailItem = app.CreateItem(Microsoft.Office.Interop.Outlook.OlItemType.olMailItem);
                        mailItem.Subject = "COPARN (Socomar)";
                        mailItem.To = "edi.tedi@socomarcm.net";
                        mailItem.Body = "COPARN (Socomar)";
                        mailItem.Attachments.Add(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\COPARN - " + booking.IdBL + ".EDI");
                        mailItem.Importance = Microsoft.Office.Interop.Outlook.OlImportance.olImportanceHigh;
                        mailItem.Display(false);
                        mailItem.Send();

                        System.IO.File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\COPARN - " + booking.IdBL + ".EDI");
                    }

                    this.Close();
                }
            }
            catch (EnregistrementInexistant ex)
            {
                MessageBox.Show(ex.Message, "Enregistrement inexistant !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            catch (HabilitationException ex)
            {
                MessageBox.Show(ex.Message, "Droits insuffisants !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        private static string FormatReferenceCOPARN(int entier)
        {
            Int32 i = entier;
            if (i >= 10000)
            {
                return i.ToString();
            }
            else if (i >= 1000)
            {
                return "0" + i.ToString();
            }
            else if (i >= 100)
            {
                return "00" + i.ToString();
            }
            else if (i >= 10)
            {
                return "000" + i.ToString();
            }
            else
            {
                return "0000" + i.ToString();
            }
        }

        private static string FormatChiffre(int entier)
        {
            Int32 i = entier;
            if (i >= 10)
            {
                return i.ToString();
            }
            else
            {
                return "0" + i.ToString();
            }
        }
    }
}
