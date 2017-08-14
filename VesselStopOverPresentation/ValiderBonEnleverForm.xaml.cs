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
    /// Logique d'interaction pour ValiderBonEnleverForm.xaml
    /// </summary>
    public partial class ValiderBonEnleverForm : Window
    {

        private BonEnleverForm baeForm;
        private List<OPERATION> operationsUser;
        private UTILISATEUR utilisateur;
        //private VsomParameters vsp = new VsomParameters();
        private VSOMAccessors vsomAcc;
        public ValiderBonEnleverForm(BonEnleverForm form, UTILISATEUR user)
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

                    baeForm = form;
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
               // using (var ctx = new VSOMClassesDataContext())
               // {
                 vsomAcc = new VSOMAccessors();
                //VsomMarchal vsomAcc = new VsomMarchal();

                 if (baeForm != null)
                 {
                     BON_ENLEVEMENT bae = vsomAcc.ValiderBonEnlevement(Convert.ToInt32(baeForm.cbIdBAE.Text), new TextRange(txtObservations.Document.ContentStart, txtObservations.Document.ContentEnd).Text, utilisateur.IdU);
                     List<BON_ENLEVEMENT> baeValides = vsomAcc.GetBonsEnleverValides();

                     baeForm.baePanel.bonsEnlever = baeValides;
                     baeForm.baePanel.cbFiltres.SelectedIndex = 1;
                     baeForm.baePanel.dataGrid.ItemsSource = baeForm.baePanel.bonsEnlever;
                     baeForm.baePanel.lblStatut.Content = baeForm.baePanel.bonsEnlever.Count + " Bon(s) à enlever";
                     baeForm.baePanel.bonsEnlever = baeValides;

                     baeForm.bonsEnlever = new List<Int32>();
                     baeForm.bonsEnlevements = new List<BON_ENLEVEMENT>();
                     baeForm.bonsEnlevements.Add(bae);
                     foreach (BON_ENLEVEMENT b in baeForm.bonsEnlevements)
                     {
                         baeForm.bonsEnlever.Add(b.IdBAE);
                     }
                     baeForm.cbIdBAE.ItemsSource = null;
                     baeForm.cbIdBAE.ItemsSource = baeForm.bonsEnlever;
                     baeForm.cbIdBAE.SelectedItem = bae.IdBAE;

                     baeForm.btnEnregistrer.IsEnabled = false;
                     baeForm.borderEtat.Visibility = System.Windows.Visibility.Visible;
                     baeForm.btnValiderBAE.Visibility = System.Windows.Visibility.Collapsed;
                 }
                //}

                MessageBox.Show("Bon à enlever validé avec succès.", "BAE validé !", MessageBoxButton.OK, MessageBoxImage.Information);

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
