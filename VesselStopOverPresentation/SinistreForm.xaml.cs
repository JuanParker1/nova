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
    /// Logique d'interaction pour SinistreForm.xaml
    /// </summary>
    public partial class SinistreForm : Window
    {
        public List<VEHICULE> vehicules { get; set; }
        public List<string> vehs { get; set; }

        public List<CONNAISSEMENT> connaissements { get; set; }
        public List<string> cons { get; set; }

        private List<TYPE_VEHICULE> typesVehicules;
        public List<string> tps { get; set; }

        public List<ElementFacturation> eltsFact { get; set; }

        public VehiculePanel vehPanel { get; set; }
        public VehiculeForm vehForm { get; set; }

        public SinistrePanel sinPanel { get; set; }

        public List<TypeSinistreVehicule> typesSinistres { get; set; }

        private List<OPERATION> operationsUser;
        private UTILISATEUR utilisateur;

        private FormLoader formLoader;

        private string typeForm;
        private VsomParameters vsp = new VsomParameters();
        public SinistreForm(SinistrePanel panel, OPERATION_SINISTRE sin, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();
                //VsomParameters vsp = new VsomParameters();
                InitializeComponent();
                this.DataContext = this;

                typesVehicules = vsp.GetTypesVehicules();
                tps = new List<string>();
                foreach (TYPE_VEHICULE t in typesVehicules)
                {
                    tps.Add(t.CodeTypeVeh);
                }

                sinPanel = panel;

                utilisateur = user;
                operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);

                formLoader = new FormLoader(utilisateur);

                formLoader.LoadSinistreForm(this, sin);
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        public SinistreForm(string type, SinistrePanel panel, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();
               // VsomParameters vsp = new VsomParameters();
                InitializeComponent();
                this.DataContext = this;

                typesVehicules = vsp.GetTypesVehicules();
                tps = new List<string>();
                foreach (TYPE_VEHICULE t in typesVehicules)
                {
                    tps.Add(t.CodeTypeVeh);
                }

                sinPanel = panel;

                utilisateur = user;
                operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);

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

        public SinistreForm(string type, VehiculeForm form, VEHICULE veh, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();
               // VsomParameters vsp = new VsomParameters();
                InitializeComponent();
                this.DataContext = this;

                typesVehicules = vsp.GetTypesVehicules();
                tps = new List<string>();
                foreach (TYPE_VEHICULE t in typesVehicules)
                {
                    tps.Add(t.CodeTypeVeh);
                }

                vehForm = form;

                utilisateur = user;
                operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);

                formLoader = new FormLoader(utilisateur);

                formLoader.LoadVehiculeForm(this, veh);
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        public SinistreForm(string type, VehiculePanel panel, VEHICULE veh, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();
                
                InitializeComponent();
                this.DataContext = this;

                typesVehicules = vsp.GetTypesVehicules();
                tps = new List<string>();
                foreach (TYPE_VEHICULE t in typesVehicules)
                {
                    tps.Add(t.CodeTypeVeh);
                }

                vehPanel = panel;

                utilisateur = user;
                operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);

                formLoader = new FormLoader(utilisateur);

                formLoader.LoadVehiculeForm(this, veh);
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        private void cbNumBL_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (cbNumBL.SelectedIndex != -1)
                {
                    int indexBL = cbNumBL.SelectedIndex;
                    CONNAISSEMENT con = connaissements.ElementAt<CONNAISSEMENT>(indexBL);
                    txtIdBL.Text = con.IdBL.ToString();
                    txtConsignee.Text = con.ConsigneeBL;
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

        private void cbTypeVehM_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                double vol = Convert.ToDouble(txtVolM.Text);
                if (vol < 16)
                {
                    cbTypeVehM.SelectedIndex = 0;
                    txtTypeVehM.Text = "Car - (De 0 à 16 m³)";
                }
                else if (vol < 50)
                {
                    cbTypeVehM.SelectedIndex = 1;
                    txtTypeVehM.Text = "Van - (De 16 à 50 m³)";
                }
                else
                {
                    cbTypeVehM.SelectedIndex = 2;
                    txtTypeVehM.Text = "Truck - (Plus de 50 m³)";
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

        private void cbTypeVehC_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                int indexType = cbTypeVehC.SelectedIndex;
                TYPE_VEHICULE t = typesVehicules.ElementAt<TYPE_VEHICULE>(indexType);
                txtTypeVehC.Text = t.LibTypeVeh + " - (De " + t.DeVol + " à " + t.LimVol + " m³)";
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

        }

        private void cbNumChassis_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                if (e.Key == Key.Return && cbNumChassis.Text.Trim() != "")
                {
                    vehicules = vsp.GetVehiculesByNumChassis(cbNumChassis.Text);

                    if (vehicules.Count == 0)
                    {
                        MessageBox.Show("Il n'existe aucun véhicule portant ce numéro de chassis", "Chassis introuvable", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                    else if (vehicules.Count == 1)
                    {
                        VEHICULE veh = vehicules.FirstOrDefault<VEHICULE>();
                        formLoader.LoadVehiculeForm(this, veh);
                    }
                    else
                    {
                        ListVehiculeForm listVehForm = new ListVehiculeForm(this, vehicules, utilisateur);
                        listVehForm.Title = "Choix multiples : Sélectionnez un véhicule";
                        listVehForm.ShowDialog();
                    }
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
