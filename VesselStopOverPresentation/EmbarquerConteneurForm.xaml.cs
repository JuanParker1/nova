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
    /// Logique d'interaction pour EmbarquerConteneurForm.xaml
    /// </summary>
    public partial class EmbarquerConteneurForm : Window
    {

        public List<TYPE_SINISTRE> typesSinistres { get; set; }

        private ConteneurExportPanel ctrExportPanel;
        private ConteneurTCPanel ctrTCPanel;
        private ConteneurTCForm ctrTCForm;
        private CONTENEUR conteneur;
        private List<OPERATION> operationsUser;
        private UTILISATEUR utilisateur;

        private FormLoader formLoader;
        private VsomParameters vsp = new VsomParameters();
        public EmbarquerConteneurForm(ConteneurExportPanel panel, CONTENEUR ctr, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();
                //VsomParameters vsprm = new VsomParameters();
                InitializeComponent();
                this.DataContext = this;

                typesSinistres = vsp.GetTypesSinistreCtr();

                conteneur = ctr;

                utilisateur = user;
                operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);

                ctrExportPanel = panel;

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

        public EmbarquerConteneurForm(ConteneurTCPanel panel, CONTENEUR ctr, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();
                VsomParameters vsp = new VsomParameters();
                InitializeComponent();
                this.DataContext = this;

                typesSinistres = vsp.GetTypesSinistreCtr();

                conteneur = ctr;

                utilisateur = user;
                operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);

                ctrTCPanel = panel;

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

        public EmbarquerConteneurForm(ConteneurTCForm form, CONTENEUR ctr, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();
                VsomParameters vsp = new VsomParameters();
                InitializeComponent();
                this.DataContext = this;

                typesSinistres = vsp.GetTypesSinistreCtr();

                conteneur = ctr;

                utilisateur = user;
                operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);

                ctrTCForm = form;

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

        private void btnEmbarquer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                if (txtSeal1.Text.Trim() == "")
                {
                    MessageBox.Show("Veuillez entrer au moins un numéro de plomb", "N° de plomb", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    if (ctrExportPanel != null)
                    {
                        CONTENEUR ctr = vsomAcc.EmbarquerConteneur(((CONTENEUR)ctrExportPanel.dataGrid.SelectedItem).IdCtr, dataGridSinistres.SelectedItems.OfType<TYPE_SINISTRE>().ToList<TYPE_SINISTRE>(), txtSeal1.Text, txtSeal2.Text, new TextRange(txtObservations.Document.ContentStart, txtObservations.Document.ContentEnd).Text, utilisateur.IdU);
                    }
                    else if (ctrTCForm != null)
                    {
                        CONTENEUR ctr = vsomAcc.EmbarquerConteneur(vsp.GetConteneurByIdCtr(Convert.ToInt32(ctrTCForm.txtIdCtr.Text)).CONTENEUR_TC.FirstOrDefault<CONTENEUR_TC>().IdCtrExport.Value, dataGridSinistres.SelectedItems.OfType<TYPE_SINISTRE>().ToList<TYPE_SINISTRE>(), txtSeal1.Text, txtSeal2.Text, new TextRange(txtObservations.Document.ContentStart, txtObservations.Document.ContentEnd).Text, utilisateur.IdU);
                    }
                    else if (ctrTCPanel != null)
                    {
                        CONTENEUR ctr = vsomAcc.EmbarquerConteneur(((CONTENEUR_TC)ctrExportPanel.dataGrid.SelectedItem).IdCtrExport.Value, dataGridSinistres.SelectedItems.OfType<TYPE_SINISTRE>().ToList<TYPE_SINISTRE>(), txtSeal1.Text, txtSeal2.Text, new TextRange(txtObservations.Document.ContentStart, txtObservations.Document.ContentEnd).Text, utilisateur.IdU);
                    }

                    MessageBox.Show("L'opération d'embarquement du conteneur s'est déroulée avec succès, consultez le journal des éléments de facturation", "Conteneur embarqué !", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();
                }
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
