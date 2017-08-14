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
    /// Logique d'interaction pour EmplacementPanel.xaml
    /// </summary>
    public partial class EmplacementPanel : DockPanel
    {

        public List<Emplacement> emplacements { get; set; }
        private UTILISATEUR utilisateur;
        private List<OPERATION> operationsUser;
        private VsomParameters vsp = new VsomParameters();
        public EmplacementPanel(UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

                listRechercher.SelectedIndex = 0;
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

        private void btnNouveau_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                if (operationsUser.Where(op => op.NomOp == "Emplacement : Enregistrement d'un nouvel emplacement").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
                {
                    MessageBox.Show("Vous n'avez pas les droits nécessaires pour créer un  emplacement. Veuillez contacter un administrateur", "Droits insuffisants !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    EmplacementForm escaleForm = new EmplacementForm("Nouveau", this, utilisateur);
                    escaleForm.Title = "Nouveau : Emplacement";
                
                    escaleForm.Show();
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
                    emplacements = vsp.GetEmplacementsAll();
                    dataGrid.ItemsSource = emplacements;
                    lblStatut.Content = emplacements.Count + " Emplacement(s)";
                }
                else if (cbFiltres.SelectedIndex == 1)
                {
                    emplacements = vsp.GetEmplacementDispos();
                    dataGrid.ItemsSource = emplacements;
                    lblStatut.Content = emplacements.Count + " Emplacement(s)";
                }
                else if (cbFiltres.SelectedIndex == 2)
                {
                    emplacements = vsp.GetEmplacementOccupes();
                    dataGrid.ItemsSource = emplacements;
                    lblStatut.Content = emplacements.Count + " Emplacement(s)";
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
                        emplacements = vsp.GetEmplacementsByEmpl(txtRechercher.Text.Trim());
                        dataGrid.ItemsSource = emplacements;
                        lblStatut.Content = emplacements.Count + " Emplacement(s) trouvé(s)";
                    }
                    else if (listRechercher.SelectedIndex == 1)
                    {
                        emplacements = vsp.GetEmplacementsByNumChassis(txtRechercher.Text.Trim());
                        dataGrid.ItemsSource = emplacements;
                        lblStatut.Content = emplacements.Count + " Emplacement(s) trouvé(s)";
                    }
                    else if (listRechercher.SelectedIndex == 2)
                    {
                        emplacements = vsp.GetEmplacementsByNumBL(txtRechercher.Text.Trim());
                        dataGrid.ItemsSource = emplacements;
                        lblStatut.Content = emplacements.Count + " Emplacement(s) trouvé(s)";
                    }
                    else if (listRechercher.SelectedIndex == 3)
                    {
                        emplacements = vsp.GetEmplacementsByNumEsc(txtRechercher.Text.Trim());
                        dataGrid.ItemsSource = emplacements;
                        lblStatut.Content = emplacements.Count + " Emplacement(s) trouvé(s)";
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

        private void mnDelEmpl_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                VsomConfig vsomAcc = new VsomConfig();

                if (dataGrid.SelectedItems.Count > 0)
                {
                    List<Emplacement> lst = new List<Emplacement>();
                    Emplacement item;
                    string emp = string.Empty;
                    foreach (var elmt in dataGrid.SelectedItems)
                    {
                        item = (Emplacement)elmt; 
                        if (item.NumChassis.Length != 0)
                        {
                            throw new ApplicationException("Impossible de supprimer un emplacement occupé : " + item.Empl);
                        }
                        emp = string.Format("{0}, {1}", item.Empl, emp);

                        lst.Add(item);
                    }
                    MessageBoxResult mbr = MessageBox.Show("Confirmez-vous la suppression des emplacements suivants ? \n " + emp, "Suppression Emplacement",
                         MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (mbr == MessageBoxResult.Yes)
                    {
                        if (operationsUser.Where(op => op.NomOp == "Emplacement : Suppression d'emplacement").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
                        {
                            MessageBox.Show("Vous n'avez pas les droits nécessaires pour supprimer un  emplacement. Veuillez contacter un administrateur", "Droits insuffisants !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        }
                        else
                        {
                           string resp= vsomAcc.DeleteEmplacements(lst, utilisateur.IdU);
                           if (resp.Length == 0)
                           {
                               MessageBox.Show("Opération effectuée", "Suppression d'emplacement", MessageBoxButton.OK, MessageBoxImage.Information);
                           //rafraichir la grille
                               cbFiltres_SelectionChanged(null, null);
                           }
                           else
                           {
                               MessageBox.Show("Opération effectuée. Mais certains emplacements ne sont pas supprimés: \n" + resp, "Suppression d'emplacement", MessageBoxButton.OK, MessageBoxImage.Information);
                           }
                        }
                    }
                    else
                    {

                    }

                }
                else
                {

                }
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }

           
        }
    }
}
