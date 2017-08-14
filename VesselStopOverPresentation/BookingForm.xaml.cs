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
using Excel = Microsoft.Office.Interop.Excel;
using Outlook = Microsoft.Office.Interop.Outlook;
using Office = Microsoft.Office.Core;
using System.Net.Mail;
using System.Configuration;
using System.Net;
using System.Runtime.InteropServices;
using System.IO;
using System.IO.Compression;

namespace VesselStopOverPresentation
{
    /// <summary>
    /// Logique d'interaction pour BookingForm.xaml
    /// </summary>
    public partial class BookingForm : Window
    {
        private string typeForm;
        private BookingPanel bookingPanel;
        private EclaterBLExportForm eclaterForm;

        public List<CONNAISSEMENT> bookings { get; set; }
        public List<string> books { get; set; }

        public List<PORT> ports { get; set; }
        public List<string> prts { get; set; }

        public List<NAVIRE> navires { get; set; }
        public List<string> navs { get; set; }

        private List<TYPE_CONVENTIONNEL> typesGC;
        public List<string> typesMses { get; set; }

        private List<TYPE_CONTENEUR> typesConteneurs;
        public List<string> typesCtrs { get; set; }

        public List<CLIENT> clients { get; set; }
        public List<string> clts { get; set; }

        public List<ESCALE> escales { get; set; }
        public List<Int32> escs { get; set; }

        public List<PROFORMA> proformas { get; set; }

        public List<ElementBookingGC> eltsBookingGC { get; set; }
        public List<ElementBookingCtr> eltsBookingCtr { get; set; }
        public List<DISPOSITION_CONTENEUR> eltsDispositionCtr { get; set; }
        public List<ElementFacturation> eltsFact { get; set; }

        public StatutLoadUnload statutChargement { get; set; }

        private UTILISATEUR utilisateur;

        private List<OPERATION> operationsUser;

        private FormLoader formLoader;
        //private VsomParameters vsp = new VsomParameters();
        private  VSOMAccessors vsomAcc;
        public BookingForm(UTILISATEUR user)
        {
            try
            {
                 vsomAcc = new VSOMAccessors();
                //VsomParameters vsp = new VsomParameters();

                InitializeComponent();
                this.DataContext = this;

                InitializeCombos();

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

        public BookingForm(string typeForm, BookingPanel panel, UTILISATEUR user)
        {
            try
            {
                 vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

                InitializeCombos();

                utilisateur = user;
                operationsUser = vsomAcc.GetOperationsUtilisateur(utilisateur.IdU);

                bookingPanel = panel;

                formLoader = new FormLoader(utilisateur);

                this.typeForm = typeForm;
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        public BookingForm(BookingPanel panel, CONNAISSEMENT booking, UTILISATEUR user)
        {
            try
            {
                 vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

                InitializeCombos();

                utilisateur = user;
                operationsUser = vsomAcc.GetOperationsUtilisateur(utilisateur.IdU);

                formLoader = new FormLoader(utilisateur);

                bookingPanel = panel;

                formLoader.LoadBookingForm(this, booking);
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

        public BookingForm(EclaterBLExportForm form, CONNAISSEMENT booking, UTILISATEUR user)
        {
            try
            {
                 vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

                InitializeCombos();

                utilisateur = user;
                operationsUser = vsomAcc.GetOperationsUtilisateur(utilisateur.IdU);

                formLoader = new FormLoader(utilisateur);

                eclaterForm = form;

                formLoader.LoadBookingForm(this, booking);
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

        private void InitializeCombos()
        {
            try
            {
               // VSOMAccessors vsomAcc = new VSOMAccessors();
                //VsomParameters vsp = new VsomParameters();
                clients = vsomAcc.GetClientsActifs();
                clts = new List<string>();
                foreach (CLIENT cl in clients)
                {
                    clts.Add(cl.NomClient);
                }

                typesGC = vsomAcc.GetTypesConventionnelsExport();
                typesMses = new List<string>();
                foreach (TYPE_CONVENTIONNEL type in typesGC)
                {
                    typesMses.Add(type.LibTypeGC);
                }

                typesConteneurs = vsomAcc.GetTypesConteneurs();
                typesCtrs = new List<string>();
                foreach (TYPE_CONTENEUR t in typesConteneurs)
                {
                    typesCtrs.Add(t.CodeTypeCtr);
                }

                ports = vsomAcc.GetPortsOrderByCode();
                prts = new List<string>();
                foreach (PORT prt in ports)
                {
                    prts.Add(prt.CodePort);
                }

                navires = vsomAcc.GetNaviresActifs();
                navs = new List<string>();
                foreach (NAVIRE nav in navires)
                {
                    navs.Add(nav.NomNav);
                }

                cbTypeMses.SelectedValue = "Bois débité";
                cbTypeCtr.SelectedValue = "FCL/FCL";
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }

        }

        private void cbNumBooking_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
               // VSOMAccessors vsomAcc = new VSOMAccessors();

                if (e.Key == Key.Return && cbNumBooking.Text.Trim() != "")
                {
                    bookings = vsomAcc.GetBookingByNumBooking(cbNumBooking.Text);

                    if (bookings.Count == 0)
                    {
                        MessageBox.Show("Il n'existe aucun booking portant ce numéro", "Booking introuvable", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                    else if (bookings.Count == 1)
                    {
                        CONNAISSEMENT book = bookings.FirstOrDefault<CONNAISSEMENT>();
                        formLoader.LoadBookingForm(this, book);
                    }
                    else
                    {
                        ListBookingForm listBookForm = new ListBookingForm(this, bookings, utilisateur);
                        listBookForm.Title = "Choix multiples : Sélectionnez un booking";
                        listBookForm.ShowDialog();
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

        private void cbClient_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                txtCodeClient.Text = clients.ElementAt<CLIENT>(cbClient.SelectedIndex).CodeClient;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnEnregistrer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                 vsomAcc = new VSOMAccessors();
                //VsomMarchal vsm = new VsomMarchal();
                if (txtNumVoyage.Text.Trim() == "")
                {
                    MessageBox.Show("Vous devez indiquer à quelle escale est associée ce booking", "N° Escale ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (txtNatMses.Text.Trim() == "")
                {
                    MessageBox.Show("Vous devez saisir la nature des marchandises associées ce booking", "Nature des marchandises ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (cbClient.SelectedIndex == -1)
                {
                    MessageBox.Show("Veuillez sélectionner un client", "Client ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (cbDestination.SelectedIndex == -1)
                {
                    MessageBox.Show("Vous devez indiquer à le point de livraison des marchandises est associée ce booking", "Place of delivery ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (cbVia.SelectedIndex == -1)
                {
                    MessageBox.Show("Vous devez indiquer à le point de déchargement des marchandises est associée ce booking", "Place of discharge ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (txtConsignee.Text.Trim() == "")
                {
                    MessageBox.Show("Vous devez le nom du shipper effectuant ce booking", "Consignee ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (txtShipper.Text.Trim() == "")
                {
                    MessageBox.Show("Vous devez le nom du consignee pour qui ce booking est effectué", "Shipper ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (txtNotify.Text.Trim() == "")
                {
                    MessageBox.Show("Vous devez le nom du notify relatif à ce booking", "Notify ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (chkFretPrepaid.IsChecked == true && txtFretPrepaid.Text == "")
                {
                    MessageBox.Show("Le montant à payer pour le fret n'a pas été saisis", "Montant fret ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (dataGridGC.Items.OfType<ElementBookingGC>().ToList<ElementBookingGC>().Count == 0 && dataGridConteneurs.Items.OfType<ElementBookingCtr>().ToList<ElementBookingCtr>().Count == 0 && dataGridCtrsDispo.Items.OfType<DISPOSITION_CONTENEUR>().ToList<DISPOSITION_CONTENEUR>().Count == 0)
                {
                    MessageBox.Show("Vous devez ajouter au moins une marchandise pour créer un booking", "Marchandises ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    txtNatMses.Focus();
                    if (typeForm == "Nouveau")
                    {
                        CONNAISSEMENT booking = vsomAcc.InsertBooking(escales.ElementAt<ESCALE>(cbEscale.SelectedIndex).IdEsc, clients.ElementAt<CLIENT>(cbClient.SelectedIndex).IdClient, txtNatMses.Text, cbVia.Text, cbDestination.Text, cbVia.Text, chkGN.IsChecked == true ? "Y" : "N", chkHinterland.IsChecked == true ? "Y" : "N", chkFretPrepaid.IsChecked == true ? "Y" : "N", txtFretPrepaid.Text, txtShipper.Text, txtAdresseShipper.Text, txtEmailShipper.Text, txtTelephoneShipper.Text, txtConsignee.Text, txtAdresseConsigee.Text, txtNotify.Text, txtAdresseNotify.Text, txtEmailNotify.Text, txtTelephoneNotify.Text, txtNotify2.Text, txtAdresseNotify2.Text, txtEmailNotify2.Text, txtTelephoneNotify2.Text, txtPayor.Text, txtClearAgent.Text, chkSEPBC.IsChecked == true ? "Y" : "N", dataGridConteneurs.Items.OfType<ElementBookingCtr>().ToList<ElementBookingCtr>(), dataGridGC.Items.OfType<ElementBookingGC>().ToList<ElementBookingGC>(), dataGridCtrsDispo.Items.OfType<DISPOSITION_CONTENEUR>().ToList<DISPOSITION_CONTENEUR>(), utilisateur.IdU);

                        //Raffraîchir les informations
                        formLoader.LoadBookingForm(this, booking);

                        bookingPanel.bookings = vsomAcc.GetBookingByStatut("Booking");
                        bookingPanel.dataGrid.ItemsSource = bookingPanel.bookings;

                        //cbIdMan.IsEnabled = true;
                        actionsBorder.Visibility = System.Windows.Visibility.Visible;
                        typeForm = "";
                        MessageBox.Show("Le booking a été enregistré au numéro " + booking.NumBooking, "Booking enregistré !", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {

                        List<CONVENTIONNEL> convs = new List<CONVENTIONNEL>();

                        if (eltsBookingGC != null)
                        {
                            foreach (ElementBookingGC gc in eltsBookingGC)
                            {
                                CONVENTIONNEL conv = new CONVENTIONNEL();

                                conv.SensGC = "E";
                                conv.NumGC = gc.NumGC;
                                conv.DescGC = gc.Description;
                                conv.DescGCEmbarq = gc.Description;
                                conv.LongMGC = gc.Longueur;
                                conv.LargMGC = gc.Largeur;
                                conv.HautMGC = gc.Hauteur;
                                conv.LongCGC = gc.Longueur;
                                conv.LargCGC = gc.Largeur;
                                conv.HautCGC = gc.Hauteur;
                                conv.NumItem = gc.Quantite;
                                conv.QteBGC = gc.Quantite;
                                conv.QteRGC = gc.Quantite;
                                conv.VolMGC = gc.Volume;
                                conv.VolRGC = gc.Volume;
                                conv.VolCGC = gc.Volume;
                                conv.PoidsMGC = gc.Poids;
                                conv.PoidsRGC = gc.Poids;
                                conv.PoidsCGC = gc.Poids;
                                conv.TypeMGC = gc.TypeMses;
                                conv.TypeCGC = gc.TypeMses;
                                conv.DCGC = DateTime.Now;
                                conv.StatGC = "Non initié";

                                convs.Add(conv);
                            }
                        }

                        List<CONTENEUR> conteneurs = new List<CONTENEUR>();

                        if (eltsBookingCtr != null)
                        {
                            foreach (ElementBookingCtr ct in eltsBookingCtr)
                            {
                                CONTENEUR ctr = new CONTENEUR();

                                ctr.SensCtr = "E";
                                ctr.NumCtr = ct.NumCtr;
                                ctr.DescCtr = ct.Description;
                                ctr.IMDGCode = ct.UNCode;
                                ctr.DescMses = ct.DescMses;
                                ctr.StatutCtr = ct.StatutCtr;
                                ctr.TypeMCtr = ct.TypeCtr;
                                ctr.TypeCCtr = ct.TypeCtr;
                                ctr.TypeMses = ct.TypeMsesCtr;
                                ctr.VolMCtr = ct.Volume;
                                ctr.PoidsMCtr = ct.Poids;
                                ctr.PoidsCCtr = ct.Poids;
                                ctr.Seal1Ctr = ct.Seal1;
                                ctr.Seal2Ctr = ct.Seal2;
                                ctr.DCCtr = DateTime.Now;
                                ctr.StatCtr = "Non initié";
                                //AH 7juillet16
                                ctr.VGM = ct.VGM;

                                conteneurs.Add(ctr);
                            }
                        }

                        List<DISPOSITION_CONTENEUR> dispositionConteneurs = new List<DISPOSITION_CONTENEUR>();

                        if (eltsDispositionCtr != null)
                        {
                            foreach (DISPOSITION_CONTENEUR dispoCtr in eltsDispositionCtr.Where(dd => dd.CONTENEUR_TC.Count == 0))
                            {
                                DISPOSITION_CONTENEUR disCtr = new DISPOSITION_CONTENEUR();

                                disCtr.IdDisposition = dispoCtr.IdDisposition;
                                disCtr.TypeCtr = dispoCtr.TypeCtr;
                                disCtr.NombreTC = dispoCtr.NombreTC;
                                disCtr.ContactTransitaire = dispoCtr.ContactTransitaire;
                                disCtr.Empotage = dispoCtr.Empotage;
                                disCtr.SortieVia = dispoCtr.SortieVia;
                                disCtr.TypeHabillage = dispoCtr.TypeHabillage;
                                disCtr.RefDisposition = dispoCtr.RefDisposition;

                                dispositionConteneurs.Add(disCtr);
                            }
                        }

                        CONNAISSEMENT booking = vsomAcc.UpdateBooking(Convert.ToInt32(txtSysId.Text), escales.ElementAt<ESCALE>(cbEscale.SelectedIndex).IdEsc, clients.ElementAt<CLIENT>(cbClient.SelectedIndex).IdClient, txtNatMses.Text, cbVia.Text, cbDestination.Text, cbVia.Text, chkGN.IsChecked == true ? "Y" : "N", chkHinterland.IsChecked == true ? "Y" : "N", chkFretPrepaid.IsChecked == true ? "Y" : "N", txtFretPrepaid.Text, txtShipper.Text, txtAdresseShipper.Text, txtEmailShipper.Text, txtTelephoneShipper.Text, txtConsignee.Text, txtAdresseConsigee.Text, txtNotify.Text, txtAdresseNotify.Text, txtEmailNotify.Text, txtTelephoneNotify.Text, txtNotify2.Text, txtAdresseNotify2.Text, txtEmailNotify2.Text, txtTelephoneNotify2.Text, txtPayor.Text, txtClearAgent.Text, chkSEPBC.IsChecked == true ? "Y" : "N", conteneurs, convs, dispositionConteneurs, utilisateur.IdU);

                        //Raffraîchir les informations
                        formLoader.LoadBookingForm(this, booking);

                        //cbIdMan.IsEnabled = true;
                        actionsBorder.Visibility = System.Windows.Visibility.Visible;

                        MessageBox.Show("Le booking " + booking.NumBooking + " a été mis à jour", "Booking enregistré !", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (EnregistrementInexistant ex)
            {
                MessageBox.Show(ex.Message, "Enregistrement inexistant !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            catch (HabilitationException ex)
            {
                MessageBox.Show(ex.Message, "Droits insuffisants !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
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

        private void btnAjoutNote_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                NoteForm noteForm = new NoteForm("Nouveau", this, utilisateur);
                noteForm.Title = "Création de note - Booking - " + cbNumBooking.Text;
                noteForm.lblStatut.Content = "Note crée par : " + utilisateur.NU;
                noteForm.ShowDialog();
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
            try
            {

            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        private void txtDim_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9,.]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void txtPoidsCtr_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void chkFretPrepaid_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //VSOMAccessors vsomAcc = new VSOMAccessors();

                if (chkFretPrepaid.IsChecked == true)
                {
                    txtFretPrepaid.IsEnabled = true;
                }
                else
                {
                    txtFretPrepaid.IsEnabled = false;
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

        private void btnAjoutGC_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //contTab.Visibility = System.Windows.Visibility.Collapsed;

                if (txtCodeGC.Text.Trim() == "")
                {
                    MessageBox.Show("Vous devez saisir le numéro du general cargo que vous essayez d'ajouter.", "N° general cargo ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (txtDescGC.Text.Trim() == "")
                {
                    MessageBox.Show("Vous devez saisir la description du general cargo que vous essayez d'ajouter.", "Description conventionnel ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (txtLongGC.Text.Trim() == "")
                {
                    MessageBox.Show("Vous devez saisir la longueur du general cargo que vous essayez d'ajouter.", "Longueur conventionnel ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (txtLargGC.Text.Trim() == "")
                {
                    MessageBox.Show("Vous devez saisir la largeur du general cargo que vous essayez d'ajouter.", "Largeur conventionnel ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (txtHautGC.Text.Trim() == "")
                {
                    MessageBox.Show("Vous devez saisir la hauteur du general cargo que vous essayez d'ajouter.", "Hauteur conventionnel ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (txtVolGC.Text.Trim() == "" || txtVolGC.Text.Trim() == "0")
                {
                    MessageBox.Show("Vous devez saisir le volume total du general cargo que vous essayez d'ajouter.", "Volume conventionnel ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (txtPoidsGC.Text.Trim() == "" || txtPoidsGC.Text.Trim() == "0")
                {
                    MessageBox.Show("Vous devez saisir le poids total du general cargo que vous essayez d'ajouter.", "Poids conventionnel ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (dataGridGC.Items.OfType<ElementBookingGC>().Count(el => el.NumGC == txtCodeGC.Text) != 0)
                {
                    MessageBox.Show("Il existe déjà dans cette liste, un general cargo portant ce code ; vous ne pouvez pas l'ajouter à nouveau ...", "Code conventionnel ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    txtVolGC.Text = txtVolGC.Text.Replace(" ", "").Replace(".", ",");
                    txtPoidsGC.Text = txtPoidsGC.Text.Replace(" ", "").Replace(".", ",");
                    txtQteGC.Text = txtQteGC.Text.Replace(" ", "").Replace(".", ",");
                    txtLongGC.Text = txtLongGC.Text.Replace(" ", "").Replace(".", ",");
                    txtLargGC.Text = txtLargGC.Text.Replace(" ", "").Replace(".", ",");
                    txtHautGC.Text = txtHautGC.Text.Replace(" ", "").Replace(".", ",");

                    ElementBookingGC elt = new ElementBookingGC();
                    elt.NumGC = txtCodeGC.Text;
                    elt.Description = txtDescGC.Text;
                    elt.Hauteur = (float) Convert.ToDouble(txtHautGC.Text);
                    elt.Largeur = (float) Convert.ToDouble(txtLargGC.Text);
                    elt.Longueur = (float) Convert.ToDouble(txtLongGC.Text);
                    elt.Poids = Convert.ToDouble(txtPoidsGC.Text);
                    elt.Quantite = Convert.ToInt16(txtQteGC.Text);
                    elt.TypeMses = typesGC.ElementAt<TYPE_CONVENTIONNEL>(cbTypeMsesGC.SelectedIndex).LibTypeGC;
                    elt.Volume = Convert.ToDouble(txtVolGC.Text);

                    if (eltsBookingGC == null)
                    {
                        eltsBookingGC = new List<ElementBookingGC>();
                    }

                    eltsBookingGC.Add(elt);

                    dataGridGC.ItemsSource = null;
                    dataGridGC.ItemsSource = eltsBookingGC;
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

        private void btnAjoutCtr_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //gcTab.Visibility = System.Windows.Visibility.Collapsed;

                if(utilisateur.LU != "Admin")
                {
                    MessageBox.Show("Vous n'avez pas les droits pour ajouter manuellement un conteneur sur les booking", "Droits insuffisants !", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    if (txtNumCtr.Text.Trim() == "")
                    {
                        MessageBox.Show("Vous devez saisir le numéro du conteneur que vous essayez d'ajouter.", "N° conteneur ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                    else if (txtDescCtr.Text.Trim() == "")
                    {
                        MessageBox.Show("Vous devez saisir une description pour le conteneur que vous essayez d'ajouter.", "Description conteneur ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                    else if (cbTypeCtr.SelectedIndex == -1)
                    {
                        MessageBox.Show("Vous devez saisir le type de conteneur dont il s'agit.", "Type de conteneur ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                    else if (txtVolCtr.Text.Trim() == "")
                    {
                        MessageBox.Show("Vous devez saisir le volume total du conteneur que vous essayez d'ajouter.", "Volume conteneur ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                    else if (txtPoidsCtr.Text.Trim() == "")
                    {
                        MessageBox.Show("Vous devez saisir le poids total du conteneur que vous essayez d'ajouter.", "Poids conteneur ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                    else if (dataGridConteneurs.Items.OfType<ElementBookingCtr>().Count(el => el.NumCtr == txtNumCtr.Text) != 0)
                    {
                        MessageBox.Show("Il existe déjà dans cette liste, un conteneur portant ce code ; vous ne pouvez pas l'ajouter à nouveau ...", "Num conteneur ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                    else
                    {
                        txtVolCtr.Text = txtVolCtr.Text.Replace(" ", "").Replace(".", ",");
                        txtPoidsCtr.Text = txtPoidsCtr.Text.Replace(" ", "").Replace(".", ",");

                        ElementBookingCtr elt = new ElementBookingCtr();
                        elt.NumCtr = txtNumCtr.Text;
                        elt.Description = txtDescCtr.Text;
                        elt.UNCode = txtUNCode.Text;
                        elt.DescMses = txtDescMses.Text;
                        elt.Poids = Convert.ToInt32(txtPoidsCtr.Text);
                        elt.TypeCtr = typesConteneurs.ElementAt<TYPE_CONTENEUR>(cbTypeCtr.SelectedIndex).CodeTypeCtr;
                        elt.Volume = (float)Convert.ToDouble(txtVolCtr.Text);
                        elt.StatutCtr = cbEtatCtr.Text;
                        elt.TypeMsesCtr = cbTypeMses.Text;
                        elt.Seal1 = txtSeal1Ctr.Text;
                        elt.Seal2 = txtSeal2Ctr.Text;
                        //AH 7juillet16
                        elt.VGM = Convert.ToInt32(txtVGM.Text);
                        if (eltsBookingCtr == null)
                        {
                            eltsBookingCtr = new List<ElementBookingCtr>();
                        }

                        eltsBookingCtr.Add(elt);

                        dataGridConteneurs.ItemsSource = null;
                        dataGridConteneurs.ItemsSource = eltsBookingCtr;
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

        private void dataGridGC_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                //VSOMAccessors vsomAcc = new VSOMAccessors();

                if (dataGridGC.Items.Count != 0)
                {
                    ElementBookingGC elt = (ElementBookingGC)dataGridGC.SelectedItem;

                    txtCodeGC.Text = elt.NumGC;
                    txtDescGC.Text = elt.Description;
                    txtHautGC.Text = elt.Hauteur.ToString();
                    txtLargGC.Text = elt.Largeur.ToString();
                    txtLongGC.Text = elt.Longueur.ToString();
                    txtPoidsGC.Text = elt.Poids.ToString();
                    txtQteGC.Text = elt.Quantite.ToString();
                    cbTypeMsesGC.SelectedItem = elt.TypeMses;
                    txtVolGC.Text = elt.Volume.ToString();

                    if (elt.StatGC == "Cargo Loaded")
                    {
                        ctxTransfertBookingGC.IsEnabled = false;
                    }
                    //if (dataGridGC.SelectedIndex == -1)
                    //{
                    //    ctxTransfertBookingGC.IsEnabled = false;
                    //}
                    if (dataGridGC.Items.Count + dataGridConteneurs.Items.Count == 1)
                    {
                        ctxTransfertBookingGC.IsEnabled = false;
                    }
                    if (elt.StatGC != "Cargo Loaded" && dataGridGC.Items.Count + dataGridConteneurs.Items.Count > 1 && dataGridGC.SelectedIndex != -1)
                    {
                        ctxTransfertBookingGC.IsEnabled = true;
                    }

                    CONVENTIONNEL conv = vsomAcc.GetConventionnelById(elt.IdGC);

                    ctnBooke.Content = conv.QteBGC.ToString() + " Colis - " + conv.PoidsMGC + " t - " + conv.VolMGC + " m³";
                    ctnRecept.Content = conv.QteRGC.ToString() + " Colis - " + conv.PoidsRGC + " t - " + conv.VolRGC + " m³";
                    ctnEmbarq.Content = conv.NumItem.ToString() + " Colis - " + conv.PoidsCGC + " t - " + conv.VolCGC + " m³";
                }
                if (dataGridGC.SelectedIndex == -1)
                {
                    ctxTransfertBookingGC.IsEnabled = false;
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

        private void dataGridConteneurs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (dataGridConteneurs.Items.Count != 0)
                {
                    ElementBookingCtr elt = (ElementBookingCtr)dataGridConteneurs.SelectedItem;

                    txtNumCtr.Text = elt.NumCtr;
                    txtDescCtr.Text = elt.Description;
                    txtUNCode.Text = elt.UNCode;
                    txtDescMses.Text = elt.DescMses;
                    txtPoidsCtr.Text = elt.Poids.ToString();
                    cbTypeCtr.SelectedItem = elt.TypeCtr;
                    txtVolCtr.Text = elt.Volume.ToString();
                    if (elt.StatutCtr == "FCL/FCL")
                    {
                        cbEtatCtr.SelectedIndex = 0;
                    }
                    else if (elt.StatutCtr == "FCL/LCL")
                    {
                        cbEtatCtr.SelectedIndex = 1;
                    }
                    else if (elt.StatutCtr == "LCL/FCL")
                    {
                        cbEtatCtr.SelectedIndex = 2;
                    }
                    else if (elt.StatutCtr == "Empty")
                    {
                        cbEtatCtr.SelectedIndex = 3;
                    }

                    if (elt.TypeMsesCtr == "Coton/Banane")
                    {
                        cbTypeMses.SelectedIndex = 0;
                    }
                    else if (elt.TypeMsesCtr == "Café/Cacao")
                    {
                        cbTypeMses.SelectedIndex = 1;
                    }
                    else if (elt.TypeMsesCtr == "Bois débité")
                    {
                        cbTypeMses.SelectedIndex = 2;
                    }
                    else if (elt.TypeMsesCtr == "Autres produits")
                    {
                        cbTypeMses.SelectedIndex = 3;
                    }

                    txtSeal1Ctr.Text = elt.Seal1;
                    txtSeal2Ctr.Text = elt.Seal2;
                    txtVGM.Text= elt.VGM.ToString()  ;

                    if (elt.StatCtr == "Cargo Loaded")
                    {
                        ctxTransfertBookingCtr.IsEnabled = false;
                    }
                    //if (dataGridConteneurs.SelectedIndex == -1)
                    //{
                    //    ctxTransfertBookingCtr.IsEnabled = false;
                    //}
                    if (dataGridConteneurs.Items.Count + dataGridGC.Items.Count == 1)
                    {
                        ctxTransfertBookingCtr.IsEnabled = false;
                    }
                    if (elt.StatCtr != "Cargo Loaded" && dataGridConteneurs.Items.Count + dataGridGC.Items.Count > 1 && dataGridConteneurs.SelectedIndex != -1)
                    {
                        ctxTransfertBookingCtr.IsEnabled = true;
                    }
                }
                if (dataGridConteneurs.SelectedIndex == -1)
                {
                    ctxTransfertBookingCtr.IsEnabled = false;
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

        private void btnSupprimerGC_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (txtCodeGC.Text.Trim() != "")
                {
                    eltsBookingGC.RemoveAll(el => el.NumGC == txtCodeGC.Text);
                    dataGridGC.ItemsSource = null;
                    dataGridGC.ItemsSource = eltsBookingGC;
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

        private void btnSupprimerCtr_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (txtNumCtr.Text.Trim() != "")
                {
                    eltsBookingCtr.RemoveAll(el => el.NumCtr == txtNumCtr.Text);
                    dataGridConteneurs.ItemsSource = null;
                    dataGridConteneurs.ItemsSource = eltsBookingCtr;
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

        private void btnModifierGC_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dataGridGC.SelectedItems.Count == 0)
                {
                    MessageBox.Show("Veuillez sélectionner un conventionnel à modifier", "Conventionnel ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                if (txtCodeGC.Text.Trim() == "")
                {
                    MessageBox.Show("Vous devez saisir le numéro du general cargo que vous essayez d'ajouter.", "N° general cargo ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (txtDescGC.Text.Trim() == "")
                {
                    MessageBox.Show("Vous devez saisir la description du general cargo que vous essayez de modifier.", "Description conventionnel ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (txtVolGC.Text.Trim() == "" || txtVolGC.Text.Trim() == "0")
                {
                    MessageBox.Show("Vous devez saisir le volume total du general cargo que vous essayez de modifier.", "Volume conventionnel ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (txtPoidsGC.Text.Trim() == "" || txtPoidsGC.Text.Trim() == "0")
                {
                    MessageBox.Show("Vous devez saisir le poids total du general cargo que vous essayez de modifier.", "Poids conventionnel ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    ElementBookingGC elt = (ElementBookingGC)dataGridGC.SelectedItem;
                    elt.NumGC = txtCodeGC.Text;
                    elt.Description = txtDescGC.Text;
                    elt.Hauteur = (float)Convert.ToDouble(txtHautGC.Text.Replace(".", ","));
                    elt.Largeur = (float)Convert.ToDouble(txtLargGC.Text.Replace(".", ","));
                    elt.Longueur = (float)Convert.ToDouble(txtLongGC.Text.Replace(".", ","));
                    elt.Poids = Convert.ToDouble(txtPoidsGC.Text.Replace(".", ","));
                    elt.Quantite = Convert.ToInt16(txtQteGC.Text.Replace(".", ","));
                    elt.TypeMses = typesGC.ElementAt<TYPE_CONVENTIONNEL>(cbTypeMsesGC.SelectedIndex).LibTypeGC;
                    elt.Volume = Convert.ToDouble(txtVolGC.Text.Replace(".", ","));

                    dataGridGC.ItemsSource = null;
                    dataGridGC.ItemsSource = eltsBookingGC;
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

        private void btnModifierCtr_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dataGridConteneurs.SelectedItems.Count == 0)
                {
                    MessageBox.Show("Veuillez sélectionner un conteneur à modifier", "Conteneur ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (txtNumCtr.Text.Trim() == "")
                {
                    MessageBox.Show("Vous devez saisir le numéro du conteneur que vous essayez d'ajouter.", "N° conteneur ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (txtDescCtr.Text.Trim() == "")
                {
                    MessageBox.Show("Vous devez saisir une description pour le conteneur que vous essayez d'ajouter.", "Description conteneur ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (cbTypeCtr.SelectedIndex == -1)
                {
                    MessageBox.Show("Vous devez saisir le type de conteneur dont il s'agit.", "Type de conteneur ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (txtPoidsCtr.Text.Trim() == "")
                {
                    MessageBox.Show("Vous devez saisir le poids total du conteneur que vous essayez d'ajouter.", "Poids conteneur ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    ElementBookingCtr elt = (ElementBookingCtr)dataGridConteneurs.SelectedItem;
                    elt.NumCtr = txtNumCtr.Text;
                    elt.Description = txtDescCtr.Text;
                    elt.UNCode = txtUNCode.Text;
                    elt.DescMses = txtDescMses.Text;
                    elt.TypeMsesCtr = cbTypeMses.Text;
                    elt.Poids = Convert.ToInt32(txtPoidsCtr.Text.Replace(".", ","));
                    elt.TypeCtr = typesConteneurs.ElementAt<TYPE_CONTENEUR>(cbTypeCtr.SelectedIndex).CodeTypeCtr;
                    elt.Volume = (float)Convert.ToDouble(txtVolCtr.Text.Replace(".", ","));
                    elt.StatutCtr = cbEtatCtr.Text;
                    elt.Seal1 = txtSeal1Ctr.Text;
                    elt.Seal2 = txtSeal2Ctr.Text;
                    //AH 7juillet16
                    elt.VGM = Convert.ToInt32(txtVGM.Text);

                    dataGridConteneurs.ItemsSource = null;
                    dataGridConteneurs.ItemsSource = eltsBookingCtr;
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

        private void cbEscale_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
               // VSOMAccessors vsomAcc = new VSOMAccessors();

                if (e.Key == Key.Return && cbEscale.Text.Trim() != "")
                {
                    int result;
                    escales = vsomAcc.GetEscalesGrimaldiByNumEscale(Int32.TryParse(cbEscale.Text.Trim(), out result) ? result : -1);

                    if (escales.Count == 0)
                    {
                        MessageBox.Show("Il n'existe aucune escale portant ce numéro", "Escale introuvable", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                    else if (escales.Count == 1)
                    {
                        ESCALE esc = escales.FirstOrDefault<ESCALE>();
                        formLoader.LoadEscaleForm(this, esc);
                    }
                    else
                    {
                        ListEscaleForm listEscForm = new ListEscaleForm(this, escales, utilisateur);
                        listEscForm.Title = "Choix multiples : Sélectionnez une escale";
                        listEscForm.ShowDialog();
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

        private void dataGridCompteBooking_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                //VSOMAccessors vsomAcc = new VSOMAccessors();
                if (dataGridCompteBooking.SelectedIndex != -1)
                {
                    ElementCompte eltCompte = (ElementCompte)dataGridCompteBooking.SelectedItem;
                    if (eltCompte.TypeDoc == "FA")
                    {
                        FactureForm factForm = new FactureForm(this, vsomAcc.GetFactureByIdFact(eltCompte.Id), utilisateur);
                        factForm.Show();
                    }
                    else if (eltCompte.TypeDoc == "PA")
                    {
                        PaiementForm payForm = new PaiementForm(this, vsomAcc.GetPaiementByIdPay(eltCompte.Id), utilisateur);
                        payForm.btnEnregistrer.IsEnabled = false;
                        payForm.Show();
                    }
                    else if (eltCompte.TypeDoc == "CN")
                    {
                        AvoirForm avoirForm = new AvoirForm(this, vsomAcc.GetAvoirByIdAvoir(eltCompte.Id), utilisateur);
                        avoirForm.btnEnregistrer.IsEnabled = false;
                        avoirForm.Show();
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

        private void dataGridProfs_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                //VSOMAccessors vsomAcc = new VSOMAccessors();
                if (dataGridProfs.SelectedIndex != -1)
                {
                    ProformaForm profForm = new ProformaForm(this, vsomAcc.GetProformaByIdProf(((PROFORMA)dataGridProfs.SelectedItem).IdFP), utilisateur);
                    profForm.Show();
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

        private void btnClearance_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                vsomAcc = new VSOMAccessors();

                CONNAISSEMENT book = vsomAcc.GetBookingByIdBooking(Convert.ToInt32(txtSysId.Text));
                ClearanceForm clearanceForm = new ClearanceForm(this, book, utilisateur);
                clearanceForm.Title = "Clearance booking N° " + book.NumBooking;
                clearanceForm.ShowDialog();
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        private void btnValidClearance_Click(object sender, RoutedEventArgs e)
        {
            try
            {
              vsomAcc = new VSOMAccessors();

                CONNAISSEMENT book = vsomAcc.GetBookingByIdBooking(Convert.ToInt32(txtSysId.Text));
                ValiderClearanceForm validClrForm = new ValiderClearanceForm(this, book, utilisateur);
                validClrForm.Title = "Validation de clearance booking N° " + book.NumBooking;
                validClrForm.ShowDialog();
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        private void btnGenererBL_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                 vsomAcc = new VSOMAccessors();

                CONNAISSEMENT book = vsomAcc.GetBookingByIdBooking(Convert.ToInt32(txtSysId.Text));
                GenererBLForm genererBLForm = new GenererBLForm(this, utilisateur);
                genererBLForm.Title = "Génération du BL Booking N° " + book.NumBooking;
                genererBLForm.ShowDialog();
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        private void btnProforma_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                vsomAcc = new VSOMAccessors();

                if (operationsUser.Where(op => op.NomOp == "Proforma : Enregistrement d'un nouvel élément").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
                {
                    MessageBox.Show("Vous n'avez pas les droits nécessaires pour créer une nouvelle proforma. Veuillez contacter un administrateur", "Droits insuffisants !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    ProformaForm profForm = new ProformaForm("Nouveau", this, vsomAcc.GetBookingByIdBooking(Convert.ToInt32(txtSysId.Text)), utilisateur);
                    profForm.cbIdProf.IsEnabled = false;
                    profForm.Title = "Nouveau : Proforma";
                    profForm.borderActions.Visibility = System.Windows.Visibility.Collapsed;
                    profForm.borderEtat.Visibility = System.Windows.Visibility.Collapsed;
                    profForm.eltBorder.IsEnabled = false;
                    profForm.Show();
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

        private void btnImprimerBL_Click(object sender, RoutedEventArgs e)
        {
            Excel.Application xlApp = null;
            Excel.Workbook xlWorkBook = null;
            Excel.Worksheet xlWorkSheet = null;
            Excel.Range range;

            try
            {

                if (operationsUser.Where(op => op.NomOp == "Booking : Impression BL").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
                {
                    MessageBox.Show("Vous n'avez pas les droits nécessaires pour imprimer un BL. Veuillez contacter un administrateur", "Droits insuffisants !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    //VSOMAccessors vsomAcc = new VSOMAccessors();
                    //VsomParameters vsp = new VsomParameters();

                    CONNAISSEMENT book = vsomAcc.GetBookingByIdBooking(Convert.ToInt32(txtSysId.Text));

                    xlApp = new Excel.Application();
                    xlWorkBook = xlApp.Workbooks.Open(Environment.CurrentDirectory + "//Ressources//BLExport.xlsx", 0, true, Type.Missing, Type.Missing, Type.Missing, true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
                    xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

                    range = xlWorkSheet.UsedRange;

                    (range.Cells[4, 8] as Excel.Range).Value2 = book.IdBL;
                    (range.Cells[5, 8] as Excel.Range).Value2 = book.IdBL;
                    (range.Cells[5, 10] as Excel.Range).Value2 = book.NumBL;
                    (range.Cells[5, 2] as Excel.Range).Value2 = book.ConsigneeBL;
                    (range.Cells[6, 2] as Excel.Range).Value2 = book.AdresseBL;
                    (range.Cells[7, 2] as Excel.Range).Value2 = book.EmailBL + " " + book.PhoneManBL;
                    //(range.Cells[7, 6] as Excel.Range).Value2 = book.PhoneManBL;
                    (range.Cells[10, 2] as Excel.Range).Value2 = book.ConsigneeBooking;
                    (range.Cells[11, 2] as Excel.Range).Value2 = book.AdresseConsignee;
                    (range.Cells[15, 2] as Excel.Range).Value2 = book.NotifyBL;
                    (range.Cells[16, 2] as Excel.Range).Value2 = book.AdresseNotify;
                    (range.Cells[17, 2] as Excel.Range).Value2 = book.EmailNotify + " " + book.TelNotify;
                    (range.Cells[15, 10] as Excel.Range).Value2 = book.NotifyBL2;
                    (range.Cells[16, 10] as Excel.Range).Value2 = book.AdresseNotify2;
                    (range.Cells[17, 10] as Excel.Range).Value2 = book.EmailNotify2 + " " + book.TelNotify2;
                    //(range.Cells[17, 6] as Excel.Range).Value2 = book.TelNotify;
                    (range.Cells[22, 2] as Excel.Range).Value2 = book.ESCALE.NAVIRE.NomNav + " - " + book.ESCALE.NumVoySCR;
                    (range.Cells[24, 2] as Excel.Range).Value2 = vsomAcc.GetPortsByCodePort(book.LDBL).FirstOrDefault<PORT>().NomPortEN;
                    (range.Cells[22, 5] as Excel.Range).Value2 = "DOUALA";
                    (range.Cells[24, 4] as Excel.Range).Value2 = vsomAcc.GetPortsByCodePort(book.DPBL).FirstOrDefault<PORT>().NomPortEN;
                    (range.Cells[44, 2] as Excel.Range).Value2 = "HS Code : " + book.NumHSCode + " - N° E " + book.NumDEBL + " - N° BESC : " + book.NumBESCBL + " - N° CT : " + book.NumCtrBL;
                    ////(range.Cells[44, 6] as Excel.Range).Value2 = "N° D6 " + book.NumDEBL;
                    ////(range.Cells[44, 7] as Excel.Range).Value2 = "N° BESC : " + book.NumBESCBL;
                    ////(range.Cells[44, 9] as Excel.Range).Value2 = "N° CT : " + book.NumCtrBL;
                    //(range.Cells[51, 8] as Excel.Range).Value2 = book.LDBL + ", " + book.DVCBLI;
                    (range.Cells[51, 8] as Excel.Range).Value2 = "DOUALA, " + (book.ESCALE.SailingDate.HasValue ? book.ESCALE.SailingDate.Value.ToShortDateString() : "");
                    //(range.Cells[53, 8] as Excel.Range).Value2 = book.LPFret;
                    (range.Cells[53, 8] as Excel.Range).Value2 = book.CCBL == "Y" ? "Prepaid" : "Destination";
                    (range.Cells[55, 8] as Excel.Range).Value2 = "THREE/03";

                    if (book.CONVENTIONNEL.Count + book.CONTENEUR.Count <= 9)
                    {
                        for (int i = 1; i <= 18; i = i + 2)
                        {
                            range.Range[range.Cells[i + 25, 2], range.Cells[i + 25 + 1, 2]].Merge();
                            range.Range[range.Cells[i + 25, 4], range.Cells[i + 25 + 1, 4]].Merge();
                            range.Range[range.Cells[i + 25, 6], range.Cells[i + 25 + 1, 6]].Merge();
                            range.Range[range.Cells[i + 25, 9], range.Cells[i + 25 + 1, 9]].Merge();
                            range.Range[range.Cells[i + 25, 11], range.Cells[i + 25 + 1, 11]].Merge();
                        }
                    }

                    if (book.CONVENTIONNEL.Count > 0)
                    {
                        int i = 1;
                        foreach (CONVENTIONNEL conv in book.CONVENTIONNEL)
                        {
                            (range.Cells[i + 25, 2] as Excel.Range).Value2 = conv.NumGC;
                            (range.Cells[i + 25, 4] as Excel.Range).Value2 = conv.NumItem;
                            (range.Cells[i + 25, 6] as Excel.Range).Value2 = conv.DescGCEmbarq + " x " + conv.LongCGC + " x " + conv.LargCGC;
                            (range.Cells[i + 25, 9] as Excel.Range).Value2 = conv.PoidsCGC * 1000;
                            (range.Cells[i + 25, 11] as Excel.Range).Value2 = conv.VolCGC;
                            if (book.CONVENTIONNEL.Count + book.CONTENEUR.Count <= 9)
                            {
                                i = i + 2;
                            }
                            else
                            {
                                i++;
                            }

                            if (i + 25 == 44)
                            {
                                //i = 35;
                                i = 60;
                            }
                        }

                        (range.Cells[45, 6] as Excel.Range).Value2 = book.CONVENTIONNEL.Sum(gc => gc.NumItem) + " COLIS  -  (ON BOARD) STC" + book.DescBL;

                        (range.Cells[45, 9] as Excel.Range).Value2 = book.CONVENTIONNEL.Sum(ct => ct.PoidsCGC.Value * 1000);
                        (range.Cells[45, 11] as Excel.Range).Value2 = book.CONVENTIONNEL.Sum(ct => ct.VolCGC.Value);
                    }

                    if (book.CONTENEUR.Count > 0)
                    {
                        int i = 1;
                        foreach (CONTENEUR ctr in book.CONTENEUR)
                        {
                            (range.Cells[i + 25, 2] as Excel.Range).Value2 = ctr.NumCtr + " - " + ctr.Seal1Ctr;
                            (range.Cells[i + 25, 4] as Excel.Range).Value2 = "1 x " + ctr.TypeCCtr.Substring(0, 2) + "'";
                            (range.Cells[i + 25, 6] as Excel.Range).Value2 = /*ctr.StatutCtr + " - " +*/ ctr.DescMses + "(" + ctr.StatutCtr + ")";
                            (range.Cells[i + 25, 9] as Excel.Range).Value2 = ctr.PoidsCCtr;
                            (range.Cells[i + 25, 11] as Excel.Range).Value2 = "";
                            if (book.CONVENTIONNEL.Count + book.CONTENEUR.Count <= 9)
                            {
                                i = i + 2;
                            }
                            else
                            {
                                i++;
                            }

                            if (i + 25 == 44)
                            {
                                //i = 35;
                                i = 60;
                            }
                        }

                        if (book.CONTENEUR.Count(ctr => ctr.TypeCCtr.Substring(0, 2) == "20") > 0)
                        {
                            (range.Cells[45, 6] as Excel.Range).Value2 = book.CONTENEUR.Count(ctr => ctr.TypeCCtr.Substring(0, 2) == "20") + " x 20'  -  (ON BOARD) STC" + book.DescBL;
                        }
                        if (book.CONTENEUR.Count(ctr => ctr.TypeCCtr.Substring(0, 2) == "40") > 0)
                        {
                            (range.Cells[45, 6] as Excel.Range).Value2 = (range.Cells[45, 6] as Excel.Range).Value2 + book.CONTENEUR.Count(ctr => ctr.TypeCCtr.Substring(0, 2) == "40") + " x 40'  -  (ON BOARD) STC" + book.DescBL;
                        }

                        (range.Cells[45, 9] as Excel.Range).Value2 = book.CONTENEUR.Sum(ct => ct.PoidsCCtr.Value);
                        (range.Cells[45, 11] as Excel.Range).Value2 = book.CONTENEUR.Sum(ct => ct.VolMCtr.Value);
                    }

                    xlWorkBook.SaveAs(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\BLExport - " + book.ESCALE.NumEsc + " - " + book.NumBL + " .xlsx", Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Excel.XlSaveAsAccessMode.xlExclusive, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                    //xlWorkBook.ExportAsFixedFormat(Excel.XlFixedFormatType.xlTypePDF, Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\BLExport - " + book.ESCALE.NumEsc + " - " + book.NumBL + " .pdf", Microsoft.Office.Interop.Excel.XlFixedFormatQuality.xlQualityStandard, Type.Missing, true, Type.Missing, Type.Missing, true, Type.Missing);
                    MessageBox.Show("Edition du BL export terminée", "Edition du BL terminée !", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                if (xlWorkSheet != null)
                {
                    releaseObject(xlWorkSheet);
                }

                if (xlWorkBook != null)
                {
                    xlWorkBook.Close(true, Type.Missing, Type.Missing);
                    releaseObject(xlWorkBook);
                }

                bool excelWasRunning = System.Diagnostics.Process.GetProcessesByName("EXCEL.EXE").Length > 0;

                if (excelWasRunning)
                {
                    xlApp.Quit();
                    releaseObject(xlApp);
                }
            }
        }

        private void btnImprimerDraftBL_Click(object sender, RoutedEventArgs e)
        {
            Excel.Application xlApp = null;
            Excel.Workbook xlWorkBook = null;
            Excel.Worksheet xlWorkSheet = null;
            Excel.Range range;

            try
            {

                if (operationsUser.Where(op => op.NomOp == "Booking : Impression BL").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
                {
                    MessageBox.Show("Vous n'avez pas les droits nécessaires pour imprimer un BL. Veuillez contacter un administrateur", "Droits insuffisants !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    //VSOMAccessors vsomAcc = new VSOMAccessors();
                    //VsomParameters vsp = new VsomParameters();
                    CONNAISSEMENT book = vsomAcc.GetBookingByIdBooking(Convert.ToInt32(txtSysId.Text));

                    xlApp = new Excel.Application();
                    xlWorkBook = xlApp.Workbooks.Open(Environment.CurrentDirectory + "//Ressources/BLExportPreview.xlsx", 0, true, Type.Missing, Type.Missing, Type.Missing, true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
                    xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

                    range = xlWorkSheet.UsedRange;

                    (range.Cells[4, 8] as Excel.Range).Value2 = book.IdBL;
                    (range.Cells[5, 8] as Excel.Range).Value2 = book.IdBL;
                    (range.Cells[5, 10] as Excel.Range).Value2 = book.NumBL;
                    (range.Cells[5, 2] as Excel.Range).Value2 = book.ConsigneeBL;
                    (range.Cells[6, 2] as Excel.Range).Value2 = book.AdresseBL;
                    (range.Cells[7, 2] as Excel.Range).Value2 = book.EmailBL + " " + book.PhoneManBL;
                    //(range.Cells[7, 6] as Excel.Range).Value2 = book.PhoneManBL;
                    (range.Cells[10, 2] as Excel.Range).Value2 = book.ConsigneeBooking;
                    (range.Cells[11, 2] as Excel.Range).Value2 = book.AdresseConsignee;
                    (range.Cells[15, 2] as Excel.Range).Value2 = book.NotifyBL;
                    (range.Cells[16, 2] as Excel.Range).Value2 = book.AdresseNotify;
                    (range.Cells[17, 2] as Excel.Range).Value2 = book.EmailNotify + " " + book.TelNotify;
                    (range.Cells[15, 10] as Excel.Range).Value2 = book.NotifyBL2;
                    (range.Cells[16, 10] as Excel.Range).Value2 = book.AdresseNotify2;
                    (range.Cells[17, 10] as Excel.Range).Value2 = book.EmailNotify2 + " " + book.TelNotify2;
                    //(range.Cells[17, 6] as Excel.Range).Value2 = book.TelNotify;
                    (range.Cells[22, 2] as Excel.Range).Value2 = book.ESCALE.NAVIRE.NomNav + " - " + book.ESCALE.NumVoySCR;
                    (range.Cells[24, 2] as Excel.Range).Value2 = vsomAcc.GetPortsByCodePort(book.LDBL).FirstOrDefault<PORT>().NomPortEN;
                    (range.Cells[22, 5] as Excel.Range).Value2 = "DOUALA";
                    (range.Cells[24, 4] as Excel.Range).Value2 = vsomAcc.GetPortsByCodePort(book.DPBL).FirstOrDefault<PORT>().NomPortEN;
                    (range.Cells[44, 2] as Excel.Range).Value2 = "HS Code : " + book.NumHSCode + " - N° E " + book.NumDEBL + " - N° BESC : " + book.NumBESCBL + " - N° CT : " + book.NumCtrBL;
                    ////(range.Cells[44, 6] as Excel.Range).Value2 = "N° D6 " + book.NumDEBL;
                    ////(range.Cells[44, 7] as Excel.Range).Value2 = "N° BESC : " + book.NumBESCBL;
                    ////(range.Cells[44, 9] as Excel.Range).Value2 = "N° CT : " + book.NumCtrBL;
                    //(range.Cells[51, 8] as Excel.Range).Value2 = book.LDBL + ", " + book.DVCBLI;
                    (range.Cells[51, 8] as Excel.Range).Value2 = "DOUALA, " + (book.ESCALE.SailingDate.HasValue ? book.ESCALE.SailingDate.Value.ToShortDateString() : "");
                    //(range.Cells[53, 8] as Excel.Range).Value2 = book.LPFret;
                    (range.Cells[53, 8] as Excel.Range).Value2 = book.CCBL == "Y" ? "Prepaid" : "Destination";
                    (range.Cells[55, 8] as Excel.Range).Value2 = "THREE/03";

                    if (book.CONVENTIONNEL.Count + book.CONTENEUR.Count <= 9)
                    {
                        for (int i = 1; i <= 18; i = i + 2)
                        {
                            range.Range[range.Cells[i + 25, 2], range.Cells[i + 25 + 1, 2]].Merge();
                            range.Range[range.Cells[i + 25, 4], range.Cells[i + 25 + 1, 4]].Merge();
                            range.Range[range.Cells[i + 25, 6], range.Cells[i + 25 + 1, 6]].Merge();
                            range.Range[range.Cells[i + 25, 9], range.Cells[i + 25 + 1, 9]].Merge();
                            range.Range[range.Cells[i + 25, 11], range.Cells[i + 25 + 1, 11]].Merge();
                        }
                    }

                    if (book.CONVENTIONNEL.Count > 0)
                    {
                        int i = 1;
                        foreach (CONVENTIONNEL conv in book.CONVENTIONNEL)
                        {
                            (range.Cells[i + 25, 2] as Excel.Range).Value2 = conv.NumGC;
                            (range.Cells[i + 25, 4] as Excel.Range).Value2 = conv.NumItem;
                            (range.Cells[i + 25, 6] as Excel.Range).Value2 = conv.DescGCEmbarq + " x " + conv.LongCGC + " x " + conv.LargCGC;
                            (range.Cells[i + 25, 9] as Excel.Range).Value2 = conv.PoidsCGC * 1000;
                            (range.Cells[i + 25, 11] as Excel.Range).Value2 = conv.VolCGC;
                            if (book.CONVENTIONNEL.Count + book.CONTENEUR.Count <= 9)
                            {
                                i = i + 2;
                            }
                            else
                            {
                                i++;
                            }

                            if (i + 25 == 44)
                            {
                                //i = 35;
                                i = 60;
                            }
                        }

                        (range.Cells[45, 6] as Excel.Range).Value2 = book.CONVENTIONNEL.Sum(gc => gc.NumItem) + " COLIS  -  (ON BOARD) STC" + book.DescBL;

                        (range.Cells[45, 9] as Excel.Range).Value2 = book.CONVENTIONNEL.Sum(ct => ct.PoidsCGC.Value * 1000);
                        (range.Cells[45, 11] as Excel.Range).Value2 = book.CONVENTIONNEL.Sum(ct => ct.VolCGC.Value);
                    }

                    if (book.CONTENEUR.Count > 0)
                    {
                        int i = 1;
                        foreach (CONTENEUR ctr in book.CONTENEUR)
                        {
                            (range.Cells[i + 25, 2] as Excel.Range).Value2 = ctr.NumCtr + " - " + ctr.Seal1Ctr;
                            (range.Cells[i + 25, 4] as Excel.Range).Value2 = "1 x " + ctr.TypeCCtr.Substring(0, 2) + "'";
                            (range.Cells[i + 25, 6] as Excel.Range).Value2 = /*ctr.StatutCtr + " - " +*/ ctr.DescMses + "(" + ctr.StatutCtr + ")";
                            (range.Cells[i + 25, 9] as Excel.Range).Value2 = ctr.PoidsCCtr;
                            (range.Cells[i + 25, 11] as Excel.Range).Value2 = "";
                            if (book.CONVENTIONNEL.Count + book.CONTENEUR.Count <= 9)
                            {
                                i = i + 2;
                            }
                            else
                            {
                                i++;
                            }

                            if (i + 25 == 44)
                            {
                                //i = 35;
                                i = 60;
                            }
                        }

                        if (book.CONTENEUR.Count(ctr => ctr.TypeCCtr.Substring(0, 2) == "20") > 0)
                        {
                            (range.Cells[45, 6] as Excel.Range).Value2 = book.CONTENEUR.Count(ctr => ctr.TypeCCtr.Substring(0, 2) == "20") + " x 20'  -  (ON BOARD) STC" + book.DescBL;
                        }
                        if (book.CONTENEUR.Count(ctr => ctr.TypeCCtr.Substring(0, 2) == "40") > 0)
                        {
                            (range.Cells[45, 6] as Excel.Range).Value2 = (range.Cells[45, 6] as Excel.Range).Value2 + book.CONTENEUR.Count(ctr => ctr.TypeCCtr.Substring(0, 2) == "40") + " x 40'  -  (ON BOARD) STC" + book.DescBL;
                        }

                        (range.Cells[45, 9] as Excel.Range).Value2 = book.CONTENEUR.Sum(ct => ct.PoidsCCtr.Value);
                        (range.Cells[45, 11] as Excel.Range).Value2 = book.CONTENEUR.Sum(ct => ct.VolMCtr.Value);
                    }

                    xlWorkBook.SaveAs(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\BLExport - " + book.ESCALE.NumEsc + " - " + book.NumBL + " .xlsx", Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Excel.XlSaveAsAccessMode.xlExclusive, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                    //xlWorkBook.ExportAsFixedFormat(Excel.XlFixedFormatType.xlTypePDF, Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\BLExport - " + book.ESCALE.NumEsc + " - " + book.NumBL + " .pdf", Microsoft.Office.Interop.Excel.XlFixedFormatQuality.xlQualityStandard, Type.Missing, true, Type.Missing, Type.Missing, true, Type.Missing);
                    MessageBox.Show("Edition du BL export terminée", "Edition du BL terminée !", MessageBoxButton.OK, MessageBoxImage.Information);
                }


            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                if (xlWorkSheet != null)
                {
                    releaseObject(xlWorkSheet);
                }

                if (xlWorkBook != null)
                {
                    xlWorkBook.Close(true, Type.Missing, Type.Missing);
                    releaseObject(xlWorkBook);
                }

                bool excelWasRunning = System.Diagnostics.Process.GetProcessesByName("EXCEL.EXE").Length > 0;

                if (excelWasRunning)
                {
                    xlApp.Quit();
                    releaseObject(xlApp);
                }
            }
        }

        private void releaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception ex)
            {
                obj = null;
                //MessageBox.Show("Unable to release the Object " + ex.ToString());
            }
            finally
            {
                GC.Collect();
            }
        }

        private void btnAnnuler_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AnnulerBookingForm annulerForm = new AnnulerBookingForm(this, utilisateur);
                annulerForm.Title = "Annulation du booking: " + cbNumBooking.Text;
                annulerForm.ShowDialog();
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        private void ctxImporterCtrNDS_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

                dlg.DefaultExt = ".xlsx";
                dlg.Filter = "Conteneurs export (*.xlsx)|*.xlsx";
                Nullable<bool> result = dlg.ShowDialog();
                if (result == true)
                {
                    gcTab.Visibility = System.Windows.Visibility.Collapsed;

                    string filename = dlg.FileName;

                    // vider les listes
                    dataGridConteneurs.ItemsSource = null;
                    dataGridGC.ItemsSource = null;

                    eltsBookingCtr = new List<ElementBookingCtr>();
                    eltsBookingGC = new List<ElementBookingGC>();

                    excelLoadConteneurNDS(filename);
                }
                else
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message, "Operation Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void excelLoadConteneurNDS(string path)
        {
            Excel.Application xlApp = null;
            Excel.Workbook xlWorkBook = null;
            Excel.Worksheet xlWorkSheet = null;
            Excel.Range range;

            try
            {
               // VSOMAccessors vsomAcc = new VSOMAccessors();
               // VsomParameters vsp = new VsomParameters();

                int rCnt = 2;

                xlApp = new Excel.Application();
                xlWorkBook = xlApp.Workbooks.Open(path, 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
                xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

                range = xlWorkSheet.UsedRange;

                List<TYPE_CONTENEUR> typesCtr = vsomAcc.GetTypesConteneurs();

                while (((string)(range.Cells[rCnt, 6] as Excel.Range).Value2) != null)
                {
                    ElementBookingCtr elt = new ElementBookingCtr();
                    elt.NumCtr = (string)(range.Cells[rCnt, 6] as Excel.Range).Value2;
                    elt.Description = (string)(range.Cells[rCnt, 20] as Excel.Range).Value2;
                    elt.UNCode = (string)(range.Cells[rCnt, 7] as Excel.Range).Value2;
                    elt.DescMses = (string)(range.Cells[rCnt, 20] as Excel.Range).Value2;
                    elt.Poids = Convert.ToInt32((double)(range.Cells[rCnt, 18] as Excel.Range).Value2);
                    elt.TypeCtr = "20BX";
                    elt.Volume = 0;
                    elt.StatutCtr = "Empty";
                    elt.TypeMsesCtr = "Autres produits";
                    elt.Seal1 = (string)(range.Cells[rCnt, 14] as Excel.Range).Value2;

                    eltsBookingCtr.Add(elt);

                    rCnt++;
                }

                dataGridConteneurs.ItemsSource = null;
                dataGridConteneurs.ItemsSource = eltsBookingCtr;

            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
            }
            finally
            {
                if (xlWorkSheet != null)
                {
                    releaseObject(xlWorkSheet);
                }

                if (xlWorkBook != null)
                {
                    xlWorkBook.Close(true, Type.Missing, Type.Missing);
                    releaseObject(xlWorkBook);
                }

                bool excelWasRunning = System.Diagnostics.Process.GetProcessesByName("EXCEL.EXE").Length > 0;

                if (excelWasRunning)
                {
                    xlApp.Quit();
                    releaseObject(xlApp);
                }
            }
        }

        private void ctxImporterCtrShipping_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

                dlg.DefaultExt = ".xlsx";
                dlg.Filter = "Conteneurs export (*.xlsx)|*.xlsx";
                Nullable<bool> result = dlg.ShowDialog();
                if (result == true)
                {
                    //gcTab.Visibility = System.Windows.Visibility.Collapsed;

                    string filename = dlg.FileName;

                    // vider les listes
                    dataGridConteneurs.ItemsSource = null;
                    //dataGridGC.ItemsSource = null;

                    eltsBookingCtr = new List<ElementBookingCtr>();
                    //eltsBookingGC = new List<ElementBookingGC>();

                    excelLoadConteneurShipping(filename);
                }
                else
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message, "Operation Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void excelLoadConteneurShipping(string path)
        {
            Excel.Application xlApp = null;
            Excel.Workbook xlWorkBook = null;
            Excel.Worksheet xlWorkSheet = null;
            Excel.Range range;

            try
            {
                //VSOMAccessors vsomAcc = new VSOMAccessors();
                //VsomParameters vsp = new VsomParameters();

                int rCnt = 2;

                xlApp = new Excel.Application();
                xlWorkBook = xlApp.Workbooks.Open(path, 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
                xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

                range = xlWorkSheet.UsedRange;

                List<TYPE_CONTENEUR> typesCtr = vsomAcc.GetTypesConteneurs();

                while (((string)(range.Cells[rCnt, 6] as Excel.Range).Value2) != null)
                {
                    ElementBookingCtr elt = new ElementBookingCtr();
                    elt.NumCtr = (string)(range.Cells[rCnt, 1] as Excel.Range).Value2;
                    elt.Description = (string)(range.Cells[rCnt, 2] as Excel.Range).Value2;
                    elt.UNCode = (string)(range.Cells[rCnt, 3] as Excel.Range).Value2;
                    elt.UNCode = elt.UNCode == null ? "" : elt.UNCode;
                    elt.DescMses = (string)(range.Cells[rCnt, 2] as Excel.Range).Value2;
                    elt.Poids = Convert.ToInt32((double)(range.Cells[rCnt, 9] as Excel.Range).Value2);
                    elt.TypeCtr = (string)(range.Cells[rCnt, 7] as Excel.Range).Value2;
                    elt.Volume = (float)(range.Cells[rCnt, 8] as Excel.Range).Value2;
                    elt.StatutCtr = (string)(range.Cells[rCnt, 6] as Excel.Range).Value2;
                    elt.TypeMsesCtr = (string)(range.Cells[rCnt, 5] as Excel.Range).Value2;
                    elt.Seal1 = (string)(range.Cells[rCnt, 10] as Excel.Range).Value2;
                    elt.Seal2 = (string)(range.Cells[rCnt, 11] as Excel.Range).Value2;

                    eltsBookingCtr.Add(elt);

                    rCnt++;
                }

                dataGridConteneurs.ItemsSource = null;
                dataGridConteneurs.ItemsSource = eltsBookingCtr;

            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
            }
            finally
            {
                if (xlWorkSheet != null)
                {
                    releaseObject(xlWorkSheet);
                }

                if (xlWorkBook != null)
                {
                    xlWorkBook.Close(true, Type.Missing, Type.Missing);
                    releaseObject(xlWorkBook);
                }

                bool excelWasRunning = System.Diagnostics.Process.GetProcessesByName("EXCEL.EXE").Length > 0;

                if (excelWasRunning)
                {
                    xlApp.Quit();
                    releaseObject(xlApp);
                }
            }
        }

        private void btnTransfertEscale_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TransfertBookingEscaleForm transfertBookingEscaleForm = new TransfertBookingEscaleForm(this, utilisateur);
                transfertBookingEscaleForm.Title = "Transfert d'escale - Booking N° " + cbNumBooking.Text;
                transfertBookingEscaleForm.ShowDialog();
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        private void ctxImporterGCShipping_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

                dlg.DefaultExt = ".xlsx";
                dlg.Filter = "Conventionnels export (*.xlsx)|*.xlsx";
                Nullable<bool> result = dlg.ShowDialog();
                if (result == true)
                {
                    //contTab.Visibility = System.Windows.Visibility.Collapsed;

                    string filename = dlg.FileName;

                    // vider les listes
                    //dataGridConteneurs.ItemsSource = null;
                    dataGridGC.ItemsSource = null;

                    //eltsBookingCtr = new List<ElementBookingCtr>();
                    eltsBookingGC = new List<ElementBookingGC>();

                    excelLoadConventionnelShipping(filename);
                }
                else
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Operation Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void excelLoadConventionnelShipping(string path)
        {
            Excel.Application xlApp = null;
            Excel.Workbook xlWorkBook = null;
            Excel.Worksheet xlWorkSheet = null;
            Excel.Range range;

            try
            {
                //VSOMAccessors vsomAcc = new VSOMAccessors();
                //VsomParameters vsp = new VsomParameters();
                int rCnt = 2;

                xlApp = new Excel.Application();
                xlWorkBook = xlApp.Workbooks.Open(path, 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
                xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

                range = xlWorkSheet.UsedRange;

                List<TYPE_CONVENTIONNEL> typesGC = vsomAcc.GetTypesConventionnelsExport();

                while (((string)(range.Cells[rCnt, 2] as Excel.Range).Value2) != null)
                {
                    ElementBookingGC elt = new ElementBookingGC();

                    elt.NumGC = (string)(range.Cells[rCnt, 1] as Excel.Range).Value2;
                    elt.Description = (string)(range.Cells[rCnt, 2] as Excel.Range).Value2;
                    elt.Hauteur = (float)(range.Cells[rCnt, 6] as Excel.Range).Value2;
                    elt.Largeur = (float)(range.Cells[rCnt, 5] as Excel.Range).Value2;
                    elt.Longueur = (float)(range.Cells[rCnt, 4] as Excel.Range).Value2;
                    elt.Poids = Math.Round((double)(range.Cells[rCnt, 8] as Excel.Range).Value2, 3);
                    elt.Quantite = Convert.ToInt16((double)(range.Cells[rCnt, 3] as Excel.Range).Value2);
                    elt.TypeMses = typesGC.FirstOrDefault<TYPE_CONVENTIONNEL>(t => t.CodeTypeGC == (string)(range.Cells[rCnt, 9] as Excel.Range).Value2).LibTypeGC;
                    elt.Volume = Math.Round((float)(range.Cells[rCnt, 7] as Excel.Range).Value2, 3);

                    eltsBookingGC.Add(elt);

                    rCnt++;
                }

                dataGridGC.ItemsSource = null;
                dataGridGC.ItemsSource = eltsBookingGC;

            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
            }
            finally
            {
                if (xlWorkSheet != null)
                {
                    releaseObject(xlWorkSheet);
                }

                if (xlWorkBook != null)
                {
                    xlWorkBook.Close(true, Type.Missing, Type.Missing);
                    releaseObject(xlWorkBook);
                }

                bool excelWasRunning = System.Diagnostics.Process.GetProcessesByName("EXCEL.EXE").Length > 0;

                if (excelWasRunning)
                {
                    xlApp.Quit();
                    releaseObject(xlApp);
                }
            }
        }

        private void ctxTransfertBookingGC_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TransfertBookingGCForm transfertBookingGCForm = new TransfertBookingGCForm(this, (ElementBookingGC)dataGridGC.SelectedItem, utilisateur);
                transfertBookingGCForm.Title = "Transfert de booking - Conventionnel N° " + ((ElementBookingGC)dataGridGC.SelectedItem).NumGC;
                transfertBookingGCForm.ShowDialog();
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        private void ctxTransfertBookingCtr_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TransfertBookingCtrForm transfertBookingCtrForm = new TransfertBookingCtrForm(this, (ElementBookingCtr)dataGridConteneurs.SelectedItem, utilisateur);
                transfertBookingCtrForm.Title = "Transfert de booking - Conteneur N° " + ((ElementBookingCtr)dataGridConteneurs.SelectedItem).NumCtr;
                transfertBookingCtrForm.ShowDialog();
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        private void btnEclaterBL_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                EclaterBLExportForm eclaterBLForm = new EclaterBLExportForm(this, Convert.ToInt32(txtSysId.Text), utilisateur);
                eclaterBLForm.Title = "Eclatement du BL export - N° " + cbNumBooking.Text;
                eclaterBLForm.ShowDialog();
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        private void btnChangerClient_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ChangerClientBookingForm changerClientBookingForm = new ChangerClientBookingForm(this, utilisateur);
                changerClientBookingForm.txtClientAncien.Text = cbClient.Text;
                changerClientBookingForm.txtCodeClientAncien.Text = txtCodeClient.Text;
                changerClientBookingForm.Title = "Changement de client - Booking N° " + cbNumBooking.Text;
                changerClientBookingForm.ShowDialog();
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        private void dataGridGC_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (dataGridGC.SelectedIndex != -1)
                {
                    ElementBookingGC elt = (ElementBookingGC)dataGridGC.SelectedItem;
                    UpdateVolPoidsConvForm updatevolPoidsGCForm = new UpdateVolPoidsConvForm(this, Convert.ToInt32(txtSysId.Text), utilisateur);
                    updatevolPoidsGCForm.cbElt.SelectedItem = elt.NumGC + " - " + elt.Description;
                    updatevolPoidsGCForm.Title = "Conventionnels - Booking N° " + cbNumBooking.Text;
                    updatevolPoidsGCForm.ShowDialog();
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

        private void dataGridCtrsDispo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (dataGridCtrsDispo.Items.Count != 0)
                {
                    DISPOSITION_CONTENEUR elt = (DISPOSITION_CONTENEUR)dataGridCtrsDispo.SelectedItem;

                    cbTypeCtrDispo.SelectedItem = elt.TypeCtr;
                    txtQteCtrDispo.Text = elt.NombreTC.ToString();
                    txtContactTransitaire.Text = elt.ContactTransitaire;
                    txtEmpotage.Text = elt.Empotage;
                    cbVia.Text = elt.SortieVia;
                    txtTypeHabillage.Text = elt.TypeHabillage;

                    if (elt.CONTENEUR_TC.Count != 0)
                    {
                        borderAjoutDispo.IsEnabled = false;
                    }
                    else
                    {
                        borderAjoutDispo.IsEnabled = true;
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

        private void btnAjoutDispo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (cbTypeCtrDispo.SelectedIndex == -1)
                {
                    MessageBox.Show("Vous devez saisir le type de conteneur dont il s'agit.", "Type de conteneur ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (txtQteCtrDispo.Text.Trim() == "")
                {
                    MessageBox.Show("Vous devez saisir le nombre de conteneur.", "Nombre de conteneur ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (cbVia.SelectedIndex == -1)
                {
                    MessageBox.Show("Vous devez saisir le moyen de transport des conteneurs", "Moyen de transport ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    txtQteCtrDispo.Text = txtQteCtrDispo.Text.Replace(" ", "");

                    DISPOSITION_CONTENEUR elt = new DISPOSITION_CONTENEUR();
                    elt.TypeCtr = cbTypeCtrDispo.Text;
                    elt.NombreTC = Convert.ToInt32(txtQteCtrDispo.Text);
                    elt.ContactTransitaire = txtContactTransitaire.Text.Trim();
                    elt.Empotage = txtEmpotage.Text.Trim();
                    elt.SortieVia = cbVia.Text;
                    elt.TypeHabillage = txtTypeHabillage.Text.Trim();
                    //elt.RefDisposition = "MAD" + FormatReferenceMAD(elt.IdDisposition);

                    if (eltsDispositionCtr == null)
                    {
                        eltsDispositionCtr = new List<DISPOSITION_CONTENEUR>();
                    }

                    eltsDispositionCtr.Add(elt);

                    dataGridCtrsDispo.ItemsSource = null;
                    dataGridCtrsDispo.ItemsSource = eltsDispositionCtr;

                    cbTypeCtrDispo.SelectedIndex = -1;
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

        private void btnModifierDispo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dataGridCtrsDispo.SelectedItems.Count == 0)
                {
                    MessageBox.Show("Veuillez sélectionner un type de conteneur à modifier", "Conteneur ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (cbTypeCtrDispo.SelectedIndex == -1)
                {
                    MessageBox.Show("Vous devez saisir le type de conteneur dont il s'agit.", "Type de conteneur ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (txtQteCtrDispo.Text.Trim() == "")
                {
                    MessageBox.Show("Vous devez saisir le nombre de conteneur.", "Nombre de conteneur ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    DISPOSITION_CONTENEUR elt = (DISPOSITION_CONTENEUR)dataGridCtrsDispo.SelectedItem;
                    elt.TypeCtr = cbTypeCtrDispo.Text;
                    elt.NombreTC = Convert.ToInt32(txtQteCtrDispo.Text);
                    elt.ContactTransitaire = txtContactTransitaire.Text.Trim();
                    elt.Empotage = txtEmpotage.Text.Trim();
                    elt.SortieVia = cbVia.Text;
                    elt.TypeHabillage = txtTypeHabillage.Text.Trim();

                    dataGridCtrsDispo.ItemsSource = null;
                    dataGridCtrsDispo.ItemsSource = eltsDispositionCtr;
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

        private void btnSupprimerDispo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (cbTypeCtrDispo.Text.Trim() != "")
                {
                    eltsDispositionCtr.RemoveAll(el => el.TypeCtr == cbTypeCtrDispo.Text);
                    dataGridCtrsDispo.ItemsSource = null;
                    dataGridCtrsDispo.ItemsSource = eltsDispositionCtr;
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

        private void txtQteCtrDispo_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void txtQteCtrDispo_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter)
                {
                    btnAjoutDispo_Click(sender, e);
                    cbTypeCtrDispo.SelectedIndex = -1;
                }
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

        private void dataGridEltsFact_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (dataGridEltsFact.SelectedIndex != -1)
                {
                    UpdateEltBookingForm updateEltForm = new UpdateEltBookingForm(this, Convert.ToInt32(txtSysId.Text), utilisateur);
                    updateEltForm.cbElt.SelectedItem = ((ElementFacturation)dataGridEltsFact.SelectedItem).LibArticle;
                    updateEltForm.Title = "Eléments de facturation - Booking N° " + cbNumBooking.Text;
                    updateEltForm.ShowDialog();
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

        private void txtContactTransitaire_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter)
                {
                    btnAjoutDispo_Click(sender, e);
                    cbTypeCtrDispo.SelectedIndex = -1;
                }
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

        private void txtEmpotage_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter)
                {
                    btnAjoutDispo_Click(sender, e);
                    cbTypeCtrDispo.SelectedIndex = -1;
                }
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

        private void txtTypeHabillage_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter)
                {
                    btnAjoutDispo_Click(sender, e);
                    cbTypeCtrDispo.SelectedIndex = -1;
                }
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

        private void cbTypeCtrDispo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                //VSOMAccessors vsomAcc = new VSOMAccessors();

                if (cbTypeCtrDispo.SelectedIndex != -1)
                {
                    txtQteEnStock.Text = vsomAcc.GetQuantiteCtrEnStock(typesConteneurs.ElementAt<TYPE_CONTENEUR>(cbTypeCtrDispo.SelectedIndex).CodeTypeCtr).ToString();
                }
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

        private void dataGridCtrsDispo_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (((DISPOSITION_CONTENEUR)dataGridCtrsDispo.SelectedItem).IdDisposition != 0)
                {
                    MiseDispositionReport miseDispositionReport = new MiseDispositionReport(this, ((DISPOSITION_CONTENEUR)dataGridCtrsDispo.SelectedItem).RefDisposition);
                    miseDispositionReport.Title = "Impression de la mise à disposition de conteneurs N° " + ((DISPOSITION_CONTENEUR)dataGridCtrsDispo.SelectedItem).RefDisposition + " - Booking : " + cbNumBooking.Text;
                    miseDispositionReport.Show();
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

        private void btnGenererCoparn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                 vsomAcc = new VSOMAccessors();

                CONNAISSEMENT booking = vsomAcc.GetBookingByIdBooking(Convert.ToInt32(txtSysId.Text));

                StringBuilder sb = new StringBuilder();
                DateTime dte = DateTime.Now;

                int i = 1;
                sb.Append("UNB+UNOA:1+").Append(booking.ESCALE.ARMATEUR.CodeArm == "INARME" ? "GRIMALDI" : booking.ESCALE.ARMATEUR.CodeArm).Append("+DITCAMDLA+" + booking.DCBL.Value.Year.ToString().Substring(2, 2) + FormatChiffre(booking.DCBL.Value.Month) + FormatChiffre(booking.DCBL.Value.Day) + ":" + FormatChiffre(booking.DCBL.Value.Hour) + FormatChiffre(booking.DCBL.Value.Minute) + "+" + booking.IdBL + "'").Append(Environment.NewLine);
                foreach (CONTENEUR ctr in booking.CONTENEUR)
                {
                    sb.Append("UNH+" + ctr.IdBL + FormatReferenceCOPARN(i) + "+COPARN:D:95B:UN'").Append(Environment.NewLine);
                    sb.Append("BGM+11+" + booking.DCBL.Value.Year + FormatChiffre(booking.DCBL.Value.Month) + FormatChiffre(booking.DCBL.Value.Day) + FormatChiffre(booking.DCBL.Value.Hour) + FormatChiffre(booking.DCBL.Value.Minute) + FormatChiffre(booking.DCBL.Value.Second) + "+5'").Append(Environment.NewLine);
                    sb.Append("RFF+BN:" + booking.NumBooking + "'").Append(Environment.NewLine);
                    //sb.Append("TDT+20+" + (booking.ESCALE.NumVoySCR.Length == 4 ? booking.ESCALE.NAVIRE.CodeNav + booking.ESCALE.NumVoySCR : booking.ESCALE.NumVoySCR) + "+1++" + booking.ESCALE.ARMATEUR.CodeArm + ":172:20+++" + booking.ESCALE.NAVIRE.CodeRadio + ":103::" + booking.ESCALE.NAVIRE.NomNav + "'").Append(Environment.NewLine);
                    if (booking.ESCALE.ARMATEUR.CodeArm == "INARME")
                    {
                        sb.Append("TDT+20+" + booking.ESCALE.NumVoyDIT + "+1++").Append("GRIMALDI").Append(":172:20+++" + booking.ESCALE.NAVIRE.CodeRadio + ":103::" + booking.ESCALE.NAVIRE.NomNav + "'").Append(Environment.NewLine);
                    }
                    else if((booking.ESCALE.ARMATEUR.CodeArm == "NILEDUTCH" || booking.ESCALE.ARMATEUR.CodeArm == "NILE DUTCH" || booking.ESCALE.ARMATEUR.CodeArm == "NDS"))
                    {
                        sb.Append("TDT+20+" + booking.ESCALE.NumVoyDIT + "+1++").Append("NDS").Append(":172:20+++" + booking.ESCALE.NAVIRE.CodeRadio + ":103::" + booking.ESCALE.NAVIRE.NomNav + "'").Append(Environment.NewLine);
                    }
                    sb.Append("LOC+88+CMDLA:139:6'").Append(Environment.NewLine);
                    sb.Append("LOC+9+CMDLA:139:6'").Append(Environment.NewLine);
                    sb.Append("DTM+133:" + booking.DCBL.Value.Year + FormatChiffre(booking.DCBL.Value.Month) + FormatChiffre(booking.DCBL.Value.Day) + FormatChiffre(booking.DCBL.Value.Hour) + FormatChiffre(booking.DCBL.Value.Minute) + FormatChiffre(booking.DCBL.Value.Second) + ":203'").Append(Environment.NewLine);
                    sb.Append("NAD+CZ++" + booking.ConsigneeBL + "'").Append(Environment.NewLine);
                    sb.Append("NAD+FW++'" + booking.NotifyBL + "'").Append(Environment.NewLine);
                    if (booking.ESCALE.ARMATEUR.CodeArm == "INARME")
                    {
                        sb.Append("NAD+CA+").Append("GRIMALDI").Append(":172:20'").Append(Environment.NewLine);
                    }
                    else if ((booking.ESCALE.ARMATEUR.CodeArm == "NILEDUTCH" || booking.ESCALE.ARMATEUR.CodeArm == "NILE DUTCH" || booking.ESCALE.ARMATEUR.CodeArm == "NDS"))
                    {
                        sb.Append("NAD+CA+").Append("NDS").Append(":172:20'").Append(Environment.NewLine);
                    }
                    sb.Append("GID+1'").Append(Environment.NewLine);
                    sb.Append("FTX+AAA+++" + booking.DescBL + "'").Append(Environment.NewLine);
                    if (ctr.TypeCCtr == "20BX" || ctr.TypeCCtr == "20DV")
                    {
                        sb.Append("EQD+CN+" + ctr.NumCtr + "+22G1:102:5+2+2+5'").Append(Environment.NewLine);
                    }
                    else if (ctr.TypeCCtr == "40BX" || ctr.TypeCCtr == "40DV")
                    {
                        sb.Append("EQD+CN+" + ctr.NumCtr + "+42G1:102:5+2+2+5'").Append(Environment.NewLine);
                    }
                    else if (ctr.TypeCCtr == "40HC")
                    {
                        sb.Append("EQD+CN+" + ctr.NumCtr + "+45G1:102:5+2+2+5'").Append(Environment.NewLine);
                    }
                    else if (ctr.TypeCCtr == "40OT")
                    {
                        sb.Append("EQD+CN+" + ctr.NumCtr + "+42U1:102:5+2+2+5'").Append(Environment.NewLine);
                    }
                    else if (ctr.TypeCCtr == "40FL")
                    {
                        sb.Append("EQD+CN+" + ctr.NumCtr + "+45P3:102:5+2+2+5'").Append(Environment.NewLine);
                    }
                    else if (ctr.TypeCCtr == "20OT")
                    {
                        sb.Append("EQD+CN+" + ctr.NumCtr + "+22U1:102:5+2+2+5'").Append(Environment.NewLine);
                    }
                    else
                    {
                        sb.Append("EQD+CN+" + ctr.NumCtr + "+" + ctr.TypeCCtr + ":102:5+2+2+5'").Append(Environment.NewLine);
                    }
                    sb.Append("RFF+BN:" + booking.NumBooking + "'").Append(Environment.NewLine);
                    sb.Append("EQN+35'").Append(Environment.NewLine);
                    sb.Append("TMD+3++2'").Append(Environment.NewLine);
                    sb.Append("LOC+8+" + booking.DPBL + ":139:6'").Append(Environment.NewLine);
                    sb.Append("LOC+98+" + booking.LPBL + ":139:6'").Append(Environment.NewLine);
                    sb.Append("LOC+11+" + booking.LDBL + ":139:6'").Append(Environment.NewLine);
                    /* 
                     * AH 23juin16
                     * commente pour ajour VGM  
                     
                     sb.Append("MEA+AAE+G+KGM:" + ctr.PoidsCCtr + "'").Append(Environment.NewLine);
                     if (ctr.TypeCCtr.Substring(0, 2) == "20")
                     {
                         sb.Append("MEA+AAE+T+KGM:2280'").Append(Environment.NewLine);
                     }
                     else if (ctr.TypeCCtr.Substring(0, 2) == "40")
                     {
                         sb.Append("MEA+AAE+T+KGM:4480'").Append(Environment.NewLine);
                     }

                     if (ctr.TypeCCtr.Substring(0, 2) == "20")
                     {
                         sb.Append("MEA+AAE+EGW+KGM:" + (ctr.PoidsCCtr.Value + 2280).ToString() + "'").Append(Environment.NewLine);
                     }
                     else if (ctr.TypeCCtr.Substring(0, 2) == "40")
                     {
                         sb.Append("MEA+AAE+T+KGM:" + (ctr.PoidsCCtr.Value + 4480).ToString() + "'").Append(Environment.NewLine);
                     }
                    */ 

                    //AH 23JUIN16 ligne de la VGM 
                    sb.Append("MEA+AAE+VGM+KGM:" + ctr.VGM.Value.ToString() + "'").Append(Environment.NewLine);
                    

                    sb.Append("FTX+AAI+++" + ctr.DescMses + "'").Append(Environment.NewLine);
                    
                    sb.Append("CNT+16:1'").Append(Environment.NewLine);
                    sb.Append("UNT+25+" + booking.IdBL + FormatReferenceCOPARN(i) + "'").Append(Environment.NewLine);
                    i++;
                }
                sb.Append("UNZ+1+" + booking.IdBL).Append(Environment.NewLine);

                System.IO.File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\COPARN - " + booking.IdBL + ".EDI", sb.ToString(), Encoding.GetEncoding("ISO-8859-1"));

                Microsoft.Office.Interop.Outlook.Application app = new Microsoft.Office.Interop.Outlook.Application();
                Microsoft.Office.Interop.Outlook.MailItem mailItem = app.CreateItem(Microsoft.Office.Interop.Outlook.OlItemType.olMailItem);
                mailItem.Subject = "COPARN (Socomar)";
                mailItem.To = "edi.tedi@socomarcm.net";
                mailItem.Body = "COPARN (Socomar)";
                mailItem.Attachments.Add(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\COPARN - " + booking.IdBL + ".EDI");
                mailItem.Importance = Microsoft.Office.Interop.Outlook.OlImportance.olImportanceHigh;
                mailItem.Display(false);
                mailItem.Send();

                MessageBox.Show("COPARN généré", "COPARN généré !", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        private static string FormatReferenceCOPARN(int entier)
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

        private static string FormatReferenceMAD(int entier)
        {
            Int32 i = entier;
            if (i >= 10000000)
            {
                return i.ToString();
            }
            else if (i >= 1000000)
            {
                return "0" + i.ToString();
            }
            else if (i >= 100000)
            {
                return "00" + i.ToString();
            }
            else if (i >= 10000)
            {
                return "000" + i.ToString();
            }
            else if (i >= 1000)
            {
                return "0000" + i.ToString();
            }
            else if (i >= 100)
            {
                return "00000" + i.ToString();
            }
            else if (i >= 10)
            {
                return "000000" + i.ToString();
            }
            else
            {
                return "0000000" + i.ToString();
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
