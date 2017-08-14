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
    /// Logique d'interaction pour CubagePanel.xaml
    /// </summary>
    public partial class CubagePanel : DockPanel
    {
        private List<OPERATION> operationsUser;
        private UTILISATEUR utilisateur;
        private VsomParameters vsp = new VsomParameters();
        public CubagePanel(UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;
                listRechercher.SelectedIndex = 0;
                cbFiltres.SelectedIndex = 0;
                lblStatut.Content = cubages.Count + " Projet(s) de cubage";

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

        public List<CUBAGE> cubages { get; set; }

        private void dataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                if (dataGrid.SelectedIndex != -1)
                {
                    CubageForm cubForm = new CubageForm(this, vsp.GetCubageByIdCub(((CUBAGE)dataGrid.SelectedItem).IdCubage), utilisateur);
                    cubForm.Show();
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
                if (operationsUser.Where(op => op.NomOp == "Cubage : Enregistrement d'une nouvelle opération").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
                {
                    MessageBox.Show("Vous n'avez pas les droits nécessaires pour créer un nouveau projet de cubage. Veuillez contacter un administrateur", "Droits insuffisants !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    CubageForm cubForm = new CubageForm("Nouveau", this, utilisateur);
                    cubForm.cbIdCub.IsEnabled = false;
                    cubForm.Title = "Nouveau : projet de cubage";
                    cubForm.borderActions.Visibility = System.Windows.Visibility.Collapsed;
                    cubForm.borderEtat.Visibility = System.Windows.Visibility.Collapsed;
                    cubForm.Show();
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
                    cubages = vsp.GetCubages();
                    dataGrid.ItemsSource = cubages;
                    lblStatut.Content = cubages.Count + " projet(s) de cubage";
                }
                else if (cbFiltres.SelectedIndex == 1)
                {
                    cubages = vsp.GetCubagesEnCours();
                    dataGrid.ItemsSource = cubages;
                    lblStatut.Content = cubages.Count + " projet(s) de cubage";
                }
                else if (cbFiltres.SelectedIndex == 2)
                {
                    cubages = vsp.GetCubagesClotures();
                    dataGrid.ItemsSource = cubages;
                    lblStatut.Content = cubages.Count + " projet(s) de cubage";
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
                    int result;
                    if (listRechercher.SelectedIndex == 0)
                    {
                        cubages = vsp.GetCubageByNumEsc(Int32.TryParse(txtRechercher.Text.Trim(), out result) ? result : -1);
                        dataGrid.ItemsSource = cubages;
                        lblStatut.Content = cubages.Count + " projet(s) de cubage";
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
    }
}
