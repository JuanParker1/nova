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
    /// Logique d'interaction pour VAEForm.xaml
    /// </summary>
    public partial class VAEForm : Window
    {
        private VehiculeForm vehForm;
        private VEHICULE vehicule;
        private List<OPERATION> operationsUser;
        private UTILISATEUR utilisateur;

        public List<CONNAISSEMENT> connaissements { get; set; }
        public List<string> cons { get; set; }

        private List<CLIENT> clients;
        public List<string> clts { get; set; }

        public List<ESCALE> escales { get; set; }
        public List<Int32> escs { get; set; }

        public List<MANIFESTE> manifestes { get; set; }
        public List<Int32> manifs { get; set; }

        private FormLoader formLoader;
        //private VsomParameters vsp = new VsomParameters();
        private VSOMAccessors vsomAcc;
        public VAEForm(VehiculeForm form, VEHICULE veh, UTILISATEUR user)
        {
            try
            {
                 vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

                vehicule = veh;

                InitializeCombos();

                utilisateur = user;
                operationsUser = vsomAcc.GetOperationsUtilisateur(utilisateur.IdU);

                vehForm = form;

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

        private void InitializeCombos()
        {
            try
            {
                //VSOMAccessors vsomAcc = new VSOMAccessors();
                //VsomParameters vsp = new VsomParameters();
                clients = vsomAcc.GetClientsActifs();
                clts = new List<string>();
                foreach (CLIENT cl in clients)
                {
                    clts.Add(cl.NomClient);
                }
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }

        }

        private void cbClient_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                txtCodeClient.Text = clients.ElementAt<CLIENT>(cbClient.SelectedIndex).CodeClient;
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }

        }

        private void cbEscale_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (cbEscale.Items.Count != 0)
                {
                    txtNumVoyage.Text = escales.ElementAt<ESCALE>(cbEscale.SelectedIndex).NumVoySCR;
                }
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }

        }

        private void cbManifeste_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (cbManifeste.Items.Count != 0)
                {
                    txtCodePort.Text = manifestes.ElementAt<MANIFESTE>(cbManifeste.SelectedIndex).CodePort;
                }
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }

        }

        private void btnVAE_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                if (txtDateVAE.SelectedDate == null)
                {
                    MessageBox.Show("Vous n'avez pas saisi la date de départ du navire.", "Date de départ du navire ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    VEHICULE veh = vsomAcc.PasserEnVAE(vehicule.IdVeh, clients.ElementAt<CLIENT>(cbClient.SelectedIndex).IdClient, txtConsignee.Text, txtAdresse.Text, txtNotify.Text, txtEmail.Text, txtDateVAE.SelectedDate.Value, utilisateur.IdU);
                    formLoader.LoadVehiculeForm(vehForm, veh);
                }

                MessageBox.Show("L'opération de mise en VAE s'est déroulée avec succès", "Véhicule passé en VAE !", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (HabilitationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération", MessageBoxButton.OK, MessageBoxImage.Error);
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
    }
}
