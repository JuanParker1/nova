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
    /// Logique d'interaction pour ProformaForm.xaml
    /// </summary>
    public partial class PaiementForm : Window
    {
        public List<CONNAISSEMENT> connaissements { get; set; }
        public List<string> cons { get; set; }

        private List<CLIENT> clients;
        public List<string> clts { get; set; }

        public List<ESCALE> escales { get; set; }
        public List<Int32> escs { get; set; }

        private List<PAYEMENT> paiements;
        public List<Int32> pays { get; set; }

        private List<PARAMETRE> modesPayement;
        public List<string> modePays { get; set; }

        private List<OPERATION> operationsUser;
        private UTILISATEUR utilisateur;

        public List<PROFORMA> proformas { get; set; }
        public List<FACTURE> factures { get; set; }

        private List<NAVIRE> navires;
        public List<string> navs { get; set; }

        private List<BANQUE> banques;
        public List<string> banks { get; set; }

        public List<VEHICULE> vehicules { get; set; }
        public List<CONTENEUR> conteneurs { get; set; }
        public List<CONVENTIONNEL> conventionnels { get; set; }

        private PaiementPanel payPanel;
        private FactureForm factForm;
        private ConnaissementForm conForm;
        private BookingForm bookForm;
        private ManifesteForm manForm;
        private EscaleForm escForm;

        private FormLoader formLoader;
        VSOMAccessors vsomAcc = new VSOMAccessors();
        VsomParameters vsp = new VsomParameters();
        public PaiementForm(PaiementPanel panel, PAYEMENT pay, UTILISATEUR user)
        {
            try
            {
                
                
                InitializeComponent();
                this.DataContext = this;

                clients = vsp.GetClientsActifs();
                clts = new List<string>();
                foreach (CLIENT clt in clients)
                {
                    clts.Add(clt.NomClient);
                }

                navires = vsp.GetNaviresActifs();
                navs = new List<string>();
                foreach (NAVIRE nav in navires)
                {
                    navs.Add(nav.NomNav);
                }

                banques = vsp.GetBanques();
                banks = new List<string>();
                foreach (BANQUE b in banques)
                {
                    banks.Add(b.CodeBanque);
                }

                modesPayement = vsp.GetModesPayement();
                modePays = new List<string>();
                foreach (PARAMETRE par in modesPayement)
                {
                    modePays.Add(par.NomAF);
                }

                payPanel = panel;

                utilisateur = user;
                operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);

                formLoader = new FormLoader(utilisateur);

                formLoader.LoadPaiementForm(this, pay);
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
            
        }

        public PaiementForm(ConnaissementForm form, PAYEMENT pay, UTILISATEUR user)
        {
            try
            {
                //VSOMAccessors vsomAcc = new VSOMAccessors();
                //VsomParameters vsp = new VsomParameters();
                InitializeComponent();
                this.DataContext = this;

                clients = vsp.GetClientsActifs();
                clts = new List<string>();
                foreach (CLIENT clt in clients)
                {
                    clts.Add(clt.NomClient);
                }

                navires = vsp.GetNaviresActifs();
                navs = new List<string>();
                foreach (NAVIRE nav in navires)
                {
                    navs.Add(nav.NomNav);
                }

                banques = vsp.GetBanques();
                banks = new List<string>();
                foreach (BANQUE b in banques)
                {
                    banks.Add(b.CodeBanque);
                }

                modesPayement = vsp.GetModesPayement();
                modePays = new List<string>();
                foreach (PARAMETRE par in modesPayement)
                {
                    modePays.Add(par.NomAF);
                }

                conForm = form;

                utilisateur = user;
                operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);

                formLoader = new FormLoader(utilisateur);

                formLoader.LoadPaiementForm(this, pay);
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }

        }

        public PaiementForm(FactureForm form, PAYEMENT pay, UTILISATEUR user)
        {
            try
            {
                //VSOMAccessors vsomAcc = new VSOMAccessors();
                //VsomParameters vsp = new VsomParameters();
                InitializeComponent();
                this.DataContext = this;

                clients = vsp.GetClientsActifs();
                clts = new List<string>();
                foreach (CLIENT clt in clients)
                {
                    clts.Add(clt.NomClient);
                }

                navires = vsp.GetNaviresActifs();
                navs = new List<string>();
                foreach (NAVIRE nav in navires)
                {
                    navs.Add(nav.NomNav);
                }

                banques = vsp.GetBanques();
                banks = new List<string>();
                foreach (BANQUE b in banques)
                {
                    banks.Add(b.CodeBanque);
                }

                modesPayement = vsp.GetModesPayement();
                modePays = new List<string>();
                foreach (PARAMETRE par in modesPayement)
                {
                    modePays.Add(par.NomAF);
                }

                factForm = form;

                utilisateur = user;
                operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);

                formLoader = new FormLoader(utilisateur);

                formLoader.LoadPaiementForm(this, pay);
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }

        }

        public PaiementForm(BookingForm form, PAYEMENT pay, UTILISATEUR user)
        {
            try
            {
                //VSOMAccessors vsomAcc = new VSOMAccessors();
                //VsomParameters vsp = new VsomParameters();
                InitializeComponent();
                this.DataContext = this;

                clients = vsp.GetClientsActifs();
                clts = new List<string>();
                foreach (CLIENT clt in clients)
                {
                    clts.Add(clt.NomClient);
                }

                banques = vsp.GetBanques();
                banks = new List<string>();
                foreach (BANQUE b in banques)
                {
                    banks.Add(b.CodeBanque);
                }

                navires = vsp.GetNaviresActifs();
                navs = new List<string>();
                foreach (NAVIRE nav in navires)
                {
                    navs.Add(nav.NomNav);
                }

                modesPayement = vsp.GetModesPayement();
                modePays = new List<string>();
                foreach (PARAMETRE par in modesPayement)
                {
                    modePays.Add(par.NomAF);
                }

                bookForm = form;

                utilisateur = user;
                operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);

                formLoader = new FormLoader(utilisateur);

                formLoader.LoadPaiementForm(this, pay);
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }

        }

        public PaiementForm(EscaleForm form, PAYEMENT pay, UTILISATEUR user)
        {
            try
            {
                //VSOMAccessors vsomAcc = new VSOMAccessors();
                //VsomParameters vsp = new VsomParameters();
                InitializeComponent();
                this.DataContext = this;

                clients = vsp.GetClientsActifs();
                clts = new List<string>();
                foreach (CLIENT clt in clients)
                {
                    clts.Add(clt.NomClient);
                }

                banques = vsp.GetBanques();
                banks = new List<string>();
                foreach (BANQUE b in banques)
                {
                    banks.Add(b.CodeBanque);
                }

                navires = vsp.GetNaviresActifs();
                navs = new List<string>();
                foreach (NAVIRE nav in navires)
                {
                    navs.Add(nav.NomNav);
                }

                modesPayement = vsp.GetModesPayement();
                modePays = new List<string>();
                foreach (PARAMETRE par in modesPayement)
                {
                    modePays.Add(par.NomAF);
                }

                escForm = form;

                utilisateur = user;
                operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);

                formLoader = new FormLoader(utilisateur);

                formLoader.LoadPaiementForm(this, pay);
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }

        }

        public PaiementForm(ManifesteForm form, PAYEMENT pay, UTILISATEUR user)
        {
            try
            {
                //VSOMAccessors vsomAcc = new VSOMAccessors();
                //VsomParameters vsp = new VsomParameters();
                InitializeComponent();
                this.DataContext = this;

                clients = vsp.GetClientsActifs();
                clts = new List<string>();
                foreach (CLIENT clt in clients)
                {
                    clts.Add(clt.NomClient);
                }

                banques = vsp.GetBanques();
                banks = new List<string>();
                foreach (BANQUE b in banques)
                {
                    banks.Add(b.CodeBanque);
                }

                navires = vsp.GetNaviresActifs();
                navs = new List<string>();
                foreach (NAVIRE nav in navires)
                {
                    navs.Add(nav.NomNav);
                }

                modesPayement = vsp.GetModesPayement();
                modePays = new List<string>();
                foreach (PARAMETRE par in modesPayement)
                {
                    modePays.Add(par.NomAF);
                }

                manForm = form;

                utilisateur = user;
                operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);

                formLoader = new FormLoader(utilisateur);

                formLoader.LoadPaiementForm(this, pay);
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }

        }

        public PaiementForm(string type, PaiementPanel panel, UTILISATEUR user)
        {
            try
            {
                //VSOMAccessors vsomAcc = new VSOMAccessors();
                //VsomParameters vsp = new VsomParameters();
                InitializeComponent();
                this.DataContext = this;

                //connaissements = vsomAcc.GetConnaissementsFactures();
                //cons = new List<string>();
                //foreach (CONNAISSEMENT bl in connaissements)
                //{
                //    cons.Add(bl.NumBL);
                //}

                clients = vsp.GetClientsActifs();
                clts = new List<string>();
                foreach (CLIENT clt in clients)
                {
                    clts.Add(clt.NomClient);
                }

                banques = vsp.GetBanques();
                banks = new List<string>();
                foreach (BANQUE b in banques)
                {
                    banks.Add(b.CodeBanque);
                }

                navires = vsp.GetNaviresActifs();
                navs = new List<string>();
                foreach (NAVIRE nav in navires)
                {
                    navs.Add(nav.NomNav);
                }

                modesPayement = vsp.GetModesPayement();
                modePays = new List<string>();
                foreach (PARAMETRE par in modesPayement)
                {
                    modePays.Add(par.NomAF);
                }

                payPanel = panel;

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

        private void cbNumEsc_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                txtNumVoy.Text = escales.ElementAt<ESCALE>(cbNumEsc.SelectedIndex).NumVoySCR;
                cbNavire.SelectedItem = escales.ElementAt<ESCALE>(cbNumEsc.SelectedIndex).NAVIRE.NomNav;
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
                VSOMAccessors vsomAcc = new VSOMAccessors();

                if (txtIdBL.Text == "")
                {
                    MessageBox.Show("Veuillez indiquer le connaissement pour lequel vous voulez effectuer un paiement.", "N° connaissement ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (factTab.IsSelected == true && dataGridFacts.SelectedItems.Count == 0)
                {
                    MessageBox.Show("Il faut sélectionner au moins une facture pour pouvoir procéder au paiement.", "Facture ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (cautionsTab.IsSelected == true && dataGridCautions.SelectedItems.Count == 0)
                {
                    MessageBox.Show("Il faut sélectionner au moins un conteneur pour pouvoir procéder au paiement de cette caution.", "Conteneur ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (restCautionTab.IsSelected == true && dataGridRestCautions.SelectedItems.Count == 0)
                {
                    MessageBox.Show("Il faut sélectionner au moins une ligne de caution pour pouvoir procéder à cette opération de restitution.", "Caution ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (txtMRPay.Text == "")
                {
                    MessageBox.Show("Vous ne pouvez pas enregistrer de paiement sans saisir le montant reçu.", "Montant reçu ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (Convert.ToInt32(txtMRendrePay.Text) < 0 && chkRetenueIS.IsChecked == false)
                {
                    MessageBox.Show("Vous ne pouvez pas enregistrer de paiement si la montant reçu est inférieur au montant dû.", "Vérifiez le montant reçu ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (dataGridFacts.SelectedItems.OfType<FACTURE>().ToList<FACTURE>().Count(fact => fact.IdPay.HasValue) != 0)
                {
                    MessageBox.Show("Parmi les factures sélectionnées, il en existe qui, ont déjà fait l'objet de paiement, procedez à une vérification.", "Facture(s) payée(s) ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (dataGridCautions.SelectedItems.OfType<CONTENEUR>().ToList<CONTENEUR>().Count(ctr => ctr.IdPay.HasValue) != 0)
                {
                    MessageBox.Show("Parmi les conteneurs sélectionnées, il en existe qui, ont déjà fait l'objet de paiement d'une caution, procedez à une vérification.", "Caution(s) payée(s) ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (dataGridRestCautions.SelectedItems.OfType<PAYEMENT>().ToList<PAYEMENT>().Count(pay => pay.IdPayDRC.HasValue) != 0)
                {
                    MessageBox.Show("Parmi les cautions sélectionnées, il en existe qui, ont déjà fait l'objet d'une restitution, procedez à une vérification.", "Caution(s) déjà restituée(s) ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (cbModePay.Text == "Paiement par Chèques" && (txtNumCheque.Text.Trim() == "" || txtNumCompte.Text.Trim() == ""))
                {
                    MessageBox.Show("Veuillez saisir les références du chèque", "Références du chèque ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (cbModePay.Text == "Paiement par Traitre" && (txtNumCheque.Text.Trim() == "" || txtNumCompte.Text.Trim() == ""))
                {
                    MessageBox.Show("Veuillez saisir les références du traitre", "Références du traitre ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (cbModePay.Text == "Paiement par Virement" && (txtRefVirt.Text.Trim() == "" || cbBanque.SelectedIndex == -1))
                {
                    MessageBox.Show("Veuillez saisir les références du virement", "Références du virement ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (cbCompteCaisse.SelectedIndex == -1)
                {
                    MessageBox.Show("Veuillez saisir le compte caisse à utiliser", "Compte Caisse ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    short objetPayement = 0;
                    if (profTab.IsSelected)
                    {
                        objetPayement = 0;
                    }
                    else if (factTab.IsSelected)
                    {
                        objetPayement = 1;
                    }
                    else if (cautionsTab.IsSelected)
                    {
                        objetPayement = 2;
                    }
                    else if (restCautionTab.IsSelected)
                    {
                        objetPayement = 3;
                    }

                    short modePayement = 0;
                    //if (cbModePay.Text == "Paiement Espèces")
                    //{
                    //    modePayement = 1;
                    //}
                    //if (cbModePay.Text == "Paiement par Chèques")
                    //{
                    //    modePayement = 2;
                    //}
                    //if (cbModePay.Text == "Paiement par Virement")
                    //{
                    //    modePayement = 3;
                    //}
                    if (cbModePay.SelectedIndex == 0)//espece
                    {
                        modePayement = 1;
                    }
                    if (cbModePay.SelectedIndex == 1)//cheque
                    {
                        modePayement = 2;
                    }
                    if (cbModePay.SelectedIndex == 2)//traitre
                    {
                        modePayement = 4;
                    }
                    if (cbModePay.SelectedIndex == 3) //Virement
                    {
                        modePayement = 3;
                    }

                    // ne plus prendre le montant recu mais plutôt le montant à payer
                    PAYEMENT pay = vsomAcc.InsertPaiement(Convert.ToInt32(txtIdBL.Text), objetPayement, modePayement, txtComptePay.Text, cbCompteCaisse.Text, (chkRetenueIS.IsChecked == true ? "Y" : "N"), txtBanque.Text, txtAgence.Text, txtNumCompte.Text.Replace(" ", ""), txtNumCheque.Text.Replace(" ", ""), txtRefVirt.Text, txtCCBanque.Text, 
                        Convert.ToInt32(txtMAPay.Text), Convert.ToInt32(txtMAPay.Text), dataGridProfs.SelectedItems.OfType<PROFORMA>().ToList<PROFORMA>(), dataGridFacts.SelectedItems.OfType<FACTURE>().ToList<FACTURE>(), dataGridCautions.SelectedItems.OfType<CONTENEUR>().ToList<CONTENEUR>(), dataGridRestCautions.SelectedItems.OfType<PAYEMENT>().ToList<PAYEMENT>(), new TextRange(txtObservations.Document.ContentStart, txtObservations.Document.ContentEnd).Text, cbBanque.SelectedIndex != -1 ? banques.ElementAt<BANQUE>(cbBanque.SelectedIndex).IdBanque : 0, utilisateur.IdU);

                    //payPanel.paiements = vsomAcc.GetPaiements();
                    //payPanel.cbFiltres.SelectedIndex = 0;
                    //payPanel.dataGrid.ItemsSource = payPanel.paiements;
                    //payPanel.lblStatut.Content = payPanel.paiements.Count + " Paiement(s)";
                    //paiements = payPanel.paiements;
                    formLoader.LoadPaiementForm(this, pay);
                    borderActions.Visibility = System.Windows.Visibility.Visible;
                    borderEtat.Visibility = System.Windows.Visibility.Visible;
                    btnEnregistrer.IsEnabled = false;
                    MessageBox.Show("Enregistrement effectué avec succès.", "Enregistrement effectué !", MessageBoxButton.OK, MessageBoxImage.Information);
                }
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

        private void cbModePay_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                txtComptePay.Text = modesPayement.ElementAt<PARAMETRE>(cbModePay.SelectedIndex).CodeAF.ToString();
                if (cbModePay.SelectedIndex == 1)
                {
                    txtNumCheque.IsEnabled = true;
                    txtAgence.IsEnabled = true;
                    txtBanque.IsEnabled = true;
                    txtNumCompte.IsEnabled = true;
                    txtRefVirt.IsEnabled = false;
                    txtRefVirt.Text = "";
                    cbBanque.IsEnabled = false;
                    txtCCBanque.Text = "";
                }
                else if (cbModePay.SelectedIndex == 2) //AH 16aout2016 paiement par traite
                {
                    txtNumCheque.IsEnabled = true;
                    txtAgence.IsEnabled = true;
                    txtBanque.IsEnabled = true;
                    txtNumCompte.IsEnabled = true;
                    txtRefVirt.IsEnabled = false;
                    txtRefVirt.Text = "";
                    cbBanque.IsEnabled = false;
                    txtCCBanque.Text = "";
                }
                else if (cbModePay.SelectedIndex == 3) //AH 16aout2016 changement index paiement par virement
                {
                    txtNumCheque.IsEnabled = false;
                    txtNumCompte.IsEnabled = false;
                    txtAgence.IsEnabled = false;
                    txtBanque.IsEnabled = false;
                    txtNumCheque.Text = "";
                    txtNumCompte.Text = "";
                    txtAgence.Text = "";
                    txtBanque.Text = "";
                    txtRefVirt.IsEnabled = true;
                    cbBanque.IsEnabled = true;
                }
                else if (cbModePay.SelectedIndex == 0)
                {
                    txtBanque.IsEnabled = false; txtAgence.IsEnabled = false;
                    txtNumCompte.IsEnabled = false; txtNumCheque.IsEnabled = false;
                    
                    txtRefVirt.IsEnabled = false;
                    txtRefVirt.Text = "";
                    cbBanque.IsEnabled = false;
                    txtCCBanque.Text = "";
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

        private void txtMRPay_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void dataGridFacts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (dataGridFacts.SelectedItems.Count != 0)
                {
                    List<FACTURE> listFactures = dataGridFacts.SelectedItems.OfType<FACTURE>().ToList<FACTURE>();

                    txtMAPay.Text = listFactures.Sum(fact => fact.MTTC).ToString();
                    //txtMAPay.Text = listFactures.Sum(fact => fact.Solde).ToString();
                    txtMRendrePay.Text = "-" + txtMAPay.Text;
                    txtMRPay.Text = "0";
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

        private void txtMRPay_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                txtMRendrePay.Text = (Convert.ToInt32(txtMRPay.Text) - Convert.ToInt32(txtMAPay.Text)).ToString();
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
            
        }

        private void dataGridCtrs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (dataGridCautions.SelectedItems.Count != 0)
                {
                    List<CONTENEUR> listConteneur = dataGridCautions.SelectedItems.OfType<CONTENEUR>().ToList<CONTENEUR>();

                    txtMAPay.Text = listConteneur.Sum(ctr => ctr.MCCtr).ToString();
                    txtMRendrePay.Text = "-" + txtMAPay.Text;
                    txtMRPay.Text = "0";
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
                //VSOMAccessors vsomAcc = new VSOMAccessors();
                //VsomParameters vsp = new VsomParameters();
                if (e.Key == Key.Return && cbNumBL.Text.Trim() != "")
                {
                    connaissements = vsp.GetConnaissementsPayablesByNumBL(cbNumBL.Text.Trim());

                    if (connaissements.Count == 0)
                    {
                        MessageBox.Show("Il n'existe aucun connaissement payable portant ce numéro", "Connaissement introuvable", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                    else if (connaissements.Count == 1)
                    {
                        CONNAISSEMENT con = connaissements.FirstOrDefault<CONNAISSEMENT>();
                        formLoader.LoadConnaissementForm(this, con);

                        //mise en place promo 2016 50% remise sur manu et prestation gestion parc sur bl grimaldi et vehidule 18m3.
                        if (DateTime.Today >= DateTime.Parse("18/12/2016") && DateTime.Today <= DateTime.Parse("24/12/2016") && con.ESCALE.IdArm == 1 && con.ESCALE.DRAEsc <= DateTime.Parse("31/08/2016"))
                        {
                            vsomAcc = new VSOMAccessors(); 
                            List<PROFORMA> lstPro = vsomAcc.Promo2016(proformas); 
                            proformas= lstPro;
                            dataGridProfs.ItemsSource = proformas;
                        } 
                    }
                    else
                    {
                        ListConnaissementForm listConForm = new ListConnaissementForm(this, connaissements, utilisateur);
                        listConForm.Title = "Choix multiples : Sélectionnez un connaissement";
                        listConForm.ShowDialog();
                    }
                }
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void cbIdPay_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                //VSOMAccessors vsomAcc = new VSOMAccessors();
                //VsomParameters vsps = new VsomParameters();
                if (e.Key == Key.Return && cbIdPay.Text.Trim() != "")
                {
                    int result;

                    paiements = new List<PAYEMENT>();

                    PAYEMENT p = vsp.GetPaiementByIdPay(Int32.TryParse(cbIdPay.Text.Trim(), out result) ? result : -1);

                    if (p != null)
                    {
                        paiements.Add(p);
                    }

                    if (paiements.Count == 0)
                    {
                        MessageBox.Show("Il n'existe aucun paiement portant ce numéro", "Paiement introuvable", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                    else if (paiements.Count == 1)
                    {
                        PAYEMENT pay = paiements.FirstOrDefault<PAYEMENT>();
                        formLoader.LoadPaiementForm(this, pay);
                    }
                    else
                    {
                        ListPaiementForm listPayForm = new ListPaiementForm(this, paiements, utilisateur);
                        listPayForm.Title = "Choix multiples : Sélectionnez un paiment";
                        listPayForm.ShowDialog();
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

        private void cbIdPay_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void dataGridProfs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (dataGridProfs.SelectedItems.Count != 0)
                {
                    List<PROFORMA> listProformas = dataGridProfs.SelectedItems.OfType<PROFORMA>().ToList<PROFORMA>();
                     
                    txtMAPay.Text = listProformas.Sum(prof => prof.MTTC).ToString();
                    txtMRendrePay.Text = "-" + txtMAPay.Text;
                    txtMRPay.Text = "0";
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

        private void dataGridFacts_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
               // VSOMAccessors vsomAcc = new VSOMAccessors();

                FactureForm factForm = new FactureForm(this, ((FACTURE)dataGridFacts.SelectedItem), utilisateur);
                factForm.Show();
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        private void txtNumCheque_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void cbBanque_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                txtCCBanque.Text = banques.ElementAt<BANQUE>(cbBanque.SelectedIndex).CCBanque;
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        private void dataGridRestCautions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (dataGridRestCautions.SelectedItems.Count != 0)
                {
                    List<PAYEMENT> listPayement = dataGridRestCautions.SelectedItems.OfType<PAYEMENT>().ToList<PAYEMENT>();

                    txtMAPay.Text = listPayement.Sum(pay => pay.MAPay).ToString();
                    txtMRendrePay.Text = "-" + txtMAPay.Text;
                    txtMRPay.Text = "0";
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

        private void btnAnnuler_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (operationsUser.Where(op => op.NomOp == "Paiement : Suppression d'un élément existant").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
                {
                    MessageBox.Show("Vous n'avez pas les droits nécessaires pour annuler un paiement. Veuillez contacter un administrateur", "Droits insuffisants !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    AnnulerPaiementForm annulationForm = new AnnulerPaiementForm(this, utilisateur);
                    annulationForm.Title = "Annulation du paiement : " + cbIdPay.Text + " - Consignée " + txtConsignee.Text;
                    annulationForm.ShowDialog();
                }
            }
            catch (EnregistrementInexistant ex)
            {
                MessageBox.Show(ex.Message, "Enregistrement inexistant !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        private void btnImprimerRecu_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                RecuReport recuReport = new RecuReport(this);
                recuReport.Title = "Impression du reçu de paiement : " + cbIdPay.Text + " - Escale : " + cbNumEsc.Text;
                recuReport.Show();
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
