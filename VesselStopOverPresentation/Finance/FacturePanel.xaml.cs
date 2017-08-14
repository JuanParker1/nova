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
    /// Logique d'interaction pour FacturePanel.xaml
    /// </summary>
    public partial class FacturePanel : DockPanel
    {

        public List<FACTURE> factures { get; set; }
        private UTILISATEUR utilisateur;
        private List<OPERATION> operationsUser;
        private VsomParameters vsp = new VsomParameters();
        public FacturePanel(UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

                listRechercher.SelectedIndex = 0;
                //cbFiltres.SelectedIndex = 0;

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
                if (operationsUser.Where(op => op.NomOp == "Facture : Enregistrement d'un nouvel élément").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
                {
                    MessageBox.Show("Vous n'avez pas les droits nécessaires pour créer une nouvelle facture. Veuillez contacter un administrateur", "Droits insuffisants !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    FactureForm factForm = new FactureForm("Nouveau", this, utilisateur);
                    factForm.cbIdFact.IsEnabled = false;
                    factForm.Title = "Nouveau : Facture";
                    factForm.borderActions.Visibility = System.Windows.Visibility.Collapsed;
                    factForm.borderEtat.Visibility = System.Windows.Visibility.Collapsed;
                    factForm.Show();
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
                    FactureForm factForm = new FactureForm(this, vsp.GetFactureByIdFact(((FACTURE)dataGrid.SelectedItem).IdFD), utilisateur);
                    factForm.Show(); 
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
                    factures = vsp.GetFactures();
                    dataGrid.ItemsSource = factures;
                    lblStatut.Content = factures.Count + " Facture(s)";
                }
                else if (cbFiltres.SelectedIndex == 1)
                {
                    factures = vsp.GetFacturesEnAttentePaiement();
                    dataGrid.ItemsSource = factures;
                    lblStatut.Content = factures.Count + " Facture(s)";
                }
                else if (cbFiltres.SelectedIndex == 2)
                {
                    factures = vsp.GetFacturesElementBL();
                    dataGrid.ItemsSource = factures;
                    lblStatut.Content = factures.Count + " Facture(s)";
                }
                else if (cbFiltres.SelectedIndex == 3)
                {
                    factures = vsp.GetFacturesElementBLEnAttentePaiement();
                    dataGrid.ItemsSource = factures;
                    lblStatut.Content = factures.Count + " Facture(s)";
                }
                else if (cbFiltres.SelectedIndex == 4)
                {
                    factures = vsp.GetFacturesElementVeh();
                    dataGrid.ItemsSource = factures;
                    lblStatut.Content = factures.Count + " Facture(s)";
                }
                else if (cbFiltres.SelectedIndex == 5)
                {
                    factures = vsp.GetFacturesElementVehEnAttentePaiement();
                    dataGrid.ItemsSource = factures;
                    lblStatut.Content = factures.Count + " Facture(s)";
                }
                else if (cbFiltres.SelectedIndex == 6)
                {
                    factures = vsp.GetFacturesElementCtr();
                    dataGrid.ItemsSource = factures;
                    lblStatut.Content = factures.Count + " Facture(s)";
                }
                else if (cbFiltres.SelectedIndex == 7)
                {
                    factures = vsp.GetFacturesElementCtrEnAttentePaiement();
                    dataGrid.ItemsSource = factures;
                    lblStatut.Content = factures.Count + " Facture(s)";
                }
                else if (cbFiltres.SelectedIndex == 8)
                {
                    factures = vsp.GetFacturesElementGC();
                    dataGrid.ItemsSource = factures;
                    lblStatut.Content = factures.Count + " Facture(s)";
                }
                else if (cbFiltres.SelectedIndex == 9)
                {
                    factures = vsp.GetFacturesElementGCEnAttentePaiement();
                    dataGrid.ItemsSource = factures;
                    lblStatut.Content = factures.Count + " Facture(s)";
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
                        int result;
                        factures = new List<FACTURE>();
                        FACTURE fact = vsp.GetFactureByIdDocSAP(Int32.TryParse(txtRechercher.Text.Trim(), out result) ? result : -1);
                        if (fact != null)
                        {
                            factures.Add(fact);
                        }
                        dataGrid.ItemsSource = factures;
                        lblStatut.Content = factures.Count + " Facture(s) trouvée(s)";
                    }
                    else if (listRechercher.SelectedIndex == 1)
                    {
                        factures = vsp.GetFacturesByNumBL(txtRechercher.Text.Trim());
                        dataGrid.ItemsSource = factures;
                        lblStatut.Content = factures.Count + " Facture(s) trouvée(s)";
                    }
                    else if (listRechercher.SelectedIndex == 2)
                    {
                        factures = vsp.GetFacturesByEscale(txtRechercher.Text.Trim());
                        dataGrid.ItemsSource = factures;
                        lblStatut.Content = factures.Count + " Facture(s) trouvée(s)";
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
