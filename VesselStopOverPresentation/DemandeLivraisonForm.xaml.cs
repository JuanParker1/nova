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
    /// Logique d'interaction pour DemandeLivraisonForm.xaml
    /// </summary>
    public partial class DemandeLivraisonForm : Window
    {
        public List<CONNAISSEMENT> connaissements { get; set; }
        public List<string> cons { get; set; }
        //public CONNAISSEMENT _con;

        public List<CLIENT> clients { get; set; }
        public List<string> clts { get; set; }

        public List<ESCALE> escales { get; set; }
        public List<Int32> escs { get; set; }

        public List<DEMANDE_LIVRAISON> demandesLivraison { get; set; }
        public List<Int32> livs { get; set; }

        public List<FACTURE> factures { get; set; }

        public List<NAVIRE> navires { get; set; }
        public List<string> navs { get; set; }

        public List<VEHICULE> vehicules { get; set; }
        public List<CONTENEUR> conteneurs { get; set; }
        public List<CONVENTIONNEL> conventionnels { get; set; }
        public List<MAFI> mafis { get; set; }

        private UTILISATEUR utilisateur;
        private List<OPERATION> operationsUser;

        public DemandeLivraisonPanel demandeLivraisonPanel { get; set; }

        private FormLoader formLoader;
       // private VsomParameters vsp = new VsomParameters();
        private VSOMAccessors vsomAcc;
        public DemandeLivraisonForm(DemandeLivraisonPanel panel, DEMANDE_LIVRAISON livraison, UTILISATEUR user)
        {
            try
            {
                //VSOMAccessors vsomAcc = new VSOMAccessors();
                //VsomParameters vsp = new VsomParameters();
                InitializeComponent();
                this.DataContext = this;
                vsomAcc = new VSOMAccessors();
                clients = vsomAcc.GetClientsActifs();
                clts = new List<string>();
                foreach (CLIENT clt in clients)
                {
                    clts.Add(clt.NomClient);
                }

                navires = vsomAcc.GetNaviresActifs();
                navs = new List<string>();
                foreach (NAVIRE nav in navires)
                {
                    navs.Add(nav.NomNav);
                }

                demandeLivraisonPanel = panel;

                utilisateur = user;
                operationsUser = vsomAcc.GetOperationsUtilisateur(utilisateur.IdU);

                formLoader = new FormLoader(utilisateur);

                formLoader.LoadDemandeLivraisonForm(this, livraison);
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
            
        }

        public DemandeLivraisonForm(string type, DemandeLivraisonPanel panel, UTILISATEUR user)
        {
            try
            {
                //VSOMAccessors vsomAcc = new VSOMAccessors();
               // VsomParameters vsp = new VsomParameters();
                InitializeComponent();
                this.DataContext = this;
                vsomAcc = new VSOMAccessors();
                clients = vsomAcc.GetClientsActifs();
                clts = new List<string>();
                foreach (CLIENT clt in clients)
                {
                    clts.Add(clt.NomClient);
                }

                navires = vsomAcc.GetNaviresActifs();
                navs = new List<string>();
                foreach (NAVIRE nav in navires)
                {
                    navs.Add(nav.NomNav);
                }

                demandeLivraisonPanel = panel;

                utilisateur = user;
                operationsUser = vsomAcc.GetOperationsUtilisateur(utilisateur.IdU);

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

        private void btnDetailsVeh_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DetailsLivraisonVehForm detailsVehForm = new DetailsLivraisonVehForm(this, Convert.ToInt32(txtIdBL.Text), utilisateur);
                detailsVehForm.ShowDialog();
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
            
        }

        private void btnDetailsCtr_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DetailsLivraisonCtrForm detailsCtrForm = new DetailsLivraisonCtrForm(this, Convert.ToInt32(txtIdBL.Text), utilisateur);
                detailsCtrForm.ShowDialog();
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
                 vsomAcc = new VSOMAccessors();
                //VsomMarchal vsomAcc = new VsomMarchal();
                 

                if (cbIdDL.IsEnabled == false && txtIdBL.Text != "")
                {
                    CONNAISSEMENT bl = vsomAcc.GetConnaissementByIdBL(Convert.ToInt32(txtIdBL.Text));

                    double eltsBL = 0;
                    double eltsVeh = 0;
                    eltsBL = Math.Round(bl.ELEMENT_FACTURATION.Where(el => el.StatutEF != "Annule" && !el.IdVeh.HasValue && !el.IdCtr.HasValue && !el.IdGC.HasValue && !el.IdMafi.HasValue).Sum(elt => elt.PUEF.Value * elt.QTEEF.Value), 0, MidpointRounding.AwayFromZero) + Math.Round(bl.ELEMENT_FACTURATION.Where(el => el.StatutEF != "Annule" && !el.IdVeh.HasValue && !el.IdCtr.HasValue && !el.IdGC.HasValue && !el.IdMafi.HasValue).Sum(elt => elt.PUEF.Value * elt.QTEEF.Value * (elt.TauxTVA.Value / 100)), 0, MidpointRounding.AwayFromZero);
                    eltsVeh = Math.Round(dataGridVehicules.SelectedItems.OfType<VEHICULE>().Sum(veh => veh.ELEMENT_FACTURATION.Where(el => el.StatutEF != "Annule").Sum(elt => elt.PUEF * elt.QTEEF)).Value, 0, MidpointRounding.AwayFromZero) + Math.Round(dataGridVehicules.SelectedItems.OfType<VEHICULE>().Sum(veh => veh.ELEMENT_FACTURATION.Where(el => el.StatutEF != "Annule").Sum(elt => elt.PUEF * elt.QTEEF * (elt.TauxTVA.Value / 100))).Value, 0, MidpointRounding.AwayFromZero);

                    double eltsVehAutreAcc = Math.Round(dataGridVehicules.SelectedItems.OfType<VEHICULE>().Sum(veh => veh.ELEMENT_FACTURATION.Where(el => el.StatutEF != "Annule" && (el.LIGNE_PRIX.CodeArticle == 1604 || el.LIGNE_PRIX.CodeArticle == 1605 || el.LIGNE_PRIX.CodeArticle == 1707 || el.LIGNE_PRIX.CodeArticle == 1815 || el.LIGNE_PRIX.CodeArticle == 1820 || el.LIGNE_PRIX.CodeArticle == 1801)).Sum(elt => elt.PUEF * elt.QTEEF)).Value, 0, MidpointRounding.AwayFromZero) + Math.Round(dataGridVehicules.SelectedItems.OfType<VEHICULE>().Sum(veh => veh.ELEMENT_FACTURATION.Where(el => el.StatutEF != "Annule" && (el.LIGNE_PRIX.CodeArticle == 1604 || el.LIGNE_PRIX.CodeArticle == 1605 || el.LIGNE_PRIX.CodeArticle == 1707 || el.LIGNE_PRIX.CodeArticle == 1815 || el.LIGNE_PRIX.CodeArticle == 1820 || el.LIGNE_PRIX.CodeArticle == 1801)).Sum(elt => elt.PUEF * elt.QTEEF * (elt.TauxTVA.Value / 100))).Value, 0, MidpointRounding.AwayFromZero);

                    double payBL = vsomAcc.GetPaiementsOfConnaissement(bl.IdBL).Sum(pay => pay.MAPay.Value);

                    double somme = eltsBL + eltsVeh;
                    int sompay = (int)vsomAcc.GetPaiementsFactureOfConnaissement(bl.IdBL).Sum(pay => pay.MAPay);

                    if (somme > (100 + sompay) && bl.ESCALE.IdAcc == 1 && bl.IdClient == 1 /* && MessageBox.Show("Le compte de ce BL n'est pas soldé. Voulez-vous tout de même établir ce BAE ?", "Compte BL non soldé !", MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.No*/)
                    {
                        MessageBox.Show(string.Format("Le compte de ce BL n'est pas soldé, il ne vous est donc pas possible d'établir ce document \n Element facturation : {0} \n Payement {1}", somme, sompay), "Compte BL non soldé !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                    else if ((eltsVehAutreAcc) > (100 + sompay) && bl.ESCALE.IdAcc != 1 && bl.IdClient == 1/* && MessageBox.Show("Le compte de ce BL n'est pas soldé. Voulez-vous tout de même établir ce BAE ?", "Compte BL non soldé !", MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.No*/)
                    {
                        MessageBox.Show(string.Format("Le compte de ce BL n'est pas soldé, il ne vous est donc pas possible d'établir ce document \n Element Vehicule autre Acconnier : {0} \n Payement associée(+100) : {1}", eltsVehAutreAcc, (100 + sompay)), "Compte BL non soldé !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    } 
                    else if (dataGridVehicules.SelectedItems.Count == 0 && dataGridConteneurs.SelectedItems.Count == 0 && dataGridGC.SelectedItems.Count == 0 && dataGridMafis.SelectedItems.Count == 0)
                    {
                        MessageBox.Show("Veuillez sélectionner au moins une marchandise pour établir ce BL", "Sélectionnez des marchandises !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                    else if (dataGridMafis.SelectedItems.OfType<MAFI>().Count(mf => mf.NumAVIMafi == null) != 0)
                    {
                        MessageBox.Show("Vous devez saisir le numéro SGS (AVI) de tous les mafis sélectionnés de ce BL", "N° SGS !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                    else if (dataGridMafis.SelectedItems.OfType<MAFI>().Count(mf => mf.NumADDMafi == null) != 0)
                    {
                        MessageBox.Show("Vous devez saisir le numéro d'attestation de dédouanement de tous les mafis sélectionnés de ce BL", "N° Attestation de dédouanement !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                    else if (dataGridGC.SelectedItems.OfType<CONVENTIONNEL>().Count(gc => gc.NumCIVIOGC == null) != 0)
                    {
                        MessageBox.Show("Vous devez saisir le numéro SGS (AVI) de tous les conventionnels sélectionnés de ce BL", "N° SGS !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                    else if (dataGridGC.SelectedItems.OfType<CONVENTIONNEL>().Count(gc => gc.NumADDGC == null) != 0)
                    {
                        MessageBox.Show("Vous devez saisir le numéro d'attestation de dédouanement de tous les conventionnels sélectionnés de ce BL", "N° Attestation de dédouanement !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                    else if (dataGridConteneurs.SelectedItems.OfType<CONTENEUR>().Count(ctr => ctr.NumAVICtr == null) != 0)
                    {
                        MessageBox.Show("Vous devez saisir le numéro SGS (AVI) de tous les conteneurs sélectionnés de ce BL", "N° SGS !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                    else if (dataGridConteneurs.SelectedItems.OfType<CONTENEUR>().Count(ctr => ctr.NumADDCtr == null) != 0)
                    {
                        MessageBox.Show("Vous devez saisir le numéro d'attestation de dédouanement de tous les conteneurs sélectionnés de ce BL", "N° Attestation de dédouanement !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                    else if (dataGridVehicules.SelectedItems.OfType<VEHICULE>().Count(veh => veh.NumCIVIOveh == null) != 0)
                    {
                        MessageBox.Show("Vous devez saisir le numéro SGS (CIVIO) de tous les véhicules sélectionnés de ce BL", "N° SGS !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                    else if (dataGridVehicules.SelectedItems.OfType<VEHICULE>().Count(veh => veh.NumADDVeh == null) != 0)
                    {
                        MessageBox.Show("Vous devez saisir le numéro d'attestation de dédouanement de tous les véhicules sélectionnés de ce BL", "N° Attestation de dédouanement !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                    else
                    {
                        DEMANDE_LIVRAISON liv = vsomAcc.InsertDemandeLivraison(bl.IdBL, dataGridVehicules.SelectedItems.OfType<VEHICULE>().ToList<VEHICULE>(), dataGridConteneurs.SelectedItems.OfType<CONTENEUR>().ToList<CONTENEUR>(), dataGridGC.SelectedItems.OfType<CONVENTIONNEL>().ToList<CONVENTIONNEL>(), dataGridMafis.SelectedItems.OfType<MAFI>().ToList<MAFI>(), new TextRange(txtObservations.Document.ContentStart, txtObservations.Document.ContentEnd).Text, utilisateur.IdU);

                        if (demandeLivraisonPanel.cbFiltres.SelectedIndex != 2)
                        {
                            demandeLivraisonPanel.cbFiltres.SelectedIndex = 2;
                        }
                        else
                        {
                            demandeLivraisonPanel.demandesLivraison = vsomAcc.GetDemandesLivraisonEnAttente();
                            demandeLivraisonPanel.dataGrid.ItemsSource = demandeLivraisonPanel.demandesLivraison;
                            demandeLivraisonPanel.lblStatut.Content = demandeLivraisonPanel.demandesLivraison.Count + " Demande(s) de livraison";
                        }

                        formLoader.LoadDemandeLivraisonForm(this, liv);

                        btnValiderDL.Visibility = System.Windows.Visibility.Visible;
                        //borderEtat.Visibility = System.Windows.Visibility.Visible;

                        MessageBox.Show("Enregistrement effectué avec succès.", "Enregistrement effectué !", MessageBoxButton.OK, MessageBoxImage.Information);
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

        private void btnValiderDL_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //VSOMAccessors vsomAcc = new VSOMAccessors();

                if (operationsUser.Where(op => op.NomOp == "Demande de livraison : Validation d'un élément").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
                {
                    MessageBox.Show("Vous n'avez pas les droits nécessaires pour valider une demande de livraison. Veuillez contacter un administrateur", "Droits insuffisants !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    ValiderDemandeLivraisonForm validForm = new ValiderDemandeLivraisonForm(this, utilisateur);
                    validForm.Title = "Validation de la demande de livraison : " + cbIdDL.Text + " - Consignée " + txtConsignee.Text;
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
                  vsomAcc = new VSOMAccessors();

                if (e.Key == Key.Return && cbNumBL.Text.Trim() != "")
                {
                    if (utilisateur.IdAcc == 1)
                    {
                        connaissements = vsomAcc.GetConnaissementPourLivraisonByNumBL(cbNumBL.Text);
                        //connaissements = vsomAcc.GetConnaissementByNumBL(cbNumBL.Text);
                    }
                    else
                    {
                        connaissements = vsomAcc.GetConnaissementPourLivraisonByNumBL(cbNumBL.Text, utilisateur.IdAcc.Value);
                    }

                    if (connaissements.Count == 0)
                    {
                        MessageBox.Show("Il n'existe aucun connaissement ayant un bon à enlever validé portant ce numéro", "Connaissement introuvable", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                    //if (connaissements.Count == 0)
                    //{
                    //    MessageBox.Show("Il n'existe aucun connaissement portant ce numéro", "Connaissement introuvable", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    //}
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

        private void btnEtat_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LivraisonReport livReport = new LivraisonReport(this);
                livReport.Title = "Impression de la demande de livraison : " + cbIdDL.Text + " - Escale : " + cbNumEsc.Text;
                livReport.Show();
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

        private void btnMAJDateDepot_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                 vsomAcc = new VSOMAccessors();

                DEMANDE_LIVRAISON liv = vsomAcc.GetDemandeLivraisonByIdDBL(Convert.ToInt32(cbIdDL.Text));

                MAJDateDepotForm mAJDateDepotForm = new MAJDateDepotForm(this, utilisateur);
                if (liv.DateDepotDBL.HasValue)
                {
                    mAJDateDepotForm.txtDateDepot.SelectedDate = liv.DateDepotDBL.Value;
                }
                mAJDateDepotForm.Title = "Mise à jour date de depot - DL N° : " + cbIdDL.Text;
                mAJDateDepotForm.ShowDialog();
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

        private void btnDetailsGC_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DetailsLivraisonGCForm detailsGCForm = new DetailsLivraisonGCForm(this, Convert.ToInt32(txtIdBL.Text), utilisateur);
                detailsGCForm.ShowDialog();
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        private void btnDetailsMafi_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DetailsLivraisonMafiForm detailsMafiForm = new DetailsLivraisonMafiForm(this, Convert.ToInt32(txtIdBL.Text), utilisateur);
                detailsMafiForm.ShowDialog();
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        private void btnGenererCoreor_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                vsomAcc = new VSOMAccessors();

                DEMANDE_LIVRAISON demandeLivraison = vsomAcc.GetDemandeLivraisonByIdDBL(Convert.ToInt32(cbIdDL.Text));

                StringBuilder sb = new StringBuilder();
                DateTime dte = DateTime.Now;

                int i = 1;
                sb.Append("UNB+UNOA:2+").Append(demandeLivraison.CONNAISSEMENT.ESCALE.ARMATEUR.CodeArm == "INARME" ? "GRIMALDI" : demandeLivraison.CONNAISSEMENT.ESCALE.ARMATEUR.CodeArm).Append("+DITCAMDLA+" + demandeLivraison.DateDBL.Value.Year.ToString().Substring(2, 2) + FormatChiffre(demandeLivraison.DateDBL.Value.Month) + FormatChiffre(demandeLivraison.DateDBL.Value.Day) + ":" + FormatChiffre(demandeLivraison.DateDBL.Value.Hour) + FormatChiffre(demandeLivraison.DateDBL.Value.Minute) + "+" + demandeLivraison.IdDBL + "'").Append(Environment.NewLine);
                foreach (CONTENEUR ctr in demandeLivraison.CONTENEUR)
                {
                    sb.Append("UNH+" + demandeLivraison.IdDBL + "+COREOR:D:95B:UN'").Append(Environment.NewLine);
                    sb.Append("BGM+12+" + demandeLivraison.IdDBL + "+9'").Append(Environment.NewLine);
                    sb.Append("RFF+REO:DO-" + demandeLivraison.IdDBL + "-" + demandeLivraison.DateDBL.Value.Year + "-" + demandeLivraison.CONNAISSEMENT.ESCALE.ARMATEUR.CodeArm + "'").Append(Environment.NewLine);
                    sb.Append("TDT+20+" + demandeLivraison.CONNAISSEMENT.ESCALE.NumVoySCR + "+1++" + demandeLivraison.CONNAISSEMENT.ESCALE.ARMATEUR.CodeArm + ":172:20+++" + demandeLivraison.CONNAISSEMENT.ESCALE.NAVIRE.CodeRadio + ":103::" + demandeLivraison.CONNAISSEMENT.ESCALE.NAVIRE.NomNav + "'").Append(Environment.NewLine);
                    sb.Append("NAD+CA+").Append(demandeLivraison.CONNAISSEMENT.ESCALE.ARMATEUR.CodeArm == "INARME" ? "GRIMALDI" : demandeLivraison.CONNAISSEMENT.ESCALE.ARMATEUR.CodeArm).Append(":172:" + ctr.TypeCCtr.Substring(0, 2) + "'").Append(Environment.NewLine);
                    sb.Append("NAD+CF+").Append(demandeLivraison.CONNAISSEMENT.ESCALE.ARMATEUR.CodeArm == "INARME" ? "GRIMALDI" : demandeLivraison.CONNAISSEMENT.ESCALE.ARMATEUR.CodeArm).Append(":172" + ctr.TypeCCtr.Substring(0, 2) + "'").Append(Environment.NewLine);

                    if (ctr.TypeCCtr == "20BX" || ctr.TypeCCtr == "20DV")
                    {
                        sb.Append("EQD+CN+" + ctr.NumCtr + "+22G1:102:5+2+3+5'").Append(Environment.NewLine);
                    }
                    else if (ctr.TypeCCtr == "40BX" || ctr.TypeCCtr == "40DV")
                    {
                        sb.Append("EQD+CN+" + ctr.NumCtr + "+42G1:102:5+2+3+5'").Append(Environment.NewLine);
                    }
                    else if (ctr.TypeCCtr == "40HC")
                    {
                        sb.Append("EQD+CN+" + ctr.NumCtr + "+45G1:102:5+2+3+5'").Append(Environment.NewLine);
                    }
                    else if (ctr.TypeCCtr == "40OT")
                    {
                        sb.Append("EQD+CN+" + ctr.NumCtr + "+42U1:102:5+2+3+5'").Append(Environment.NewLine);
                    }
                    else if (ctr.TypeCCtr == "40FL")
                    {
                        sb.Append("EQD+CN+" + ctr.NumCtr + "+45P3:102:5+2+3+5'").Append(Environment.NewLine);
                    }
                    else if (ctr.TypeCCtr == "20OT")
                    {
                        sb.Append("EQD+CN+" + ctr.NumCtr + "+22U1:102:5+2+3+5'").Append(Environment.NewLine);
                    }
                    else
                    {
                        sb.Append("EQD+CN+" + ctr.NumCtr + "+" + ctr.TypeCCtr + ":102:5+2+3+5'").Append(Environment.NewLine);
                    }

                    sb.Append("MEA+AAE+G+KGM:" + ctr.PoidsCCtr + "'").Append(Environment.NewLine);
                    if (ctr.TypeCCtr.Substring(0, 2) == "20")
                    {
                        sb.Append("MEA+AAE+T+KGM:2280'").Append(Environment.NewLine);
                    }
                    else if (ctr.TypeCCtr.Substring(0, 2) == "40")
                    {
                        sb.Append("MEA+AAE+T+KGM:4480'").Append(Environment.NewLine);
                    }

                    sb.Append("RFF+TF:DO-" + demandeLivraison.IdDBL + "-" + demandeLivraison.DateDBL.Value.Year + "-" + demandeLivraison.CONNAISSEMENT.ESCALE.ARMATEUR.CodeArm + "'").Append(Environment.NewLine);
                    sb.Append("DTM+400:" + ctr.FSCtr.Value.Year + FormatChiffre(ctr.FSCtr.Value.Month) + FormatChiffre(ctr.FSCtr.Value.Day) + "1200:203'").Append(Environment.NewLine);
                    sb.Append("CNT+16:1'").Append(Environment.NewLine);
                    sb.Append("UNT+13+" + demandeLivraison.IdDBL + FormatReferenceCOREOR(i) + "'").Append(Environment.NewLine);
                    i++;
                }

                sb.Append("UNZ+3+" + demandeLivraison.IdDBL).Append(Environment.NewLine);

                System.IO.File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\COREOR - " + demandeLivraison.IdDBL + " - " + demandeLivraison.DateDBL.Value.Year + FormatChiffre(demandeLivraison.DateDBL.Value.Month) + FormatChiffre(demandeLivraison.DateDBL.Value.Day) + FormatChiffre(demandeLivraison.DateDBL.Value.Hour) + FormatChiffre(demandeLivraison.DateDBL.Value.Minute) + FormatChiffre(demandeLivraison.DateDBL.Value.Second) + ".EDI", sb.ToString(), Encoding.GetEncoding("ISO-8859-1"));

                Microsoft.Office.Interop.Outlook.Application app = new Microsoft.Office.Interop.Outlook.Application();
                Microsoft.Office.Interop.Outlook.MailItem mailItem = app.CreateItem(Microsoft.Office.Interop.Outlook.OlItemType.olMailItem);
                mailItem.Subject = "COREOR (Socomar)";
                mailItem.To = "edi.tedi@socomarcm.net";
                mailItem.Body = "COREOR (Socomar)";
                mailItem.Attachments.Add(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\COREOR - " + demandeLivraison.IdDBL + " - " + demandeLivraison.DateDBL.Value.Year + FormatChiffre(demandeLivraison.DateDBL.Value.Month) + FormatChiffre(demandeLivraison.DateDBL.Value.Day) + FormatChiffre(demandeLivraison.DateDBL.Value.Hour) + FormatChiffre(demandeLivraison.DateDBL.Value.Minute) + FormatChiffre(demandeLivraison.DateDBL.Value.Second) + ".EDI");
                mailItem.Importance = Microsoft.Office.Interop.Outlook.OlImportance.olImportanceHigh;
                mailItem.Display(false);
                mailItem.Send();

                MessageBox.Show("COREOR généré", "COREOR généré !", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private static string FormatReferenceCOREOR(int entier)
        {
            Int32 i = entier;
            if (i >= 10000)
            {
                return i.ToString();
            }
            else if (i >= 1000)
            {
                return "0" + i.ToString();
            }
            else if (i >= 100)
            {
                return "00" + i.ToString();
            }
            else if (i >= 10)
            {
                return "000" + i.ToString();
            }
            else
            {
                return "0000" + i.ToString();
            }
        }

        private static string FormatChiffre(int entier)
        {
            Int32 i = entier;
            if (i >= 10)
            {
                return i.ToString();
            }
            else
            {
                return "0" + i.ToString();
            }
        }
    }
}
