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
    /// Logique d'interaction pour AttelerVehiculeForm.xaml
    /// </summary>
    public partial class AttelerVehiculeForm : Window
    {

        public List<VEHICULE> vehiculesAP { get; set; }
        public List<string> vehsAP { get; set; }

        private VehiculeForm vehForm;
        private VehiculePanel vehPanel;
        private VEHICULE vehicule;
        private List<OPERATION> operationsUser;
        private UTILISATEUR utilisateur;

        private FormLoader formLoader;
       // private VsomParameters vsp = new VsomParameters();
        private VSOMAccessors vsomAcc;
        public AttelerVehiculeForm(VehiculeForm form, UTILISATEUR user)
        {
            try
            {
                 vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

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

        public AttelerVehiculeForm(VehiculePanel panel, VEHICULE veh, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

                vehicule = veh;

                utilisateur = user;
                operationsUser = vsomAcc.GetOperationsUtilisateur(utilisateur.IdU);

                vehPanel = panel;

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

        private void btnAttelerPorter_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //VSOMAccessors vsomAcc = new VSOMAccessors();
               // VsomMarchal vsomAcc = new VsomMarchal();

                VEHICULE veh = null;
                if ((chkAttelle.IsChecked == false && chkPorte.IsChecked == false))
                {
                    MessageBox.Show("Veuillez sélectionner au moins une option", "Attelé ou porté ?", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
                else if ((chkAttelle.IsChecked == true || chkPorte.IsChecked == true))
                {
                    veh = vsomAcc.SetVehiculeAP(vehicule.IdVeh, (chkPorte.IsChecked == true ? "Y" : "N"), (chkAttelle.IsChecked == true ? "Y" : "N"), vehiculesAP.ElementAt<VEHICULE>(cbNumChassisAP.SelectedIndex).IdVeh, new TextRange(txtObservations.Document.ContentStart, txtObservations.Document.ContentEnd).Text, utilisateur.IdU);
                }
                //Raffraîchir les informations
                formLoader.LoadVehiculeForm(vehForm, veh);

                MessageBox.Show("L'opération de mise jour d'informations sur AP s'est déroulée avec succès", "Véhicule attellé/porté !", MessageBoxButton.OK, MessageBoxImage.Information);
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

            }
        }

        private void chkAttelle_Click(object sender, RoutedEventArgs e)
        {
            try
            {
               // VSOMAccessors vsomAcc = new VSOMAccessors();

                if (chkAttelle.IsChecked == true)
                {
                    cbNumChassisAP.IsEnabled = true;
                    if (vehForm != null)
                    {
                        vehicule = vsomAcc.GetVehiculeByIdVeh(Convert.ToInt32(vehForm.txtIdChassis.Text));
                    }
                    vehiculesAP = vsomAcc.GetVehiculesOfConnaissementNonThis(vehicule.IdBL.Value, vehicule.IdVeh);
                    if (vehiculesAP.Count != 0)
                    {
                        vehsAP = new List<string>();
                        foreach (VEHICULE veh in vehiculesAP)
                        {
                            vehsAP.Add(veh.NumChassis);
                        }
                        cbNumChassisAP.ItemsSource = vehsAP;
                    }
                    else
                    {
                        MessageBox.Show("Le connaissement de base ne contient que ce véhicule", "Pas d'autres véhicules dans le BL", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        chkAttelle.IsChecked = false;
                        cbNumChassisAP.IsEnabled = false;
                    }
                }
                else if(chkPorte.IsChecked == false)
                {
                    cbNumChassisAP.IsEnabled = false;
                    cbNumChassisAP.ItemsSource = null;
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

        private void chkPorte_Click(object sender, RoutedEventArgs e)
        {
            try
            {
               // VSOMAccessors vsomAcc = new VSOMAccessors();

                if (chkPorte.IsChecked == true)
                {
                    cbNumChassisAP.IsEnabled = true;
                    if (vehForm != null)
                    {
                        vehicule = vsomAcc.GetVehiculeByIdVeh(Convert.ToInt32(vehForm.txtIdChassis.Text));
                    }
                    vehiculesAP = vsomAcc.GetVehiculesOfConnaissementNonThis(vehicule.IdBL.Value, vehicule.IdVeh);
                    if (vehiculesAP.Count != 0)
                    {
                        vehsAP = new List<string>();
                        foreach (VEHICULE veh in vehiculesAP)
                        {
                            vehsAP.Add(veh.NumChassis);
                        }
                        cbNumChassisAP.ItemsSource = vehsAP;
                    }
                    else
                    {
                        MessageBox.Show("Le connaissement de base ne contient que ce véhicule", "Pas d'autres véhicules dans le BL", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        chkPorte.IsChecked = false;
                        cbNumChassisAP.IsEnabled = false;
                    }
                }
                else if(chkAttelle.IsChecked == false)
                {
                    cbNumChassisAP.IsEnabled = false;
                    cbNumChassisAP.ItemsSource = null;
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
    }
}
