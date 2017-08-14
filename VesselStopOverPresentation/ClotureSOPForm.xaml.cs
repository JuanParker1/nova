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
    /// Logique d'interaction pour ClotureSOPForm.xaml
    /// </summary>
    public partial class ClotureSOPForm : Window
    {

        private SummaryOPForm sopForm;
        private List<OPERATION> operationsUser;
        private UTILISATEUR utilisateur;

        private FormLoader formLoader;
        //private VsomParameters vsp = new VsomParameters();
        private VSOMAccessors vsomAcc;

        public ClotureSOPForm(SummaryOPForm form, UTILISATEUR user)
        {
            try
            {
                 vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

                utilisateur = user;
                operationsUser = vsomAcc.GetOperationsUtilisateur(utilisateur.IdU);

                sopForm = form;

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
                 vsomAcc = new VSOMAccessors();

                ESCALE esc = vsomAcc.GetEscaleById(Convert.ToInt32(sopForm.txtEscaleSysID.Text));

                if (!txtSailingDate.SelectedDate.HasValue)
                {
                    MessageBox.Show("Veuillez entrer la date de clôture du navire", "Date clôture navire ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    if (MessageBox.Show("Il ne sera plus possible d'apporter des modifications sur les informations du summary of operation, voulez-vous vraiment lancer cette clôture maintenant ?", "Clôture du summary of operation !", MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
                    {
                        esc = vsomAcc.CloturerSummaryOperations(esc.IdEsc, txtSailingDate.SelectedDate.Value, utilisateur.IdU);

                        formLoader.LoadEscaleForm(sopForm, esc);

                        formLoader.LoadEscaleForm(sopForm.escForm, esc);

                        MessageBox.Show("Le summary of operations a été clôturé avec succès et les éléments de facturation y afférant générés", "Summary of operations clôturé !", MessageBoxButton.OK, MessageBoxImage.Information);
                        this.Close();
                    }
                }
            }
            catch (FacturationException gpex)
            {
                MessageBox.Show(gpex.Message, "Rôle gestionnaire de parc non défini !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            catch (IdentificationException vehex)
            {
                MessageBox.Show(vehex.Message, "Conteneur non identifié !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
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
