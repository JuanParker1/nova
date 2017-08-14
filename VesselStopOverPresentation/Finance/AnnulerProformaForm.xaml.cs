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
    /// Logique d'interaction pour AnnulerProformaForm.xaml
    /// </summary>
    public partial class AnnulerProformaForm : Window
    {

        private ProformaForm profForm;
        private List<OPERATION> operationsUser;
        private UTILISATEUR utilisateur;

        private FormLoader formLoader;
        //private VsomParameters vsp = new VsomParameters();
        private VSOMAccessors vsomAcc;
        public AnnulerProformaForm(ProformaForm form, UTILISATEUR user)
        {
            try
            {
                 vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

                utilisateur = user;
                operationsUser = vsomAcc.GetOperationsUtilisateur(utilisateur.IdU);

                profForm = form;

                //txtRefAS400.Text = form.txtRefAS400.Text;

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

        private void btnAnnuler_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                 vsomAcc = new VSOMAccessors();
                int idProf = Convert.ToInt32(profForm.cbIdProf.Text);
                PROFORMA prof = vsomAcc.AnnulerProforma(idProf, new TextRange(txtObservations.Document.ContentStart, txtObservations.Document.ContentEnd).Text, utilisateur.IdU);
                List<PROFORMA> profAnnulees = vsomAcc.GetProformasAnnulees();

                if (profForm.profPanel != null)
                {
                    if (profForm.profPanel.cbFiltres.SelectedIndex != 1)
                    {
                        profForm.profPanel.cbFiltres.SelectedIndex = 1;
                    }
                    else
                    {
                        profForm.profPanel.proformas = profAnnulees;
                        profForm.profPanel.dataGrid.ItemsSource = profForm.profPanel.proformas;
                        profForm.profPanel.lblStatut.Content = profForm.profPanel.proformas.Count + " Proforma(s)";
                    }
                }

                formLoader.LoadProformaForm(profForm, prof);

                profForm.cbIdProf.IsEnabled = true;

                MessageBox.Show("Proforma annulée avec succès.", "Proforma annulée !", MessageBoxButton.OK, MessageBoxImage.Information);

                this.Close();
            }
            catch (HabilitationException ex)
            {
                MessageBox.Show(ex.Message, "Droits insuffisants !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
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
