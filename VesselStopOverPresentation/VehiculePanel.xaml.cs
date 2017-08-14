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
using System.Windows.Navigation;
using System.Windows.Shapes;
using VesselStopOverData;

namespace VesselStopOverPresentation
{
    /// <summary>
    /// Logique d'interaction pour VehiculePanel.xaml
    /// </summary>
    public partial class VehiculePanel : DockPanel
    {
        //VsomParameters vsp = new VsomParameters();
        VSOMAccessors vsomAcc;

        public VehiculePanel(UTILISATEUR user)
        {
            try
            {
                vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;
                listRechercher.SelectedIndex = 0;
                //cbFiltres.SelectedIndex = 0;
                //lblStatut.Content = vehicules.Count + " Vehicule(s)";

                actionsBorder.Visibility = System.Windows.Visibility.Collapsed;

                utilisateur = user;
                operationsUser = vsomAcc.GetOperationsUtilisateur(utilisateur.IdU);
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
            
        }

        public List<VEHICULE> vehicules { get; set; }
        private List<OPERATION> operationsUser;
        private UTILISATEUR utilisateur;

        private void dataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                //VSOMAccessors vsomAcc = new VSOMAccessors();
                VEHICULE veh = vsomAcc.GetVehiculeByIdVeh(((VEHICULE)dataGrid.SelectedItem).IdVeh);
                if (dataGrid.SelectedIndex != -1)
                {
                    VehiculeForm vehForm = new VehiculeForm(this, veh, utilisateur);
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

        private void cbFiltres_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                //VSOMAccessors vsomAcc = new VSOMAccessors();

                if (cbFiltres.SelectedIndex == 0)
                {
                    if (utilisateur.IdAcc == 1)
                    {
                        vehicules = vsomAcc.GetVehiculesImport();
                    }
                    else
                    {
                        vehicules = vsomAcc.GetVehiculesImport(utilisateur.IdAcc.Value);
                    }
                    
                    dataGrid.ItemsSource = vehicules;
                    lblStatut.Content = vehicules.Count + " Véhicule(s)";
                }
                else if (cbFiltres.SelectedIndex == 1)
                {
                    if (utilisateur.IdAcc == 1)
                    {
                        vehicules = vsomAcc.GetVehiculesImportByStatut("Non initié");
                    }
                    else
                    {
                        vehicules = vsomAcc.GetVehiculesImportByStatut("Non initié", utilisateur.IdAcc.Value);
                    }
                    
                    dataGrid.ItemsSource = vehicules;
                    lblStatut.Content = vehicules.Count + " Véhicule(s)";
                }
                else if (cbFiltres.SelectedIndex == 2)
                {
                    if (utilisateur.IdAcc == 1)
                    {
                        vehicules = vsomAcc.GetVehiculesImportByStatut("Traité");
                    }
                    else
                    {
                        vehicules = vsomAcc.GetVehiculesImportByStatut("Traité", utilisateur.IdAcc.Value);
                    }
                    
                    dataGrid.ItemsSource = vehicules;
                    lblStatut.Content = vehicules.Count + " Véhicule(s)";
                }
                else if (cbFiltres.SelectedIndex == 3)
                {
                    if (utilisateur.IdAcc == 1)
                    {
                        vehicules = vsomAcc.GetVehiculesImportByStatut("Manifesté");
                    }
                    else
                    {
                        vehicules = vsomAcc.GetVehiculesImportByStatut("Manifesté", utilisateur.IdAcc.Value);
                    }
                    
                    dataGrid.ItemsSource = vehicules;
                    lblStatut.Content = vehicules.Count + " Véhicule(s)";
                }
                else if (cbFiltres.SelectedIndex == 4)
                {
                    if (utilisateur.IdAcc == 1)
                    {
                        vehicules = vsomAcc.GetVehiculesImportByStatut("Identifié/Déchargé");
                    }
                    else
                    {
                        vehicules = vsomAcc.GetVehiculesImportByStatut("Identifié/Déchargé", utilisateur.IdAcc.Value);
                    }
                    
                    dataGrid.ItemsSource = vehicules;
                    lblStatut.Content = vehicules.Count + " Véhicule(s)";
                }
                else if (cbFiltres.SelectedIndex == 5)
                {
                    if (utilisateur.IdAcc == 1)
                    {
                        vehicules = vsomAcc.GetVehiculesImportByStatut("Parqué");
                    }
                    else
                    {
                        vehicules = vsomAcc.GetVehiculesImportByStatut("Parqué", utilisateur.IdAcc.Value);
                    }
                    
                    dataGrid.ItemsSource = vehicules;
                    lblStatut.Content = vehicules.Count + " Véhicule(s)";
                }
                else if (cbFiltres.SelectedIndex == 6)
                {
                    if (utilisateur.IdAcc == 1)
                    {
                        vehicules = vsomAcc.GetVehiculesImportByStatut("Enlèvement");
                    }
                    else
                    {
                        vehicules = vsomAcc.GetVehiculesImportByStatut("Enlèvement", utilisateur.IdAcc.Value);
                    }
                    
                    dataGrid.ItemsSource = vehicules;
                    lblStatut.Content = vehicules.Count + " Véhicule(s)";
                }
                else if (cbFiltres.SelectedIndex == 7)
                {
                    if (utilisateur.IdAcc == 1)
                    {
                        vehicules = vsomAcc.GetVehiculesImportByStatut("Livraison");
                    }
                    else
                    {
                        vehicules = vsomAcc.GetVehiculesImportByStatut("Livraison", utilisateur.IdAcc.Value);
                    }
                    
                    dataGrid.ItemsSource = vehicules;
                    lblStatut.Content = vehicules.Count + " Véhicule(s)";
                }
                else if (cbFiltres.SelectedIndex == 8)
                {
                    if (utilisateur.IdAcc == 1)
                    {
                        vehicules = vsomAcc.GetVehiculesImportByStatut("Sortie en cours");
                    }
                    else
                    {
                        vehicules = vsomAcc.GetVehiculesImportByStatut("Sortie en cours", utilisateur.IdAcc.Value);
                    }
                    
                    dataGrid.ItemsSource = vehicules;
                    lblStatut.Content = vehicules.Count + " Véhicule(s)";
                }
                else if (cbFiltres.SelectedIndex == 9)
                {
                    if (utilisateur.IdAcc == 1)
                    {
                        vehicules = vsomAcc.GetVehiculesImportByStatut("Livré");
                    }
                    else
                    {
                        vehicules = vsomAcc.GetVehiculesImportByStatut("Livré", utilisateur.IdAcc.Value);
                    }
                    
                    dataGrid.ItemsSource = vehicules;
                    lblStatut.Content = vehicules.Count + " Véhicule(s)";
                }
                else if (cbFiltres.SelectedIndex == 10)
                {
                    if (utilisateur.IdAcc == 1)
                    {
                        vehicules = vsomAcc.GetVehiculesEnCubage();
                    }
                    else
                    {
                        vehicules = vsomAcc.GetVehiculesEnCubageByIdAcc(utilisateur.IdAcc.Value);
                    }
                    
                    dataGrid.ItemsSource = vehicules;
                    lblStatut.Content = vehicules.Count + " Véhicule(s)";
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

        private void txtRechercher_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                //VSOMAccessors vsomAcc = new VSOMAccessors();

                if (e.Key == Key.Return && listRechercher.SelectedItem != null)
                {
                    if (listRechercher.SelectedIndex == 0)
                    {
                        if (cbFiltres.SelectedIndex == 0 || cbFiltres.SelectedIndex == -1)
                        {
                            if (utilisateur.IdAcc == 1)
                            {
                                vehicules = vsomAcc.GetVehiculesByNumChassis(txtRechercher.Text.Trim());
                            }
                            else
                            {
                                vehicules = vsomAcc.GetVehiculesByNumChassis(txtRechercher.Text.Trim(), utilisateur.IdAcc.Value);
                            }
                        }
                        else if (cbFiltres.SelectedIndex == 1)
                        {
                            if (utilisateur.IdAcc == 1)
                            {
                                vehicules = vsomAcc.GetVehiculesImportByStatut(txtRechercher.Text.Trim(), "Non initié");
                            }
                            else
                            {
                                vehicules = vsomAcc.GetVehiculesImportByStatut(txtRechercher.Text.Trim(), "Non initié", utilisateur.IdAcc.Value);
                            }
                        }
                        else if (cbFiltres.SelectedIndex == 2)
                        {
                            if (utilisateur.IdAcc == 1)
                            {
                                vehicules = vsomAcc.GetVehiculesImportByStatut(txtRechercher.Text.Trim(), "Traité");
                            }
                            else
                            {
                                vehicules = vsomAcc.GetVehiculesImportByStatut(txtRechercher.Text.Trim(), "Traité", utilisateur.IdAcc.Value);
                            }
                        }
                        else if (cbFiltres.SelectedIndex == 3)
                        {
                            if (utilisateur.IdAcc == 1)
                            {
                                vehicules = vsomAcc.GetVehiculesImportByStatut(txtRechercher.Text.Trim(), "Manifesté");
                            }
                            else
                            {
                                vehicules = vsomAcc.GetVehiculesImportByStatut(txtRechercher.Text.Trim(), "Manifesté", utilisateur.IdAcc.Value);
                            }
                        }
                        else if (cbFiltres.SelectedIndex == 4)
                        {
                            if (utilisateur.IdAcc == 1)
                            {
                                vehicules = vsomAcc.GetVehiculesImportByStatut(txtRechercher.Text.Trim(), "Identifié/Déchargé");
                            }
                            else
                            {
                                vehicules = vsomAcc.GetVehiculesImportByStatut(txtRechercher.Text.Trim(), "Identifié/Déchargé", utilisateur.IdAcc.Value);
                            }
                        }
                        else if (cbFiltres.SelectedIndex == 5)
                        {
                            if (utilisateur.IdAcc == 1)
                            {
                                vehicules = vsomAcc.GetVehiculesImportByStatut(txtRechercher.Text.Trim(), "Parqué");
                            }
                            else
                            {
                                vehicules = vsomAcc.GetVehiculesImportByStatut(txtRechercher.Text.Trim(), "Parqué", utilisateur.IdAcc.Value);
                            }
                        }
                        else if (cbFiltres.SelectedIndex == 6)
                        {
                            if (utilisateur.IdAcc == 1)
                            {
                                vehicules = vsomAcc.GetVehiculesImportByStatut(txtRechercher.Text.Trim(), "Enlèvement");
                            }
                            else
                            {
                                vehicules = vsomAcc.GetVehiculesImportByStatut(txtRechercher.Text.Trim(), "Enlèvement", utilisateur.IdAcc.Value);
                            }
                        }
                        else if (cbFiltres.SelectedIndex == 7)
                        {
                            if (utilisateur.IdAcc == 1)
                            {
                                vehicules = vsomAcc.GetVehiculesImportByStatut(txtRechercher.Text.Trim(), "Livraison");
                            }
                            else
                            {
                                vehicules = vsomAcc.GetVehiculesImportByStatut(txtRechercher.Text.Trim(), "Livraison", utilisateur.IdAcc.Value);
                            }
                        }
                        else if (cbFiltres.SelectedIndex == 8)
                        {
                            if (utilisateur.IdAcc == 1)
                            {
                                vehicules = vsomAcc.GetVehiculesImportByStatut(txtRechercher.Text.Trim(), "Sortie");
                            }
                            else
                            {
                                vehicules = vsomAcc.GetVehiculesImportByStatut(txtRechercher.Text.Trim(), "Sortie", utilisateur.IdAcc.Value);
                            }
                        }
                        else if (cbFiltres.SelectedIndex == 9)
                        {
                            if (utilisateur.IdAcc == 1)
                            {
                                vehicules = vsomAcc.GetVehiculesImportByStatut(txtRechercher.Text.Trim(), "Livré");
                            }
                            else
                            {
                                vehicules = vsomAcc.GetVehiculesImportByStatut(txtRechercher.Text.Trim(), "Livré", utilisateur.IdAcc.Value);
                            }
                        }
                        else if (cbFiltres.SelectedIndex == 10)
                        {
                            if (utilisateur.IdAcc == 1)
                            {
                                vehicules = vsomAcc.GetVehiculesEnCubage(txtRechercher.Text.Trim());
                            }
                            else
                            {
                                vehicules = vsomAcc.GetVehiculesEnCubageByIdAcc(txtRechercher.Text.Trim(), utilisateur.IdAcc.Value);
                            }
                        }
                    }
                    else if (listRechercher.SelectedIndex == 1)
                    {
                        if (cbFiltres.SelectedIndex == 0 || cbFiltres.SelectedIndex == -1)
                        {
                            if (utilisateur.IdAcc == 1)
                            {
                                vehicules = vsomAcc.GetVehiculesImportByNumBL(txtRechercher.Text.Trim());
                            }
                            else
                            {
                                vehicules = vsomAcc.GetVehiculesByNumBL(txtRechercher.Text.Trim(), utilisateur.IdAcc.Value);
                            }
                        }
                        else if (cbFiltres.SelectedIndex == 1)
                        {
                            if (utilisateur.IdAcc == 1)
                            {
                                vehicules = vsomAcc.GetVehiculesImportByNumBLAndStatut(txtRechercher.Text.Trim(), "Non initié");
                            }
                            else
                            {
                                vehicules = vsomAcc.GetVehiculesImportByNumBLAndStatut(txtRechercher.Text.Trim(), "Non initié", utilisateur.IdAcc.Value);
                            }
                        }
                        else if (cbFiltres.SelectedIndex == 2)
                        {
                            if (utilisateur.IdAcc == 1)
                            {
                                vehicules = vsomAcc.GetVehiculesImportByNumBLAndStatut(txtRechercher.Text.Trim(), "Traité");
                            }
                            else
                            {
                                vehicules = vsomAcc.GetVehiculesImportByNumBLAndStatut(txtRechercher.Text.Trim(), "Traité", utilisateur.IdAcc.Value);
                            }
                        }
                        else if (cbFiltres.SelectedIndex == 3)
                        {
                            if (utilisateur.IdAcc == 1)
                            {
                                vehicules = vsomAcc.GetVehiculesImportByNumBLAndStatut(txtRechercher.Text.Trim(), "Manifesté");
                            }
                            else
                            {
                                vehicules = vsomAcc.GetVehiculesImportByNumBLAndStatut(txtRechercher.Text.Trim(), "Manifesté", utilisateur.IdAcc.Value);
                            }
                        }
                        else if (cbFiltres.SelectedIndex == 4)
                        {
                            if (utilisateur.IdAcc == 1)
                            {
                                vehicules = vsomAcc.GetVehiculesImportByNumBLAndStatut(txtRechercher.Text.Trim(), "Identifié/Déchargé");
                            }
                            else
                            {
                                vehicules = vsomAcc.GetVehiculesImportByNumBLAndStatut(txtRechercher.Text.Trim(), "Identifié/Déchargé", utilisateur.IdAcc.Value);
                            }
                        }
                        else if (cbFiltres.SelectedIndex == 5)
                        {
                            if (utilisateur.IdAcc == 1)
                            {
                                vehicules = vsomAcc.GetVehiculesImportByNumBLAndStatut(txtRechercher.Text.Trim(), "Parqué");
                            }
                            else
                            {
                                vehicules = vsomAcc.GetVehiculesImportByNumBLAndStatut(txtRechercher.Text.Trim(), "Parqué", utilisateur.IdAcc.Value);
                            }
                        }
                        else if (cbFiltres.SelectedIndex == 6)
                        {
                            if (utilisateur.IdAcc == 1)
                            {
                                vehicules = vsomAcc.GetVehiculesImportByNumBLAndStatut(txtRechercher.Text.Trim(), "Enlèvement");
                            }
                            else
                            {
                                vehicules = vsomAcc.GetVehiculesImportByNumBLAndStatut(txtRechercher.Text.Trim(), "Enlèvement", utilisateur.IdAcc.Value);
                            }
                        }
                        else if (cbFiltres.SelectedIndex == 7)
                        {
                            if (utilisateur.IdAcc == 1)
                            {
                                vehicules = vsomAcc.GetVehiculesImportByNumBLAndStatut(txtRechercher.Text.Trim(), "Livraison");
                            }
                            else
                            {
                                vehicules = vsomAcc.GetVehiculesImportByNumBLAndStatut(txtRechercher.Text.Trim(), "Livraison", utilisateur.IdAcc.Value);
                            }
                        }
                        else if (cbFiltres.SelectedIndex == 8)
                        {
                            if (utilisateur.IdAcc == 1)
                            {
                                vehicules = vsomAcc.GetVehiculesImportByNumBLAndStatut(txtRechercher.Text.Trim(), "Sortie");
                            }
                            else
                            {
                                vehicules = vsomAcc.GetVehiculesImportByNumBLAndStatut(txtRechercher.Text.Trim(), "Sortie", utilisateur.IdAcc.Value);
                            }
                        }
                        else if (cbFiltres.SelectedIndex == 9)
                        {
                            if (utilisateur.IdAcc == 1)
                            {
                                vehicules = vsomAcc.GetVehiculesImportByNumBLAndStatut(txtRechercher.Text.Trim(), "Livré");
                            }
                            else
                            {
                                vehicules = vsomAcc.GetVehiculesImportByNumBLAndStatut(txtRechercher.Text.Trim(), "Livré", utilisateur.IdAcc.Value);
                            }
                        }
                        else if (cbFiltres.SelectedIndex == 10)
                        {
                            if (utilisateur.IdAcc == 1)
                            {
                                vehicules = vsomAcc.GetVehiculesEnCubageByNumBL(txtRechercher.Text.Trim());
                            }
                            else
                            {
                                vehicules = vsomAcc.GetVehiculesEnCubageByNumBLAndIdAcc(txtRechercher.Text.Trim(), utilisateur.IdAcc.Value);
                            }
                        }
                    }
                    else if (listRechercher.SelectedIndex == 2)
                    {
                        int result;
                        if (utilisateur.IdAcc == 1)
                        {
                            vehicules = vsomAcc.GetVehiculesByNumEscale(Int32.TryParse(txtRechercher.Text.Trim(), out result) ? result : -1);
                        }
                        else
                        {
                            vehicules = vsomAcc.GetVehiculesByNumEscale(Int32.TryParse(txtRechercher.Text.Trim(), out result) ? result : -1, utilisateur.IdAcc.Value);
                        }
                    }
                    else if (listRechercher.SelectedIndex == 3)
                    {
                        if (utilisateur.IdAcc == 1)
                        {
                            vehicules = vsomAcc.GetVehiculesByDescription(txtRechercher.Text.Trim());
                        }
                        else
                        {
                            vehicules = vsomAcc.GetVehiculesByDescription(txtRechercher.Text.Trim(), utilisateur.IdAcc.Value);
                        }
                    }
                    dataGrid.ItemsSource = vehicules;
                    lblStatut.Content = vehicules.Count + " Véhicule(s)";
                }
                else if (e.Key == Key.Escape)
                {
                    txtRechercher.Text = null;
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

        private void btnNouveau_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (operationsUser.Where(op => op.NomOp == "Véhicule : Enregistrement d'un nouvel élément").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
                {
                    MessageBox.Show("Vous n'avez pas les droits nécessaires pour créer un nouveau véhicule. Veuillez contacter un administrateur", "Droits insuffisants !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    VehiculeForm vehForm = new VehiculeForm("Nouveau", this, utilisateur);
                    vehForm.cbNumChassis.IsEditable = true;
                    vehForm.txtLongM.Text = "0";
                    vehForm.txtLargM.Text = "0";
                    vehForm.txtHautM.Text = "0";
                    vehForm.txtVolM.Text = "0";
                    vehForm.txtPoidsM.Text = "0";
                    vehForm.txtEtatM.SelectedIndex = 0;
                    vehForm.Title = "Nouveau : Véhicule";
                    vehForm.actionsBorder.Visibility = System.Windows.Visibility.Collapsed;
                    vehForm.borderEtat.Visibility = System.Windows.Visibility.Collapsed;
                    vehForm.txtBarCode.IsReadOnly = false;
                    vehForm.txtBarCode.Background = Brushes.White;
                    vehForm.txtLongM.IsReadOnly = false;
                    vehForm.txtLongM.Background = Brushes.White;
                    vehForm.txtLargM.IsReadOnly = false;
                    vehForm.txtLargM.Background = Brushes.White;
                    vehForm.txtHautM.IsReadOnly = false;
                    vehForm.txtHautM.Background = Brushes.White;
                    vehForm.txtVolM.IsReadOnly = false;
                    vehForm.txtVolM.Background = Brushes.White;
                    vehForm.txtPoidsM.IsReadOnly = false;
                    vehForm.txtPoidsM.Background = Brushes.White;
                    vehForm.txtEtatM.IsReadOnly = false;
                    vehForm.txtEtatM.Background = Brushes.White;
                    vehForm.cbNumBL.IsEditable = true;
                    vehForm.groupInfosCub.IsEnabled = false;
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

        private void btnAnnulerRecherche_Click(object sender, RoutedEventArgs e)
        {
            txtRechercher.Text = null;
        }

        private void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (dataGrid.SelectedIndex != -1 && dataGrid.SelectedItems.Count == 1 && utilisateur.IdAcc == 1)
                {
                    actionsBorder.Visibility = System.Windows.Visibility.Visible;
                    VEHICULE veh = (VEHICULE)dataGrid.SelectedItem;

                    btnIdentifier.IsEnabled = false;
                    btnCuber.IsEnabled = false;
                    btnConstatSinistre.IsEnabled = false;
                    btnReceptionner.IsEnabled = false;
                    btnTransfertZoneSortie.IsEnabled = false;
                    btnTransfEmpl.IsEnabled = false;
                    btnSortir.IsEnabled = false;

                    if (veh.StatVeh == "Non initié")
                    {
                        btnIdentifier.IsEnabled = true;
                        btnReceptionner.IsEnabled = true;
                    }
                    else if (veh.StatVeh == "Initié")
                    {
                        btnIdentifier.IsEnabled = true;
                        btnReceptionner.IsEnabled = true;
                    }
                    else if (veh.StatVeh == "Traité")
                    {
                        btnIdentifier.IsEnabled = true;
                    }
                    else if (veh.StatVeh == "Manifesté")
                    {
                        btnIdentifier.IsEnabled = true;
                    }
                    else if (veh.StatVeh == "Identifié/Déchargé")
                    {
                        btnIdentifier.IsEnabled = true;
                        btnCuber.IsEnabled = true;
                        btnConstatSinistre.IsEnabled = true;
                        btnReceptionner.IsEnabled = true;
                    }
                    else if (veh.StatVeh == "Parqué")
                    {
                        btnTransfEmpl.IsEnabled = true;
                        btnCuber.IsEnabled = true;
                        btnConstatSinistre.IsEnabled = true;
                    }
                    else if (veh.StatVeh == "Enlèvement")
                    {
                        btnTransfEmpl.IsEnabled = true;
                        btnConstatSinistre.IsEnabled = true;
                    }
                    else if (veh.StatVeh == "Livraison")
                    {
                        btnTransfEmpl.IsEnabled = true;
                        btnTransfertZoneSortie.IsEnabled = true;
                        btnConstatSinistre.IsEnabled = true;
                    }
                    else if (veh.StatVeh == "Sortie en cours" || veh.StatVeh == "Zone Sortie")
                    {
                        //btnSortir.IsEnabled = true;
                    }
                    else if (veh.StatVeh == "Livré")
                    {

                    }
                }
                else
                {
                    actionsBorder.Visibility = System.Windows.Visibility.Collapsed;
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

        private void btnIdentifier_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //VSOMAccessors vsomAcc = new VSOMAccessors();

                if (dataGrid.SelectedIndex != -1 && dataGrid.SelectedItems.Count == 1)
                {
                    VEHICULE veh = vsomAcc.GetVehiculeByIdVeh(((VEHICULE)dataGrid.SelectedItem).IdVeh);
                    IdentificationVehiculeForm identForm = new IdentificationVehiculeForm(this, veh, utilisateur);
                    identForm.cbNumChassisAP.IsEnabled = false;
                    identForm.Title = "Identification - Chassis N° : " + veh.NumChassis;
                    identForm.ShowDialog();
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
                //VSOMAccessors vsomAcc = new VSOMAccessors();

                if (dataGrid.SelectedIndex != -1 && dataGrid.SelectedItems.Count == 1)
                {
                    VEHICULE veh = vsomAcc.GetVehiculeByIdVeh(((VEHICULE)dataGrid.SelectedItem).IdVeh);
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

        private void btnReceptionner_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //VSOMAccessors vsomAcc = new VSOMAccessors();

                if (dataGrid.SelectedIndex != -1 && dataGrid.SelectedItems.Count == 1)
                {
                    VEHICULE veh = vsomAcc.GetVehiculeByIdVeh(((VEHICULE)dataGrid.SelectedItem).IdVeh);
                    ReceptionVehiculeForm receptionForm = new ReceptionVehiculeForm(this, veh, utilisateur);
                    receptionForm.Title = "Réception - Chassis N° : " + veh.NumChassis;
                    receptionForm.ShowDialog();
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

        private void btnTransfEmpl_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //VSOMAccessors vsomAcc = new VSOMAccessors();

                if (dataGrid.SelectedIndex != -1 && dataGrid.SelectedItems.Count == 1)
                {
                    VEHICULE veh = vsomAcc.GetVehiculeByIdVeh(((VEHICULE)dataGrid.SelectedItem).IdVeh);
                    TransfertEmplacementForm transfertForm = new TransfertEmplacementForm(this, veh, utilisateur);
                    transfertForm.Title = "Transfert - Chassis N° : " + veh.NumChassis;
                    transfertForm.ShowDialog();
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
                //VSOMAccessors vsomAcc = new VSOMAccessors();

                if (dataGrid.SelectedIndex != -1 && dataGrid.SelectedItems.Count == 1)
                {
                    VEHICULE veh = vsomAcc.GetVehiculeByIdVeh(((VEHICULE)dataGrid.SelectedItem).IdVeh);
                    TransfertZoneSortieVehiculeForm transfertForm = new TransfertZoneSortieVehiculeForm(this, veh, utilisateur);
                    transfertForm.Title = "Transfert en zone de sortie - Chassis N° : " + veh.NumChassis;
                    transfertForm.ShowDialog();
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
                //VSOMAccessors vsomAcc = new VSOMAccessors();

                if (dataGrid.SelectedIndex != -1 && dataGrid.SelectedItems.Count == 1)
                {
                    VEHICULE veh = vsomAcc.GetVehiculeByIdVeh(((VEHICULE)dataGrid.SelectedItem).IdVeh);
                    SortirVehiculeForm sortieForm = new SortirVehiculeForm(this, veh, utilisateur);
                    sortieForm.Title = "Sortie - Chassis N° : " + veh.NumChassis;
                    sortieForm.ShowDialog();
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

        private void btnConstatSinistre_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //VSOMAccessors vsomAcc = new VSOMAccessors();

                if (dataGrid.SelectedIndex != -1 && dataGrid.SelectedItems.Count == 1)
                {
                    VEHICULE veh = vsomAcc.GetVehiculeByIdVeh(((VEHICULE)dataGrid.SelectedItem).IdVeh);
                    ConstatSinistreVehiculeForm sinistreForm = new ConstatSinistreVehiculeForm(this, veh, utilisateur);
                    sinistreForm.Title = "Constat de sinistre - Chassis N° : " + veh.NumChassis;
                    sinistreForm.ShowDialog();
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
