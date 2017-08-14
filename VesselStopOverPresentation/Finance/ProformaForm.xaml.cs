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
    /// Logique d'interaction pour ProformaForm.xaml
    /// </summary>
    public partial class ProformaForm : Window
    {
        private List<CONNAISSEMENT> connaissements;
        public List<string> cons { get; set; }

        private List<CLIENT> clients;
        public List<string> clts { get; set; }

        public List<ESCALE> escales { get; set; }
        public List<Int32> escs { get; set; }

        public List<PROFORMA> proformas { get; set; }
        public List<Int32> profs { get; set; }

        private List<NAVIRE> navires;
        public List<string> navs { get; set; }

        public List<VEHICULE> vehicules { get; set; }
        public List<CONTENEUR> conteneurs { get; set; }
        public List<CONVENTIONNEL> conventionnels { get; set; }

        public List<ElementFacturation> eltsFact { get; set; }

        public ConnaissementForm conForm { get; set; }
        public FactureForm factForm { get; set; }
        public BookingForm bookForm { get; set; }

        private List<OPERATION> operationsUser;
        private UTILISATEUR utilisateur;

        public ProformaPanel profPanel { get; set; }

        private FormLoader formLoader;
       // private VsomParameters vsp = new VsomParameters();
       private VSOMAccessors vsomAcc;
        public ProformaForm(ProformaPanel panel, PROFORMA prof, UTILISATEUR user)
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

                escales = vsomAcc.GetEscales();
                escs = new List<Int32>();
                foreach (ESCALE esc in escales)
                {
                    escs.Add(esc.NumEsc.Value);
                }

                profPanel = panel;
                utilisateur = user;
                operationsUser = vsomAcc.GetOperationsUtilisateur(utilisateur.IdU);

                formLoader = new FormLoader(utilisateur);

                formLoader.LoadProformaForm(this, prof);
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        public ProformaForm(ConnaissementForm form, PROFORMA prof, UTILISATEUR user)
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

                conForm = form;
                utilisateur = user;
                operationsUser = vsomAcc.GetOperationsUtilisateur(utilisateur.IdU);

                formLoader = new FormLoader(utilisateur);

                formLoader.LoadProformaForm(this, prof);
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        public ProformaForm(FactureForm form, PROFORMA prof, UTILISATEUR user)
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

                factForm = form;
                utilisateur = user;
                operationsUser = vsomAcc.GetOperationsUtilisateur(utilisateur.IdU);

                formLoader = new FormLoader(utilisateur);

                formLoader.LoadProformaForm(this, prof);
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        public ProformaForm(BookingForm form, PROFORMA prof, UTILISATEUR user)
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

                bookForm = form;
                utilisateur = user;
                operationsUser = vsomAcc.GetOperationsUtilisateur(utilisateur.IdU);

                formLoader = new FormLoader(utilisateur);

                formLoader.LoadProformaForm(this, prof);
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        public ProformaForm(string type, ProformaPanel panel, UTILISATEUR user)
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

                escales = vsomAcc.GetEscales();
                escs = new List<Int32>();
                foreach (ESCALE esc in escales)
                {
                    escs.Add(esc.NumEsc.Value);
                }

                profPanel = panel;
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

        public ProformaForm(string type, ConnaissementForm form, CONNAISSEMENT con, UTILISATEUR user)
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

                escales = vsomAcc.GetEscales();
                escs = new List<Int32>();
                foreach (ESCALE esc in escales)
                {
                    escs.Add(esc.NumEsc.Value);
                }

                conForm = form;
                utilisateur = user;
                operationsUser = vsomAcc.GetOperationsUtilisateur(utilisateur.IdU);

                formLoader = new FormLoader(utilisateur);

                formLoader.LoadConnaissementForm(this, con);
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        public ProformaForm(string type, BookingForm form, CONNAISSEMENT con, UTILISATEUR user)
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

                escales = vsomAcc.GetEscales();
                escs = new List<Int32>();
                foreach (ESCALE esc in escales)
                {
                    escs.Add(esc.NumEsc.Value);
                }

                bookForm = form;
                utilisateur = user;
                operationsUser = vsomAcc.GetOperationsUtilisateur(utilisateur.IdU);

                formLoader = new FormLoader(utilisateur);

                formLoader.LoadConnaissementForm(this, con);
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
            if (cbIdProf.IsEnabled == false)
            {
                if (operationsUser.Where(op => op.NomOp == "Proforma : Enregistrement d'un nouvel élément").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
                {
                    MessageBox.Show("Vous n'avez pas les droits nécessaires pour créer une nouvelle proforma. Veuillez contacter un administrateur", "Droits insuffisants !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (txtIdBL.Text == "")
                {
                    MessageBox.Show("Vous devez indiquer un connaissement pour la création d'une nouvelle proforma", "N° Connaissement ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (txtClientAFacturer.Text.Trim() == "")
                {
                    MessageBox.Show("Veuillez indiquer le client à facturer", "Client à facturer ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (dataGridEltsFact.SelectedItems.OfType<ElementFacturation>().ToList<ElementFacturation>().Count == 0)
                {
                    MessageBox.Show("Vous devez renseigner au moins une ligne de facturation objet de cette facture proforma", "Eléments de facturation ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    try
                    {
                        List<ElementFacturation> listElements = dataGridEltsFact.SelectedItems.OfType<ElementFacturation>().OrderBy(el => el.IdElt).ToList<ElementFacturation>();

                        PROFORMA prof = vsomAcc.InsertProforma(Convert.ToInt32(txtIdBL.Text), txtClientAFacturer.Text, listElements, new TextRange(txtObservations.Document.ContentStart, txtObservations.Document.ContentEnd).Text, utilisateur.IdU);

                        if (profPanel != null)
                        {
                            if (profPanel.cbFiltres.SelectedIndex != 2)
                            {
                                profPanel.cbFiltres.SelectedIndex = 2;
                            }
                            else
                            {
                                profPanel.proformas = vsomAcc.GetProformasEnAttente();
                                profPanel.dataGrid.ItemsSource = profPanel.proformas;
                                profPanel.lblStatut.Content = profPanel.proformas.Count + " Proforma(s)";
                            }
                        }

                        formLoader.LoadProformaForm(this, prof);

                        cbIdProf.IsEnabled = true;

                        borderActions.Visibility = System.Windows.Visibility.Visible;
                        borderEtat.Visibility = System.Windows.Visibility.Visible;
                        eltBorder.IsEnabled = true;
                        cbFiltres.SelectedIndex = 2;
                        MessageBox.Show("Proforma enregistrée avec succès.", "Enregistrement effectué !", MessageBoxButton.OK, MessageBoxImage.Information);

                    }
                    catch (EnregistrementInexistant ex)
                    {
                        MessageBox.Show(ex.Message, "Enregistrement inexistant !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                    catch (CubageException vehex)
                    {
                        MessageBox.Show(vehex.Message, "Echec de l'opération", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                    catch (ApplicationException ex)
                    {
                        MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
                    }
                }
            }
            else
            {
                if (operationsUser.Where(op => op.NomOp == "Proforma : Modification des informations sur un élément existant").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
                {
                    MessageBox.Show("Vous n'avez pas les droits nécessaires pour modifier une proforma. Veuillez contacter un administrateur", "Droits insuffisants !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (txtIdBL.Text == "")
                {
                    MessageBox.Show("Vous devez indiquer un connaissement", "N° Connaissement ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (dataGridEltsFact.SelectedItems.OfType<ElementFacturation>().ToList<ElementFacturation>().Count == 0)
                {
                    MessageBox.Show("Vous devez renseigner au moins une ligne de facturation objet de cette facture proforma", "Eléments de facturation ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    try
                    {
                        List<ElementFacturation> listElements = dataGridEltsFact.SelectedItems.OfType<ElementFacturation>().OrderBy(el => el.IdElt).ToList<ElementFacturation>();

                        PROFORMA prof = vsomAcc.UpdateProforma(Convert.ToInt32(cbIdProf.Text), Convert.ToInt32(txtIdBL.Text), txtClientAFacturer.Text, listElements, new TextRange(txtObservations.Document.ContentStart, txtObservations.Document.ContentEnd).Text, utilisateur.IdU);

                        if (profPanel != null)
                        {
                            if (profPanel.cbFiltres.SelectedIndex != 2)
                            {
                                profPanel.cbFiltres.SelectedIndex = 2;
                            }
                            else
                            {
                                profPanel.proformas = vsomAcc.GetProformasEnAttente();
                                profPanel.dataGrid.ItemsSource = profPanel.proformas;
                                profPanel.lblStatut.Content = profPanel.proformas.Count + " Proforma(s)";
                            }
                        }

                        formLoader.LoadProformaForm(this, prof);

                        cbIdProf.IsEnabled = true;

                        borderActions.Visibility = System.Windows.Visibility.Visible;
                        borderEtat.Visibility = System.Windows.Visibility.Visible;
                        eltBorder.IsEnabled = true;
                        cbFiltres.SelectedIndex = 2;
                        MessageBox.Show("Mise à jour effectuée avec succès.", "Mise à jour effectuée !", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (EnregistrementInexistant ex)
                    {
                        MessageBox.Show(ex.Message, "Enregistrement inexistant !", MessageBoxButton.OK, MessageBoxImage.Error);
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

                    }
                }
            }
        }

        private void btnValider_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (operationsUser.Where(op => op.NomOp == "Proforma : Validation d'un élément").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
                {
                    MessageBox.Show("Vous n'avez pas les droits nécessaires pour valider une proforma. Veuillez contacter un administrateur", "Droits insuffisants !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    ValiderProformaForm validForm = new ValiderProformaForm(this, utilisateur);
                    validForm.Title = "Validation de la proforma : " + cbIdProf.Text + " - Consignée " + txtConsignee.Text;
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

        private void cbFiltres_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                //VSOMAccessors vsomAcc = new VSOMAccessors();

                CONNAISSEMENT con = vsomAcc.GetConnaissementByIdBL(Convert.ToInt32(txtIdBL.Text));

                if (cbFiltres.SelectedIndex == 0)
                {
                    eltsFact = vsomAcc.GetElementFacturationBL(con.IdBL);
                    dataGridEltsFact.ItemsSource = eltsFact;
                }
                else if (cbFiltres.SelectedIndex == 1)
                {
                    eltsFact = vsomAcc.GetElementNonFactureBL(con.IdBL);
                    dataGridEltsFact.ItemsSource = eltsFact;
                }
                else if (cbFiltres.SelectedIndex == 2)
                {
                    int result;
                    eltsFact = vsomAcc.GetLignesProf(vsomAcc.GetProformaByIdProf(Int32.TryParse(cbIdProf.Text.Trim(), out result) ? result : -1).IdFP);
                    dataGridEltsFact.ItemsSource = eltsFact;
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
               // VSOMAccessors vsomAcc = new VSOMAccessors();

                if (e.Key == Key.Return && cbNumBL.Text.Trim() != "")
                {
                    connaissements = vsomAcc.GetConnaissementsFacturablesByNumBL(cbNumBL.Text);

                    if (connaissements.Count == 0)
                    {
                        MessageBox.Show("Il n'existe aucun connaissement portant ce numéro", "Connaissement introuvable", MessageBoxButton.OK, MessageBoxImage.Exclamation);
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

        private void btnImprimerProforma_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ProformaReport profReport = new ProformaReport(this);
                profReport.Title = "Impression de la facture proforma : " + cbIdProf.Text + " - Escale : " + cbNumEsc.Text;
                profReport.Show();
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        private void cbIdProf_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                //VSOMAccessors vsomAcc = new VSOMAccessors();

                if (e.Key == Key.Return && cbNumBL.Text.Trim() != "")
                {
                    int result;
                    proformas = new List<PROFORMA>();

                    PROFORMA p = vsomAcc.GetProformaByIdProf(Int32.TryParse(cbIdProf.Text.Trim(), out result) ? result : -1);

                    if (p != null)
                    {
                        proformas.Add(p);
                    }

                    if (proformas.Count == 0)
                    {
                        MessageBox.Show("Il n'existe aucune proforma portant ce numéro", "Proforma introuvable", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                    else if (proformas.Count == 1)
                    {
                        PROFORMA prof = proformas.FirstOrDefault<PROFORMA>();
                        formLoader.LoadProformaForm(this, prof);
                    }
                    else
                    {
                        ListProformaForm listProfForm = new ListProformaForm(this, proformas, utilisateur);
                        listProfForm.Title = "Choix multiples : Sélectionnez une proforma";
                        listProfForm.ShowDialog();
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

        private void cbIdProf_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void btnAnnuler_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //VSOMAccessors vsomAcc = new VSOMAccessors();

                //if (operationsUser.Where(op => op.NomOp == "Proforma : Annulation d'un élément existant").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
                //{
                //    MessageBox.Show("Vous n'avez pas les droits nécessaires pour annuler une proforma. Veuillez contacter un administrateur", "Droits insuffisants !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                //}
                //else
                //{
                //    PROFORMA prof = vsomAcc.AnnulerProforma(Convert.ToInt32(cbIdProf.Text), "Proforma annulée", utilisateur.IdU);

                //    formLoader.LoadProformaForm(this, prof);

                //    MessageBox.Show("Proforma annulée avec succès.", "Proforma annulée !", MessageBoxButton.OK, MessageBoxImage.Information);
                //}

                if (operationsUser.Where(op => op.NomOp == "Proforma : Annulation d'un élément existant").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
                {
                    MessageBox.Show("Vous n'avez pas les droits nécessaires pour annuler une proforma. Veuillez contacter un administrateur", "Droits insuffisants !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    AnnulerProformaForm annulationForm = new AnnulerProformaForm(this, utilisateur);
                    annulationForm.Title = "Annulation de la proforma : " + cbIdProf.Text + " - Consignée " + txtConsignee.Text;
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

        private void listNotes_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }

        private void btnFacture_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                vsomAcc = new VSOMAccessors();

                FACTURE fact = vsomAcc.GetProformaByIdProf(Convert.ToInt32(cbIdProf.Text)).FACTURE.FirstOrDefault<FACTURE>();

                FactureForm factForm = new FactureForm(this, fact, utilisateur);
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
    }
}
