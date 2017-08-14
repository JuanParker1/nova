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
    /// Logique d'interaction pour DemandeLivraisonPanel.xaml
    /// </summary>
    public partial class DemandeLivraisonPanel : DockPanel
    {

        public List<DEMANDE_LIVRAISON> demandesLivraison { get; set; }
        private UTILISATEUR utilisateur;
        private List<OPERATION> operationsUser;
        private VsomParameters vsp ;
        public DemandeLivraisonPanel(UTILISATEUR user)
        {
            try
            {
               // VSOMAccessors vsomAcc = new VSOMAccessors();
                

                InitializeComponent();
                //using (var ctx = new VSOMClassesDataContext())
                //{
                     vsp = new VsomParameters();
                    this.DataContext = this;
                    utilisateur = user;
                    operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);
                    listRechercher.SelectedIndex = 0;
                    cbFiltres.SelectedIndex = 2;
                //}
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
                    DemandeLivraisonForm livraisonForm = new DemandeLivraisonForm(this, (DEMANDE_LIVRAISON)dataGrid.SelectedItem, utilisateur);
                    livraisonForm.Show();
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
                if (operationsUser.Where(op => op.NomOp == "Demande de livraison : Enregistrement d'un nouvel élément").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
                {
                    MessageBox.Show("Vous n'avez pas les droits nécessaires pour créer une nouvelle demande de livraison. Veuillez contacter un administrateur", "Droits insuffisants !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    DemandeLivraisonForm livraisonForm = new DemandeLivraisonForm("Nouveau", this, utilisateur);
                    livraisonForm.Title = "Nouveau : Demande de livraison";
                    livraisonForm.cbIdDL.IsEnabled = false;
                    livraisonForm.borderActions.Visibility = System.Windows.Visibility.Collapsed;
                    livraisonForm.btnValiderDL.Visibility = System.Windows.Visibility.Collapsed;
                    livraisonForm.borderEtat.Visibility = System.Windows.Visibility.Collapsed;
                    livraisonForm.Show();
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
                //using (var ctx = new VSOMClassesDataContext())
                //{
                    VSOMAccessors vsomAcc = new VSOMAccessors();

                    if (cbFiltres.SelectedIndex == 0)
                    {
                        if (utilisateur.IdAcc == 1)
                        {
                            demandesLivraison = vsp.GetDemandesLivraison();
                        }
                        else
                        {
                            demandesLivraison = vsp.GetDemandesLivraisonByIdAcc(utilisateur.IdAcc.Value);
                        }

                        dataGrid.ItemsSource = demandesLivraison;
                        lblStatut.Content = demandesLivraison.Count + " Demande(s) de livraison";
                    }
                    else if (cbFiltres.SelectedIndex == 1)
                    {
                        if (utilisateur.IdAcc == 1)
                        {
                            demandesLivraison = vsp.GetDemandesLivraisonValides();
                        }
                        else
                        {
                            demandesLivraison = vsp.GetDemandesLivraisonValidesByIdAcc(utilisateur.IdAcc.Value);
                        }

                        dataGrid.ItemsSource = demandesLivraison;
                        lblStatut.Content = demandesLivraison.Count + " Demande(s) de livraison";
                    }
                    else if (cbFiltres.SelectedIndex == 2)
                    {
                        if (utilisateur.IdAcc == 1)
                        {
                            demandesLivraison = vsp.GetDemandesLivraisonEnAttente();
                        }
                        else
                        {
                            demandesLivraison = vsp.GetDemandesLivraisonEnAttenteByIdAcc(utilisateur.IdAcc.Value);
                        }

                        dataGrid.ItemsSource = demandesLivraison;
                        lblStatut.Content = demandesLivraison.Count + " Demande(s) de livraison";
                    }
                    else if (cbFiltres.SelectedIndex == 3)
                    {
                        if (utilisateur.IdAcc == 1)
                        {
                            demandesLivraison = vsp.GetDemandesLivraisonAvecDossierPhysique();
                        }
                        else
                        {
                            demandesLivraison = vsp.GetDemandesLivraisonAvecDossierPhysiqueByIdAcc(utilisateur.IdAcc.Value);
                        }

                        dataGrid.ItemsSource = demandesLivraison;
                        lblStatut.Content = demandesLivraison.Count + " Demande(s) de livraison";
                    }
                    else if (cbFiltres.SelectedIndex == 4)
                    {
                        if (utilisateur.IdAcc == 1)
                        {
                            demandesLivraison = vsp.GetDemandesLivraisonSansDossierPhysique();
                        }
                        else
                        {
                            demandesLivraison = vsp.GetDemandesLivraisonSansDossierPhysiqueByIdAcc(utilisateur.IdAcc.Value);
                        }

                        dataGrid.ItemsSource = demandesLivraison;
                        lblStatut.Content = demandesLivraison.Count + " Demande(s) de livraison";
                    }
                //}
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
                //using (var ctx = new VSOMClassesDataContext())
                //{
                    vsp = new VsomParameters();

                    if (e.Key == Key.Return && listRechercher.SelectedItem != null)
                    {
                        if (listRechercher.SelectedIndex == 0)
                        {
                            if (utilisateur.IdAcc == 1)
                            {
                                demandesLivraison = vsp.GetDemandesLivraisonByNumBL(txtRechercher.Text.Trim());
                            }
                            else
                            {
                                demandesLivraison = vsp.GetDemandesLivraisonByNumBL(txtRechercher.Text.Trim(), utilisateur.IdAcc.Value);
                            }
                        }
                        else if (listRechercher.SelectedIndex == 1)
                        {
                            if (utilisateur.IdAcc == 1)
                            {
                                demandesLivraison = vsp.GetDemandesLivraisonByNumChassis(txtRechercher.Text.Trim());
                            }
                            else
                            {
                                demandesLivraison = vsp.GetDemandesLivraisonByNumChassis(txtRechercher.Text.Trim(), utilisateur.IdAcc.Value);
                            }
                        }
                        else if (listRechercher.SelectedIndex == 2)
                        {
                            if (utilisateur.IdAcc == 1)
                            {
                                demandesLivraison = vsp.GetDemandesLivraisonByConsignee(txtRechercher.Text.Trim());
                            }
                            else
                            {
                                demandesLivraison = vsp.GetDemandesLivraisonByConsignee(txtRechercher.Text.Trim(), utilisateur.IdAcc.Value);
                            }
                        }

                        dataGrid.ItemsSource = demandesLivraison;
                        lblStatut.Content = demandesLivraison.Count + " Demande(s) de livraison trouvée(s)";
                    }
                    else if (e.Key == Key.Escape)
                    {
                        txtRechercher.Text = null;
                    }
                //}
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
