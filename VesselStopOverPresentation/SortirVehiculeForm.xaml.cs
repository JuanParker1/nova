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
    /// Logique d'interaction pour SortirVehiculeForm.xaml
    /// </summary>
    public partial class SortirVehiculeForm : Window
    {

        private VehiculeForm vehForm;

        private VehiculePanel vehPanel;

        private VEHICULE vehicule;

        private List<OPERATION> operationsUser;
        private UTILISATEUR utilisateur;

        private FormLoader formLoader;
        private VsomParameters vsp = new VsomParameters();
        public SortirVehiculeForm(VehiculeForm form, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

                utilisateur = user;
                operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);

                vehForm = form;

                formLoader = new FormLoader(utilisateur);

                //if (utilisateur.LU != "Admin")
                //{
                //    txtDateSortie.IsEnabled = false;
                //}

                if (operationsUser.Where(op => op.NomOp == "Field : Autorisation sur date").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
                {
                    txtDateSortie.IsEnabled = false;
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

        public SortirVehiculeForm(VehiculePanel panel, VEHICULE veh, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

                vehicule = veh;

                utilisateur = user;
                operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);

                vehPanel = panel;

                formLoader = new FormLoader(utilisateur);

                //if (utilisateur.LU != "Admin")
                //{
                //    txtDateSortie.IsEnabled = false;
                //}

                if (operationsUser.Where(op => op.NomOp == "Field : Autorisation sur date").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
                {
                    txtDateSortie.IsEnabled = false;
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

        private void btnSortir_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                if (!txtDateSortie.SelectedDate.HasValue)
                {
                    MessageBox.Show("Veuillez entrer une date de sortie", "Echec de l'opération", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (vehForm != null)
                {
                    VEHICULE veh = vsomAcc.SortirVehicule(Convert.ToInt32(vehForm.txtIdChassis.Text), 3, txtDateSortie.SelectedDate.Value, new TextRange(txtObservations.Document.ContentStart, txtObservations.Document.ContentEnd).Text, utilisateur.IdU);
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
                            vehForm.vehPanel.vehicules = vsp.GetVehiculesImport();
                            vehForm.vehPanel.dataGrid.ItemsSource = vehForm.vehPanel.vehicules;
                        }
                    }
                    MessageBox.Show("Véhciule sorti", "Vehicule sorti !", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();
                }
                else if (vehPanel != null)
                {
                    VEHICULE veh = vsomAcc.SortirVehicule(vehicule.IdVeh, 3, txtDateSortie.SelectedDate.Value, new TextRange(txtObservations.Document.ContentStart, txtObservations.Document.ContentEnd).Text, utilisateur.IdU);
                    if (vehPanel.cbFiltres.SelectedIndex != 0)
                    {
                        vehPanel.cbFiltres.SelectedIndex = 0;
                    }
                    else
                    {
                        vehPanel.vehicules = vsp.GetVehiculesImport();
                        vehPanel.dataGrid.ItemsSource = vehPanel.vehicules;
                    }
                    MessageBox.Show("Véhciule sorti", "Vehicule sorti !", MessageBoxButton.OK, MessageBoxImage.Information);
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
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
    }
}
