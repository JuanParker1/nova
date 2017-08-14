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
using System.Windows.Shapes;
using VesselStopOverData;
using System.Text.RegularExpressions;

namespace VesselStopOverPresentation
{
    /// <summary>
    /// Logique d'interaction pour ListConteneurTCPositionnerForm.xaml
    /// </summary>
    public partial class ListConteneurTCPositionnerForm : Window
    {
        public List<CONTENEUR_TC> conteneursTC { get; set; }

        private MiseDispositionPanel dispoCtrPanel;
        private List<OPERATION> operationsUser;
        private UTILISATEUR utilisateur;

        private FormLoader formLoader;

        private int idDisposition;
        private VsomParameters vsp = new VsomParameters();
        public ListConteneurTCPositionnerForm(MiseDispositionPanel panel, int idMiseDisposition, List<CONTENEUR_TC> listConteneurs, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

                conteneursTC = listConteneurs;
                dataGrid.ItemsSource = conteneursTC;
                
                utilisateur = user;
                operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);

                dispoCtrPanel = panel;

                idDisposition = idMiseDisposition;

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

        private void dataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (dataGrid.SelectedIndex != -1)
                {
                    VSOMAccessors vsomAcc = new VSOMAccessors();

                    CONTENEUR_TC contTC = (CONTENEUR_TC)dataGrid.SelectedItem;

                    PositionnerConteneurForm positionnerForm = new PositionnerConteneurForm(this, contTC, idDisposition, utilisateur);
                    positionnerForm.txtParc.Text = contTC.PARC.NomParc;
                    positionnerForm.txtEmplacement.Text = vsp.GetEmplacementByIdEmpl(contTC.IdEmplacementParc.Value).LigEmpl + vsp.GetEmplacementByIdEmpl(contTC.IdEmplacementParc.Value).ColEmpl;
                    positionnerForm.Title = "Positionnement - Conteneur N° : " + contTC.NumTC;
                    positionnerForm.ShowDialog();
                    this.Close();
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

        private void btnSelectionner_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dataGrid.SelectedIndex != -1 && dataGrid.SelectedItems.Count == 1)
                {
                    VSOMAccessors vsomAcc = new VSOMAccessors();

                    CONTENEUR_TC contTC = (CONTENEUR_TC)dataGrid.SelectedItem;

                    PositionnerConteneurForm positionnerForm = new PositionnerConteneurForm(this, contTC, idDisposition, utilisateur);
                    positionnerForm.txtParc.Text = contTC.PARC.NomParc;
                    positionnerForm.txtEmplacement.Text = vsp.GetEmplacementByIdEmpl(contTC.IdEmplacementParc.Value).LigEmpl + vsp.GetEmplacementByIdEmpl(contTC.IdEmplacementParc.Value).ColEmpl;
                    positionnerForm.Title = "Positionnement - Conteneur N° : " + contTC.NumTC;
                    positionnerForm.ShowDialog();
                    this.Close();
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

                if (e.Key == Key.Return)
                {
                    dataGrid.ItemsSource = vsp.GetConteneurTCsByTypeAndStatutAndNumTC(vsp.GetMiseDispositionByIdDisposition(idDisposition).TypeCtr, "Parqué", utilisateur.IdParc.Value, txtRechercher.Text.Trim());
                }
                else if (e.Key == Key.Escape)
                {
                    txtRechercher.Text = null;
                    dataGrid.ItemsSource = conteneursTC;
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
