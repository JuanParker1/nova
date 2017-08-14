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
    /// Logique d'interaction pour DemandeReductionForm.xaml
    /// </summary>
    public partial class DemandeReductionForm : Window
    {
        public List<CONNAISSEMENT> connaissements { get; set; }
        public List<string> cons { get; set; }

        public List<CLIENT> clients { get; set; }
        public List<string> clts { get; set; }

        public List<ESCALE> escales { get; set; }
        public List<Int32> escs { get; set; }

        public List<DEMANDE_REDUCTION> demandesReduction { get; set; }
        public List<Int32> reducs { get; set; }

        public List<ElementFacturation> eltsFact { get; set; }

        public List<NAVIRE> navires { get; set; }
        public List<string> navs { get; set; }

        public List<ARTICLE> articles { get; set; }
        public List<string> arts { get; set; }

        public List<VEHICULE> vehicules { get; set; }
        public List<CONTENEUR> conteneurs { get; set; }
        public List<CONVENTIONNEL> conventionnels { get; set; }

        private UTILISATEUR utilisateur;
        private List<OPERATION> operationsUser;

        private string typeForm;

        public DemandeReductionPanel demandeReductionPanel { get; set; }

        private FormLoader formLoader;

       // private VsomParameters vsp = new VsomParameters();
        private VSOMAccessors vsomAcc;
        public DemandeReductionForm(DemandeReductionPanel panel, DEMANDE_REDUCTION reduc, UTILISATEUR user)
        {
            try
            {
                 vsomAcc = new VSOMAccessors();
                //VsomParameters vsparam = new VsomParameters();
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

                demandeReductionPanel = panel;

                utilisateur = user;
                operationsUser = vsomAcc.GetOperationsUtilisateur(utilisateur.IdU);

                formLoader = new FormLoader(utilisateur);

                formLoader.LoadDemandeReductionForm(this, reduc);
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
            
        }

        public DemandeReductionForm(string type, DemandeReductionPanel panel, UTILISATEUR user)
        {
            try
            {
                 vsomAcc = new VSOMAccessors();
                //VsomParameters vsparam = new VsomParameters();
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

                demandeReductionPanel = panel;

                this.typeForm = type;

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
                CONNAISSEMENT bl = vsomAcc.GetConnaissementByIdBL(Convert.ToInt32(txtIdBL.Text));

                if (cbArticle.SelectedIndex == -1)
                {
                    MessageBox.Show("Veuillez sélectionner un article pour lequel sera appliqué la réduction", "Article ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (txtLibelle.Text.Trim() == "")
                {
                    MessageBox.Show("Vous devez saisir le libellé de cette ligne de réduction", "Libellé ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (txtPourcent.Text.Trim() == "")
                {
                    MessageBox.Show("Vous ne pouvez pas enregistrer de réduction avec un pourcentage nul", "Pourcentage de réduction ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (txtMHT.Text.Trim() == "0")
                {
                    MessageBox.Show("Vous ne pouvez pas enregistrer de réduction avec un montant nul", "Montant réduction ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    if (typeForm == "Nouveau")
                    {
                        if (operationsUser.Where(op => op.NomOp == "Demande de réduction : Enregistrement d'un nouvel élément").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
                        {
                            MessageBox.Show("Vous n'avez pas les droits nécessaires pour créer une nouvelle demande de réduction. Veuillez contacter un administrateur", "Droits insuffisants !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        }
                        else
                        {
                            //ah ajout pour control du type de tva de l'article

                            /* 
                             * DEMANDE_REDUCTION ddeReduc = vsomAcc.InsertDemandeReduction(Convert.ToInt32(txtIdBL.Text), Convert.ToInt16(txtCodeArticle.Text), txtLibelle.Text, Convert.ToDouble(txtPourcent.Text.Replace(".", ",")), Convert.ToInt32(txtMHT.Text), 
                                (articles.ElementAt<ARTICLE>(cbArticle.SelectedIndex).CODE_TVA.CodeTVA == "TVAAP" || articles.ElementAt<ARTICLE>(cbArticle.SelectedIndex).CODE_TVA.CodeTVA == "TVATI") ? ((bl.BLIL == "Y" || bl.BLGN == "Y") ? 0 : 19.25f) : 0, 
                                new TextRange(txtObservations.Document.ContentStart, txtObservations.Document.ContentEnd).Text, utilisateur.IdU);
                             */
                            DEMANDE_REDUCTION ddeReduc = vsomAcc.InsertDemandeReduction(Convert.ToInt32(txtIdBL.Text), Convert.ToInt16(txtCodeArticle.Text), txtLibelle.Text, Convert.ToDouble(txtPourcent.Text.Replace(".", ",")), Convert.ToInt32(txtMHT.Text),
                                (articles.ElementAt<ARTICLE>(cbArticle.SelectedIndex).CODE_TVA.TauxTVA.ToString() != "0") ? ((bl.BLIL == "Y" || bl.BLGN == "Y") ? 0 : 19.25f) : 0,
                                new TextRange(txtObservations.Document.ContentStart, txtObservations.Document.ContentEnd).Text, utilisateur.IdU);
                            if (demandeReductionPanel.cbFiltres.SelectedIndex != 2)
                            {
                                demandeReductionPanel.cbFiltres.SelectedIndex = 2;
                            }
                            else
                            {
                                demandeReductionPanel.reductions = vsomAcc.GetDemandesReductionEnAttente();
                                demandeReductionPanel.dataGrid.ItemsSource = demandeReductionPanel.reductions;
                                demandeReductionPanel.lblStatut.Content = demandeReductionPanel.reductions.Count + " Demande(s) de réduction";
                            }

                            formLoader.LoadDemandeReductionForm(this, ddeReduc);

                            borderActions.Visibility = System.Windows.Visibility.Visible;
                            btnValider.Visibility = System.Windows.Visibility.Visible;

                            MessageBox.Show("Enregistrement effectué avec succès.", "Enregistrement effectué !", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                    else
                    {
                        if (operationsUser.Where(op => op.NomOp == "Demande de réduction : Enregistrement d'un nouvel élément").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
                        {
                            MessageBox.Show("Vous n'avez pas les droits nécessaires pour créer une nouvelle demande de réduction. Veuillez contacter un administrateur", "Droits insuffisants !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        }
                        else
                        {
                            DEMANDE_REDUCTION ddeReduc = vsomAcc.UpdateDemandeReduction(Convert.ToInt32(cbIdRed.Text), Convert.ToInt32(txtIdBL.Text), 
                                Convert.ToInt16(txtCodeArticle.Text), txtLibelle.Text, Convert.ToDouble(txtPourcent.Text.Replace(".", ",")), 
                                Convert.ToInt32(txtMHT.Text), 
                                (articles.ElementAt<ARTICLE>(cbArticle.SelectedIndex).CODE_TVA.CodeTVA == "TVAAP" || articles.ElementAt<ARTICLE>(cbArticle.SelectedIndex).CODE_TVA.CodeTVA == "TVATI") ? ((bl.BLIL == "Y" || bl.BLGN == "Y") ? 0 : 19.25f) : 0, 
                                new TextRange(txtObservations.Document.ContentStart, txtObservations.Document.ContentEnd).Text, utilisateur.IdU);

                            if (demandeReductionPanel.cbFiltres.SelectedIndex != 2)
                            {
                                demandeReductionPanel.cbFiltres.SelectedIndex = 2;
                            }
                            else
                            {
                                demandeReductionPanel.reductions = vsomAcc.GetDemandesReductionEnAttente();
                                demandeReductionPanel.dataGrid.ItemsSource = demandeReductionPanel.reductions;
                                demandeReductionPanel.lblStatut.Content = demandeReductionPanel.reductions.Count + " Demande(s) de réduction";
                            }

                            formLoader.LoadDemandeReductionForm(this, ddeReduc);

                            borderActions.Visibility = System.Windows.Visibility.Visible;
                            btnValider.Visibility = System.Windows.Visibility.Visible;

                            MessageBox.Show("Mise à jour effectuée avec succès.", "Mise à jour effectuée !", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
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

        private void btnValider_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (operationsUser.Where(op => op.NomOp == "Demande de réduction : Validation d'un élément").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
                {
                    MessageBox.Show("Vous n'avez pas les droits nécessaires pour valider une demande de réduction. Veuillez contacter un administrateur", "Droits insuffisants !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    ValiderReductionForm validForm = new ValiderReductionForm(this, utilisateur);
                    validForm.Title = "Validation réduction : " + cbIdRed.Text + " - Consignée " + txtConsignee.Text;
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
                    connaissements = vsomAcc.GetConnaissementsFacturablesByNumBL(cbNumBL.Text);

                    if (connaissements.Count == 0)
                    {
                        MessageBox.Show("Il n'existe aucun connaissement accompli portant ce numéro", "Connaissement introuvable", MessageBoxButton.OK, MessageBoxImage.Exclamation);
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

        private void txtPourcent_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                 vsomAcc = new VSOMAccessors();

                double pourcentage = Convert.ToDouble(txtPourcent.Text.Replace(".",","));
                CONNAISSEMENT con = vsomAcc.GetConnaissementByIdBL(Convert.ToInt32(txtIdBL.Text));
                if (cbArticle.SelectedIndex != -1 && pourcentage > 0 && pourcentage <= 100)
                {
                    txtMHT.Text = Math.Round(con.ELEMENT_FACTURATION.Where(el => el.LIGNE_PRIX.CodeArticle.Value == articles.ElementAt<ARTICLE>(cbArticle.SelectedIndex).CodeArticle && (el.StatutEF == "En cours" || el.StatutEF == "Proforma")).Sum(el => el.PUEFBase.Value * el.QTEEF.Value * pourcentage / 100)).ToString();
                    txtMTVA.Text = Math.Round((articles.ElementAt<ARTICLE>(cbArticle.SelectedIndex).CODE_TVA.TauxTVA.Value / 100 * Convert.ToDouble(txtMHT.Text)), 3).ToString();
                    txtMTTC.Text = (Math.Round(Convert.ToDouble(txtMHT.Text), 3) + Math.Round(Convert.ToDouble(txtMTVA.Text), 3)).ToString();

                    txtMTTCAvant.Text = ((Convert.ToDouble(txtMTTC.Text) * 100) / pourcentage).ToString();
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

        private void txtPourcent_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9,.]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void cbArticle_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (cbArticle.SelectedIndex != -1)
                {
                    txtCodeArticle.Text = articles.ElementAt<ARTICLE>(cbArticle.SelectedIndex).CodeArticle.ToString();
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
                    noteForm.Title = "Note - " + note.IdNote + " - Réduction - " + note.DEMANDE_REDUCTION.IdDDR;
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

        private void btnAjoutNote_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                NoteForm noteForm = new NoteForm("Nouveau", this, utilisateur);
                noteForm.Title = "Création de note - Réduction - " + cbIdRed.Text;
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

        private void btnAnnuler_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AnnulerReductionForm annulerForm = new AnnulerReductionForm(this, utilisateur);
                annulerForm.Title = "Annulation de la demande de réduction : " + cbIdRed.Text;
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
    }
}
