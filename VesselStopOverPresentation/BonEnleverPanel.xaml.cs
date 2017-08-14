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
    /// Logique d'interaction pour BonEnleverPanel.xaml
    /// </summary>
    public partial class BonEnleverPanel : DockPanel
    {

        public List<BON_ENLEVEMENT> bonsEnlever { get; set; }
        private UTILISATEUR utilisateur;
        private List<OPERATION> operationsUser;
       // private VsomParameters vsp = new VsomParameters();
        private VSOMAccessors vsomAcc;
        public BonEnleverPanel(UTILISATEUR user)
        {
            try
            {
                 vsomAcc = new VSOMAccessors();
                //VsomParameters vsp = new VsomParameters();

                InitializeComponent();
                this.DataContext = this;
                utilisateur = user;
                operationsUser = vsomAcc.GetOperationsUtilisateur(utilisateur.IdU);
                listRechercher.SelectedIndex = 0;
                //cbFiltres.SelectedIndex = 2;
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
                    BON_ENLEVEMENT bae = (BON_ENLEVEMENT)dataGrid.SelectedItem;
                    BonEnleverForm baeForm = new BonEnleverForm(this, bae, utilisateur);
                    baeForm.Show();
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
                if (operationsUser.Where(op => op.NomOp == "Bon à enlever : Enregistrement d'un nouvel élément").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
                {
                    MessageBox.Show("Vous n'avez pas les droits nécessaires pour créer un nouveau BAE. Veuillez contacter un administrateur", "Droits insuffisants !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    BonEnleverForm baeForm = new BonEnleverForm("Nouveau", this, utilisateur);
                    baeForm.Title = "Nouveau : Bon à enlever";
                    baeForm.cbIdBAE.IsEnabled = false;
                    baeForm.borderActions.Visibility = System.Windows.Visibility.Collapsed;
                    baeForm.btnValiderBAE.Visibility = System.Windows.Visibility.Collapsed;
                    baeForm.borderEtat.Visibility = System.Windows.Visibility.Collapsed;
                    baeForm.Show();
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
               // VSOMAccessors vsomAcc = new VSOMAccessors();

                if (cbFiltres.SelectedIndex == 0)
                {
                    if (utilisateur.IdAcc == 1)
                    {
                        bonsEnlever = vsomAcc.GetBonsEnlever();
                    }
                    else
                    {
                        bonsEnlever = vsomAcc.GetBonsEnleverByIdAcc(utilisateur.IdAcc.Value);
                    }

                    dataGrid.ItemsSource = bonsEnlever;
                    lblStatut.Content = bonsEnlever.Count + " Bon(s) à enlever";
                }
                else if (cbFiltres.SelectedIndex == 1)
                {
                    if (utilisateur.IdAcc == 1)
                    {
                        bonsEnlever = vsomAcc.GetBonsEnleverValides();
                    }
                    else
                    {
                        bonsEnlever = vsomAcc.GetBonsEnleverValidesByIdAcc(utilisateur.IdAcc.Value);
                    }
                    
                    dataGrid.ItemsSource = bonsEnlever;
                    lblStatut.Content = bonsEnlever.Count + " Bon(s) à enlever";
                }
                else if (cbFiltres.SelectedIndex == 2)
                {
                    if (utilisateur.IdAcc == 1)
                    {
                        bonsEnlever = vsomAcc.GetBonsEnleverEnAttente();
                    }
                    else
                    {
                        bonsEnlever = vsomAcc.GetBonsEnleverEnAttenteByIdAcc(utilisateur.IdAcc.Value);
                    }

                    dataGrid.ItemsSource = bonsEnlever;
                    lblStatut.Content = bonsEnlever.Count + " Bon(s) à enlever";
                }
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération (ex)!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void txtRechercher_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                //VSOMAccessors vsomAcc = new VSOMAccessors();

                if (e.Key == Key.Return && listRechercher.SelectedItem != null)
                {
                    if (listRechercher.SelectedIndex == 0)
                    {
                        if (utilisateur.IdAcc == 1)
                        {
                            bonsEnlever = vsomAcc.GetBonsEnleverByNumBL(txtRechercher.Text.Trim());
                        }
                        else
                        {
                            bonsEnlever = vsomAcc.GetBonsEnleverByNumBL(txtRechercher.Text.Trim(), utilisateur.IdAcc.Value);
                        }
                    }
                    else if (listRechercher.SelectedIndex == 1)
                    {
                        if (utilisateur.IdAcc == 1)
                        {
                            bonsEnlever = vsomAcc.GetBonsEnleverByNumChassis(txtRechercher.Text.Trim());
                        }
                        else
                        {
                            bonsEnlever = vsomAcc.GetBonsEnleverByNumChassis(txtRechercher.Text.Trim(), utilisateur.IdAcc.Value);
                        }
                    }
                    else if (listRechercher.SelectedIndex == 2)
                    {
                        if (utilisateur.IdAcc == 1)
                        {
                            bonsEnlever = vsomAcc.GetBonsEnleverByConsignee(txtRechercher.Text.Trim());
                        }
                        else
                        {
                            bonsEnlever = vsomAcc.GetBonsEnleverByConsignee(txtRechercher.Text.Trim(), utilisateur.IdAcc.Value);
                        }
                    }
                    dataGrid.ItemsSource = bonsEnlever;
                    lblStatut.Content = bonsEnlever.Count + " BAE trouvé(s)";
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
