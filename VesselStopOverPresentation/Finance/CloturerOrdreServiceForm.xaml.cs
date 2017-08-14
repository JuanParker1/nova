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
    /// Logique d'interaction pour CloturerOrdreServiceForm.xaml
    /// </summary>
    public partial class CloturerOrdreServiceForm : Window
    {

        private OrdreServiceForm osForm;
        private List<OPERATION> operationsUser;
        private UTILISATEUR utilisateur;

        private FormLoader formLoader;
        //private VsomParameters vsp = new VsomParameters();
        private VSOMAccessors vsomAcc;
        public CloturerOrdreServiceForm(OrdreServiceForm form, UTILISATEUR user)
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

            }
        }

        private void btnCloturer_Click(object sender, RoutedEventArgs e)
        {
           
             

            try
            {
                //throw new ApplicationException("Cette action est indisposnible pour le moment");

                vsomAcc = new VSOMAccessors();

                ORDRE_SERVICE os = vsomAcc.CloturerOrdreService(Convert.ToInt32(osForm.cbIdOS.Text), osForm.txtDateCloture.SelectedDate.Value, utilisateur.IdU);

                //Raffraîchir les informations
                formLoader.LoadOrdreServiceForm(osForm, os);

                MessageBox.Show("Ordre de service clôturé : Les réception et facture fournisseur ont été crées avec succès dans l'ERP", "Ordre de service cloturé !", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.StackTrace, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
