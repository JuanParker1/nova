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
    /// Logique d'interaction pour OrdreServiceForm.xaml
    /// </summary>
    public partial class OrdreServiceForm : Window
    {
        private string typeForm;

        private OrdreServicePanel ordreServicePanel;

        public List<FOURNISSEUR> fournisseurs { get; set; }
        public List<string> fsseurs { get; set; }

        public List<CONNAISSEMENT> connaissements { get; set; }
        public List<string> cons { get; set; }

        public List<VEHICULE> vehicules { get; set; }
        public List<string> vehs { get; set; }

        public List<CONTENEUR> conteneurs { get; set; }
        public List<string> ctrs { get; set; }

        public List<MAFI> mafis { get; set; }
        public List<string> mfs { get; set; }

        public List<CONVENTIONNEL> conventionnels { get; set; }
        public List<string> gcs { get; set; }

        public List<FAMILLE_ARTICLE> famillesArticle { get; set; }
        public List<string> fams { get; set; }

        public List<ARTICLE> articles { get; set; }
        public List<string> arts { get; set; }

        public List<ESCALE> escales { get; set; }
        public List<Int32> escs { get; set; }

        public List<ElementLigneOS> eltsLigneOS { get; set; }

        private UTILISATEUR utilisateur;
        private List<OPERATION> operationsUser;

        private FormLoader formLoader;
        //private VsomParameters vsp = new VsomParameters();
        private VSOMAccessors vsomAcc;
        public OrdreServiceForm(UTILISATEUR user)
        {
            try
            {
                 vsomAcc = new VSOMAccessors();
                //VsomParameters vsparam = new VsomParameters();
                InitializeComponent();
                this.DataContext = this;

                fournisseurs = vsomAcc.GetFournisseursActifs();
                fsseurs = new List<string>();
                foreach (FOURNISSEUR fs in fournisseurs)
                {
                    fsseurs.Add(fs.NomFsseur);
                }

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

        public OrdreServiceForm(string typeForm, OrdreServicePanel panel, UTILISATEUR user)
        {
            try
            {
                 vsomAcc = new VSOMAccessors();
                //VsomParameters vsparam = new VsomParameters();
                InitializeComponent();
                this.DataContext = this;

                fournisseurs = vsomAcc.GetFournisseursActifs();
                fsseurs = new List<string>();
                foreach (FOURNISSEUR fs in fournisseurs)
                {
                    fsseurs.Add(fs.NomFsseur);
                }

                utilisateur = user;
                operationsUser = vsomAcc.GetOperationsUtilisateur(utilisateur.IdU);

                formLoader = new FormLoader(utilisateur);

                this.typeForm = typeForm;

                famillesArticle = vsomAcc.GetFamillesArticles();
                fams = new List<string>();
                foreach (FAMILLE_ARTICLE fam in famillesArticle)
                {
                    fams.Add(fam.LibFamArt);
                }

                cbFamilleArticle.ItemsSource = null;
                cbFamilleArticle.ItemsSource = fams; 
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        public OrdreServiceForm(OrdreServicePanel panel, ORDRE_SERVICE service, UTILISATEUR user)
        {
            try
            {
                 vsomAcc = new VSOMAccessors();
                //VsomParameters vsparam = new VsomParameters();
                InitializeComponent();
                this.DataContext = this;

                fournisseurs = vsomAcc.GetFournisseursActifs();
                fsseurs = new List<string>();
                foreach (FOURNISSEUR fs in fournisseurs)
                {
                    fsseurs.Add(fs.NomFsseur);
                }

                utilisateur = user;
                operationsUser = vsomAcc.GetOperationsUtilisateur(utilisateur.IdU);

                formLoader = new FormLoader(utilisateur);

                formLoader.LoadOrdreServiceForm(this, service);
                 
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
                //VsomMarchal vsm = new VsomMarchal();

                if (operationsUser.Where(op => op.NomOp == "Ordre de service : Enregistrement d'un nouvel élément").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
                {
                    MessageBox.Show("Vous n'avez pas les droits nécessaires pour créer un nouvel ordre de service. Veuillez contacter un administrateur", "Droits insuffisants !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (cbFsseur.SelectedIndex == -1)
                {
                    MessageBox.Show("Vous devez sélectionner un fournisseur pour la création de cet ordre de service", "Fournisseur ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (txtObjetOS.Text == "")
                {
                    MessageBox.Show("Vous devez spécifier un objet pour la création de cet ordre de service", "Objet ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (txtNumVoyage.Text == "")
                {
                    MessageBox.Show("Vous devez indiquer une escale pour la création de cet ordre de service", "Escale ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (!txtDateExecPrevue.SelectedDate.HasValue)
                {
                    MessageBox.Show("Vous devez indiquer une date d'exécution prévisionnelle pour la création de cet ordre de service", "Date d'exécution prévisionnelle ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (dataGridEltOS.Items.OfType<ElementLigneOS>().ToList<ElementLigneOS>().Count == 0)
                {
                    MessageBox.Show("Vous devez renseigner au moins une ligne pour créer cet ordre de service", "Ligne de service ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    if (typeForm == "Nouveau")
                    {
                        if (true)
                        {
                            List<ElementLigneOS> listElements = dataGridEltOS.Items.OfType<ElementLigneOS>().ToList<ElementLigneOS>();

                            ORDRE_SERVICE os = vsomAcc.InsertOrdreService(escales.FirstOrDefault<ESCALE>().IdEsc, fournisseurs.ElementAt<FOURNISSEUR>(cbFsseur.SelectedIndex).IdFsseur, txtIdBL.Text != "" ? Convert.ToInt32(txtIdBL.Text) : -1, txtObjetOS.Text, txtDateExecPrevue.SelectedDate.Value, 
                                txtDateExecReelle.SelectedDate.HasValue ? txtDateExecReelle.SelectedDate.Value : new DateTime(0), 
                                (chkChassis.IsChecked == true && txtIdMarch.Text != "") ? Convert.ToInt32(txtIdMarch.Text) : -1, 
                                (chkConteneur.IsChecked == true && txtIdMarch.Text != "") ? Convert.ToInt32(txtIdMarch.Text) : -1, 
                                (chkMafi.IsChecked == true && txtIdMarch.Text != "") ? Convert.ToInt32(txtIdMarch.Text) : -1, 
                                (chkGC.IsChecked == true && txtIdMarch.Text != "") ? Convert.ToInt32(txtIdMarch.Text) : -1, 
                                listElements, new TextRange(txtObservations.Document.ContentStart, txtObservations.Document.ContentEnd).Text, 
                                utilisateur.IdU);

                            if (ordreServicePanel != null)
                            {
                                if (ordreServicePanel.cbFiltres.SelectedIndex != 1)
                                {
                                    ordreServicePanel.cbFiltres.SelectedIndex = 1;
                                }
                                else
                                {
                                    ordreServicePanel.ordresServices = vsomAcc.GetOrdresServiceEnAttente();
                                    ordreServicePanel.dataGrid.ItemsSource = ordreServicePanel.ordresServices;
                                    ordreServicePanel.lblStatut.Content = ordreServicePanel.ordresServices.Count + " Ordre(s) de service";
                                }
                            }

                            formLoader.LoadOrdreServiceForm(this, os);

                            cbIdOS.IsEnabled = true;
                            borderActions.Visibility = System.Windows.Visibility.Visible;
                            typeForm = "";
                            MessageBox.Show("Ordre de service enregistrée avec succès.", "Enregistrement effectué !", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                    else
                    {
                        List<LIGNE_SERVICE> lignes = new List<LIGNE_SERVICE>();

                        if (eltsLigneOS != null)
                        {
                            foreach (ElementLigneOS elt in eltsLigneOS)
                            {
                                LIGNE_SERVICE lg = new LIGNE_SERVICE();

                                lg.CodeArticle = Convert.ToInt16(elt.Code);
                                //lg.CodeTVA = elt.TVA == 0 ? "TVAEX" : "TVAAP";
                                lg.CodeTVA =  elt.CodeTVA;
                                lg.PULS = elt.PrixUnitaire;
                                lg.QLS = elt.Qte;
                                lg.ULS = elt.Unite;
                                lg.CCArticle = elt.CompteComptable;

                                lignes.Add(lg);
                            }
                        }

                        ORDRE_SERVICE os = vsomAcc.UpdateOrdreService(Convert.ToInt32(cbIdOS.Text), fournisseurs.ElementAt<FOURNISSEUR>(cbFsseur.SelectedIndex).IdFsseur, escales.FirstOrDefault<ESCALE>().IdEsc, txtIdBL.Text != "" ? Convert.ToInt32(txtIdBL.Text) : -1, txtObjetOS.Text, txtDateExecPrevue.SelectedDate.Value, txtDateExecReelle.SelectedDate.HasValue ? txtDateExecReelle.SelectedDate.Value : new DateTime(0), (chkChassis.IsChecked == true && txtIdMarch.Text != "") ? Convert.ToInt32(txtIdMarch.Text) : -1, (chkConteneur.IsChecked == true && txtIdMarch.Text != "") ? Convert.ToInt32(txtIdMarch.Text) : -1, (chkMafi.IsChecked == true && txtIdMarch.Text != "") ? Convert.ToInt32(txtIdMarch.Text) : -1, (chkGC.IsChecked == true && txtIdMarch.Text != "") ? Convert.ToInt32(txtIdMarch.Text) : -1, lignes, new TextRange(txtObservations.Document.ContentStart, txtObservations.Document.ContentEnd).Text, utilisateur.IdU);

                        //Raffraîchir les informations
                        formLoader.LoadOrdreServiceForm(this, os);

                        //cbIdMan.IsEnabled = true;
                        borderActions.Visibility = System.Windows.Visibility.Visible;

                        MessageBox.Show("L'ordre de service " + os.IdOS + " a été mis à jour", "Ordre de service enregistré !", MessageBoxButton.OK, MessageBoxImage.Information);
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

            }
        }

        private void cbEscale_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void cbEscale_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
               // //VSOMAccessors vsomAcc = new VSOMAccessors();

                if (e.Key == Key.Return && cbEscale.Text.Trim() != "")
                {
                    int result;
                    escales = vsomAcc.GetEscalesByNumEscale(Int32.TryParse(cbEscale.Text.Trim(), out result) ? result : -1);

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

        private void cbNumBL_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                ////VSOMAccessors vsomAcc = new VSOMAccessors();

                if (e.Key == Key.Return && cbNumBL.Text.Trim() != "")
                {
                    connaissements = vsomAcc.GetConnaissementByEscaleAndNumBL(Convert.ToInt32(cbEscale.Text), cbNumBL.Text);

                    if (connaissements.Count == 0)
                    {
                        MessageBox.Show("Il n'existe aucun connaissement de l'escale sélectionnée portant ce numéro", "Connaissement introuvable", MessageBoxButton.OK, MessageBoxImage.Exclamation);
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

        private void radioArmateur_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //VsomParameters vsparam = new VsomParameters();

                famillesArticle = vsomAcc.GetFamillesArticlesArmateurs();
                fams = new List<string>();
                foreach (FAMILLE_ARTICLE fam in famillesArticle)
                {
                    fams.Add(fam.LibFamArt);
                }

                cbFamilleArticle.ItemsSource = null;
                cbFamilleArticle.ItemsSource = fams;
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        private void radioClient_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //VsomParameters vsomAcc = new VsomParameters();

                famillesArticle = vsomAcc.GetFamillesArticlesClients();
                fams = new List<string>();
                foreach (FAMILLE_ARTICLE fam in famillesArticle)
                {
                    fams.Add(fam.LibFamArt);
                }

                cbFamilleArticle.ItemsSource = null;
                cbFamilleArticle.ItemsSource = fams;
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        private void radioTous_Click(object sender, RoutedEventArgs e)
        {
            try
            {
               // VsomParameters vsomAcc = new VsomParameters();

                famillesArticle = vsomAcc.GetFamillesArticles();
                fams = new List<string>();
                foreach (FAMILLE_ARTICLE fam in famillesArticle)
                {
                    fams.Add(fam.LibFamArt);
                }

                cbFamilleArticle.ItemsSource = null;
                cbFamilleArticle.ItemsSource = fams;
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        private void cbFamilleArticle_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                //VsomParameters vsomAcc = new VsomParameters();

                if (cbFamilleArticle.Items.Count != 0)
                {
                    txtCodeFamille.Text = famillesArticle.ElementAt<FAMILLE_ARTICLE>(cbFamilleArticle.SelectedIndex).CodeFamArt.ToString();

                    articles = vsomAcc.GetArticleByFamille(Convert.ToInt32(txtCodeFamille.Text));
                    arts = new List<string>();
                    foreach (ARTICLE art in articles)
                    {
                        arts.Add(art.LibArticle);
                    }
                    cbArticle.ItemsSource = null;
                    cbArticle.ItemsSource = arts;
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

        private void cbArticle_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                 vsomAcc = new VSOMAccessors();

                if (cbArticle.Items.Count != 0)
                {
                    ARTICLE art = articles.ElementAt<ARTICLE>(cbArticle.SelectedIndex);
                    txtCodeArticle.Text = art.CodeArticle.ToString();
                    
                    txtCompteComptable.Text = txtIdBL.Text != "" ? (vsomAcc.GetConnaissementByIdBL(Convert.ToInt32(txtIdBL.Text)).CodeTVA == "TVAAP" ? art.CCArticle : art.CCArticleEx) : escales.FirstOrDefault<ESCALE>().ARMATEUR.CCArm;
                    //txtCompteComptable.Text = art.CCArticle == "" ? escales.FirstOrDefault<ESCALE>().ARMATEUR.CCArm : (txtIdBL.Text != "" ? (vsomAcc.GetConnaissementByIdBL(Convert.ToInt32(txtIdBL.Text)).CodeTVA == "TVAAP" ? art.CCArticle : art.CCArticleEx) : escales.FirstOrDefault<ESCALE>().ARMATEUR.CCArm);
                    txtQte.Text = "";
                    txtUnite.Text = articles.ElementAt<ARTICLE>(cbArticle.SelectedIndex).LIGNE_PRIX.FirstOrDefault<LIGNE_PRIX>() != null ? articles.ElementAt<ARTICLE>(cbArticle.SelectedIndex).LIGNE_PRIX.FirstOrDefault<LIGNE_PRIX>().UniteLP : "u";

                    if (art.CodeTVA == "TVAAP" || art.CodeTVA == "TVATI")
                    {

                    }
                    else
                    { 
                    
                    }
                    txtTVA.Text = (art.CodeTVA == "TVAAP" || art.CodeTVA == "TVATI" || art.CodeTVA=="TVADA") ? (txtIdBL.Text != "" ? (vsomAcc.GetConnaissementByIdBL(Convert.ToInt32(txtIdBL.Text)).CodeTVA == "TVAAP" ? "19,25 %" : "0 %") : ((art.CodeTVA == "TVAAP" || art.CodeTVA == "TVATI" || art.CodeTVA=="TVADA") ? "19,25 %" : "0 %")) : "0 %";
                    txtPU.Text = art.LIGNE_PRIX.FirstOrDefault<LIGNE_PRIX>().PU1LP.ToString();
                    txtPT.Text = "0";
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

        private void txtQte_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                //VSOMAccessors vsomAcc = new VSOMAccessors();

                if (cbArticle.SelectedIndex != -1)
                {
                    if (txtTVA.Text == "19,25 %")
                    {
                        string tva = Math.Round((0.1925 * Convert.ToDouble(txtPU.Text.Replace(".", ",")) * Convert.ToDouble(txtQte.Text.Replace(".", ","))), 3).ToString();
                        txtPT.Text = (Math.Round(Convert.ToDouble(txtPU.Text.Replace(".", ",")) * Convert.ToDouble(txtQte.Text.Replace(".", ",")), 3) + Math.Round(Convert.ToDouble(tva), 3)).ToString();
                    }
                    else
                    {
                        txtPT.Text = (Math.Round(Convert.ToDouble(txtPU.Text.Replace(".", ",")) * Convert.ToDouble(txtQte.Text.Replace(".", ",")), 3)).ToString();
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

        private void btnAjoutLS_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int result;
                if (cbArticle.SelectedIndex == -1)
                {
                    MessageBox.Show("Veuillez sélectionner l'article de service que vous voulez ajouter", "Article ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                //else if ((Int32.TryParse(txtCompteComptable.Text.Trim(), out result) ? result : -1) == -1)
                //{
                //    MessageBox.Show("Vous ne pouvez pas créer cet ordre de service. Veuillez mettre à jour le compte comptable de l'armateur", "Compte comptable ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                //}
                else if (txtQte.Text.Trim() == "" || txtQte.Text == "0")
                {
                    MessageBox.Show("Vous devez saisir la quantité", "Quantité ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (txtUnite.Text.Trim() == "")
                {
                    MessageBox.Show("Vous devez saisir l'unité de facturation", "Unité ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (txtPU.Text.Trim() == "" || txtPU.Text == "0")
                {
                    MessageBox.Show("Vous devez saisir le prix unitaire", "Prix unitaire ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    if (txtTVA.Text == "19,25 %")
                    {
                        string tva = Math.Round((0.1925 * Convert.ToDouble(txtPU.Text.Replace(".", ",")) * Convert.ToDouble(txtQte.Text.Replace(".", ","))), 3).ToString();
                        txtPT.Text = (Math.Round(Convert.ToDouble(txtPU.Text.Replace(".", ",")) * Convert.ToDouble(txtQte.Text.Replace(".", ",")), 3) + Math.Round(Convert.ToDouble(tva), 3)).ToString();
                    }
                    else
                    {
                        txtPT.Text = (Math.Round(Convert.ToDouble(txtPU.Text.Replace(".", ",")) * Convert.ToDouble(txtQte.Text.Replace(".", ",")), 3)).ToString();
                    }
                    ElementLigneOS elt = new ElementLigneOS();
                    elt.Code = txtCodeArticle.Text;
                    elt.Libelle = cbArticle.Text;
                    elt.PrixTotal = (float)Convert.ToDouble(txtPT.Text.Replace(".", ","));
                    elt.PrixUnitaire = Convert.ToInt32(txtPU.Text.Replace(".", ","));
                    elt.Qte = (float)Convert.ToDouble(txtQte.Text.Replace(".", ","));
                    elt.TVA = txtTVA.Text == "19,25 %" ? elt.PrixTotal - elt.PrixUnitaire * elt.Qte : 0;
                    elt.CompteComptable = txtCompteComptable.Text;
                    elt.Unite = txtUnite.Text;
                    elt.Remarques = txtRemarques.Text;
                    ARTICLE art = articles.ElementAt<ARTICLE>(cbArticle.SelectedIndex);
                    elt.CodeTVA = txtTVA.Text == "19,25 %"? art.CodeTVA : "TVAEX";

                    if (eltsLigneOS == null)
                    {
                        eltsLigneOS = new List<ElementLigneOS>();
                    }

                    eltsLigneOS.Add(elt);

                    #region ajout de commission 2.5% pour peine et soin : article 1102
                    if (art.CodeArticle == 1102)
                    {
                        VsomParameters vs = new VsomParameters();
                        ARTICLE artcom = vs.GetArticleByCodeArticle(1203).First<ARTICLE>() ;
                        ElementLigneOS eltCommission = new ElementLigneOS();
                        eltCommission.Code = "1203";// txtCodeArticle.Text;
                        eltCommission.Libelle = "Commission debours administratif";
                       
                        eltCommission.PrixUnitaire = Convert.ToInt32(elt.PrixUnitaire * 0.025f);
                        eltCommission.Qte = 1;
                        eltCommission.TVA =txtTVA.Text == "19,25 %" ?  (eltCommission.PrixUnitaire * 0.1925f) : 0;
                         eltCommission.PrixTotal = eltCommission.PrixUnitaire+eltCommission.TVA ;
                         eltCommission.CompteComptable = artcom.CCArticle;
                        eltCommission.Unite = "u";
                        //elt.Remarques = txtRemarques.Text;
                        eltCommission.CodeTVA = txtTVA.Text == "19,25 %" ? artcom.CodeTVA : "TVAEX"; // artcom.CodeTVA;

                        eltsLigneOS.Add(eltCommission);
                    }
                    #endregion

                    dataGridEltOS.ItemsSource = null;
                    dataGridEltOS.ItemsSource = eltsLigneOS;
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

        private void txtQte_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9,.]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void txtPU_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void txtPU_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                //VSOMAccessors vsomAcc = new VSOMAccessors();

                if (cbArticle.SelectedIndex != -1)
                {
                    if (txtTVA.Text == "19,25 %")
                    {
                        string tva = Math.Round((0.1925 * Convert.ToDouble(txtPU.Text.Replace(".", ",")) * Convert.ToDouble(txtQte.Text.Replace(".", ","))), 0, MidpointRounding.AwayFromZero).ToString();
                        txtPT.Text = (Math.Round(Convert.ToDouble(txtPU.Text.Replace(".", ",")) * Convert.ToDouble(txtQte.Text.Replace(".", ",")), 0, MidpointRounding.AwayFromZero) + Math.Round(Convert.ToDouble(tva), 0, MidpointRounding.AwayFromZero)).ToString();
                    }
                    else
                    {
                        txtPT.Text = (Math.Round(Convert.ToDouble(txtPU.Text.Replace(".", ",")) * Convert.ToDouble(txtQte.Text.Replace(".", ",")), 0, MidpointRounding.AwayFromZero)).ToString();
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

        private void txtNumVoyage_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                cbNumBL.IsEnabled = true;
                cbArticle.IsEnabled = true;
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        private void txtCodeArticle_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                btnAjoutLS.IsEnabled = true;
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        private void txtIdBL_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                groupMarchandises.IsEnabled = true;
                if (connaissements != null && connaissements.FirstOrDefault<CONNAISSEMENT>().VEHICULE.Count != 0)
                {
                    chkChassis.IsEnabled = true;
                }
                else
                {
                    chkChassis.IsEnabled = false;
                }

                if (connaissements != null && connaissements.FirstOrDefault<CONNAISSEMENT>().CONTENEUR.Count != 0)
                {
                    chkConteneur.IsEnabled = true;
                }
                else
                {
                    chkConteneur.IsEnabled = false;
                }

                if (connaissements != null && connaissements.FirstOrDefault<CONNAISSEMENT>().MAFI.Count != 0)
                {
                    chkMafi.IsEnabled = true;
                }
                else
                {
                    chkMafi.IsEnabled = false;
                }

                if (connaissements != null && connaissements.FirstOrDefault<CONNAISSEMENT>().CONVENTIONNEL.Count != 0)
                {
                    chkGC.IsEnabled = true;
                }
                else
                {
                    chkGC.IsEnabled = false;
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

        private void chkConteneur_Click(object sender, RoutedEventArgs e)
        {
            try
            {
               // VsomParameters vsomAcc = new VsomParameters();

                conteneurs = vsomAcc.GetConteneursByNumBL(cbNumBL.Text);
                ctrs = new List<string>();
                foreach (CONTENEUR ctr in conteneurs)
                {
                    ctrs.Add(ctr.NumCtr);
                }
                cbMarch.ItemsSource = null;
                cbMarch.ItemsSource = ctrs;
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        private void chkMafi_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //VSOMAccessors vsomAcc = new VSOMAccessors();

                mafis = vsomAcc.GetMafisByNumBL(cbNumBL.Text);
                mfs = new List<string>();
                foreach (MAFI mf in mafis)
                {
                    mfs.Add(mf.NumMafi);
                }
                cbMarch.ItemsSource = null;
                cbMarch.ItemsSource = mfs;
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        private void chkChassis_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //VSOMAccessors vsomAcc = new VSOMAccessors();

                vehicules = vsomAcc.GetVehiculesByNumBL(cbNumBL.Text);
                vehs = new List<string>();
                foreach (VEHICULE veh in vehicules)
                {
                    vehs.Add(veh.NumChassis);
                }
                cbMarch.ItemsSource = null;
                cbMarch.ItemsSource = vehs;
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        private void chkGC_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //VsomParameters vsomAcc = new  VsomParameters();

                conventionnels = vsomAcc.GetConventionnelsByNumBL(cbNumBL.Text);
                gcs = new List<string>();
                foreach (CONVENTIONNEL gc in conventionnels)
                {
                    gcs.Add(gc.NumGC);
                }
                cbMarch.ItemsSource = null;
                cbMarch.ItemsSource = gcs;
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        private void cbMarch_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (cbMarch.Items.Count != 0)
                {
                    if (chkChassis.IsChecked.Value)
                    {
                        VEHICULE veh = vehicules.ElementAt<VEHICULE>(cbMarch.SelectedIndex);
                        txtIdMarch.Text = veh.IdVeh.ToString();
                    }
                    else if (chkConteneur.IsChecked.Value)
                    {
                        CONTENEUR ctr = conteneurs.ElementAt<CONTENEUR>(cbMarch.SelectedIndex);
                        txtIdMarch.Text = ctr.IdCtr.ToString();
                    }
                    else if (chkMafi.IsChecked.Value)
                    {
                        MAFI mf = mafis.ElementAt<MAFI>(cbMarch.SelectedIndex);
                        txtIdMarch.Text = mf.IdMafi.ToString();
                    }
                    else if (chkGC.IsChecked.Value)
                    {
                        CONVENTIONNEL gc = conventionnels.ElementAt<CONVENTIONNEL>(cbMarch.SelectedIndex);
                        txtIdMarch.Text = gc.IdGC.ToString();
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
                if (!txtDateExecPrevue.SelectedDate.HasValue)
                {
                    MessageBox.Show("Veuillez renseigner une date d'exécution prévisionnelle de cet ordre de service", "Date d'exécution prévisionnelle ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (!txtDateExecReelle.SelectedDate.HasValue)
                {
                    MessageBox.Show("Veuillez renseigner une date d'exécution réelle de cet ordre de service", "Date d'exécution réelle ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    ValiderOrdreServiceForm validerForm = new ValiderOrdreServiceForm(this, utilisateur);
                    validerForm.Title = "Validation de l'ordre de service : " + cbIdOS.Text + " - Escale : " + cbEscale.Text;
                    validerForm.ShowDialog();
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
                if (!txtDateCloture.SelectedDate.HasValue)
                {
                    MessageBox.Show("Veuillez renseigner une date de clôture de cet ordre de service", "Date de clôture ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    CloturerOrdreServiceForm clotureForm = new CloturerOrdreServiceForm(this, utilisateur);
                    clotureForm.Title = "Clôture de l'ordre de service : " + cbIdOS.Text + " - Escale : " + cbEscale.Text;
                    clotureForm.ShowDialog();
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

        private void dataGridEltOS_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                //VSOMAccessors vsomAcc = new VSOMAccessors();
                //VsomParameters vsparam = new VsomParameters();

                if (dataGridEltOS.Items.Count != 0)
                {
                    ElementLigneOS elt = (ElementLigneOS)dataGridEltOS.SelectedItem;
                    //lorsquil sagit de la ligne commission fret a collecter et quil existe dans la liste peine et soin, interdire la modif
                    if (elt.Code == "1203")
                    {
                        if (eltsLigneOS.Count(p => p.Code == "1102") > 0)
                        {
                            MessageBox.Show("Impossible de modifier cette ligne. elle est dépendante d'une autre");
                            return;
                        }
                    }

                    ARTICLE art = vsomAcc.GetArticleByCodeArticle(Convert.ToInt32(elt.Code)).FirstOrDefault<ARTICLE>();

                    famillesArticle = new List<FAMILLE_ARTICLE>();
                    famillesArticle.Add(art.FAMILLE_ARTICLE);
                    fams = new List<string>();
                    foreach (FAMILLE_ARTICLE fam in famillesArticle)
                    {
                        fams.Add(fam.LibFamArt);
                    }

                    cbFamilleArticle.ItemsSource = null;
                    cbFamilleArticle.ItemsSource = fams;
                    cbFamilleArticle.SelectedIndex = 0;

                    cbArticle.SelectedItem = elt.Libelle;
                    txtPU.Text = elt.PrixUnitaire.ToString();
                    txtQte.Text = elt.Qte.ToString();
                    txtUnite.Text = elt.Unite;
                    txtTVA.Text = elt.TVA == 0 ? "0 %" : "19,25 %";
                    txtCompteComptable.Text = elt.CompteComptable;
                    txtPT.Text = elt.PrixTotal.ToString();

                    txtRemarques.Text = elt.Remarques;
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

        private void btnModifierLS_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dataGridEltOS.SelectedItems.Count == 0)
                {
                    MessageBox.Show("Veuillez sélectionner une ligne de service à modifier", "Ligne de service ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (cbArticle.SelectedIndex == -1)
                {
                    MessageBox.Show("Veuillez sélectionner l'article de service que vous voulez modifier", "Article ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (txtQte.Text.Trim() == "" || txtQte.Text == "0")
                {
                    MessageBox.Show("Vous devez saisir la quantité", "Quantité ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (txtUnite.Text.Trim() == "")
                {
                    MessageBox.Show("Vous devez saisir l'unité de facturation", "Unité ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (txtPU.Text.Trim() == "" || txtPU.Text == "0")
                {
                    MessageBox.Show("Vous devez saisir le prix unitaire", "Prix unitaire ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    ElementLigneOS elt = (ElementLigneOS)dataGridEltOS.SelectedItem;

                    if (txtTVA.Text == "19,25 %")
                    {
                        string tva = Math.Round((0.1925 * Convert.ToDouble(txtPU.Text.Replace(".", ",")) * Convert.ToDouble(txtQte.Text.Replace(".", ","))), 3).ToString();
                        txtPT.Text = (Math.Round(Convert.ToDouble(txtPU.Text.Replace(".", ",")) * Convert.ToDouble(txtQte.Text.Replace(".", ",")), 3) + Math.Round(Convert.ToDouble(tva), 3)).ToString();
                    }
                    else
                    {
                        txtPT.Text = (Math.Round(Convert.ToDouble(txtPU.Text.Replace(".", ",")) * Convert.ToDouble(txtQte.Text.Replace(".", ",")), 3)).ToString();
                    }
                    
                    elt.Code = txtCodeArticle.Text;
                    elt.Libelle = cbArticle.Text;
                    elt.PrixTotal = (float)Convert.ToDouble(txtPT.Text.Replace(".", ","));
                    elt.PrixUnitaire = Convert.ToInt32(txtPU.Text.Replace(".", ","));
                    elt.Qte = (float)Convert.ToDouble(txtQte.Text.Replace(".", ","));
                    elt.TVA = txtTVA.Text == "19,25 %" ? elt.PrixTotal - elt.PrixUnitaire * elt.Qte : 0;
                    elt.CompteComptable = txtCompteComptable.Text;
                    elt.Unite = txtUnite.Text;
                    elt.Remarques = txtRemarques.Text;
                    ARTICLE art = articles.ElementAt<ARTICLE>(cbArticle.SelectedIndex);
                    elt.CodeTVA = art.CodeTVA;
                     
                    dataGridEltOS.ItemsSource = null;
                    dataGridEltOS.ItemsSource = eltsLigneOS;
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

        private void btnSupprimerLS_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (txtCodeArticle.Text.Trim() != "")
                {
                    eltsLigneOS.RemoveAll(el => el.Code == txtCodeArticle.Text);
                    dataGridEltOS.ItemsSource = null;
                    dataGridEltOS.ItemsSource = eltsLigneOS;
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

        private void cbFsseur_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                txtCodeFsseur.Text = fournisseurs.ElementAt<FOURNISSEUR>(cbFsseur.SelectedIndex).CodeFsseur;
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
                AnnulerOSForm annulerForm = new AnnulerOSForm(this, utilisateur);
                annulerForm.Title = "Annulation de l'ordre de service : " + cbIdOS.Text;
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
