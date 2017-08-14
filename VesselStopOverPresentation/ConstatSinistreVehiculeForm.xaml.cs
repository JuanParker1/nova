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
    /// Logique d'interaction pour ConstatSinistreVehiculeForm.xaml
    /// </summary>
    public partial class ConstatSinistreVehiculeForm : Window
    {
        public List<TypeSinistreVehicule> typesSinistres { get; set; }

        private VehiculeForm vehForm;
        private VehiculePanel vehPanel;
        private VEHICULE vehicule;
        private List<OPERATION> operationsUser;
        private UTILISATEUR utilisateur;

        private FormLoader formLoader;
        private VsomParameters vsp = new VsomParameters();

        public ConstatSinistreVehiculeForm(VehiculeForm form, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();
                //VsomParameters vsprm = new VsomParameters();
                InitializeComponent();
                this.DataContext = this;

                typesSinistres = vsp.GetTypesSinistreVeh();

                utilisateur = user;
                operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);

                vehForm = form;

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

        public ConstatSinistreVehiculeForm(VehiculePanel panel, VEHICULE veh, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();
                VsomParameters vsp = new VsomParameters();
                InitializeComponent();
                this.DataContext = this;

                typesSinistres = vsp.GetTypesSinistreVeh();

                vehicule = veh;

                utilisateur = user;
                operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);

                vehPanel = panel;

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

        private void btnConstatSinistre_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //VSOMAccessors vsomAcc = new VSOMAccessors();
                VsomMarchal vsomAcc = new VsomMarchal();
                txtTest.Focus();
                VEHICULE veh = null;
                if (vehForm != null)
                {
                    if (dataGridSinistres.SelectedItems.Count == 0)
                    {
                        MessageBox.Show("Veuillez sélectionner au moins un sinistre", "Sinistre ?", MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }
                    else
                    {
                        veh = vsomAcc.ConstaterSinistres(Convert.ToInt32(vehForm.txtIdChassis.Text), dataGridSinistres.Items.OfType<TypeSinistreVehicule>().ToList<TypeSinistreVehicule>(), utilisateur.IdU, "");

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
                    }
                }
                else if (vehPanel != null)
                {
                    if (dataGridSinistres.SelectedItems.Count == 0)
                    {
                        MessageBox.Show("Veuillez sélectionner au moins un sinistre", "Sinistre ?", MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }
                    else
                    {
                        veh = vsomAcc.ConstaterSinistres(vehicule.IdVeh, dataGridSinistres.Items.OfType<TypeSinistreVehicule>().ToList<TypeSinistreVehicule>(), utilisateur.IdU, "");
                        //Raffraîchir les informations
                        if (vehPanel.cbFiltres.SelectedIndex != 0)
                        {
                            vehPanel.cbFiltres.SelectedIndex = 0;
                        }
                        else
                        {
                            vehPanel.vehicules = vsp.GetVehiculesImport();
                            vehPanel.dataGrid.ItemsSource = vehPanel.vehicules;
                        }
                    }
                }

                MessageBox.Show("L'opération de constat de sinistre s'est déroulée avec succès", "Constat de sinistre effectué !", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (HabilitationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération", MessageBoxButton.OK, MessageBoxImage.Error);
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
