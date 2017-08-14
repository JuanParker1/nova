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
    /// Logique d'interaction pour ConteneurPanel.xaml
    /// </summary>
    public partial class ConteneurPanel : DockPanel
    {
        private List<OPERATION> operationsUser;
        private UTILISATEUR utilisateur;

        private VsomParameters vsp = new VsomParameters();
        public ConteneurPanel(UTILISATEUR user)
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

        public List<CONTENEUR> conteneurs { get; set; }

        private void dataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();
                CONTENEUR ctr = vsp.GetConteneurImportByIdCtr(((CONTENEUR)dataGrid.SelectedItem).IdCtr);
                if (dataGrid.SelectedIndex != -1)
                {
                    ConteneurForm contForm = new ConteneurForm(this, ctr, utilisateur);
                    contForm.Show();
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
                if (operationsUser.Where(op => op.NomOp == "Conteneur : Enregistrement d'un nouvel élément").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
                {
                    MessageBox.Show("Vous n'avez pas les droits nécessaires pour créer un nouveau conteneur. Veuillez contacter un administrateur", "Droits insuffisants !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    ConteneurForm contForm = new ConteneurForm("Nouveau", this, utilisateur);
                    contForm.cbNumCtr.IsEditable = true;
                    contForm.actionsBorder.Visibility = System.Windows.Visibility.Collapsed;
                    contForm.txtDescription.IsReadOnly = false;
                    contForm.txtDescription.Background = Brushes.White;
                    contForm.txtDescMses.IsReadOnly = false;
                    contForm.txtDescMses.Background = Brushes.White;
                    contForm.txtIMDGCode.IsReadOnly = false;
                    contForm.txtIMDGCode.Background = Brushes.White;
                    contForm.txtPoids.IsReadOnly = false;
                    contForm.txtPoids.Background = Brushes.White;
                    contForm.cbEtatM.IsEnabled = true;
                    contForm.cbTypeCtrM.IsEnabled = true;
                    contForm.cbNumBL.IsEditable = true;
                    contForm.cbNumBL.IsEnabled = true;
                    contForm.groupInfosCub.IsEnabled = false;
                    contForm.Title = "Nouveau : Conteneur";
                    contForm.actionsBorder.Visibility = System.Windows.Visibility.Collapsed;
                    contForm.Show();
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
                    conteneurs = vsp.GetConteneursImport();
                    dataGrid.ItemsSource = conteneurs;
                    lblStatut.Content = conteneurs.Count + " Conteneur(s)";
                }
                else if (cbFiltres.SelectedIndex == 1)
                {
                    conteneurs = vsp.GetConteneursImportByStatut("Non initié");
                    dataGrid.ItemsSource = conteneurs;
                    lblStatut.Content = conteneurs.Count + " Conteneur(s)";
                }
                else if (cbFiltres.SelectedIndex == 2)
                {
                    conteneurs = vsp.GetConteneursImportByStatut("Traité");
                    dataGrid.ItemsSource = conteneurs;
                    lblStatut.Content = conteneurs.Count + " Conteneur(s)";
                }
                else if (cbFiltres.SelectedIndex == 3)
                {
                    conteneurs = vsp.GetConteneursImportByStatut("Manifesté");
                    dataGrid.ItemsSource = conteneurs;
                    lblStatut.Content = conteneurs.Count + " Conteneur(s)";
                }
                else if (cbFiltres.SelectedIndex == 4)
                {
                    conteneurs = vsp.GetConteneursImportByStatut("Déchargé");
                    dataGrid.ItemsSource = conteneurs;
                    lblStatut.Content = conteneurs.Count + " Conteneur(s)";
                }
                else if (cbFiltres.SelectedIndex == 5)
                {
                    conteneurs = vsp.GetConteneursImportByStatut("Parqué");
                    dataGrid.ItemsSource = conteneurs;
                    lblStatut.Content = conteneurs.Count + " Conteneur(s)";
                }
                else if (cbFiltres.SelectedIndex == 6)
                {
                    conteneurs = vsp.GetConteneursImportByStatut("Enlèvement");
                    dataGrid.ItemsSource = conteneurs;
                    lblStatut.Content = conteneurs.Count + " Conteneur(s)";
                }
                else if (cbFiltres.SelectedIndex == 7)
                {
                    conteneurs = vsp.GetConteneursImportByStatut("Livré");
                    dataGrid.ItemsSource = conteneurs;
                    lblStatut.Content = conteneurs.Count + " Conteneur(s)";
                }
                else if (cbFiltres.SelectedIndex == 8)
                {
                    conteneurs = vsp.GetConteneursImportByStatut("Retourné");
                    dataGrid.ItemsSource = conteneurs;
                    lblStatut.Content = conteneurs.Count + " Conteneur(s)";
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
                        conteneurs = vsp.GetConteneursImportByNumBL(txtRechercher.Text.Trim());
                        dataGrid.ItemsSource = conteneurs;
                        lblStatut.Content = conteneurs.Count + " Conteneur(s) trouvé(s)";
                    }
                    else if (listRechercher.SelectedIndex == 1)
                    {
                        conteneurs = vsp.GetConteneursImportByNumCtr(txtRechercher.Text.Trim());
                        dataGrid.ItemsSource = conteneurs;
                        lblStatut.Content = conteneurs.Count + " Conteneur(s) trouvé(s)";
                    }
                    else if (listRechercher.SelectedIndex == 2)
                    {
                        int result;
                        conteneurs = vsp.GetConteneursImportByNumEscale(Int32.TryParse(txtRechercher.Text.Trim(), out result) ? result : -1);
                        dataGrid.ItemsSource = conteneurs;
                        lblStatut.Content = conteneurs.Count + " Conteneur(s) trouvé(s)";
                    }
                    else if (listRechercher.SelectedIndex == 3)
                    {
                        conteneurs = vsp.GetConteneursImportByDescription(txtRechercher.Text.Trim());
                        dataGrid.ItemsSource = conteneurs;
                        lblStatut.Content = conteneurs.Count + " Conteneur(s) trouvé(s)";
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

        private void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                if (dataGrid.SelectedIndex != -1 && dataGrid.SelectedItems.Count == 1)
                {
                    CONTENEUR ctr = vsp.GetConteneurImportByIdCtr(((CONTENEUR)dataGrid.SelectedItem).IdCtr);
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
                    CONTENEUR ctr = vsp.GetConteneurImportByIdCtr(((CONTENEUR)dataGrid.SelectedItem).IdCtr);
                    IdentificationConteneurForm identForm = new IdentificationConteneurForm(this, ctr, utilisateur);
                    identForm.txtSeal1.Text = ctr.Seal1Ctr;
                    identForm.txtSeal2.Text = ctr.Seal2Ctr;
                    identForm.Title = "Identification - Conteneur N° : " + ctr.NumCtr;
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
                    CONTENEUR ctr = vsp.GetConteneurImportByIdCtr(((CONTENEUR)dataGrid.SelectedItem).IdCtr);
                    SortirConteneurForm sortirForm = new SortirConteneurForm(this, ctr, utilisateur);
                    sortirForm.Title = "Sortir - Conteneur N° : " + ctr.NumCtr;
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

        private void btnRetour_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                if (dataGrid.SelectedIndex != -1 && dataGrid.SelectedItems.Count == 1)
                {
                    CONTENEUR ctr = vsp.GetConteneurImportByIdCtr(((CONTENEUR)dataGrid.SelectedItem).IdCtr);
                    RetourConteneurForm retourForm = new RetourConteneurForm(this, ctr, utilisateur);
                    retourForm.Title = "Retour - Conteneur N° : " + ctr.NumCtr;
                    retourForm.ShowDialog();
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
