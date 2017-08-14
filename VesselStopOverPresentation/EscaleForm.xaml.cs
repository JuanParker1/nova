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
using System.IO;
using System.IO.Compression;

namespace VesselStopOverPresentation
{
    /// <summary>
    /// Logique d'interaction pour EscaleForm.xaml
    /// </summary>
    public partial class EscaleForm : Window
    {
        private List<NAVIRE> navires;
        public List<string> navs { get; set; }

        private List<ACCONIER> acconiers;
        public List<string> accs { get; set; }

        private List<ARMATEUR> armateurs;
        public List<string> arms { get; set; }

        public List<ESCALE> escales { get; set; }
        /// <summary>
        /// list  de numero d'escal 
        /// </summary>
        public List<Int32> escs { get; set; }

        public List<MANIFESTE> manifestes { get; set; }
        public List<FACTURE_ARMATEUR> facturesArmateur { get; set; }
        public List<FACTURE_SOCOMAR> facturesSocomar { get; set; }
        public List<ElementCompteEscale> eltsValeurEsc { get; set; }
        public List<ElementCompteDIT> eltsCompteDIT { get; set; }
        public List<ElementCompteSEPBC> eltsCompteSEPBC { get; set; }
        public List<ElementCompteFactureDIT> facturesDIT { get; set; }
        public List<ElementFacturation> eltsFact { get; set; }
        public List<ElementCompteArmRegroup> eltsRegroupArm { get; set; }
        public StatutLoadUnload statutDechargement { get; set; }

        private EscalePanel escPanel;
        private UTILISATEUR utilisateur;
        private List<OPERATION> operationsUser;

        private string typeForm;

        private FormLoader formLoader;
        private VsomParameters vsp = new VsomParameters();
        public EscaleForm(EscalePanel panel, ESCALE esc, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;
                
                InitializeCombos();

                escPanel = panel;
                utilisateur = user;
                operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);

                formLoader = new FormLoader(utilisateur);
                
                formLoader.LoadEscaleForm(this, esc);
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

        public EscaleForm(string type, EscalePanel panel, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

                InitializeCombos();

                cbAcconier.SelectedItem = "Socomar";
                cbNumEscale.Focus();
                actionsBorder.Visibility = System.Windows.Visibility.Collapsed;
                borderEtat.Visibility = System.Windows.Visibility.Collapsed;

                typeForm = type;
                escPanel = panel;
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

        private void InitializeCombos()
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();
                VsomParameters vsp = new VsomParameters();
                navires = vsp.GetNaviresActifs();
                navs = new List<string>();
                foreach (NAVIRE nav in navires)
                {
                    navs.Add(nav.NomNav);
                }

                armateurs = vsp.GetArmateursActifs();
                arms = new List<string>();
                foreach (ARMATEUR arm in armateurs)
                {
                    arms.Add(arm.NomArm);
                }

                acconiers = vsp.GetAcconiersActifs();
                accs = new List<string>();
                foreach (ACCONIER acc in acconiers)
                {
                    accs.Add(acc.NomAcc);
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
                VSOMAccessors vsomAcc = new VSOMAccessors();

                if (cbNumEscale.Text.Trim() == "")
                {
                    MessageBox.Show("Vous n'avez pas saisi le numéro de l'escale ; elle est indispensable pour la création d'une escale.", "N° escale ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    cbNumEscale.Focus();
                }
                else if (txtDateDepart.SelectedDate == null)
                {
                    MessageBox.Show("Vous n'avez pas saisi la date de départ du navire.", "Date de départ du navire ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (txtDateArrPrev.SelectedDate == null)
                {
                    MessageBox.Show("Vous n'avez pas saisi la date d'arrivée prévue du navire.", "Date d'arrivée prévue ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (txtDateArrReelle.SelectedDate.HasValue && !txtDateDech.SelectedDate.HasValue)
                {
                    MessageBox.Show("Vous n'avez pas saisi la date d'arrivée réelle du navire", "Date d'arrivée réelle du navire ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (txtNbManifestesPrevus.Text.Trim() == "")
                {
                    MessageBox.Show("Vous devez indiquer le nombre de manifestes import prévu pour cette escale.", "Nombre de manifeste import ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    txtNbManifestesPrevus.Focus();
                }
                else if (cbNavire.SelectedIndex == -1)
                {
                    MessageBox.Show("Vous n'avez pas saisi le nom du navire affreté pour cette escale.", "Navire ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (cbArmateur.SelectedIndex == -1)
                {
                    MessageBox.Show("Vous n'avez pas saisi le nom de l'armateur ayant affreté ce navire.", "Armateur ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (cbAcconier.SelectedIndex == -1)
                {
                    MessageBox.Show("Vous devez indiquer l'acconier pour lequel cette escale est créée.", "Acconier ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    if (typeForm == "Nouveau")
                    {
                        if (txtDateArrReelle.SelectedDate.HasValue == true)
                        {
                            ESCALE es = vsomAcc.InsertEscale(Convert.ToInt32(cbNumEscale.Text), txtNumVoyageSCR.Text, txtNumVoyageDIT.Text, txtNomCapt.Text, txtDateDepart.SelectedDate.Value, txtDateArrPrev.SelectedDate.Value, txtDateArrReelle.SelectedDate.Value, txtDateDech.SelectedDate.Value, txtNumSydonia.Text.Trim(), Convert.ToInt16(txtNbManifestesPrevus.Text), (chkGestPar.IsChecked == true ? "Y" : "N"), (chkRepresentant.IsChecked == true ? "Y" : "N"), (chkAcconier.IsChecked == true ? "Y" : "N"), (chkConsignataire.IsChecked == true ? "Y" : "N"), new TextRange(txtObservations.Document.ContentStart, txtObservations.Document.ContentEnd).Text, navires.ElementAt<NAVIRE>(cbNavire.SelectedIndex).IdNav, armateurs.ElementAt<ARMATEUR>(cbArmateur.SelectedIndex).IdArm, acconiers.ElementAt<ACCONIER>(cbAcconier.SelectedIndex).IdAcc, utilisateur.IdU);
                            if (escPanel.cbFiltres.SelectedIndex != 1)
                            {
                                escPanel.cbFiltres.SelectedIndex = 1;
                            }
                            else
                            {
                                escPanel.escales = vsp.GetEscalesByStatut("En cours");
                                escPanel.dataGrid.ItemsSource = escPanel.escales;
                                escPanel.lblStatut.Content = escPanel.escales.Count + " Escale(s) en cours";
                            }

                            formLoader.LoadEscaleForm(this, es);

                            valeurEscTab.Visibility = System.Windows.Visibility.Visible;
                            //compteDITTab.Visibility = System.Windows.Visibility.Visible;
                            compteSEPBCTab.Visibility = System.Windows.Visibility.Visible;
                            comptePADTab.Visibility = System.Windows.Visibility.Visible;
                            compteArmateurTab.Visibility = System.Windows.Visibility.Visible;
                            actionsBorder.Visibility = System.Windows.Visibility.Visible;
                        }
                        else
                        {
                            ESCALE es = vsomAcc.InsertEscale(Convert.ToInt32(cbNumEscale.Text), txtNumVoyageSCR.Text, txtNumVoyageDIT.Text, txtNomCapt.Text, txtDateDepart.SelectedDate.Value, txtDateArrPrev.SelectedDate.Value, txtNumSydonia.Text.Trim(), Convert.ToInt16(txtNbManifestesPrevus.Text), (chkGestPar.IsChecked == true ? "Y" : "N"), (chkRepresentant.IsChecked == true ? "Y" : "N"), (chkAcconier.IsChecked == true ? "Y" : "N"), (chkConsignataire.IsChecked == true ? "Y" : "N"), new TextRange(txtObservations.Document.ContentStart, txtObservations.Document.ContentEnd).Text, navires.ElementAt<NAVIRE>(cbNavire.SelectedIndex).IdNav, armateurs.ElementAt<ARMATEUR>(cbArmateur.SelectedIndex).IdArm, acconiers.ElementAt<ACCONIER>(cbAcconier.SelectedIndex).IdAcc, utilisateur.IdU);
                            if (escPanel.cbFiltres.SelectedIndex != 1)
                            {
                                escPanel.cbFiltres.SelectedIndex = 1;
                            }
                            else
                            {
                                escPanel.escales = vsp.GetEscalesByStatut("En cours");
                                escPanel.dataGrid.ItemsSource = escPanel.escales;
                                escPanel.lblStatut.Content = escPanel.escales.Count + " Escale(s) en cours";
                            }

                            formLoader.LoadEscaleForm(this, es);

                            valeurEscTab.Visibility = System.Windows.Visibility.Visible;
                            //compteDITTab.Visibility = System.Windows.Visibility.Visible;
                            compteSEPBCTab.Visibility = System.Windows.Visibility.Visible;
                            comptePADTab.Visibility = System.Windows.Visibility.Visible;
                            compteArmateurTab.Visibility = System.Windows.Visibility.Visible;
                            actionsBorder.Visibility = System.Windows.Visibility.Visible;
                        }
                        MessageBox.Show("Enregistrement effectué avec succès.", "Enregistrement effectué !", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        if (txtDateArrReelle.SelectedDate.HasValue == true)
                        {
                            ESCALE es = vsomAcc.UpdateEscale(Convert.ToInt32(txtEscaleSysID.Text), Convert.ToInt32(cbNumEscale.Text), txtNumVoyageSCR.Text, txtNumVoyageDIT.Text, txtNomCapt.Text, txtDateDepart.SelectedDate.Value, txtDateArrPrev.SelectedDate.Value, txtDateArrReelle.SelectedDate.Value, txtDateDech.SelectedDate.Value, txtNumSydonia.Text.Trim(), Convert.ToInt16(txtNbManifestesPrevus.Text), (chkGestPar.IsChecked == true ? "Y" : "N"), (chkRepresentant.IsChecked == true ? "Y" : "N"), (chkAcconier.IsChecked == true ? "Y" : "N"), (chkConsignataire.IsChecked == true ? "Y" : "N"), new TextRange(txtObservations.Document.ContentStart, txtObservations.Document.ContentEnd).Text, navires.ElementAt<NAVIRE>(cbNavire.SelectedIndex).IdNav, armateurs.ElementAt<ARMATEUR>(cbArmateur.SelectedIndex).IdArm, acconiers.ElementAt<ACCONIER>(cbAcconier.SelectedIndex).IdAcc, utilisateur.IdU);
                            if (escPanel.cbFiltres.SelectedIndex != 1)
                            {
                                escPanel.cbFiltres.SelectedIndex = 1;
                            }
                            else
                            {
                                escPanel.escales = vsp.GetEscalesByStatut("En cours");
                                escPanel.dataGrid.ItemsSource = escPanel.escales;
                                escPanel.lblStatut.Content = escPanel.escales.Count + " Escale(s) en cours";
                            }
                            formLoader.LoadEscaleForm(this, es);
                        }
                        else
                        {
                            ESCALE es = vsomAcc.UpdateEscale(Convert.ToInt32(txtEscaleSysID.Text), Convert.ToInt32(cbNumEscale.Text), txtNumVoyageSCR.Text, txtNumVoyageDIT.Text, txtNomCapt.Text, txtDateDepart.SelectedDate.Value, txtDateArrPrev.SelectedDate.Value, txtNumSydonia.Text.Trim(), Convert.ToInt16(txtNbManifestesPrevus.Text), (chkGestPar.IsChecked == true ? "Y" : "N"), (chkRepresentant.IsChecked == true ? "Y" : "N"), (chkAcconier.IsChecked == true ? "Y" : "N"), (chkConsignataire.IsChecked == true ? "Y" : "N"), new TextRange(txtObservations.Document.ContentStart, txtObservations.Document.ContentEnd).Text, navires.ElementAt<NAVIRE>(cbNavire.SelectedIndex).IdNav, armateurs.ElementAt<ARMATEUR>(cbArmateur.SelectedIndex).IdArm, acconiers.ElementAt<ACCONIER>(cbAcconier.SelectedIndex).IdAcc, utilisateur.IdU);
                            if (escPanel.cbFiltres.SelectedIndex != 1)
                            {
                                escPanel.cbFiltres.SelectedIndex = 1;
                            }
                            else
                            {
                                escPanel.escales = vsp.GetEscalesByStatut("En cours");
                                escPanel.dataGrid.ItemsSource = escPanel.escales;
                                escPanel.lblStatut.Content = escPanel.escales.Count + " Escale(s) en cours";
                            }
                            formLoader.LoadEscaleForm(this, es);
                        }
                        MessageBox.Show("L'escale a été mise à jour avec succès.", "Mise à jour effectuée !", MessageBoxButton.OK, MessageBoxImage.Information);

                    }
                }
            }
            catch (HabilitationException ex)
            {
                MessageBox.Show(ex.Message, "Droits insuffisants !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            catch (EscaleException ex)
            {
                MessageBox.Show(ex.Message, "N° escale déjà attribué !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message + "\n" + ex.StackTrace, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void cbNavire_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                txtCodeNavire.Text = navires.ElementAt<NAVIRE>(cbNavire.SelectedIndex).CodeNav;
                if (typeForm == "Nouveau")
                {
                    cbArmateur.SelectedItem = navires.ElementAt<NAVIRE>(cbNavire.SelectedIndex).ARMATEUR.NomArm;
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

        private void cbArmateur_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                txtCodeArmateur.Text = armateurs.ElementAt<ARMATEUR>(cbArmateur.SelectedIndex).CodeArm;
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
            
        }

        private void cbAcconier_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                txtCodeAcconier.Text = acconiers.ElementAt<ACCONIER>(cbAcconier.SelectedIndex).CodeAcc;
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        private void txtNbManifestesPrevus_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void dataGridManifeste_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                VsomParameters vsomAcc = new VsomParameters();

                if (dataGridManifeste.SelectedIndex != -1)
                {
                    MANIFESTE man = vsomAcc.GetManifesteByIdMan(((MANIFESTE)dataGridManifeste.SelectedItem).IdMan);
                    if (man.StatutMan == "Del")
                    {
                        MessageBox.Show("Ce manifeste n'est pas disponible pour traitement", "Escale");
                    }
                    else
                    {
                        ManifesteForm manifesteForm = new ManifesteForm(this, man, utilisateur);
                        manifesteForm.cbNumEscale.IsEnabled = false;
                        manifesteForm.Show();
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

        private void cbNumEscale_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void btnImportMan_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                if (operationsUser.Where(op => op.NomOp == "Manifeste : Enregistrement d'un nouvel élément").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
                {
                    MessageBox.Show("Vous n'avez pas les droits nécessaires pour créer un nouveau manifeste. Veuillez contacter un administrateur", "Droits insuffisants !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    ManifesteForm manifesteForm = new ManifesteForm("Import", this, utilisateur);
                    manifesteForm.cbIdMan.IsEnabled = false;
                    manifesteForm.cbNumEscale.SelectedItem = Convert.ToInt32(cbNumEscale.Text);
                    manifesteForm.cbNumEscale.IsEnabled = false;
                    manifesteForm.Title = "Importation de manifeste - Escale : " + cbNumEscale.Text;
                    manifesteForm.conTab.IsSelected = true;
                    manifesteForm.compteManTab.Visibility = System.Windows.Visibility.Collapsed;
                    //manifesteForm.compteDITTab.Visibility = System.Windows.Visibility.Collapsed;
                    manifesteForm.Show();
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

        private void txtNumVoyage_LostFocus(object sender, RoutedEventArgs e)
        {
            txtNumVoyageSCR.Text = txtNumVoyageSCR.Text.ToUpper();
        }

        private void valeurEscale_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            valeurEscTab.IsSelected = true;
        }

        private void cpteDIT_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //compteDITTab.IsSelected = true;
        }

        private void statManifeste_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            manifTab.IsSelected = true;
        }

        private void btnAjoutNote_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                NoteForm noteForm = new NoteForm("Nouveau", this, utilisateur);
                noteForm.Title = "Création de note - Escale " + cbNumEscale.Text;
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

        private void montantFactureCpteSEPBC_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            compteSEPBCTab.IsSelected = true;
        }

        private void montantFactureCptePAD_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            comptePADTab.IsSelected = true;
        }

        private void cbNumEscale_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                if (e.Key == Key.Return && cbNumEscale.Text.Trim() != "" && typeForm != "Nouveau")
                {
                    int result;
                    escales = vsp.GetEscalesByNumEscale(Int32.TryParse(cbNumEscale.Text.Trim(), out result) ? result : -1);

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

        private void listNotes_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (listNotes.SelectedIndex != -1)
                {
                    NOTE note = (NOTE)listNotes.SelectedItem;
                    NoteForm noteForm = new NoteForm(this, note, utilisateur);
                    noteForm.Title = "Note - " + note.IdNote + " - Escale - " + note.ESCALE.NumEsc;
                    noteForm.lblStatut.Content = "Note crée le : " + note.DateNote + " par " + note.UTILISATEUR.NU;
                    noteForm.ShowDialog();
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

        private void btnCloturer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CloturerEscaleForm clotureForm = new CloturerEscaleForm(this, utilisateur);
                clotureForm.Title = "Clôture de l'escale : " + cbNumEscale.Text;
                clotureForm.ShowDialog();
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        private void listStatuts_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (listStatuts.SelectedIndex != -1)
                {
                    OPERATION_ESCALE opEsc = (OPERATION_ESCALE)listStatuts.SelectedItem;
                    if (opEsc.DateOp.HasValue)
                    {
                        StatutForm statutForm = new StatutForm(opEsc);
                        statutForm.Title = "Statut - " + opEsc.TYPE_OPERATION.LibTypeOp;
                        statutForm.ShowDialog();
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

        private void dataGridCompteEsc_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();
                if (dataGridCompteEsc.SelectedIndex != -1)
                {
                    ElementCompte eltCompte = (ElementCompte)dataGridCompteEsc.SelectedItem;
                    if (eltCompte.TypeDoc == "FA")
                    {
                        FactureForm factForm = new FactureForm(this, vsp.GetFactureByIdFact(eltCompte.Id), utilisateur);
                        factForm.Show();
                    }
                    else if (eltCompte.TypeDoc == "PA")
                    {
                        PaiementForm payForm = new PaiementForm(this, vsp.GetPaiementByIdPay(eltCompte.Id), utilisateur);
                        payForm.btnEnregistrer.IsEnabled = false;
                        payForm.Show();
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

        private void btnMAJNumEsc_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                MAJNumEscaleForm mAJNumEscaleForm = new MAJNumEscaleForm(this, vsp.GetEscaleById(Convert.ToInt32(txtEscaleSysID.Text)), utilisateur);
                mAJNumEscaleForm.txtAncienNumEscale.Text = cbNumEscale.Text;
                mAJNumEscaleForm.Title = "Mise à jour du n° d'escale - Escale N° : " + cbNumEscale.Text;
                mAJNumEscaleForm.ShowDialog();
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        private void btnSummary_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SummaryOPForm summaryForm = new SummaryOPForm(this, utilisateur);
                formLoader.LoadEscaleForm(summaryForm, escales.ElementAt<ESCALE>(cbNumEscale.SelectedIndex));
                summaryForm.ShowDialog();
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message + "\n" + ex.StackTrace, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnFacturerArm_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FacturerArmateurForm factureArmateurForm = new FacturerArmateurForm(this, utilisateur);
                factureArmateurForm.Title = "Facture armateur - Escale : " + cbNumEscale.Text;
                factureArmateurForm.ShowDialog();
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        private void btnImprimerBooking_Click(object sender, RoutedEventArgs e)
        {
            Excel.Application xlApp = null;
            Excel.Workbook xlWorkBook = null;
            Excel.Worksheet xlWorkSheet = null;
            Excel.Range range;

            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                ESCALE esc = vsp.GetEscaleById(Convert.ToInt32(txtEscaleSysID.Text));

                xlApp = new Excel.Application();
                xlWorkBook = xlApp.Workbooks.Open(Environment.CurrentDirectory + "//Ressources//Booking.xlsx", 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
                xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

                range = xlWorkSheet.UsedRange;

                (range.Cells[3, 4] as Excel.Range).Value2 = esc.NAVIRE.NomNav;
                (range.Cells[5, 4] as Excel.Range).Value2 = "Douala";
                (range.Cells[3, 10] as Excel.Range).Value2 = esc.NumVoySCR;
                (range.Cells[5, 10] as Excel.Range).Value2 = esc.NumEsc;
                (range.Cells[3, 16] as Excel.Range).Value2 = esc.DRAEsc.HasValue ? esc.DRAEsc.Value.ToShortDateString() : esc.DPAEsc.Value.ToShortDateString();

                int i = 8;
                List<CONNAISSEMENT> books = esc.CONNAISSEMENT.Where(con => con.SensBL == "E" && con.StatutBL != "Annulé").OrderBy(c => c.LDBL).ThenBy(c => c.DPBL).ThenBy(c => c.DCBLI).ToList<CONNAISSEMENT>();
                string lpbl = "";
                string dpbl = "";
                if (books.Count > 0)
                {
                    lpbl = books.FirstOrDefault<CONNAISSEMENT>().LPBL;
                    dpbl = books.FirstOrDefault<CONNAISSEMENT>().DPBL;
                }

                xlWorkSheet.Columns[22].NumberFormat = "[$-00040C]jj/mm/aaaa;@";
                foreach (CONNAISSEMENT book in books)
                {
                    if (i == 8)
                    {
                        i++;
                        (range.Cells[i, 1] as Excel.Range).Value2 = book.LPBL; // A revoir
                        (range.Cells[i, 2] as Excel.Range).Value2 = book.DPBL; // A revoir
                        range.Range[range.Cells[i, 3], range.Cells[i, 22]].Merge();
                        range.Range[range.Cells[i, 1], range.Cells[i, 22]].Font.Bold = true;
                    }
                    if (lpbl != book.LPBL || dpbl != book.DPBL)
                    {
                        if (i > 10)
                        {
                            i++;
                            range.Range[range.Cells[i, 1], range.Cells[i, 8]].Merge();
                            range.Range[range.Cells[i, 17], range.Cells[i, 22]].Merge();
                            range.Range[range.Cells[i, 1], range.Cells[i, 22]].Font.Bold = true;
                            (range.Cells[i, 9] as Excel.Range).Value2 = esc.CONNAISSEMENT.Where(b => b.SensBL == "E" && (b.LPBL == lpbl && b.DPBL == dpbl)).Sum(b => b.PoidsBL);
                            (range.Cells[i, 10] as Excel.Range).Value2 = esc.CONNAISSEMENT.Where(b => b.SensBL == "E" && (b.LPBL == lpbl && b.DPBL == dpbl)).Sum(b => b.VolBL);
                            (range.Cells[i, 11] as Excel.Range).Value2 = esc.CONVENTIONNEL.Where(gc => gc.SensGC == "E" && (gc.CONNAISSEMENT.LPBL == lpbl && gc.CONNAISSEMENT.DPBL == dpbl)).Sum(gc => gc.NumItem);
                            (range.Cells[i, 12] as Excel.Range).Value2 = esc.CONVENTIONNEL.Where(gc => gc.SensGC == "E" && (gc.CONNAISSEMENT.LPBL == lpbl && gc.CONNAISSEMENT.DPBL == dpbl)).Sum(gc => gc.QteRGC);
                            //(range.Cells[i, 13] as Excel.Range).Value2 = esc.CONTENEUR.Where(ctr => ctr.SensCtr == "E" && (ctr.CONNAISSEMENT.LPBL == lpbl && ctr.CONNAISSEMENT.DPBL == dpbl)).Count(ctr => ctr.TypeCCtr.Substring(0, 2) == "20");
                            //(range.Cells[i, 16] as Excel.Range).Value2 = esc.CONTENEUR.Where(ctr => ctr.SensCtr == "E" && (ctr.CONNAISSEMENT.LPBL == lpbl && ctr.CONNAISSEMENT.DPBL == dpbl)).Count(ctr => ctr.TypeCCtr.Substring(0, 2) == "40");

                            (range.Cells[i, 13] as Excel.Range).Value2 = esc.CONTENEUR.Where(ctr => ctr.SensCtr == "E" && (ctr.CONNAISSEMENT.LPBL == lpbl && ctr.CONNAISSEMENT.DPBL == dpbl)).Count(ctr => ctr.TypeCCtr.Substring(0, 4) == "20DV");
                            (range.Cells[i, 14] as Excel.Range).Value2 = esc.CONTENEUR.Where(ctr => ctr.SensCtr == "E" && (ctr.CONNAISSEMENT.LPBL == lpbl && ctr.CONNAISSEMENT.DPBL == dpbl)).Count(ctr => ctr.TypeCCtr.Substring(0, 4) == "20VT");
                            (range.Cells[i, 15] as Excel.Range).Value2 = esc.CONTENEUR.Where(ctr => ctr.SensCtr == "E" && (ctr.CONNAISSEMENT.LPBL == lpbl && ctr.CONNAISSEMENT.DPBL == dpbl)).Count(ctr => ctr.TypeCCtr.Substring(0, 4) == "40DV");
                            (range.Cells[i, 16] as Excel.Range).Value2 = esc.CONTENEUR.Where(ctr => ctr.SensCtr == "E" && (ctr.CONNAISSEMENT.LPBL == lpbl && ctr.CONNAISSEMENT.DPBL == dpbl)).Count(ctr => ctr.TypeCCtr.Substring(0, 4) == "40HC");
                        }
                        i++;
                        range.Range[range.Cells[i, 1], range.Cells[i, 22]].Merge();
                        i++;
                        (range.Cells[i, 1] as Excel.Range).Value2 = book.LPBL; // A revoir
                        (range.Cells[i, 2] as Excel.Range).Value2 = book.DPBL; // A revoir
                        range.Range[range.Cells[i, 3], range.Cells[i, 22]].Merge();
                        range.Range[range.Cells[i, 1], range.Cells[i, 22]].Font.Bold = true;

                        lpbl = book.LPBL;
                        dpbl = book.DPBL;
                    }
                    i++;
                    (range.Cells[i, 2] as Excel.Range).Value2 = book.NumBooking;
                    (range.Cells[i, 3] as Excel.Range).Value2 = book.ConsigneeBL;
                    (range.Cells[i, 4] as Excel.Range).Value2 = book.NotifyBL;
                    (range.Cells[i, 5] as Excel.Range).Value2 = book.CCBL;
                    (range.Cells[i, 6] as Excel.Range).Value2 = book.DescBL;
                    (range.Cells[i, 7] as Excel.Range).Value2 = book.TypeBL;
                    (range.Cells[i, 8] as Excel.Range).Value2 = book.CONTENEUR.FirstOrDefault<CONTENEUR>(ctr => ctr.IMDGCode.Trim() != "") != null ? book.CONTENEUR.FirstOrDefault<CONTENEUR>(ctr => ctr.IMDGCode.Trim() != "").IMDGCode : "";
                    (range.Cells[i, 9] as Excel.Range).Value2 = book.PoidsBL;
                    (range.Cells[i, 10] as Excel.Range).Value2 = book.VolBL;
                    (range.Cells[i, 11] as Excel.Range).Value2 = book.CONVENTIONNEL.Sum(gc => gc.NumItem);
                    (range.Cells[i, 12] as Excel.Range).Value2 = book.CONVENTIONNEL.Sum(gc => gc.QteRGC);
                    (range.Cells[i, 13] as Excel.Range).Value2 = book.CONTENEUR.Count(ctr => ctr.TypeCCtr.Substring(0, 4) == "20DV");
                    (range.Cells[i, 14] as Excel.Range).Value2 = book.CONTENEUR.Count(ctr => ctr.TypeCCtr.Substring(0, 4) == "20VT");
                    (range.Cells[i, 15] as Excel.Range).Value2 = book.CONTENEUR.Count(ctr => ctr.TypeCCtr.Substring(0, 4) == "40DV");
                    (range.Cells[i, 16] as Excel.Range).Value2 = book.CONTENEUR.Count(ctr => ctr.TypeCCtr.Substring(0, 4) == "40HC");
                    (range.Cells[i, 17] as Excel.Range).Value2 = book.AIBL;
                    (range.Cells[i, 19] as Excel.Range).Value2 = book.ClearAgent;
                    (range.Cells[i, 20] as Excel.Range).Value2 = book.NumCtrBL;
                    (range.Cells[i, 21] as Excel.Range).Value2 = book.NumDEBL;
                    if (!book.DCBLI.HasValue)
                    {
                        (range.Cells[i, 22] as Excel.Range).Value2 = "";
                        range.Range[range.Cells[i, 1], range.Cells[i, 22]].Font.Italic = true;
                    }
                    else
                    {
                        (range.Cells[i, 22] as Excel.Range).Value2 = book.DCBLI.Value;
                    }
                }

                if (i > 10)
                {
                    i++;
                    range.Range[range.Cells[i, 1], range.Cells[i, 8]].Merge();
                    range.Range[range.Cells[i, 17], range.Cells[i, 22]].Merge();
                    range.Range[range.Cells[i, 1], range.Cells[i, 22]].Font.Bold = true;
                    (range.Cells[i, 9] as Excel.Range).Value2 = esc.CONNAISSEMENT.Where(book => book.SensBL == "E" && (book.LPBL == lpbl && book.DPBL == dpbl)).Sum(book => book.PoidsBL);
                    (range.Cells[i, 10] as Excel.Range).Value2 = esc.CONNAISSEMENT.Where(book => book.SensBL == "E" && (book.LPBL == lpbl && book.DPBL == dpbl)).Sum(book => book.VolBL);
                    (range.Cells[i, 11] as Excel.Range).Value2 = esc.CONVENTIONNEL.Where(gc => gc.SensGC == "E" && (gc.CONNAISSEMENT.LPBL == lpbl && gc.CONNAISSEMENT.DPBL == dpbl)).Sum(gc => gc.NumItem);
                    (range.Cells[i, 12] as Excel.Range).Value2 = esc.CONVENTIONNEL.Where(gc => gc.SensGC == "E" && (gc.CONNAISSEMENT.LPBL == lpbl && gc.CONNAISSEMENT.DPBL == dpbl)).Sum(gc => gc.QteRGC);
                    (range.Cells[i, 13] as Excel.Range).Value2 = esc.CONTENEUR.Where(ctr => ctr.SensCtr == "E" && (ctr.CONNAISSEMENT.LPBL == lpbl && ctr.CONNAISSEMENT.DPBL == dpbl)).Count(ctr => ctr.TypeCCtr.Substring(0, 4) == "20DV");
                    (range.Cells[i, 14] as Excel.Range).Value2 = esc.CONTENEUR.Where(ctr => ctr.SensCtr == "E" && (ctr.CONNAISSEMENT.LPBL == lpbl && ctr.CONNAISSEMENT.DPBL == dpbl)).Count(ctr => ctr.TypeCCtr.Substring(0, 4) == "20VT");
                    (range.Cells[i, 15] as Excel.Range).Value2 = esc.CONTENEUR.Where(ctr => ctr.SensCtr == "E" && (ctr.CONNAISSEMENT.LPBL == lpbl && ctr.CONNAISSEMENT.DPBL == dpbl)).Count(ctr => ctr.TypeCCtr.Substring(0, 4) == "40DV");
                    (range.Cells[i, 16] as Excel.Range).Value2 = esc.CONTENEUR.Where(ctr => ctr.SensCtr == "E" && (ctr.CONNAISSEMENT.LPBL == lpbl && ctr.CONNAISSEMENT.DPBL == dpbl)).Count(ctr => ctr.TypeCCtr.Substring(0, 4) == "40HC");

                    i++;
                    range.Range[range.Cells[i, 1], range.Cells[i, 22]].Merge();
                    i++;
                    (range.Cells[i, 1] as Excel.Range).Value2 = "GRAND TOTAL VESSEL";
                    (range.Cells[i, 9] as Excel.Range).Value2 = esc.CONNAISSEMENT.Where(book => book.SensBL == "E").Sum(book => book.PoidsBL);
                    (range.Cells[i, 10] as Excel.Range).Value2 = esc.CONNAISSEMENT.Where(book => book.SensBL == "E").Sum(book => book.VolBL);
                    (range.Cells[i, 11] as Excel.Range).Value2 = esc.CONVENTIONNEL.Where(gc => gc.SensGC == "E").Sum(gc => gc.NumItem);
                    (range.Cells[i, 13] as Excel.Range).Value2 = esc.CONTENEUR.Where(ctr => ctr.SensCtr == "E").Count(ctr => ctr.TypeCCtr.Substring(0, 4) == "20DV");
                    (range.Cells[i, 14] as Excel.Range).Value2 = esc.CONTENEUR.Where(ctr => ctr.SensCtr == "E").Count(ctr => ctr.TypeCCtr.Substring(0, 4) == "20VT");
                    (range.Cells[i, 15] as Excel.Range).Value2 = esc.CONTENEUR.Where(ctr => ctr.SensCtr == "E").Count(ctr => ctr.TypeCCtr.Substring(0, 4) == "40DV");
                    (range.Cells[i, 16] as Excel.Range).Value2 = esc.CONTENEUR.Where(ctr => ctr.SensCtr == "E").Count(ctr => ctr.TypeCCtr.Substring(0, 4) == "40HC");
                    range.Range[range.Cells[i, 1], range.Cells[i, 8]].Merge();
                    range.Range[range.Cells[i, 17], range.Cells[i, 22]].Merge();
                    range.Range[range.Cells[i, 1], range.Cells[i, 22]].Font.Bold = true;
                    i++;
                    (range.Cells[i, 9] as Excel.Range).Value2 = "TONS";
                    (range.Cells[i, 10] as Excel.Range).Value2 = "CBM";
                    (range.Cells[i, 11] as Excel.Range).Value2 = "COLIS";
                    (range.Cells[i, 13] as Excel.Range).Value2 = "DV";
                    (range.Cells[i, 14] as Excel.Range).Value2 = "VT";
                    (range.Cells[i, 15] as Excel.Range).Value2 = "DV";
                    (range.Cells[i, 16] as Excel.Range).Value2 = "VT";
                    range.Range[range.Cells[i, 1], range.Cells[i, 8]].Merge();
                    range.Range[range.Cells[i, 17], range.Cells[i, 22]].Merge();
                    range.Range[range.Cells[i, 1], range.Cells[i, 22]].Font.Bold = true;
                    i++;
                    (range.Cells[i, 13] as Excel.Range).Value2 = "20\"";
                    (range.Cells[i, 15] as Excel.Range).Value2 = "40\"";
                    range.Range[range.Cells[i, 1], range.Cells[i, 12]].Merge();
                    range.Range[range.Cells[i, 13], range.Cells[i, 14]].Merge();
                    range.Range[range.Cells[i, 15], range.Cells[i, 16]].Merge();
                    range.Range[range.Cells[i, 17], range.Cells[i, 22]].Merge();
                    range.Range[range.Cells[i, 1], range.Cells[i, 22]].Font.Bold = true;
                    i++;
                    (range.Cells[i, 11] as Excel.Range).Value2 = "TARE";
                    (range.Cells[i, 13] as Excel.Range).Value2 = esc.CONTENEUR.Where(ctr => ctr.SensCtr == "E").Count(ctr => ctr.TypeCCtr.Substring(0, 2) == "20") * 2.2 + esc.CONTENEUR.Where(ctr => ctr.SensCtr == "E").Count(ctr => ctr.TypeCCtr.Substring(0, 2) == "40") * 4.4;
                    (range.Cells[i, 19] as Excel.Range).Value2 = "MTS";
                    range.Range[range.Cells[i, 1], range.Cells[i, 10]].Merge();
                    range.Range[range.Cells[i, 11], range.Cells[i, 12]].Merge();
                    range.Range[range.Cells[i, 13], range.Cells[i, 16]].Merge();
                    range.Range[range.Cells[i, 20], range.Cells[i, 22]].Merge();
                    range.Range[range.Cells[i, 1], range.Cells[i, 22]].Font.Bold = true;
                }

                range.Range[range.Cells[9, 1], range.Cells[i, 22]].Columns.Borders.LineStyle = 1;
                //range.Range[range.Cells[9, 1], range.Cells[i, 22]].Columns.Borders.Width = 2;

                xlWorkBook.SaveAs(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\Booking Export - Escale - " + esc.NumEsc.ToString() + ".xlsx", Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Excel.XlSaveAsAccessMode.xlExclusive, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                MessageBox.Show("Edition du Booking terminée", "Edition du Booking terminée !", MessageBoxButton.OK, MessageBoxImage.Information);
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

        private void btnImprimerRecap_Click(object sender, RoutedEventArgs e)
        {
            Excel.Application xlApp = null;
            Excel.Workbook xlWorkBook = null;
            Excel.Worksheet xlWorkSheet = null;
            Excel.Range range;

            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();
                VsomParameters vsp = new VsomParameters();
                ESCALE esc = vsp.GetEscaleById(Convert.ToInt32(txtEscaleSysID.Text));

                xlApp = new Excel.Application();
                xlWorkBook = xlApp.Workbooks.Open(Environment.CurrentDirectory + "//Ressources//Recap3.xlsx", 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
                xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

                range = xlWorkSheet.UsedRange;

                //(range.Cells[1, 1] as Excel.Range).Value2 = esc.NAVIRE.NomNav + " " + esc.NumVoySCR;

                int i = 5;
                List<CONNAISSEMENT> books = esc.CONNAISSEMENT.Where(con => con.SensBL == "E" && con.StatutBL != "Annulé").OrderBy(c => c.LDBL).ThenBy(c => c.DPBL).ToList<CONNAISSEMENT>();

                xlWorkSheet.Columns[4].NumberFormat = "[$-00040C]jj/mm/aaaa;@";
                foreach (CONNAISSEMENT book in books)
                {
                    foreach (CONTENEUR ctr in book.CONTENEUR)
                    {
                        (range.Cells[i, 1] as Excel.Range).Value2 = book.ESCALE.NAVIRE.NomNav;
                        (range.Cells[i, 2] as Excel.Range).Value2 = book.ESCALE.NumVoySCR;
                        (range.Cells[i, 3] as Excel.Range).Value2 = "Douala";
                        (range.Cells[i, 4] as Excel.Range).Value2 = DateTime.Now;
                        (range.Cells[i, 5] as Excel.Range).Value2 = vsp.GetPortsByCodePort(book.DPBL).FirstOrDefault<PORT>().NomPortEN;
                        if (book.DPBL != book.LPBL)
                        {
                            (range.Cells[i, 6] as Excel.Range).Value2 = vsp.GetPortsByCodePort(book.LPBL).FirstOrDefault<PORT>().NomPortEN;
                        }
                        (range.Cells[i, 7] as Excel.Range).Value2 = book.NumBL;
                        (range.Cells[i, 8] as Excel.Range).Value2 = book.ConsigneeBL;
                        (range.Cells[i, 9] as Excel.Range).Value2 = book.NotifyBL;
                        //(range.Cells[i, 10] as Excel.Range).Value2 = book.CONTENEUR.Count + book.CONVENTIONNEL.Sum(gc => gc.QteBGC.HasValue ? gc.QteBGC.Value : gc.NumItem.Value);
                        (range.Cells[i, 10] as Excel.Range).Value2 = 1;
                        //(range.Cells[i, 11] as Excel.Range).Value2 = book.CONTENEUR.Count != 0 ? book.CONTENEUR.First<CONTENEUR>().TypeCCtr : "BDLS";
                        (range.Cells[i, 11] as Excel.Range).Value2 = ctr.TypeCCtr;
                        (range.Cells[i, 12] as Excel.Range).Value2 = book.DescBL;
                        //(range.Cells[i, 13] as Excel.Range).Value2 = book.CONTENEUR.Count != 0 ? book.CONTENEUR.First<CONTENEUR>().NumCtr : "";
                        (range.Cells[i, 13] as Excel.Range).Value2 = ctr.NumCtr;
                        //(range.Cells[i, 14] as Excel.Range).Value2 = book.VolBL;
                        (range.Cells[i, 14] as Excel.Range).Value2 = ctr.VolMCtr;
                        //(range.Cells[i, 15] as Excel.Range).Value2 = book.PoidsBL;
                        (range.Cells[i, 15] as Excel.Range).Value2 = ctr.PoidsCCtr;

                        if (ctr.TypeCCtr.Substring(0, 2) == "20")
                        {
                            (range.Cells[i, 20] as Excel.Range).Value2 = 6.096;
                            (range.Cells[i, 21] as Excel.Range).Value2 = 2.438;
                            (range.Cells[i, 22] as Excel.Range).Value2 = ctr.TypeCCtr == "20HC" ? 2.896 : 2.591;
                        }
                        else if (ctr.TypeCCtr.Substring(0, 2) == "40")
                        {
                            (range.Cells[i, 20] as Excel.Range).Value2 = 12.192;
                            (range.Cells[i, 21] as Excel.Range).Value2 = 2.438;
                            (range.Cells[i, 22] as Excel.Range).Value2 = ctr.TypeCCtr == "40HC" ? 2.896 : 2.591;
                        }

                        //AH 7juillet16 ajout vgm
                        (range.Cells[i, 29] as Excel.Range).Value2 = ctr.VGM;

                        i++;
                    }

                    foreach (CONVENTIONNEL gc in book.CONVENTIONNEL)
                    {
                        (range.Cells[i, 1] as Excel.Range).Value2 = book.ESCALE.NAVIRE.NomNav;
                        (range.Cells[i, 2] as Excel.Range).Value2 = book.ESCALE.NumVoySCR;
                        (range.Cells[i, 3] as Excel.Range).Value2 = "Douala";
                        (range.Cells[i, 4] as Excel.Range).Value2 = DateTime.Now;
                        (range.Cells[i, 5] as Excel.Range).Value2 = vsp.GetPortsByCodePort(book.DPBL).FirstOrDefault<PORT>().NomPortEN;
                        if (book.DPBL != book.LPBL)
                        {
                            (range.Cells[i, 6] as Excel.Range).Value2 = vsp.GetPortsByCodePort(book.LPBL).FirstOrDefault<PORT>().NomPortEN;
                        }
                        (range.Cells[i, 7] as Excel.Range).Value2 = book.NumBL;
                        (range.Cells[i, 8] as Excel.Range).Value2 = book.ConsigneeBL;
                        (range.Cells[i, 9] as Excel.Range).Value2 = book.NotifyBL;
                        (range.Cells[i, 10] as Excel.Range).Value2 = gc.QteBGC.HasValue ? gc.QteBGC.Value : gc.NumItem.Value;
                        (range.Cells[i, 11] as Excel.Range).Value2 = "Bundles";
                        (range.Cells[i, 12] as Excel.Range).Value2 = book.DescBL;
                        (range.Cells[i, 13] as Excel.Range).Value2 = gc.NumGC;
                        (range.Cells[i, 14] as Excel.Range).Value2 = gc.VolMGC.HasValue ? gc.VolMGC.Value : gc.VolCGC.Value;
                        (range.Cells[i, 15] as Excel.Range).Value2 = gc.PoidsMGC.HasValue ? gc.PoidsMGC.Value : gc.PoidsCGC.Value;

                        i++;
                    }

                }

                Type colorType = typeof(System.Drawing.Color);
                System.Reflection.PropertyInfo[] propInfos;
                Random rand = new Random();
                propInfos = colorType.GetProperties(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);

                if (i > 4)
                {
                    Random random = new Random();
                    int j = 5;
                    System.Drawing.Color color = System.Drawing.Color.FromName(propInfos[rand.Next(0, propInfos.Length)].Name);
                    string lpbl = null;// (range.Cells[j, 5] as Excel.Range).Value2;

                    while ((range.Cells[j + 1, 5] as Excel.Range).Value2 != null)
                    {
                        if ((range.Cells[j, 5] as Excel.Range).Value2 != lpbl)
                        {
                            color = System.Drawing.Color.FromName(propInfos[rand.Next(0, propInfos.Length)].Name);
                        }

                        range.Range[range.Cells[j, 5], range.Cells[j, 5]].Interior.Color = color;
                        lpbl = (range.Cells[j, 5] as Excel.Range).Value2;
                        j++;
                    }

                    if ((range.Cells[j, 5] as Excel.Range).Value2 != lpbl)
                    {
                        color = System.Drawing.Color.FromName(propInfos[rand.Next(0, propInfos.Length)].Name);
                    }
                    range.Range[range.Cells[j, 5], range.Cells[j, 5]].Interior.Color = color;
                }

                range.Range[range.Cells[5, 1], range.Cells[i - 1, 29]].Columns.Borders.LineStyle = 1;
                range.Range[range.Cells[5, 1], range.Cells[i - 1, 29]].Font.Name = "Tw Cen MT";

                xlWorkBook.SaveAs(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\Recap Export - Escale - " + esc.NumEsc.ToString() + ".xlsx", Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Excel.XlSaveAsAccessMode.xlExclusive, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                MessageBox.Show("Edition du recap booking terminée", "Edition du recap booking terminée !", MessageBoxButton.OK, MessageBoxImage.Information);
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

        private void btnImportSOCAR_Click(object sender, RoutedEventArgs e)
        {
            Excel.Application xlApp = null;
            Excel.Workbook xlWorkBook = null;
            Excel.Worksheet xlWorkSheet = null;
            Excel.Range range;

            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

                dlg.DefaultExt = ".xlsx";
                dlg.Filter = "Infos SOCAR (*.xls)|*.xls|Infos SOCAR (*.xlsx)|*.xlsx";
                Nullable<bool> result = dlg.ShowDialog();
                string filename = "";
                if (result == true)
                {
                    filename = dlg.FileName;
                }
                else
                {
                    return;
                }

                int rCnt = 2;

                if (filename != "")
                {
                    xlApp = new Excel.Application();
                    xlWorkBook = xlApp.Workbooks.Open(filename, 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
                    xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

                    range = xlWorkSheet.UsedRange;

                    int idEsc = Convert.ToInt32(txtEscaleSysID.Text);

                    StringBuilder sb = new StringBuilder();

                    while (((string)(range.Cells[rCnt, 3] as Excel.Range).Value2) != null)
                    {
                        string numBL = ((string)(range.Cells[rCnt, 4] as Excel.Range).Value2).Substring(0, 11).Replace("-", "");
                        CONNAISSEMENT bl = vsomAcc.UpdateInfosSOCAR(idEsc, numBL);

                        if (bl != null)
                        {
                            sb.Append("Mise à jour OK : Ligne - ").Append(rCnt.ToString()).Append(" ").Append(bl.NumBL).Append(" ").Append(bl.ConsigneeBL).Append(Environment.NewLine);
                            range.Range[range.Cells[rCnt, 1], range.Cells[rCnt, 29]].Interior.Color = System.Drawing.Color.LightGreen;
                        }
                        else
                        {
                            sb.Append("Mise à jour KO : Ligne - ").Append(rCnt.ToString()).Append(" ").Append(numBL).Append(Environment.NewLine);
                            range.Range[range.Cells[rCnt, 1], range.Cells[rCnt, 29]].Interior.Color = System.Drawing.Color.Magenta;
                        }

                        rCnt++;
                    }

                    DirectoryInfo parentDir = Directory.GetParent(filename);
                    System.IO.File.WriteAllText(parentDir.FullName + "\\Log Importation SOCAR.txt", sb.ToString(), Encoding.GetEncoding("ISO-8859-1"));
                    xlWorkBook.SaveAs(parentDir.FullName + "\\SOCAR Report.xlsx", Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Excel.XlSaveAsAccessMode.xlExclusive, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                    MessageBox.Show("Importation terminée", "Importation terminée !", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

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

        private void btnManifestASYCUDA_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                ESCALE esc = vsp.GetEscaleById(Convert.ToInt32(txtEscaleSysID.Text));

                var manifestData = new Manifeste();

                DateTime dateJour = DateTime.Now;

                ManifesteGeneral_segment generalSegment = new ManifesteGeneral_segment();
                generalSegment.General_segment_id = new ManifesteGeneral_segmentGeneral_segment_id
                {
                    Customs_office_code = "CMDLP",
                    Date_of_arrival = esc.DRAEsc.HasValue ? esc.DRAEsc.Value.Year + FormatChiffre(esc.DRAEsc.Value.Month) + FormatChiffre(esc.DRAEsc.Value.Day) : esc.DPAEsc.Value.Year + FormatChiffre(esc.DPAEsc.Value.Month) + FormatChiffre(esc.DPAEsc.Value.Day),
                    Date_of_departure = esc.DDEsc.Value,
                    //Time_of_arrival = man.ESCALE.DRAEsc.HasValue ?  man.ESCALE.DRAEsc.Value : man.ESCALE.DPAEsc.Value,
                    Time_of_arrivalSpecified = false,
                    Voyage_number = esc.NumVoySCR
                };

                generalSegment.Load_unload_place = new ManifesteGeneral_segmentLoad_unload_place
                {
                    Place_of_departure_code = esc.MANIFESTE.FirstOrDefault<MANIFESTE>().CodePort,
                    Place_of_destination_code = "CMDLP"
                };

                generalSegment.Tonnage = new ManifesteGeneral_segmentTonnage
                {
                    Tonnage_gross_weight = Math.Round(esc.VEHICULE.Sum(veh => veh.PoidsMVeh.Value) + esc.CONTENEUR.Sum(ctr => ctr.PoidsMCtr.Value) + esc.CONVENTIONNEL.Sum(gc => gc.PoidsMGC.Value), 3),
                    Tonnage_net_weight = Math.Round(esc.VEHICULE.Sum(veh => veh.PoidsMVeh.Value) + esc.CONTENEUR.Sum(ctr => ctr.PoidsMCtr.Value) + esc.CONVENTIONNEL.Sum(gc => gc.PoidsMGC.Value), 3)
                };

                generalSegment.Totals_segment = new ManifesteGeneral_segmentTotals_segment
                {
                    Total_gross_mass = Math.Round(esc.VEHICULE.Sum(veh => veh.PoidsMVeh.Value) + esc.CONTENEUR.Where(ctr => ctr.SensCtr == "I").Sum(ctr => ctr.PoidsMCtr.Value) + esc.CONVENTIONNEL.Where(gc => gc.SensGC == "I").Sum(gc => gc.PoidsMGC.Value), 3),
                    Total_number_of_bols = esc.CONNAISSEMENT.Count(bl => bl.SensBL == "I"),
                    Total_number_of_containers = esc.CONTENEUR.Count(ctr => ctr.SensCtr == "I"),
                    Total_number_of_packages = esc.VEHICULE.Count(veh => veh.SensVeh == "I") + esc.CONTENEUR.Count(ctr => ctr.SensCtr == "I") + esc.CONVENTIONNEL.Count(gc => gc.SensGC == "I")
                };

                generalSegment.Transport_information = new ManifesteGeneral_segmentTransport_information
                {
                    Carrier = new ManifesteGeneral_segmentTransport_informationCarrier
                    {
                        Carrier_address1 = "BP 12351 Douala",
                        Carrier_address2 = "",
                        Carrier_address3 = "",
                        Carrier_address4 = "",
                        Carrier_code = "5014",
                        Carrier_name = "SOCOMAR"
                    },
                    Date_of_registration = dateJour.Year + FormatChiffre(dateJour.Month) + FormatChiffre(dateJour.Day),
                    Identity_of_transporter = esc.NAVIRE.NomNav,
                    Master_information = esc.NomCpt,
                    Mode_of_transport_code = "10",
                    Nationality_of_transporter_code = esc.NAVIRE.AINav,
                    Place_of_transporter = esc.NAVIRE.NomNav,
                    Registration_number_of_transport_code = esc.NAVIRE.NomNav,
                    Date_of_last_discharge = esc.DDechEsc.Value.Year + FormatChiffre(esc.DDechEsc.Value.Month) + FormatChiffre(esc.DDechEsc.Value.Day)
                };

                manifestData.General_segment = generalSegment;

                //manifestData.CODE = "MAN" + esc.NumEsc;

                manifestData.Bol_segment = new ManifesteBol_segment[esc.CONNAISSEMENT.Count];

                int i = 0;
                foreach (CONNAISSEMENT bl in esc.CONNAISSEMENT.Where(con => con.SensBL == "I"))
                {
                    ManifesteBol_segment bolSegment = new ManifesteBol_segment();

                    bolSegment.Bol_id = new ManifesteBol_segmentBol_id
                    {
                        Bol_nature = bl.BLIL == "Y" ? ManifesteBol_segmentBol_idBol_nature.Item24 : ManifesteBol_segmentBol_idBol_nature.Item23,
                        Bol_reference = bl.NumBL,
                        Bol_type_code = "CO2",
                        Line_number = new ManifesteBol_segmentBol_idLine_number
                        {
                            Value = i
                        }
                        //Master_bol_ref_number = bl.NumBL
                        //Unique_carrier_reference = bl.ESCALE.ARMATEUR.CodeArm
                    };

                    bolSegment.Goods_segment = new ManifesteBol_segmentGoods_segment
                    {
                        Gross_mass = Math.Round(bl.VEHICULE.Sum(veh => veh.PoidsMVeh.Value) + bl.CONTENEUR.Sum(ctr => ctr.PoidsMCtr.Value) + bl.CONVENTIONNEL.Sum(gc => gc.PoidsMGC.Value), 3),
                        Information = bl.DescBL,
                        Num_of_ctn_for_this_bol = bl.CONTENEUR.Count,
                        Number_of_packages = bl.CONVENTIONNEL.Count + bl.VEHICULE.Count + bl.CONTENEUR.Count,
                        Volume_in_cubic_meters = Math.Round(bl.VEHICULE.Sum(veh => veh.VolMVeh.Value) + bl.CONTENEUR.Sum(ctr => ctr.VolMCtr.Value) + bl.CONVENTIONNEL.Sum(gc => gc.VolMGC.Value), 3),
                        Volume_in_cubic_metersSpecified = true
                    };

                    bolSegment.Goods_segment.Goods_detail_segment = new ManifesteBol_segmentGoods_segmentGoods_detail_segment[bl.VEHICULE.Count + bl.CONTENEUR.Count + bl.CONVENTIONNEL.Count];

                    int j = 0;
                    foreach (VEHICULE veh in bl.VEHICULE)
                    {
                        bolSegment.Goods_segment.Goods_detail_segment[j] = new ManifesteBol_segmentGoods_segmentGoods_detail_segment
                        {
                            Goods_description = veh.DescVeh.Length > 35 ? veh.DescVeh.Substring(0, 35) : veh.DescVeh,
                            Gross_mass = veh.PoidsCVeh.Value,
                            Number_of_packages = 1,
                            //Package_type_code = "VEHICULE(S)",
                            Package_type_code = "VEHICULE",
                            Seals_segment = new ManifesteBol_segmentGoods_segmentGoods_detail_segmentSeals_segment
                            {
                                Marks_of_seals = "",
                                Number_of_seals = 1,
                                Number_of_sealsSpecified = true,
                                Sealing_party_code = "SGS"
                            },
                            //Shipping_marks = veh.NumChassis + " " + veh.DescVeh,
                            Shipping_marks = veh.NumChassis,
                            Volume_in_cubic_meters = Math.Round(veh.VolMVeh.Value, 3),
                            Volume_in_cubic_metersSpecified = true
                        };
                        j++;
                    }

                    foreach (CONTENEUR ctr in bl.CONTENEUR)
                    {
                        bolSegment.Goods_segment.Goods_detail_segment[j] = new ManifesteBol_segmentGoods_segmentGoods_detail_segment
                        {
                            Goods_description = ctr.DescMses.Length > 35 ? ctr.DescMses.Substring(0, 35) : ctr.DescMses,
                            Gross_mass = ctr.PoidsMCtr.Value,
                            Number_of_packages = 1,
                            Package_type_code = "COLIS",
                            Seals_segment = new ManifesteBol_segmentGoods_segmentGoods_detail_segmentSeals_segment
                            {
                                Marks_of_seals = ctr.Seal1Ctr + (ctr.Seal2Ctr.Trim() != "" ? ("-" + ctr.Seal2Ctr) : ""),
                                Number_of_seals = (ctr.Seal1Ctr.Trim() != "" ? 1 : 0) + (ctr.Seal2Ctr.Trim() != "" ? 1 : 0),
                                Number_of_sealsSpecified = true,
                                Sealing_party_code = "SGS"
                            },
                            Shipping_marks = ctr.NumCtr,
                            Volume_in_cubic_meters = Math.Round(ctr.VolMCtr.Value, 3),
                            Volume_in_cubic_metersSpecified = true
                        };
                        j++;
                    }

                    foreach (CONVENTIONNEL gc in bl.CONVENTIONNEL)
                    {
                        bolSegment.Goods_segment.Goods_detail_segment[j] = new ManifesteBol_segmentGoods_segmentGoods_detail_segment
                        {
                            Goods_description = gc.DescGC.Length > 35 ? gc.DescGC.Substring(0, 35) : gc.DescGC,
                            Gross_mass = gc.PoidsCGC.Value,
                            Number_of_packages = 1,
                            Package_type_code = "COLIS",
                            Seals_segment = new ManifesteBol_segmentGoods_segmentGoods_detail_segmentSeals_segment
                            {
                                Marks_of_seals = "",
                                Number_of_seals = 1,
                                Number_of_sealsSpecified = true,
                                Sealing_party_code = "SGS"
                            },
                            Shipping_marks = gc.NumGC,
                            Volume_in_cubic_meters = Math.Round(gc.VolMGC.Value, 3),
                            Volume_in_cubic_metersSpecified = true
                        };
                        j++;
                    }

                    bolSegment.Load_unload_place = new ManifesteBol_segmentLoad_unload_place
                    {
                        Place_of_loading_code = bl.LPBL,
                        Place_of_unloading_code = "CMDLP"
                    };

                    bolSegment.Location = new ManifesteBol_segmentLocation
                    {
                        Location_code = bl.VEHICULE.Count > 0 ? "SOCOMAR_AUTOS" : (bl.CONTENEUR.Count > 0 ? "DIT_SOCOMAR" : "SOCOMAR_N°_9"),
                        Location_info = bl.VEHICULE.Count > 0 ? "Parc autos SOCOMAR" : (bl.CONTENEUR.Count > 0 ? "DIT_SOCOMAR" : "Magasin N° 9 (SOCOMAR)")
                    };

                    bolSegment.Traders_segment = new ManifesteBol_segmentTraders_segment
                    {
                        Consignee = new ManifesteBol_segmentTraders_segmentConsignee
                        {
                            Consignee_address1 = bl.AdresseConsignee,
                            Consignee_address2 = "",
                            Consignee_address3 = "",
                            Consignee_address4 = "",
                            Consignee_code = "",
                            Consignee_name = bl.ConsigneeBL
                        },
                        Exporter = new ManifesteBol_segmentTraders_segmentExporter
                        {
                            Exporter_address1 = bl.AdresseCharger,
                            Exporter_address2 = "",
                            Exporter_address3 = "",
                            Exporter_address4 = "",
                            Exporter_name = bl.NomCharger
                        },
                        Notify = new ManifesteBol_segmentTraders_segmentNotify
                        {
                            Notify_address1 = bl.AdresseNotify,
                            Notify_address2 = "",
                            Notify_address3 = "",
                            Notify_address4 = "",
                            Notify_code = "",
                            Notify_name = bl.NotifyBL
                        }
                    };

                    bolSegment.Value_segment = new ManifesteBol_segmentValue_segment
                    {
                        Customs_segment = new ManifesteBol_segmentValue_segmentCustoms_segment
                        {
                            Customs_currency = "XAF",
                            //Customs_value = 0,
                            Customs_valueSpecified = false
                        },
                        Freight_segment = new ManifesteBol_segmentValue_segmentFreight_segment
                        {
                            Freight_currency = "XAF",
                            Freight_value = 0,
                            PC_indicator = bl.CCBL == "Y" ? "YES" : "NO"
                        },
                        Insurance_segment = new ManifesteBol_segmentValue_segmentInsurance_segment
                        {
                            Insurance_currency = "XAF",
                            //Insurance_value = 0,
                            Insurance_valueSpecified = false
                        },
                        Transport_segment = new ManifesteBol_segmentValue_segmentTransport_segment
                        {
                            Transport_currency = "XAF",
                            //Transport_value = 0
                            Transport_valueSpecified = false
                        }
                    };

                    bolSegment.ctn_segment = new ManifesteBol_segmentCtn_segment[bl.CONTENEUR.Count];

                    int k = 0;
                    foreach (CONTENEUR ctr in bl.CONTENEUR)
                    {
                        bolSegment.ctn_segment[k] = new ManifesteBol_segmentCtn_segment
                        {
                            Ctn_reference = ctr.NumCtr,
                            Empty_Full = ctr.StatutCtr == "E" ? "1/0" : "1/1",
                            Marks1 = ctr.Seal1Ctr,
                            Marks2 = ctr.Seal2Ctr,
                            Marks3 = "",
                            Number_of_packages = 1,
                            Sealing_Party = "SGS",
                            Seals = ctr.Seal1Ctr + (ctr.Seal2Ctr.Trim() != "" ? ("-" + ctr.Seal2Ctr) : ""),
                            Type_of_container = ctr.TypeCCtr.Substring(0, 2)
                        };
                        k++;
                    }

                    manifestData.Bol_segment[i] = bolSegment;
                    i++;
                }

                var serializer = new System.Xml.Serialization.XmlSerializer(typeof(Manifeste));
                using (var stream = new StreamWriter(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\Manifest GUCE - " + esc.NumEsc + ".xml"))
                    serializer.Serialize(stream, manifestData);

                MessageBox.Show("Edition du manifeste ASYCUDA terminée", "Edition du manifeste terminée !", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
            }
        }

        private void btnImprimerFactureStevedoring_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FactureStevedoringReport factStevedoringReport = new FactureStevedoringReport(this);
                factStevedoringReport.Title = "Impression de la facture de stevedoring - Escale : " + cbNumEscale.Text;
                factStevedoringReport.Show();
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        private void dataGridFactArm_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                FactureArmateurReport factArmateurReport = new FactureArmateurReport(this, ((FACTURE_ARMATEUR)dataGridFactArm.SelectedItem).IdDocSAP.Value);
                factArmateurReport.Title = "Impression de la facture armateur N° " + ((FACTURE_ARMATEUR)dataGridFactArm.SelectedItem).IdDocSAP.ToString() + " - Escale : " + cbNumEscale.Text;
                factArmateurReport.Show();
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        private void btnRapportDeb_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                //VSOMAccessors vsomAcc = new VSOMAccessors();

                //if (operationsUser.Where(op => op.NomOp == "Report : Rapport débarquement").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
                //{
                //    MessageBox.Show("Vous n'avez pas les droits nécessaires pour éditer le rapport de débarquement. Veuillez contacter un administrateur", "Droits insuffisants !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                //}
                //else
                //{
                //    ESCALE esc = vsp.GetEscaleById(Convert.ToInt32(txtEscaleSysID.Text));

                //    StringBuilder sb = new StringBuilder();
                //    DateTime dte = DateTime.Now;

                //    List<CONTENEUR> listCtrs = vsp.GetConteneursDebarques(esc.IdEsc);
                //    int numMessage = vsomAcc.GetNumMessageGR();

                //    int i = 1;

                //    sb.Append("SOCOMAR").Append(Environment.NewLine);
                //    sb.Append("B.P:12351 Douala Cameroun").Append(Environment.NewLine);
                //    sb.Append("Til:(237) 33424550. Fax:(237) 33424936").Append(Environment.NewLine);
                //    sb.Append("Nr.Message: ").Append(numMessage).Append(" Date ").Append(dte.Day + "." + dte.Month + "." + dte.Year).Append(Environment.NewLine);
                //    sb.Append("Operation: SBARCOGR M/V ").Append(esc.NAVIRE.NomNav).Append(" VOY ").Append(esc.NumVoySCR).Append(" OF ").Append(esc.DRAEsc.Value.Day + "." + esc.DRAEsc.Value.Month + "." + esc.DRAEsc.Value.Year).Append(Environment.NewLine);

                //    sb.Append(Environment.NewLine);
                //    sb.Append(Environment.NewLine);

                //    sb.Append("0004043077").Append(FormatChiffre(esc.DRAEsc.Value.Year).Substring(2, 2) + FormatChiffre(esc.DRAEsc.Value.Month) + FormatChiffre(esc.DRAEsc.Value.Day) + FormatChiffre(esc.DRAEsc.Value.Hour) + FormatChiffre(esc.DRAEsc.Value.Minute)).Append("1915GR").Append(Environment.NewLine);
                //    sb.Append("050077").Append(FormatChiffre(esc.DRAEsc.Value.Year).Substring(2, 2) + FormatChiffre(esc.DRAEsc.Value.Month) + FormatChiffre(esc.DRAEsc.Value.Day)).Append(esc.NAVIRE.CodeTracking).Append(esc.NumVoyDIT).Append("SBARCOGR").Append(Environment.NewLine);
                //    foreach (CONTENEUR ctr in listCtrs.GroupBy(c => c.NumCtr).Select(c => c.First()))
                //    {
                //        sb.Append("051").Append(ctr.NumCtr.PadLeft(11, 'X')).Append("077").Append(ctr.StatutCtr).Append("SBARCOGR").Append("  ").Append(ctr.CONNAISSEMENT.ConsigneeBL.Trim().PadRight(20, ' ').Substring(0, 20)).Append((ctr.FFCtr.Value.Date - esc.DDechEsc.Value.Date).Days.ToString().PadLeft(3, '0')).Append(Environment.NewLine);
                //        i++;
                //    }
                //    sb.Append("9994043077").Append(FormatChiffre(dte.Year).Substring(2, 2) + FormatChiffre(dte.Month) + FormatChiffre(dte.Day) + FormatChiffre(dte.Hour) + FormatChiffre(dte.Minute)).Append("000").Append((listCtrs.GroupBy(c => c.NumCtr).Select(c => c.First()).Count() + 2).ToString().PadLeft(3, '0')).Append(Environment.NewLine);

                //    System.IO.File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\Rapport Débarquement - Escale " + esc.NumEsc + ".txt", sb.ToString(), Encoding.GetEncoding("ISO-8859-1"));

                //    MessageBox.Show("Rapport de débarquement édité avec succès", "Rapport de débarquement édité !", MessageBoxButton.OK, MessageBoxImage.Information);
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

        private void btnRapportEmb_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                //VSOMAccessors vsomAcc = new VSOMAccessors();

                //if (operationsUser.Where(op => op.NomOp == "Report : Rapport embarquement").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
                //{
                //    MessageBox.Show("Vous n'avez pas les droits nécessaires pour éditer le rapport de embarquement. Veuillez contacter un administrateur", "Droits insuffisants !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                //}
                //else
                //{
                //    ESCALE esc = vsp.GetEscaleById(Convert.ToInt32(txtEscaleSysID.Text));

                //    StringBuilder sb = new StringBuilder();
                //    DateTime dte = DateTime.Now;

                //    List<CONTENEUR> listCtrs = vsp.GetConteneursEmbarques(esc.IdEsc); //esc.CONTENEUR.Where<CONTENEUR>(ct => /*ct.SensCtr == "I" &&*/ ct.CONTENEUR_TC.FirstOrDefault<CONTENEUR_TC>().MOUVEMENT_TC.FirstOrDefault<MOUVEMENT_TC>(op => op.IdTypeOp == 283) != null).ToList<CONTENEUR>();
                //    int numMessage = vsomAcc.GetNumMessageGR();

                //    int i = 1;

                //    sb.Append("SOCOMAR").Append(Environment.NewLine);
                //    sb.Append("B.P:12351 Douala Cameroun").Append(Environment.NewLine);
                //    sb.Append("Til:(237) 33424550. Fax:(237) 33424936").Append(Environment.NewLine);
                //    sb.Append("Nr.Message: ").Append(numMessage).Append(" Date ").Append(dte.Day + "." + dte.Month + "." + dte.Year).Append(Environment.NewLine);
                //    sb.Append("Operation: IMBARCOG M/V ").Append(esc.NAVIRE.NomNav).Append(" VOY ").Append(esc.NumVoySCR).Append(" OF ").Append(esc.DRAEsc.Value.Day + "." + esc.DRAEsc.Value.Month + "." + esc.DRAEsc.Value.Year).Append(Environment.NewLine);

                //    sb.Append(Environment.NewLine);
                //    sb.Append(Environment.NewLine);

                //    sb.Append("0004043077").Append(FormatChiffre(esc.DRAEsc.Value.Year).Substring(2, 2) + FormatChiffre(esc.DRAEsc.Value.Month) + FormatChiffre(esc.DRAEsc.Value.Day) + FormatChiffre(esc.DRAEsc.Value.Hour) + FormatChiffre(esc.DRAEsc.Value.Minute)).Append("1915GR").Append(Environment.NewLine);
                //    sb.Append("140077").Append(FormatChiffre(esc.DRAEsc.Value.Year).Substring(2, 2) + FormatChiffre(esc.DRAEsc.Value.Month) + FormatChiffre(esc.DRAEsc.Value.Day)).Append(esc.NAVIRE.CodeTracking).Append(esc.NumVoyDIT).Append("IMBARCOG").Append(Environment.NewLine);
                //    foreach (CONTENEUR ctr in listCtrs)
                //    {
                //        sb.Append("141").Append(ctr.NumCtr.PadLeft(11, 'X')).Append(vsomAcc.GetPortsByCodePort(ctr.CONNAISSEMENT.LPBL).FirstOrDefault<PORT>().CodeRadio).Append(ctr.StatutCtr.Substring(0, 1)).Append("IMBARCOG").Append(Environment.NewLine);
                //        i++;
                //    }
                //    sb.Append("9994043077").Append(FormatChiffre(dte.Year).Substring(2, 2) + FormatChiffre(dte.Month) + FormatChiffre(dte.Day) + FormatChiffre(dte.Hour) + FormatChiffre(dte.Minute)).Append("000").Append((listCtrs.Count + 2).ToString().PadLeft(3, '0')).Append(Environment.NewLine);

                //    System.IO.File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\Rapport Embarquement - Escale " + esc.NumEsc + ".txt", sb.ToString(), Encoding.GetEncoding("ISO-8859-1"));

                //    MessageBox.Show("Rapport de embarquement édité avec succès", "Rapport de embarquement édité !", MessageBoxButton.OK, MessageBoxImage.Information);
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

        private void btnFactureSocomar_Click_1(object sender, RoutedEventArgs e)
        {
            VSOMAccessors vsomAcc = new VSOMAccessors();
            try
            {
                ESCALE esc = vsp.GetEscaleById(int.Parse(txtEscaleSysID.Text));
                if (esc != null)
                {
                    if (esc.SOP == "C")
                    {
                        if (esc.FIntern == "Y")
                        {
                            MessageBox.Show("Vous ne pouvez plus refacturer SOCOMAR pour cette escale", "Echec de l'opération", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        else
                        {
                            if (MessageBox.Show("Cette opération entraine une comptabilisation définitive de la facture SOCOMAR, voulez-vous vraiment effectuer l'opération maintenant ?", "Facturation de SOCOMAR !", MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
                            {
                                vsomAcc.FacturerSocomar(esc.IdEsc, utilisateur.IdU);
                                MessageBox.Show("Facturation SOCOMAR effectuée", "Opération effectuée", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                                formLoader.LoadEscaleForm(this, esc);
                            }
                       }
                    }
                    else
                    {
                        MessageBox.Show("Aucune facturation SOCOMAR n'est possible avant la cloture du SOP", "Annulation de l'opération", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                }
                else
                {
                    MessageBox.Show("Impossible de retrouver l'escale sélectionné", " Echec de l'opération", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "echec de l'opération facturation socomar", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void btnTestAvoir_Click_1(object sender, RoutedEventArgs e)
        {
           
        }
    }
}
