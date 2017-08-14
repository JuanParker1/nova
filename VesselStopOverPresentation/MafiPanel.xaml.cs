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
    /// Logique d'interaction pour MafiPanel.xaml
    /// </summary>
    public partial class MafiPanel : DockPanel
    {
        private List<OPERATION> operationsUser;
        private UTILISATEUR utilisateur;
        private VsomParameters vsp = new VsomParameters();
        public MafiPanel(UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;
                listRechercher.SelectedIndex = 0;
                //cbFiltres.SelectedIndex = 0;
                //lblStatut.Content = conteneurs.Count + " Conteneur(s)";

                actionsBorder.Visibility = System.Windows.Visibility.Collapsed;

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

        public List<MAFI> mafis { get; set; }

        private void dataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();
                MAFI mf = vsp.GetMafiImportByIdMafi(((MAFI)dataGrid.SelectedItem).IdMafi);
                if (dataGrid.SelectedIndex != -1)
                {
                    MafiForm mafiForm = new MafiForm(this, mf, utilisateur);
                    mafiForm.Show();
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
                if (operationsUser.Where(op => op.NomOp == "Mafi : Enregistrement d'un nouvel élément").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
                {
                    MessageBox.Show("Vous n'avez pas les droits nécessaires pour créer un nouveau mafi. Veuillez contacter un administrateur", "Droits insuffisants !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    MafiForm mafiForm = new MafiForm("Nouveau", this, utilisateur);
                    mafiForm.cbNumMafi.IsEditable = true;
                    mafiForm.actionsBorder.Visibility = System.Windows.Visibility.Collapsed;
                    mafiForm.txtDescription.IsReadOnly = false;
                    mafiForm.txtDescription.Background = Brushes.White;
                    mafiForm.txtDescMses.IsReadOnly = false;
                    mafiForm.txtDescMses.Background = Brushes.White;
                    mafiForm.txtIMDGCode.IsReadOnly = false;
                    mafiForm.txtIMDGCode.Background = Brushes.White;
                    mafiForm.txtPoids.IsReadOnly = false;
                    mafiForm.txtPoids.Background = Brushes.White;
                    mafiForm.txtTypeMMafi.IsReadOnly = false;
                    mafiForm.txtTypeMMafi.Background = Brushes.White;
                    mafiForm.cbNumBL.IsEditable = true;
                    mafiForm.cbNumBL.IsEnabled = true;
                    //mafiForm.groupInfosCub.IsEnabled = false;
                    mafiForm.Title = "Nouveau : Mafi";
                    mafiForm.actionsBorder.Visibility = System.Windows.Visibility.Collapsed;
                    mafiForm.Show();
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
                    mafis = vsp.GetMafisImport();
                }
                else if (cbFiltres.SelectedIndex == 1)
                {
                    mafis = vsp.GetMafisImportByStatut("Non initié");
                }
                else if (cbFiltres.SelectedIndex == 2)
                {
                    mafis = vsp.GetMafisImportByStatut("Traité");
                }
                else if (cbFiltres.SelectedIndex == 3)
                {
                    mafis = vsp.GetMafisImportByStatut("Manifesté");
                }
                else if (cbFiltres.SelectedIndex == 4)
                {
                    mafis = vsp.GetMafisImportByStatut("Déchargé");
                }
                else if (cbFiltres.SelectedIndex == 5)
                {
                    mafis = vsp.GetMafisImportByStatut("Parqué");
                }
                else if (cbFiltres.SelectedIndex == 6)
                {
                    mafis = vsp.GetMafisImportByStatut("Enlèvement");
                }
                else if (cbFiltres.SelectedIndex == 7)
                {
                    mafis = vsp.GetMafisImportByStatut("Sortis");
                }
                dataGrid.ItemsSource = mafis;
                lblStatut.Content = mafis.Count + " Mafi(s)";
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
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
                        mafis = vsp.GetMafisImportByNumBL(txtRechercher.Text.Trim());
                    }
                    else if (listRechercher.SelectedIndex == 1)
                    {
                        mafis = vsp.GetMafisImportByNumMafi(txtRechercher.Text.Trim());
                    }
                    else if (listRechercher.SelectedIndex == 2)
                    {
                        int result;
                        mafis = vsp.GetMafisImportByNumEscale(Int32.TryParse(txtRechercher.Text.Trim(), out result) ? result : -1);
                    }
                    else if (listRechercher.SelectedIndex == 3)
                    {
                        mafis = vsp.GetMafisImportByDescription(txtRechercher.Text.Trim());
                    }
                    dataGrid.ItemsSource = mafis;
                    lblStatut.Content = mafis.Count + " Mafi(s) trouvé(s)";
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

        private void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                if (dataGrid.SelectedIndex != -1 && dataGrid.SelectedItems.Count == 1)
                {
                    MAFI mf = vsp.GetMafiImportByIdMafi(((MAFI)dataGrid.SelectedItem).IdMafi);
                    actionsBorder.Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    actionsBorder.Visibility = System.Windows.Visibility.Collapsed;
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

        private void btnIdentifier_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                if (dataGrid.SelectedIndex != -1 && dataGrid.SelectedItems.Count == 1)
                {
                    MAFI mf = vsp.GetMafiImportByIdMafi(((MAFI)dataGrid.SelectedItem).IdMafi);
                    IdentificationMafiForm identForm = new IdentificationMafiForm(this, mf, utilisateur);
                    identForm.Title = "Identification - Mafi N° : " + mf.NumMafi;
                    identForm.ShowDialog();
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

        private void btnSortir_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                if (dataGrid.SelectedIndex != -1 && dataGrid.SelectedItems.Count == 1)
                {
                    MAFI mf = vsp.GetMafiImportByIdMafi(((MAFI)dataGrid.SelectedItem).IdMafi);
                    SortirMafiForm sortirForm = new SortirMafiForm(this, mf, utilisateur);
                    sortirForm.Title = "Sortir - Mafi N° : " + mf.NumMafi;
                    sortirForm.ShowDialog();
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
