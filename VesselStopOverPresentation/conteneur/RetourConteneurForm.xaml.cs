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
    /// Logique d'interaction pour RetourConteneurForm.xaml
    /// </summary>
    public partial class RetourConteneurForm : Window
    {

        public List<TYPE_SINISTRE> typesSinistres { get; set; }

        private List<PARC> parcs;
        public List<string> prcs { get; set; }

        //private List<EMPLACEMENT> emplacements;
        //public List<string> empls { get; set; }

        private ConteneurForm ctrForm;
        private ConteneurTCForm ctrTCForm;
        private ConteneurPanel ctrPanel;
        private CONTENEUR conteneur;
        private ConteneurTCPanel ctrTCPanel;
        private CONTENEUR_TC conteneurTC;
        private List<OPERATION> operationsUser;
        private UTILISATEUR utilisateur;

        private FormLoader formLoader;

        public RetourConteneurForm(ConteneurForm form, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();
                VsomParameters vsp = new VsomParameters();
                InitializeComponent();
                this.DataContext = this;

                typesSinistres = vsp.GetTypesSinistreCtr();

                parcs = vsp.GetParcs("C");
                prcs = new List<string>();
                foreach (PARC p in parcs)
                {
                    prcs.Add(p.NomParc);
                }

                utilisateur = user;
                operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);

                ctrForm = form;

                formLoader = new FormLoader(utilisateur);

                //if (utilisateur.LU != "Admin")
                //{
                //    txtDateRetour.IsEnabled = false;
                //}

                if (operationsUser.Where(op => op.NomOp == "Field : Autorisation sur date").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
                {
                    txtDateRetour.IsEnabled = false;
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

        public RetourConteneurForm(ConteneurTCForm form, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();
                //VsomParameters vsp = new VsomParameters();
                InitializeComponent();
                this.DataContext = this;

                typesSinistres = vsomAcc.GetTypesSinistreCtr();

                parcs = vsomAcc.GetParcs("C");
                prcs = new List<string>();
                foreach (PARC p in parcs)
                {
                    prcs.Add(p.NomParc);
                }

                utilisateur = user;
                operationsUser = vsomAcc.GetOperationsUtilisateur(utilisateur.IdU);

                ctrTCForm = form;

                formLoader = new FormLoader(utilisateur);

                //if (utilisateur.LU != "Admin")
                //{
                //    txtDateRetour.IsEnabled = false;
                //}

                if (operationsUser.Where(op => op.NomOp == "Field : Autorisation sur date").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
                {
                    txtDateRetour.IsEnabled = false;
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

        public RetourConteneurForm(ConteneurPanel panel, CONTENEUR ctr, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();
                VsomParameters vsp = new VsomParameters();
                InitializeComponent();
                this.DataContext = this;

                typesSinistres = vsp.GetTypesSinistreCtr();

                parcs = vsp.GetParcs("C");
                prcs = new List<string>();
                foreach (PARC p in parcs)
                {
                    prcs.Add(p.NomParc);
                }

                conteneur = ctr;

                utilisateur = user;
                operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);

                ctrPanel = panel;

                formLoader = new FormLoader(utilisateur); 

                if (operationsUser.Where(op => op.NomOp == "Field : Autorisation sur date").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
                {
                    txtDateRetour.IsEnabled = false;
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

        public RetourConteneurForm(ConteneurTCPanel panel, CONTENEUR_TC ctr, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();
                VsomParameters vsp = new VsomParameters();
                InitializeComponent();
                this.DataContext = this;

                typesSinistres = vsp.GetTypesSinistreCtr();

                parcs = vsp.GetParcs("C");
                prcs = new List<string>();
                foreach (PARC p in parcs)
                {
                    prcs.Add(p.NomParc);
                }

                conteneurTC = ctr;

                utilisateur = user;
                operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);

                ctrTCPanel = panel;

                formLoader = new FormLoader(utilisateur);

                //if (utilisateur.LU != "Admin")
                //{
                //    txtDateRetour.IsEnabled = false;
                //}

                if (operationsUser.Where(op => op.NomOp == "Field : Autorisation sur date").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
                {
                    txtDateRetour.IsEnabled = false;
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

        private void btnRetour_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                if (!txtDateRetour.SelectedDate.HasValue)
                {
                    MessageBox.Show("Veuillez entrer une date de retour", "Echec de l'opération", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (cbParc.SelectedIndex == -1)
                {
                    MessageBox.Show("Veuillez sélectionner un parc où sera parqué ce conteneur", "Parc ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    CONTENEUR ctr = null;
                    DateTime dretour = txtDateRetour.SelectedDate.Value;
                    dretour = dretour.AddHours(11);
                    dretour = dretour.AddMinutes(59);

                    if (ctrForm != null)
                    {
                        ctr = vsomAcc.RetourConteneur(Convert.ToInt32(ctrForm.txtIdCtr.Text), dretour, parcs.ElementAt<PARC>(cbParc.SelectedIndex).IdParc, dataGridSinistres.SelectedItems.OfType<TYPE_SINISTRE>().ToList<TYPE_SINISTRE>(), new TextRange(txtObservations.Document.ContentStart, txtObservations.Document.ContentEnd).Text, utilisateur.IdU);
                        //Raffraîchir les informations

                        formLoader.LoadConteneurForm(ctrForm, ctr);
                    }
                    else if (ctrTCForm != null)
                    {
                        ctr = vsomAcc.RetourConteneur(Convert.ToInt32(ctrTCForm.txtIdCtr.Text), dretour, parcs.ElementAt<PARC>(cbParc.SelectedIndex).IdParc, dataGridSinistres.SelectedItems.OfType<TYPE_SINISTRE>().ToList<TYPE_SINISTRE>(), new TextRange(txtObservations.Document.ContentStart, txtObservations.Document.ContentEnd).Text, utilisateur.IdU);
                        //Raffraîchir les informations

                        formLoader.LoadConteneurTCForm(ctrTCForm, ctr.CONTENEUR_TC.FirstOrDefault<CONTENEUR_TC>());
                    }
                    else if (ctrPanel != null)
                    {
                        ctr = vsomAcc.RetourConteneur(((CONTENEUR)ctrPanel.dataGrid.SelectedItem).IdCtr, dretour, parcs.ElementAt<PARC>(cbParc.SelectedIndex).IdParc, dataGridSinistres.SelectedItems.OfType<TYPE_SINISTRE>().ToList<TYPE_SINISTRE>(), new TextRange(txtObservations.Document.ContentStart, txtObservations.Document.ContentEnd).Text, utilisateur.IdU);
                        //Raffraîchir les informations
                        //ctrPanel.dataGrid.ItemsSource = vsomAcc.GetConteneursImport();
                    }
                    else if (ctrTCPanel != null)
                    {
                        ctr = vsomAcc.RetourConteneur(((CONTENEUR_TC)ctrTCPanel.dataGrid.SelectedItem).IdCtr.Value, dretour, parcs.ElementAt<PARC>(cbParc.SelectedIndex).IdParc, dataGridSinistres.SelectedItems.OfType<TYPE_SINISTRE>().ToList<TYPE_SINISTRE>(), new TextRange(txtObservations.Document.ContentStart, txtObservations.Document.ContentEnd).Text, utilisateur.IdU);
                        //Raffraîchir les informations
                        //ctrTCPanel.dataGrid.ItemsSource = vsomAcc.GetConteneursTC();
                    }

                    MessageBox.Show("L'opération de retour de conteneur s'est déroulée avec succès, consultez le journal des éléments de facturation", "Conteneur retourné !", MessageBoxButton.OK, MessageBoxImage.Information);

                    if (ctr.ESCALE.ARMATEUR.CodeArm == "NILEDUTCH" || ctr.ESCALE.ARMATEUR.CodeArm == "NDS")
                    {
                        StringBuilder sb = new StringBuilder();
                        DateTime dte = DateTime.Now;

                        OPERATION_CONTENEUR opReceptCtr = ctr.OPERATION_CONTENEUR.FirstOrDefault<OPERATION_CONTENEUR>(o => o.IdCtr == ctr.IdCtr && o.IdTypeOp == 19);

                        sb.Append("UNB+UNOA:2+SOC+NDAL+").Append(opReceptCtr.DateOp.Value.Year.ToString() + FormatChiffre(opReceptCtr.DateOp.Value.Month) + FormatChiffre(opReceptCtr.DateOp.Value.Day) + ":" + FormatChiffre(opReceptCtr.DateOp.Value.Hour) + FormatChiffre(opReceptCtr.DateOp.Value.Minute) + "+" + opReceptCtr.IdOpCtr + "'").Append(Environment.NewLine);
                        sb.Append("UNH+1+CODECO:D:95B:UN:SMDG20'").Append(Environment.NewLine);
                        sb.Append("BGM+34+" + ctr.NumCtr + "+9'").Append(Environment.NewLine);
                        sb.Append("NAD+CA+NDAL:172:").Append(ctr.TypeCCtr.Substring(0, 2)).Append("'").Append(Environment.NewLine);

                        if (ctr.TypeCCtr == "20BX")
                        {
                            sb.Append("EQD+CN+" + ctr.NumCtr + "+22G1:102+2+2+4'").Append(Environment.NewLine);
                        }
                        else if (ctr.TypeCCtr == "40BX")
                        {
                            sb.Append("EQD+CN+" + ctr.NumCtr + "+42G1:102+2+2+4'").Append(Environment.NewLine);
                        }
                        else if (ctr.TypeCCtr == "40HC")
                        {
                            sb.Append("EQD+CN+" + ctr.NumCtr + "+45G1:102+2+2+4'").Append(Environment.NewLine);
                        }
                        else if (ctr.TypeCCtr == "40OT")
                        {
                            sb.Append("EQD+CN+" + ctr.NumCtr + "+42U1:102+2+2+4'").Append(Environment.NewLine);
                        }
                        else if (ctr.TypeCCtr == "40FL")
                        {
                            sb.Append("EQD+CN+" + ctr.NumCtr + "+45P3:102+2+2+4'").Append(Environment.NewLine);
                        }
                        else if (ctr.TypeCCtr == "20OT")
                        {
                            sb.Append("EQD+CN+" + ctr.NumCtr + "+22U1:102+2+2+4'").Append(Environment.NewLine);
                        }
                        else
                        {
                            sb.Append("EQD+CN+" + ctr.NumCtr + "+" + ctr.TypeCCtr + ":102+2+2+4'").Append(Environment.NewLine);
                        }

                        //sb.Append("RFF+BN:" + ctr.CONNAISSEMENT.NumBooking + "'").Append(Environment.NewLine);
                        //sb.Append("RFF+VM:" + ctr.ESCALE.NAVIRE.CodeRadio + "'").Append(Environment.NewLine);
                        sb.Append("DTM+7:" + opReceptCtr.DateOp.Value.Year + FormatChiffre(opReceptCtr.DateOp.Value.Month) + FormatChiffre(opReceptCtr.DateOp.Value.Day) + FormatChiffre(opReceptCtr.DateOp.Value.Hour) + FormatChiffre(opReceptCtr.DateOp.Value.Minute) + ":203'").Append(Environment.NewLine);
                        sb.Append("MEA+AAE+G+KGM:" + ctr.PoidsCCtr + "'").Append(Environment.NewLine);
                        if (ctr.TypeCCtr.Substring(0, 2) == "20")
                        {
                            sb.Append("MEA+AAE+T+KGM:2280'").Append(Environment.NewLine);
                        }
                        else if (ctr.TypeCCtr.Substring(0, 2) == "40")
                        {
                            sb.Append("MEA+AAE+T+KGM:4480'").Append(Environment.NewLine);
                        }
                        sb.Append("SEL+").Append(ctr.Seal1Ctr).Append("'").Append(Environment.NewLine);
                        sb.Append("TDT+1++3++3:172'").Append(Environment.NewLine);
                        sb.Append("LOC+165+CMDLA:139:6+SOC:REP:ZZZ'").Append(Environment.NewLine);
                        sb.Append("NAD+CF").Append(ctr.ESCALE.ARMATEUR.CodeArm == "INARME" ? "GRI" : (ctr.ESCALE.ARMATEUR.CodeArm == "NILEDUTCH" ? "NDS" : ctr.ESCALE.ARMATEUR.CodeArm)).Append("160:20'").Append(Environment.NewLine);
                        sb.Append("CNT+16:1'").Append(Environment.NewLine);
                        sb.Append("UNT+13+1'").Append(Environment.NewLine);
                        sb.Append("UNZ+1+" + opReceptCtr.IdOpCtr + "'").Append(Environment.NewLine);

                        System.IO.File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\CODECO - GATE IN - " + ctr.NumCtr + ".EDI", sb.ToString(), Encoding.GetEncoding("ISO-8859-1"));

                        Microsoft.Office.Interop.Outlook.Application app = new Microsoft.Office.Interop.Outlook.Application();
                        Microsoft.Office.Interop.Outlook.MailItem mailItem = app.CreateItem(Microsoft.Office.Interop.Outlook.OlItemType.olMailItem);
                        mailItem.Subject = "Regarding call C1505-750: testing of EDI receival from our depot in Douala (Socomar)";
                        mailItem.To = "CMC@niledutch.com";
                        mailItem.Body = "CODECO - GATE IN";
                        mailItem.Attachments.Add(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\CODECO - GATE IN - " + ctr.NumCtr + ".EDI");
                        mailItem.Importance = Microsoft.Office.Interop.Outlook.OlImportance.olImportanceHigh;
                        mailItem.Display(false);
                        mailItem.Send();

                        System.IO.File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\CODECO - GATE IN - " + ctr.NumCtr + ".EDI");
                    }

                    this.Close();
                }
            }
            catch (HabilitationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (IdentificationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
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
