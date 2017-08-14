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

namespace VesselStopOverPresentation
{
    /// <summary>
    /// Logique d'interaction pour ProformaForm.xaml
    /// </summary>
    public partial class FactureForm : Window
    {
        public List<CONNAISSEMENT> connaissements { get; set; }
        public List<string> cons { get; set; }

        private List<CLIENT> clients;
        public List<string> clts { get; set; }

        public List<ESCALE> escales { get; set; }
        public List<Int32> escs { get; set; }

        private List<PROFORMA> proformas;
        public List<Int32> profs { get; set; }

        public List<FACTURE> factures { get; set; }
        public List<Int32> facts { get; set; }

        private List<NAVIRE> navires;
        public List<string> navs { get; set; }

        public List<VEHICULE> vehicules { get; set; }
        public List<CONTENEUR> conteneurs { get; set; }
        public List<CONVENTIONNEL> conventionnels { get; set; }

        public List<ElementFacturation> eltsFact { get; set; }

        public FacturePanel factPanel { get; set; }
        public ConnaissementForm conForm { get; set; }
        public PaiementForm payForm { get; set; }
        public ProformaForm profForm { get; set; }
        public BookingForm bookForm { get; set; }
        public EscaleForm escForm { get; set; }
        public ManifesteForm manForm { get; set; }

        private List<OPERATION> operationsUser;
        private UTILISATEUR utilisateur;

        private FormLoader formLoader;
       // private VsomParameters vsp = new VsomParameters();
        private VSOMAccessors vsomAcc;

        public FactureForm(FacturePanel panel, FACTURE fact, UTILISATEUR user)
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

                factPanel = panel;
                utilisateur = user;
                operationsUser = vsomAcc.GetOperationsUtilisateur(utilisateur.IdU);

                formLoader = new FormLoader(utilisateur);

                formLoader.LoadFactureForm(this, fact);
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
            
        }

        public FactureForm(ConnaissementForm form, FACTURE fact, UTILISATEUR user)
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

                formLoader.LoadFactureForm(this, fact);
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }

        }

        public FactureForm(PaiementForm form, FACTURE fact, UTILISATEUR user)
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

                payForm = form;
                utilisateur = user;
                operationsUser = vsomAcc.GetOperationsUtilisateur(utilisateur.IdU);

                formLoader = new FormLoader(utilisateur);

                formLoader.LoadFactureForm(this, fact);
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }

        }

        public FactureForm(ProformaForm form, FACTURE fact, UTILISATEUR user)
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

                profForm = form;
                utilisateur = user;
                operationsUser = vsomAcc.GetOperationsUtilisateur(utilisateur.IdU);

                formLoader = new FormLoader(utilisateur);

                formLoader.LoadFactureForm(this, fact);
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }

        }

        public FactureForm(BookingForm form, FACTURE fact, UTILISATEUR user)
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

                formLoader.LoadFactureForm(this, fact);
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }

        }

        public FactureForm(EscaleForm form, FACTURE fact, UTILISATEUR user)
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

                escForm = form;
                utilisateur = user;
                operationsUser = vsomAcc.GetOperationsUtilisateur(utilisateur.IdU);

                formLoader = new FormLoader(utilisateur);

                formLoader.LoadFactureForm(this, fact);
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }

        }

        public FactureForm(ManifesteForm form, FACTURE fact, UTILISATEUR user)
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

                manForm = form;
                utilisateur = user;
                operationsUser = vsomAcc.GetOperationsUtilisateur(utilisateur.IdU);

                formLoader = new FormLoader(utilisateur);

                formLoader.LoadFactureForm(this, fact);
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }

        }

        public FactureForm(string type, FacturePanel panel, UTILISATEUR user)
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

                factPanel = panel;
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

        private void btnEnregistrer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                vsomAcc = new VSOMAccessors();

                if (cbIdProf.SelectedIndex == -1)
                {
                    MessageBox.Show("Veuillez indiquer une proforma en attente de facturation", "Proforma à facturer ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    FACTURE fact = vsomAcc.InsertFacture(Convert.ToInt32(cbIdProf.Text), new TextRange(txtObservations.Document.ContentStart, txtObservations.Document.ContentEnd).Text, utilisateur.IdU);

                    formLoader.LoadFactureForm(this, fact);

                    borderActions.Visibility = System.Windows.Visibility.Visible;
                    borderEtat.Visibility = System.Windows.Visibility.Visible;

                    MessageBox.Show("Facture créée avec succès.", "Facture créée !", MessageBoxButton.OK, MessageBoxImage.Information);
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

        private void btnImprimerFacture_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FactureReport factReport = new FactureReport(this);
                factReport.Title = "Impression de la facture définitive : " + cbIdFact.Text + " - Escale : " + cbNumEsc.Text;
                factReport.Show();
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

        private void btnAjoutNote_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnProforma_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                 vsomAcc = new VSOMAccessors();

                PROFORMA prof = vsomAcc.GetFactureByIdFact(Convert.ToInt32(cbIdFact.Text)).PROFORMA;

                ProformaForm profForm = new ProformaForm(this, prof, utilisateur);
                profForm.Show();
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        private void btnPaiement_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                 vsomAcc = new VSOMAccessors();

                PAYEMENT pay = vsomAcc.GetFactureByIdFact(Convert.ToInt32(cbIdFact.Text)).PAYEMENT;

                PaiementForm payForm = new PaiementForm(this, pay, utilisateur);
                payForm.btnEnregistrer.IsEnabled = false;
                payForm.Show();
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
