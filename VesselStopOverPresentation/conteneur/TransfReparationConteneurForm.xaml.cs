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
    /// Logique d'interaction pour TransfReparationConteneurForm.xaml
    /// </summary>
    public partial class TransfReparationConteneurForm : Window
    {
        public List<PARC> parcs { get; set; }
        public List<string> prcs { get; set; }

        private List<EMPLACEMENT> emplacements;
        public List<string> empls { get; set; }

        private ConteneurTCForm ctrTCForm;
        private CONTENEUR_TC conteneurTC;
        private ConteneurTCPanel ctrTCPanel;
        private List<OPERATION> operationsUser;
        private UTILISATEUR utilisateur;

        private FormLoader formLoader;
        //private VsomParameters vsp = new VsomParameters();
        private VSOMAccessors vsomAcc;
        public TransfReparationConteneurForm(ConteneurTCForm form, UTILISATEUR user)
        {
            try
            {
                //VSOMAccessors vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

                parcs = vsomAcc.GetParcs("C");
                prcs = new List<string>();
                foreach (PARC p in parcs)
                {
                    prcs.Add(p.NomParc);
                }

                utilisateur = user;
                operationsUser = vsomAcc.GetOperationsUtilisateur(utilisateur.IdU);

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

        public TransfReparationConteneurForm(ConteneurTCPanel form, CONTENEUR_TC ctr, UTILISATEUR user)
        {
            try
            {
                //VSOMAccessors vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

                parcs = vsomAcc.GetParcs("C");
                prcs = new List<string>();
                foreach (PARC p in parcs)
                {
                    prcs.Add(p.NomParc);
                }

                utilisateur = user;
                operationsUser = vsomAcc.GetOperationsUtilisateur(utilisateur.IdU);

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

        private void btnReparation_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //VSOMAccessors vsomAcc = new VSOMAccessors();
                //VsomMarchal vsomAcc = new VsomMarchal();

                if (cbEmplacement.SelectedIndex == -1)
                {
                    MessageBox.Show("Veuillez sélectionner un emplacement de réparation où sera parqué ce conteneur", "Emplacement ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    CONTENEUR ctr = null;
                    if (ctrTCForm != null)
                    {
                        ctr = vsomAcc.TransfConteneurZoneReparation(Convert.ToInt32(ctrTCForm.txtIdCtr.Text), emplacements.ElementAt<EMPLACEMENT>(cbEmplacement.SelectedIndex).IdEmpl, (chkReparation.IsChecked == true ? "Y" : "N"), (chkNettoyage.IsChecked == true ? "Y" : "N"), (chkLavage.IsChecked == true ? "Y" : "N"), (chkIrrep.IsChecked == true ? "Y" : "N"), new TextRange(txtObservations.Document.ContentStart, txtObservations.Document.ContentEnd).Text, utilisateur.IdU);
                        //Raffraîchir les informations

                        formLoader.LoadConteneurTCForm(ctrTCForm, ctr.CONTENEUR_TC.FirstOrDefault<CONTENEUR_TC>());
                    }
                    else if (ctrTCPanel != null)
                    {
                        ctr = vsomAcc.TransfConteneurZoneReparation(((CONTENEUR_TC)ctrTCPanel.dataGrid.SelectedItem).IdCtr.Value, emplacements.ElementAt<EMPLACEMENT>(cbEmplacement.SelectedIndex).IdEmpl, (chkReparation.IsChecked == true ? "Y" : "N"), (chkNettoyage.IsChecked == true ? "Y" : "N"), (chkLavage.IsChecked == true ? "Y" : "N"), (chkIrrep.IsChecked == true ? "Y" : "N"), new TextRange(txtObservations.Document.ContentStart, txtObservations.Document.ContentEnd).Text, utilisateur.IdU);
                        //Raffraîchir les informations
                    }

                    MessageBox.Show("L'opération de transfert en zone de réparation de ce conteneur s'est déroulée avec succès, consultez le journal des éléments de facturation", "Conteneur en réparation !", MessageBoxButton.OK, MessageBoxImage.Information);

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

        private void cbParc_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                //VSOMAccessors vsomAcc = new VSOMAccessors();

                cbEmplacement.ItemsSource = null;
                emplacements = vsomAcc.GetEmplacementConteneurByIdParc(parcs.ElementAt<PARC>(cbParc.SelectedIndex).IdParc, "Reparation");

                empls = new List<string>();
                foreach (EMPLACEMENT em in emplacements)
                {
                    empls.Add(em.LigEmpl + em.ColEmpl);
                }
                cbEmplacement.ItemsSource = empls;
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
