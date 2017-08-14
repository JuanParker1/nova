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
    /// Logique d'interaction pour ReceptionVehiculeForm.xaml
    /// </summary>
    public partial class ReceptionVehiculeForm : Window
    {

        private VehiculeForm vehForm;
        private VehiculePanel vehPanel;

        private VEHICULE vehicule;

        private List<PARC> parcs;
        public List<string> prcs { get; set; }

        private List<EMPLACEMENT> emplacements;
        public List<string> empls { get; set; }

        private List<OPERATION> operationsUser;
        private UTILISATEUR utilisateur;

        private FormLoader formLoader;
        private VsomParameters vsp = new VsomParameters();
        public ReceptionVehiculeForm(VehiculeForm form, VEHICULE veh, UTILISATEUR user)
        {
            try
            {

                InitializeComponent();
               // using (var ctx = new VSOMClassesDataContext())
               // {
                    VSOMAccessors vsomAcc = new VSOMAccessors();

                    this.DataContext = this;

                    parcs = vsp.GetParcs("V");
                    prcs = new List<string>();
                    foreach (PARC p in parcs)
                    {
                        prcs.Add(p.NomParc);
                    }

                    utilisateur = user;
                    operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);

                    vehicule = veh;
                    vehForm = form;

                    formLoader = new FormLoader(utilisateur);
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

        public ReceptionVehiculeForm(VehiculePanel panel, VEHICULE veh, UTILISATEUR user)
        {
            try
            {

                InitializeComponent();
               // using (var ctx = new VSOMClassesDataContext())
                //{
                    VSOMAccessors vsomAcc = new VSOMAccessors();

                    this.DataContext = this;

                    parcs = vsp.GetParcs("V");
                    prcs = new List<string>();
                    foreach (PARC p in parcs)
                    {
                        prcs.Add(p.NomParc);
                    }

                    utilisateur = user;
                    operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);

                    vehicule = veh;
                    vehPanel = panel;

                    formLoader = new FormLoader(utilisateur);
               // }
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        private void cbParc_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
               // using (var ctx = new VSOMClassesDataContext())
               // {
                   // VSOMAccessors vs = new VSOMAccessors(ctx);
                    VsomParameters vsomAcc = new VsomParameters();

                    if (parcs.ElementAt<PARC>(cbParc.SelectedIndex).IdParc == 1)
                    {
                        cbEmplacement.ItemsSource = null;
                        emplacements = vsomAcc.GetEmplacementDisposByIdParc(parcs.ElementAt<PARC>(cbParc.SelectedIndex).IdParc, vehicule.LongCVeh.Value, vehicule.LargCVeh.Value);
                    }
                    else
                    {
                        cbEmplacement.ItemsSource = null;
                        emplacements = vsomAcc.GetEmplacementByIdParc(parcs.ElementAt<PARC>(cbParc.SelectedIndex).IdParc);
                    }
                    empls = new List<string>();
                    foreach (EMPLACEMENT em in emplacements)
                    {
                        empls.Add(em.LigEmpl + em.ColEmpl);
                    }
                    cbEmplacement.ItemsSource = empls;
               // }
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
            
        }

        private void btnReceptionner_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //VSOMAccessors vsomAcc = new VSOMAccessors();
                //using (var ctx = new VSOMClassesDataContext())
               // {
                    //VSOMAccessors vs = new VSOMAccessors(ctx);
                    VsomMarchal vsomAcc = new VsomMarchal();

                    if (cbParc.SelectedIndex == -1)
                    {
                        MessageBox.Show("Veuillez sélectionner un parc de reception pour ce véhicule", "Parc ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                    else if (cbEmplacement.SelectedIndex == -1)
                    {
                        MessageBox.Show("Veuillez sélectionner un emplacement où sera parqué ce véhicule", "Emplacement ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                    else
                    {
                        VEHICULE veh = vsomAcc.ReceptionnerVehicule(vehicule.IdVeh, emplacements.ElementAt<EMPLACEMENT>(cbEmplacement.SelectedIndex).IdEmpl, 3, new TextRange(txtObservations.Document.ContentStart, txtObservations.Document.ContentEnd).Text, utilisateur.IdU);
                        //Raffraîchir les informations
                        if (vehForm != null)
                        {
                            formLoader.LoadVehiculeForm(vehForm, veh);
                            if (vehForm.vehPanel != null)
                            {
                                if (vehForm.vehPanel.cbFiltres.SelectedIndex != 0)
                                {
                                    vehForm.vehPanel.cbFiltres.SelectedIndex = 0;
                                }
                                else
                                {
                                    vehForm.vehPanel.vehicules = vsp.GetVehiculesImport();
                                    vehForm.vehPanel.dataGrid.ItemsSource = vehForm.vehPanel.vehicules;
                                }
                            }
                        }
                        else if (vehPanel != null)
                        {
                            if (vehPanel.cbFiltres.SelectedIndex != 0)
                            {
                                vehPanel.cbFiltres.SelectedIndex = 0;
                            }
                            else
                            {
                                vehPanel.vehicules = vsp.GetVehiculesImport();
                                vehPanel.dataGrid.ItemsSource = vehPanel.vehicules;
                            }
                        }

                        MessageBox.Show("L'opération de réception au parc auto s'est déroulée avec succès", "Véhicule réceptionné !", MessageBoxButton.OK, MessageBoxImage.Information);
                        this.Close();
                    }
                //}
            }
            catch (HabilitationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (EnregistrementInexistant ex)
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
    }
}
