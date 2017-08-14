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
    /// Logique d'interaction pour UtilisateurPanel.xaml
    /// </summary>
    public partial class UtilisateurPanel : DockPanel
    {
        private List<OPERATION> operationsUser;
        private UTILISATEUR utilisateur;

        private FormLoader formLoader;
        //private VsomParameters vsomAcc = new VsomParameters();
        private VSOMAccessors vsomAcc;
        public UtilisateurPanel(UTILISATEUR user)
        {
            try
            {
                 vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;
                listRechercher.SelectedIndex = 0;
                //cbFiltres.SelectedIndex = 0;
                //lblStatut.Content = conteneurs.Count + " Conteneur(s)";

                utilisateur = user;
                operationsUser = vsomAcc.GetOperationsUtilisateur(utilisateur.IdU);

                formLoader = new FormLoader(utilisateur);
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
            
        }

        public List<UTILISATEUR> utilisateurs { get; set; }

        private void dataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
               // VSOMAccessors vsomAcc = new VSOMAccessors();
               // VsomParameters vsomAcc = new VsomParameters();
                if (dataGrid.SelectedIndex != -1)
                {
                    UtilisateurForm userForm = new UtilisateurForm(this, utilisateur);
                    UTILISATEUR u = vsomAcc.GetUtilisateursByIdU(((UTILISATEUR)dataGrid.SelectedItem).IdU);
                    userForm.userOfForm = u;
                    formLoader.LoadUtilisateurForm(userForm, u);
                    userForm.Show();
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
                if (operationsUser.Where(op => op.NomOp == "Opération sur les comptes utilisateurs").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
                {
                    MessageBox.Show("Vous n'avez pas les droits nécessaires pour créer un nouvel utilisateur. Veuillez contacter un administrateur", "Droits insuffisants !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    UtilisateurForm utilisateurForm = new UtilisateurForm(this, utilisateur);
                    utilisateurForm.Title = "Nouveau : Utilisateur";
                    utilisateurForm.Show();
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
               // VsomParameters vsomAcc = new VsomParameters();
                if (cbFiltres.SelectedIndex == 0)
                {
                    utilisateurs = vsomAcc.GetUtilisateurs();
                    dataGrid.ItemsSource = utilisateurs;
                    lblStatut.Content = utilisateurs.Count + " Utilisateur(s)";
                }
                else if (cbFiltres.SelectedIndex == 1)
                {
                    utilisateurs = vsomAcc.GetUtilisateursByStatut("A");
                    dataGrid.ItemsSource = utilisateurs;
                    lblStatut.Content = utilisateurs.Count + " Utilisateur(s)";
                }
                else if (cbFiltres.SelectedIndex == 2)
                {
                    utilisateurs = vsomAcc.GetUtilisateursByStatut("I");
                    dataGrid.ItemsSource = utilisateurs;
                    lblStatut.Content = utilisateurs.Count + " Utilisateur(s)";
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
                //VSOMAccessors vsomAcc = new VSOMAccessors();
                //VsomParameters vsomAcc = new VsomParameters();
                if (e.Key == Key.Return && listRechercher.SelectedItem != null)
                {
                    if (listRechercher.SelectedIndex == 0)
                    {
                        utilisateurs = vsomAcc.GetUtilisateursByNom(txtRechercher.Text.Trim());
                        dataGrid.ItemsSource = utilisateurs;
                        lblStatut.Content = utilisateurs.Count + " Utilisateur(s) trouvé(s)";
                    }
                    else if (listRechercher.SelectedIndex == 1)
                    {
                        utilisateurs = vsomAcc.GetUtilisateursByAcconier(txtRechercher.Text.Trim());
                        dataGrid.ItemsSource = utilisateurs;
                        lblStatut.Content = utilisateurs.Count + " Utilisateur(s) trouvé(s)";
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
