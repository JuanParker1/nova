using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using VesselStopOverData;

namespace VesselStopOverPresentation
{
    /// <summary>
    /// Logique d'interaction pour PaiementClient.xaml
    /// </summary>
    public partial class PaiementClient : Window
    {
        private List<CLIENT> clients;
        private List<PARAMETRE> modesPayement;
        private List<OPERATION> operationsUser;
        private UTILISATEUR utilisateur;
        public List<FACTURE> factures;
        public List<AVOIR> avoirs;
        private List<BANQUE> banques;
        public List<string> banks ;
        public List<FacturePaiementClient> _ligneFacture;
        private VsomParameters vsp = new VsomParameters();

        public PaiementClient(PaiementPanel panel, UTILISATEUR user)
        {
            try
            {

                InitializeComponent();
                VSOMAccessors vsomAcc = new VSOMAccessors();
                VsomParameters vsprm = new VsomParameters();
                clients = vsprm.GetClientsActifs();
                cbCodeClient.ItemsSource = null; cbCodeClient.ItemsSource = clients;
                cbCodeClient.DisplayMemberPath = "NomClient";

                //clts = new List<string>();
                //foreach (CLIENT clt in clients)
                //{
                //    clts.Add(clt.NomClient);
                //}

                banques = vsprm.GetBanques();
                cbBanque.ItemsSource = banques;
                cbBanque.DisplayMemberPath = "CodeBanque";
                //banks = new List<string>();
                //foreach (BANQUE b in banques)
                //{
                //    banks.Add(b.CodeBanque);
                //}

                modesPayement = vsprm.GetModesPayement();
                cbModePay.ItemsSource = modesPayement;
                cbModePay.DisplayMemberPath = "NomAF";
                //modePays = new List<string>();
                //foreach (PARAMETRE par in modesPayement)
                //{
                //    modePays.Add(par.NomAF);
                //}

                utilisateur = user;
                operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);
                this.DataContext = this;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Paiement CLient");
            }
        }

        private void btnEnregistrer_Click_1(object sender, RoutedEventArgs e)
        {
            List<FacturePaiementClient> _selected = dataGridEltsFact.SelectedItems.OfType<FacturePaiementClient>().ToList<FacturePaiementClient>();
            VSOMAccessors vsomAcc = new VSOMAccessors();
            
             if (cbCodeClient.SelectedIndex == -1)
            {
                MessageBox.Show("Selectionner un client", "Paiement Client");
            }
            else if (_selected==null || _selected.Count==0)
            {
                MessageBox.Show("Vous ne pouvez pas enregistrer de paiement sans definir les factures.", "Factures Paiement ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            else if (txtMRPay.Text == "")
            {
                MessageBox.Show("Vous ne pouvez pas enregistrer de paiement sans saisir le montant reçu.", "Montant reçu ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            else if (Convert.ToInt32(txtMRendrePay.Text) < 0 )
            {
                MessageBox.Show("Vous ne pouvez pas enregistrer de paiement si la montant reçu est inférieur au montant dû.", "Vérifiez le montant reçu ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
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
            //else if (cbCompteCaisse.SelectedIndex == -1)
            //{
            //    MessageBox.Show("Veuillez saisir le compte caisse à utiliser", "Compte Caisse ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            //}
            else
            {
                int objetPayement = 1;
                short modePayement = 0;
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
                 CLIENT cl =(CLIENT) cbCodeClient.SelectedItem;

                 try
                 {
                     PAYEMENT pay = vsomAcc.InsertPaiementClient(cl, modePayement, txtComptePay.Text, string.Empty,
                                 txtBanque.Text, txtAgence.Text, txtNumCompte.Text.Replace(" ", ""), txtNumCheque.Text.Replace(" ", ""), txtRefVirt.Text, txtCCBanque.Text,
                                  Convert.ToInt32(txtMAPay.Text), Convert.ToInt32(txtMAPay.Text),
                                   new TextRange(txtObservations.Document.ContentStart, txtObservations.Document.ContentEnd).Text,
                                   cbBanque.SelectedIndex != -1 ? banques.ElementAt<BANQUE>(cbBanque.SelectedIndex).IdBanque : 0, utilisateur.IdU,
                                  _selected);

                     if (pay != null)
                     {
                         cbIdPay.Text = pay.IdPaySAP.ToString();
                         MessageBox.Show("Paiement effectué", "Paiement Client");
                         btnEnregistrer.IsEnabled = false;
                     }
                 }
                 catch (Exception ex)
                 {
                     MessageBox.Show(ex.StackTrace, "Paiement Facture Spote");
                 }
            }
        }

        private void btnAnnuler_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void btnImprimerRecu_Click_1(object sender, RoutedEventArgs e)
        {

        }
         
        private void cbIdPay_PreviewTextInput_1(object sender, TextCompositionEventArgs e)
        {

        }

        private void cbModePay_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
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

        private void txtMRPay_PreviewTextInput_1(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }


        private void cbBanque_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
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

        private void cbCodeClient_PreviewTextInput_1(object sender, TextCompositionEventArgs e)
        {

        }

        private void txtNumCompte_PreviewTextInput_1(object sender, TextCompositionEventArgs e)
        {

        }
         
        private void txtMRPay_LostFocus_1(object sender, RoutedEventArgs e)
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
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void cbCodeClient_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            if (cbCodeClient.SelectedIndex != -1)
            {
                CLIENT c = (CLIENT)cbCodeClient.SelectedItem;
                txtClientSysID.Text = c.CodeClient;
                 
                VSOMAccessors vsom = new VSOMAccessors();
                factures = vsp.getFactureUnPaidByClient(c.IdClient);
                avoirs = vsp.GetAvailableAvoirSpotByClient(c.IdClient);
                _ligneFacture = new List<FacturePaiementClient>();
                foreach (FACTURE f in factures)
                {
                    if (f.PROFORMA == null)
                    {
                        _ligneFacture.Add(new FacturePaiementClient { Facture=f, Date=f.DCFD.Value ,MHT=f.MHT.Value, MTTC=f.MTTC.Value, TVA=f.MTVA.Value, BL=string.Empty , NumElmt="FC"+f.IdDocSAP});
                    }
                    else 
                    {
                        _ligneFacture.Add(new FacturePaiementClient { Facture = f, Date=f.DCFD.Value,MHT=f.MHT.Value, MTTC=f.MTTC.Value, TVA=f.MTVA.Value,NumElmt="FC"+f.IdDocSAP, Escale=f.PROFORMA.CONNAISSEMENT.ESCALE.NumEsc.ToString(), BL = f.PROFORMA.CONNAISSEMENT.NumBL, idBL=f.PROFORMA.CONNAISSEMENT.IdBL });
                    }

                }

                foreach (AVOIR av in avoirs)
                {
                    _ligneFacture.Add(new FacturePaiementClient { Avoir = av, Date = av.DCFA.Value, MHT = (-1 * av.MHT.Value), MTTC = (-1 * av.MTTC.Value), TVA = (-1 * av.MTVA.Value), BL = string.Empty, NumElmt = "AV" + av.IdFA });
                }

                dataGridEltsFact.ItemsSource = _ligneFacture;
                lblStatut.Content = factures.Count + " Facture(s) trouvées et "+avoirs.Count+" avoirs trouvés";
                txtMAPay.Text = string.Empty; txtMRendrePay.Text = string.Empty;
            }
        }

        private void CheckBox_Click_1(object sender, RoutedEventArgs e)
        {
           //object o= e.Source;
            //FacturePaiementClient f = (FacturePaiementClient)dataGridEltsFact.SelectedItem;
            List<FacturePaiementClient> lf = dataGridEltsFact.SelectedItems.OfType<FacturePaiementClient>().ToList<FacturePaiementClient>();
            int mapay=(int)lf.Sum(r=>r.MTTC); 
            txtMAPay.Text = mapay.ToString();
        }

        private void dataGridEltsFact_Selected_1(object sender, RoutedEventArgs e)
        {
            
        }

        private void CheckBox_Unchecked_1(object sender, RoutedEventArgs e)
        {
           
        }
         
    }
}
