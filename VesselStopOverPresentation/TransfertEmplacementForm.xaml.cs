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
    /// Logique d'interaction pour TransfertEmplacementForm.xaml
    /// </summary>
    public partial class TransfertEmplacementForm : Window
    {

        private VehiculeForm vehForm;
        private VehiculePanel vehPanel;

        private VEHICULE vehicule;

        private List<PARC> parcs;
        public List<string> prcsNouveau { get; set; }

        private List<EMPLACEMENT> emplacements;
        public List<string> emplsNouveau { get; set; }

        private List<OPERATION> operationsUser;
        private UTILISATEUR utilisateur;

        private FormLoader formLoader;
        //private VsomParameters vsp = new VsomParameters();
        private VSOMAccessors vsomAcc;
        public TransfertEmplacementForm(VehiculeForm form, VEHICULE veh, UTILISATEUR user)
        {
            try
            {
                 vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

                parcs = vsomAcc.GetParcs("V");
                prcsNouveau = new List<string>();
                foreach (PARC p in parcs)
                {
                    prcsNouveau.Add(p.NomParc);
                }

                utilisateur = user;
                operationsUser = vsomAcc.GetOperationsUtilisateur(utilisateur.IdU);

                vehicule = veh;
                vehForm = form;

                List<OCCUPATION> occupations = veh.OCCUPATION.Where(occ => !occ.DateFin.HasValue).ToList<OCCUPATION>();
                if (occupations.Count != 0)
                {
                    EMPLACEMENT empl = occupations.FirstOrDefault<OCCUPATION>().EMPLACEMENT;
                    txtParcActuel.Text = empl.PARC.NomParc;
                    txtEmplacementActuel.Text = empl.LigEmpl + empl.ColEmpl;
                }

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

        public TransfertEmplacementForm(VehiculePanel panel, VEHICULE veh, UTILISATEUR user)
        {
            try
            {
                 vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

                parcs = vsomAcc.GetParcs("V");
                prcsNouveau = new List<string>();
                foreach (PARC p in parcs)
                {
                    prcsNouveau.Add(p.NomParc);
                }

                utilisateur = user;
                operationsUser = vsomAcc.GetOperationsUtilisateur(utilisateur.IdU);

                vehicule = veh;
                vehPanel = panel;

                List<OCCUPATION> occupations = veh.OCCUPATION.Where(occ => !occ.DateFin.HasValue).ToList<OCCUPATION>();
                if (occupations.Count != 0)
                {
                    EMPLACEMENT empl = occupations.FirstOrDefault<OCCUPATION>().EMPLACEMENT;
                    txtParcActuel.Text = empl.PARC.NomParc;
                    txtEmplacementActuel.Text = empl.LigEmpl + empl.ColEmpl;
                }

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

        private void btnTransferer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //VSOMAccessors vsomAcc = new VSOMAccessors();
                //VsomMarchal vsomAcc = new VsomMarchal();

                if (cbParcNouveau.SelectedIndex == -1)
                {
                    MessageBox.Show("Veuillez sélectionner la zone où sera parqué ce véhicule", "Parc ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (cbEmplacementNouveau.SelectedIndex == -1)
                {
                    MessageBox.Show("Veuillez sélectionner un emplacement où sera parqué ce véhicule", "Emplacement ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    VEHICULE veh = vsomAcc.TransfertEmplacement(vehicule.IdVeh, emplacements.ElementAt<EMPLACEMENT>(cbEmplacementNouveau.SelectedIndex).IdEmpl, 3, new TextRange(txtObservations.Document.ContentStart, txtObservations.Document.ContentEnd).Text, utilisateur.IdU);
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
                                vehForm.vehPanel.vehicules = vsomAcc.GetVehiculesImport();
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
                            vehPanel.vehicules = vsomAcc.GetVehiculesImport();
                            vehPanel.dataGrid.ItemsSource = vehPanel.vehicules;
                        }
                    }

                    MessageBox.Show("L'opération de transfert d'emplacement s'est déroulée avec succès", "Véhicule transféré !", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();
                }

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

        private void cbParcNouveau_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                //VSOMAccessors vsomAcc = new VSOMAccessors();

                if (parcs.ElementAt<PARC>(cbParcNouveau.SelectedIndex).IdParc == 1)
                {
                    cbEmplacementNouveau.ItemsSource = null;
                    emplacements = vsomAcc.GetEmplacementDisposByIdParc(parcs.ElementAt<PARC>(cbParcNouveau.SelectedIndex).IdParc, vehicule.LongCVeh.Value, vehicule.LargCVeh.Value);
                }
                else
                {
                    cbEmplacementNouveau.ItemsSource = null;
                    emplacements = vsomAcc.GetEmplacementByIdParc(parcs.ElementAt<PARC>(cbParcNouveau.SelectedIndex).IdParc);
                }
                
                emplsNouveau = new List<string>();
                foreach (EMPLACEMENT em in emplacements)
                {
                    emplsNouveau.Add(em.LigEmpl + em.ColEmpl);
                }
                cbEmplacementNouveau.ItemsSource = emplsNouveau;
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
