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
    /// Logique d'interaction pour FactureDITPanel.xaml
    /// </summary>
    public partial class FactureDITPanel : DockPanel
    {

        public List<FACTURE_DIT> facturesDIT { get; set; }
        private List<OPERATION> operationsUser;
        private UTILISATEUR utilisateur;
        private VsomParameters vsp = new VsomParameters();
        public FactureDITPanel(UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

                listRechercher.SelectedIndex = 0;
                cbFiltres.SelectedIndex = 0;

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
                if (operationsUser.Where(op => op.NomOp == "Mise à jour des prix DIT").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
                {
                    MessageBox.Show("Vous n'avez pas les droits nécessaires pour créer une nouvelle facture DIT. Veuillez contacter un administrateur", "Droits insuffisants !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    FactureDITForm factDIT = new FactureDITForm("Nouveau", this, utilisateur);
                    factDIT.cbIdFact.IsEnabled = false;
                    factDIT.Title = "Nouveau : Facture DIT";
                    factDIT.borderActions.Visibility = System.Windows.Visibility.Collapsed;
                    factDIT.Show();
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
                    FactureDITForm factDIT = new FactureDITForm(this, vsp.GetFacturesDITByIdFact(((FACTURE_DIT)dataGrid.SelectedItem).IdFactDIT), utilisateur);
                    factDIT.Show();
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
                    facturesDIT = vsp.GetFacturesDIT();
                    dataGrid.ItemsSource = facturesDIT;
                    lblStatut.Content = facturesDIT.Count + " Facture(s) DIT";
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
                        facturesDIT = vsp.GetFacturesDITByNumBL(txtRechercher.Text.Trim());
                        dataGrid.ItemsSource = facturesDIT;
                        lblStatut.Content = facturesDIT.Count + " Facture(s) DIT trouvée(s)";
                    }
                    else if (listRechercher.SelectedIndex == 1)
                    {
                        int result;
                        facturesDIT = vsp.GetFacturesDITByNumEsc(Int32.TryParse(txtRechercher.Text.Trim(), out result) ? result : -1);
                        dataGrid.ItemsSource = facturesDIT;
                        lblStatut.Content = facturesDIT.Count + " Facture(s) DIT trouvée(s)";
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
