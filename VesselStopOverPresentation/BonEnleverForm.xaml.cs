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
using System.Net.Http;
using System.Net.Http.Headers;

namespace VesselStopOverPresentation
{
    /// <summary>
    /// Logique d'interaction pour BonEnleverForm.xaml
    /// </summary>
    public partial class BonEnleverForm : Window
    {
        public List<CONNAISSEMENT> connaissements { get; set; }
        public List<string> cons { get; set; }

        public List<CLIENT> clients { get; set; }
        public List<string> clts { get; set; }

        public List<ESCALE> escales { get; set; }
        public List<Int32> escs { get; set; }

        public List<BON_ENLEVEMENT> bonsEnlevements { get; set; }
        public List<Int32> bonsEnlever { get; set; }

        public List<NAVIRE> navires { get; set; }
        public List<string> navs { get; set; }

        public List<VEHICULE> vehicules { get; set; }
        public List<CONTENEUR> conteneurs { get; set; }
        public List<MAFI> mafis { get; set; }
        public List<CONVENTIONNEL> conventionnels { get; set; }

        public List<ElementFacturation> eltsFact { get; set; }

        private UTILISATEUR utilisateur;
        private List<OPERATION> operationsUser;

        public BonEnleverPanel baePanel { get; set; }

        private FormLoader formLoader;
        //private VsomParameters vsp = new VsomParameters();
        private VSOMAccessors vsomAcc;
        public BonEnleverForm(BonEnleverPanel panel, BON_ENLEVEMENT bae, UTILISATEUR user)
        {
            try
            {
                InitializeComponent();
                //using (var ctx = new VSOMClassesDataContext())
                //{
                   // VSOMAccessors vs = new VSOMAccessors(ctx);
                    vsomAcc = new VSOMAccessors();
                    //VsomParameters vsp = new VsomParameters();
                   
                    this.DataContext = this;

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

                    baePanel = panel;

                    utilisateur = user;
                    operationsUser = vsomAcc.GetOperationsUtilisateur(utilisateur.IdU);

                    formLoader = new FormLoader(utilisateur);

                    formLoader.LoadBonEnleverForm(this, bae);
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

        public BonEnleverForm(string type, BonEnleverPanel panel, UTILISATEUR user)
        {
            try
            {
                InitializeComponent();
               // using (var ctx = new VSOMClassesDataContext())
               // {
                    //VSOMAccessors vs = new VSOMAccessors(ctx);
                    vsomAcc = new VSOMAccessors();
                    //VsomParameters vsp = new VsomParameters();
                    
                    this.DataContext = this;

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

                    baePanel = panel;

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
        private void Enregistrer2(object sender, RoutedEventArgs e)
        {
            try
            {
                if (cbIdBAE.IsEnabled == false && txtIdBL.Text.Trim() != "")
                {

                    if (dataGridVehicules.SelectedItems.Count == 0 && dataGridConteneurs.SelectedItems.Count == 0 && dataGridGC.SelectedItems.Count == 0 && dataGridMafis.SelectedItems.Count == 0)
                    {
                        MessageBox.Show("Veuillez sélectionner au moins une marchandise pour établir ce BAE", "Sélectionnez des marchandises !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                    else
                    {
                        HttpClient client = new HttpClient();
                        client.BaseAddress = new Uri("http://localhost:12789/");
                        //client.DefaultRequestHeaders.Add("appkey", "myapp_key");
                        client.DefaultRequestHeaders.Accept.Add(
                           new MediaTypeWithQualityHeaderValue("application/json"));

                        HttpResponseMessage response = client.GetAsync("api/employee").Result;
                        if (response.IsSuccessStatusCode)
                        {
                            //var employees = response.Content.ReadAsAsync<IEnumerable<Employee>>().Result;
                            

                        }
                        else
                        {
                            MessageBox.Show("Error Code" + response.StatusCode + " : Message - " + response.ReasonPhrase);
                        }
                    }
                }
            }
            catch (Exception ex)
            { 
            
            }
        }

        private void btnEnregistrer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //using (var ctx = new VSOMClassesDataContext())
                //{
                    //VSOMAccessors vs = new VSOMAccessors(ctx);
                    vsomAcc = new VSOMAccessors();
                    // VsomMarchal vsomAcc = new VsomMarchal();

                    if (cbIdBAE.IsEnabled == false && txtIdBL.Text.Trim() != "")
                    {
                        
                        if (dataGridVehicules.SelectedItems.Count == 0 && dataGridConteneurs.SelectedItems.Count == 0 && dataGridGC.SelectedItems.Count == 0 && dataGridMafis.SelectedItems.Count == 0)
                        {
                            MessageBox.Show("Veuillez sélectionner au moins une marchandise pour établir ce BAE", "Sélectionnez des marchandises !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        }
                        else
                        {
                            CONNAISSEMENT bl = vsomAcc.GetConnaissementByIdBL(Convert.ToInt32(txtIdBL.Text));

                            double eltsBL = 0;
                            double eltsVeh = 0;
                            /*AH8nov16 prendre en consideration le taux element facturation et non le taux code_tva 
                             double eltsBL = Math.Round(bl.ELEMENT_FACTURATION.Where(el => el.StatutEF != "Annule" && !el.IdVeh.HasValue && !el.IdCtr.HasValue && !el.IdGC.HasValue && !el.IdMafi.HasValue).Sum(elt => elt.PUEF.Value * elt.QTEEF.Value), 0, MidpointRounding.AwayFromZero) + Math.Round(bl.ELEMENT_FACTURATION.Where(el => el.StatutEF != "Annule" && !el.IdVeh.HasValue && !el.IdCtr.HasValue && !el.IdGC.HasValue && !el.IdMafi.HasValue).Sum(elt => elt.PUEF.Value * elt.QTEEF.Value * (elt.CODE_TVA.TauxTVA.Value / 100)), 0, MidpointRounding.AwayFromZero);
                             */

                            //if (DateTime.Today >= DateTime.Parse("18/12/2016") && DateTime.Today <= DateTime.Parse("24/12/2016") && bl.ESCALE.IdArm == 1 && bl.ESCALE.DRAEsc <= DateTime.Parse("31/08/2016"))
                            //{
                            //    eltsBL = Math.Round(bl.ELEMENT_FACTURATION.Where(el => el.StatutEF != "Annule" && el.REF_IDJEF.HasValue == false && !el.IdVeh.HasValue && !el.IdCtr.HasValue && !el.IdGC.HasValue && !el.IdMafi.HasValue).Sum(elt => elt.PUEF.Value * elt.QTEEF.Value), 0, MidpointRounding.AwayFromZero) + Math.Round(bl.ELEMENT_FACTURATION.Where(el => el.StatutEF != "Annule" && el.REF_IDJEF.HasValue == false && !el.IdVeh.HasValue && !el.IdCtr.HasValue && !el.IdGC.HasValue && !el.IdMafi.HasValue).Sum(elt => elt.PUEF.Value * elt.QTEEF.Value * (elt.TauxTVA.Value / 100)), 0, MidpointRounding.AwayFromZero);

                            //}
                            //else
                            //{
                                eltsBL = Math.Round(bl.ELEMENT_FACTURATION.Where(el => el.StatutEF != "Annule" && !el.IdVeh.HasValue && !el.IdCtr.HasValue && !el.IdGC.HasValue && !el.IdMafi.HasValue).Sum(elt => elt.PUEF.Value * elt.QTEEF.Value), 0, MidpointRounding.AwayFromZero) + Math.Round(bl.ELEMENT_FACTURATION.Where(el => el.StatutEF != "Annule" && !el.IdVeh.HasValue && !el.IdCtr.HasValue && !el.IdGC.HasValue && !el.IdMafi.HasValue).Sum(elt => elt.PUEF.Value * elt.QTEEF.Value * (elt.TauxTVA.Value / 100)), 0, MidpointRounding.AwayFromZero);

                            //}

                            /*AH8nov16 prendre en consideration le taux element facturation et non le taux code_tva 
                            double eltsVeh = Math.Round(dataGridVehicules.SelectedItems.OfType<VEHICULE>().Sum(veh => veh.ELEMENT_FACTURATION.Where(el => el.StatutEF != "Annule").Sum(elt => elt.PUEF * elt.QTEEF)).Value, 0, MidpointRounding.AwayFromZero) + Math.Round(dataGridVehicules.SelectedItems.OfType<VEHICULE>().Sum(veh => veh.ELEMENT_FACTURATION.Where(el => el.StatutEF != "Annule").Sum(elt => elt.PUEF * elt.QTEEF * (elt.CODE_TVA.TauxTVA / 100))).Value, 0, MidpointRounding.AwayFromZero);
                            */
                            //if (DateTime.Today >= DateTime.Parse("18/12/2016") && DateTime.Today <= DateTime.Parse("24/12/2016") && bl.ESCALE.IdArm == 1 && bl.ESCALE.DRAEsc <= DateTime.Parse("31/08/2016"))
                            //{
                            //    eltsVeh = Math.Round(dataGridVehicules.SelectedItems.OfType<VEHICULE>().Sum(veh => veh.ELEMENT_FACTURATION.Where(el => el.StatutEF != "Annule" && el.REF_IDJEF.HasValue == false).Sum(elt => elt.PUEF * elt.QTEEF)).Value, 0, MidpointRounding.AwayFromZero) + Math.Round(dataGridVehicules.SelectedItems.OfType<VEHICULE>().Sum(veh => veh.ELEMENT_FACTURATION.Where(el => el.StatutEF != "Annule" && el.REF_IDJEF.HasValue == false).Sum(elt => elt.PUEF * elt.QTEEF * (elt.TauxTVA.Value / 100))).Value, 0, MidpointRounding.AwayFromZero);

                            //}
                            //else
                            //{
                                eltsVeh = Math.Round(dataGridVehicules.SelectedItems.OfType<VEHICULE>().Sum(veh => veh.ELEMENT_FACTURATION.Where(el => el.StatutEF != "Annule").Sum(elt => elt.PUEF * elt.QTEEF)).Value, 0, MidpointRounding.AwayFromZero) + Math.Round(dataGridVehicules.SelectedItems.OfType<VEHICULE>().Sum(veh => veh.ELEMENT_FACTURATION.Where(el => el.StatutEF != "Annule").Sum(elt => elt.PUEF * elt.QTEEF * (elt.TauxTVA.Value / 100))).Value, 0, MidpointRounding.AwayFromZero);

                            //}

                            /*AH8nov16 prendre en consideration le taux element facturation et non le taux code_tva 
                            double eltsVehAutreAcc = Math.Round(dataGridVehicules.SelectedItems.OfType<VEHICULE>().Sum(veh => veh.ELEMENT_FACTURATION.Where(el => el.StatutEF != "Annule" && (el.LIGNE_PRIX.CodeArticle == 1604 || el.LIGNE_PRIX.CodeArticle == 1605 || el.LIGNE_PRIX.CodeArticle == 1707 || el.LIGNE_PRIX.CodeArticle == 1815 || el.LIGNE_PRIX.CodeArticle == 1820 || el.LIGNE_PRIX.CodeArticle == 1801)).Sum(elt => elt.PUEF * elt.QTEEF)).Value, 0, MidpointRounding.AwayFromZero) + Math.Round(dataGridVehicules.SelectedItems.OfType<VEHICULE>().Sum(veh => veh.ELEMENT_FACTURATION.Where(el => el.StatutEF != "Annule" && (el.LIGNE_PRIX.CodeArticle == 1604 || el.LIGNE_PRIX.CodeArticle == 1605 || el.LIGNE_PRIX.CodeArticle == 1707 || el.LIGNE_PRIX.CodeArticle == 1815 || el.LIGNE_PRIX.CodeArticle == 1820 || el.LIGNE_PRIX.CodeArticle == 1801)).Sum(elt => elt.PUEF * elt.QTEEF * (elt.CODE_TVA.TauxTVA / 100))).Value, 0, MidpointRounding.AwayFromZero);
                            */
                            double eltsVehAutreAcc = Math.Round(dataGridVehicules.SelectedItems.OfType<VEHICULE>().Sum(veh => veh.ELEMENT_FACTURATION.Where(el => el.StatutEF != "Annule" && (el.LIGNE_PRIX.CodeArticle == 1604 || el.LIGNE_PRIX.CodeArticle == 1605 || el.LIGNE_PRIX.CodeArticle == 1707 || el.LIGNE_PRIX.CodeArticle == 1815 || el.LIGNE_PRIX.CodeArticle == 1820 || el.LIGNE_PRIX.CodeArticle == 1801)).Sum(elt => elt.PUEF * elt.QTEEF)).Value, 0, MidpointRounding.AwayFromZero) + Math.Round(dataGridVehicules.SelectedItems.OfType<VEHICULE>().Sum(veh => veh.ELEMENT_FACTURATION.Where(el => el.StatutEF != "Annule" && (el.LIGNE_PRIX.CodeArticle == 1604 || el.LIGNE_PRIX.CodeArticle == 1605 || el.LIGNE_PRIX.CodeArticle == 1707 || el.LIGNE_PRIX.CodeArticle == 1815 || el.LIGNE_PRIX.CodeArticle == 1820 || el.LIGNE_PRIX.CodeArticle == 1801)).Sum(elt => elt.PUEF * elt.QTEEF * (elt.TauxTVA.Value / 100))).Value, 0, MidpointRounding.AwayFromZero);

                            double eltsCtr = Math.Round(dataGridConteneurs.SelectedItems.OfType<CONTENEUR>().Sum(ctr => ctr.ELEMENT_FACTURATION.Where(el => el.StatutEF != "Annule").Sum(elt => elt.PUEF * elt.QTEEF)).Value, 0, MidpointRounding.AwayFromZero) + Math.Round(dataGridConteneurs.SelectedItems.OfType<CONTENEUR>().Sum(ctr => ctr.ELEMENT_FACTURATION.Where(el => el.StatutEF != "Annule").Sum(elt => elt.PUEF * elt.QTEEF * (elt.CODE_TVA.TauxTVA / 100))).Value, 0, MidpointRounding.AwayFromZero);
                            double eltsMafi = Math.Round(dataGridConteneurs.SelectedItems.OfType<MAFI>().Sum(mf => mf.ELEMENT_FACTURATION.Where(el => el.StatutEF != "Annule").Sum(elt => elt.PUEF * elt.QTEEF)).Value, 0, MidpointRounding.AwayFromZero) + Math.Round(dataGridMafis.SelectedItems.OfType<MAFI>().Sum(mf => mf.ELEMENT_FACTURATION.Where(el => el.StatutEF != "Annule").Sum(elt => elt.PUEF * elt.QTEEF * (elt.CODE_TVA.TauxTVA / 100))).Value, 0, MidpointRounding.AwayFromZero);
                            double eltsGC = Math.Round(dataGridGC.SelectedItems.OfType<CONVENTIONNEL>().Sum(conv => conv.ELEMENT_FACTURATION.Where(el => el.StatutEF != "Annule").Sum(elt => elt.PUEF * elt.QTEEF)).Value, 0, MidpointRounding.AwayFromZero) + Math.Round(dataGridGC.SelectedItems.OfType<CONVENTIONNEL>().Sum(conv => conv.ELEMENT_FACTURATION.Where(el => el.StatutEF != "Annule").Sum(elt => elt.PUEF * elt.QTEEF * (elt.CODE_TVA.TauxTVA / 100))).Value, 0, MidpointRounding.AwayFromZero);

                            double payBL = vsomAcc.GetPaiementsOfConnaissement(bl.IdBL).Sum(pay => pay.MAPay.Value);

                            double debit = dataGridCompteBL.Items.OfType<ElementCompte>().Sum(el => el.Debit);
                            double credit = dataGridCompteBL.Items.OfType<ElementCompte>().Sum(el => el.Credit);
                            double somme = eltsBL + eltsVeh + eltsCtr + eltsGC + eltsMafi;
                            int sompay = (int)vsomAcc.GetPaiementsFactureOfConnaissement(bl.IdBL).Sum(pay => pay.MAPay);



                            if (bl.BlBloque == "Y")
                            {
                                MessageBox.Show("Vous ne pouvez pas enregistrer un bon d'enlèvement sur un connaissement bloqué", "BL Bloqué !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                            }
                            else if (somme > (100 + sompay) && bl.ESCALE.IdAcc == 1 && bl.IdClient == 1 /* && MessageBox.Show("Le compte de ce BL n'est pas soldé. Voulez-vous tout de même établir ce BAE ?", "Compte BL non soldé !", MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.No*/)
                            {
                                MessageBox.Show(string.Format("Le compte de ce BL n'est pas soldé, il ne vous est donc pas possible d'établir un bon à enlever \n Element facturation : {0} \n Payement {1}", somme, sompay), "Compte BL non soldé !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                            }
                            else if ((eltsVehAutreAcc) > (100 + sompay) && bl.ESCALE.IdAcc != 1 && bl.IdClient == 1/* && MessageBox.Show("Le compte de ce BL n'est pas soldé. Voulez-vous tout de même établir ce BAE ?", "Compte BL non soldé !", MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.No*/)
                            {
                                MessageBox.Show(string.Format("Le compte de ce BL n'est pas soldé, il ne vous est donc pas possible d'établir un bon à enlever \n Element Vehicule autre Acconnier : {0} \n Payement associée(+100) : {1}", eltsVehAutreAcc, (100 + sompay)), "Compte BL non soldé !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                            }
                            //else if ((debit - credit) > 10 && bl.ESCALE.IdAcc == 1 && bl.IdClient == 1)
                            //{
                            //    MessageBox.Show("Le compte de ce BL n'est pas soldé, il ne vous est donc pas possible d'établir un bon à enlever", "Compte BL non soldé !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                            //}
                            else if (dataGridConteneurs.SelectedItems.Count != 0)
                            {
                                #region conteneure
                                List<CONTENEUR> ctrsSelect = dataGridConteneurs.SelectedItems.OfType<CONTENEUR>().ToList<CONTENEUR>();
                                if (ctrsSelect.Count(ctr => ctr.NumBESCCtr == null) != 0)
                                {
                                    MessageBox.Show("Veuillez remplir toutes les informations d'enlèvement sur les conteneurs sélectionnés", "Informations d'enlèvement !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                                }
                                else
                                {
                                    int cautionPayee = dataGridConteneurs.SelectedItems.OfType<CONTENEUR>().Where(ctr => ctr.IdPay.HasValue).Sum(ctr => ctr.PAYEMENT.MAPay).Value;
                                    int montantCaution = dataGridCautions.SelectedItems.OfType<CONTENEUR>().Sum(ctr => ctr.MCCtr).Value;
                                    if (cautionPayee > 0 && cautionPayee < montantCaution)
                                    {
                                        MessageBox.Show("Le montant payé pour la caution est inférieur au montant de caution dû pour les conteneurs sélectionnés. Par conséquent, vous ne pouvez pas établir ce bon à enlever", "Caution insuffisante !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                                    }
                                    else if (cautionPayee == 0 && montantCaution != 0)
                                    {
                                        if (txtEmetteur.Text.Trim() == "")
                                        {
                                            MessageBox.Show("Vous devez saisir le nom de l'émetteur de la lettre de garantie", "Emetteur de la lettre de garantie ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                                        }
                                        else if (txtDestinataire.Text.Trim() == "")
                                        {
                                            MessageBox.Show("Vous devez saisir le nom de le destinataire de la lettre de garantie", "Destinataire de la lettre de garantie ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                                        }
                                        else if (!txtDateEmission.SelectedDate.HasValue)
                                        {
                                            MessageBox.Show("Vous devez saisir la date d'émission de la lettre de garantie", "Date d'émission de la lettre de garantie ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                                        }
                                        else if (!txtDateFinValidite.SelectedDate.HasValue)
                                        {
                                            MessageBox.Show("Vous devez saisir la date de fin de validité de la lettre de garantie", "Date de fin de validité de la lettre de garantie ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                                        }
                                        else
                                        {
                                            BON_ENLEVEMENT bae = vsomAcc.InsertBonEnlevement(bl.IdBL, txtEmetteur.Text, txtDestinataire.Text, txtDateEmission.SelectedDate.Value, txtDateFinValidite.SelectedDate.Value, dataGridVehicules.SelectedItems.OfType<VEHICULE>().ToList<VEHICULE>(), dataGridConteneurs.SelectedItems.OfType<CONTENEUR>().ToList<CONTENEUR>(), dataGridGC.SelectedItems.OfType<CONVENTIONNEL>().ToList<CONVENTIONNEL>(), dataGridMafis.SelectedItems.OfType<MAFI>().ToList<MAFI>(), new TextRange(txtObservations.Document.ContentStart, txtObservations.Document.ContentEnd).Text, utilisateur.IdU);

                                            if (baePanel.cbFiltres.SelectedIndex != 2)
                                            {
                                                baePanel.cbFiltres.SelectedIndex = 2;
                                            }
                                            else
                                            {
                                                baePanel.bonsEnlever = vsomAcc.GetBonsEnleverEnAttente();
                                                baePanel.dataGrid.ItemsSource = baePanel.bonsEnlever;
                                                baePanel.lblStatut.Content = baePanel.bonsEnlever.Count + " Bon(s) à enlever";
                                            }

                                            formLoader.LoadBonEnleverForm(this, bae);

                                            borderActions.Visibility = System.Windows.Visibility.Visible;
                                            btnValiderBAE.Visibility = System.Windows.Visibility.Visible;
                                            //borderEtat.Visibility = System.Windows.Visibility.Visible;
                                            MessageBox.Show("Enregistrement effectué avec succès.", "Enregistrement effectué !", MessageBoxButton.OK, MessageBoxImage.Information);
                                        }
                                    }
                                    else if (cautionPayee >= montantCaution)
                                    {
                                        BON_ENLEVEMENT bae = vsomAcc.InsertBonEnlevement(bl.IdBL, dataGridVehicules.SelectedItems.OfType<VEHICULE>().ToList<VEHICULE>(), dataGridConteneurs.SelectedItems.OfType<CONTENEUR>().ToList<CONTENEUR>(), dataGridGC.SelectedItems.OfType<CONVENTIONNEL>().ToList<CONVENTIONNEL>(), dataGridMafis.SelectedItems.OfType<MAFI>().ToList<MAFI>(), new TextRange(txtObservations.Document.ContentStart, txtObservations.Document.ContentEnd).Text, utilisateur.IdU);

                                        if (baePanel.cbFiltres.SelectedIndex != 2)
                                        {
                                            baePanel.cbFiltres.SelectedIndex = 2;
                                        }
                                        else
                                        {
                                            baePanel.bonsEnlever = vsomAcc.GetBonsEnleverEnAttente();
                                            baePanel.dataGrid.ItemsSource = baePanel.bonsEnlever;
                                            baePanel.lblStatut.Content = baePanel.bonsEnlever.Count + " Bon(s) à enlever";
                                        }

                                        formLoader.LoadBonEnleverForm(this, bae);

                                        borderActions.Visibility = System.Windows.Visibility.Visible;
                                        btnValiderBAE.Visibility = System.Windows.Visibility.Visible;
                                        //borderEtat.Visibility = System.Windows.Visibility.Visible;
                                        MessageBox.Show("Enregistrement effectué avec succès.", "Enregistrement effectué !", MessageBoxButton.OK, MessageBoxImage.Information);
                                    }
                                } 
                                #endregion
                            }
                            else
                            {
                                List<VEHICULE> vehsSelect = dataGridVehicules.SelectedItems.OfType<VEHICULE>().ToList<VEHICULE>();
                                List<CONVENTIONNEL> convsSelect = dataGridGC.SelectedItems.OfType<CONVENTIONNEL>().ToList<CONVENTIONNEL>();
                                List<MAFI> mafisSelect = dataGridMafis.SelectedItems.OfType<MAFI>().ToList<MAFI>();
                                if (vehsSelect.Count(veh => veh.NumBESCVeh == null) != 0)
                                {
                                    MessageBox.Show("Veuillez remplir toutes les informations d'enlèvement sur les véhicules sélectionnés", "Informations d'enlèvement !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                                }
                                else if (convsSelect.Count(gc => gc.NumBESCGC == null) != 0)
                                {
                                    MessageBox.Show("Veuillez remplir toutes les informations d'enlèvement sur les conventionnels sélectionnés", "Informations d'enlèvement !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                                }
                                else if (mafisSelect.Count(mf => mf.NumBESCMafi == null) != 0)
                                {
                                    MessageBox.Show("Veuillez remplir toutes les informations d'enlèvement sur les mafis sélectionnés", "Informations d'enlèvement !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                                }
                                else
                                {
                                    BON_ENLEVEMENT bae = vsomAcc.InsertBonEnlevement(bl.IdBL, dataGridVehicules.SelectedItems.OfType<VEHICULE>().ToList<VEHICULE>(), dataGridConteneurs.SelectedItems.OfType<CONTENEUR>().ToList<CONTENEUR>(), dataGridGC.SelectedItems.OfType<CONVENTIONNEL>().ToList<CONVENTIONNEL>(), dataGridMafis.SelectedItems.OfType<MAFI>().ToList<MAFI>(), new TextRange(txtObservations.Document.ContentStart, txtObservations.Document.ContentEnd).Text, utilisateur.IdU);

                                    if (baePanel.cbFiltres.SelectedIndex != 2)
                                    {
                                        baePanel.cbFiltres.SelectedIndex = 2;
                                    }
                                    else
                                    {
                                        baePanel.bonsEnlever = vsomAcc.GetBonsEnleverEnAttente();
                                        baePanel.dataGrid.ItemsSource = baePanel.bonsEnlever;
                                        baePanel.lblStatut.Content = baePanel.bonsEnlever.Count + " Bon(s) à enlever";
                                    }

                                    formLoader.LoadBonEnleverForm(this, bae);

                                    borderActions.Visibility = System.Windows.Visibility.Visible;
                                    btnValiderBAE.Visibility = System.Windows.Visibility.Visible;
                                    //borderEtat.Visibility = System.Windows.Visibility.Visible;
                                    MessageBox.Show("Enregistrement effectué avec succès.", "Enregistrement effectué !", MessageBoxButton.OK, MessageBoxImage.Information);
                                }
                            }
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
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnDetailsVeh_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DetailsEnlevementVehForm detailsVehForm = new DetailsEnlevementVehForm(this, Convert.ToInt32(txtIdBL.Text), utilisateur);
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
                DetailsEnlevementCtrForm detailsCtrForm = new DetailsEnlevementCtrForm(this, Convert.ToInt32(txtIdBL.Text), utilisateur);
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

        private void btnValiderBAE_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (operationsUser.Where(op => op.NomOp == "Bon à enlever : Validation d'un élément").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
                {
                    MessageBox.Show("Vous n'avez pas les droits nécessaires pour valider un BAE. Veuillez contacter un administrateur", "Droits insuffisants !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    ValiderBonEnleverForm validForm = new ValiderBonEnleverForm(this, utilisateur);
                    validForm.Title = "Validation du BAE : " + cbIdBAE.Text + " - Consignée " + txtConsignee.Text;
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

                if (e.Key == Key.Return && cbNumBL.Text.Trim() != "")
                {
                    if (utilisateur.IdAcc == 1)
                    {
                        connaissements = vsomAcc.GetConnaissementPourEnlevementByNumBL(cbNumBL.Text);
                        //connaissements = vsomAcc.GetConnaissementByNumBL(cbNumBL.Text);
                    }
                    else
                    {
                        connaissements = vsomAcc.GetConnaissementPourEnlevementByNumBL(cbNumBL.Text, utilisateur.IdAcc.Value);
                    }

                    if (connaissements.Count == 0)
                    {
                        if (utilisateur.IdAcc == 1)
                        {
                            MessageBox.Show("Il n'existe aucun connaissement accompli portant ce numéro", "Connaissement introuvable", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        }
                        else
                        {
                            MessageBox.Show("Il n'existe aucun connaissement portant ce numéro", "Connaissement introuvable", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        }
                        //MessageBox.Show("Il n'existe aucun connaissement portant ce numéro", "Connaissement introuvable", MessageBoxButton.OK, MessageBoxImage.Exclamation);
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
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        private void btnImprimerBAE_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                BAEReport baeReport = new BAEReport(this);
                baeReport.Title = "Impression du bon à enlever : " + cbIdBAE.Text + " - Escale : " + cbNumEsc.Text;
                baeReport.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération", MessageBoxButton.OK, MessageBoxImage.Error);
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

        private void listNotes_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }

        private void btnDetailsMafi_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DetailsEnlevementMafiForm detailsMafiForm = new DetailsEnlevementMafiForm(this, Convert.ToInt32(txtIdBL.Text), utilisateur);
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

        private void btnDetailsGC_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DetailsEnlevementGCForm detailsGCForm = new DetailsEnlevementGCForm(this, Convert.ToInt32(txtIdBL.Text), utilisateur);
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

        private void btnImprimerBAD_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                BADReport badReport = new BADReport(this);
                badReport.Title = "Impression du bon à délivrer : " + cbIdBAE.Text + " - Escale : " + cbNumEsc.Text;
                badReport.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
