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
using System.Text.RegularExpressions;
using VesselStopOverData;

namespace VesselStopOverPresentation
{
    /// <summary>
    /// Logique d'interaction pour FactureDITForm.xaml
    /// </summary>
    public partial class FactureDITForm : Window
    {
        private List<CONNAISSEMENT> connaissements;
        public List<string> cons { get; set; }

        private List<CLIENT> clients;
        public List<string> clts { get; set; }

        private List<ESCALE> escales;
        public List<Int32> escs { get; set; }

        public List<FACTURE_DIT> factures { get; set; }
        public List<Int32> facts { get; set; }

        private List<NAVIRE> navires;
        public List<string> navs { get; set; }

        public List<CONTENEUR> conteneurs { get; set; }

        public List<ElementFacturation> eltsFact { get; set; }

        private List<OPERATION> operationsUser;
        private UTILISATEUR utilisateur;

        public FactureDITPanel factDITPanel { get; set; }

        private FormLoader formLoader;
        private VsomParameters vsp = new VsomParameters();
        public FactureDITForm(FactureDITPanel panel, FACTURE_DIT fact, UTILISATEUR user)
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

                escales = vsp.GetEscales();
                escs = new List<Int32>();
                foreach (ESCALE esc in escales)
                {
                    escs.Add(esc.NumEsc.Value);
                }

                factDITPanel = panel;
                utilisateur = user;
                operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);

                formLoader = new FormLoader(utilisateur);

                formLoader.LoadFactureDITForm(this, fact);
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        public FactureDITForm(string type, FactureDITPanel panel, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();
               // VsomParameters vsp = new VsomParameters();
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

                escales = vsp.GetEscales();
                escs = new List<Int32>();
                foreach (ESCALE esc in escales)
                {
                    escs.Add(esc.NumEsc.Value);
                }

                factDITPanel = panel;
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

        private void btnEnregistrer_Click(object sender, RoutedEventArgs e)
        {

            VSOMAccessors vsomAcc = new VSOMAccessors();

            //Enregistrement
            if (cbIdFact.IsEnabled == false)
            {
                if (operationsUser.Where(op => op.NomOp == "Mise à jour des prix DIT").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
                {
                    MessageBox.Show("Vous n'avez pas les droits nécessaires pour créer une nouvelle facture DIT. Veuillez contacter un administrateur", "Droits insuffisants !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (txtIdBL.Text == "")
                {
                    MessageBox.Show("Vous devez indiquer un connaissement pour la création d'une nouvelle facture DIT", "N° Connaissement ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (txtNumFactDIT.Text == "")
                {
                    MessageBox.Show("Vous devez indiquer le numéro de la facture DIT", "N° facture ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (dataGridEltsFact.SelectedItems.OfType<ElementFacturation>().ToList<ElementFacturation>().Count == 0)
                {
                    MessageBox.Show("Vous devez renseigner au moins une ligne de facturation objet de cette facture proforma", "Eléments de facturation ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    try
                    {
                        List<ElementFacturation> listElements = dataGridEltsFact.SelectedItems.OfType<ElementFacturation>().ToList<ElementFacturation>();

                        // Insertion de la facture DIT

                        FACTURE_DIT factDIT = vsomAcc.InsertFactureDIT(Convert.ToInt32(txtIdBL.Text), txtNumFactDIT.Text, txtDateFacture.SelectedDate.Value, listElements, utilisateur.IdU);

                        if (factDITPanel != null)
                        {
                            factDITPanel.facturesDIT = vsp.GetFacturesDIT();
                            factDITPanel.dataGrid.ItemsSource = factDITPanel.facturesDIT;
                            factDITPanel.lblStatut.Content = factDITPanel.facturesDIT.Count + " Facture(s) DIT";
                        }

                        formLoader.LoadFactureDITForm(this, factDIT);

                        cbIdFact.IsEnabled = true;

                        borderActions.Visibility = System.Windows.Visibility.Visible;
                        MessageBox.Show("Facture DIT enregistrée avec succès.", "Enregistrement effectué !", MessageBoxButton.OK, MessageBoxImage.Information);
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

        private void cbNumBL_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                if (e.Key == Key.Return && cbNumBL.Text.Trim() != "")
                {
                    connaissements = vsp.GetConnaissementsWithDITByNumBL(cbNumBL.Text);

                    if (connaissements.Count == 0)
                    {
                        MessageBox.Show("Il n'existe aucun connaissement possédant des conteneurs portant ce numéro", "Connaissement introuvable", MessageBoxButton.OK, MessageBoxImage.Exclamation);
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

        private void cbIdFact_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                if (e.Key == Key.Return && cbNumBL.Text.Trim() != "")
                {
                    int result;
                    factures = new List<FACTURE_DIT>();

                    FACTURE_DIT f = vsp.GetFacturesDITByIdFact(Int32.TryParse(cbIdFact.Text.Trim(), out result) ? result : -1);

                    if (f != null)
                    {
                        factures.Add(f);
                    }

                    if (factures.Count == 0)
                    {
                        MessageBox.Show("Il n'existe aucune facture DIT portant ce numéro", "Facture DIT introuvable", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                    else if (factures.Count == 1)
                    {
                        FACTURE_DIT fact = factures.FirstOrDefault<FACTURE_DIT>();
                        formLoader.LoadFactureDITForm(this, fact);
                    }
                    else
                    {
                        ListFactureDITForm listFactDITForm = new ListFactureDITForm(this, factures, utilisateur);
                        listFactDITForm.Title = "Choix multiples : Sélectionnez une facture DIT";
                        listFactDITForm.ShowDialog();
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

        private void cbIdFact_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void dataGridEltsFact_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (dataGridEltsFact.SelectedIndex != -1)
                {
                    DITForm ditForm = new DITForm(this, Convert.ToInt32(txtIdBL.Text), utilisateur);
                    ditForm.cbElt.SelectedItem = ((ElementFacturation)dataGridEltsFact.SelectedItem).LibArticle;
                    ditForm.Title = "Eléments de facturation DIT - BL N° " + cbNumBL.Text;
                    ditForm.ShowDialog();
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
