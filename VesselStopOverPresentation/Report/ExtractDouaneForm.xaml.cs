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
using Excel = Microsoft.Office.Interop.Excel;
using System.IO;
using System.IO.Compression;

namespace VesselStopOverPresentation
{
    /// <summary>
    /// Logique d'interaction pour ExtractDouaneForm.xaml
    /// </summary>
    public partial class ExtractDouaneForm : Window
    {

        private ReportingControlPanel reportPanel;

        private List<OPERATION> operationsUser;
        private UTILISATEUR utilisateur;

        private FormLoader formLoader;
        private VsomParameters vsp = new VsomParameters();
        public ExtractDouaneForm(ReportingControlPanel panel, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

                utilisateur = user;
                operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);

                reportPanel = panel;

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

        private void btnExtractDouane_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                if (cbTypeExtract.SelectedIndex == -1)
                {
                    MessageBox.Show("Veuillez sélectionner le type d'extraction à effectuer", "Sélectionnez le type d'extraction", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (!txtDateDebut.SelectedDate.HasValue)
                {
                    MessageBox.Show("Veuillez saisir la date de début", "Date début ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (!txtDateFin.SelectedDate.HasValue)
                {
                    MessageBox.Show("Veuillez saisir la date de fin", "Date fin ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    List<VEHICULE> listVehicules = new List<VEHICULE>();
                    if (cbTypeExtract.SelectedIndex == 0)
                    {
                        listVehicules = vsp.GetVehiculesExtractEntrees(txtDateDebut.SelectedDate.Value, txtDateFin.SelectedDate.Value, false);
                    }
                    else
                    {
                        listVehicules = vsp.GetVehiculesExtractSorties(txtDateDebut.SelectedDate.Value, txtDateFin.SelectedDate.Value, false);
                    }

                    StringBuilder sb = new StringBuilder();
                    DateTime dte = DateTime.Now;

                    foreach (VEHICULE veh in listVehicules)
                    {
                        sb.Append(veh.ESCALE.NumEsc.ToString()).Append(";");
                        sb.Append(veh.ESCALE.NAVIRE.NomNav).Append(";");
                        sb.Append(veh.MANIFESTE.IdMan.ToString()).Append(";");
                        sb.Append(veh.CONNAISSEMENT.NumBL.Length > 13 ? veh.CONNAISSEMENT.NumBL.Substring(veh.CONNAISSEMENT.NumBL.Length - 13, 13) : veh.CONNAISSEMENT.NumBL).Append(";");
                        sb.Append(veh.NumChassis).Append(";");
                        sb.Append(veh.StatutCVeh == "U" ? "2" : "1").Append(";");
                        sb.Append(Math.Round(veh.VolCVeh.Value, 2)).Append(";");
                        sb.Append("CMDLP").Append(";");
                        sb.Append(veh.NumDDVeh).Append(";");
                        sb.Append("20000101").Append(";");
                        sb.Append("1").Append(";");
                        sb.Append("NNNNNNNN").Append(";");
                        sb.Append("NNNNNNNN").Append(";");
                        sb.Append("20000101").Append(";");
                        sb.Append(veh.NumQDVeh).Append(";");
                        sb.Append("20000101").Append(";");
                        sb.Append("99999999");
                        sb.Append(Environment.NewLine);
                    }

                    System.IO.File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\Extract Douanes " + cbTypeExtract.Text + " - " + txtDateDebut.SelectedDate.Value.ToShortDateString().Replace("/", "") + " au " + txtDateFin.SelectedDate.Value.ToShortDateString().Replace("/", "") + ".txt", sb.ToString(), Encoding.GetEncoding("ISO-8859-1"));
                    MessageBox.Show("Extraction terminée", "Extraction terminée !", MessageBoxButton.OK, MessageBoxImage.Information);
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
        
    }
}
