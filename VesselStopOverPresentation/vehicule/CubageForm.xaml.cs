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

namespace VesselStopOverPresentation
{
    /// <summary>
    /// Logique d'interaction pour CubageForm.xaml
    /// </summary>
    public partial class CubageForm : Window
    {
        public List<ESCALE> escales { get; set; }
        public List<Int32> escs { get; set; }

        public List<CUBAGE> cubages { get; set; }
        public List<Int32> cubs { get; set; }

        public List<VehiculeCubage> vehicules { get; set; }

        private CubagePanel cubPanel;

        private List<OPERATION> operationsUser;
        private UTILISATEUR utilisateur;

        private FormLoader formLoader;
       // private VsomParameters vsp = new VsomParameters();
        private VSOMAccessors vsomAcc;
        public CubageForm(CubagePanel panel, CUBAGE cub, UTILISATEUR user)
        {
            try
            {
                 vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

                cubPanel = panel;

                btnCuber.Visibility = System.Windows.Visibility.Collapsed;
                btnValiderCubage.Visibility = System.Windows.Visibility.Collapsed;

                utilisateur = user;
                operationsUser = vsomAcc.GetOperationsUtilisateur(utilisateur.IdU);

                formLoader = new FormLoader(utilisateur);

                formLoader.LoadCubageForm(this, cub);
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
            
        }

        public CubageForm(string type, CubagePanel panel, UTILISATEUR user)
        {
            try
            {
                 vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

                borderActions.Visibility = System.Windows.Visibility.Collapsed;
                borderEtat.Visibility = System.Windows.Visibility.Collapsed;

                cubPanel = panel;

                btnCuber.Visibility = System.Windows.Visibility.Collapsed;
                btnValiderCubage.Visibility = System.Windows.Visibility.Collapsed;

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
            if (cbNumEscale.SelectedIndex == -1)
            {
                MessageBox.Show("Vous devez indiquer une escale pour créer un projet de cubage", "Escale ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            else if (!txtDateExecution.SelectedDate.HasValue)
            {
                MessageBox.Show("Veuillez indiquer une date d'exécution de ce projet de cubage", "Date d'exécution ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            else if(dataGridVehicules.Items.Count == 0)
            {
                MessageBox.Show("Veuillez ajouter au moins un véhicule à ce projet de cubage", "Véhicules à cuber ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            else
            {
                try
                {
                    if (cbIdCub.IsEnabled == false)
                    {
                         vsomAcc = new VSOMAccessors();
                        //VsomMarchal vsomAcc = new VsomMarchal();

                        List<VehiculeCubage> listVeh = dataGridVehicules.SelectedItems.OfType<VehiculeCubage>().ToList<VehiculeCubage>();

                        CUBAGE cub = vsomAcc.InsertProjetCubage(DateTime.Now, txtDateExecution.SelectedDate.Value, new TextRange(txtRemarques.Document.ContentStart, txtRemarques.Document.ContentEnd).Text, listVeh, Convert.ToInt32(txtEscaleSysID.Text), utilisateur.IdU);

                        if (cubPanel.cbFiltres.SelectedIndex != 1)
                        {
                            cubPanel.cbFiltres.SelectedIndex = 1;
                        }
                        else
                        {
                            cubPanel.cubages = vsomAcc.GetCubagesEnCours();
                            cubPanel.dataGrid.ItemsSource = cubPanel.cubages;
                            cubPanel.lblStatut.Content = cubPanel.cubages.Count + " Projet(s) de cubage en cours";
                        }

                        cbIdCub.IsEnabled = true;

                        formLoader.LoadCubageForm(this, cub);

                        borderActions.Visibility = System.Windows.Visibility.Visible;
                        btnCloturerCub.Visibility = System.Windows.Visibility.Collapsed;
                        borderEtat.Visibility = System.Windows.Visibility.Visible;
                        MessageBox.Show("Enregistrement effectué avec succès.", "Enregistrement effectué !", MessageBoxButton.OK, MessageBoxImage.Information);
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

        private void btnCloturerCub_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                 vsomAcc = new VSOMAccessors();
                //VsomMarchal vsomAcc = new VsomMarchal();

                CUBAGE cub = vsomAcc.CloturerProjetCubage(Convert.ToInt32(cbIdCub.Text), utilisateur.IdU);

                if (cubPanel.cbFiltres.SelectedIndex != 2)
                {
                    cubPanel.cbFiltres.SelectedIndex = 2;
                }
                else
                {
                    cubPanel.cubages = vsomAcc.GetCubagesEnCours();
                    cubPanel.dataGrid.ItemsSource = cubPanel.cubages;
                    cubPanel.lblStatut.Content = cubPanel.cubages.Count + " Projet(s) de cubage en clôturé(s)";
                }

                formLoader.LoadCubageForm(this, cub);

                borderActions.Visibility = System.Windows.Visibility.Visible;
                borderEtat.Visibility = System.Windows.Visibility.Visible;
                MessageBox.Show("Cubage clôturé avec succès.", "Cubage clôturé !", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (CubageException ex)
            {
                MessageBox.Show(ex.Message, "Véhicules non cubés", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        private void dataGridVehicules_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                vsomAcc = new VSOMAccessors();

                if (dataGridVehicules.SelectedIndex != -1)
                {
                    VehiculeForm vehForm = new VehiculeForm(this, vsomAcc.GetVehiculeByIdVeh(((VehiculeCubage)dataGridVehicules.SelectedItem).IdVeh), utilisateur);
                    vehForm.Show();
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

        private void btnCuber_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                vsomAcc = new VSOMAccessors();

                if (dataGridVehicules.SelectedIndex != -1 && dataGridVehicules.SelectedItems.Count == 1)
                {
                    VEHICULE veh = vsomAcc.GetVehiculeByIdVeh(((VehiculeCubage)dataGridVehicules.SelectedItem).IdVeh);
                    CubageVehiculeForm cubForm = new CubageVehiculeForm(this, veh, utilisateur);
                    cubForm.Title = "Cubage - Chassis N° : " + veh.NumChassis;
                    cubForm.ShowDialog();
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

        private void dataGridVehicules_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (dataGridVehicules.SelectedIndex != -1 && dataGridVehicules.SelectedItems.Count == 1)
                {
                    btnCuber.Visibility = System.Windows.Visibility.Collapsed;
                    btnValiderCubage.Visibility = System.Windows.Visibility.Collapsed;
                    if (((VehiculeCubage)dataGridVehicules.SelectedItem).DateVal.HasValue)
                    {
                        btnCuber.Visibility = System.Windows.Visibility.Collapsed;
                        btnValiderCubage.Visibility = System.Windows.Visibility.Collapsed;
                    }
                    else if (((VehiculeCubage)dataGridVehicules.SelectedItem).VolumeCube == 0)
                    {
                        btnCuber.Visibility = System.Windows.Visibility.Visible;
                    }
                    else if (((VehiculeCubage)dataGridVehicules.SelectedItem).VolumeCube != 0 && !((VehiculeCubage)dataGridVehicules.SelectedItem).DateVal.HasValue)
                    {
                        btnValiderCubage.Visibility = System.Windows.Visibility.Visible;
                    }
                }
                else
                {
                    btnCuber.Visibility = System.Windows.Visibility.Collapsed;
                    btnValiderCubage.Visibility = System.Windows.Visibility.Collapsed;
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

        private void btnEtat_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CubageReport cubReport = new CubageReport(this);
                cubReport.Title = "Impression du projet de cubage : " + cbIdCub.Text + " - Escale : " + cbNumEscale.Text;
                cubReport.Show();
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        private void cbIdCub_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                //VSOMAccessors vsomAcc = new VSOMAccessors();

                if (e.Key == Key.Return && cbIdCub.Text.Trim() != "")
                {
                    int result;
                    cubages = new List<CUBAGE>();

                    CUBAGE cb = vsomAcc.GetCubageByIdCub(Int32.TryParse(cbIdCub.Text.Trim(), out result) ? result : -1);

                    if (cb != null)
                    {
                        cubages.Add(cb);
                    }

                    if (cubages.Count == 0)
                    {
                        MessageBox.Show("Il n'existe aucun projet de cubage portant ce numéro", "Projet de introuvable", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                    else if (cubages.Count == 1)
                    {
                        CUBAGE cub = cubages.FirstOrDefault<CUBAGE>();
                        formLoader.LoadCubageForm(this, cub);
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

        private void btnValiderCubage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                VehiculeCubage vehCub = (VehiculeCubage)dataGridVehicules.SelectedItem;
                ValiderCubageForm validCub = new ValiderCubageForm(this, vehCub.IdVeh, utilisateur);
                validCub.txtVolAncien.Text = Math.Round(vehCub.VolumeManifeste, 3).ToString();
                validCub.txtVolCube.Text = Math.Round(vehCub.VolumeCube, 3).ToString();
                if (vehCub.VolumeCube >= vehCub.VolumeManifeste)
                {
                    validCub.radioCube.IsChecked = true;
                }
                else
                {
                    validCub.radioAncien.IsChecked = true;
                }
                validCub.Title = "Validation du cubage : " + vehCub.NumChassis;
                validCub.ShowDialog();
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        private void cbFiltres_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                //VSOMAccessors vsomAcc = new VSOMAccessors();

                vehicules = vsomAcc.GetVehiculesEnCubage(vsomAcc.GetCubageByIdCub(Convert.ToInt32(cbIdCub.Text)).IdCubage);

                if (cbFiltres.SelectedIndex == 0)
                {
                    dataGridVehicules.ItemsSource = vehicules;
                }
                else if (cbFiltres.SelectedIndex == 1)
                {
                    dataGridVehicules.ItemsSource = vehicules.Where(v => !v.IsCubed);
                }
                else if (cbFiltres.SelectedIndex == 2)
                {
                    dataGridVehicules.ItemsSource = vehicules.Where(v => v.IsCubed && !v.DateVal.HasValue);
                }
                else if (cbFiltres.SelectedIndex == 3)
                {
                    dataGridVehicules.ItemsSource = vehicules.Where(v => v.DateVal.HasValue);
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

        private void btnRetirer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //VSOMAccessors vsomAcc = new VSOMAccessors();

                RetirerVehiculeCubageForm retirerVehCub = new RetirerVehiculeCubageForm(this, vsomAcc.GetVehiculesNonCubesOfCubage(Convert.ToInt32(cbIdCub.Text)), utilisateur);
                retirerVehCub.Title = "Retrait de véhicules du projet de cubage : " + cbIdCub.Text;
                retirerVehCub.ShowDialog();
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        private void btnAjout_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //VSOMAccessors vsomAcc = new VSOMAccessors();

                AjoutVehiculeCubageForm ajoutVehCub = new AjoutVehiculeCubageForm(this, vsomAcc.GetVehiculesEscalePourCubage(Convert.ToInt32(txtEscaleSysID.Text)), utilisateur);
                ajoutVehCub.Title = "Ajout de véhicules au projet de cubage : " + cbIdCub.Text;
                ajoutVehCub.ShowDialog();
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
                //VSOMAccessors vsomAcc = new VSOMAccessors();

                if (e.Key == Key.Return && cbNumEscale.Text.Trim() != "")
                {
                    int result;
                    escales = vsomAcc.GetEscalesByNumEscale(Int32.TryParse(cbNumEscale.Text.Trim(), out result) ? result : -1);

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
    }
}
