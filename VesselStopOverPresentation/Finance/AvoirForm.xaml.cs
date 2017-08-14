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
    /// Logique d'interaction pour AvoirForm.xaml
    /// </summary>
    public partial class AvoirForm : Window
    {
        private List<CLIENT> clients;
        public List<string> clts { get; set; }

        public List<ESCALE> escales;
        public List<Int32> escs { get; set; }

        public List<AVOIR> avoirs { get; set; }
        public List<Int32> avs { get; set; }

        private List<NAVIRE> navires;
        public List<string> navs { get; set; }

        public List<VEHICULE> vehicules { get; set; }
        public List<CONTENEUR> conteneurs { get; set; }
        public List<CONVENTIONNEL> conventionnels { get; set; }

        public List<ElementFacturation> eltsFact { get; set; }

        public ConnaissementForm conForm { get; set; }
        public BookingForm bookForm { get; set; }

        private List<OPERATION> operationsUser;
        private UTILISATEUR utilisateur;

        public AvoirPanel avoirPanel { get; set; }

        private FormLoader formLoader;
        //private// VsomParameters vsp = new VsomParameters();
        private VSOMAccessors vsomAcc;
        public AvoirForm(AvoirPanel panel, AVOIR avoir, UTILISATEUR user)
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

                avoirPanel = panel;
                utilisateur = user;
                operationsUser = vsomAcc.GetOperationsUtilisateur(utilisateur.IdU);

                formLoader = new FormLoader(utilisateur);

                formLoader.LoadAvoirForm(this, avoir);
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        public AvoirForm(ConnaissementForm form, AVOIR av, UTILISATEUR user)
        {
            try
            {
                 vsomAcc = new VSOMAccessors();
               // VsomParameters vsp = new VsomParameters();
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

                conForm = form;
                utilisateur = user;
                operationsUser = vsomAcc.GetOperationsUtilisateur(utilisateur.IdU);

                formLoader = new FormLoader(utilisateur);

                formLoader.LoadAvoirForm(this, av);
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        public AvoirForm(BookingForm form, AVOIR av, UTILISATEUR user)
        {
            try
            {
                 vsomAcc = new VSOMAccessors();
               // VsomParameters vsp = new VsomParameters();
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

                bookForm = form;
                utilisateur = user;
                operationsUser = vsomAcc.GetOperationsUtilisateur(utilisateur.IdU);

                formLoader = new FormLoader(utilisateur);

                formLoader.LoadAvoirForm(this, av);
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        public AvoirForm(string type, AvoirPanel panel, UTILISATEUR user)
        {
            try
            {
                vsomAcc = new VSOMAccessors();
               // VsomParameters vsp = new VsomParameters();
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

                escales = vsomAcc.GetEscales();
                escs = new List<Int32>();
                foreach (ESCALE esc in escales)
                {
                    escs.Add(esc.NumEsc.Value);
                }

                avoirPanel = panel;
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

        private void btnEnregistrer_Click(object sender, RoutedEventArgs e)
        {

             vsomAcc = new VSOMAccessors();

            // test si c'est une nouvelle proforma en se basant si le truc de l'idProforma est actif ou pas
            //Enregistrement
            if (cbIdAvoir.IsEnabled == false)
            {
                if (operationsUser.Where(op => op.NomOp == "Avoir : Enregistrement d'un nouvel élément").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
                {
                    MessageBox.Show("Vous n'avez pas les droits nécessaires pour créer un nouvel avoir. Veuillez contacter un administrateur", "Droits insuffisants !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (txtNumBL.Text == "")
                {
                    MessageBox.Show("Vous devez indiquer un connaissement pour la création d'un nouvel avoir", "N° Connaissement ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (dataGridEltsFact.SelectedItems.OfType<ElementFacturation>().ToList<ElementFacturation>().Count == 0 && cbTypeAvoir.Text == "Partiel")
                {
                    MessageBox.Show("Vous devez renseigner au moins une ligne de facturation objet de cette facture avoir", "Eléments de facturation ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (dataGridEltsFact.SelectedItems.OfType<ElementFacturation>().ToList<ElementFacturation>().Count(el => el.Qte < 0) != 0 && cbTypeAvoir.Text == "Partiel")
                {
                    MessageBox.Show("Vous ne pouvez pas créer d'avoir partiel sur des éléments de facture issus d'un avoir partiel", "Eléments de facturation ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    try
                    {
                        AVOIR avoir = null;

                        if (dataGridEltsFact.Items.Count != dataGridEltsFact.SelectedItems.Count && dataGridEltsFact.SelectedItems.Count != 0)
                        {
                            cbTypeAvoir.Text = "Partiel";
                        }

                        bool writeSAP = true; //indique ecriture ou non dans SAP
                        int numAvoirSAP = 0;
                        if ((bool)chkAvoirManuel.IsChecked == true && txtNumAvoirManuel.Text.Trim().Length != 0)
                        {
                            writeSAP = false;
                            numAvoirSAP=int.Parse(txtNumAvoirManuel.Text.Trim());
                        }

                        if (cbTypeAvoir.Text == "Total")
                        {
                            

                            List<ElementFacturation> listElements = dataGridEltsFact.Items.OfType<ElementFacturation>().ToList<ElementFacturation>();
                            if (listElements.Count == 0)
                            {
                                throw new ApplicationException("Impossible d'effectuer l'avoir. Aucun elment de facturation n'est disponible");
                            }

                            avoir = vsomAcc.InsertAvoir(Convert.ToInt32(cbNumFact.Text), listElements, new TextRange(txtObservations.Document.ContentStart, txtObservations.Document.ContentEnd).Text, 
                                utilisateur.IdU,writeSAP,numAvoirSAP);

                            formLoader.LoadAvoirForm(this, avoir);

                            cbIdAvoir.IsEnabled = true;

                            borderActions.Visibility = System.Windows.Visibility.Visible;
                            borderEtat.Visibility = System.Windows.Visibility.Visible;
                            MessageBox.Show("Avoir enregistré avec succès.", "Enregistrement effectué !", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else if (cbTypeAvoir.Text == "Partiel")
                        {
                            List<ElementFacturation> listElements = dataGridEltsFact.SelectedItems.OfType<ElementFacturation>().OrderBy(el => el.IdElt).ToList<ElementFacturation>();
                            //avoir = vsomAcc.InsertAvoirPartiel(Convert.ToInt32(cbNumFact.Text), listElements, new TextRange(txtObservations.Document.ContentStart, txtObservations.Document.ContentEnd).Text, utilisateur.IdU);
                            UpdateEltAvoirPartielForm updateEltForm = new UpdateEltAvoirPartielForm(this, listElements, utilisateur);
                            updateEltForm.Title = "Eléments de facturation - Avoir - Fact N° " + cbNumFact.Text;
                            updateEltForm.ShowDialog();
                        }

                        if (avoirPanel != null)
                        {
                            if (avoirPanel.cbFiltres.SelectedIndex != 2)
                            {
                                avoirPanel.cbFiltres.SelectedIndex = 2;
                            }
                            else
                            {
                                avoirPanel.avoirs = vsomAcc.GetAvoirsEnAttente();
                                avoirPanel.dataGrid.ItemsSource = avoirPanel.avoirs;
                                avoirPanel.lblStatut.Content = avoirPanel.avoirs.Count + " Avoir(s)";
                            }
                        }
                    }
                    catch (EnregistrementInexistant ex)
                    {
                        MessageBox.Show(ex.Message, "Enregistrement inexistant !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                    catch (CubageException vehex)
                    {
                        MessageBox.Show(vehex.Message, "Echec de l'opération", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    catch (ApplicationException ex)
                    {
                        MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Echec de l'opération !!", MessageBoxButton.OK, MessageBoxImage.Error);
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

        private void cbIdFact_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void btnImprimer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AvoirReport avoirReport = new AvoirReport(this);
                avoirReport.Title = "Impression de la facture avoir : " + cbIdAvoir.Text + " - Escale : " + cbNumEsc.Text;
                avoirReport.Show();
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

        private void cbNumFact_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                 vsomAcc = new VSOMAccessors();

                if (e.Key == Key.Return && cbNumFact.Text.Trim() != "")
                {
                    FACTURE fact = vsomAcc.GetFactureByIdDocSAP(Convert.ToInt32(cbNumFact.Text));

                    if (fact == null)
                    {
                        MessageBox.Show("Il n'existe aucune facture portant ce numéro", "Facture introuvable", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                    else
                    {
                        formLoader.LoadFactureForm(this, fact, 0);
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

        private void txtNumAvoirManuel_MouseDoubleClick_1(object sender, MouseButtonEventArgs e)
        {
            
        }

        private void chkAvoirManuel_Checked_1(object sender, RoutedEventArgs e)
        {
            //nactiv la zone de text que lorsquon est en creation
            if (txtIdAvoir.Text.Trim().Length == 0)
            {
                if ((bool)chkAvoirManuel.IsChecked == true)
                {
                    if (operationsUser.Where(op => op.NomOp == "Avoir : enregistrement avoir manuel").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
                    {
                        MessageBox.Show("Vous n'avez pas les droits nécessaires pour créer un  avoir manuel. Veuillez contacter un administrateur", "Droits insuffisants !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                    else
                    {
                        txtNumAvoirManuel.IsEnabled = true;
                    }
                }
                else
                {
                    txtNumAvoirManuel.IsEnabled = false;
                }
            }
           
        }

        private void chkAvoirManuel_Unchecked_1(object sender, RoutedEventArgs e)
        {
            if ((bool)chkAvoirManuel.IsChecked == false)
            {
                txtNumAvoirManuel.IsEnabled = false;
            }
        }
    }
}
