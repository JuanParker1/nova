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
    /// Logique d'interaction pour ListDispositionCtrForm.xaml
    /// </summary>
    public partial class ListDispositionCtrForm : Window
    {
        public List<DISPOSITION_CONTENEUR> posiConteneursTC { get; set; }

        private ConteneurTCForm ctrTCForm;
        private ConteneurTCPanel ctrTCPanel;
        private CONTENEUR_TC ctrTC;
        private List<OPERATION> operationsUser;
        private UTILISATEUR utilisateur;

        private FormLoader formLoader;
        private VsomParameters vsp = new VsomParameters();
        public ListDispositionCtrForm(ConteneurTCForm form, List<DISPOSITION_CONTENEUR> listPosiConteneurs, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

                posiConteneursTC = listPosiConteneurs;
                dataGrid.ItemsSource = posiConteneursTC;
                
                utilisateur = user;
                operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);

                ctrTCForm = form;
                ctrTC = vsp.GetConteneurTCByIdCtr(Convert.ToInt32(ctrTCForm.txtIdCtr.Text));

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

        public ListDispositionCtrForm(ConteneurTCPanel form, CONTENEUR_TC ctr, List<DISPOSITION_CONTENEUR> listPosiConteneurs, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

                posiConteneursTC = listPosiConteneurs;
                dataGrid.ItemsSource = posiConteneursTC;

                utilisateur = user;
                operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);

                ctrTCPanel = form;
                ctrTC = ctr;

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

                    PositionnerConteneurForm positionnerForm = new PositionnerConteneurForm(this, ctrTC, ((DISPOSITION_CONTENEUR)dataGrid.SelectedItem).IdDisposition, utilisateur);
                    if (ctrTC.PARC != null)
                    {
                        positionnerForm.txtParc.Text = ctrTC.PARC.NomParc;
                        positionnerForm.txtEmplacement.Text = vsp.GetEmplacementByIdEmpl(ctrTC.IdEmplacementParc.Value).LigEmpl + vsp.GetEmplacementByIdEmpl(ctrTC.IdEmplacementParc.Value).ColEmpl;
                    }
                    
                    positionnerForm.Title = "Positionnement - Conteneur N° : " + ctrTC.NumTC;
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
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnSelectionner_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dataGrid.SelectedIndex != -1 && dataGrid.SelectedItems.Count == 1)
                {
                    VSOMAccessors vsomAcc = new VSOMAccessors();

                    PositionnerConteneurForm positionnerForm = new PositionnerConteneurForm(this, ctrTC, ((DISPOSITION_CONTENEUR)dataGrid.SelectedItem).IdDisposition, utilisateur);
                    if (ctrTC.PARC != null)
                    {
                        positionnerForm.txtParc.Text = ctrTC.PARC.NomParc;
                        positionnerForm.txtEmplacement.Text = vsp.GetEmplacementByIdEmpl(ctrTC.IdEmplacementParc.Value).LigEmpl + vsp.GetEmplacementByIdEmpl(ctrTC.IdEmplacementParc.Value).ColEmpl;
                    }
                    
                    positionnerForm.Title = "Positionnement - Conteneur N° : " + ctrTC.NumTC;
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
                    dataGrid.ItemsSource = vsp.GetDemandesDispositionEnCoursByTypeCtrAndBooking(posiConteneursTC.FirstOrDefault<DISPOSITION_CONTENEUR>().TypeCtr, txtRechercher.Text.Trim());
                }
                else if (e.Key == Key.Escape)
                {
                    txtRechercher.Text = null;
                    dataGrid.ItemsSource = posiConteneursTC;
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
