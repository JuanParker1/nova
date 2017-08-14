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
    /// Logique d'interaction pour ExtensionFranchisePanel.xaml
    /// </summary>
    public partial class ExtensionFranchisePanel : DockPanel
    {

        public List<EXTENSION_FRANCHISE> extensions { get; set; }
        private UTILISATEUR utilisateur;
        private List<OPERATION> operationsUser;
        private VsomParameters vsp = new VsomParameters();
        public ExtensionFranchisePanel(UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;
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

        private void dataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                if (dataGrid.SelectedIndex != -1)
                {
                    ExtensionFranchiseForm extensionForm = new ExtensionFranchiseForm(this, vsp.GetExtensionFranchiseByIdExt(((EXTENSION_FRANCHISE)dataGrid.SelectedItem).IdDEXT), utilisateur);
                    extensionForm.Show();
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
                if (operationsUser.Where(op => op.NomOp == "Demande d'extention de franchise : Enregistrement d'un nouvel élément").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
                {
                    MessageBox.Show("Vous n'avez pas les droits nécessaires pour créer une nouvelle demande d'extension de franchise. Veuillez contacter un administrateur", "Droits insuffisants !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    ExtensionFranchiseForm extForm = new ExtensionFranchiseForm("Nouveau", this, utilisateur);
                    extForm.Title = "Nouveau : Demande d'extension de franchise";
                    extForm.cbIdExt.IsEnabled = false;
                    extForm.borderActions.Visibility = System.Windows.Visibility.Collapsed;
                    extForm.btnValider.Visibility = System.Windows.Visibility.Collapsed;
                    extForm.Show();
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
                    extensions = vsp.GetExtensionsFranchise();
                    dataGrid.ItemsSource = extensions;
                    lblStatut.Content = extensions.Count + " Demande(s) d'extension";
                }
                else if (cbFiltres.SelectedIndex == 1)
                {
                    extensions = vsp.GetExtensionsFranchiseValidees();
                    dataGrid.ItemsSource = extensions;
                    lblStatut.Content = extensions.Count + " Demande(s) d'extension";
                }
                else if (cbFiltres.SelectedIndex == 2)
                {
                    extensions = vsp.GetExtensionsFranchiseEnAttente();
                    dataGrid.ItemsSource = extensions;
                    lblStatut.Content = extensions.Count + " Demande(s) d'extension";
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
