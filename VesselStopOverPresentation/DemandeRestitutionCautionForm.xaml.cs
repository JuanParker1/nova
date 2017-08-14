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
    /// Logique d'interaction pour DemandeRestitutionCautionForm.xaml
    /// </summary>
    public partial class DemandeRestitutionCautionForm : Window
    {
        public List<CONNAISSEMENT> connaissements { get; set; }
        public List<string> cons { get; set; }

        public List<CLIENT> clients { get; set; }
        public List<string> clts { get; set; }

        public List<ESCALE> escales { get; set; }
        public List<Int32> escs { get; set; }

        public List<DEMANDE_CAUTION> demandesCaution { get; set; }
        public List<Int32> cautions { get; set; }

        public List<FACTURE> factures { get; set; }

        public List<NAVIRE> navires { get; set; }
        public List<string> navs { get; set; }

        public List<CONTENEUR> conteneurs { get; set; }

        private UTILISATEUR utilisateur;
        private List<OPERATION> operationsUser;

        public DemandeRestitutionCautionPanel demandeCautionPanel { get; set; }

        private FormLoader formLoader;
        private VsomParameters vsp = new VsomParameters();
        public DemandeRestitutionCautionForm(DemandeRestitutionCautionPanel panel, DEMANDE_CAUTION caution, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();
               // VsomParameters vsp = new VsomParameters();
                InitializeComponent();
                this.DataContext = this;

                demandesCaution = vsp.GetDemandesRestitution();
                cautions = new List<Int32>();
                foreach (DEMANDE_CAUTION drc in demandesCaution)
                {
                    cautions.Add(drc.IdDRC);
                }

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

                demandeCautionPanel = panel;

                utilisateur = user;
                operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);

                formLoader = new FormLoader(utilisateur);

                formLoader.LoadDemandeCautionForm(this, caution);
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
            
        }

        public DemandeRestitutionCautionForm(string type, DemandeRestitutionCautionPanel panel, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();
                
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

                demandeCautionPanel = panel;

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
                if (cbClient.SelectedIndex != -1)
                {
                    txtCodeClient.Text = clients.ElementAt<CLIENT>(cbClient.SelectedIndex).CodeClient;
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

        private void cbNumEsc_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (cbNumEsc.SelectedIndex != -1)
                {
                    txtNumVoy.Text = escales.ElementAt<ESCALE>(cbNumEsc.SelectedIndex).NumVoySCR;
                    cbNavire.SelectedItem = escales.ElementAt<ESCALE>(cbNumEsc.SelectedIndex).NAVIRE.NomNav;
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

        private void btnEnregistrer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //VSOMAccessors vsomAcc = new VSOMAccessors();
                VsomMarchal vsomAcc = new VsomMarchal();

                if (cbIdDRC.IsEnabled == false && txtIdBL.Text != "")
                {
                    if (dataGridConteneurs.SelectedItems.Count == 0)
                    {
                        MessageBox.Show("Veuillez sélectionner au moins un conteneur", "Sélectionnez au moins un conteneur !", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else
                    {
                        CONNAISSEMENT bl = vsp.GetConnaissementByIdBL(Convert.ToInt32(txtIdBL.Text));

                        double eltsBL = bl.ELEMENT_FACTURATION.Where(el =>  el.StatutEF != "Annule" && !el.IdVeh.HasValue && !el.IdCtr.HasValue && !el.IdGC.HasValue).Sum(elt => elt.PUEF.Value * elt.QTEEF.Value * (1 + elt.CODE_TVA.TauxTVA.Value / 100));
                        double eltsCtr = dataGridConteneurs.SelectedItems.OfType<CONTENEUR>().Sum(ctr => ctr.ELEMENT_FACTURATION.Where(el => el.StatutEF != "Annule").Sum(elt => elt.PUEF * elt.QTEEF * (1 + elt.CODE_TVA.TauxTVA / 100))).Value;
                        int blpay=(int) vsp.GetPaiementsFactureOfConnaissement(bl.IdBL).Sum(pay => pay.MAPay);
                        if ((eltsBL + eltsCtr) > 100 +blpay  && bl.ESCALE.IdAcc == 1 && bl.IdClient == 1)
                        {
                            MessageBox.Show("Le compte de ce BL n'est pas soldé, il ne vous est donc pas possible d'établir de demande de restitution de caution", "Compte BL non soldé !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        } 
                        else if(dataGridConteneurs.SelectedItems.OfType<CONTENEUR>().Count(ctr => !ctr.DRCtr.HasValue) != 0)
                        {
                            MessageBox.Show("Il existe des conteneurs non retournés. Par conséquent, vous ne pouvez pas établir cette demande de restitution de caution", "Conteneur non retourné !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        }
                        else
                        {
                            DEMANDE_CAUTION caution = vsomAcc.InsertDemandeRestitutionCaution(bl.IdBL, dataGridConteneurs.SelectedItems.OfType<CONTENEUR>().ToList<CONTENEUR>(), new TextRange(txtObservations.Document.ContentStart, txtObservations.Document.ContentEnd).Text, utilisateur.IdU);

                            if (demandeCautionPanel.cbFiltres.SelectedIndex != 2)
                            {
                                demandeCautionPanel.cbFiltres.SelectedIndex = 2;
                            }
                            else
                            {
                                demandeCautionPanel.demandesCaution = vsp.GetDemandesRestitutionEnAttente();
                                demandeCautionPanel.dataGrid.ItemsSource = demandeCautionPanel.demandesCaution;
                                demandeCautionPanel.lblStatut.Content = demandeCautionPanel.demandesCaution.Count + " Demande(s) de restitution de caution";
                            }

                            formLoader.LoadDemandeCautionForm(this, caution);

                            btnValider.Visibility = System.Windows.Visibility.Visible;
                            MessageBox.Show("Enregistrement effectué avec succès.", "Enregistrement effectué !", MessageBoxButton.OK, MessageBoxImage.Information);
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

        private void btnValider_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (operationsUser.Where(op => op.NomOp == "Demande de restitution de caution : Validation d'un élément").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
                {
                    MessageBox.Show("Vous n'avez pas les droits nécessaires pour valider une demande de restitution de caution. Veuillez contacter un administrateur", "Droits insuffisants !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    ValiderDemandeCautionForm validForm = new ValiderDemandeCautionForm(this, utilisateur);
                    validForm.Title = "Validation de la demande de caution : " + cbIdDRC.Text + " - Consignée " + txtConsignee.Text;
                    validForm.ShowDialog();
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

        private void cbNumBL_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                //VSOMAccessors vsomAcc = new VSOMAccessors();
                vsp = new VsomParameters();
                if (e.Key == Key.Return && cbNumBL.Text.Trim() != "")
                {
                    connaissements = vsp.GetConnaissementPourCautionByNumBL(cbNumBL.Text);
                    if (connaissements.Count == 0)
                    {
                        MessageBox.Show("Il n'existe aucun connaissement ayant une caution payée portant ce numéro", "Connaissement introuvable", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                    else if (connaissements.Count == 1)
                    {
                        CONNAISSEMENT con = connaissements.FirstOrDefault<CONNAISSEMENT>();
                        formLoader.LoadConnaissementForm(this, con, 0);
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

            }
        }

        private void dataGridConteneurs_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (dataGridConteneurs.SelectedIndex != -1)
                {
                    ConteneurForm ctrForm = new ConteneurForm(this, (CONTENEUR)dataGridConteneurs.SelectedItem, utilisateur);
                    ctrForm.Show();
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

        private void listNotes_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }

        private void dataGridConteneurs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                if (dataGridConteneurs.SelectedIndex != -1)
                {
                    List<HistoriqueInterchange> interchanges = vsp.GetInterchangeCtr(((CONTENEUR)dataGridConteneurs.SelectedItem).IdCtr);
                    dataGridSinistres.ItemsSource = null;
                    dataGridSinistres.ItemsSource = interchanges;
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
