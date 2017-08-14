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
    /// Logique d'interaction pour ProgVisiteDouaneCtrForm.xaml
    /// </summary>
    public partial class ProgVisiteDouaneCtrForm : Window
    {

        private ConteneurForm ctrForm;
        private List<OPERATION> operationsUser;
        private UTILISATEUR utilisateur;

        private FormLoader formLoader;
        private VsomParameters vsp = new VsomParameters();
        public ProgVisiteDouaneCtrForm(ConteneurForm form, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

                utilisateur = user;
                operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);

                ctrForm = form;

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

        private void btnFacturer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                if (!txtDateVisite.SelectedDate.HasValue)
                {
                    MessageBox.Show("Veuillez entrer une date prévue pour la visite de la douane", "Date prévue pour la visite ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (txtDateVisite.SelectedDate.Value < vsp.GetConteneurImportByIdCtr(Convert.ToInt32(ctrForm.txtIdCtr.Text)).ESCALE.DRAEsc)
                {
                    MessageBox.Show("La date prévue pour la visite de la douane pour ce conteneur ne peut pas être inférieure ou égale à la date réelle d'arrivée du navire", "Date prévue pour la visite  incohérente", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (txtDateVisite.SelectedDate.Value < DateTime.Now.Date)
                {
                    if (MessageBox.Show("La date saisie pour la visite de la douane sur ce conteneur est inférieure à la date du jour. Voulez-vous continuer ?", "Date prévue pour la visite  incohérente", MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
                    {
                        CONTENEUR ctr = vsomAcc.ProgrammerVisiteDouane(Convert.ToInt32(ctrForm.txtIdCtr.Text), txtDateVisite.SelectedDate.Value, utilisateur.IdU);

                        formLoader.LoadConteneurForm(ctrForm, ctr);

                        MessageBox.Show("L'opération de programmation de visite douane s'est déroulée avec succès, consultez le journal des éléments de facturation", "Facturation du stationnement réussie !", MessageBoxButton.OK, MessageBoxImage.Information);

                        this.Close();
                    }
                }
                else
                {
                    CONTENEUR ctr = vsomAcc.ProgrammerVisiteDouane(Convert.ToInt32(ctrForm.txtIdCtr.Text), txtDateVisite.SelectedDate.Value, utilisateur.IdU);

                    formLoader.LoadConteneurForm(ctrForm, ctr);

                    MessageBox.Show("L'opération de programmation de visite douane s'est déroulée avec succès, consultez le journal des éléments de facturation", "Facturation du stationnement réussie !", MessageBoxButton.OK, MessageBoxImage.Information);

                    this.Close();
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

            }
        }
    }
}
