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
    /// Logique d'interaction pour MiseDispositionPanel.xaml
    /// </summary>
    public partial class MiseDispositionPanel : DockPanel
    {
        private List<OPERATION> operationsUser;
        private UTILISATEUR utilisateur;
        private VsomParameters vsp = new VsomParameters();
        public MiseDispositionPanel(UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();
               // VsomParameters vsprm = new VsomParameters();
                InitializeComponent();
                this.DataContext = this;
                listRechercher.SelectedIndex = 0;
                //cbFiltres.SelectedIndex = 0;
                //lblStatut.Content = conteneurs.Count + " ConteneurTC(s)";

                actionsBorder.Visibility = System.Windows.Visibility.Collapsed;

                utilisateur = vsp.GetUtilisateursByIdU(user.IdU);
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

        public List<DISPOSITION_CONTENEUR> dispositionCtr { get; set; }

        private void cbFiltres_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                if (cbFiltres.SelectedIndex == 0)
                {
                    dispositionCtr = vsp.GetDemandesDisposition();
                }
                else if (cbFiltres.SelectedIndex == 1)
                {
                    dispositionCtr = vsp.GetDemandesDispositionEnCours();
                }
                else if (cbFiltres.SelectedIndex == 2)
                {
                    dispositionCtr = vsp.GetDemandesDispositionCloturees();
                }
                dataGrid.ItemsSource = dispositionCtr;
                lblStatut.Content = dispositionCtr.Count + " Mise(s) à disposition";
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
                        dispositionCtr = vsp.GetDemandesDispositionByRefDisposition(txtRechercher.Text.Trim());
                    }
                    else if (listRechercher.SelectedIndex == 1)
                    {
                        dispositionCtr = vsp.GetDemandesDispositionByNumBooking(txtRechercher.Text.Trim());
                    }
                    else if (listRechercher.SelectedIndex == 2)
                    {
                        int result;
                        dispositionCtr = vsp.GetDemandesDispositionByNumEscale(Int32.TryParse(txtRechercher.Text.Trim(), out result) ? result : -1);
                    }

                    dataGrid.ItemsSource = dispositionCtr;
                    lblStatut.Content = dispositionCtr.Count + " Mise(s) à disposition";
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

        private void btnPositionnerCtr_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                if (((DISPOSITION_CONTENEUR)dataGrid.SelectedItem).CONTENEUR_TC.Count < ((DISPOSITION_CONTENEUR)dataGrid.SelectedItem).NombreTC)
                {
                    if (utilisateur.IdParc.HasValue)
                    {
                        List<CONTENEUR_TC> conteneurs = vsp.GetConteneurTCsByTypeAndStatut(((DISPOSITION_CONTENEUR)dataGrid.SelectedItem).TypeCtr, "Parqué", utilisateur.IdParc.Value);

                        ListConteneurTCPositionnerForm listCtrForm = new ListConteneurTCPositionnerForm(this, ((DISPOSITION_CONTENEUR)dataGrid.SelectedItem).IdDisposition, conteneurs, utilisateur);
                        listCtrForm.Title = "Choix multiples : Sélectionnez un conteneur";
                        listCtrForm.ShowDialog();
                    }
                    else
                    {
                        throw new ApplicationException("Positionnement impossible : veuillez attribuer un parc à cet utilisateur");
                    }
                }
                else
                {
                    throw new ApplicationException("Positionnement impossible : cette demande de mise a disposition a été épuisée");
                }
                
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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

        private void dataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                MiseDispositionReport miseDispositionReport = new MiseDispositionReport(this, ((DISPOSITION_CONTENEUR)dataGrid.SelectedItem).RefDisposition);
                miseDispositionReport.Title = "Impression de la mise à disposition de conteneurs N° " + ((DISPOSITION_CONTENEUR)dataGrid.SelectedItem).RefDisposition + " - Booking : " + ((DISPOSITION_CONTENEUR)dataGrid.SelectedItem).CONNAISSEMENT.NumBooking;
                miseDispositionReport.Show();
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
