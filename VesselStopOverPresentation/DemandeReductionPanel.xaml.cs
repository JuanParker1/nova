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
    /// Logique d'interaction pour DemandeReductionPanel.xaml
    /// </summary>
    public partial class DemandeReductionPanel : DockPanel
    {

        public List<DEMANDE_REDUCTION> reductions { get; set; }
        private UTILISATEUR utilisateur;
        private List<OPERATION> operationsUser;
        private VsomParameters vsp = new VsomParameters();
        public DemandeReductionPanel(UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;
                listRechercher.SelectedIndex = 1;
                cbFiltres.SelectedIndex = 2;

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

        private void dataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (dataGrid.SelectedIndex != -1)
                {
                    DemandeReductionForm reducForm = new DemandeReductionForm(this, (DEMANDE_REDUCTION)dataGrid.SelectedItem, utilisateur);
                    reducForm.Show();
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
                if (operationsUser.Where(op => op.NomOp == "Demande de réduction : Enregistrement d'un nouvel élément").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
                {
                    MessageBox.Show("Vous n'avez pas les droits nécessaires pour créer une nouvelle demande de réduction. Veuillez contacter un administrateur", "Droits insuffisants !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    DemandeReductionForm reducForm = new DemandeReductionForm("Nouveau", this, utilisateur);
                    reducForm.Title = "Nouveau : Demande de réduction";
                    reducForm.cbIdRed.IsEnabled = false;
                    reducForm.borderActions.Visibility = System.Windows.Visibility.Collapsed;
                    reducForm.btnValider.Visibility = System.Windows.Visibility.Collapsed;
                    reducForm.Show();
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
                        reductions = new List<DEMANDE_REDUCTION>();
                        DEMANDE_REDUCTION red = vsp.GetDemandesReductionById(Int32.TryParse(txtRechercher.Text.Trim(), out result) ? result : -1);
                        if (red != null)
                        {
                            reductions.Add(red);
                        }
                        dataGrid.ItemsSource = reductions;
                        lblStatut.Content = reductions.Count + " Réduction(s) trouvée(s)";
                    }
                    else if (listRechercher.SelectedIndex == 1)
                    {
                        reductions = vsp.GetDemandesReductionByNumBL(txtRechercher.Text.Trim());
                        dataGrid.ItemsSource = reductions;
                        lblStatut.Content = reductions.Count + " Réduction(s) trouvée(s)";
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
                    reductions = vsp.GetDemandesReduction();
                    dataGrid.ItemsSource = reductions;
                    lblStatut.Content = reductions.Count + " Demande(s) de réduction";
                }
                else if (cbFiltres.SelectedIndex == 1)
                {
                    reductions = vsp.GetDemandesReductionValidees();
                    dataGrid.ItemsSource = reductions;
                    lblStatut.Content = reductions.Count + " Demande(s) de réduction";
                }
                else if (cbFiltres.SelectedIndex == 2)
                {
                    reductions = vsp.GetDemandesReductionEnAttente();
                    dataGrid.ItemsSource = reductions;
                    lblStatut.Content = reductions.Count + " Demande(s) de réduction";
                }
                else if (cbFiltres.SelectedIndex == 3)
                {
                    reductions = vsp.GetDemandesReductionAnnulee();
                    dataGrid.ItemsSource = reductions;
                    lblStatut.Content = reductions.Count + " Demande(s) de réduction";
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
