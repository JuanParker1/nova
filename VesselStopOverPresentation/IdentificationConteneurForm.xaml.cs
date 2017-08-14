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
    /// Logique d'interaction pour IdentificationConteneurForm.xaml
    /// </summary>
    public partial class IdentificationConteneurForm : Window
    {

        public List<TYPE_SINISTRE> typesSinistres { get; set; }

        private ConteneurForm ctrForm;
        private ConteneurTCForm ctrTCForm;
        private ConteneurPanel ctrPanel;
        private ConteneurTCPanel ctrTCPanel;
        private CONTENEUR conteneur;
        private CONTENEUR_TC conteneurTC;
        private List<OPERATION> operationsUser;
        private UTILISATEUR utilisateur;

        private FormLoader formLoader;
        private VsomParameters vsp = new VsomParameters();

        public IdentificationConteneurForm(ConteneurForm form, UTILISATEUR user)
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

                ctrForm = form;

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

        public IdentificationConteneurForm(ConteneurTCForm form, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();
                VsomParameters vsprm = new VsomParameters();
                InitializeComponent();
                this.DataContext = this;

                typesSinistres = vsprm.GetTypesSinistreCtr();

                utilisateur = user;
                operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);

                ctrTCForm = form;
                labelBouton.Text = "Débarquer";
                lableBlockBouton.Text = "Débarquer";

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

        public IdentificationConteneurForm(ConteneurPanel panel, CONTENEUR ctr, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();
                VsomParameters vsprm = new VsomParameters();
                InitializeComponent();
                this.DataContext = this;

                typesSinistres = vsprm.GetTypesSinistreCtr();

                conteneur = ctr;

                utilisateur = user;
                operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);

                ctrPanel = panel;

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

        public IdentificationConteneurForm(ConteneurTCPanel panel, CONTENEUR_TC ctr, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();
                VsomParameters vsprm = new VsomParameters();
                InitializeComponent();
                this.DataContext = this;

                typesSinistres = vsprm.GetTypesSinistreCtr();

                conteneurTC = ctr;

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

        private void btnIdentifier_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //VSOMAccessors vsomAcc = new VSOMAccessors();
                VsomMarchal vsomAcc = new VsomMarchal();
                if (txtSeal1.Text.Trim() == "")
                {
                    MessageBox.Show("Veuillez entrer au moins un numéro de plomb", "N° de plomb", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    CONTENEUR ctr = null;
                    if (ctrForm != null)
                    {
                        ctr = vsomAcc.IdentifierConteneur(Convert.ToInt32(ctrForm.txtIdCtr.Text), dataGridSinistres.SelectedItems.OfType<TYPE_SINISTRE>().ToList<TYPE_SINISTRE>(), txtSeal1.Text, txtSeal2.Text, new TextRange(txtObservations.Document.ContentStart, txtObservations.Document.ContentEnd).Text, utilisateur.IdU);
                        //Raffraîchir les informations

                        formLoader.LoadConteneurForm(ctrForm, ctr);
                    }
                    if (ctrTCForm != null)
                    {
                        ctr = vsomAcc.IdentifierConteneur(Convert.ToInt32(ctrTCForm.txtIdCtr.Text), dataGridSinistres.SelectedItems.OfType<TYPE_SINISTRE>().ToList<TYPE_SINISTRE>(), txtSeal1.Text, txtSeal2.Text, new TextRange(txtObservations.Document.ContentStart, txtObservations.Document.ContentEnd).Text, utilisateur.IdU);
                        //Raffraîchir les informations

                        formLoader.LoadConteneurTCForm(ctrTCForm, ctr.CONTENEUR_TC.FirstOrDefault<CONTENEUR_TC>());
                    }
                    else if (ctrPanel != null)
                    {
                        ctr = vsomAcc.IdentifierConteneur(((CONTENEUR)ctrPanel.dataGrid.SelectedItem).IdCtr, dataGridSinistres.SelectedItems.OfType<TYPE_SINISTRE>().ToList<TYPE_SINISTRE>(), txtSeal1.Text, txtSeal2.Text, new TextRange(txtObservations.Document.ContentStart, txtObservations.Document.ContentEnd).Text, utilisateur.IdU);
                        //Raffraîchir les informations
                        //ctrPanel.dataGrid.ItemsSource = vsomAcc.GetConteneursImport();
                    }
                    else if (ctrTCPanel != null)
                    {
                        ctr = vsomAcc.IdentifierConteneur(((CONTENEUR_TC)ctrTCPanel.dataGrid.SelectedItem).IdCtr.Value, dataGridSinistres.SelectedItems.OfType<TYPE_SINISTRE>().ToList<TYPE_SINISTRE>(), txtSeal1.Text, txtSeal2.Text, new TextRange(txtObservations.Document.ContentStart, txtObservations.Document.ContentEnd).Text, utilisateur.IdU);
                        //Raffraîchir les informations
                        //ctrPanel.dataGrid.ItemsSource = vsomAcc.GetConteneursImport();
                    }

                    MessageBox.Show("L'opération d'identification s'est déroulée avec succès, consultez le journal des éléments de facturation", "Conteneur débarqué !", MessageBoxButton.OK, MessageBoxImage.Information);

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

        private static string FormatChiffre(int entier)
        {
            Int32 i = entier;
            if (i >= 10)
            {
                return i.ToString();
            }
            else
            {
                return "0" + i.ToString();
            }
        }
    }
}
