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
    /// Logique d'interaction pour TransfertZoneSortieVehiculeForm.xaml
    /// </summary>
    public partial class TransfertZoneSortieVehiculeForm : Window
    {

        private VehiculeForm vehForm;
        private VehiculePanel vehPanel;

        private VEHICULE vehicule;

        private List<OPERATION> operationsUser;
        private UTILISATEUR utilisateur;

        private FormLoader formLoader;
        //private VsomParameters vsp = new VsomParameters();
        private VSOMAccessors vsomAcc;

        public TransfertZoneSortieVehiculeForm(VehiculeForm form, UTILISATEUR user)
        {
            try
            {
                 vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

                utilisateur = user;
                operationsUser = vsomAcc.GetOperationsUtilisateur(utilisateur.IdU);

                vehForm = form;

                formLoader = new FormLoader(utilisateur);

                //if (utilisateur.LU != "Admin")
                //{
                //    txtDateTransfert.IsEnabled = false;
                //}

                if (operationsUser.Where(op => op.NomOp == "Field : Autorisation sur date").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
                {
                    txtDateTransfert.IsEnabled = false;
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

        public TransfertZoneSortieVehiculeForm(VehiculePanel panel, VEHICULE veh, UTILISATEUR user)
        {
            try
            {
                //VSOMAccessors vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

                vehicule = veh;

                utilisateur = user;
                operationsUser = vsomAcc.GetOperationsUtilisateur(utilisateur.IdU);

                vehPanel = panel;

                formLoader = new FormLoader(utilisateur);

                //if (utilisateur.LU != "Admin")
                //{
                //    txtDateTransfert.IsEnabled = false;
                //}

                if (operationsUser.Where(op => op.NomOp == "Field : Autorisation sur date").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
                {
                    txtDateTransfert.IsEnabled = false;
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

        private void btnTransfertZoneSortie_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                vsomAcc = new VSOMAccessors(); 
                
                if (vehForm != null)
                {
                    VEHICULE veh = vsomAcc.TransfertZoneSortie(Convert.ToInt32(vehForm.txtIdChassis.Text), 3, txtDateTransfert.SelectedDate.Value, new TextRange(txtObservations.Document.ContentStart, txtObservations.Document.ContentEnd).Text, utilisateur.IdU);
                    //Raffraîchir les informations

                    formLoader.LoadVehiculeForm(vehForm, veh);
                    if (vehForm.vehPanel != null)
                    {
                        if (vehForm.vehPanel.cbFiltres.SelectedIndex != 0)
                        {
                            vehForm.vehPanel.cbFiltres.SelectedIndex = 0;
                        }
                        else
                        {
                            vehForm.vehPanel.vehicules = vsomAcc.GetVehiculesImport();
                            vehForm.vehPanel.dataGrid.ItemsSource = vehForm.vehPanel.vehicules;
                        }
                    }
                }
                else if (vehPanel != null)
                {
                    VEHICULE veh = vsomAcc.TransfertZoneSortie(vehicule.IdVeh, 3, txtDateTransfert.SelectedDate.Value, new TextRange(txtObservations.Document.ContentStart, txtObservations.Document.ContentEnd).Text, utilisateur.IdU);
                    if (vehPanel.cbFiltres.SelectedIndex != 0)
                    {
                        vehPanel.cbFiltres.SelectedIndex = 0;
                    }
                    else
                    {
                        vehPanel.vehicules = vsomAcc.GetVehiculesImport();
                        vehPanel.dataGrid.ItemsSource = vehPanel.vehicules;
                    }
                }

                MessageBox.Show("L'opération de transfert en zone de sortie s'est déroulée avec succès. Un bon de sortie a été généré et est disponible pour impression", "Véhicule tranféré en zone de sortie !", MessageBoxButton.OK, MessageBoxImage.Information);
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
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
            }
        }
    }
}
