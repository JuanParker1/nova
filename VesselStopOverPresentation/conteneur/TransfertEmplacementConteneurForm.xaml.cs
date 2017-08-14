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
    /// Logique d'interaction pour TransfertEmplacementConteneurForm.xaml
    /// </summary>
    public partial class TransfertEmplacementConteneurForm : Window
    {

        private ConteneurTCForm ctrTCForm;
        private ConteneurTCPanel ctrTCPanel;

        private CONTENEUR_TC conteneurTC;

        public List<PARC> parcs { get; set; }
        public List<string> prcsNouveau { get; set; }

        private List<EMPLACEMENT> emplacements;
        public List<string> emplsNouveau { get; set; }

        private List<OPERATION> operationsUser;
        private UTILISATEUR utilisateur;

        private FormLoader formLoader;
        //private VsomParameters vsp = new VsomParameters();
        private VSOMAccessors vsomAcc;
        public TransfertEmplacementConteneurForm(ConteneurTCForm form, CONTENEUR_TC contTC, UTILISATEUR user)
        {
            try
            {
                 vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

                parcs = vsomAcc.GetParcs("C");
                prcsNouveau = new List<string>();
                foreach (PARC p in parcs)
                {
                    prcsNouveau.Add(p.NomParc);
                }

                utilisateur = user;
                operationsUser = vsomAcc.GetOperationsUtilisateur(utilisateur.IdU);

                conteneurTC = contTC;
                ctrTCForm = form;

                this.txtParcActuel.Text = conteneurTC.PARC.NomParc;
                this.txtEmplacementActuel.Text = vsomAcc.GetEmplacementByIdEmpl(conteneurTC.IdEmplacementParc.Value).LigEmpl + vsomAcc.GetEmplacementByIdEmpl(conteneurTC.IdEmplacementParc.Value).ColEmpl;

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

        public TransfertEmplacementConteneurForm(ConteneurTCPanel panel, CONTENEUR_TC contTC, UTILISATEUR user)
        {
            try
            {
                vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

                parcs = vsomAcc.GetParcs("C");
                prcsNouveau = new List<string>();
                foreach (PARC p in parcs)
                {
                    prcsNouveau.Add(p.NomParc);
                }

                utilisateur = user;
                operationsUser = vsomAcc.GetOperationsUtilisateur(utilisateur.IdU);

                conteneurTC = contTC;
                ctrTCPanel = panel;

                this.txtParcActuel.Text = conteneurTC.PARC.NomParc;
                this.txtEmplacementActuel.Text = vsomAcc.GetEmplacementByIdEmpl(conteneurTC.IdEmplacementParc.Value).LigEmpl + vsomAcc.GetEmplacementByIdEmpl(conteneurTC.IdEmplacementParc.Value).ColEmpl;

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

        private void btnTransferer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                 vsomAcc = new VSOMAccessors();
                //VsomMarchal vsomAcc = new VsomMarchal();

                if (cbParcNouveau.SelectedIndex == -1)
                {
                    MessageBox.Show("Veuillez sélectionner la zone où sera parqué ce véhicule", "Parc ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (cbEmplacementNouveau.SelectedIndex == -1)
                {
                    MessageBox.Show("Veuillez sélectionner un emplacement où sera parqué ce véhicule", "Emplacement ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    //Raffraîchir les informations
                    if (ctrTCForm != null)
                    {
                        CONTENEUR ctr = vsomAcc.TransfEmplacementConteneur(conteneurTC.IdCtr.Value, emplacements.ElementAt<EMPLACEMENT>(cbEmplacementNouveau.SelectedIndex).IdEmpl, new TextRange(txtObservations.Document.ContentStart, txtObservations.Document.ContentEnd).Text, utilisateur.IdU);
                        formLoader.LoadConteneurTCForm(ctrTCForm, ctr.CONTENEUR_TC.FirstOrDefault<CONTENEUR_TC>());
                        
                        //GENERATION COPARN POUR TRANSFERT VIDE VERS DIT
                        if (parcs.ElementAt<PARC>(cbParcNouveau.SelectedIndex).NomParc.Contains("DIT"))
                        {
                            StringBuilder sb = new StringBuilder();
                            DateTime dte = DateTime.Now;

                            sb.Append("UNB+UNOA:1+").Append(ctr.ESCALE.ARMATEUR.CodeArm == "INARME" ? "GRIMALDI" : ctr.ESCALE.ARMATEUR.CodeArm).Append("+DITCAMDLA+" + ctr.CONNAISSEMENT.DCBL.Value.Year.ToString().Substring(2, 2) + FormatChiffre(ctr.CONNAISSEMENT.DCBL.Value.Month) + FormatChiffre(ctr.CONNAISSEMENT.DCBL.Value.Day) + ":" + FormatChiffre(ctr.CONNAISSEMENT.DCBL.Value.Hour) + FormatChiffre(ctr.CONNAISSEMENT.DCBL.Value.Minute) + "+" + ctr.CONNAISSEMENT.IdBL + "'").Append(Environment.NewLine);
                            sb.Append("UNH+" + ctr.IdBL + FormatReferenceCOPARN(1) + "+COPARN:D:95B:UN'").Append(Environment.NewLine);
                            sb.Append("BGM+11+" + ctr.CONNAISSEMENT.DCBL.Value.Year + FormatChiffre(ctr.CONNAISSEMENT.DCBL.Value.Month) + FormatChiffre(ctr.CONNAISSEMENT.DCBL.Value.Day) + FormatChiffre(ctr.CONNAISSEMENT.DCBL.Value.Hour) + FormatChiffre(ctr.CONNAISSEMENT.DCBL.Value.Minute) + FormatChiffre(ctr.CONNAISSEMENT.DCBL.Value.Second) + "+5'").Append(Environment.NewLine);
                            sb.Append("RFF+BN:" + ctr.CONNAISSEMENT.NumBooking + "'").Append(Environment.NewLine);
                            //sb.Append("TDT+20+" + (ctr.ESCALE.NumVoySCR.Length == 4 ? ctr.ESCALE.NAVIRE.CodeNav + ctr.ESCALE.NumVoySCR : ctr.ESCALE.NumVoySCR) + "+1++" + ctr.ESCALE.ARMATEUR.CodeArm + ":172:20+++" + ctr.ESCALE.NAVIRE.CodeRadio + ":103::" + ctr.ESCALE.NAVIRE.NomNav + "'").Append(Environment.NewLine);
                            if (ctr.ESCALE.ARMATEUR.CodeArm == "INARME")
                            {
                                sb.Append("TDT+20+" + ctr.ESCALE.NumVoyDIT + "+1++").Append("GRIMALDI").Append(":172:20+++" + ctr.ESCALE.NAVIRE.CodeRadio + ":103::" + ctr.ESCALE.NAVIRE.NomNav + "'").Append(Environment.NewLine);
                            }
                            else if ((ctr.ESCALE.ARMATEUR.CodeArm == "NILEDUTCH" || ctr.ESCALE.ARMATEUR.CodeArm == "NILE DUTCH" || ctr.ESCALE.ARMATEUR.CodeArm == "NDS"))
                            {
                                sb.Append("TDT+20+" + ctr.ESCALE.NumVoyDIT + "+1++").Append("NDS").Append(":172:20+++" + ctr.ESCALE.NAVIRE.CodeRadio + ":103::" + ctr.ESCALE.NAVIRE.NomNav + "'").Append(Environment.NewLine);
                            }
                            sb.Append("LOC+88+CMDLA:139:6'").Append(Environment.NewLine);
                            sb.Append("LOC+9+CMDLA:139:6'").Append(Environment.NewLine);
                            sb.Append("DTM+133:" + ctr.CONNAISSEMENT.DCBL.Value.Year + FormatChiffre(ctr.CONNAISSEMENT.DCBL.Value.Month) + FormatChiffre(ctr.CONNAISSEMENT.DCBL.Value.Day) + FormatChiffre(ctr.CONNAISSEMENT.DCBL.Value.Hour) + FormatChiffre(ctr.CONNAISSEMENT.DCBL.Value.Minute) + FormatChiffre(ctr.CONNAISSEMENT.DCBL.Value.Second) + ":203'").Append(Environment.NewLine);
                            sb.Append("NAD+CZ++" + ctr.CONNAISSEMENT.ConsigneeBL + "'").Append(Environment.NewLine);
                            sb.Append("NAD+FW++'" + ctr.CONNAISSEMENT.NotifyBL + "'").Append(Environment.NewLine);
                            if (ctr.ESCALE.ARMATEUR.CodeArm == "INARME")
                            {
                                sb.Append("NAD+CA+").Append("GRIMALDI").Append(":172:20'").Append(Environment.NewLine);
                            }
                            else if ((ctr.ESCALE.ARMATEUR.CodeArm == "NILEDUTCH" || ctr.ESCALE.ARMATEUR.CodeArm == "NILE DUTCH" || ctr.ESCALE.ARMATEUR.CodeArm == "NDS"))
                            {
                                sb.Append("NAD+CA+").Append("NDS").Append(":172:20'").Append(Environment.NewLine);
                            }
                            sb.Append("GID+1'").Append(Environment.NewLine);
                            sb.Append("FTX+AAA+++" + ctr.CONNAISSEMENT.DescBL + "'").Append(Environment.NewLine);
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
                            sb.Append("RFF+BN:" + ctr.CONNAISSEMENT.NumBooking + "'").Append(Environment.NewLine);
                            sb.Append("EQN+35'").Append(Environment.NewLine);
                            sb.Append("TMD+3++2'").Append(Environment.NewLine);
                            sb.Append("LOC+8+" + ctr.CONNAISSEMENT.DPBL + ":139:6'").Append(Environment.NewLine);
                            sb.Append("LOC+98+" + ctr.CONNAISSEMENT.LPBL + ":139:6'").Append(Environment.NewLine);
                            sb.Append("LOC+11+" + ctr.CONNAISSEMENT.LDBL + ":139:6'").Append(Environment.NewLine);
                            
                            /* 
                             * 23juin a commente pour gestion VGM
                              
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
                            sb.Append("UNT+25+" + ctr.CONNAISSEMENT.IdBL + FormatReferenceCOPARN(1) + "'").Append(Environment.NewLine);
                            sb.Append("UNZ+1+" + ctr.CONNAISSEMENT.IdBL).Append(Environment.NewLine);

                            System.IO.File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\COPARN - " + ctr.CONNAISSEMENT.IdBL + ".EDI", sb.ToString(), Encoding.GetEncoding("ISO-8859-1"));

                            Microsoft.Office.Interop.Outlook.Application app = new Microsoft.Office.Interop.Outlook.Application();
                            Microsoft.Office.Interop.Outlook.MailItem mailItem = app.CreateItem(Microsoft.Office.Interop.Outlook.OlItemType.olMailItem) as Microsoft.Office.Interop.Outlook.MailItem;
                            mailItem.Subject = "COPARN (Socomar)";
                            mailItem.To = "edi.tedi@socomarcm.net;cellule_tracking@socomarcm.net;ndoutoumou@socomarcm.net";
                            mailItem.Body = "COPARN (Socomar)";
                            mailItem.Attachments.Add(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\COPARN - " + ctr.CONNAISSEMENT.IdBL + ".EDI");
                            mailItem.Importance = Microsoft.Office.Interop.Outlook.OlImportance.olImportanceHigh;
                            mailItem.Display(false);
                            mailItem.Send();
                            app.Quit();

                            System.IO.File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\COPARN - " + ctr.CONNAISSEMENT.IdBL + ".EDI");

                            if (txtEmplacementActuel.Text.Contains("Parc30") && (ctr.ESCALE.ARMATEUR.CodeArm == "NILEDUTCH" || ctr.ESCALE.ARMATEUR.CodeArm == "NDS"))
                            {
                                sb = new StringBuilder();

                                OPERATION_CONTENEUR opTransfCtr = ctr.OPERATION_CONTENEUR.FirstOrDefault<OPERATION_CONTENEUR>(o => o.IdCtr == ctr.IdCtr && o.IdTypeOp == 285);

                                sb.Append("UNB+UNOA:2+SOC+NDAL+").Append(opTransfCtr.DateOp.Value.Year.ToString() + FormatChiffre(opTransfCtr.DateOp.Value.Month) + FormatChiffre(opTransfCtr.DateOp.Value.Day) + ":" + FormatChiffre(opTransfCtr.DateOp.Value.Hour) + FormatChiffre(opTransfCtr.DateOp.Value.Minute) + "+" + opTransfCtr.IdOpCtr + "'").Append(Environment.NewLine);
                                sb.Append("UNH+1+CODECO:D:95B:UN:SMDG20'").Append(Environment.NewLine);
                                sb.Append("BGM+36+" + ctr.NumCtr + "+9'").Append(Environment.NewLine);
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
                                sb.Append("DTM+7:" + opTransfCtr.DateOp.Value.Year + FormatChiffre(opTransfCtr.DateOp.Value.Month) + FormatChiffre(opTransfCtr.DateOp.Value.Day) + FormatChiffre(opTransfCtr.DateOp.Value.Hour) + FormatChiffre(opTransfCtr.DateOp.Value.Minute) + ":203'").Append(Environment.NewLine);
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
                                sb.Append("NAD+CF+").Append(ctr.ESCALE.ARMATEUR.CodeArm == "INARME" ? "GRI" : (ctr.ESCALE.ARMATEUR.CodeArm == "NILEDUTCH" ? "NDS" : ctr.ESCALE.ARMATEUR.CodeArm)).Append("+160:20'").Append(Environment.NewLine);
                                sb.Append("CNT+16:1'").Append(Environment.NewLine);
                                sb.Append("UNT+14+1'").Append(Environment.NewLine);
                                sb.Append("UNZ+1+" + opTransfCtr.IdOpCtr + "'").Append(Environment.NewLine);

                                System.IO.File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\CODECO - GATE OUT - " + ctr.NumCtr + ".EDI", sb.ToString(), Encoding.GetEncoding("ISO-8859-1"));

                                
                                app = new Microsoft.Office.Interop.Outlook.Application();

                                Microsoft.Office.Interop.Outlook.MailItem mailItem2 = app.CreateItem(Microsoft.Office.Interop.Outlook.OlItemType.olMailItem) as Microsoft.Office.Interop.Outlook.MailItem;
                                mailItem2.Subject = "Regarding call C1505-750: testing of EDI receival from our depot in Douala (Socomar)";
                                mailItem2.To = "CMC@niledutch.com";
                                mailItem2.Body = "CODECO - GATE OUT";
                                mailItem2.Attachments.Add(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\CODECO - GATE OUT - " + ctr.NumCtr + ".EDI");
                                mailItem2.Importance = Microsoft.Office.Interop.Outlook.OlImportance.olImportanceHigh;
                                mailItem2.Display(false);
                                mailItem2.Send();

                                System.IO.File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\CODECO - GATE OUT - " + ctr.NumCtr + ".EDI");
                            }
                        }
                    }
                    else if (ctrTCPanel != null)
                    {
                        CONTENEUR ctr = vsomAcc.TransfEmplacementConteneur(conteneurTC.IdCtr.Value, emplacements.ElementAt<EMPLACEMENT>(cbEmplacementNouveau.SelectedIndex).IdEmpl, new TextRange(txtObservations.Document.ContentStart, txtObservations.Document.ContentEnd).Text, utilisateur.IdU);
                        
                        //GENERATION COPARN POUR TRANSFERT VIDE VERS DIT
                        if (parcs.ElementAt<PARC>(cbParcNouveau.SelectedIndex).NomParc.Contains("DIT"))
                        {
                            StringBuilder sb = new StringBuilder();
                            DateTime dte = DateTime.Now;

                            sb.Append("UNB+UNOA:1+").Append(ctr.ESCALE.ARMATEUR.CodeArm == "INARME" ? "GRIMALDI" : ctr.ESCALE.ARMATEUR.CodeArm).Append("+DITCAMDLA+" + ctr.CONNAISSEMENT.DCBL.Value.Year.ToString().Substring(2, 2) + FormatChiffre(ctr.CONNAISSEMENT.DCBL.Value.Month) + FormatChiffre(ctr.CONNAISSEMENT.DCBL.Value.Day) + ":" + FormatChiffre(ctr.CONNAISSEMENT.DCBL.Value.Hour) + FormatChiffre(ctr.CONNAISSEMENT.DCBL.Value.Minute) + "+" + ctr.CONNAISSEMENT.IdBL + "'").Append(Environment.NewLine);
                            sb.Append("UNH+" + ctr.IdBL + FormatReferenceCOPARN(1) + "+COPARN:D:95B:UN'").Append(Environment.NewLine);
                            sb.Append("BGM+11+" + ctr.CONNAISSEMENT.DCBL.Value.Year + FormatChiffre(ctr.CONNAISSEMENT.DCBL.Value.Month) + FormatChiffre(ctr.CONNAISSEMENT.DCBL.Value.Day) + FormatChiffre(ctr.CONNAISSEMENT.DCBL.Value.Hour) + FormatChiffre(ctr.CONNAISSEMENT.DCBL.Value.Minute) + FormatChiffre(ctr.CONNAISSEMENT.DCBL.Value.Second) + "+5'").Append(Environment.NewLine);
                            sb.Append("RFF+BN:" + ctr.CONNAISSEMENT.NumBooking + "'").Append(Environment.NewLine);
                            //sb.Append("TDT+20+" + (ctr.ESCALE.NumVoySCR.Length == 4 ? ctr.ESCALE.NAVIRE.CodeNav + ctr.ESCALE.NumVoySCR : ctr.ESCALE.NumVoySCR) + "+1++" + ctr.ESCALE.ARMATEUR.CodeArm + ":172:20+++" + ctr.ESCALE.NAVIRE.CodeRadio + ":103::" + ctr.ESCALE.NAVIRE.NomNav + "'").Append(Environment.NewLine);
                            if (ctr.ESCALE.ARMATEUR.CodeArm == "INARME")
                            {
                                sb.Append("TDT+20+" + ctr.ESCALE.NumVoyDIT + "+1++").Append("GRIMALDI").Append(":172:20+++" + ctr.ESCALE.NAVIRE.CodeRadio + ":103::" + ctr.ESCALE.NAVIRE.NomNav + "'").Append(Environment.NewLine);
                            }
                            else if ((ctr.ESCALE.ARMATEUR.CodeArm == "NILEDUTCH" || ctr.ESCALE.ARMATEUR.CodeArm == "NILE DUTCH" || ctr.ESCALE.ARMATEUR.CodeArm == "NDS"))
                            {
                                sb.Append("TDT+20+" + ctr.ESCALE.NumVoyDIT + "+1++").Append("NDS").Append(":172:20+++" + ctr.ESCALE.NAVIRE.CodeRadio + ":103::" + ctr.ESCALE.NAVIRE.NomNav + "'").Append(Environment.NewLine);
                            }
                            sb.Append("LOC+88+CMDLA:139:6'").Append(Environment.NewLine);
                            sb.Append("LOC+9+CMDLA:139:6'").Append(Environment.NewLine);
                            sb.Append("DTM+133:" + ctr.CONNAISSEMENT.DCBL.Value.Year + FormatChiffre(ctr.CONNAISSEMENT.DCBL.Value.Month) + FormatChiffre(ctr.CONNAISSEMENT.DCBL.Value.Day) + FormatChiffre(ctr.CONNAISSEMENT.DCBL.Value.Hour) + FormatChiffre(ctr.CONNAISSEMENT.DCBL.Value.Minute) + FormatChiffre(ctr.CONNAISSEMENT.DCBL.Value.Second) + ":203").Append(Environment.NewLine);
                            sb.Append("NAD+CZ++" + ctr.CONNAISSEMENT.ConsigneeBL + "'").Append(Environment.NewLine);
                            sb.Append("NAD+FW++'" + ctr.CONNAISSEMENT.NotifyBL + "'").Append(Environment.NewLine);
                            if (ctr.ESCALE.ARMATEUR.CodeArm == "INARME")
                            {
                                sb.Append("NAD+CA+").Append("GRIMALDI").Append(":172:20'").Append(Environment.NewLine);
                            }
                            else if ((ctr.ESCALE.ARMATEUR.CodeArm == "NILEDUTCH" || ctr.ESCALE.ARMATEUR.CodeArm == "NILE DUTCH" || ctr.ESCALE.ARMATEUR.CodeArm == "NDS"))
                            {
                                sb.Append("NAD+CA+").Append("NDS").Append(":172:20'").Append(Environment.NewLine);
                            }
                            sb.Append("GID+1'").Append(Environment.NewLine);
                            sb.Append("FTX+AAA+++" + ctr.CONNAISSEMENT.DescBL + "'").Append(Environment.NewLine);
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
                            sb.Append("RFF+BN:" + ctr.CONNAISSEMENT.NumBooking + "'").Append(Environment.NewLine);
                            sb.Append("EQN+35'").Append(Environment.NewLine);
                            sb.Append("TMD+3++2'").Append(Environment.NewLine);
                            sb.Append("LOC+8+" + ctr.CONNAISSEMENT.DPBL + ":139:6'").Append(Environment.NewLine);
                            sb.Append("LOC+98+" + ctr.CONNAISSEMENT.LPBL + ":139:6'").Append(Environment.NewLine);
                            sb.Append("LOC+11+" + ctr.CONNAISSEMENT.LDBL + ":139:6'").Append(Environment.NewLine);
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
                            sb.Append("FTX+AAI+++" + ctr.DescMses + "'").Append(Environment.NewLine);

                            sb.Append("CNT+16:1'").Append(Environment.NewLine);
                            sb.Append("UNT+25+" + ctr.CONNAISSEMENT.IdBL + FormatReferenceCOPARN(1) + "'").Append(Environment.NewLine);
                            sb.Append("UNZ+1+" + ctr.CONNAISSEMENT.IdBL).Append(Environment.NewLine);

                            System.IO.File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\COPARN - " + ctr.CONNAISSEMENT.IdBL + ".EDI", sb.ToString(), Encoding.GetEncoding("ISO-8859-1"));

                            Microsoft.Office.Interop.Outlook.Application app = new Microsoft.Office.Interop.Outlook.Application();
                            Microsoft.Office.Interop.Outlook.MailItem mailItem = app.CreateItem(Microsoft.Office.Interop.Outlook.OlItemType.olMailItem) as Microsoft.Office.Interop.Outlook.MailItem;
                            mailItem.Subject = "COPARN (Socomar)";
                            mailItem.To = "edi.tedi@socomarcm.net;cellule_tracking@socomarcm.net;ndoutoumou@socomarcm.net";
                            mailItem.Body = "COPARN (Socomar)";
                            mailItem.Attachments.Add(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\COPARN - " + ctr.CONNAISSEMENT.IdBL + ".EDI");
                            mailItem.Importance = Microsoft.Office.Interop.Outlook.OlImportance.olImportanceHigh;
                            mailItem.Display(false);
                            mailItem.Send();

                            System.IO.File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\COPARN - " + ctr.CONNAISSEMENT.IdBL + ".EDI");

                            if (txtEmplacementActuel.Text.Contains("Parc30") && ctr.ESCALE.ARMATEUR.CodeArm == "NILEDUTCH")
                            {
                                sb = new StringBuilder();

                                OPERATION_CONTENEUR opTransfCtr = ctr.OPERATION_CONTENEUR.FirstOrDefault<OPERATION_CONTENEUR>(o => o.IdCtr == ctr.IdCtr && o.IdTypeOp == 285);

                                sb.Append("UNB+UNOA:2+SOC+NDAL+").Append(opTransfCtr.DateOp.Value.Year.ToString() + FormatChiffre(opTransfCtr.DateOp.Value.Month) + FormatChiffre(opTransfCtr.DateOp.Value.Day) + ":" + FormatChiffre(opTransfCtr.DateOp.Value.Hour) + FormatChiffre(opTransfCtr.DateOp.Value.Minute) + "+" + opTransfCtr.IdOpCtr + "'").Append(Environment.NewLine);
                                sb.Append("UNH+1+CODECO:D:95B:UN:SMDG20'").Append(Environment.NewLine);
                                sb.Append("BGM+36+" + ctr.NumCtr + "+9'").Append(Environment.NewLine);
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
                                sb.Append("DTM+7:" + opTransfCtr.DateOp.Value.Year + FormatChiffre(opTransfCtr.DateOp.Value.Month) + FormatChiffre(opTransfCtr.DateOp.Value.Day) + FormatChiffre(opTransfCtr.DateOp.Value.Hour) + FormatChiffre(opTransfCtr.DateOp.Value.Minute) + ":203'").Append(Environment.NewLine);
                                sb.Append("MEA+AAE+G+KGM:" + ctr.PoidsCCtr + "'").Append(Environment.NewLine);
                                if (ctr.TypeCCtr.Substring(0, 2) == "20")
                                {
                                    sb.Append("MEA+AAE+T+KGM:2280'").Append(Environment.NewLine);
                                }
                                else if (ctr.TypeCCtr.Substring(0, 2) == "40")
                                {
                                    sb.Append("MEA+AAE+T+KGM:4480'").Append(Environment.NewLine);
                                }
                                //sb.Append("SEL+").Append(ctr.Seal1Ctr).Append("'").Append(Environment.NewLine);
                                sb.Append("TDT+1++3++3:172'").Append(Environment.NewLine);
                                sb.Append("LOC+165+CMDLA:139:6+SOC:REP:ZZZ'").Append(Environment.NewLine);
                                sb.Append("NAD+CF+").Append(ctr.ESCALE.ARMATEUR.CodeArm == "INARME" ? "GRI" : (ctr.ESCALE.ARMATEUR.CodeArm == "NILEDUTCH" ? "NDS" : ctr.ESCALE.ARMATEUR.CodeArm)).Append("+160:20'").Append(Environment.NewLine);
                                sb.Append("CNT+16:1'").Append(Environment.NewLine);
                                sb.Append("UNT+12+1'").Append(Environment.NewLine);
                                sb.Append("UNZ+1+" + opTransfCtr.IdOpCtr + "'").Append(Environment.NewLine);

                                System.IO.File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\CODECO - GATE OUT - " + ctr.NumCtr + ".EDI", sb.ToString(), Encoding.GetEncoding("ISO-8859-1"));

                                //Microsoft.Office.Interop.Outlook.Application app2 = new Microsoft.Office.Interop.Outlook.Application();
                                Microsoft.Office.Interop.Outlook.MailItem mailItem2 = app.CreateItem(Microsoft.Office.Interop.Outlook.OlItemType.olMailItem) as Microsoft.Office.Interop.Outlook.MailItem;
                                mailItem2.Subject = "Regarding call C1505-750: testing of EDI receival from our depot in Douala (Socomar)";
                                mailItem2.To = "CMC@niledutch.com";
                                mailItem2.Body = "CODECO - GATE OUT";
                                mailItem2.Attachments.Add(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\CODECO - GATE OUT - " + ctr.NumCtr + ".EDI");
                                mailItem2.Importance = Microsoft.Office.Interop.Outlook.OlImportance.olImportanceHigh;
                                mailItem2.Display(false);
                                mailItem2.Send();

                                System.IO.File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\CODECO - GATE OUT - " + ctr.NumCtr + ".EDI");
                            }
                        }
                    }

                    MessageBox.Show("L'opération de transfert d'emplacement s'est déroulée avec succès", "Véhicule transféré !", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();
                }

            }
            catch (HabilitationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (EnregistrementInexistant ex)
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

        private void cbParcNouveau_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                //VsomParameters vsomAcc = new VsomParameters();

                cbEmplacementNouveau.ItemsSource = null;
                emplacements = vsomAcc.GetEmplacementConteneurByIdParc(parcs.ElementAt<PARC>(cbParcNouveau.SelectedIndex).IdParc, "Disponible");
                
                emplsNouveau = new List<string>();
                foreach (EMPLACEMENT em in emplacements)
                {
                    emplsNouveau.Add(em.LigEmpl + em.ColEmpl);
                }
                cbEmplacementNouveau.ItemsSource = emplsNouveau;
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

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
    }
}
