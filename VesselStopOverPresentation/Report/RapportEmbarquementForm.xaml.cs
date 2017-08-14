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
    /// Logique d'interaction pour RapportEmbarquementForm.xaml
    /// </summary>
    public partial class RapportEmbarquementForm : Window
    {
        public List<CONTENEUR> conteneurs { get; set; }

        private TrackingControlPanel trackingPanel;

        private List<OPERATION> operationsUser;
        private UTILISATEUR utilisateur;

        public List<ESCALE> escales { get; set; }
        public List<Int32> escs { get; set; }

        private FormLoader formLoader;
        private VsomParameters vsp = new VsomParameters();
        public RapportEmbarquementForm(TrackingControlPanel panel, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;
                
                utilisateur = user;
                operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);

                trackingPanel = panel;

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

        private void cbNumEscale_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                if (e.Key == Key.Return && cbNumEscale.Text.Trim() != "")
                {
                    int result;
                    escales = vsp.GetEscalesByNumEscale(Int32.TryParse(cbNumEscale.Text.Trim(), out result) ? result : -1);

                    if (escales.Count == 0)
                    {
                        MessageBox.Show("Il n'existe aucune escale portant ce numéro", "Escale introuvable", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                    else if (escales.Count == 1)
                    {
                        ESCALE esc = escales.FirstOrDefault<ESCALE>();
                        formLoader.LoadEscaleForm(this, esc);
                    }
                    else
                    {
                        ListEscaleForm listEscForm = new ListEscaleForm(this, escales, utilisateur);
                        listEscForm.Title = "Choix multiples : Sélectionnez une escale";
                        listEscForm.ShowDialog();
                    }
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

        private void cbNumEscale_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void btnExtraire_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                VSOMAccessors vsomAcc = new VSOMAccessors();
                VsomParameters vsp = new VsomParameters();
                if (operationsUser.Where(op => op.NomOp == "Report : Rapport embarquement").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
                {
                    MessageBox.Show("Vous n'avez pas les droits nécessaires pour éditer le rapport de embarquement. Veuillez contacter un administrateur", "Droits insuffisants !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    ESCALE esc = escales.FirstOrDefault<ESCALE>();

                    StringBuilder sb = new StringBuilder();
                    DateTime dte = DateTime.Now;

                    List<CONTENEUR> listCtrs = vsp.GetConteneursEmbarques(esc.IdEsc); //esc.CONTENEUR.Where<CONTENEUR>(ct => /*ct.SensCtr == "I" &&*/ ct.CONTENEUR_TC.FirstOrDefault<CONTENEUR_TC>().MOUVEMENT_TC.FirstOrDefault<MOUVEMENT_TC>(op => op.IdTypeOp == 283) != null).ToList<CONTENEUR>();
                    int numMessage = vsomAcc.GetNumMessageGR();

                    int i = 1;

                    sb.Append("SOCOMAR").Append(Environment.NewLine);
                    sb.Append("B.P:12351 Douala Cameroun").Append(Environment.NewLine);
                    sb.Append("Til:(237) 33424550. Fax:(237) 33424936").Append(Environment.NewLine);
                    sb.Append("Nr.Message: ").Append(numMessage).Append(" Date ").Append(dte.Day + "." + dte.Month + "." + dte.Year).Append(Environment.NewLine);
                    sb.Append("Operation: IMBARCOG M/V ").Append(esc.NAVIRE.NomNav).Append(" VOY ").Append(esc.NumVoySCR).Append(" OF ").Append(esc.DRAEsc.Value.Day + "." + esc.DRAEsc.Value.Month + "." + esc.DRAEsc.Value.Year).Append(Environment.NewLine);

                    sb.Append(Environment.NewLine);
                    sb.Append(Environment.NewLine);

                    sb.Append("0004043077").Append(FormatChiffre(esc.DRAEsc.Value.Year).Substring(2, 2) + FormatChiffre(esc.DRAEsc.Value.Month) + FormatChiffre(esc.DRAEsc.Value.Day) + FormatChiffre(esc.DRAEsc.Value.Hour) + FormatChiffre(esc.DRAEsc.Value.Minute)).Append("1915GR").Append(Environment.NewLine);
                    sb.Append("140077").Append(FormatChiffre(esc.DRAEsc.Value.Year).Substring(2, 2) + FormatChiffre(esc.DRAEsc.Value.Month) + FormatChiffre(esc.DRAEsc.Value.Day)).Append(esc.NAVIRE.CodeTracking).Append(esc.NumVoyDIT).Append("IMBARCOG").Append(Environment.NewLine);
                    foreach (CONTENEUR ctr in listCtrs)
                    {
                        sb.Append("141").Append(ctr.NumCtr.PadLeft(11, 'X')).Append(vsp.GetPortsByCodePort(ctr.CONNAISSEMENT.LPBL).FirstOrDefault<PORT>().CodeRadio).Append(ctr.StatutCtr.Substring(0, 1)).Append("IMBARCOG").Append(Environment.NewLine);
                        i++;
                    }
                    sb.Append("9994043077").Append(FormatChiffre(dte.Year).Substring(2, 2) + FormatChiffre(dte.Month) + FormatChiffre(dte.Day) + FormatChiffre(dte.Hour) + FormatChiffre(dte.Minute)).Append("000").Append((listCtrs.Count + 2).ToString().PadLeft(3, '0')).Append(Environment.NewLine);

                    Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
                    dlg.FileName = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\Rapport Embarquement - Escale " + esc.NumEsc + ".txt";
                    dlg.DefaultExt = ".txt"; // Default file extension
                    dlg.Filter = "Text Documents (.txt)|*.txt"; // Filter files by extension

                    // Show save file dialog box
                    Nullable<bool> result = dlg.ShowDialog();

                    string filename = "";
                    // Process save file dialog box results
                    if (result == true)
                    {
                        // Save document
                        filename = dlg.FileName;

                        System.IO.File.WriteAllText(filename, sb.ToString(), Encoding.GetEncoding("ISO-8859-1"));

                        MessageBox.Show("Rapport de embarquement édité avec succès", "Rapport de embarquement édité !", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
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
