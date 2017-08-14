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
    /// Logique d'interaction pour SortirConventionnelForm.xaml
    /// </summary>
    public partial class SortirConventionnelForm : Window
    {

        private ConventionnelForm convForm;

        private ConventionnelPanel convPanel;

        private CONVENTIONNEL conventionnel;

        private List<OPERATION> operationsUser;
        private UTILISATEUR utilisateur;

        private FormLoader formLoader;
        private VsomParameters vsp = new VsomParameters();
        public SortirConventionnelForm(ConventionnelForm form, UTILISATEUR user)
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

        public SortirConventionnelForm(ConventionnelPanel panel, CONVENTIONNEL conv, UTILISATEUR user)
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
                else if (convForm != null)
                {
                    CONVENTIONNEL conv = vsomAcc.SortirConventionnel(Convert.ToInt32(convForm.txtIdGC.Text), txtDateSortie.SelectedDate.Value, new TextRange(txtObservations.Document.ContentStart, txtObservations.Document.ContentEnd).Text, utilisateur.IdU);
                    //Raffraîchir les informations

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
                    MessageBox.Show("L'opération de sortie s'est déroulée avec succès", "General cargo sorti !", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();
                }
                else if (convPanel != null)
                {
                    CONVENTIONNEL conv = vsomAcc.SortirConventionnel(conventionnel.IdGC, txtDateSortie.SelectedDate.Value, new TextRange(txtObservations.Document.ContentStart, txtObservations.Document.ContentEnd).Text, utilisateur.IdU);
                    if (convPanel.cbFiltres.SelectedIndex != 0)
                    {
                        convPanel.cbFiltres.SelectedIndex = 0;
                    }
                    else
                    {
                        convPanel.conventionnels = vsp.GetConventionnelsImport();
                        convPanel.dataGrid.ItemsSource = convPanel.conventionnels;
                    }
                    MessageBox.Show("L'opération de sortie s'est déroulée avec succès", "General cargo sorti !", MessageBoxButton.OK, MessageBoxImage.Information);
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

            }
        }
        
    }
}
