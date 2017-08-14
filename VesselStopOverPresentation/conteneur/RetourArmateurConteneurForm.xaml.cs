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
    /// Logique d'interaction pour RetourArmateurConteneurForm.xaml
    /// </summary>
    public partial class RetourArmateurConteneurForm : Window
    {
        public List<TYPE_SINISTRE> typesSinistres { get; set; }

        private ConteneurTCForm ctrTCForm;
        private ConteneurTCPanel ctrTCPanel;
        private CONTENEUR_TC conteneurTC;
        private List<OPERATION> operationsUser;
        private UTILISATEUR utilisateur;

        private FormLoader formLoader;
        private VsomParameters vsp = new VsomParameters();
        public RetourArmateurConteneurForm(ConteneurTCForm form, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();
                //VsomParameters vsprm = new VsomParameters();
                InitializeComponent();
                this.DataContext = this;

                typesSinistres = vsp.GetTypesSinistreCtr();

                utilisateur = user;
                operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);

                ctrTCForm = form;

                formLoader = new FormLoader(utilisateur);

                //if (utilisateur.LU != "Admin")
                //{
                //    txtDateRetour.IsEnabled = false;
                //}

            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        public RetourArmateurConteneurForm(ConteneurTCPanel form, CONTENEUR_TC ctr, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();
                VsomParameters vsp = new VsomParameters();
                InitializeComponent();
                this.DataContext = this;

                typesSinistres = vsp.GetTypesSinistreCtr();

                utilisateur = user;
                operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);

                ctrTCPanel = form;
                conteneurTC = ctr;

                formLoader = new FormLoader(utilisateur);

                //if (utilisateur.LU != "Admin")
                //{
                //    txtDateRetour.IsEnabled = false;
                //}

            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        private void btnRetourArmateur_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                CONTENEUR ctr = null;
                if (ctrTCForm != null)
                {
                    ctr = vsomAcc.RetournerConteneurArmateur(Convert.ToInt32(ctrTCForm.txtIdCtr.Text), dataGridSinistres.SelectedItems.OfType<TYPE_SINISTRE>().ToList<TYPE_SINISTRE>(), new TextRange(txtObservations.Document.ContentStart, txtObservations.Document.ContentEnd).Text, utilisateur.IdU);
                    //Raffraîchir les informations

                    formLoader.LoadConteneurTCForm(ctrTCForm, ctr.CONTENEUR_TC.FirstOrDefault<CONTENEUR_TC>());
                }
                else if (ctrTCPanel != null)
                {
                    ctr = vsomAcc.RetournerConteneurArmateur(((CONTENEUR_TC)ctrTCPanel.dataGrid.SelectedItem).IdCtr.Value, dataGridSinistres.SelectedItems.OfType<TYPE_SINISTRE>().ToList<TYPE_SINISTRE>(), new TextRange(txtObservations.Document.ContentStart, txtObservations.Document.ContentEnd).Text, utilisateur.IdU);
                    //Raffraîchir les informations
                }

                MessageBox.Show("L'opération de retour de conteneur à l'armateur s'est déroulée avec succès, consultez le journal des éléments de facturation", "Conteneur retourné à l'armateur !", MessageBoxButton.OK, MessageBoxImage.Information);

                this.Close();
            }
            catch (HabilitationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (IdentificationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération", MessageBoxButton.OK, MessageBoxImage.Error);
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
    }
}
