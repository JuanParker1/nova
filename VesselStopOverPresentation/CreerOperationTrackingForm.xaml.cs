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
    /// Logique d'interaction pour CreerOperationTrackingForm.xaml
    /// </summary>
    public partial class CreerOperationTrackingForm : Window
    {
        private ConteneurTCForm ctrTCForm;

        private List<TYPE_OPERATION> typesOperations;
        public List<string> ops { get; set; }

        private List<UTILISATEUR> utilisateurs;
        public List<string> users { get; set; }

        public List<PARC> parcs { get; set; }
        public List<string> prcs { get; set; }

        private List<EMPLACEMENT> emplacements;
        public List<string> empls { get; set; }

        private UTILISATEUR utilisateur;
        private List<OPERATION> operationsUser;

        private FormLoader formLoader;
        private VSOMAccessors vsomAcc;
        public CreerOperationTrackingForm(ConteneurTCForm form, int idCtr, UTILISATEUR user)
        {
            try
            {
                 vsomAcc = new VSOMAccessors();
                //VsomParameters vsp = new VsomParameters();
                InitializeComponent();
                this.DataContext = this;

                typesOperations = vsomAcc.GetTypeOperationsCtr();
                ops = new List<string>();
                foreach (TYPE_OPERATION t in typesOperations)
                {
                    ops.Add(t.LibTypeOp);
                }

                utilisateurs = vsomAcc.GetUtilisateurs();
                users = new List<string>();
                foreach (UTILISATEUR usr in utilisateurs)
                {
                    users.Add(usr.NU);
                }

                parcs = vsomAcc.GetParcs("C");
                prcs = new List<string>();
                foreach (PARC p in parcs)
                {
                    prcs.Add(p.NomParc);
                }

                ctrTCForm = form;

                utilisateur = user;
                operationsUser = vsomAcc.GetOperationsUtilisateur(utilisateur.IdU);

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

        private void btnEnregistrer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //VSOMAccessors vsomAcc = new VSOMAccessors();
                //VsomMarchal vsomAcc = new VsomMarchal();

                if (cbOperation.SelectedIndex == -1)
                {
                    MessageBox.Show("Vous devez saisir sélectionner un mouvement", "Mouvement ?", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (!txtDate.SelectedDate.HasValue)
                {
                    MessageBox.Show("Vous devez saisir la date de l'operation", "Date ?", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (cbUtilisateur.SelectedIndex == -1)
                {
                    MessageBox.Show("Vous devez sélectionner l'utilisateur qui a effectué l'operation", "Utilisateur ?", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (cbParc.SelectedIndex == -1)
                {
                    MessageBox.Show("Vous devez sélectionner le parc dans lequel a été effectué l'operation", "Parc ?", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (cbStatut.SelectedIndex == -1)
                {
                    MessageBox.Show("Vous devez sélectionner un statut", "Statut ?", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {

                    CONTENEUR_TC ctrTC = vsomAcc.InsertOperationTracking(typesOperations.ElementAt<TYPE_OPERATION>(cbOperation.SelectedIndex).IdTypeOp, Convert.ToInt32(ctrTCForm.txtIdCtr.Text), txtDate.SelectedDate.Value, utilisateurs.ElementAt<UTILISATEUR>(cbUtilisateur.SelectedIndex).IdU, parcs.ElementAt<PARC>(cbParc.SelectedIndex).IdParc, cbStatut.Text);

                    formLoader.LoadConteneurTCForm(ctrTCForm, ctrTC);

                    MessageBox.Show("Operation créée avec succès", "Operation créée !", MessageBoxButton.OK, MessageBoxImage.Information);

                    this.Close();
                }
            }
            catch (EnregistrementInexistant ei)
            {
                MessageBox.Show(ei.Message, "Enregistrement inexistant", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void cbParc_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                //VsomParameters vsp = new VsomParameters();

                cbEmplacement.ItemsSource = null;
                emplacements = vsomAcc.GetEmplacementConteneurByIdParc(parcs.ElementAt<PARC>(cbParc.SelectedIndex).IdParc, "Disponible");

                empls = new List<string>();
                foreach (EMPLACEMENT em in emplacements)
                {
                    empls.Add(em.LigEmpl + em.ColEmpl);
                }
                cbEmplacement.ItemsSource = empls;
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
