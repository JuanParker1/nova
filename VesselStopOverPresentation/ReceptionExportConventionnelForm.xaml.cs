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
    /// Logique d'interaction pour ReceptionExportConventionnelForm.xaml
    /// </summary>
    public partial class ReceptionExportConventionnelForm : Window
    {
        private ConventionnelExportPanel convExportPanel;
        private CONVENTIONNEL conventionnel;
        private List<OPERATION> operationsUser;
        private UTILISATEUR utilisateur;

        private FormLoader formLoader;
        private VsomParameters vsp = new VsomParameters();
        public ReceptionExportConventionnelForm(ConventionnelExportPanel panel, CONVENTIONNEL conv, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

                conventionnel = conv;

                utilisateur = user;
                operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);

                convExportPanel = panel;

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
                //VSOMAccessors vsomAcc = new VSOMAccessors();
                VsomMarchal vsomAcc = new VsomMarchal();

                if (txtDesc.Text.Trim() == "")
                {
                    MessageBox.Show("Veuillez indiquer la description de la marchandise", "Description ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (txtVol.Text.Trim() == "")
                {
                    MessageBox.Show("Veuillez entrer le volume reçu de general cargo", "Volume ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (txtPoids.Text.Trim() == "")
                {
                    MessageBox.Show("Veuillez entrer le poids reçu de general cargo", "Poids ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (txtQte.Text.Trim() == "")
                {
                    MessageBox.Show("Veuillez entrer la quantité reçue de general cargo", "Quantité ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    txtVol.Text = txtVol.Text.Replace(" ", "").Replace(".", ",");
                    txtQte.Text = txtQte.Text.Replace(" ", "").Replace(".", ",");
                    txtPoids.Text = txtPoids.Text.Replace(" ", "").Replace(".", ",");

                    CONVENTIONNEL conv = vsomAcc.ReceptionExportConventionnel(((CONVENTIONNEL)convExportPanel.dataGrid.SelectedItem).IdGC, txtDesc.Text, Convert.ToDouble(txtVol.Text), Convert.ToDouble(txtPoids.Text), Convert.ToInt32(txtQte.Text), new TextRange(txtObservations.Document.ContentStart, txtObservations.Document.ContentEnd).Text, utilisateur.IdU);
                    //Raffraîchir les informations
                    //convExportPanel.dataGrid.ItemsSource = vsomAcc.GetConventionnelsExport();

                    MessageBox.Show("L'opération de réception du conventionnel s'est déroulée avec succès", "Conventionnel réceptionné !", MessageBoxButton.OK, MessageBoxImage.Information);
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

        private void txtDim_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9,.]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
