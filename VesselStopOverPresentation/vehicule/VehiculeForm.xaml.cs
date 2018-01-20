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
using System.Drawing.Printing;
using System.Diagnostics;

namespace VesselStopOverPresentation
{
    /// <summary>
    /// Logique d'interaction pour VehiculeForm.xaml
    /// </summary>
    public partial class VehiculeForm : Window
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
        public CubageForm cubForm { get; set; }
        private ManifesteForm manifForm;
        private ConnaissementForm connaissementForm;
        private DemandeVisiteForm visiteForm;
        private DemandeLivraisonForm livraisonForm;
        private BonEnleverForm baeForm;

        private List<OPERATION> operationsUser;
        private UTILISATEUR utilisateur;

        private FormLoader formLoader;

        private string typeForm;
        //private //VsomParameters vsomAcc = new VsomParameters();
        private VSOMAccessors vsomAcc;

        private System.IO.StreamReader streamToPrint;
        private System.Drawing.Font printFont;
        /// <summary>
        /// represent le fichier sortie encours d'impression
        /// </summary>
        private string filepath;

        public VehiculeForm(VehiculePanel panel, VEHICULE veh, UTILISATEUR user)
        {
            try
            {
                InitializeComponent();
               // using (var ctx = new VSOMClassesDataContext())
               // {
                    vsomAcc = new VSOMAccessors();
                    ////VsomParameters vsomAcc = new VsomParameters();

                    this.DataContext = this;

                    typesVehicules = vsomAcc.GetTypesVehicules();
                    tps = new List<string>();
                    foreach (TYPE_VEHICULE t in typesVehicules)
                    {
                        tps.Add(t.CodeTypeVeh);
                    }

                    vehPanel = panel;

                    utilisateur = user;
                    operationsUser = vsomAcc.GetOperationsUtilisateur(utilisateur.IdU);

                    formLoader = new FormLoader(utilisateur);

                    formLoader.LoadVehiculeForm(this, veh);
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

        public VehiculeForm(CubageForm form, VEHICULE veh, UTILISATEUR user)
        {
            try
            {
                //VSOMAccessors vsomAcc = new VSOMAccessors();
                //VsomParameters vsomAcc = new VsomParameters();

                InitializeComponent();
                //using (var ctx = new VSOMClassesDataContext())
                //{
                    vsomAcc = new VSOMAccessors();

                    this.DataContext = this;

                    typesVehicules = vsomAcc.GetTypesVehicules();
                    tps = new List<string>();
                    foreach (TYPE_VEHICULE t in typesVehicules)
                    {
                        tps.Add(t.CodeTypeVeh);
                    }

                    cubForm = form;

                    utilisateur = user;
                    operationsUser = vsomAcc.GetOperationsUtilisateur(utilisateur.IdU);

                    formLoader = new FormLoader(utilisateur);

                    formLoader.LoadVehiculeForm(this, veh);
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

        public VehiculeForm(ManifesteForm form, VEHICULE veh, UTILISATEUR user)
        {
            try
            {
                //VSOMAccessors vsomAcc = new VSOMAccessors();
                //VsomParameters vsomAcc = new VsomParameters();
                InitializeComponent();
                //using (var ctx = new VSOMClassesDataContext())
                //{
                    vsomAcc = new VSOMAccessors();

                    this.DataContext = this;

                    typesVehicules = vsomAcc.GetTypesVehicules();
                    tps = new List<string>();
                    foreach (TYPE_VEHICULE t in typesVehicules)
                    {
                        tps.Add(t.CodeTypeVeh);
                    }

                    manifForm = form;

                    utilisateur = user;
                    operationsUser = vsomAcc.GetOperationsUtilisateur(utilisateur.IdU);

                    formLoader = new FormLoader(utilisateur);

                    formLoader.LoadVehiculeForm(this, veh);
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

        public VehiculeForm(ConnaissementForm form, VEHICULE veh, UTILISATEUR user)
        {
            try
            {
                //VSOMAccessors vsomAcc = new VSOMAccessors();
                //VsomParameters vsomAcc = new VsomParameters();
                InitializeComponent();
               // using (var ctx = new VSOMClassesDataContext())
               // {
                    vsomAcc = new VSOMAccessors();

                    this.DataContext = this;

                    typesVehicules = vsomAcc.GetTypesVehicules();
                    tps = new List<string>();
                    foreach (TYPE_VEHICULE t in typesVehicules)
                    {
                        tps.Add(t.CodeTypeVeh);
                    }

                    connaissementForm = form;
                    utilisateur = user;
                    operationsUser = vsomAcc.GetOperationsUtilisateur(utilisateur.IdU);

                    formLoader = new FormLoader(utilisateur);

                    formLoader.LoadVehiculeForm(this, veh);
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

        public VehiculeForm(DemandeVisiteForm form, VEHICULE veh, UTILISATEUR user)
        {
            try
            {
                //VSOMAccessors vsomAcc = new VSOMAccessors();
                //VsomParameters vsomAcc = new VsomParameters();

                InitializeComponent();
               // using (var ctx = new VSOMClassesDataContext())
               // {
                    vsomAcc = new VSOMAccessors();
                    this.DataContext = this;

                    typesVehicules = vsomAcc.GetTypesVehicules();
                    tps = new List<string>();
                    foreach (TYPE_VEHICULE t in typesVehicules)
                    {
                        tps.Add(t.CodeTypeVeh);
                    }

                    visiteForm = form;
                    utilisateur = user;
                    operationsUser = vsomAcc.GetOperationsUtilisateur(utilisateur.IdU);

                    formLoader = new FormLoader(utilisateur);

                    formLoader.LoadVehiculeForm(this, veh);
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

        public VehiculeForm(DemandeLivraisonForm form, VEHICULE veh, UTILISATEUR user)
        {
            try
            {
                //VSOMAccessors vsomAcc = new VSOMAccessors();
                //VsomParameters vsomAcc = new VsomParameters();
                
                InitializeComponent();
               // using (var ctx = new VSOMClassesDataContext())
               // {
                    vsomAcc = new VSOMAccessors();
                    this.DataContext = this;

                    typesVehicules = vsomAcc.GetTypesVehicules();
                    tps = new List<string>();
                    foreach (TYPE_VEHICULE t in typesVehicules)
                    {
                        tps.Add(t.CodeTypeVeh);
                    }

                    livraisonForm = form;
                    utilisateur = user;
                    operationsUser = vsomAcc.GetOperationsUtilisateur(utilisateur.IdU);

                    formLoader = new FormLoader(utilisateur);

                    formLoader.LoadVehiculeForm(this, veh);
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

        public VehiculeForm(BonEnleverForm form, VEHICULE veh, UTILISATEUR user)
        {
            try
            {
                //VSOMAccessors vsomAcc = new VSOMAccessors();
                //VsomParameters vsomAcc = new VsomParameters();
               
                InitializeComponent();
               // using (var ctx = new VSOMClassesDataContext())
               // {
                    vsomAcc = new VSOMAccessors();
                    this.DataContext = this;

                    typesVehicules = vsomAcc.GetTypesVehicules();
                    tps = new List<string>();
                    foreach (TYPE_VEHICULE t in typesVehicules)
                    {
                        tps.Add(t.CodeTypeVeh);
                    }

                    baeForm = form;
                    utilisateur = user;
                    operationsUser = vsomAcc.GetOperationsUtilisateur(utilisateur.IdU);

                    formLoader = new FormLoader(utilisateur);

                    formLoader.LoadVehiculeForm(this, veh);
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

        public VehiculeForm(string type, VehiculePanel panel, UTILISATEUR user)
        {
            try
            {
                //VSOMAccessors vsomAcc = new VSOMAccessors();
                //VsomParameters vsomAcc = new VsomParameters();

                InitializeComponent();
                //using (var ctx = new VSOMClassesDataContext())
                //{
                    vsomAcc = new VSOMAccessors();

                    this.DataContext = this;

                    typesVehicules = vsomAcc.GetTypesVehicules();
                    tps = new List<string>();
                    foreach (TYPE_VEHICULE t in typesVehicules)
                    {
                        tps.Add(t.CodeTypeVeh);
                    }

                    vehPanel = panel;

                    utilisateur = user;
                    operationsUser = vsomAcc.GetOperationsUtilisateur(utilisateur.IdU);

                    typeForm = type;

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

        public VehiculeForm(string type, VehiculeForm form, VEHICULE veh, UTILISATEUR user)
        {
            try
            {
                //VSOMAccessors vsomAcc = new VSOMAccessors();
                //VsomParameters vsomAcc = new VsomParameters();
                InitializeComponent();
                //using (var ctx = new VSOMClassesDataContext())
                //{
                    vsomAcc = new VSOMAccessors();

                    this.DataContext = this;

                    connaissements = new List<CONNAISSEMENT>();
                    connaissements.Add(veh.CONNAISSEMENT);
                    cons = new List<string>();
                    foreach (CONNAISSEMENT bl in connaissements)
                    {
                        cons.Add(bl.NumBL);
                    }

                    typesVehicules = vsomAcc.GetTypesVehicules();
                    tps = new List<string>();
                    foreach (TYPE_VEHICULE t in typesVehicules)
                    {
                        tps.Add(t.CodeTypeVeh);
                    }

                    vehForm = form;

                    utilisateur = user;
                    operationsUser = vsomAcc.GetOperationsUtilisateur(utilisateur.IdU);

                    typeForm = type;

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
                double vol = Convert.ToDouble(txtVolM.Text.Replace(".", ","));
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
            try
            {
                ////VSOMAccessors vsomAcc = new VSOMAccessors();

                //using (var ctx = new VSOMClassesDataContext())
                //{
                    VsomMarchal vsomAcc = new VsomMarchal();

                    if (cbNumChassis.Text == "")
                    {
                        MessageBox.Show("Il faut saisir le numéro de chassis de ce véhicule", "N° de chassis du véhicule?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                    else if (txtDescription.Text == "")
                    {
                        MessageBox.Show("Il faut saisir la description ou la marque de ce véhicule", "Description du véhicule ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                    else if (txtLongC.Text == "0")
                    {
                        if (MessageBox.Show("La longueur cubée est nulle, voulez-vous la modifier maintenant ?", "Longueur cubée ?", MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
                        {
                            txtLongC.SelectAll();
                            txtLongC.Focus();
                        }
                    }
                    else if (txtLargC.Text == "0")
                    {
                        if (MessageBox.Show("La largeur cubée est nulle, voulez-vous la modifier maintenant ?", "Largeur cubée ?", MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
                        {
                            txtLargC.SelectAll();
                            txtLargC.Focus();
                        }
                    }
                    else if (txtHautC.Text == "0")
                    {
                        if (MessageBox.Show("La hauteur cubée est nulle, voulez-vous la modifier maintenant ?", "Hauteur cubée ?", MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
                        {
                            txtHautC.SelectAll();
                            txtHautC.Focus();
                        }
                    }
                    else if (txtVolC.Text == "0")
                    {
                        MessageBox.Show("Vous ne pouvez pas enregistrer un véhicule avec un cubé volume nul", "Volume cubé ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                    else if (txtPoidsC.Text == "0")
                    {
                        if (MessageBox.Show("Le poids réel est nul, voulez-vous le modifier maintenant ?", "Poids réel ?", MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
                        {
                            txtPoidsC.SelectAll();
                            txtPoidsC.Focus();
                        }
                    }
                    else
                    {

                        if (typeForm == "Nouveau AP")
                        {
                            if (txtLongC.Text == "0")
                            {
                                if (MessageBox.Show("La longueur cubée est nulle, voulez-vous la modifier maintenant ?", "Longueur cubée ?", MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
                                {
                                    txtLongC.SelectAll();
                                    txtLongC.Focus();
                                }
                            }
                            else if (txtLargC.Text == "0")
                            {
                                if (MessageBox.Show("La largeur cubée est nulle, voulez-vous la modifier maintenant ?", "Largeur cubée ?", MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
                                {
                                    txtLargC.SelectAll();
                                    txtLargC.Focus();
                                }
                            }
                            else if (txtHautC.Text == "0")
                            {
                                if (MessageBox.Show("La hauteur cubée est nulle, voulez-vous la modifier maintenant ?", "Hauteur cubée ?", MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
                                {
                                    txtHautC.SelectAll();
                                    txtHautC.Focus();
                                }
                            }
                            else if (txtVolC.Text == "0")
                            {
                                MessageBox.Show("Vous ne pouvez pas enregistrer un véhicule avec un cubé volume nul", "Volume cubé ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                            }
                            else if (txtPoidsC.Text == "0")
                            {
                                if (MessageBox.Show("Le poids réel est nul, voulez-vous le modifier maintenant ?", "Poids réel ?", MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
                                {
                                    txtPoidsC.SelectAll();
                                    txtPoidsC.Focus();
                                }
                            }
                            else
                            {
                                VEHICULE veh = vsomAcc.InsertVehiculeAP(Convert.ToInt32(txtIdBL.Text), Convert.ToInt32(vehForm.txtIdChassis.Text), cbNumChassis.Text, txtDescription.Text, txtBarCode.Text, txtEtatM.Text, cbTypeVehM.Text.Substring(0, 1), Convert.ToInt32(txtPoidsM.Text.Replace(" ", "").Replace(".", ",")), Convert.ToDouble(txtVolM.Text.Replace(" ", "").Replace(".", ",")), Convert.ToDouble(txtLongM.Text.Replace(" ", "").Replace(".", ",")), Convert.ToDouble(txtLargM.Text.Replace(" ", "").Replace(".", ",")), Convert.ToDouble(txtHautM.Text.Replace(" ", "").Replace(".", ",")), utilisateur.IdU);

                                formLoader.LoadVehiculeForm(this, veh);
                                typeForm = "";
                                MessageBox.Show("Véhicule créé avec succès", "Véhicule crée !", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                        }
                        else if (typeForm == "Nouveau")
                        {
                            if (txtIdBL.Text == "")
                            {
                                MessageBox.Show("Veuillez spécifier un BL pour la création d'un véhicule", "Sélectionnez un BL ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                            }
                            else if (txtVolM.Text == "0")
                            {
                                MessageBox.Show("Vous ne pouvez pas enregistrer un véhicule avec un volume manifesté nul", "Volume manifesté ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                            }
                            else
                            {
                                VEHICULE veh = vsomAcc.InsertVehicule(Convert.ToInt32(txtIdBL.Text), cbNumChassis.Text, txtDescription.Text, txtBarCode.Text, txtEtatM.Text, cbTypeVehM.Text.Substring(0, 1), Convert.ToInt32(txtPoidsM.Text.Replace(" ", "").Replace(".", ",")), Convert.ToDouble(txtVolM.Text.Replace(" ", "").Replace(".", ",")), Convert.ToDouble(txtLongM.Text.Replace(" ", "").Replace(".", ",")), Convert.ToDouble(txtLargM.Text.Replace(" ", "").Replace(".", ",")), Convert.ToDouble(txtHautM.Text.Replace(" ", "").Replace(".", ",")), utilisateur.IdU);

                                formLoader.LoadVehiculeForm(this, veh);
                                typeForm = "";
                                MessageBox.Show("Véhicule créé avec succès", "Véhicule crée !", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                        }
                        else
                        {
                            VEHICULE veh = null;
                            if (groupInfosManif.IsEnabled == true)
                            {
                                veh = vsomAcc.UpdateVehicule(Convert.ToInt32(txtIdChassis.Text), cbNumChassis.Text, txtDescription.Text, txtEtatM.Text, cbTypeVehM.Text, Convert.ToInt32(txtPoidsM.Text.Replace(".", ",")), Convert.ToDouble(txtVolM.Text.Replace(".", ",")), Convert.ToDouble(txtLongM.Text.Replace(".", ",")), Convert.ToDouble(txtLargM.Text.Replace(".", ",")), Convert.ToDouble(txtHautM.Text.Replace(".", ",")), utilisateur.IdU);
                            }

                            veh = vsomAcc.UpdateVehicule(Convert.ToInt32(txtIdChassis.Text), txtEtatM.Text, cbTypeVehC.Text, Convert.ToInt32(txtPoidsC.Text.Replace(".", ",")), Convert.ToDouble(txtVolC.Text.Replace(".", ",")), Convert.ToDouble(txtLongC.Text.Replace(".", ",")), Convert.ToDouble(txtLargC.Text.Replace(".", ",")), Convert.ToDouble(txtHautC.Text.Replace(".", ",")), utilisateur.IdU);

                            formLoader.LoadVehiculeForm(this, veh);

                            MessageBox.Show("Mise à jour effectuée avec succès", "Modification validée !", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                //}
            }
            catch (HabilitationException ex)
            {
                MessageBox.Show(ex.Message, "Droits insuffisants !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Erreur de l'application", MessageBoxButton.OK, MessageBoxImage.Stop);
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
            }
        }

        private void btnHistOps_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //VSOMAccessors vsomAcc = new VSOMAccessors();

                HistoriqueOperationsForm opForm = new HistoriqueOperationsForm(this, Convert.ToInt32(txtIdChassis.Text));
                opForm.Title = "Historique des opérations - Chassis N° " + cbNumChassis.Text;
                opForm.ShowDialog();
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
            
        }

        private void btnCalculerSejour_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                CalculerSejourVehiculeForm calcForm = new CalculerSejourVehiculeForm(this, utilisateur);
                calcForm.Title = "Calcul du séjour séjour - Chassis N° : " + cbNumChassis.Text;
                calcForm.ShowDialog();
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Calcule stationnement");
            }
        }

        private void txtDim_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9,.]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void txtDimC_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                if (txtLongC.Text != "" && txtLargC.Text != "" && txtHautC.Text != "")
                {
                    txtVolC.Text = Math.Round((Convert.ToDouble(txtLongC.Text.Replace(".", ",")) * Convert.ToDouble(txtLargC.Text.Replace(".", ",")) * Convert.ToDouble(txtHautC.Text.Replace(".", ","))), 3).ToString();
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

        private void txtVolC_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                double vol = Convert.ToDouble(txtVolC.Text.Replace(".", ","));
                if (vol <= 15)
                {
                    cbTypeVehC.SelectedItem = "C";
                }
                else if (vol > 15 && vol <= 25)
                {
                    cbTypeVehC.SelectedItem = "V";
                }
                else if (vol > 25 && vol <= 70)
                {
                    cbTypeVehC.SelectedItem = "T";
                }
                else if (vol > 70 && vol <= 140)
                {
                    cbTypeVehC.SelectedItem = "L";
                }
                else if (vol > 140 && vol <= 32500)
                {
                    cbTypeVehC.SelectedItem = "P";
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

        private void txtDimM_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                if (txtLongM.Text != "" && txtLargM.Text != "" && txtHautM.Text != "")
                {
                    txtVolM.Text = Math.Round((Convert.ToDouble(txtLongM.Text.Replace(".", ",")) * Convert.ToDouble(txtLargM.Text.Replace(".", ",")) * Convert.ToDouble(txtHautM.Text.Replace(".", ","))), 3).ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void txtVolM_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                double vol = Convert.ToDouble(txtVolM.Text.Replace(".", ","));
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

        private void btnIdentifier_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                IdentificationVehiculeForm identForm = new IdentificationVehiculeForm(this, utilisateur);
                identForm.cbNumChassisAP.IsEnabled = false;
                identForm.Title = "Identification - Chassis N° : " + cbNumChassis.Text;
                identForm.ShowDialog();
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
            
        }

        private void btnCuber_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CubageVehiculeForm cubForm = new CubageVehiculeForm(this, utilisateur);
                cubForm.Title = "Cubage - Chassis N° : " + cbNumChassis.Text;
                cubForm.ShowDialog();
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
                //using (var ctx = new VSOMClassesDataContext())
                //{
                    vsomAcc = new VSOMAccessors();

                    ReceptionVehiculeForm receptionForm = new ReceptionVehiculeForm(this, vsomAcc.GetVehiculeByIdVeh(Convert.ToInt32(txtIdChassis.Text)), utilisateur);
                    receptionForm.Title = "Réception - Chassis N° : " + cbNumChassis.Text;
                    receptionForm.ShowDialog();
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

        private void btnTransfEmpl_Click(object sender, RoutedEventArgs e)
        {
            try
            {
               // using (var ctx = new VSOMClassesDataContext())
               // {
                    vsomAcc = new VSOMAccessors();

                    TransfertEmplacementForm transfertForm = new TransfertEmplacementForm(this, vsomAcc.GetVehiculeByIdVeh(Convert.ToInt32(txtIdChassis.Text)), utilisateur);
                    transfertForm.Title = "Transfert - Chassis N° : " + cbNumChassis.Text;
                    transfertForm.ShowDialog();
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

        private void btnTransfertZoneSortie_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TransfertZoneSortieVehiculeForm transfertForm = new TransfertZoneSortieVehiculeForm(this, utilisateur);
                transfertForm.Title = "Transfert en zone de sortie - Chassis N° : " + cbNumChassis.Text;
                transfertForm.ShowDialog();
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        private void btnSortir_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SortirVehiculeForm sortieForm = new SortirVehiculeForm(this, utilisateur);
                sortieForm.Title = "Sortie - Chassis N° : " + cbNumChassis.Text;
                sortieForm.ShowDialog();
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        private void btnMAJChassis_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //VSOMAccessors vsomAcc = new VSOMAccessors();

                MAJChassisForm mAJChassisForm = new MAJChassisForm(this, vsomAcc.GetVehiculeByIdVeh(Convert.ToInt32(txtIdChassis.Text)), utilisateur);
                mAJChassisForm.txtAncienChassis.Text = cbNumChassis.Text;
                mAJChassisForm.Title = "Mise à jour du chassis - Chassis N° : " + cbNumChassis.Text;
                mAJChassisForm.ShowDialog();
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        private void btnEnGC_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ////VSOMAccessors vsomAcc = new VSOMAccessors();
                VsomMarchal vsomAcc = new VsomMarchal();

                if (MessageBox.Show("Etes nous sûr(e) de vouloir convertir ce véhicule en accessoire ? Cette Opération est irréversible.", "Etes vous sûr(e)", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    CONVENTIONNEL conv = vsomAcc.ConvertirEnConventionnel(Convert.ToInt32(txtIdChassis.Text), utilisateur.IdU);

                    if (vehPanel.cbFiltres.SelectedIndex != 0)
                    {
                        vehPanel.cbFiltres.SelectedIndex = 0;
                    }
                    else
                    {
                        vehPanel.vehicules = vsomAcc.GetVehiculesImport();
                        vehPanel.dataGrid.ItemsSource = vehPanel.vehicules;
                    }

                    MessageBox.Show("Le véhicule " + cbNumChassis.Text + " a été converti en accessoire", "Véhicule converti en accessoire !", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();
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

        private void cbNumChassis_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                //VSOMAccessors vsomAcc = new VSOMAccessors();

                if (e.Key == Key.Return && typeForm == null && cbNumChassis.Text.Trim() != "")
                {
                    if (utilisateur.IdAcc == 1)
                    {
                        vehicules = vsomAcc.GetVehiculesByNumChassis(cbNumChassis.Text);
                    }
                    else
                    {
                        vehicules = vsomAcc.GetVehiculesByNumChassis(cbNumChassis.Text, utilisateur.IdAcc.Value);
                    }

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

        private void btnAjoutVehAP_Click(object sender, RoutedEventArgs e)
        {
            try
            {
               vsomAcc = new VSOMAccessors();

                if (operationsUser.Where(op => op.NomOp == "Véhicule : Enregistrement d'un nouvel élément").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
                {
                    MessageBox.Show("Vous n'avez pas les droits nécessaires pour créer un nouveau véhicule. Veuillez contacter un administrateur", "Droits insuffisants !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    VEHICULE vehPorteur = vsomAcc.GetVehiculeByIdVeh(Convert.ToInt32(txtIdChassis.Text));

                    VehiculeForm vehForm = new VehiculeForm("Nouveau AP", this, vehPorteur, utilisateur);
                    vehForm.cbNumChassis.IsEditable = true;
                    vehForm.txtLongM.Text = "0";
                    vehForm.txtLargM.Text = "0";
                    vehForm.txtHautM.Text = "0";
                    vehForm.txtVolM.Text = "0";
                    vehForm.txtPoidsM.Text = "0";
                    vehForm.txtEtatM.SelectedIndex = 0;
                    vehForm.txtChassisAP.Text = vehPorteur.NumChassis;
                    vehForm.txtIdChassisAP.Text = vehPorteur.IdVeh.ToString();
                    vehForm.Title = "Nouveau : Véhicule";
                    vehForm.actionsBorder.Visibility = System.Windows.Visibility.Collapsed;
                    vehForm.borderEtat.Visibility = System.Windows.Visibility.Collapsed;
                    vehForm.txtBarCode.IsReadOnly = false;
                    vehForm.txtBarCode.Background = Brushes.White;
                    vehForm.txtLongM.IsReadOnly = false;
                    vehForm.txtLongM.Background = Brushes.White;
                    vehForm.txtLargM.IsReadOnly = false;
                    vehForm.txtLargM.Background = Brushes.White;
                    vehForm.txtHautM.IsReadOnly = false;
                    vehForm.txtHautM.Background = Brushes.White;
                    vehForm.txtVolM.IsReadOnly = false;
                    vehForm.txtVolM.Background = Brushes.White;
                    vehForm.txtPoidsM.IsReadOnly = false;
                    vehForm.txtPoidsM.Background = Brushes.White;
                    vehForm.txtEtatM.IsReadOnly = false;
                    vehForm.txtEtatM.Background = Brushes.White;
                    vehForm.cbNumBL.SelectedItem = cbNumBL.Text;

                    vehForm.groupInfosCub.IsEnabled = false;
                    vehForm.Show();
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

        private void btnConstatSinistre_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ConstatSinistreVehiculeForm sinistreForm = new ConstatSinistreVehiculeForm(this, utilisateur);
                sinistreForm.Title = "Constat de sinistre - Chassis N° : " + cbNumChassis.Text;
                sinistreForm.ShowDialog();
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        private void listNotes_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (listNotes.SelectedIndex != -1)
                {
                    NOTE note = (NOTE)listNotes.SelectedItem;
                    NoteForm noteForm = new NoteForm(this, note, utilisateur);
                    noteForm.Title = "Note - " + note.IdNote + " - Veh - " + note.VEHICULE.NumChassis;
                    noteForm.lblStatut.Content = "Note crée le : " + note.DateNote + " par " + note.UTILISATEUR.NU;
                    noteForm.ShowDialog();
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

        private void cbNumBL_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                //using (var ctx = new VSOMClassesDataContext())
                //{
                    vsomAcc = new VSOMAccessors();

                    if (e.Key == Key.Return && cbNumBL.Text.Trim() != "")
                    {
                        connaissements = vsomAcc.GetConnaissementByNumBL(cbNumBL.Text);

                        if (connaissements.Count == 0)
                        {
                            MessageBox.Show("Il n'existe aucun connaissement portant ce numéro", "Connaissement introuvable", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        }
                        else if (connaissements.Count == 1)
                        {
                            CONNAISSEMENT con = connaissements.FirstOrDefault<CONNAISSEMENT>();
                            formLoader.LoadConnaissementForm(this, con);
                        }
                        else
                        {
                            ListConnaissementForm listConForm = new ListConnaissementForm(this, connaissements, utilisateur);
                            listConForm.Title = "Choix multiples : Sélectionnez un connaissement";
                            listConForm.ShowDialog();
                        }
                    }
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

        private void btnImprimerBonSortie_Click(object sender, RoutedEventArgs e)
        {
            btnImprimerBonSortie.IsEnabled = false;
            Cursor = Cursors.Wait;

            try
            {
               /* using (var ctx = new VSOMClassesDataContext())
                {*/
                    vsomAcc = new VSOMAccessors();

                    VEHICULE v = vsomAcc.GetVehiculeByIdVeh(Convert.ToInt32(txtIdChassis.Text));

                    /*  SortieReport sortieReport = new SortieReport(this);
                      sortieReport.Title = "Impression du bon de sortie : " + v.IdBS + " - Escale : " + v.ESCALE.NumEsc.ToString();
                      sortieReport.Show(); */

                    Microsoft.Reporting.WinForms.ReportViewer repViewer = new Microsoft.Reporting.WinForms.ReportViewer();
                    repViewer.ProcessingMode = Microsoft.Reporting.WinForms.ProcessingMode.Remote;
                    repViewer.ServerReport.ReportServerUrl = new Uri("http://192.168.0.28/ReportServer");
                    repViewer.ShowParameterPrompts = false;
                    repViewer.ServerReport.ReportPath = "/VSOMReports/Sortie_0117";

                    System.Net.NetworkCredential myCred = new System.Net.NetworkCredential("novareports", "novareports", "siege.local");
                    repViewer.ServerReport.ReportServerCredentials.NetworkCredentials = myCred;

                    Microsoft.Reporting.WinForms.ReportParameter[] parameters = new Microsoft.Reporting.WinForms.ReportParameter[2];
                    parameters[0] = new Microsoft.Reporting.WinForms.ReportParameter("RefBS",
                        v.IdBS.ToString());
                    parameters[1] = new Microsoft.Reporting.WinForms.ReportParameter("FilGran", "ACCONIER");
                    repViewer.ServerReport.SetParameters(parameters);
                    repViewer.ServerReport.Refresh();
                    repViewer.RefreshReport();

                    Microsoft.Reporting.WinForms.Warning[] warnings;
                    string[] streamIds;
                    string mimeType = string.Empty;
                    string encoding = string.Empty;
                    string extension = string.Empty;
                    string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

                    byte[] bytes = repViewer.ServerReport.Render("Image", null, out mimeType, out encoding, out extension, out streamIds, out warnings);

                    System.IO.File.WriteAllBytes(path + "/sorties_" + v.IdBS + "_A.jpg", bytes);

                    Printing(path + "/sorties_" + v.IdBS + "_A.jpg");

                    parameters[1] = new Microsoft.Reporting.WinForms.ReportParameter("FilGran", "SECURITE");
                    repViewer.ServerReport.SetParameters(parameters);
                    repViewer.ServerReport.Refresh();
                    repViewer.RefreshReport();
                    repViewer.ServerReport.SetParameters(parameters);
                    repViewer.ServerReport.Refresh();
                    repViewer.RefreshReport();

                    byte[] bytes2 = repViewer.ServerReport.Render("Image", null, out mimeType, out encoding, out extension, out streamIds, out warnings);

                    System.IO.File.WriteAllBytes(path + "/sorties_" + v.IdBS + "_S.jpg", bytes2);

                    Printing(path + "/sorties_" + v.IdBS + "_S.jpg");

                    parameters[1] = new Microsoft.Reporting.WinForms.ReportParameter("FilGran", "CLIENT");
                    repViewer.ServerReport.SetParameters(parameters);
                    repViewer.ServerReport.Refresh();
                    repViewer.RefreshReport();
                    repViewer.ServerReport.SetParameters(parameters);
                    repViewer.ServerReport.Refresh();
                    repViewer.RefreshReport();

                    byte[] bytes3 = repViewer.ServerReport.Render("Image", null, out mimeType, out encoding, out extension, out streamIds, out warnings);

                    System.IO.File.WriteAllBytes(path + "/sorties_" + v.IdBS + "_C.jpg", bytes3);

                    Printing(path + "/sorties_" + v.IdBS + "_C.jpg");

                    //supprimer les fichier
                    System.IO.File.Delete(path + "/sorties_" + v.IdBS + "_A.jpg");
                    System.IO.File.Delete(path + "/sorties_" + v.IdBS + "_S.jpg");

                    //System.IO.File.Delete(path + "/sorties_" + v.IdBS + "_C.jpg");
                //}   
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally {
                btnImprimerBonSortie.IsEnabled = true;
                Cursor = Cursors.Arrow;
            }
        }

        #region report printing AH 17 janv17

        private void Printing(string filePath)
        {
            try
            {
                filepath = filePath;
                try
                {
                    PrintDocument pd = new PrintDocument();
                    PrintController printController = new StandardPrintController();
                    pd.PrintController = printController;
                    pd.DefaultPageSettings.Margins = new Margins(0, 0, 0, 0);
                    pd.PrinterSettings.DefaultPageSettings.Margins = new Margins(0, 0, 0, 0);
                    pd.PrintPage += PrintPage;
                    pd.Print();
                }
                finally
                {

                    
                }

               /* ProcessStartInfo info = new ProcessStartInfo(filePath); 
                info.Verb = "Print"; 
                info.CreateNoWindow = false; 
                info.WindowStyle = ProcessWindowStyle.Hidden; 
                Process.Start(info);
                */
             

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void PrintPage(object o, PrintPageEventArgs e)
        {
            System.Drawing.Image img = System.Drawing.Image.FromFile(filepath);
            /*System.Drawing.Point loc = new System.Drawing.Point(12, 12);
            //e.Graphics.DrawImage(img, e.MarginBounds);
              e.Graphics.DrawImage(img, loc); */ 

            System.Drawing.Rectangle m = e.MarginBounds; 
            if ((double)img.Width / (double)img.Height > (double)m.Width / (double)m.Height) // image is wider
            {
                m.Height = (int)((double)img.Height / (double)img.Width * (double)m.Width);
            }
            else
            {
                m.Width = (int)((double)img.Width / (double)img.Height * (double)m.Height);
            }
             
            //Putting image in center of page.
            m.Y = (int)((((System.Drawing.Printing.PrintDocument)(o)).DefaultPageSettings.PaperSize.Height - m.Height) / 2);
            m.X = (int)((((System.Drawing.Printing.PrintDocument)(o)).DefaultPageSettings.PaperSize.Width - m.Width) / 2);
            e.Graphics.DrawImage(img, m);

        }

        private void print_callback()
        {
            System.IO.File.Delete(filepath);
        }

        private void pd_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs ev)
        {
            float linesPerPage = 0;
            float yPos = 0;
            int count = 0;
            float leftMargin = ev.MarginBounds.Left;
            float topMargin = ev.MarginBounds.Top;
            String line = null;

            // Calculate the number of lines per page.
            linesPerPage = ev.MarginBounds.Height /
               printFont.GetHeight(ev.Graphics);

            // Iterate over the file, printing each line.
            while (count < linesPerPage &&
               ((line = streamToPrint.ReadLine()) != null))
            {
                yPos = topMargin + (count * printFont.GetHeight(ev.Graphics));
                ev.Graphics.DrawString(line, printFont, System.Drawing.Brushes.Black,
                   leftMargin, yPos, new System.Drawing.StringFormat());
                count++;
            }

            // If more lines exist, print another page.
            if (line != null)
                ev.HasMorePages = true;
            else
                ev.HasMorePages = false;
        }


        #endregion
       
        private void btnAjoutNote_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                NoteForm noteForm = new NoteForm("Nouveau", this, utilisateur);
                noteForm.Title = "Création de note - Vehicule " + cbNumChassis.Text;
                noteForm.lblStatut.Content = "Note crée par : " + utilisateur.NU;
                noteForm.ShowDialog();
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        private void btnDuplicataBonSortie_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                vsomAcc = new VSOMAccessors();

                VEHICULE v = vsomAcc.GetVehiculeByIdVeh(Convert.ToInt32(txtIdChassis.Text));

                DuplicataSortieReport duplicataSortieReport = new DuplicataSortieReport(this);
                duplicataSortieReport.Title = "Impression du duplicata du bon de sortie : " + v.IdBS + " - Escale : " + v.ESCALE.NumEsc.ToString();
                duplicataSortieReport.Show();
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        private void txtPoids_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void btnAP_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AttelerVehiculeForm vehAPForm = new AttelerVehiculeForm(this, utilisateur);
                vehAPForm.cbNumChassisAP.IsEnabled = false;
                vehAPForm.Title = "Atteler/Porter - Chassis N° : " + cbNumChassis.Text;
                vehAPForm.ShowDialog();
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
               // using (var ctx = new VSOMClassesDataContext())
                //{
                    vsomAcc = new VSOMAccessors();

                    VEHICULE veh = vsomAcc.GetVehiculeByIdVeh(Convert.ToInt32(txtIdChassis.Text));

                    VAEForm vaeForm = new VAEForm(this, veh, utilisateur);
                    formLoader.LoadConnaissementForm(vaeForm, veh.CONNAISSEMENT);
                    vaeForm.Title = "Vente aux enchères - Chassis N° : " + cbNumChassis.Text;
                    vaeForm.ShowDialog();
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

        private void dataGridEltsFact_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (dataGridEltsFact.SelectedIndex != -1)
                {
                    UpdateEltVehForm updateEltForm = new UpdateEltVehForm(this, Convert.ToInt32(txtIdChassis.Text), utilisateur);
                    updateEltForm.cbElt.SelectedItem = ((ElementFacturation)dataGridEltsFact.SelectedItem).LibArticle;
                    updateEltForm.Title = "Eléments de facturation - Chassis N° " + cbNumChassis.Text;
                    updateEltForm.ShowDialog();
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

        private void btnRollBackStationnement_Click_1(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Confirmez-vous l'annulation des stationnements impayé ?", "Annulation Séjour", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    vsomAcc = new VSOMAccessors();
                    VEHICULE veh = vsomAcc.GetVehiculeByIdVeh(Convert.ToInt32(txtIdChassis.Text));
                    DateTime _date = vsomAcc.AnnulerSejourImpaye(Convert.ToInt32(txtIdChassis.Text), utilisateur);
                    MessageBox.Show("Le séjour du véhicule est annulé au " + _date);
                    formLoader.LoadVehiculeForm(this, veh);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Echec de l'opération : \n " + ex.Message, "Annulation séjour");
                }
            }
            

        }

        private void btnCalculerSejour2017_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                CalculerSejourVehiculeForm calcForm = new CalculerSejourVehiculeForm(this, utilisateur,"old");
                calcForm.Title = "Calcul du séjour séjour 2017 - Chassis N° : " + cbNumChassis.Text;
                calcForm.ShowDialog();
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Calcule stationnement");
            }
        }
    }
}
