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
    /// Logique d'interaction pour CubageVehiculeForm.xaml
    /// </summary>
    public partial class CubageVehiculeForm : Window
    {

        private VehiculeForm vehForm;
        private VehiculePanel vehPanel;
        private CubageForm cubForm;

        private VEHICULE vehicule;

        private List<TYPE_VEHICULE> typesVehicules;
        public List<string> tps { get; set; }

        private List<OPERATION> operationsUser;
        private UTILISATEUR utilisateur;

        private FormLoader formLoader;
       // private VsomParameters vsp = new VsomParameters();
        private VSOMAccessors vsomAcc;

        public CubageVehiculeForm(VehiculeForm form, UTILISATEUR user)
        {
            try
            {
                 vsomAcc = new VSOMAccessors();
                //VsomParameters vsprm = new VsomParameters();

                InitializeComponent();
                this.DataContext = this;

                typesVehicules = vsomAcc.GetTypesVehicules();
                tps = new List<string>();
                foreach (TYPE_VEHICULE t in typesVehicules)
                {
                    tps.Add(t.CodeTypeVeh);
                }

                utilisateur = user;
                operationsUser = vsomAcc.GetOperationsUtilisateur(utilisateur.IdU);

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

        public CubageVehiculeForm(VehiculePanel panel, VEHICULE veh, UTILISATEUR user)
        {
            try
            {
               // VSOMAccessors vsomAcc = new VSOMAccessors();
                //VsomParameters vsp = new VsomParameters();
                InitializeComponent();
                this.DataContext = this;

                typesVehicules = vsomAcc.GetTypesVehicules();
                tps = new List<string>();
                foreach (TYPE_VEHICULE t in typesVehicules)
                {
                    tps.Add(t.CodeTypeVeh);
                }

                vehicule = veh;

                utilisateur = user;
                operationsUser = vsomAcc.GetOperationsUtilisateur(utilisateur.IdU);

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

        public CubageVehiculeForm(CubageForm form, VEHICULE veh, UTILISATEUR user)
        {
            try
            {
                 vsomAcc = new VSOMAccessors();
                //VsomParameters vsp = new VsomParameters();
                InitializeComponent();
                this.DataContext = this;

                typesVehicules = vsomAcc.GetTypesVehicules();
                tps = new List<string>();
                foreach (TYPE_VEHICULE t in typesVehicules)
                {
                    tps.Add(t.CodeTypeVeh);
                }

                vehicule = veh;

                utilisateur = user;
                operationsUser = vsomAcc.GetOperationsUtilisateur(utilisateur.IdU);

                cubForm = form;

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

        private void cbTypeVehC_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                int indexType = cbTypeVehC.SelectedIndex;
                TYPE_VEHICULE t = typesVehicules.ElementAt<TYPE_VEHICULE>(indexType);
                txtTypeVehC.Text = t.LibTypeVeh + " - (De " + t.DeVol + " à " + t.LimVol + " m³)";
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }

        }

        private void btnCuber_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //VSOMAccessors vsomAcc = new VSOMAccessors();
                //VsomMarchal vsomAcc = new VsomMarchal();

                if (txtLongC.Text == "" && txtLargC.Text == "" && txtHautC.Text == "" && txtVolC.Text == "")
                {
                    MessageBox.Show("Veuillez remplir les dimensions du véhicule", "Dimensions", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    if (vehForm != null)
                    {
                        VEHICULE veh = vsomAcc.CuberVehicule(Convert.ToInt32(vehForm.txtIdChassis.Text), Convert.ToDouble(txtLongC.Text.Replace(".", ",")), Convert.ToDouble(txtLargC.Text.Replace(".", ",")), Convert.ToDouble(txtHautC.Text.Replace(".", ",")), Convert.ToDouble(txtVolC.Text.Replace(".", ",")), 3, new TextRange(txtObservations.Document.ContentStart, txtObservations.Document.ContentEnd).Text, utilisateur.IdU);
                        //Raffraîchir les informations
                        formLoader.LoadVehiculeForm(vehForm, veh);
                    }
                    else if (vehPanel != null)
                    {
                        VEHICULE veh = vsomAcc.CuberVehicule(vehicule.IdVeh, Convert.ToDouble(txtLongC.Text.Replace(".", ",")), Convert.ToDouble(txtLargC.Text.Replace(".", ",")), Convert.ToDouble(txtHautC.Text.Replace(".", ",")), Convert.ToDouble(txtVolC.Text.Replace(".", ",")), 3, new TextRange(txtObservations.Document.ContentStart, txtObservations.Document.ContentEnd).Text, utilisateur.IdU);
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
                    else if (cubForm != null)
                    {
                        VEHICULE veh = vsomAcc.CuberVehicule(vehicule.IdVeh, Convert.ToDouble(txtLongC.Text.Replace(".", ",")), Convert.ToDouble(txtLargC.Text.Replace(".", ",")), Convert.ToDouble(txtHautC.Text.Replace(".", ",")), Convert.ToDouble(txtVolC.Text.Replace(".", ",")), 3, new TextRange(txtObservations.Document.ContentStart, txtObservations.Document.ContentEnd).Text, utilisateur.IdU);

                        int idCub = Convert.ToInt32(cubForm.cbIdCub.Text);
                        formLoader.LoadCubageForm(cubForm, vsomAcc.GetCubageByIdCub(idCub));
                    }

                    MessageBox.Show("L'opération de cubage s'est déroulée avec succès, consultez le journal des éléments de facturation", "Véhicule cubé !", MessageBoxButton.OK, MessageBoxImage.Information);
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

            }
        }

        private void txtDim_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9,.]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void txtDim_LostFocus(object sender, RoutedEventArgs e)
        {
            // vérifier qu'il y a des valeurs dans les champs
            try
            {
                if (txtLongC.Text != "" && txtLargC.Text != "" && txtHautC.Text != "")
                {
                    txtVolC.Text = Math.Round((Convert.ToDouble(txtLongC.Text.Replace(".", ",")) * Convert.ToDouble(txtLargC.Text.Replace(".", ",")) * Convert.ToDouble(txtHautC.Text.Replace(".", ","))), 3).ToString();
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

        private void txtVolC_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                double vol = Convert.ToDouble(txtVolC.Text.Replace(".", ","));
                if (vol <= 15)
                {
                    cbTypeVehC.SelectedItem = "C";
                }
                else if (vol > 15 && vol <= 25)
                {
                    cbTypeVehC.SelectedItem = "V";
                }
                else if (vol > 25 && vol <= 70)
                {
                    cbTypeVehC.SelectedItem = "T";
                }
                else if (vol > 70 && vol <= 140)
                {
                    cbTypeVehC.SelectedItem = "L";
                }
                else if (vol > 140 && vol <= 32500)
                {
                    cbTypeVehC.SelectedItem = "P";
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
