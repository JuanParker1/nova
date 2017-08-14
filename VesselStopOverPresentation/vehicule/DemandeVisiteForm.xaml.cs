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
    /// Logique d'interaction pour DemandeVisiteForm.xaml
    /// </summary>
    public partial class DemandeVisiteForm : Window
    {
        public List<CONNAISSEMENT> connaissements { get; set; }
        public List<string> cons { get; set; }

        public List<CLIENT> clients { get; set; }
        public List<string> clts { get; set; }

        public List<ESCALE> escales { get; set; }
        public List<Int32> escs { get; set; }

        public List<DEMANDE_VISITE> demandesVisite { get; set; }
        public List<Int32> visites { get; set; }

        public List<FACTURE> factures { get; set; }

        public List<NAVIRE> navires { get; set; }
        public List<string> navs { get; set; }

        private List<TYPE_VISITE> typesVisistes;
        public List<string> typesVis { get; set; }

        public List<VEHICULE> vehicules { get; set; }

        private UTILISATEUR utilisateur;
        private List<OPERATION> operationsUser;

        public DemandeVisitePanel demandeVisitePanel { get; set; }

        private FormLoader formLoader;
        private VsomParameters vsp = new VsomParameters();
        public DemandeVisiteForm(DemandeVisitePanel panel, DEMANDE_VISITE visite, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();
               // VsomParameters vsp = new VsomParameters();
                InitializeComponent();
                this.DataContext = this;

                demandesVisite = vsp.GetDemandesVisite();
                visites = new List<Int32>();
                foreach (DEMANDE_VISITE dv in demandesVisite)
                {
                    visites.Add(dv.IdDV);
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

                typesVisistes = vsp.GetTypesVisites();
                typesVis = new List<string>();
                foreach (TYPE_VISITE t in typesVisistes)
                {
                    typesVis.Add(t.LibTypeVisite);
                }

                demandeVisitePanel = panel;

                utilisateur = user;
                operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);

                formLoader = new FormLoader(utilisateur);

                formLoader.LoadDemandeVisiteForm(this, visite);
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
            
        }

        public DemandeVisiteForm(string type, DemandeVisitePanel panel, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();
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

                typesVisistes = vsp.GetTypesVisites();
                typesVis = new List<string>();
                foreach (TYPE_VISITE t in typesVisistes)
                {
                    typesVis.Add(t.LibTypeVisite);
                }

                demandeVisitePanel = panel;

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

                if (cbIdDV.IsEnabled == false && txtIdBL.Text != "" && cbTypeVisite.SelectedIndex != -1)
                {
                    if (dataGridVehicules.SelectedItems.Count == 0)
                    {
                        MessageBox.Show("Veuillez sélectionner au moins un véhicule", "Sélectionnez au moins un véhicule !", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else
                    {
                        CONNAISSEMENT bl = vsp.GetConnaissementByIdBL(Convert.ToInt32(txtIdBL.Text));

                        DEMANDE_VISITE vis = vsomAcc.InsertDemandeVisite(bl.IdBL, typesVisistes.ElementAt<TYPE_VISITE>(cbTypeVisite.SelectedIndex).IdTypeVisite, dataGridVehicules.SelectedItems.OfType<VEHICULE>().ToList<VEHICULE>(), new TextRange(txtObservations.Document.ContentStart, txtObservations.Document.ContentEnd).Text, utilisateur.IdU);

                        if (demandeVisitePanel.cbFiltres.SelectedIndex != 2)
                        {
                            demandeVisitePanel.cbFiltres.SelectedIndex = 2;
                        }
                        else
                        {
                            demandeVisitePanel.demandesVisite = vsp.GetDemandesVisiteEnAttente();
                            demandeVisitePanel.dataGrid.ItemsSource = demandeVisitePanel.demandesVisite;
                            demandeVisitePanel.lblStatut.Content = demandeVisitePanel.demandesVisite.Count + " Demande(s) de visite";
                        }

                        formLoader.LoadDemandeVisiteForm(this, vis);

                        btnValiderDV.Visibility = System.Windows.Visibility.Visible;
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
                MessageBox.Show(ex.StackTrace);
            }
        }

        private void btnValiderDV_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (operationsUser.Where(op => op.NomOp == "Demande de visite : Validation d'un élément").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
                {
                    MessageBox.Show("Vous n'avez pas les droits nécessaires pour valider une demande de visite. Veuillez contacter un administrateur", "Droits insuffisants !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    ValiderDemandeVisiteForm validForm = new ValiderDemandeVisiteForm(this, utilisateur);
                    validForm.Title = "Validation de la demande de visite : " + cbIdDV.Text + " - Consignée " + txtConsignee.Text;
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
                VSOMAccessors vsomAcc = new VSOMAccessors();

                if (e.Key == Key.Return && cbNumBL.Text.Trim() != "")
                {
                    if (utilisateur.IdAcc == 1)
                    {
                        connaissements = vsp.GetConnaissementByNumBL(cbNumBL.Text);
                    }
                    else
                    {
                        connaissements = vsp.GetConnaissementByNumBL(cbNumBL.Text, utilisateur.IdAcc.Value);
                    }

                    if (connaissements.Count == 0)
                    {
                        MessageBox.Show("Il n'existe aucun connaissement portant ce numéro", "Connaissement introuvable", MessageBoxButton.OK, MessageBoxImage.Exclamation);
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
        
    }
}
