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
    /// Logique d'interaction pour BonSortiePanel.xaml
    /// </summary>
    public partial class BonSortiePanel : DockPanel
    {

        public List<BON_SORTIE> bonsSortie { get; set; }
        private UTILISATEUR utilisateur;
        private List<OPERATION> operationsUser;
        private VsomParameters vsp = new VsomParameters();

        public BonSortiePanel(UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;
                utilisateur = user;
                operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);
                //cbFiltres.SelectedIndex = 0;
                listRechercher.SelectedIndex = 0;
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
                    BonSortieForm sortieForm = new BonSortieForm(this, (BON_SORTIE)dataGrid.SelectedItem, utilisateur);
                    sortieForm.Show();
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
                if (operationsUser.Where(op => op.NomOp == "Bon de sortie : Enregistrement d'un nouvel élément").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
                {
                    MessageBox.Show("Vous n'avez pas les droits nécessaires pour créer un nouveau bon de sortie. Veuillez contacter un administrateur", "Droits insuffisants !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    BonSortieForm sortieForm = new BonSortieForm("Nouveau", this, utilisateur);
                    sortieForm.Title = "Nouveau : Bon de sortie";
                    sortieForm.cbIdBS.IsEnabled = false;
                    sortieForm.borderActions.Visibility = System.Windows.Visibility.Collapsed;
                    sortieForm.borderEtat.Visibility = System.Windows.Visibility.Collapsed;
                    sortieForm.Show();
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
                        bonsSortie = vsp.GetBonsSortie();
                    }
                    else
                    {
                        bonsSortie = vsp.GetBonsSortieByIdAcc(utilisateur.IdAcc.Value);
                    }
                    
                    dataGrid.ItemsSource = bonsSortie;
                    lblStatut.Content = bonsSortie.Count + " Bon(s) de sortie";
                }
                else if (cbFiltres.SelectedIndex == 1)
                {
                    if (utilisateur.IdAcc == 1)
                    {
                        bonsSortie = vsp.GetBonsSortieVeh();
                    }
                    else
                    {
                        bonsSortie = vsp.GetBonsSortieVehByIdAcc(utilisateur.IdAcc.Value);
                    }
                    
                    dataGrid.ItemsSource = bonsSortie;
                    lblStatut.Content = bonsSortie.Count + " Bon(s) de sortie";
                }
                else if (cbFiltres.SelectedIndex == 2)
                {
                    if (utilisateur.IdAcc == 1)
                    {
                        bonsSortie = vsp.GetBonsSortieCtr();
                    }
                    else
                    {
                        bonsSortie = vsp.GetBonsSortieCtrByIdAcc(utilisateur.IdAcc.Value);
                    }

                    dataGrid.ItemsSource = bonsSortie;
                    lblStatut.Content = bonsSortie.Count + " Bon(s) de sortie";
                }
                else if (cbFiltres.SelectedIndex == 3)
                {
                    if (utilisateur.IdAcc == 1)
                    {
                        bonsSortie = vsp.GetBonsSortieGC();
                    }
                    else
                    {
                        bonsSortie = vsp.GetBonsSortieGCByIdAcc(utilisateur.IdAcc.Value);
                    }
                    
                    dataGrid.ItemsSource = bonsSortie;
                    lblStatut.Content = bonsSortie.Count + " Bon(s) de sortie";
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
                            bonsSortie = vsp.GetBonSortieByNumBL(txtRechercher.Text.Trim());
                        }
                        else
                        {
                            bonsSortie = vsp.GetBonSortieByNumBL(txtRechercher.Text.Trim(), utilisateur.IdAcc.Value);
                        }
                    }
                    else if (listRechercher.SelectedIndex == 1)
                    {
                        if (utilisateur.IdAcc == 1)
                        {
                            bonsSortie = vsp.GetBonSortieByNumChassis(txtRechercher.Text.Trim());
                        }
                        else
                        {
                            bonsSortie = vsp.GetBonSortieByNumChassis(txtRechercher.Text.Trim(), utilisateur.IdAcc.Value);
                        }
                    }
                    else if (listRechercher.SelectedIndex == 2)
                    {
                        if (utilisateur.IdAcc == 1)
                        {
                            bonsSortie = vsp.GetBonSortieByConsignee(txtRechercher.Text.Trim());
                        }
                        else
                        {
                            bonsSortie = vsp.GetBonSortieByConsignee(txtRechercher.Text.Trim(), utilisateur.IdAcc.Value);
                        }
                    }

                    dataGrid.ItemsSource = bonsSortie;
                    lblStatut.Content = bonsSortie.Count + " Bon(s) de sortie trouvé(s)";
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
