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
    /// Logique d'interaction pour IdentificationConventionnelForm.xaml
    /// </summary>
    public partial class IdentificationConventionnelForm : Window
    {
        private ConventionnelForm convForm;
        private ConventionnelPanel convPanel;
        private CONVENTIONNEL conventionnel;
        private List<OPERATION> operationsUser;
        private UTILISATEUR utilisateur;

        private FormLoader formLoader;
        private VsomParameters vsp = new VsomParameters();
        public IdentificationConventionnelForm(ConventionnelForm form, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

                utilisateur = user;
                operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);

                convForm = form;

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

        public IdentificationConventionnelForm(ConventionnelPanel panel, CONVENTIONNEL conv, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

                conventionnel = conv;

                utilisateur = user;
                operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);

                convPanel = panel;

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
                CONVENTIONNEL conv = null;
                if (convForm != null)
                {
                    conv = vsomAcc.IdentifierConventionnel(Convert.ToInt32(convForm.txtIdGC.Text), new TextRange(txtObservations.Document.ContentStart, txtObservations.Document.ContentEnd).Text, utilisateur.IdU);

                    //Raffraîchir les informations
                    formLoader.LoadConventionnelForm(convForm, conv);

                    if (convForm.convPanel != null)
                    {
                        //if (convForm.convPanel.cbFiltres.SelectedIndex != 0)
                        //{
                        //    convForm.convPanel.cbFiltres.SelectedIndex = 0;
                        //}
                        //else
                        //{
                        //    convForm.convPanel.conventionnels = vsomAcc.GetConventionnelsImport();
                        //    convForm.convPanel.dataGrid.ItemsSource = convForm.convPanel.conventionnels;
                        //}
                    }
                }
                else if (convPanel != null)
                {
                    conv = vsomAcc.IdentifierConventionnel(conventionnel.IdGC, new TextRange(txtObservations.Document.ContentStart, txtObservations.Document.ContentEnd).Text, utilisateur.IdU);

                    //Raffraîchir les informations
                    //if (convPanel.cbFiltres.SelectedIndex != 0)
                    //{
                    //    convPanel.cbFiltres.SelectedIndex = 0;
                    //}
                    //else
                    //{
                    //    convPanel.conventionnels = vsomAcc.GetConventionnelsImport();
                    //    convPanel.dataGrid.ItemsSource = convPanel.conventionnels;
                    //}
                }

                MessageBox.Show("L'opération d'identification s'est déroulée avec succès, consultez le journal des éléments de facturation", "General cargo identifié !", MessageBoxButton.OK, MessageBoxImage.Information);
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

            }
        }
    }
}
