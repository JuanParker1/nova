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
    /// Logique d'interaction pour ValiderOrdreServiceForm.xaml
    /// </summary>
    public partial class ValiderOrdreServiceForm : Window
    {

        private OrdreServiceForm osForm;
        private List<OPERATION> operationsUser;
        private UTILISATEUR utilisateur;

        private FormLoader formLoader;
        //VsomParameters vsp = new VsomParameters();
        private VSOMAccessors vsomAcc;
        public ValiderOrdreServiceForm(OrdreServiceForm form, UTILISATEUR user)
        {
            try
            {
                 vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

                utilisateur = user;
                operationsUser = vsomAcc.GetOperationsUtilisateur(utilisateur.IdU);

                osForm = form;

                formLoader = new FormLoader(utilisateur);
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'operation de validation", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnValider_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                vsomAcc = new VSOMAccessors();

                ORDRE_SERVICE os = vsomAcc.ValiderOrdreService(Convert.ToInt32(osForm.cbIdOS.Text), osForm.txtDateExecReelle.SelectedDate.Value, osForm.txtDateExecPrevue.SelectedDate.Value, osForm.txtObjetOS.Text, new TextRange(osForm.txtObservations.Document.ContentStart, osForm.txtObservations.Document.ContentEnd).Text, utilisateur.IdU);

                //Raffraîchir les informations
                formLoader.LoadOrdreServiceForm(osForm, os);

                MessageBox.Show("Ordre de service validé : Les éléments de facturation ont été générés avec succès", "Ordre de service validé !", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
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
