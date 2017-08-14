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
    /// Logique d'interaction pour HistConnaissementForm.xaml
    /// </summary>
    public partial class HistConnaissementForm : Window
    {
        public List<CONNAISSEMENT> connaissements { get; set; }
        public List<string> cons { get; set; }

        public List<HIST_CONNAISSEMENT> histConnaissements { get; set; }
        public List<Int32> histCons { get; set; }

        private List<CLIENT> clients;
        public List<string> clts { get; set; }

        private List<ESCALE> escales;
        public List<Int32> escs { get; set; }

        private List<MANIFESTE> manifestes;
        public List<Int32> manifs { get; set; }

        public List<VEHICULE> vehicules { get; set; }
        public List<CONTENEUR> conteneurs { get; set; }
        public List<CONVENTIONNEL> conventionnels { get; set; }
        public List<ElementFacturation> eltsFact { get; set; }
        public List<PROFORMA> proformas { get; set; }
        public List<FACTURE> factures { get; set; }
        public List<PAYEMENT> paiements { get; set; }
        public List<OPERATION_CONNAISSEMENT> statuts { get; set; }
        public StatutLoadUnload statutDechargement { get; set; }

        private List<OPERATION> operationsUser;
        private UTILISATEUR utilisateur;

        public ConnaissementForm conForm { get; set; }
        private VsomParameters vsp = new VsomParameters();
        public HistConnaissementForm(ConnaissementForm form, CONNAISSEMENT con, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;
                InitializeCombos();

                conForm = form;
                utilisateur = user;
                operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);

                histConnaissements = vsp.GetHistConnaissement(con.IdBL);
                histCons = new List<Int32>();
                foreach (HIST_CONNAISSEMENT histBL in histConnaissements)
                {
                    histCons.Add(histBL.IdHistBL);
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

        private void InitializeCombos()
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();
                VsomParameters vsp = new VsomParameters();
                clients = vsp.GetClientsActifs();
                clts = new List<string>();
                foreach (CLIENT cl in clients)
                {
                    clts.Add(cl.NomClient);
                }

                //escales = vsp.GetEscalesByStatut("En cours");
                //escs = new List<Int32>();
                //foreach (ESCALE esc in escales)
                //{
                //    escs.Add(esc.NumEsc.Value);
                //}

                //manifestes = vsomAcc.GetManifestes();
                //manifs = new List<Int32>();
                //foreach (MANIFESTE man in manifestes)
                //{
                //    manifs.Add(man.IdMan);
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
                txtNumVoyage.Text = escales.ElementAt<ESCALE>(cbEscale.SelectedIndex).NumVoySCR;
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
                txtCodePort.Text = manifestes.ElementAt<MANIFESTE>(cbManifeste.SelectedIndex).CodePort;
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
            
        }

        private void btnFermer_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void cbIdHistBL_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                if (cbIdHistBL.Items.Count != 0)
                {
                    int indexHist = cbIdHistBL.SelectedIndex;
                    HIST_CONNAISSEMENT histBL = histConnaissements.ElementAt<HIST_CONNAISSEMENT>(indexHist);

                    txtNumBL.Text = histBL.NumBL;

                    cbClient.SelectedItem = histBL.CLIENT.NomClient;
                    escales = new List<ESCALE>();
                    escales.Add(histBL.ESCALE);
                    escs = new List<Int32>();
                    foreach (ESCALE esc in escales)
                    {
                        escs.Add(esc.NumEsc.Value);
                    }
                    cbEscale.ItemsSource = null;
                    cbEscale.ItemsSource = escs;
                    cbEscale.SelectedItem = histBL.ESCALE.NumEsc;

                    manifestes = new List<MANIFESTE>();
                    manifs = new List<Int32>();
                    manifestes.Add(histBL.MANIFESTE);
                    foreach (MANIFESTE man in manifestes)
                    {
                        manifs.Add(man.IdMan);
                    }
                    cbManifeste.ItemsSource = null;
                    cbManifeste.ItemsSource = manifs;
                    cbManifeste.SelectedItem = histBL.MANIFESTE.IdMan;

                    txtPortProv.Text = histBL.LPBL;
                    cbDestFinale.Text = histBL.DestBL;
                    txtConsignee.Text = histBL.ConsigneeBL;
                    txtAdresse.Text = histBL.AdresseBL;
                    txtNotify.Text = histBL.NotifyBL;

                    if (histBL.BLIL.Equals("Y"))
                    {
                        chkHinterland.IsChecked = true;
                    }
                    else
                    {
                        chkHinterland.IsChecked = false;
                    }

                    if (histBL.BLGN.Equals("Y"))
                    {
                        chkGN.IsChecked = true;
                    }
                    else
                    {
                        chkGN.IsChecked = false;
                    }

                    if (histBL.BLLT.Equals("Y"))
                    {
                        chkLineTerm.IsChecked = true;
                    }
                    else
                    {
                        chkLineTerm.IsChecked = false;
                    }

                    if (histBL.BLFO.Equals("Y"))
                    {
                        chkFreeOut.IsChecked = true;
                    }
                    else
                    {
                        chkFreeOut.IsChecked = false;
                    }

                    if (histBL.BlBloque.Equals("Y"))
                    {
                        chkBLBloque.IsChecked = true;
                    }
                    else
                    {
                        chkBLBloque.IsChecked = false;
                    }

                    if (histBL.BLSocar.Equals("Y"))
                    {
                        chkBLSOCAR.IsChecked = true;
                        txtNumSocar.IsEnabled = true;
                        txtNumSocar.Text = histBL.NumSocar;
                    }
                    else
                    {
                        chkBLSOCAR.IsChecked = false;
                        txtNumSocar.IsEnabled = false;
                        txtNumSocar.Text = "";
                    }

                    if (histBL.CCBL.Equals("Y"))
                    {
                        chkFretCollecter.IsChecked = true;
                        txtFretCollecter.IsEnabled = true;
                        txtFretCollecter.Text = histBL.CCBLMontant.ToString();
                    }
                    else
                    {
                        chkFretCollecter.IsChecked = false;
                        txtFretCollecter.IsEnabled = false;
                        txtFretCollecter.Text = "0";
                    }

                    if (histBL.BLDette.Equals("Y"))
                    {
                        chkDetteCollecter.IsChecked = true;
                        txtDetteCollecter.IsEnabled = true;
                        txtDetteCollecter.Text = histBL.DetteMontant.ToString();
                    }
                    else
                    {
                        chkDetteCollecter.IsChecked = false;
                        txtDetteCollecter.IsEnabled = false;
                        txtDetteCollecter.Text = "0";
                    }

                    if (histBL.BLER.Equals("Y"))
                    {
                        chkBLER.IsChecked = true;
                        //txtBLEL.Visibility = System.Windows.Visibility.Visible;
                        txtBLER.Text = histBL.BLERNote;
                    }
                    else
                    {
                        chkBLER.IsChecked = false;
                        //txtBLEL.Visibility = System.Windows.Visibility.Hidden;
                        txtBLER.Text = "";
                    }

                    conventionnels = histBL.CONNAISSEMENT.CONVENTIONNEL.ToList<CONVENTIONNEL>();
                    if (conventionnels.Count == 0)
                    {
                        gcTab.Visibility = System.Windows.Visibility.Collapsed;
                    }
                    else
                    {
                        dataGridGC.ItemsSource = conventionnels;
                        gcTab.Visibility = System.Windows.Visibility.Visible;
                        gcTab.IsSelected = true;
                    }

                    conteneurs = histBL.CONNAISSEMENT.CONTENEUR.ToList<CONTENEUR>();
                    if (conteneurs.Count == 0)
                    {
                        contTab.Visibility = System.Windows.Visibility.Collapsed;
                    }
                    else
                    {
                        dataGridConteneurs.ItemsSource = conteneurs;
                        contTab.Visibility = System.Windows.Visibility.Visible;
                        contTab.IsSelected = true;
                    }

                    vehicules = histBL.CONNAISSEMENT.VEHICULE.ToList<VEHICULE>();
                    if (vehicules.Count == 0)
                    {
                        vehTab.Visibility = System.Windows.Visibility.Collapsed;
                    }
                    else
                    {
                        dataGridVehicules.ItemsSource = vehicules;
                        vehTab.Visibility = System.Windows.Visibility.Visible;
                        vehTab.IsSelected = true;
                    }

                    listStatuts.ItemsSource = vsp.GetStatutsConnaissement(histBL.CONNAISSEMENT.IdBL);

                    dataGridCompteBL.ItemsSource = vsp.GetCompteBL(histBL.CONNAISSEMENT.IdBL);

                    //Valeur du BL
                    eltsFact = vsp.GetElementFacturationBL(histBL.CONNAISSEMENT.IdBL);
                    dataGridEltsFact.ItemsSource = eltsFact;
                    montantHTCpteBL.Content = eltsFact.Sum(elt => elt.PrixUnitaire * elt.Qte);
                    montantTVACpteBL.Content = eltsFact.Sum(elt => elt.MontantTVA);
                    montantTTCCpteBL.Content = eltsFact.Sum(elt => elt.MontantTTC);

                    // statut de déchargement
                    statutDechargement = vsp.GetStatutDechargementBL(histBL.CONNAISSEMENT.IdBL);
                    nbVeh.Content = statutDechargement.NbVehicule + "/" + histBL.CONNAISSEMENT.VEHICULE.Count;
                    nbCont.Content = statutDechargement.NbConteneur + "/" + histBL.CONNAISSEMENT.CONTENEUR.Count;
                    nbCargos.Content = statutDechargement.NbConventionnel + "/" + histBL.CONNAISSEMENT.CONVENTIONNEL.Count;

                    proformas = vsp.GetProformasOfConnaissement(histBL.CONNAISSEMENT.IdBL);
                    if (proformas.Count == 0)
                    {
                        profTab.Visibility = System.Windows.Visibility.Collapsed;
                    }
                    else
                    {
                        dataGridProfs.ItemsSource = proformas;
                        profTab.Visibility = System.Windows.Visibility.Visible;
                    }

                    lblStatut.Content = "Dernière modification effectuée par : " + histBL.UTILISATEUR.NU;

                    this.Title = "BL - " + histBL.NumBL + " - Consignée - " + histBL.ConsigneeBL;
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
