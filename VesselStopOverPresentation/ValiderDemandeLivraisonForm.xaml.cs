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
    /// Logique d'interaction pour ValiderDemandeLivraisonForm.xaml
    /// </summary>
    public partial class ValiderDemandeLivraisonForm : Window
    {

        private DemandeLivraisonForm livForm;
        private List<OPERATION> operationsUser;
        private UTILISATEUR utilisateur;

        private FormLoader formLoader;
       // private VsomParameters vsp = new VsomParameters();
        private VSOMAccessors vsomAcc;
        public ValiderDemandeLivraisonForm(DemandeLivraisonForm form, UTILISATEUR user)
        {
            try
            {

                InitializeComponent();
                using (var ctx = new VSOMClassesDataContext())
                {
                    vsomAcc = new VSOMAccessors(ctx);

                    this.DataContext = this;

                    utilisateur = user;
                    operationsUser = vsomAcc.GetOperationsUtilisateur(utilisateur.IdU);

                    livForm = form;

                    formLoader = new FormLoader(utilisateur);
                }
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        private void btnValider_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //using (var ctx = new VSOMClassesDataContext())
                //{
                    vsomAcc = new VSOMAccessors();
                    //VsomMarchal vsomAcc = new VsomMarchal();

                    DEMANDE_LIVRAISON demandeLivraison = vsomAcc.ValiderDemandeLivraison(Convert.ToInt32(livForm.cbIdDL.Text), new TextRange(txtObservations.Document.ContentStart, txtObservations.Document.ContentEnd).Text, utilisateur.IdU);

                    formLoader.LoadDemandeLivraisonForm(livForm, demandeLivraison);

                    livForm.btnEnregistrer.IsEnabled = false;
                    livForm.borderEtat.Visibility = System.Windows.Visibility.Visible;
                    livForm.btnValiderDL.Visibility = System.Windows.Visibility.Collapsed;

                    MessageBox.Show("Demande de livraison validée avec succès.", "Demande de livraison validée !", MessageBoxButton.OK, MessageBoxImage.Information);

                    // COREOR

                    if (demandeLivraison.CONTENEUR.Count > 0)
                    {
                        StringBuilder sb = new StringBuilder();
                        DateTime dte = DateTime.Now;

                        int i = 1;
                        sb.Append("UNB+UNOA:2+").Append(demandeLivraison.CONNAISSEMENT.ESCALE.ARMATEUR.CodeArm == "INARME" ? "GRIMALDI" : demandeLivraison.CONNAISSEMENT.ESCALE.ARMATEUR.CodeArm).Append("+DITCAMDLA+" + demandeLivraison.DateDBL.Value.Year.ToString().Substring(2, 2) + FormatChiffre(demandeLivraison.DateDBL.Value.Month) + FormatChiffre(demandeLivraison.DateDBL.Value.Day) + ":" + FormatChiffre(demandeLivraison.DateDBL.Value.Hour) + FormatChiffre(demandeLivraison.DateDBL.Value.Minute) + "+" + demandeLivraison.IdDBL + "'").Append(Environment.NewLine);
                        foreach (CONTENEUR ctr in demandeLivraison.CONTENEUR)
                        {
                            sb.Append("UNH+" + demandeLivraison.IdDBL + "+COREOR:D:95B:UN'").Append(Environment.NewLine);
                            sb.Append("BGM+12+" + demandeLivraison.IdDBL + "+9'").Append(Environment.NewLine);
                            sb.Append("RFF+REO:DO-" + demandeLivraison.IdDBL + "-" + demandeLivraison.DateDBL.Value.Year + "-" + demandeLivraison.CONNAISSEMENT.ESCALE.ARMATEUR.CodeArm + "'").Append(Environment.NewLine);
                            sb.Append("TDT+20+" + demandeLivraison.CONNAISSEMENT.ESCALE.NumVoySCR + "+1++" + demandeLivraison.CONNAISSEMENT.ESCALE.ARMATEUR.CodeArm + ":172:20+++" + demandeLivraison.CONNAISSEMENT.ESCALE.NAVIRE.CodeRadio + ":103::" + demandeLivraison.CONNAISSEMENT.ESCALE.NAVIRE.NomNav + "'").Append(Environment.NewLine);
                            sb.Append("NAD+CA+").Append(demandeLivraison.CONNAISSEMENT.ESCALE.ARMATEUR.CodeArm == "INARME" ? "GRIMALDI" : demandeLivraison.CONNAISSEMENT.ESCALE.ARMATEUR.CodeArm).Append(":172:" + ctr.TypeCCtr.Substring(0, 2) + "'").Append(Environment.NewLine);
                            sb.Append("NAD+CF+").Append(demandeLivraison.CONNAISSEMENT.ESCALE.ARMATEUR.CodeArm == "INARME" ? "GRIMALDI" : demandeLivraison.CONNAISSEMENT.ESCALE.ARMATEUR.CodeArm).Append(":172" + ctr.TypeCCtr.Substring(0, 2) + "'").Append(Environment.NewLine);

                            if (ctr.TypeCCtr == "20BX" || ctr.TypeCCtr == "20DV")
                            {
                                sb.Append("EQD+CN+" + ctr.NumCtr + "+22G1:102:5+2+3+5'").Append(Environment.NewLine);
                            }
                            else if (ctr.TypeCCtr == "40BX" || ctr.TypeCCtr == "40DV")
                            {
                                sb.Append("EQD+CN+" + ctr.NumCtr + "+42G1:102:5+2+3+5'").Append(Environment.NewLine);
                            }
                            else if (ctr.TypeCCtr == "40HC")
                            {
                                sb.Append("EQD+CN+" + ctr.NumCtr + "+45G1:102:5+2+3+5'").Append(Environment.NewLine);
                            }
                            else if (ctr.TypeCCtr == "40OT")
                            {
                                sb.Append("EQD+CN+" + ctr.NumCtr + "+42U1:102:5+2+3+5'").Append(Environment.NewLine);
                            }
                            else if (ctr.TypeCCtr == "40FL")
                            {
                                sb.Append("EQD+CN+" + ctr.NumCtr + "+45P3:102:5+2+3+5'").Append(Environment.NewLine);
                            }
                            else if (ctr.TypeCCtr == "20OT")
                            {
                                sb.Append("EQD+CN+" + ctr.NumCtr + "+22U1:102:5+2+3+5'").Append(Environment.NewLine);
                            }
                            else
                            {
                                sb.Append("EQD+CN+" + ctr.NumCtr + "+" + ctr.TypeCCtr + ":102:5+2+3+5'").Append(Environment.NewLine);
                            }

                            sb.Append("MEA+AAE+G+KGM:" + ctr.PoidsCCtr + "'").Append(Environment.NewLine);
                            if (ctr.TypeCCtr.Substring(0, 2) == "20")
                            {
                                sb.Append("MEA+AAE+T+KGM:2280'").Append(Environment.NewLine);
                            }
                            else if (ctr.TypeCCtr.Substring(0, 2) == "40")
                            {
                                sb.Append("MEA+AAE+T+KGM:4480'").Append(Environment.NewLine);
                            }

                            sb.Append("RFF+TF:DO-" + demandeLivraison.IdDBL + "-" + demandeLivraison.DateDBL.Value.Year + "-" + demandeLivraison.CONNAISSEMENT.ESCALE.ARMATEUR.CodeArm + "'").Append(Environment.NewLine);
                            sb.Append("DTM+400:" + ctr.FSCtr.Value.Year + FormatChiffre(ctr.FSCtr.Value.Month) + FormatChiffre(ctr.FSCtr.Value.Day) + "1200:203'").Append(Environment.NewLine);
                            sb.Append("CNT+16:1'").Append(Environment.NewLine);
                            sb.Append("UNT+13+" + demandeLivraison.IdDBL + FormatReferenceCOREOR(i) + "'").Append(Environment.NewLine);
                            i++;
                        }

                        sb.Append("UNZ+3+" + demandeLivraison.IdDBL).Append(Environment.NewLine);

                        System.IO.File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\COREOR - " + demandeLivraison.IdDBL + " - " + demandeLivraison.DateDBL.Value.Year + FormatChiffre(demandeLivraison.DateDBL.Value.Month) + FormatChiffre(demandeLivraison.DateDBL.Value.Day) + FormatChiffre(demandeLivraison.DateDBL.Value.Hour) + FormatChiffre(demandeLivraison.DateDBL.Value.Minute) + FormatChiffre(demandeLivraison.DateDBL.Value.Second) + ".EDI", sb.ToString(), Encoding.GetEncoding("ISO-8859-1"));

                        Microsoft.Office.Interop.Outlook.Application app = new Microsoft.Office.Interop.Outlook.Application();
                        Microsoft.Office.Interop.Outlook.MailItem mailItem = app.CreateItem(Microsoft.Office.Interop.Outlook.OlItemType.olMailItem);
                        mailItem.Subject = "COREOR (Socomar)";
                        mailItem.To = "edi.tedi@ditcameroun.com";
                        mailItem.Body = "COREOR (Socomar)";
                        mailItem.Attachments.Add(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\COREOR - " + demandeLivraison.IdDBL + " - " + demandeLivraison.DateDBL.Value.Year + FormatChiffre(demandeLivraison.DateDBL.Value.Month) + FormatChiffre(demandeLivraison.DateDBL.Value.Day) + FormatChiffre(demandeLivraison.DateDBL.Value.Hour) + FormatChiffre(demandeLivraison.DateDBL.Value.Minute) + FormatChiffre(demandeLivraison.DateDBL.Value.Second) + ".EDI");
                        mailItem.Importance = Microsoft.Office.Interop.Outlook.OlImportance.olImportanceHigh;
                        mailItem.Display(false);
                        mailItem.Send();

                        System.IO.File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\COREOR - " + demandeLivraison.IdDBL + " - " + demandeLivraison.DateDBL.Value.Year + FormatChiffre(demandeLivraison.DateDBL.Value.Month) + FormatChiffre(demandeLivraison.DateDBL.Value.Day) + FormatChiffre(demandeLivraison.DateDBL.Value.Hour) + FormatChiffre(demandeLivraison.DateDBL.Value.Minute) + FormatChiffre(demandeLivraison.DateDBL.Value.Second) + ".EDI");
                    }
                //}
                this.Close();
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

        private static string FormatReferenceCOREOR(int entier)
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
