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
    /// Logique d'interaction pour SortirConteneurForm.xaml
    /// </summary>
    public partial class SortirConteneurForm : Window
    {

        public List<TYPE_SINISTRE> typesSinistres { get; set; }

        private ConteneurForm ctrForm;
        private ConteneurTCForm ctrTCForm;
        private ConteneurPanel ctrPanel;
        private CONTENEUR conteneur;
        private ConteneurTCPanel ctrTCPanel;
        private CONTENEUR_TC conteneurTC;
        private List<OPERATION> operationsUser;
        private UTILISATEUR utilisateur;

        private FormLoader formLoader;
        private VsomParameters vsp = new VsomParameters();
        public SortirConteneurForm(ConteneurForm form, UTILISATEUR user)
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

                //if (utilisateur.LU != "Admin")
                //{
                //    txtDateSortie.IsEnabled = false;
                //}

                if (operationsUser.Where(op => op.NomOp == "Field : Autorisation sur date").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
                {
                    txtDateSortie.IsEnabled = false;
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

        public SortirConteneurForm(ConteneurTCForm form, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();
               // VsomParameters vsp = new VsomParameters();
                InitializeComponent();
                this.DataContext = this;

                typesSinistres = vsp.GetTypesSinistreCtr();

                utilisateur = user;
                operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);

                ctrTCForm = form;

                formLoader = new FormLoader(utilisateur);

                //if (utilisateur.LU != "Admin")
                //{
                //    txtDateSortie.IsEnabled = false;
                //}

                if (operationsUser.Where(op => op.NomOp == "Field : Autorisation sur date").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
                {
                    txtDateSortie.IsEnabled = false;
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

        public SortirConteneurForm(ConteneurPanel panel, CONTENEUR ctr, UTILISATEUR user)
        {
            try
            {
                //VSOMAccessors vsomAcc = new VSOMAccessors();
                //VsomParameters vsp = new VsomParameters();
                InitializeComponent();
                this.DataContext = this;

                typesSinistres = vsp.GetTypesSinistreCtr();

                conteneur = ctr;

                utilisateur = user;
                operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);

                ctrPanel = panel;

                formLoader = new FormLoader(utilisateur);

                //if (utilisateur.LU != "Admin")
                //{
                //    txtDateSortie.IsEnabled = false;
                //}

                if (operationsUser.Where(op => op.NomOp == "Field : Autorisation sur date").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
                {
                    txtDateSortie.IsEnabled = false;
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

        public SortirConteneurForm(ConteneurTCPanel panel, CONTENEUR_TC ctr, UTILISATEUR user)
        {
            try
            {
                //VSOMAccessors vsomAcc = new VSOMAccessors();
                //VsomParameters vsprm = new VsomParameters();
                InitializeComponent();
                this.DataContext = this;

                typesSinistres = vsp.GetTypesSinistreCtr();

                conteneurTC = ctr;

                utilisateur = user;
                operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);

                ctrTCPanel = panel;

                formLoader = new FormLoader(utilisateur);

                //if (utilisateur.LU != "Admin")
                //{
                //    txtDateSortie.IsEnabled = false;
                //}

                if (operationsUser.Where(op => op.NomOp == "Field : Autorisation sur date").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
                {
                    txtDateSortie.IsEnabled = false;
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

        private void btnSortir_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //VSOMAccessors vsomAcc = new VSOMAccessors();
                VsomMarchal vsomAcc = new VsomMarchal();

                if (!txtDateSortie.SelectedDate.HasValue)
                {
                    MessageBox.Show("Veuillez entrer une date de sortie", "Echec de l'opération", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    CONTENEUR ctr = null;
                    if (ctrForm != null)
                    {
                        ctr = vsomAcc.SortirConteneur(Convert.ToInt32(ctrForm.txtIdCtr.Text), txtDateSortie.SelectedDate.Value, dataGridSinistres.SelectedItems.OfType<TYPE_SINISTRE>().ToList<TYPE_SINISTRE>(), new TextRange(txtObservations.Document.ContentStart, txtObservations.Document.ContentEnd).Text, utilisateur.IdU);
                        //Raffraîchir les informations

                        formLoader.LoadConteneurForm(ctrForm, ctr);
                    }
                    else if (ctrTCForm != null)
                    {
                        ctr = vsomAcc.SortirConteneur(Convert.ToInt32(ctrTCForm.txtIdCtr.Text), txtDateSortie.SelectedDate.Value, dataGridSinistres.SelectedItems.OfType<TYPE_SINISTRE>().ToList<TYPE_SINISTRE>(), new TextRange(txtObservations.Document.ContentStart, txtObservations.Document.ContentEnd).Text, utilisateur.IdU);
                        //Raffraîchir les informations

                        formLoader.LoadConteneurTCForm(ctrTCForm, ctr.CONTENEUR_TC.FirstOrDefault<CONTENEUR_TC>());
                    }
                    else if (ctrPanel != null)
                    {
                        ctr = vsomAcc.SortirConteneur(((CONTENEUR)ctrPanel.dataGrid.SelectedItem).IdCtr, txtDateSortie.SelectedDate.Value, dataGridSinistres.SelectedItems.OfType<TYPE_SINISTRE>().ToList<TYPE_SINISTRE>(), new TextRange(txtObservations.Document.ContentStart, txtObservations.Document.ContentEnd).Text, utilisateur.IdU);
                        //Raffraîchir les informations
                        //ctrPanel.dataGrid.ItemsSource = vsomAcc.GetConteneursImport();
                    }
                    else if (ctrTCPanel != null)
                    {
                        ctr = vsomAcc.SortirConteneur(((CONTENEUR_TC)ctrTCPanel.dataGrid.SelectedItem).IdCtr.Value, txtDateSortie.SelectedDate.Value, dataGridSinistres.SelectedItems.OfType<TYPE_SINISTRE>().ToList<TYPE_SINISTRE>(), new TextRange(txtObservations.Document.ContentStart, txtObservations.Document.ContentEnd).Text, utilisateur.IdU);
                        //Raffraîchir les informations
                        //ctrPanel.dataGrid.ItemsSource = vsomAcc.GetConteneursImport();
                    }

                    MessageBox.Show("L'opération de sortie de conteneur s'est déroulée avec succès, consultez le journal des éléments de facturation", "Conteneur sorti !", MessageBoxButton.OK, MessageBoxImage.Information);
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
