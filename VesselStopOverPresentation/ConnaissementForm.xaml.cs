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
    /// Logique d'interaction pour ConnaissementForm.xaml
    /// </summary>
    public partial class ConnaissementForm : Window
    {
        public List<CONNAISSEMENT> connaissements { get; set; }
        public List<string> cons { get; set; }

        private List<CLIENT> clients;
        public List<string> clts { get; set; }

        public List<ESCALE> escales { get; set; }
        public List<Int32> escs { get; set; }

        public List<MANIFESTE> manifestes { get; set; }
        public List<Int32> manifs { get; set; }

        public List<VEHICULE> vehicules { get; set; }
        public List<CONTENEUR> conteneurs { get; set; }
        public List<CONVENTIONNEL> conventionnels { get; set; }
        public List<MAFI> mafis { get; set; }
        public List<ElementFacturation> eltsFact { get; set; }
        public List<PROFORMA> proformas { get; set; }
        public List<PAYEMENT> paiements { get; set; }
        public List<OPERATION_CONNAISSEMENT> statuts { get; set; }
        public StatutLoadUnload statutDechargement { get; set; }

        private List<OPERATION> operationsUser;
        private UTILISATEUR utilisateur;

        private ManifesteForm manifForm;
        private ClientForm cltForm;
        public ConnaissementPanel conPanel { get; set; }

        private string typeForm = "";

        private FormLoader formLoader;
       // private VsomParameters vsp = new VsomParameters();
        private VSOMAccessors vsomAcc;
        public ConnaissementForm(ManifesteForm form, CONNAISSEMENT con, UTILISATEUR user)
        {
            try
            {
                InitializeComponent();
                //using (var ctx = new VSOMClassesDataContext())
               // {
                    //VSOMAccessors vs = new VSOMAccessors(ctx);
                    vsomAcc = new VSOMAccessors();
                    //VsomParameters vsparam = new VsomParameters();

                    
                    this.DataContext = this;
                    if (con.StatutBL == "Del")
                    {
                        throw new ApplicationException("Ce connaissement n'est pas disponible pour traitement");
                    }
                    InitializeCombos();

                    manifForm = form;
                    utilisateur = user;
                    operationsUser = vsomAcc.GetOperationsUtilisateur(utilisateur.IdU);

                    formLoader = new FormLoader(utilisateur);

                    formLoader.LoadConnaissementForm(this, con);

                    typeForm = "";
              //  }
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Echec du chargement " + ex.Message, "Chargement connaissement");
            }
            
        }

        public ConnaissementForm(ClientForm form, CONNAISSEMENT con, UTILISATEUR user)
        {
            try
            {
                InitializeComponent();
                //using (var ctx = new VSOMClassesDataContext())
                //{
                   // VSOMAccessors vs = new VSOMAccessors(ctx);
                    vsomAcc = new VSOMAccessors();

                    
                    this.DataContext = this;
                    if (con.StatutBL == "Del")
                    {
                        throw new ApplicationException("Ce connaissement n'est pas disponible pour traitement");
                    }
                    InitializeCombos();

                    cltForm = form;
                    utilisateur = user;
                    operationsUser = vsomAcc.GetOperationsUtilisateur(utilisateur.IdU);

                    formLoader = new FormLoader(utilisateur);

                    formLoader.LoadConnaissementForm(this, con);

                    typeForm = "";
                //}
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Echec du chargement " + ex.Message, "Chargement connaissement");
            }
            
        }

        public ConnaissementForm(ConnaissementPanel panel, CONNAISSEMENT con, UTILISATEUR user)
        {
            try
            {

                InitializeComponent();
               // using (var ctx = new VSOMClassesDataContext())
               // {
                    //VSOMAccessors vs = new VSOMAccessors(ctx);
                    vsomAcc = new VSOMAccessors();

                    this.DataContext = this;

                    if (con.StatutBL == "Del")
                    {
                        throw new ApplicationException("Ce connaissement n'est pas disponible pour traitement");
                    }
                    InitializeCombos();

                    conPanel = panel;
                    utilisateur = user;
                    operationsUser = vsomAcc.GetOperationsUtilisateur(utilisateur.IdU);

                    formLoader = new FormLoader(utilisateur);

                    formLoader.LoadConnaissementForm(this, con);

                    typeForm = "";
                   
                    
                //}
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show( ex.Message + "\n" + ex.StackTrace, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
        }

        public ConnaissementForm(string type, ConnaissementPanel panel, UTILISATEUR user)
        {
            try
            {

                InitializeComponent();
                //using (var ctx = new VSOMClassesDataContext())
                //{
                    //VSOMAccessors vs = new VSOMAccessors(ctx);
                    vsomAcc = new VSOMAccessors();

                    this.DataContext = this;
                    InitializeCombos();

                    typeForm = type;

                    conPanel = panel;
                    utilisateur = user;
                    operationsUser = vsomAcc.GetOperationsUtilisateur(utilisateur.IdU);

                    formLoader = new FormLoader(utilisateur);
                //}
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Echec du chargement " + ex.Message, "Chargement connaissement");
            }
            
        }

        private void InitializeCombos()
        {
            try
            {
                VsomParameters vsparam = new VsomParameters();

                clients = vsparam.GetClientsActifs();
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
                MessageBox.Show("Echec du chargement " + ex.Message, "Chargement connaissement");
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

        private void txtFretCollecter_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            //[^0-9.-]+
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void txtDetteCollecter_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            //[^0-9.-]+
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void btnAccomplissement_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //using (var ctx = new VSOMClassesDataContext())
                //{
                    vsomAcc = new VSOMAccessors();

                    CONNAISSEMENT con = vsomAcc.GetConnaissementByIdBL(Convert.ToInt32(txtIdBL.Text));
                    AccomplissementForm accomplissementForm = new AccomplissementForm(this, con, utilisateur);
                    accomplissementForm.Title = "Accomplissement BL N° " + con.NumBL;
                    accomplissementForm.ShowDialog();
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

        private void chkHinterland_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                // vsomAcc = new VSOMAccessors();

                chkGN.IsChecked = false;
                cbDestFinale.Text = "HINT";
                if (chkBLSOCAR.IsChecked == true)
                {
                    eltsFact.RemoveAll(el => el.LibArticle.Contains("Débours SOCAR : BESC"));
                    dataGridEltsFact.ItemsSource = null;
                    dataGridEltsFact.ItemsSource = eltsFact;
                    montantHTCpteBL.Content = eltsFact.Sum(elt => elt.PrixUnitaire * elt.Qte);
                    montantTVACpteBL.Content = eltsFact.Sum(elt => elt.MontantTVA);
                    montantTTCCpteBL.Content = eltsFact.Sum(elt => elt.MontantTTC);
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

        private void chkHinterland_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //using (var ctx = new VSOMClassesDataContext())
                //{
                    vsomAcc = new VSOMAccessors();

                    if (chkHinterland.IsChecked == false && chkBLSOCAR.IsChecked == true)
                    {
                        eltsFact.AddRange(vsomAcc.GetElementSOCARBESCBL(Convert.ToInt32(txtIdBL.Text)));
                        dataGridEltsFact.ItemsSource = null;
                        dataGridEltsFact.ItemsSource = eltsFact;
                        montantHTCpteBL.Content = eltsFact.Sum(elt => elt.PrixUnitaire * elt.Qte);
                        montantTVACpteBL.Content = eltsFact.Sum(elt => elt.MontantTVA);
                        montantTTCCpteBL.Content = eltsFact.Sum(elt => elt.MontantTTC);
                    }
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

        private void chkGN_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
               // using (var ctx = new VSOMClassesDataContext())
               // {
                    vsomAcc = new VSOMAccessors();

                    chkHinterland.IsChecked = false;
                    cbDestFinale.Text = "CMR";
                    if (chkBLSOCAR.IsChecked == true && chkHinterland.IsChecked == true)
                    {
                        eltsFact.AddRange(vsomAcc.GetElementSOCARBESCBL(Convert.ToInt32(txtIdBL.Text)));
                        dataGridEltsFact.ItemsSource = null;
                        dataGridEltsFact.ItemsSource = eltsFact;
                        montantHTCpteBL.Content = eltsFact.Sum(elt => elt.PrixUnitaire * elt.Qte);
                        montantTVACpteBL.Content = eltsFact.Sum(elt => elt.MontantTVA);
                        montantTTCCpteBL.Content = eltsFact.Sum(elt => elt.MontantTTC);
                    }
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

        private void chkLineTerm_Checked(object sender, RoutedEventArgs e)
        {
            chkFreeOut.IsChecked = false;
        }

        private void chkFreeOut_Checked(object sender, RoutedEventArgs e)
        {
            chkLineTerm.IsChecked = false;
        }

        private void chkBLER_Click(object sender, RoutedEventArgs e)
        {
           // using (var ctx = new VSOMClassesDataContext())
           // {
                vsomAcc = new VSOMAccessors();

                if (chkBLER.IsChecked == true)
                {
                    txtBLER.IsEnabled = true;
                }
                else
                {
                    txtBLER.Text = "";
                    txtBLER.IsEnabled = false;
                }

                if (chkBLER.IsChecked == true)
                {
                    eltsFact.Add(vsomAcc.GetElementFraisReleaseBL(Convert.ToInt32(txtIdBL.Text)));
                    dataGridEltsFact.ItemsSource = null;
                    dataGridEltsFact.ItemsSource = eltsFact;
                    montantHTCpteBL.Content = eltsFact.Sum(elt => elt.PrixUnitaire * elt.Qte);
                    montantTVACpteBL.Content = eltsFact.Sum(elt => elt.MontantTVA);
                    montantTTCCpteBL.Content = eltsFact.Sum(elt => elt.MontantTTC);
                }
                else
                {
                    eltsFact.RemoveAll(el => el.LibArticle.Contains("BL Express Release Fee"));
                    dataGridEltsFact.ItemsSource = null;
                    dataGridEltsFact.ItemsSource = eltsFact;
                    montantHTCpteBL.Content = eltsFact.Sum(elt => elt.PrixUnitaire * elt.Qte);
                    montantTVACpteBL.Content = eltsFact.Sum(elt => elt.MontantTVA);
                    montantTTCCpteBL.Content = eltsFact.Sum(elt => elt.MontantTTC);
                }
                finTab.IsSelected = true;
           // }
        }

        private void chkBLSOCAR_Click(object sender, RoutedEventArgs e)
        {
            try
            {
               // VSOMAccessors vsomAcc = new VSOMAccessors();

                if (chkBLSOCAR.IsChecked == true)
                {
                    //chkFretCollecter.IsEnabled = true;
                    chkDetteCollecter.IsEnabled = true;
                    txtNumSocar.IsEnabled = true;
                }
                else
                {
                    //chkFretCollecter.IsEnabled = false;
                    chkDetteCollecter.IsEnabled = false;
                    //chkFretCollecter.IsChecked = false;
                    chkDetteCollecter.IsChecked = false;
                    //txtFretCollecter.Text = "0";
                    txtDetteCollecter.Text = "0";
                    txtNumSocar.Text = "";
                    txtNumSocar.IsEnabled = false;
                }

                if (chkBLSOCAR.IsChecked == true && chkHinterland.IsChecked == false)
                {
                    eltsFact.AddRange(vsomAcc.GetElementSOCARBESCBL(Convert.ToInt32(txtIdBL.Text)));
                    dataGridEltsFact.ItemsSource = null;
                    dataGridEltsFact.ItemsSource = eltsFact;
                    montantHTCpteBL.Content = eltsFact.Sum(elt => elt.PrixUnitaire * elt.Qte);
                    montantTVACpteBL.Content = eltsFact.Sum(elt => elt.MontantTVA);
                    montantTTCCpteBL.Content = eltsFact.Sum(elt => elt.MontantTTC);
                }
                else
                {
                    eltsFact.RemoveAll(el => el.LibArticle.Contains("Débours SOCAR : BESC"));
                    dataGridEltsFact.ItemsSource = null;
                    dataGridEltsFact.ItemsSource = eltsFact;
                    montantHTCpteBL.Content = eltsFact.Sum(elt => elt.PrixUnitaire * elt.Qte);
                    montantTVACpteBL.Content = eltsFact.Sum(elt => elt.MontantTVA);
                    montantTTCCpteBL.Content = eltsFact.Sum(elt => elt.MontantTTC);
                }
                finTab.IsSelected = true;
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        private void chkFretCollecter_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (chkFretCollecter.IsChecked == true)
                {
                    txtFretCollecter.IsEnabled = true;
                    txtFretCollecter.Text = "0";
                }
                else
                {
                    txtFretCollecter.IsEnabled = false;
                    txtFretCollecter.Text = "0";
                    eltsFact.RemoveAll(el => el.LibArticle.Contains("Débours SOCAR : Fret à collecter") || el.LibArticle.Contains("Débours : Fret à collecter") || el.LibArticle.Contains("Commission sur Fret à collecter"));
                    dataGridEltsFact.ItemsSource = null;
                    dataGridEltsFact.ItemsSource = eltsFact;
                    montantHTCpteBL.Content = eltsFact.Sum(elt => elt.PrixUnitaire * elt.Qte);
                    montantTVACpteBL.Content = eltsFact.Sum(elt => elt.MontantTVA);
                    montantTTCCpteBL.Content = eltsFact.Sum(elt => elt.MontantTTC);
                }
                finTab.IsSelected = true;
                
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        private void txtFretCollecter_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                vsomAcc = new VSOMAccessors();

                if (chkFretCollecter.IsChecked == true && txtFretCollecter.Text.Trim() != "0")
                {
                    eltsFact.RemoveAll(elt => elt.LibArticle.Contains("Fret à collecter"));
                    eltsFact.AddRange(vsomAcc.GetElementSOCARFretBL(Convert.ToInt32(txtIdBL.Text), chkBLSOCAR.IsChecked == true ? "SOCAR" : "NON SOCAR", Convert.ToDouble(txtFretCollecter.Text)));
                    dataGridEltsFact.ItemsSource = null;
                    dataGridEltsFact.ItemsSource = eltsFact;
                    montantHTCpteBL.Content = eltsFact.Sum(elt => elt.PrixUnitaire * elt.Qte);
                    montantTVACpteBL.Content = eltsFact.Sum(elt => elt.MontantTVA);
                    montantTTCCpteBL.Content = eltsFact.Sum(elt => elt.MontantTTC);
                }
                finTab.IsSelected = true;
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        private void chkDetteCollecter_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (chkDetteCollecter.IsChecked == true)
                {
                    txtDetteCollecter.IsEnabled = true;
                    txtDetteCollecter.Text = "0";
                }
                else
                {
                    txtDetteCollecter.IsEnabled = false;
                    txtDetteCollecter.Text = "0";
                    eltsFact.RemoveAll(el => el.LibArticle.Contains("Débours SOCAR : Dette à collecter"));
                    dataGridEltsFact.ItemsSource = null;
                    dataGridEltsFact.ItemsSource = eltsFact;
                    montantHTCpteBL.Content = eltsFact.Sum(elt => elt.PrixUnitaire * elt.Qte);
                    montantTVACpteBL.Content = eltsFact.Sum(elt => elt.MontantTVA);
                    montantTTCCpteBL.Content = eltsFact.Sum(elt => elt.MontantTTC);
                }
                finTab.IsSelected = true;
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        private void txtDetteCollecter_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                 vsomAcc = new VSOMAccessors();

                if (chkDetteCollecter.IsChecked == true && txtDetteCollecter.Text.Trim() != "0")
                {
                    eltsFact.RemoveAll(elt => elt.LibArticle.Contains("Dette à collecter"));
                    eltsFact.Add(vsomAcc.GetElementSOCARDetteBL(Convert.ToInt32(txtIdBL.Text), Convert.ToDouble(txtDetteCollecter.Text)));
                    dataGridEltsFact.ItemsSource = null;
                    dataGridEltsFact.ItemsSource = eltsFact;
                    montantHTCpteBL.Content = eltsFact.Sum(elt => elt.PrixUnitaire * elt.Qte);
                    montantTVACpteBL.Content = eltsFact.Sum(elt => elt.MontantTVA);
                    montantTTCCpteBL.Content = eltsFact.Sum(elt => elt.MontantTTC);
                }
                finTab.IsSelected = true;
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
               // using (var ctx = new VSOMClassesDataContext())
               // {
                    vsomAcc = new VSOMAccessors();
                    //VsomMarchal vsomarchal = new VsomMarchal();

                    if (typeForm == "")
                    {
                        if (cbNumBL.Text.Trim() == "" && txtIdBL.Text == "")
                        {
                            MessageBox.Show("Veuillez sélectionner un connaissement", "N° de connaissement?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        }
                        else if (cbClient.SelectedIndex == -1)
                        {
                            MessageBox.Show("Veuillez sélectionner un client", "Client ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        }
                        else if (cbEscale.SelectedIndex == -1)
                        {
                            MessageBox.Show("Veuillez sélectionner une escale", "Escale ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        }
                        else if (txtConsignee.Text.Trim() == "")
                        {
                            MessageBox.Show("Veuillez sélectionner un consignee pour ce BL", "Consignée ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        }
                        else if (chkBLER.IsChecked == true && txtBLER.Text.Trim() == "")
                        {
                            MessageBox.Show("Vous devez absolument saisir les détails relatifs à l'Express Release.", "Note Express Release ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        }
                        else if (chkFretCollecter.IsChecked == true && txtFretCollecter.Text == "0")
                        {
                            MessageBox.Show("Vous devez absolument saisir le montant du fret à collecter sur ce connaissement.", "Fret à collecter ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        }
                        else if (chkDetteCollecter.IsChecked == true && txtDetteCollecter.Text == "0")
                        {
                            MessageBox.Show("Vous devez absolument saisir le montant de la dette à collecter sur ce connaissement.", "Dette à collecter ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        }
                        else
                        {
                            cbNumBL.Focus();
                            CONNAISSEMENT bl = vsomAcc.UpdateConnaissement(Convert.ToInt32(txtIdBL.Text), txtConsignee.Text, txtAdresse.Text, txtNotify.Text, txtEmail.Text, clients.ElementAt<CLIENT>(cbClient.SelectedIndex).IdClient, txtPortProv.Text, cbDestFinale.Text, txtNomChargeur.Text, txtAdresseChargeur.Text, (chkBLER.IsChecked == true ? "Y" : "N"), txtBLER.Text.Trim(), (chkGN.IsChecked == true ? "Y" : "N"), (chkHinterland.IsChecked == true ? "Y" : "N"), (chkFreeOut.IsChecked == true ? "Y" : "N"), (chkLineTerm.IsChecked == true ? "Y" : "N"), (chkBLBloque.IsChecked == true ? "Y" : "N"), (chkBLBloque.IsChecked == true ? txtBLBloqueNote.Text : ""), (chkFretCollecter.IsChecked == true ? "Y" : "N"), (chkFretCollecter.IsChecked == true ? Convert.ToInt32(txtFretCollecter.Text) : 0), (chkDetteCollecter.IsChecked == true ? "Y" : "N"), (chkDetteCollecter.IsChecked == true ? Convert.ToInt32(txtDetteCollecter.Text) : 0), (chkBLSOCAR.IsChecked == true ? "Y" : "N"), txtNumSocar.Text, utilisateur.IdU);
                            //Raffraîchir les informations
                            formLoader.LoadConnaissementForm(this, bl);

                            if (this.manifForm != null)
                            {
                                manifForm.manifestes = vsomAcc.GetManifestesOfEscale(bl.ESCALE.IdEsc);
                                manifForm.manifs = new List<Int32>();
                                foreach (MANIFESTE man in manifForm.manifestes)
                                {
                                    manifForm.manifs.Add(man.IdMan);
                                }
                                manifForm.cbIdMan.ItemsSource = null;
                                manifForm.cbIdMan.ItemsSource = manifForm.manifs;
                                manifForm.cbIdMan.SelectedItem = bl.IdMan;
                            }
                            else if (this.conPanel != null)
                            {
                                if (conPanel.cbFiltres.SelectedIndex != 2)
                                {
                                    conPanel.cbFiltres.SelectedIndex = 2;
                                }
                                else
                                {
                                    conPanel.connaissements = vsomAcc.GetConnaissementByStatut("Traité");
                                    conPanel.dataGrid.ItemsSource = conPanel.connaissements;
                                    conPanel.lblStatut.Content = conPanel.connaissements.Count + " Connaissement(s)";
                                }
                            }
                            MessageBox.Show("La modification apportée sur les informations du connaissement affiché ont été enregistrées, et les impacts sur la facturation pris en compte", "Modification validée !", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                    else if (typeForm == "Nouveau")
                    {
                        if (cbNumBL.Text.Trim() == "")
                        {
                            MessageBox.Show("Veuillez entrer un numéro de connaissement. Il est indispensable pour la création d'un BL", "N° de connaissement?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        }
                        else if (cbClient.SelectedIndex == -1)
                        {
                            MessageBox.Show("Veuillez sélectionner un client", "Client ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        }
                        else if (txtNumVoyage.Text == "")
                        {
                            MessageBox.Show("Veuillez sélectionner une escale", "Escale ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        }
                        else if (cbManifeste.SelectedIndex == -1)
                        {
                            MessageBox.Show("Veuillez sélectionner un manifeste", "Manifeste ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        }
                        else if (txtPortProv.Text.Trim() == "")
                        {
                            MessageBox.Show("Veuillez spécifier le port de provenance", "Port de provenance ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        }
                        else if (txtConsignee.Text.Trim() == "")
                        {
                            MessageBox.Show("Veuillez sélectionner un consignee pour ce BL", "Consignée ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        }
                        else if (txtAdresse.Text.Trim() == "")
                        {
                            MessageBox.Show("Veuillez sélectionner une adresse pour ce BL", "Adresse ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        }
                        else if (txtNotify.Text.Trim() == "")
                        {
                            MessageBox.Show("Veuillez sélectionner un notify pour ce BL", "Notify ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        }
                        else
                        {
                            cbNumBL.Focus();
                            CONNAISSEMENT bl = vsomAcc.InsertConnaissement(cbNumBL.Text, clients.ElementAt<CLIENT>(cbClient.SelectedIndex).IdClient, escales.ElementAt<ESCALE>(cbEscale.SelectedIndex).IdEsc, manifestes.ElementAt<MANIFESTE>(cbManifeste.SelectedIndex).IdMan, txtConsignee.Text, txtAdresse.Text, txtNotify.Text, txtEmail.Text, txtPortProv.Text, cbDestFinale.Text, txtNomChargeur.Text, txtAdresseChargeur.Text, (chkGN.IsChecked == true ? "Y" : "N"), (chkHinterland.IsChecked == true ? "Y" : "N"), (chkFreeOut.IsChecked == true ? "Y" : "N"), (chkLineTerm.IsChecked == true ? "Y" : "N"), utilisateur.IdU);
                            //Raffraîchir les informations
                            formLoader.LoadConnaissementForm(this, bl);
                            typeForm = "";

                            if (this.conPanel != null)
                            {
                                if (conPanel.cbFiltres.SelectedIndex != 1)
                                {
                                    conPanel.cbFiltres.SelectedIndex = 1;
                                }
                                else
                                {
                                    conPanel.connaissements = vsomAcc.GetConnaissementByStatut("Non initié");
                                    conPanel.dataGrid.ItemsSource = conPanel.connaissements;
                                    conPanel.lblStatut.Content = conPanel.connaissements.Count + " Connaissement(s)";
                                }
                            }
                            MessageBox.Show("Le connaissement N° " + cbNumBL.Text + " a été crée avec succès", "Connaissement crée !", MessageBoxButton.OK, MessageBoxImage.Information);
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
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
            }
            
        }

        private void btnValidBL_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ValiderConnaissementForm validForm = new ValiderConnaissementForm(this, utilisateur);
                validForm.Title = "Validation du connaissement : " + cbNumBL.Text + " - Escale " + cbEscale.Text;
                validForm.ShowDialog();
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        private void dataGridVehicules_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (dataGridVehicules.SelectedIndex != -1)
                {
                    VehiculeForm vehForm = new VehiculeForm(this, (VEHICULE)dataGridVehicules.SelectedItem, utilisateur);
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

        private void dataGridConteneurs_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (dataGridConteneurs.SelectedIndex != -1)
                {
                    ConteneurForm contForm = new ConteneurForm(this, (CONTENEUR)dataGridConteneurs.SelectedItem, utilisateur);
                    contForm.Show();
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

                    if (e.Key == Key.Return && cbNumBL.Text.Trim() != "" && typeForm == "")
                    {
                        connaissements = vsomAcc.GetConnaissementByNumBL(cbNumBL.Text.Trim());

                        if (connaissements.Count == 0)
                        {
                            MessageBox.Show("Il n'existe aucun connaissement en cours portant ce numéro", "Connaissement introuvable", MessageBoxButton.OK, MessageBoxImage.Exclamation);
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

        private void btnProforma_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //using (var ctx = new VSOMClassesDataContext())
               // {
                    vsomAcc = new VSOMAccessors();

                    if (operationsUser.Where(op => op.NomOp == "Proforma : Enregistrement d'un nouvel élément").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
                    {
                        MessageBox.Show("Vous n'avez pas les droits nécessaires pour créer une nouvelle proforma. Veuillez contacter un administrateur", "Droits insuffisants !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                    else
                    {
                        ProformaForm profForm = new ProformaForm("Nouveau", this, vsomAcc.GetConnaissementByIdBL(Convert.ToInt32(txtIdBL.Text)), utilisateur);
                        profForm.cbIdProf.IsEnabled = false;
                        profForm.Title = "Nouveau : Proforma";
                        profForm.borderActions.Visibility = System.Windows.Visibility.Collapsed;
                        profForm.borderEtat.Visibility = System.Windows.Visibility.Collapsed;
                        profForm.eltBorder.IsEnabled = false;
                        profForm.Show();
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

        private void chkBLBloque_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                txtBLBloqueNote.IsEnabled = chkBLBloque.IsChecked == true ? true : false;
                if (chkBLBloque.IsChecked == false)
                {
                    txtBLBloqueNote.Text = "";
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

        private void dataGridProfs_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
               // using (var ctx = new VSOMClassesDataContext())
               // {
                    vsomAcc = new VSOMAccessors();
                    if (dataGridProfs.SelectedIndex != -1)
                    {
                        ProformaForm profForm = new ProformaForm(this, vsomAcc.GetProformaByIdProf(((PROFORMA)dataGridProfs.SelectedItem).IdFP), utilisateur);
                        profForm.Show();
                    }
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

        private void listStatuts_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (listStatuts.SelectedIndex != -1)
                {
                    OPERATION_CONNAISSEMENT opBL = (OPERATION_CONNAISSEMENT)listStatuts.SelectedItem;
                    if (opBL.DateOp.HasValue)
                    {
                        if (opBL.TYPE_OPERATION.LibTypeOp == "Accompli")
                        {
                            //VSOMAccessors vsomAcc = new VSOMAccessors();

                            CONNAISSEMENT con = vsomAcc.GetConnaissementByIdBL(Convert.ToInt32(txtIdBL.Text));
                            AccomplissementForm accomplissementForm = new AccomplissementForm(this, con, utilisateur);
                            accomplissementForm.Title = "Accomplissement BL N° " + con.NumBL;
                            accomplissementForm.ShowDialog();
                        }
                        else
                        {
                            StatutForm statutForm = new StatutForm(opBL);
                            statutForm.Title = "Statut - " + opBL.TYPE_OPERATION.LibTypeOp;
                            statutForm.ShowDialog();
                        }
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

        private void dataGridCompteBL_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
               // using (var ctx = new VSOMClassesDataContext())
               // {
                    vsomAcc = new VSOMAccessors();
                    if (dataGridCompteBL.SelectedIndex != -1)
                    {
                        ElementCompte eltCompte = (ElementCompte)dataGridCompteBL.SelectedItem;
                        if (eltCompte.TypeDoc == "FA")
                        {
                            FactureForm factForm = new FactureForm(this, vsomAcc.GetFactureByIdFact(eltCompte.Id), utilisateur);
                            factForm.Show();
                        }
                        else if (eltCompte.TypeDoc == "PA")
                        {
                            PaiementForm payForm = new PaiementForm(this, vsomAcc.GetPaiementByIdPay(eltCompte.Id), utilisateur);
                            payForm.btnEnregistrer.IsEnabled = false;
                            payForm.Show();
                        }
                        else if (eltCompte.TypeDoc == "CN")
                        {
                            AvoirForm avoirForm = new AvoirForm(this, vsomAcc.GetAvoirByIdAvoir(eltCompte.Id), utilisateur);
                            avoirForm.btnEnregistrer.IsEnabled = false;
                            avoirForm.Show();
                        }
                    }
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

        private void btnHist_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //VSOMAccessors vsomAcc = new VSOMAccessors();

                HistConnaissementForm histForm = new HistConnaissementForm(this, vsomAcc.GetConnaissementByIdBL(Convert.ToInt32(txtIdBL.Text)), utilisateur);
                histForm.Title = "Historique des modifications - BL N° " + cbNumBL.Text;
                histForm.ShowDialog();
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        private void btnAjoutNote_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                NoteForm noteForm = new NoteForm("Nouveau", this, utilisateur);
                noteForm.Title = "Création de note - Connaissement - " + cbNumBL.Text;
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

        private void listNotes_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (listNotes.SelectedIndex != -1)
                {
                    NOTE note = (NOTE)listNotes.SelectedItem;
                    NoteForm noteForm = new NoteForm(this, note, utilisateur);
                    noteForm.Title = "Note - " + note.IdNote + " - BL - " + note.CONNAISSEMENT.NumBL;
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

        private void dataGridGC_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                 vsomAcc = new VSOMAccessors();
                CONVENTIONNEL conv = vsomAcc.GetConventionnelByIdGC(((CONVENTIONNEL)dataGridGC.SelectedItem).IdGC);
                if (dataGridGC.SelectedIndex != -1)
                {
                    ConventionnelForm convForm = new ConventionnelForm(this, conv, utilisateur);
                    convForm.Show();
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

        private void cbEscale_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                //using (var ctx = new VSOMClassesDataContext())
                //{
                    vsomAcc = new VSOMAccessors();

                    if (e.Key == Key.Return && cbEscale.Text.Trim() != "")
                    {
                        int result;
                        escales = vsomAcc.GetEscalesByNumEscale(Int32.TryParse(cbEscale.Text.Trim(), out result) ? result : -1);

                        if (escales.Count == 0)
                        {
                            MessageBox.Show("Il n'existe aucune escale portant ce numéro", "Escale introuvable", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        }
                        else if (escales.Count == 1)
                        {
                            ESCALE esc = escales.FirstOrDefault<ESCALE>();
                            formLoader.LoadEscaleForm(this, esc);
                        }
                        else
                        {
                            ListEscaleForm listEscForm = new ListEscaleForm(this, escales, utilisateur);
                            listEscForm.Title = "Choix multiples : Sélectionnez une escale";
                            listEscForm.ShowDialog();
                        }
                    }
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

        private void btnAnnuler_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CloturerConnaissementForm clotureForm = new CloturerConnaissementForm(this, utilisateur);
                clotureForm.Title = "Clôture du connaissement : " + cbNumBL.Text;
                clotureForm.ShowDialog();
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        private void dataGridMafi_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (dataGridMafi.SelectedIndex != -1)
                {
                    MafiForm mafiForm = new MafiForm(this, (MAFI)dataGridMafi.SelectedItem, utilisateur);
                    mafiForm.Show();
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

        private void btnExonererTVA_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ExonererConnaissementForm exonForm = new ExonererConnaissementForm(this, utilisateur);
                exonForm.Title = "Exonération de TVA - BL N° : " + cbNumBL.Text + " - Escale " + cbEscale.Text;
                exonForm.ShowDialog();
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
                    UpdateEltBLForm updateEltForm = new UpdateEltBLForm(this, Convert.ToInt32(txtIdBL.Text), utilisateur);
                    updateEltForm.cbElt.SelectedItem = ((ElementFacturation)dataGridEltsFact.SelectedItem).LibArticle;
                    updateEltForm.Title = "Eléments de facturation - BL N° " + cbNumBL.Text;
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
    }
}
