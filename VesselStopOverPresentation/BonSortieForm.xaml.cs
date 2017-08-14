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
    /// Logique d'interaction pour BonSortieForm.xaml
    /// </summary>
    public partial class BonSortieForm : Window
    {
        public List<CONNAISSEMENT> connaissements { get; set; }
        public List<string> cons { get; set; }

        public List<CLIENT> clients { get; set; }
        public List<string> clts { get; set; }

        public List<ESCALE> escales { get; set; }
        public List<Int32> escs { get; set; }

        public List<BON_SORTIE> bonsSortie { get; set; }
        public List<Int32> bons { get; set; }

        public List<FACTURE> factures { get; set; }

        public List<NAVIRE> navires { get; set; }
        public List<string> navs { get; set; }

        public List<VEHICULE> vehicules { get; set; }
        public List<CONTENEUR> conteneurs { get; set; }
        public List<CONVENTIONNEL> conventionnels { get; set; }
        public List<MAFI> mafis { get; set; }

        private UTILISATEUR utilisateur;
        private List<OPERATION> operationsUser;

        public BonSortiePanel bonSortiePanel { get; set; }

        private FormLoader formLoader;
        //private VsomParameters vsp = new VsomParameters();
       private VSOMAccessors vsomAcc;
        public BonSortieForm(BonSortiePanel panel, BON_SORTIE bon, UTILISATEUR user)
        {
            try
            {
                 vsomAcc = new VSOMAccessors();
                //VsomParameters vsp = new VsomParameters();
                InitializeComponent();
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

                bonSortiePanel = panel;

                utilisateur = user;
                operationsUser = vsomAcc.GetOperationsUtilisateur(utilisateur.IdU);

                formLoader = new FormLoader(utilisateur);

                formLoader.LoadBonSortieForm(this, bon);
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
            
        }

        public BonSortieForm(string type, BonSortiePanel panel, UTILISATEUR user)
        {
            try
            {
                 vsomAcc = new VSOMAccessors();
                //VsomParameters vsp = new VsomParameters();
                InitializeComponent();
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

                bonSortiePanel = panel;

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

        private void btnEnregistrer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                 vsomAcc = new VSOMAccessors();
                //VsomMarchal vsomAcc = new VsomMarchal();

                if (cbIdBS.IsEnabled == false && txtIdBL.Text != "")
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
                        MessageBox.Show("Vous devez sélectionner au moins une marchandise", "Sélectionner marchandise !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                    else
                    {
                        BON_SORTIE bon = vsomAcc.InsertBonSortie(bl.IdBL, dataGridVehicules.SelectedItems.OfType<VEHICULE>().ToList<VEHICULE>(), dataGridConteneurs.SelectedItems.OfType<CONTENEUR>().ToList<CONTENEUR>(), dataGridGC.SelectedItems.OfType<CONVENTIONNEL>().ToList<CONVENTIONNEL>(), dataGridMafis.SelectedItems.OfType<MAFI>().ToList<MAFI>(), new TextRange(txtObservations.Document.ContentStart, txtObservations.Document.ContentEnd).Text, utilisateur.IdU);

                        if (bonSortiePanel.cbFiltres.SelectedIndex != 0)
                        {
                            bonSortiePanel.cbFiltres.SelectedIndex = 0;
                        }
                        else
                        {
                            bonSortiePanel.bonsSortie = vsomAcc.GetBonsSortie();
                            bonSortiePanel.dataGrid.ItemsSource = bonSortiePanel.bonsSortie;
                            bonSortiePanel.lblStatut.Content = bonSortiePanel.bonsSortie.Count + " Bon(s) de sortie";
                        }

                        formLoader.LoadBonSortieForm(this, bon);

                        borderEtat.Visibility = System.Windows.Visibility.Visible;
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

        private void cbNumBL_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                 vsomAcc = new VSOMAccessors();

                if (e.Key == Key.Return && cbNumBL.Text.Trim() != "")
                {
                    if (utilisateur.IdAcc == 1)
                    {
                        connaissements = vsomAcc.GetConnaissementPourSortieByNumBL(cbNumBL.Text);
                    }
                    else
                    {
                        connaissements = vsomAcc.GetConnaissementPourSortieByNumBL(cbNumBL.Text, utilisateur.IdAcc.Value);
                    }

                    if (connaissements.Count == 0)
                    {
                        MessageBox.Show("Il n'existe aucun connaissement prêt pour sortie portant ce numéro", "Connaissement introuvable", MessageBoxButton.OK, MessageBoxImage.Exclamation);
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

        private void btnEtat_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SortieReport sortieReport = new SortieReport(this);
                sortieReport.Title = "Impression du bon de sortie : " + cbIdBS.Text + " - Escale : " + cbNumEsc.Text;
                sortieReport.Show();
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
        
    }
}
