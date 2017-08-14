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
    /// Logique d'interaction pour IdentificationMafiForm.xaml
    /// </summary>
    public partial class IdentificationMafiForm : Window
    {
        private MafiForm mafiForm;
        private MafiPanel mafiPanel;
        private MAFI mafi;
        private List<OPERATION> operationsUser;
        private UTILISATEUR utilisateur;

        private FormLoader formLoader;
        private VsomParameters vsp = new VsomParameters();
        public IdentificationMafiForm(MafiForm form, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

                utilisateur = user;
                operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);

                mafiForm = form;

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

        public IdentificationMafiForm(MafiPanel panel, MAFI mf, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

                mafi = mf;

                utilisateur = user;
                operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);

                mafiPanel = panel;

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
                MAFI mf = null;
                if (mafiForm != null)
                {
                    mf = vsomAcc.IdentifierMafi(Convert.ToInt32(mafiForm.txtIdMafi.Text), new TextRange(txtObservations.Document.ContentStart, txtObservations.Document.ContentEnd).Text, utilisateur.IdU);
                    //Raffraîchir les informations

                    formLoader.LoadMafiForm(mafiForm, mf);
                }
                else if (mafiPanel != null)
                {
                    mf = vsomAcc.IdentifierMafi(((MAFI)mafiPanel.dataGrid.SelectedItem).IdMafi, new TextRange(txtObservations.Document.ContentStart, txtObservations.Document.ContentEnd).Text, utilisateur.IdU);
                    //Raffraîchir les informations
                    //ctrPanel.dataGrid.ItemsSource = vsomAcc.GetConteneursImport();
                }

                MessageBox.Show("L'opération d'identification s'est déroulée avec succès, consultez le journal des éléments de facturation", "Mafi identifié !", MessageBoxButton.OK, MessageBoxImage.Information);
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
