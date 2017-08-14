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
    /// Logique d'interaction pour IdentificationVehiculeForm.xaml
    /// </summary>
    public partial class IdentificationVehiculeForm : Window
    {

        public List<VEHICULE> vehiculesAP { get; set; }
        public List<string> vehsAP { get; set; }

        public List<LIEU> lieux { get; set; }
        public List<string> lieuxOp { get; set; }

        private VehiculeForm vehForm;
        private VehiculePanel vehPanel;
        private VEHICULE vehicule;
        private List<OPERATION> operationsUser;
        private UTILISATEUR utilisateur;

        private FormLoader formLoader;
        //private VsomParameters vsp = new VsomParameters();
        private VSOMAccessors vsomAcc;
        public IdentificationVehiculeForm(VehiculeForm form, UTILISATEUR user)
        {
            try
            {
                 vsomAcc = new VSOMAccessors();
                //VsomParameters vsprm = new VsomParameters();
                InitializeComponent();
                this.DataContext = this;

                lieux = vsomAcc.GetLieux();
                lieuxOp = new List<string>();
                foreach (LIEU l in lieux)
                {
                    lieuxOp.Add(l.NomLieu);
                }

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

        public IdentificationVehiculeForm(VehiculePanel panel, VEHICULE veh, UTILISATEUR user)
        {
            try
            {
                 vsomAcc = new VSOMAccessors();
                //VsomParameters vsprm = new VsomParameters();
                InitializeComponent();
                this.DataContext = this;

                lieux = vsomAcc.GetLieux();
                lieuxOp = new List<string>();
                foreach (LIEU l in lieux)
                {
                    lieuxOp.Add(l.NomLieu);
                }

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

        private void btnIdentifier_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                 vsomAcc = new VSOMAccessors();
                //VsomMarchal vsomAcc = new VsomMarchal();

                VEHICULE veh = null;
                if (vehForm != null)
                {
                    if (cbNumLieu.SelectedIndex == -1)
                    {
                        MessageBox.Show("Veuillez sélectionner un lieu d'identification", "Lieu d'identification ?", MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }
                    else if ((chkAttelle.IsChecked == true || chkPorte.IsChecked == true))
                    {
                        veh = vsomAcc.IdentifierVehiculeAP(Convert.ToInt32(vehForm.txtIdChassis.Text), lieux.ElementAt<LIEU>(cbNumLieu.SelectedIndex).IdLieu, (chkPorte.IsChecked == true ? "Y" : "N"), (chkCle.IsChecked == true ? "Y" : "N"), (chkAttelle.IsChecked == true ? "Y" : "N"), (chkDemarre.IsChecked == true ? "Y" : "N"), (chkMinutie.IsChecked == true ? "Y" : "N"), vehiculesAP.ElementAt<VEHICULE>(cbNumChassisAP.SelectedIndex).IdVeh, new TextRange(txtObservations.Document.ContentStart, txtObservations.Document.ContentEnd).Text, utilisateur.IdU);
                    }
                    else
                    {
                        veh = vsomAcc.IdentifierVehicule(Convert.ToInt32(vehForm.txtIdChassis.Text), lieux.ElementAt<LIEU>(cbNumLieu.SelectedIndex).IdLieu, (chkPorte.IsChecked == true ? "Y" : "N"), (chkCle.IsChecked == true ? "Y" : "N"), (chkAttelle.IsChecked == true ? "Y" : "N"), (chkDemarre.IsChecked == true ? "Y" : "N"), (chkMinutie.IsChecked == true ? "Y" : "N"), new TextRange(txtObservations.Document.ContentStart, txtObservations.Document.ContentEnd).Text, utilisateur.IdU);
                    }
                    //Raffraîchir les informations
                    formLoader.LoadVehiculeForm(vehForm, veh);
                    if (vehForm.vehPanel != null)
                    {
                        //if (vehForm.vehPanel.cbFiltres.SelectedIndex != 0)
                        //{
                        //    vehForm.vehPanel.cbFiltres.SelectedIndex = 0;
                        //}
                        //else
                        //{
                        //    vehForm.vehPanel.vehicules = vsomAcc.GetVehiculesImport();
                        //    vehForm.vehPanel.dataGrid.ItemsSource = vehForm.vehPanel.vehicules;
                        //}
                    }
                }
                else if (vehPanel != null)
                {
                    if (cbNumLieu.SelectedIndex == -1)
                    {
                        MessageBox.Show("Veuillez sélectionner un lieu d'identification", "Lieu d'identification ?", MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }
                    else if ((chkAttelle.IsChecked == true || chkPorte.IsChecked == true))
                    {
                        veh = vsomAcc.IdentifierVehiculeAP(vehicule.IdVeh, lieux.ElementAt<LIEU>(cbNumLieu.SelectedIndex).IdLieu, (chkPorte.IsChecked == true ? "Y" : "N"), (chkCle.IsChecked == true ? "Y" : "N"), (chkAttelle.IsChecked == true ? "Y" : "N"), (chkDemarre.IsChecked == true ? "Y" : "N"), (chkMinutie.IsChecked == true ? "Y" : "N"), vehiculesAP.ElementAt<VEHICULE>(cbNumChassisAP.SelectedIndex).IdVeh, new TextRange(txtObservations.Document.ContentStart, txtObservations.Document.ContentEnd).Text, utilisateur.IdU);
                        //Raffraîchir les informations
                        //if (vehPanel.cbFiltres.SelectedIndex != 0)
                        //{
                        //    vehPanel.cbFiltres.SelectedIndex = 0;
                        //}
                        //else
                        //{
                        //    vehPanel.vehicules = vsomAcc.GetVehiculesImport();
                        //    vehPanel.dataGrid.ItemsSource = vehPanel.vehicules;
                        //}
                    }
                    else
                    {
                        veh = vsomAcc.IdentifierVehicule(vehicule.IdVeh, lieux.ElementAt<LIEU>(cbNumLieu.SelectedIndex).IdLieu, (chkPorte.IsChecked == true ? "Y" : "N"), (chkCle.IsChecked == true ? "Y" : "N"), (chkAttelle.IsChecked == true ? "Y" : "N"), (chkDemarre.IsChecked == true ? "Y" : "N"), (chkMinutie.IsChecked == true ? "Y" : "N"), new TextRange(txtObservations.Document.ContentStart, txtObservations.Document.ContentEnd).Text, utilisateur.IdU);
                        //Raffraîchir les informations
                        //if (vehPanel.cbFiltres.SelectedIndex != 0)
                        //{
                        //    vehPanel.cbFiltres.SelectedIndex = 0;
                        //}
                        //else
                        //{
                        //    vehPanel.vehicules = vsomAcc.GetVehiculesImport();
                        //    vehPanel.dataGrid.ItemsSource = vehPanel.vehicules;
                        //}
                    }
                }

                MessageBox.Show("L'opération d'identification s'est déroulée avec succès, consultez le journal des éléments de facturation", "Véhicule identifié !", MessageBoxButton.OK, MessageBoxImage.Information);
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
                //VSOMAccessors vsomAcc = new VSOMAccessors();

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
                //VSOMAccessors vsomAcc = new VSOMAccessors();

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
