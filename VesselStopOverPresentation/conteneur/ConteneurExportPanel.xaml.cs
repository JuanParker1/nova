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
    /// Logique d'interaction pour ConteneurExportPanel.xaml
    /// </summary>
    public partial class ConteneurExportPanel : DockPanel
    {
        public List<CONTENEUR> conteneurs { get; set; }

        private List<OPERATION> operationsUser;
        private UTILISATEUR utilisateur;
        private VsomParameters vsp = new VsomParameters();

        public ConteneurExportPanel(UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;
                listRechercher.SelectedIndex = 0;
                //cbFiltres.SelectedIndex = 0;
                //lblStatut.Content = conteneurs.Count + " Conteneur(s)";

                actionsBorder.Visibility = System.Windows.Visibility.Collapsed;

                utilisateur = user;
                operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);
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
                VSOMAccessors vsomAcc = new VSOMAccessors();

                if (cbFiltres.SelectedIndex == 0)
                {
                    conteneurs = vsp.GetConteneursExport();
                    dataGrid.ItemsSource = conteneurs;
                    lblStatut.Content = conteneurs.Count + " Conteneur(s)";
                }
                else if (cbFiltres.SelectedIndex == 1)
                {
                    conteneurs = vsp.GetConteneursExportByStatut("Non initié");
                    dataGrid.ItemsSource = conteneurs;
                    lblStatut.Content = conteneurs.Count + " Conteneur(s)";
                }
                else if (cbFiltres.SelectedIndex == 2)
                {
                    conteneurs = vsp.GetConteneursExportByStatut("Clearance");
                    dataGrid.ItemsSource = conteneurs;
                    lblStatut.Content = conteneurs.Count + " Conteneur(s)";
                }
                else if (cbFiltres.SelectedIndex == 3)
                {
                    conteneurs = vsp.GetConteneursExportByStatut("Final Booking");
                    dataGrid.ItemsSource = conteneurs;
                    lblStatut.Content = conteneurs.Count + " Conteneur(s)";
                }
                else if (cbFiltres.SelectedIndex == 4)
                {
                    conteneurs = vsp.GetConteneursExportByStatut("Cargo Loading");
                    dataGrid.ItemsSource = conteneurs;
                    lblStatut.Content = conteneurs.Count + " Conteneur(s)";
                }
                else if (cbFiltres.SelectedIndex == 5)
                {
                    conteneurs = vsp.GetConteneursExportByStatut("Cargo Loaded");
                    dataGrid.ItemsSource = conteneurs;
                    lblStatut.Content = conteneurs.Count + " Conteneur(s)";
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
                VSOMAccessors vsomAcc = new VSOMAccessors();

                if (e.Key == Key.Return && listRechercher.SelectedItem != null)
                {
                    if (listRechercher.SelectedIndex == 0)
                    {
                        conteneurs = vsp.GetConteneursExportByNumBooking(txtRechercher.Text.Trim());
                        dataGrid.ItemsSource = conteneurs;
                        lblStatut.Content = conteneurs.Count + " Conteneur(s) trouvé(s)";
                    }
                    else if (listRechercher.SelectedIndex == 1)
                    {
                        conteneurs = vsp.GetConteneursExportByNumCtr(txtRechercher.Text.Trim());
                        dataGrid.ItemsSource = conteneurs;
                        lblStatut.Content = conteneurs.Count + " Conteneur(s) trouvé(s)";
                    }
                    else if (listRechercher.SelectedIndex == 2)
                    {
                        int result;
                        conteneurs = vsp.GetConteneursExportByNumEscale(Int32.TryParse(txtRechercher.Text.Trim(), out result) ? result : -1);
                        dataGrid.ItemsSource = conteneurs;
                        lblStatut.Content = conteneurs.Count + " Conteneur(s) trouvé(s)";
                    }
                    else if (listRechercher.SelectedIndex == 3)
                    {
                        conteneurs = vsp.GetConteneursExportByDescription(txtRechercher.Text.Trim());
                        dataGrid.ItemsSource = conteneurs;
                        lblStatut.Content = conteneurs.Count + " Conteneur(s) trouvé(s)";
                    }
                    else if (listRechercher.SelectedIndex == 4)
                    {
                        conteneurs = vsp.GetConteneursExportByPortDest(txtRechercher.Text.Trim());
                        dataGrid.ItemsSource = conteneurs;
                        lblStatut.Content = conteneurs.Count + " Conteneur(s) trouvé(s)";
                    }
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

        private void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();
                CONTENEUR ctr = vsp.GetConteneurExportByIdCtr(((CONTENEUR)dataGrid.SelectedItem).IdCtr);
                if (dataGrid.SelectedIndex != -1 && dataGrid.SelectedItems.Count == 1 && ctr.CONNAISSEMENT.DVCBLI.HasValue && (ctr.StatCtr == "Cargo Loading" || ctr.StatCtr == "Final Booking"))
                {
                    actionsBorder.Visibility = System.Windows.Visibility.Visible;
                    if (ctr.StatCtr == "Final Booking")
                    {
                        btnReceptionner.Visibility = System.Windows.Visibility.Visible;
                        btnEmbarquer.Visibility = System.Windows.Visibility.Collapsed;
                    }
                    else if (ctr.StatCtr == "Cargo Loading")
                    {
                        btnReceptionner.Visibility = System.Windows.Visibility.Collapsed;
                        btnEmbarquer.Visibility = System.Windows.Visibility.Visible;
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

        private void btnReceptionner_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                if (dataGrid.SelectedIndex != -1 && dataGrid.SelectedItems.Count == 1)
                {
                    CONTENEUR ctr = vsp.GetConteneurExportByIdCtr(((CONTENEUR)dataGrid.SelectedItem).IdCtr);
                    ReceptionExportConteneurForm receptionForm = new ReceptionExportConteneurForm(this, ctr, utilisateur);
                    receptionForm.txtSeal1.Text = ctr.Seal1Ctr;
                    receptionForm.txtSeal2.Text = ctr.Seal2Ctr;
                    receptionForm.Title = "Réception - Conteneur N° : " + ctr.NumCtr;
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

        private void btnEmbarquer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                if (dataGrid.SelectedIndex != -1 && dataGrid.SelectedItems.Count == 1)
                {
                    CONTENEUR ctr = vsp.GetConteneurExportByIdCtr(((CONTENEUR)dataGrid.SelectedItem).IdCtr);
                    EmbarquerConteneurForm embarqForm = new EmbarquerConteneurForm(this, ctr, utilisateur);
                    embarqForm.txtSeal1.Text = ctr.Seal1Ctr;
                    embarqForm.txtSeal2.Text = ctr.Seal2Ctr;
                    embarqForm.Title = "Embarquement - Conteneur N° : " + ctr.NumCtr;
                    embarqForm.ShowDialog();
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
