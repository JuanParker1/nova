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
    /// Logique d'interaction pour FacturerArmateurForm.xaml
    /// </summary>
    public partial class FacturerArmateurForm : Window
    {

        public List<ElementFacturation> elementFactureArmateur { get; set; }

        private EscaleForm escForm;
        private List<OPERATION> operationsUser;
        private UTILISATEUR utilisateur;

        private FormLoader formLoader;
        private VsomParameters vsp = new VsomParameters();
        public FacturerArmateurForm(EscaleForm form, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

                elementFactureArmateur = vsp.GetElementFacturationEsc(Convert.ToInt32(form.txtEscaleSysID.Text)).Where(el => el.Statut != "Facturé").ToList();

                utilisateur = user;
                operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);

                escForm = form;

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

                if (dataGridEltsFact.SelectedItems.OfType<ElementFacturation>().ToList<ElementFacturation>().Count == 0)
                {
                    MessageBox.Show("Vous devez renseigner au moins une ligne à facturer à l'armateur", "Eléments de facturation ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (MessageBox.Show("Cette opération entraine une comptabilisation définitive de la facture armateur, voulez-vous vraiment lancer cette clôture maintenant ?", "Facturation de l'armateur !", MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
                {
                    ESCALE esc = vsomAcc.FacturerArmateur(Convert.ToInt32(escForm.txtEscaleSysID.Text), dataGridEltsFact.SelectedItems.OfType<ElementFacturation>().ToList<ElementFacturation>(), "", utilisateur.IdU);

                    formLoader.LoadEscaleForm(escForm, esc);

                    MessageBox.Show("La facture armateur a été comptabilisée avec succès", "Facture armateur comptabilisée !", MessageBoxButton.OK, MessageBoxImage.Information);

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
                //MessageBox.Show(ex.Message + "\n" + ex.StackTrace, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
