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
    /// Logique d'interaction pour ReceptionConventionnelForm.xaml
    /// </summary>
    public partial class ReceptionConventionnelForm : Window
    {

        private ConventionnelForm convForm;
        private ConventionnelPanel convPanel;

        private CONVENTIONNEL conventionnel;

        private List<OPERATION> operationsUser;
        private UTILISATEUR utilisateur;

        private FormLoader formLoader;
        private VsomParameters vsp = new VsomParameters();
        public ReceptionConventionnelForm(ConventionnelForm form, CONVENTIONNEL conv, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

                utilisateur = user;
                operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);

                conventionnel = conv;
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

        public ReceptionConventionnelForm(ConventionnelPanel panel, CONVENTIONNEL conv, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

                utilisateur = user;
                operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);

                conventionnel = conv;
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

        private void btnReceptionner_Click(object sender, RoutedEventArgs e)
        {
            try
            {
               // VSOMAccessors vsomAcc = new VSOMAccessors();
                VsomMarchal vsomAcc = new VsomMarchal();
                CONVENTIONNEL conv = vsomAcc.ReceptionnerConventionnel(conventionnel.IdGC, new TextRange(txtObservations.Document.ContentStart, txtObservations.Document.ContentEnd).Text, utilisateur.IdU);
                //Raffraîchir les informations
                if (convForm != null)
                {
                    formLoader.LoadConventionnelForm(convForm, conv);
                    if (convForm.convPanel != null)
                    {
                        if (convForm.convPanel.cbFiltres.SelectedIndex != 0)
                        {
                            convForm.convPanel.cbFiltres.SelectedIndex = 0;
                        }
                        else
                        {
                            convForm.convPanel.conventionnels = vsp.GetConventionnelsImport();
                            convForm.convPanel.dataGrid.ItemsSource = convForm.convPanel.conventionnels;
                        }
                    }
                }
                else if (convPanel != null)
                {
                    if (convPanel.cbFiltres.SelectedIndex != 0)
                    {
                        convPanel.cbFiltres.SelectedIndex = 0;
                    }
                    else
                    {
                        convPanel.conventionnels = vsp.GetConventionnelsImport();
                        convPanel.dataGrid.ItemsSource = convPanel.conventionnels;
                    }
                }

                MessageBox.Show("L'opération de réception au parc sous douane s'est déroulée avec succès", "General cargo réceptionné !", MessageBoxButton.OK, MessageBoxImage.Information);

            }
            catch (HabilitationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (EnregistrementInexistant ex)
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
