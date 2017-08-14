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
using System.Windows.Navigation;
using System.Windows.Shapes;
using VesselStopOverData;
using System.Text.RegularExpressions;

namespace VesselStopOverPresentation
{
    /// <summary>
    /// Logique d'interaction pour ClientForm.xaml
    /// </summary>
    public partial class ClientForm : Window
    {
        private List<CLIENT> clients;
        public List<string> clts { get; set; }

        private List<CODE_TVA> codes;
        public List<string> cds { get; set; }

        public List<ElementSituationCompteClient> situationCompte { get; set; }
        public List<FacturePaiementClient> _ligneFacture { get; set; }
        public List<CONNAISSEMENT> connaissements { get; set; }
        public List<PROFORMA> proformas { get; set; }
        public List<FACTURE> factures { get; set; }
        public List<PAYEMENT> paiements { get; set; }

        public CLIENT client { get; set; }

        private ClientPanel clientPanel;

        private List<OPERATION> operationsUser;
        private UTILISATEUR utilisateur;

        private VsomParameters vsp = new VsomParameters();
        public ClientForm(ClientPanel panel, UTILISATEUR user)
        {
            try
            {
               // VSOMAccessors vsomAcc = new VSOMAccessors();
                //VsomParameters vsprm = new VsomParameters();
                InitializeComponent();
                this.DataContext = this;

                clients = vsp.GetClientsActifs();
                clts = new List<string>();
                foreach (CLIENT clt in clients)
                {
                    clts.Add(clt.CodeClient);
                }

                codes = vsp.GetCodesTVA();
                cds = new List<string>();
                foreach (CODE_TVA cd in codes)
                {
                    cds.Add(cd.LibTVA);
                }

                client = new CLIENT();

                clientPanel = panel;
                utilisateur = user;
                operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);
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

        public ClientForm(string type, ClientPanel panel, UTILISATEUR user)
        {
            try
            {
               // VSOMAccessors vsomAcc = new VSOMAccessors();
                //VsomParameters vsprm = new VsomParameters();
                InitializeComponent();
                this.DataContext = this;

                codes = vsp.GetCodesTVA();
                cds = new List<string>();
                foreach (CODE_TVA cd in codes)
                {
                    cds.Add(cd.LibTVA);
                }

                client = new CLIENT();

                clientPanel = panel;
                cbCodeClient.Focus();

                borderActions.Visibility = System.Windows.Visibility.Collapsed;

                utilisateur = user;
                operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);
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

        private void cbGroupeTVA_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                txtCodeTVA.Text = codes.ElementAt<CODE_TVA>(cbGroupeTVA.SelectedIndex).CodeTVA;
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
            
        }

        private void cbCodeClient_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
               // VSOMAccessors vsomAcc = new VSOMAccessors();

                if (cbCodeClient.IsEditable == false && cbCodeClient.Items.Count != 0)
                {
                    int indexClient = cbCodeClient.SelectedIndex;
                    CLIENT clt = clients.ElementAt<CLIENT>(indexClient);

                    txtClientSysID.Text = clt.IdClient.ToString();
                    txtNomClient.Text = clt.NomClient;
                    txtAdresse.Text = clt.AdrClient;
                    txtTelephone.Text = clt.TelClient;
                    txtEmail.Text = clt.EmailClient;
                    txtCpteCompatble.Text = clt.CCClient;
                    txtNContrib.Text = clt.NumContrib;
                    txtRegCommerce.Text = clt.NumRegCommerce;
                    if (clt.TypeClient == "D")
                    {
                        cbTypeClient.SelectedIndex = 0;
                    }
                    else
                    {
                        cbTypeClient.SelectedIndex = 1;
                    }
                    cbGroupeTVA.SelectedItem = clt.CODE_TVA.LibTVA;

                    lblStatut.Content = "Actif";

                    ////situation du compte
                    //situationCompte = vsomAcc.GetSituationCompteClient(clt.IdClient);
                    //dataGridSituation.ItemsSource = situationCompte;
                    //valeurEncoursBL.Content = situationCompte.Where(s => s.Rubrique == "Connaissements").FirstOrDefault<ElementSituationCompteClient>().MontantTotal;
                    //valeurEncoursFacture.Content = situationCompte.Where(s => s.Rubrique == "Factures").FirstOrDefault<ElementSituationCompteClient>().MontantTotal;
                    //valeurEncoursPaiement.Content = situationCompte.Where(s => s.Rubrique == "Paiements").FirstOrDefault<ElementSituationCompteClient>().MontantTotal;
                    
                    _ligneFacture = new List<FacturePaiementClient>();
                   List<FACTURE> factures = vsp.getFactureAllUnPaidByClient(clt.IdClient);
                   foreach (FACTURE f in factures)
                   {
                       if (f.PROFORMA == null)
                       {
                           _ligneFacture.Add(new FacturePaiementClient { Facture = f, Date = f.DCFD.Value, MHT = f.MHT.Value, MTTC = f.MTTC.Value, TVA = f.MTVA.Value, BL = string.Empty, NumElmt = "FC" + f.IdDocSAP });
                       }
                       else
                       {
                           _ligneFacture.Add(new FacturePaiementClient { Facture = f, Date = f.DCFD.Value, MHT = f.MHT.Value, MTTC = f.MTTC.Value, TVA = f.MTVA.Value, NumElmt = "FC" + f.IdDocSAP, Escale = f.PROFORMA.CONNAISSEMENT.ESCALE.NumEsc.ToString(), BL = f.PROFORMA.CONNAISSEMENT.NumBL, idBL = f.PROFORMA.CONNAISSEMENT.IdBL });
                       }

                   }
                   dataGridSituation.ItemsSource = null;
                   dataGridSituation.ItemsSource = _ligneFacture;
                    this.Title = "Client - " + cbCodeClient.SelectedItem + " - " + clt.NomClient;
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

        private void cbCodeClient_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void btnEnregistrer_Click(object sender, RoutedEventArgs e)
        {
            //VSOMAccessors vsomAcc = new VSOMAccessors();
            VsomMarchal vsomAcc = new VsomMarchal();

            //VsomParameters vsprm = new VsomParameters();

            if (cbCodeClient.Text.Trim().Equals(""))
            {
                MessageBox.Show("Vous n'avez pas saisi le code du client ; il est indispensable pour la création d'un client.", "Code client ?", MessageBoxButton.OK, MessageBoxImage.Information);
                cbCodeClient.Focus();
            }
            else if (txtNomClient.Text.Trim() == "")
            {
                MessageBox.Show("Vous n'avez pas saisi le nom du client", "Nom du client ?", MessageBoxButton.OK, MessageBoxImage.Information);
                txtNomClient.Focus();
            }
            else if (txtCpteCompatble.Text.Trim() == "")
            {
                MessageBox.Show("Vous n'avez pas saisi le compte comptable du client", "Compte comptable du client ?", MessageBoxButton.OK, MessageBoxImage.Information);
                txtCpteCompatble.Focus();
            }
            else if (cbTypeClient.SelectedIndex == -1)
            {
                MessageBox.Show("Vous devez indiquer le type de client.", "Type de client ?", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else if (cbGroupeTVA.SelectedIndex == -1)
            {
                MessageBox.Show("Vous devez indiquer le groupe de taxe.", "Groupe de taxe ?", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else if ((operationsUser.Where(op => op.NomOp == "Client : Modification des élements").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin") )
            {
                MessageBox.Show("Vous n'avez pas les autorisations necessaires pour cette opération.", "Modification client", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                try
                {
                    CLIENT cl = vsomAcc.InsertOrUpdateClient(client.IdClient, cbCodeClient.Text, txtNomClient.Text, txtAdresse.Text, txtEmail.Text, txtTelephone.Text, txtNContrib.Text, txtRegCommerce.Text, txtCpteCompatble.Text, txtCodeTVA.Text, (cbTypeClient.SelectedIndex == 0 ? "D" : "C"));
                    clientPanel.clients = vsomAcc.GetClientsActifs();
                    clientPanel.dataGrid.ItemsSource = clientPanel.clients;
                    clientPanel.lblStatut.Content = clientPanel.clients.Count + " Client(s)";
                    clients = clientPanel.clients;
                    clts = new List<string>();
                    foreach (CLIENT clt in clients)
                    {
                        clts.Add(clt.CodeClient);
                    }
                    cbCodeClient.IsEditable = false;
                    cbCodeClient.ItemsSource = null;
                    cbCodeClient.ItemsSource = clts;
                    cbCodeClient.SelectedItem = cl.CodeClient;
                    borderActions.Visibility = System.Windows.Visibility.Visible;
                    MessageBox.Show("Enregistrement effectué avec succès.", "Enregistrement effectué !", MessageBoxButton.OK, MessageBoxImage.Information);
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
        }

        private void txtCpteCompatble_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void dataGridSituation_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            if (dataGridSituation.SelectedIndex != -1)
            {
                //FacturePaiementClient fpc = (FacturePaiementClient)dataGridSituation.SelectedItem;
                //txtNotes.Text = fpc.Facture.AIFD;
            }
        }
    }
}
