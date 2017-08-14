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
    /// Logique d'interaction pour PaiementPanel.xaml
    /// </summary>
    public partial class PaiementPanel : DockPanel
    {

        public List<PAYEMENT> paiements { get; set; }
        private UTILISATEUR utilisateur;
        private List<OPERATION> operationsUser;
        private VsomParameters vsp = new VsomParameters();
        public PaiementPanel(UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

                listRechercher.SelectedIndex = 0;

                //cbFiltres.SelectedIndex = 0;

                //paiements = vsomAcc.GetPaiements();
                //dataGrid.ItemsSource = paiements;

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
                if (operationsUser.Where(op => op.NomOp == "Paiement : Enregistrement d'un nouvel élément").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
                {
                    MessageBox.Show("Vous n'avez pas les droits nécessaires pour enregistrer un nouveau paiement. Veuillez contacter un administrateur", "Droits insuffisants !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    PaiementForm payForm = new PaiementForm("Nouveau", this, utilisateur);
                    payForm.cbIdPay.IsEnabled = false;
                    payForm.Title = "Nouveau : Paiement";
                    payForm.cbModePay.SelectedIndex = 0;
                    payForm.borderActions.Visibility = System.Windows.Visibility.Collapsed;
                    payForm.borderEtat.Visibility = System.Windows.Visibility.Collapsed;
                    payForm.Show();
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
                    PaiementForm payForm = new PaiementForm(this, vsp.GetPaiementByIdPay(((PAYEMENT)dataGrid.SelectedItem).IdPay), utilisateur);
                    payForm.btnEnregistrer.IsEnabled = false;
                    payForm.Show();
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
                    paiements = vsp.GetPaiements();
                    dataGrid.ItemsSource = paiements;
                    lblStatut.Content = paiements.Count + " Paiements(s)";
                }
                else if (cbFiltres.SelectedIndex == 1)
                {
                    paiements = vsp.GetPaiementsFacture();
                    dataGrid.ItemsSource = paiements;
                    lblStatut.Content = paiements.Count + " Paiements(s)";
                }
                else if (cbFiltres.SelectedIndex == 2)
                {
                    paiements = vsp.GetPaiementsCaution();
                    dataGrid.ItemsSource = paiements;
                    lblStatut.Content = paiements.Count + " Paiements(s)";
                }
                else if (cbFiltres.SelectedIndex == 3)
                {
                    paiements = vsp.GetPaiementsSOCAR();
                    dataGrid.ItemsSource = paiements;
                    lblStatut.Content = paiements.Count + " Paiements(s)";
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
                        paiements = vsp.GetPaiementsByNumBL(txtRechercher.Text.Trim());
                        dataGrid.ItemsSource = paiements;
                        lblStatut.Content = paiements.Count + " Paiements(s) trouvé(s)";
                    }
                    else if (listRechercher.SelectedIndex == 1)
                    {
                        int result;
                        paiements = vsp.GetPaiementByIdFact(Int32.TryParse(txtRechercher.Text.Trim(), out result) ? result : -1);
                        dataGrid.ItemsSource = paiements;
                        lblStatut.Content = paiements.Count + " Paiements(s) trouvé(s)";
                    }
                    else if (listRechercher.SelectedIndex == 2)
                    {
                        paiements = vsp.GetPaiementsByNumCtr(txtRechercher.Text.Trim());
                        dataGrid.ItemsSource = paiements;
                        lblStatut.Content = paiements.Count + " Paiements(s) trouvé(s)";
                    }
                    else if (listRechercher.SelectedIndex == 3)
                    {
                        paiements = vsp.GetPaiementsByNumChassis(txtRechercher.Text.Trim());
                        dataGrid.ItemsSource = paiements;
                        lblStatut.Content = paiements.Count + " Paiements(s) trouvé(s)";
                    }
                    else if (listRechercher.SelectedIndex == 4)
                    {
                        int result;
                        paiements = vsp.GetPaiementByIdEscale(Int32.TryParse(txtRechercher.Text.Trim(), out result) ? result : -1);
                        dataGrid.ItemsSource = paiements;
                        lblStatut.Content = paiements.Count + " Paiements(s) trouvé(s)";
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

        private void btnNewClientPaiement_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                if (operationsUser.Where(op => op.NomOp == "Paiement : Enregistrement d'un nouvel élément").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
                {
                    MessageBox.Show("Vous n'avez pas les droits nécessaires pour enregistrer un nouveau paiement. Veuillez contacter un administrateur", "Droits insuffisants !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    PaiementClient payForm = new PaiementClient( this, utilisateur);
                     
                    payForm.Title = "Nouveau : Paiement";
                    payForm.cbModePay.SelectedIndex = 0;
                    payForm.borderActions.Visibility = System.Windows.Visibility.Collapsed;
                    payForm.borderEtat.Visibility = System.Windows.Visibility.Collapsed;
                    payForm.Show();
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
