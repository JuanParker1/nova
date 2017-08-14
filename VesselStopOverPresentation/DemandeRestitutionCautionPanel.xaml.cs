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
    /// Logique d'interaction pour DemandeRestitutionCautionPanel.xaml
    /// </summary>
    public partial class DemandeRestitutionCautionPanel : DockPanel
    {

        public List<DEMANDE_CAUTION> demandesCaution { get; set; }
        private UTILISATEUR utilisateur;
        private List<OPERATION> operationsUser;
        private VsomParameters vsp = new VsomParameters();
        public DemandeRestitutionCautionPanel(UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();
                VsomParameters vsp = new VsomParameters();

                InitializeComponent();
                this.DataContext = this;
                utilisateur = user;
                operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);
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
                    DemandeRestitutionCautionForm restCautionForm = new DemandeRestitutionCautionForm(this, (DEMANDE_CAUTION)dataGrid.SelectedItem, utilisateur);
                    restCautionForm.Show();
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
                if (operationsUser.Where(op => op.NomOp == "Demande de restitution de caution : Enregistrement d'un nouvel élément").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
                {
                    MessageBox.Show("Vous n'avez pas les droits nécessaires pour créer une nouvelle demande de restitution de caution. Veuillez contacter un administrateur", "Droits insuffisants !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    DemandeRestitutionCautionForm restCautionForm = new DemandeRestitutionCautionForm("Nouveau", this, utilisateur);
                    restCautionForm.Title = "Nouveau : Demande de restitution de caution";
                    restCautionForm.cbIdDRC.IsEnabled = false;
                    restCautionForm.borderActions.Visibility = System.Windows.Visibility.Collapsed;
                    restCautionForm.btnValider.Visibility = System.Windows.Visibility.Collapsed;
                    restCautionForm.Show();
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
                    demandesCaution = vsp.GetDemandesRestitution();
                    dataGrid.ItemsSource = demandesCaution;
                    lblStatut.Content = demandesCaution.Count + " Demande(s) de restitution de caution";
                }
                else if (cbFiltres.SelectedIndex == 1)
                {
                    demandesCaution = vsp.GetDemandesRestitutionValidees();
                    dataGrid.ItemsSource = demandesCaution;
                    lblStatut.Content = demandesCaution.Count + " Demande(s) de restitution de caution";
                }
                else if (cbFiltres.SelectedIndex == 2)
                {
                    demandesCaution = vsp.GetDemandesRestitutionEnAttente();
                    dataGrid.ItemsSource = demandesCaution;
                    lblStatut.Content = demandesCaution.Count + " Demande(s) de restitution de caution";
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
