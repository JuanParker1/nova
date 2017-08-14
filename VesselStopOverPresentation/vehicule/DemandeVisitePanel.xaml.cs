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
    /// Logique d'interaction pour DemandeVisitePanel.xaml
    /// </summary>
    public partial class DemandeVisitePanel : DockPanel
    {

        public List<DEMANDE_VISITE> demandesVisite { get; set; }
        private UTILISATEUR utilisateur;
        private List<OPERATION> operationsUser;
        private VsomParameters vsp = new VsomParameters();
        public DemandeVisitePanel(UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();
                VsomParameters vsp = new VsomParameters();

                InitializeComponent();
                this.DataContext = this;
                utilisateur = user;
                operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);
                listRechercher.SelectedIndex = 0;
                cbFiltres.SelectedIndex = 2;
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
                    DemandeVisiteForm visiteForm = new DemandeVisiteForm(this, (DEMANDE_VISITE)dataGrid.SelectedItem, utilisateur);
                    visiteForm.Show();
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
                if (operationsUser.Where(op => op.NomOp == "Demande de visite : Enregistrement d'un nouvel élément").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
                {
                    MessageBox.Show("Vous n'avez pas les droits nécessaires pour créer une nouvelle demande de visite. Veuillez contacter un administrateur", "Droits insuffisants !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    DemandeVisiteForm visiteForm = new DemandeVisiteForm("Nouveau", this, utilisateur);
                    visiteForm.Title = "Nouveau : Demande de visite";
                    visiteForm.cbIdDV.IsEnabled = false;
                    visiteForm.borderActions.Visibility = System.Windows.Visibility.Collapsed;
                    visiteForm.btnValiderDV.Visibility = System.Windows.Visibility.Collapsed;
                    visiteForm.Show();
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
                    if (utilisateur.IdAcc == 1)
                    {
                        demandesVisite = vsp.GetDemandesVisite();
                    }
                    else
                    {
                        demandesVisite = vsp.GetDemandesVisite(utilisateur.IdAcc.Value);
                    }
                    
                    dataGrid.ItemsSource = demandesVisite;
                    lblStatut.Content = demandesVisite.Count + " Demande(s) de visite";
                }
                else if (cbFiltres.SelectedIndex == 1)
                {
                    if (utilisateur.IdAcc == 1)
                    {
                        demandesVisite = vsp.GetDemandesVisiteValidees();
                    }
                    else
                    {
                        demandesVisite = vsp.GetDemandesVisiteValidees(utilisateur.IdAcc.Value);
                    }
                    
                    dataGrid.ItemsSource = demandesVisite;
                    lblStatut.Content = demandesVisite.Count + " Demande(s) de visite";
                }
                else if (cbFiltres.SelectedIndex == 2)
                {
                    if (utilisateur.IdAcc == 1)
                    {
                        demandesVisite = vsp.GetDemandesVisiteEnAttente();
                    }
                    else
                    {
                        demandesVisite = vsp.GetDemandesVisiteEnAttente(utilisateur.IdAcc.Value);
                    }
                    
                    dataGrid.ItemsSource = demandesVisite;
                    lblStatut.Content = demandesVisite.Count + " Demande(s) de visite";
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
                        if (utilisateur.IdAcc == 1)
                        {
                            demandesVisite = vsp.GetDemandesVisiteByNumBL(txtRechercher.Text.Trim());
                        }
                        else
                        {
                            demandesVisite = vsp.GetDemandesVisiteByNumBL(txtRechercher.Text.Trim(), utilisateur.IdAcc.Value);
                        }
                    }
                    else if (listRechercher.SelectedIndex == 1)
                    {
                        if (utilisateur.IdAcc == 1)
                        {
                            demandesVisite = vsp.GetDemandesVisiteByNumChassis(txtRechercher.Text.Trim());
                        }
                        else
                        {
                            demandesVisite = vsp.GetDemandesVisiteByNumChassis(txtRechercher.Text.Trim(), utilisateur.IdAcc.Value);
                        }
                    }
                    else if (listRechercher.SelectedIndex == 2)
                    {
                        if (utilisateur.IdAcc == 1)
                        {
                            demandesVisite = vsp.GetDemandesVisiteByConsignee(txtRechercher.Text.Trim());
                        }
                        else
                        {
                            demandesVisite = vsp.GetDemandesVisiteByConsignee(txtRechercher.Text.Trim(), utilisateur.IdAcc.Value);
                        }
                    }
                    dataGrid.ItemsSource = demandesVisite;
                    lblStatut.Content = demandesVisite.Count + " Demande(s) de visites trouvé(s)";
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
