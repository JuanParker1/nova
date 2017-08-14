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
    /// Logique d'interaction pour ConventionnelExportPanel.xaml
    /// </summary>
    public partial class ConventionnelExportPanel : DockPanel
    {
        public List<CONVENTIONNEL> conventionnels { get; set; }
        private List<OPERATION> operationsUser;
        private UTILISATEUR utilisateur;
        private VsomParameters vsp = new VsomParameters();

        public ConventionnelExportPanel(UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;
                listRechercher.SelectedIndex = 0;
                //cbFiltres.SelectedIndex = 0;
                //lblStatut.Content = conventionnels.Count + " Conventionnel(s)";

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
                    conventionnels = vsp.GetConventionnelsExport();
                    dataGrid.ItemsSource = conventionnels;
                    lblStatut.Content = conventionnels.Count + " Conventionnel(s)";
                }
                else if (cbFiltres.SelectedIndex == 1)
                {
                    conventionnels = vsp.GetConventionnelsExportByStatut("Non initié");
                    dataGrid.ItemsSource = conventionnels;
                    lblStatut.Content = conventionnels.Count + " Conventionnel(s)";
                }
                else if (cbFiltres.SelectedIndex == 2)
                {
                    conventionnels = vsp.GetConventionnelsExportByStatut("Clearance");
                    dataGrid.ItemsSource = conventionnels;
                    lblStatut.Content = conventionnels.Count + " Conventionnel(s)";
                }
                else if (cbFiltres.SelectedIndex == 3)
                {
                    conventionnels = vsp.GetConventionnelsExportByStatut("Final Booking");
                    dataGrid.ItemsSource = conventionnels;
                    lblStatut.Content = conventionnels.Count + " Conventionnel(s)";
                }
                else if (cbFiltres.SelectedIndex == 4)
                {
                    conventionnels = vsp.GetConventionnelsExportByStatut("Cargo Loading");
                    dataGrid.ItemsSource = conventionnels;
                    lblStatut.Content = conventionnels.Count + " Conventionnel(s)";
                }
                else if (cbFiltres.SelectedIndex == 5)
                {
                    conventionnels = vsp.GetConventionnelsExportByStatut("Cargo Loaded");
                    dataGrid.ItemsSource = conventionnels;
                    lblStatut.Content = conventionnels.Count + " Conventionnel(s)";
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
                        conventionnels = vsp.GetConventionnelsExportByNumBooking(txtRechercher.Text.Trim());
                        dataGrid.ItemsSource = conventionnels;
                        lblStatut.Content = conventionnels.Count + " Conventionnel(s) trouvé(s)";
                    }
                    else if (listRechercher.SelectedIndex == 1)
                    {
                        conventionnels = vsp.GetConventionnelsExportByNumGC(txtRechercher.Text.Trim());
                        dataGrid.ItemsSource = conventionnels;
                        lblStatut.Content = conventionnels.Count + " Conventionnel(s) trouvé(s)";
                    }
                    else if (listRechercher.SelectedIndex == 2)
                    {
                        int result;
                        conventionnels = vsp.GetConventionnelsExportByNumEscale(Int32.TryParse(txtRechercher.Text.Trim(), out result) ? result : -1);
                        dataGrid.ItemsSource = conventionnels;
                        lblStatut.Content = conventionnels.Count + " Conventionnel(s) trouvé(s)";
                    }
                    else if (listRechercher.SelectedIndex == 3)
                    {
                        conventionnels = vsp.GetConventionnelsExportByPortDest(txtRechercher.Text.Trim());
                        dataGrid.ItemsSource = conventionnels;
                        lblStatut.Content = conventionnels.Count + " Conventionnel(s) trouvé(s)";
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

        private void btnAnnulerRecherche_Click(object sender, RoutedEventArgs e)
        {
            txtRechercher.Text = null;
        }

        private void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();
                CONVENTIONNEL conv = vsp.GetConventionnelExportByIdGC(((CONVENTIONNEL)dataGrid.SelectedItem).IdGC);
                if (dataGrid.SelectedIndex != -1 && dataGrid.SelectedItems.Count == 1 && conv.CONNAISSEMENT.DVCBLI.HasValue && (conv.StatGC == "Cargo Loading" || conv.StatGC == "Final Booking"))
                {
                    actionsBorder.Visibility = System.Windows.Visibility.Visible;
                    if (conv.StatGC == "Final Booking")
                    {
                        btnReceptionner.Visibility = System.Windows.Visibility.Visible;
                        btnEmbarquer.Visibility = System.Windows.Visibility.Collapsed;
                    }
                    else if (conv.StatGC == "Cargo Loading")
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
                    CONVENTIONNEL conv = vsp.GetConventionnelExportByIdGC(((CONVENTIONNEL)dataGrid.SelectedItem).IdGC);
                    ReceptionExportConventionnelForm receptionForm = new ReceptionExportConventionnelForm(this, conv, utilisateur);
                    receptionForm.txtDesc.Text = conv.DescGC;
                    receptionForm.txtPoids.Text = conv.PoidsMGC.ToString();
                    receptionForm.txtVol.Text = conv.VolMGC.ToString();
                    receptionForm.txtQte.Text = conv.QteBGC.ToString();
                    receptionForm.Title = "Réception - conventionnel N° : " + conv.NumGC;
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
                    CONVENTIONNEL conv = vsp.GetConventionnelExportByIdGC(((CONVENTIONNEL)dataGrid.SelectedItem).IdGC);
                    EmbarquerConventionnelForm embarqForm = new EmbarquerConventionnelForm(this, conv, utilisateur);
                    embarqForm.txtDesc.Text = conv.DescGCRecept;
                    embarqForm.txtPoids.Text = conv.PoidsRGC.ToString();
                    embarqForm.txtVol.Text = conv.VolRGC.ToString();
                    embarqForm.txtQte.Text = conv.QteRGC.ToString();
                    embarqForm.Title = "Embarquement - conventionnel N° : " + conv.NumGC;
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
