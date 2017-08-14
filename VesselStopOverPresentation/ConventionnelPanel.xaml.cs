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
    /// Logique d'interaction pour ConventionnelPanel.xaml
    /// </summary>
    public partial class ConventionnelPanel : DockPanel
    {
        public List<CONVENTIONNEL> conventionnels { get; set; }
        private List<OPERATION> operationsUser;
        private UTILISATEUR utilisateur;
        private VsomParameters vsp = new VsomParameters();

        public ConventionnelPanel(UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;
                listRechercher.SelectedIndex = 0;
                //cbFiltres.SelectedIndex = 0;
                //lblStatut.Content = conventionnels.Count + " Conventionnel(s)";

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

        private void dataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();
                CONVENTIONNEL conv = vsp.GetConventionnelByIdGC(((CONVENTIONNEL)dataGrid.SelectedItem).IdGC);
                if (dataGrid.SelectedIndex != -1)
                {
                    ConventionnelForm convForm = new ConventionnelForm(this, conv, utilisateur);
                    convForm.Show();
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
                    conventionnels = vsp.GetConventionnelsImport();
                    dataGrid.ItemsSource = conventionnels;
                    lblStatut.Content = conventionnels.Count + " Conventionnel(s)";
                }
                else if (cbFiltres.SelectedIndex == 1)
                {
                    conventionnels = vsp.GetConventionnelsImportByStatut("Non initié");
                    dataGrid.ItemsSource = conventionnels;
                    lblStatut.Content = conventionnels.Count + " Conventionnel(s)";
                }
                else if (cbFiltres.SelectedIndex == 2)
                {
                    conventionnels = vsp.GetConventionnelsImportByStatut("Traité");
                    dataGrid.ItemsSource = conventionnels;
                    lblStatut.Content = conventionnels.Count + " Conventionnel(s)";
                }
                else if (cbFiltres.SelectedIndex == 3)
                {
                    conventionnels = vsp.GetConventionnelsImportByStatut("Manifesté");
                    dataGrid.ItemsSource = conventionnels;
                    lblStatut.Content = conventionnels.Count + " Conventionnel(s)";
                }
                else if (cbFiltres.SelectedIndex == 4)
                {
                    conventionnels = vsp.GetConventionnelsImportByStatut("Identifié/Déchargé");
                    dataGrid.ItemsSource = conventionnels;
                    lblStatut.Content = conventionnels.Count + " Conventionnel(s)";
                }
                else if (cbFiltres.SelectedIndex == 5)
                {
                    conventionnels = vsp.GetConventionnelsImportByStatut("Enlèvement");
                    dataGrid.ItemsSource = conventionnels;
                    lblStatut.Content = conventionnels.Count + " Conventionnel(s)";
                }
                else if (cbFiltres.SelectedIndex == 6)
                {
                    conventionnels = vsp.GetConventionnelsImportByStatut("Sortie");
                    dataGrid.ItemsSource = conventionnels;
                    lblStatut.Content = conventionnels.Count + " Conventionnel(s)";
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
                        conventionnels = vsp.GetConventionnelsImportByNumBL(txtRechercher.Text.Trim());
                        dataGrid.ItemsSource = conventionnels;
                        lblStatut.Content = conventionnels.Count + " Conventionnel(s) trouvé(s)";
                    }
                    else if (listRechercher.SelectedIndex == 1)
                    {
                        conventionnels = vsp.GetConventionnelsByNumGC(txtRechercher.Text.Trim());
                        dataGrid.ItemsSource = conventionnels;
                        lblStatut.Content = conventionnels.Count + " Conventionnel(s) trouvé(s)";
                    }
                    else if (listRechercher.SelectedIndex == 2)
                    {
                        int result;
                        conventionnels = vsp.GetConventionnelsByNumEscale(Int32.TryParse(txtRechercher.Text.Trim(), out result) ? result : -1);
                        dataGrid.ItemsSource = conventionnels;
                        lblStatut.Content = conventionnels.Count + " Conventionnel(s) trouvé(s)";
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

        private void btnAnnulerRecherche_Click(object sender, RoutedEventArgs e)
        {
            txtRechercher.Text = null;
        }

        private void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (dataGrid.SelectedIndex != -1 && dataGrid.SelectedItems.Count == 1)
                {
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
                    CONVENTIONNEL conv = vsp.GetConventionnelByIdGC(((CONVENTIONNEL)dataGrid.SelectedItem).IdGC);
                    IdentificationConventionnelForm identForm = new IdentificationConventionnelForm(this, conv, utilisateur);
                    identForm.Title = "Identification - GC N° : " + conv.NumGC;
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

        private void btnCuber_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                if (dataGrid.SelectedIndex != -1 && dataGrid.SelectedItems.Count == 1)
                {
                    CONVENTIONNEL conv = vsp.GetConventionnelByIdGC(((CONVENTIONNEL)dataGrid.SelectedItem).IdGC);
                    CubageConventionnelForm cubForm = new CubageConventionnelForm(this, conv, utilisateur);
                    cubForm.Title = "Cubage - GC N° : " + conv.NumGC;
                    cubForm.ShowDialog();
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

        private void btnReceptionner_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                if (dataGrid.SelectedIndex != -1 && dataGrid.SelectedItems.Count == 1)
                {
                    CONVENTIONNEL conv = vsp.GetConventionnelByIdGC(((CONVENTIONNEL)dataGrid.SelectedItem).IdGC);
                    ReceptionConventionnelForm receptionForm = new ReceptionConventionnelForm(this, conv, utilisateur);
                    receptionForm.Title = "Réception - GC N° : " + conv.NumGC;
                    receptionForm.ShowDialog();
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
                    CONVENTIONNEL conv = vsp.GetConventionnelByIdGC(((CONVENTIONNEL)dataGrid.SelectedItem).IdGC);
                    SortirConventionnelForm sortieForm = new SortirConventionnelForm(this, conv, utilisateur);
                    sortieForm.Title = "Sortie - GC N° : " + conv.NumGC;
                    sortieForm.ShowDialog();
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
                if (operationsUser.Where(op => op.NomOp == "General cargo : Enregistrement d'un nouvel élément").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
                {
                    MessageBox.Show("Vous n'avez pas les droits nécessaires pour créer un nouveau general cargo. Veuillez contacter un administrateur", "Droits insuffisants !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    ConventionnelForm convForm = new ConventionnelForm("Nouveau", this, utilisateur);
                    convForm.cbNumGC.IsEditable = true;
                    convForm.txtLongM.Text = "0";
                    convForm.txtLargM.Text = "0";
                    convForm.txtHautM.Text = "0";
                    convForm.txtVolM.Text = "0";
                    convForm.txtPoidsM.Text = "0";
                    convForm.Title = "Nouveau : Véhicule";
                    convForm.actionsBorder.Visibility = System.Windows.Visibility.Collapsed;
                    convForm.txtBarCode.IsReadOnly = false;
                    convForm.txtBarCode.Background = Brushes.White;
                    convForm.txtLongM.IsReadOnly = false;
                    convForm.txtLongM.Background = Brushes.White;
                    convForm.txtLargM.IsReadOnly = false;
                    convForm.txtLargM.Background = Brushes.White;
                    convForm.txtHautM.IsReadOnly = false;
                    convForm.txtHautM.Background = Brushes.White;
                    convForm.txtVolM.IsReadOnly = false;
                    convForm.txtVolM.Background = Brushes.White;
                    convForm.txtPoidsM.IsReadOnly = false;
                    convForm.txtPoidsM.Background = Brushes.White;
                    convForm.cbNumBL.IsEditable = true;
                    convForm.groupInfosCub.IsEnabled = false;
                    convForm.Show();
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
