using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Logique d'interaction pour FactureSpot.xaml
    /// </summary>
    public partial class FactureSpot : Window
    {
        public List<FAMILLE_ARTICLE> famillesArticle { get; set; }
        public List<string> fams { get; set; }

        public List<ARTICLE> articles { get; set; }
        public List<string> arts { get; set; }

        public List<CLIENT> _client { get; set; }

        public List<ElementLigneFactureSpot> eltsLigneOS { get; set; }

        private UTILISATEUR utilisateur;
        private List<OPERATION> operationsUser;

        private FormLoader formLoader;
        private FACTURE _facture;
        //private VsomParameters vsp = new VsomParameters();
        private VSOMAccessors vsomAcc;
        public FactureSpot(FactureSpotPanel panel , UTILISATEUR user)
        {
            try
            {
                 vsomAcc = new VSOMAccessors();
                //VsomParameters vsprm = new VsomParameters();
                InitializeComponent();

                famillesArticle = vsomAcc.GetFamillesArticles();
                fams = new List<string>();
                /*foreach (FAMILLE_ARTICLE fam in famillesArticle)
                {
                    fams.Add(fam.LibFamArt);
                }*/

                cbFamilleArticle.ItemsSource = null;
                cbFamilleArticle.ItemsSource = famillesArticle;
                cbFamilleArticle.DisplayMemberPath = "LibFamArt";

                _client = vsomAcc.GetClientsActifs();
                cbClient.ItemsSource = null; cbClient.ItemsSource = _client;
                cbClient.DisplayMemberPath = "NomClient";
                utilisateur = user;
                operationsUser = vsomAcc.GetOperationsUtilisateur(utilisateur.IdU);

                //btnAnnuler.Visibility = System.Windows.Visibility.Collapsed;
                borderActions.Visibility = System.Windows.Visibility.Collapsed;
                //statutCpteFact.Content = "En Cours";
                if (operationsUser.Where(op => op.NomOp == "Field : Autorisation sur date").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
                {
                    txtDateCreation.IsEnabled = false;
                }
                else
                {
                    txtDateCreation.IsEnabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Echec du chargement de la fenêtre !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public FactureSpot(FactureSpotPanel panel, FACTURE facture, UTILISATEUR user)
        {
            try
            {
                 vsomAcc = new VSOMAccessors();
                //VsomParameters vsprm = new VsomParameters();
                InitializeComponent();

                famillesArticle = vsomAcc.GetFamillesArticlesClients();
                fams = new List<string>();
                /*foreach (FAMILLE_ARTICLE fam in famillesArticle)
                {
                    fams.Add(fam.LibFamArt);
                }*/

                cbFamilleArticle.ItemsSource = null;
                cbFamilleArticle.ItemsSource = famillesArticle;
                cbFamilleArticle.DisplayMemberPath = "LibFamArt";

                _client = vsomAcc.GetClientsActifs();
                cbClient.ItemsSource = null; cbClient.ItemsSource = _client;
                cbClient.DisplayMemberPath = "NomClient";
                utilisateur = user;
                operationsUser = vsomAcc.GetOperationsUtilisateur(utilisateur.IdU);

                //recharg la facture avant affichage
                formLoader = new FormLoader(utilisateur);
                 _facture = vsomAcc.GetFactureSpotByIdDocSAP((int)facture.IdDocSAP);
                 formLoader.LoadFactureSpotForm(this, _facture);

               
            }
            catch (Exception ex)
            {
                MessageBox.Show("Echec du chargement de la fenêtre. \n " + ex.Message, "Facture Spot : detail");
                this.Close();
            }
        }

        private void btnEnregistrer_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                 vsomAcc = new VSOMAccessors();

                if (operationsUser.Where(op => op.NomOp == "Facture spot: Enregistrement d'un nouvel élément").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
                {
                    MessageBox.Show("Vous n'avez pas les droits nécessaires pour effectuer cette action. Veuillez contacter un administrateur", "Droits insuffisants !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (cbClient.SelectedIndex == -1)
                {
                    MessageBox.Show("Vous devez sélectionner le client", "Client ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (dataGridEltOS.Items.OfType<ElementLigneFactureSpot>().ToList<ElementLigneFactureSpot>().Count == 0)
                {
                    MessageBox.Show("Vous devez renseigner au moins une ligne de facturation", "Ligne de Facturation ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    List<ElementLigneFactureSpot> listElements = dataGridEltOS.Items.OfType<ElementLigneFactureSpot>().ToList<ElementLigneFactureSpot>();

                    FACTURE fact = vsomAcc.InsertFactureSpot((CLIENT)cbClient.SelectedItem, listElements,txtDateCreation.SelectedDate.Value, txtDateDue.SelectedDate.Value,
                            new TextRange(txtObservations.Document.ContentStart, txtObservations.Document.ContentEnd).Text, utilisateur);
                    txtIdFact.Text = fact.IdDocSAP.ToString();
                    btnEnregistrer.IsEnabled = false;
                    btnAjoutLS.IsEnabled = btnModifierLS.IsEnabled =btnSupprimerLS.IsEnabled= false;
                    MessageBox.Show("Opération effectuée avec succès", "Facturation Spot");

                    borderActions.Visibility = System.Windows.Visibility.Visible;
                    
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
                MessageBox.Show("Echec de la transation : \n"+ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnValider_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void btnAnnuler_Click_1(object sender, RoutedEventArgs e)
        {
            if (operationsUser.Where(op => op.NomOp == "Facture spot: Annulation").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
            {
                MessageBox.Show("Vous n'avez pas les droits nécessaires pour effectuer cette action. Veuillez contacter un administrateur", "Droits insuffisants !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            else
            {
                AnnulerFactureSpot frm = new AnnulerFactureSpot(this, _facture, utilisateur);
                frm.ShowDialog();
            }
        }

        private void cbFamilleArticle_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                //VSOMAccessors vsomAcc = new VSOMAccessors();
               // VsomParameters vsprm = new VsomParameters();
                if (cbFamilleArticle.Items.Count != 0)
                {
                    FAMILLE_ARTICLE fa = (FAMILLE_ARTICLE) cbFamilleArticle.SelectedItem;
                    txtCodeFamille.Text = fa.CodeFamArt.ToString();

                    articles = vsomAcc.GetArticleByFamille(fa.CodeFamArt);
                    arts = new List<string>();
                    /*foreach (ARTICLE art in articles)
                    {
                        arts.Add(art.LibArticle);
                    }*/
                    cbArticle.ItemsSource = null;
                    cbArticle.ItemsSource = articles; cbArticle.DisplayMemberPath = "LibArticle";
                    txtCodeArticle.Text = ""; txtQte.Text = ""; txtUnite.Text = "";
                    txtTVA.Text = ""; txtPU.Text = ""; txtPT.Text = "0";
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

        private void cbArticle_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                //VSOMAccessors vsomAcc = new VSOMAccessors();

                if (cbArticle.Items.Count != 0)
                {
                    ARTICLE art = (ARTICLE)cbArticle.SelectedItem;
                    txtCodeArticle.Text = art.CodeArticle.ToString();
                    txtlibArticle.Text = art.LibArticle;
                    txtQte.Text = "";
                    txtUnite.Text = art.LIGNE_PRIX.FirstOrDefault<LIGNE_PRIX>() != null ? art.LIGNE_PRIX.FirstOrDefault<LIGNE_PRIX>().UniteLP : "u";
                    txtTVA.SelectedIndex = (art.CodeTVA == "TVAAP" || art.CodeTVA == "TVATI" || art.CodeTVA=="TVADA") ? 1 : 0;
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

        private void txtCodeArticle_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            btnAjoutLS.IsEnabled = true;
        }

        private void txtQte_PreviewTextInput_1(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9,.]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void txtPU_LostFocus_1(object sender, RoutedEventArgs e)
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

        private void cbClient_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            if (cbClient.SelectedIndex != -1)
            {
                CLIENT c = (CLIENT)cbClient.SelectedItem;
                txtCodeClient.Text = c.CodeClient;
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

        private void txtPU_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        
        private void btnAjoutLS_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                int result;
                if (cbArticle.SelectedIndex == -1)
                {
                    MessageBox.Show("Veuillez sélectionner l'article de service que vous voulez ajouter", "Article ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
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
                    if (eltsLigneOS == null)
                     {
                            eltsLigneOS = new List<ElementLigneFactureSpot>();
                     }
                    //controle de doublons leve pour autorise la facturation du meme article plusieres fois
                    int nbr = 0;// (from m in eltsLigneOS where m.Code == txtCodeArticle.Text select m).Count();
                    if (nbr !=0 )
                    {
                        MessageBox.Show("Cet Article est déja ajouté", "Facture Spot");
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
                        ARTICLE art = (ARTICLE)cbArticle.SelectedItem;

                        ElementLigneFactureSpot elt = new ElementLigneFactureSpot();
                        elt.Code = txtCodeArticle.Text;
                        elt.Libelle = txtlibArticle.Text; //cbArticle.Text;
                        elt.PrixTotal = (float)Convert.ToDouble(txtPT.Text.Replace(".", ","));
                        elt.PrixUnitaire = Convert.ToInt32(txtPU.Text.Replace(".", ","));
                        elt.Qte = (float)Convert.ToDouble(txtQte.Text.Replace(".", ","));
                        elt.TVA = txtTVA.Text == "19,25 %" ? (elt.PrixTotal - (elt.PrixUnitaire * elt.Qte)) : 0;
                        elt.CompteComptable = txtTVA.Text == "19,25 %" ? art.CCArticle : art.CCArticleEx; //txtCompteComptable.Text;
                        elt.Unite = txtUnite.Text;
                        //recupère le codetva de l'article
                       
                        if (txtTVA.Text == "19,25 %")
                        {
                            /*if (art.CodeTVA=="TVAEX") 
                            {
                                throw new Exception(" Une Incohérence est détectée: selection codetva"); 
                            }
                            else
                            {
                                elt.CodeTVA = art.CodeTVA;
                                
                            }*/
                            elt.CodeTVA = art.CodeTVA;
                        }
                        else
                            elt.CodeTVA = "TVAEX";

                        //elt.CodeTVA = txtTVA.Text == "19,25 %" ? "TVAAP" : "TVAEX";
                        //elt.Remarques = txtRemarques.Text;
                        
                        elt.Articl = art;
                        FAMILLE_ARTICLE fa = (FAMILLE_ARTICLE)cbFamilleArticle.SelectedItem;
                        elt.FamilleArticl = fa;

                        

                        eltsLigneOS.Add(elt);

                        dataGridEltOS.ItemsSource = null;
                        dataGridEltOS.ItemsSource = eltsLigneOS;

                        txtCodeArticle.Text = ""; txtQte.Text = ""; txtUnite.Text =txtlibArticle.Text= "";
                        txtTVA.SelectedIndex = -1; txtPU.Text = ""; txtPT.Text = "0"; cbArticle.SelectedIndex = -1;
                        //txtRemarques.Text = "";
                        //resume facture 
                        /* float */ double _ht = Math.Abs(eltsLigneOS.Sum(r => r.PrixUnitaire * r.Qte));
                        double _tva = Math.Abs(eltsLigneOS.Sum(r => r.TVA));
                        montantHTCpteFact.Content = Convert.ToInt32(_ht);
                        montantTVACpteFact.Content = Convert.ToInt32(_tva);
                        montantTTCCpteFact.Content =(Convert.ToInt32(_ht) + Convert.ToInt32(_tva));
                    }
                }
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

        private void dataGridEltOS_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                //VSOMAccessors vsomAcc = new VSOMAccessors();

                if (dataGridEltOS.Items.Count != 0)
                {
                    ElementLigneFactureSpot elt = (ElementLigneFactureSpot)dataGridEltOS.SelectedItem;

                    ARTICLE art = elt.Articl;
                        //vsomAcc.GetArticleByCodeArticle(Convert.ToInt32(elt.Code)).FirstOrDefault<ARTICLE>();

                    //famillesArticle = new List<FAMILLE_ARTICLE>();
                    //famillesArticle.Add(art.FAMILLE_ARTICLE);
                    /*fams = new List<string>();
                    foreach (FAMILLE_ARTICLE fam in famillesArticle)
                    {
                        fams.Add(fam.LibFamArt);
                    }
                    */
                    //cbFamilleArticle.ItemsSource = null;
                    //cbFamilleArticle.ItemsSource = famillesArticle;
                    cbFamilleArticle.SelectedItem = elt.FamilleArticl;

                    int i = articles.FindIndex(r=>r.CodeArticle==elt.Articl.CodeArticle);
                    cbArticle.SelectedIndex = i;
                    //cbArticle.SelectedItem = elt.Articl; 

                    txtPU.Text = elt.PrixUnitaire.ToString();
                    txtQte.Text = elt.Qte.ToString();
                    txtUnite.Text = elt.Unite;
                    txtTVA.Text = elt.TVA == 0 ? "0 %" : "19,25 %";
                    txtCompteComptable.Text = elt.CompteComptable;
                    txtPT.Text = elt.PrixTotal.ToString(); 
                    //txtRemarques.Text = elt.Remarques;
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

        private void btnModifierLS_Click_1(object sender, RoutedEventArgs e)
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
                    ElementLigneFactureSpot elt = (ElementLigneFactureSpot)dataGridEltOS.SelectedItem;
                    ARTICLE art = (ARTICLE)cbArticle.SelectedItem;
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
                    elt.Libelle = txtlibArticle.Text; 
                    elt.PrixTotal = (float)Convert.ToDouble(txtPT.Text.Replace(".", ","));
                    elt.PrixUnitaire = Convert.ToInt32(txtPU.Text.Replace(".", ","));
                    elt.Qte = (float)Convert.ToDouble(txtQte.Text.Replace(".", ","));
                    elt.TVA = txtTVA.Text == "19,25 %" ? (elt.PrixTotal - (elt.PrixUnitaire * elt.Qte)) : 0;
                   // elt.CompteComptable = txtCompteComptable.Text;
                    elt.CompteComptable = txtTVA.Text == "19,25 %" ? art.CCArticle : art.CCArticleEx; //txtCompteComptable.Text;
                    elt.Unite = txtUnite.Text;
                   
                    if (txtTVA.Text == "19,25 %")
                    {
                        if (art.CodeTVA=="TVAEX") //applique le code tva en fonction du code de part de article
                        {
                            throw new Exception(" Une Incohérence est détectée: selection codetva");
                        }
                        else
                        {
                            elt.CodeTVA = art.CodeTVA;
                            
                        }
                    }
                    else
                        elt.CodeTVA = "TVAEX";

                   // elt.CodeTVA = txtTVA.Text == "19,25 %" ? "TVAAP" : "TVAEX";
                   // elt.Remarques = txtRemarques.Text;
                   
                    elt.Articl = art;
                    FAMILLE_ARTICLE fa = (FAMILLE_ARTICLE)cbFamilleArticle.SelectedItem;
                    elt.FamilleArticl = fa;

                    dataGridEltOS.ItemsSource = null;
                    dataGridEltOS.ItemsSource = eltsLigneOS;
                    //resume facture 

                    montantHTCpteFact.Content = Math.Abs(eltsLigneOS.Sum(r => r.PrixUnitaire * r.Qte)).ToString();
                    montantTVACpteFact.Content = (Math.Abs(eltsLigneOS.Sum(r => r.TVA)).ToString());
                    montantTTCCpteFact.Content = Int32.Parse(Math.Abs(eltsLigneOS.Sum(r => r.PrixUnitaire * r.Qte)).ToString()) + Int32.Parse((Math.Abs(eltsLigneOS.Sum(r => r.TVA)).ToString()));

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

        private void btnSupprimerLS_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                int index = dataGridEltOS.SelectedIndex ;
                ElementLigneFactureSpot elos =eltsLigneOS[index];
                eltsLigneOS.RemoveAt(index);
                dataGridEltOS.ItemsSource = null;
                dataGridEltOS.ItemsSource = eltsLigneOS;

               /* if (txtCodeArticle.Text.Trim() != "")
                {
                    eltsLigneOS.RemoveAll(el => el.Code == txtCodeArticle.Text);
                    dataGridEltOS.ItemsSource = null;
                    dataGridEltOS.ItemsSource = eltsLigneOS;
                }*/
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        private void txtTVA_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
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

        private void btnImprimer_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                FactureReport factReport = new FactureReport(this);
                factReport.Title = "Impression de la facture spot : " + txtIdFact ;
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

    }
}
