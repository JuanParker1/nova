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
    /// Logique d'interaction pour OrdreServicePanel.xaml
    /// </summary>
    public partial class OrdreServicePanel : DockPanel
    {
        public List<ORDRE_SERVICE> ordresServices { get; set; }
        private List<OPERATION> operationsUser;
        private UTILISATEUR utilisateur;
        private VsomParameters vsp = new VsomParameters();
        public OrdreServicePanel(UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;
                listRechercher.SelectedIndex = 0;
                cbFiltres.SelectedIndex = 1;

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

        private void btnNouveau_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (operationsUser.Where(op => op.NomOp == "Ordre de service : Enregistrement d'un nouvel élément").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
                {
                    MessageBox.Show("Vous n'avez pas les droits nécessaires pour créer un nouvel ordre de service. Veuillez contacter un administrateur", "Droits insuffisants !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    OrdreServiceForm ordreServiceForm = new OrdreServiceForm("Nouveau", this, utilisateur);
                    ordreServiceForm.cbIdOS.IsEditable = true;
                    ordreServiceForm.Title = "Nouveau : Ordre de service";
                    ordreServiceForm.borderActions.Visibility = System.Windows.Visibility.Collapsed;
                    ordreServiceForm.Show();
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

        private void dataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                if (dataGrid.SelectedIndex != -1)
                {
                    ORDRE_SERVICE os = vsp.GetServiceByIdOS(((ORDRE_SERVICE)dataGrid.SelectedItem).IdOS);
                    OrdreServiceForm serviceForm = new OrdreServiceForm(this, os, utilisateur);
                    serviceForm.Show();
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

        private void txtRechercher_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                if (e.Key == Key.Return && listRechercher.SelectedItem != null)
                {
                    if (listRechercher.SelectedIndex == 0)
                    {
                        int result;
                        ordresServices = vsp.GetServiceByNumEscale(Int32.TryParse(txtRechercher.Text.Trim(), out result) ? result : -1);
                        dataGrid.ItemsSource = ordresServices;
                        lblStatut.Content = ordresServices.Count + " Ordre(s) de service trouvé(s)";
                    }
                    else if (listRechercher.SelectedIndex == 1)
                    {
                        ordresServices = vsp.GetServiceByNumBL(txtRechercher.Text.Trim());
                        dataGrid.ItemsSource = ordresServices;
                        lblStatut.Content = ordresServices.Count + " Ordre(s) de service trouvé(s)";
                    }
                    else if (listRechercher.SelectedIndex == 2)
                    {
                        int result;
                        ordresServices = new List<ORDRE_SERVICE>();
                        ORDRE_SERVICE os = vsp.GetServiceByIdOS(Int32.TryParse(txtRechercher.Text.Trim(), out result) ? result : -1);
                        if (os != null)
                        {
                            ordresServices.Add(os);
                        }
                        dataGrid.ItemsSource = ordresServices;
                        lblStatut.Content = ordresServices.Count + " Ordre(s) de service trouvé(s)";
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

        private void cbFiltres_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                if (cbFiltres.SelectedIndex == 0)
                {
                    ordresServices = vsp.GetServices();
                    dataGrid.ItemsSource = ordresServices;
                    lblStatut.Content = ordresServices.Count + " Ordre(s) de service";
                }
                else if (cbFiltres.SelectedIndex == 1)
                {
                    ordresServices = vsp.GetServicesByStatut("En cours");
                    dataGrid.ItemsSource = ordresServices;
                    lblStatut.Content = ordresServices.Count + " Ordre(s) de service";
                }
                else if (cbFiltres.SelectedIndex == 2)
                {
                    ordresServices = vsp.GetServicesByStatut("Validé");
                    dataGrid.ItemsSource = ordresServices;
                    lblStatut.Content = ordresServices.Count + " Ordre(s) de service";
                }
                else if (cbFiltres.SelectedIndex == 3)
                {
                    ordresServices = vsp.GetServicesByStatut("Cloturé");
                    dataGrid.ItemsSource = ordresServices;
                    lblStatut.Content = ordresServices.Count + " Ordre(s) de service";
                }
                else if (cbFiltres.SelectedIndex == 4)
                {
                    ordresServices = vsp.GetServicesByStatut("Annulé");
                    dataGrid.ItemsSource = ordresServices;
                    lblStatut.Content = ordresServices.Count + " Ordre(s) de service";
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
