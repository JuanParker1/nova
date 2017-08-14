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
    /// Logique d'interaction pour ValiderConnaissementForm.xaml
    /// </summary>
    public partial class ValiderConnaissementForm : Window
    {

        private ConnaissementForm connForm;
        private List<OPERATION> operationsUser;
        private UTILISATEUR utilisateur;

        private FormLoader formLoader;
        //private VsomParameters vsp = new VsomParameters();
        private VSOMAccessors vsomAcc;
        public ValiderConnaissementForm(ConnaissementForm form, UTILISATEUR user)
        {
            try
            {

                InitializeComponent();
                //using (var ctx = new VSOMClassesDataContext())
                //{
                    vsomAcc = new VSOMAccessors();

                    this.DataContext = this;

                    utilisateur = user;
                    operationsUser = vsomAcc.GetOperationsUtilisateur(utilisateur.IdU);

                    connForm = form;

                    formLoader = new FormLoader(utilisateur);
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

        private void btnValider_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //using (var ctx = new VSOMClassesDataContext())
                //{
                    vsomAcc = new VSOMAccessors();

                    CONNAISSEMENT con = vsomAcc.ValiderConnaissement(Convert.ToInt32(connForm.txtIdBL.Text), new TextRange(txtObservations.Document.ContentStart, txtObservations.Document.ContentEnd).Text, utilisateur.IdU);

                    //Raffraîchir les informations
                    if (connForm != null)
                    {
                        formLoader.LoadConnaissementForm(connForm, con);
                    }
                    else if (connForm.conPanel != null)
                    {
                        if (connForm.conPanel.cbFiltres.SelectedIndex != 1)
                        {
                            connForm.conPanel.cbFiltres.SelectedIndex = 1;
                        }
                        else
                        {
                            connForm.conPanel.connaissements = vsomAcc.GetConnaissementByStatut("Manifesté");
                            connForm.conPanel.dataGrid.ItemsSource = connForm.conPanel.connaissements;
                            connForm.conPanel.lblStatut.Content = connForm.conPanel.connaissements.Count + " Connaissement(s) manifesté(s)";
                        }
                    }
                //}
                MessageBox.Show("Connaissement validé : Les éléments de facturation ont été générés avec succès", "Connaissement validé !", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            catch (HabilitationException ex)
            {
                MessageBox.Show(ex.Message, "Droits insuffisants !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            catch (ManifesteException ex)
            {
                MessageBox.Show(ex.Message, "Manifeste validé !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.StackTrace, "Echec de l'opération{ex} !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
