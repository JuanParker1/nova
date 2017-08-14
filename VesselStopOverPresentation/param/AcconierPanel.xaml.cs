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
    /// Logique d'interaction pour NavirePanel.xaml
    /// </summary>
    public partial class AcconierPanel : DockPanel
    {

        private UTILISATEUR utilisateur;
        private List<OPERATION> operationsUser;
        public List<ACCONIER> acconiers { get; set; }
        //private VsomParameters vsp = new VsomParameters();
        private VSOMAccessors vsomAcc;
        public AcconierPanel(UTILISATEUR user)
        {
            try
            {
                 vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;
                cbFiltres.SelectedIndex = 0;
                listRechercher.SelectedIndex = 0;

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

        private void cbFiltres_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
               // VSOMAccessors vsomAcc = new VSOMAccessors();
               // VsomParameters vsp = new VsomParameters();
                if (cbFiltres.SelectedIndex == 0)
                {
                    acconiers = vsomAcc.GetAcconiersActifs();
                    dataGrid.ItemsSource = acconiers;
                    lblStatut.Content = acconiers.Count + " Acconier(s)";
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
                //VsomParameters vsp = new VsomParameters();
                if (e.Key == Key.Return && listRechercher.SelectedItem != null)
                {
                    if (listRechercher.SelectedIndex == 0)
                    {
                        acconiers = vsomAcc.GetAcconiersByCodeAcc(txtRechercher.Text.Trim());
                        dataGrid.ItemsSource = acconiers;
                        lblStatut.Content = acconiers.Count + " Acconier(s) trouvé(s)";
                    }
                    else if (listRechercher.SelectedIndex == 1)
                    {
                        acconiers = vsomAcc.GetAcconiersByNomAcc(txtRechercher.Text.Trim());
                        dataGrid.ItemsSource = acconiers;
                        lblStatut.Content = acconiers.Count + " Acconier(s) trouvé(s)";
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

        private void btnNouveau_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (operationsUser.Where(op => op.NomOp == "Acconier : Enregistrement d'un nouvel élément").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
                {
                    MessageBox.Show("Vous n'avez pas les droits nécessaires pour créer un nouvel acconier. Veuillez contacter un administrateur", "Droits insuffisants !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    AcconierForm acconierForm = new AcconierForm(this, utilisateur);
                    acconierForm.Title = "Nouveau : Acconier";
                    acconierForm.Show();
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
                if (dataGrid.SelectedIndex != -1)
                {
                    AcconierForm acconierForm = new AcconierForm(this, utilisateur);
                    ACCONIER a = (ACCONIER)dataGrid.SelectedItem;
                    acconierForm.acconier = a;
                    acconierForm.txtCode.Text = a.CodeAcc;
                    acconierForm.txtLibelle.Text = a.NomAcc;
                    acconierForm.Title = "Acconier : " + a.NomAcc;
                    acconierForm.lblStatut.Content = a.StatutAcc == "A" ? "Actif" : "Inactif";
                    acconierForm.Show();
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
    }
}
