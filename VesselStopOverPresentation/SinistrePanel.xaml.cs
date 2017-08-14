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
    /// Logique d'interaction pour SinistrePanel.xaml
    /// </summary>
    public partial class SinistrePanel : DockPanel
    {

        public List<OPERATION_SINISTRE> sinistres { get; set; }
        private List<OPERATION> operationsUser;
        private UTILISATEUR utilisateur;
        private VsomParameters vsp = new VsomParameters();
        public SinistrePanel(UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;
                listRechercher.SelectedIndex = 0;
                cbFiltres.SelectedIndex = 0;

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
                VsomParameters vsomAcc = new  VsomParameters();

                if (cbFiltres.SelectedIndex == 0)
                {
                    sinistres = vsomAcc.GetSinistres();
                    dataGrid.ItemsSource = sinistres;
                    lblStatut.Content = sinistres.Count + " Sinistre(s)";
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
                        sinistres = vsp.GetSinistresByNumChassis(txtRechercher.Text.Trim());
                        dataGrid.ItemsSource = sinistres;
                        lblStatut.Content = sinistres.Count + " Sinistre(s) trouvé(s)";
                    }
                    else if (listRechercher.SelectedIndex == 1)
                    {
                        sinistres = vsp.GetSinistresByNumBL(txtRechercher.Text.Trim());
                        dataGrid.ItemsSource = sinistres;
                        lblStatut.Content = sinistres.Count + " Sinistre(s) trouvé(s)";
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

        private void btnNouveau_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (operationsUser.Where(op => op.NomOp == "Véhicule : Constat de sinistre").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
                {
                    MessageBox.Show("Vous n'avez pas les droits nécessaires pour enregistrer une opération de sinistre sur un véhicule. Veuillez contacter un administrateur", "Droits insuffisants !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    SinistreForm sinForm = new SinistreForm("Nouveau", this, utilisateur);
                    sinForm.Title = "Nouveau : Opération de constat de sinistre";
                    sinForm.cbIdSinistre.IsEnabled = false;
                    sinForm.Show();
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
    }
}
