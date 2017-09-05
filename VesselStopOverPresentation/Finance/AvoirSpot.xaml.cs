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

namespace VesselStopOverPresentation.Finance
{
    /// <summary>
    /// Logique d'interaction pour AvoirSpot.xaml
    /// </summary>
    public partial class AvoirSpot : Window
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
        private VSOMAccessors vsomAcc;

        public AvoirSpot(UTILISATEUR user)
        {
            InitializeComponent();
            vsomAcc = new VSOMAccessors();
            famillesArticle = vsomAcc.GetFamillesArticles();
            fams = new List<string>();
            cbFamilleArticle.ItemsSource = null;
            cbFamilleArticle.ItemsSource = famillesArticle;
            cbFamilleArticle.DisplayMemberPath = "LibFamArt";

            _client = vsomAcc.GetClientsActifs();
            cbClient.ItemsSource = null; cbClient.ItemsSource = _client;
            cbClient.DisplayMemberPath = "NomClient";
            utilisateur = user;
            borderActions.Visibility = System.Windows.Visibility.Collapsed;
            operationsUser = vsomAcc.GetOperationsUtilisateur(utilisateur.IdU);

        }

        private void cbClient_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            if (cbClient.SelectedIndex != -1)
            {
                CLIENT c = (CLIENT)cbClient.SelectedItem;
                txtCodeClient.Text = c.CodeClient;
            }
        }
        public void SetCompulsoryElement(List<ElementFacturation> lst)
        {
            if (eltsLigneOS == null)
            {
                eltsLigneOS = new List<ElementLigneFactureSpot>();
            }
            foreach (ElementFacturation ef in lst)
            {
                ElementLigneFactureSpot elt = new ElementLigneFactureSpot();
                elt.Code = ef.CodeArticle.ToString();
                elt.Libelle = ef.LibArticle;
                elt.PrixTotal = -1 * ef.MontantTTC; //-1* ef.PrixUnitaire;
                elt.PrixUnitaire =  (int)ef.PrixUnitaire;
                elt.Qte = -1;// ef.Qte;
                elt.TVA = ef.MontantTVA;
                // elt.CompteComptable = txtTVA.Text == "19,25 %" ? art.CCArticle : art.CCArticleEx; //txtCompteComptable.Text;
                elt.CompteComptable = ef.Compte;
                elt.Unite = "u"; 
                elt.CodeTVA =ef.MontantTVA==0 ? "TVAEX" : "TVAAP";
                elt.EltCompulsory = ef;
                      
                eltsLigneOS.Add(elt);

                dataGridEltOS.ItemsSource = null;
                dataGridEltOS.ItemsSource = eltsLigneOS;

                txtCodeArticle.Text = ""; txtQte.Text = ""; txtUnite.Text = "";
                txtTVA.SelectedIndex = -1; txtPU.Text = ""; txtPT.Text = "0"; cbArticle.SelectedIndex = -1;

                
            }
            double _ht = Math.Abs(eltsLigneOS.Sum(r => r.PrixUnitaire * r.Qte));
            double _tva = Math.Abs(eltsLigneOS.Sum(r => r.TVA));
            montantHTCpteFact.Content = Convert.ToInt32(_ht);
            montantTVACpteFact.Content = Convert.ToInt32(_tva);
            montantTTCCpteFact.Content = (Convert.ToInt32(_ht) + Convert.ToInt32(_tva));
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
                    if (nbr != 0)
                    {
                        MessageBox.Show("Cet Article est déja ajouté", "Avoir Spot");
                    }
                    else
                    {
                        if (txtTVA.Text == "19,25 %")
                        {
                           /* string tva = Math.Round((0.1925 * Convert.ToDouble(txtPU.Text.Replace(".", ",")) * Convert.ToDouble(txtQte.Text.Replace(".", ","))), 3).ToString();
                            txtPT.Text = (Math.Round(Convert.ToDouble(txtPU.Text.Replace(".", ",")) * Convert.ToDouble(txtQte.Text.Replace(".", ",")), 3) + Math.Round(Convert.ToDouble(tva), 3)).ToString();
                            */
                            string tva = (0.1925 * Convert.ToDouble(txtPU.Text.Replace(".", ",")) * Convert.ToDouble(txtQte.Text.Replace(".", ","))).ToString();
                            txtPT.Text = (Math.Round(Convert.ToDouble(txtPU.Text.Replace(".", ",")) * Convert.ToDouble(txtQte.Text.Replace(".", ",")), 3) + Convert.ToDouble(tva)).ToString();
                        
                        }
                        else
                        {
                           // txtPT.Text = (Math.Round(Convert.ToDouble(txtPU.Text.Replace(".", ",")) * Convert.ToDouble(txtQte.Text.Replace(".", ",")), 3)).ToString();
                            txtPT.Text = (Convert.ToDouble(txtPU.Text.Replace(".", ",")) * Convert.ToDouble(txtQte.Text.Replace(".", ","))).ToString();

                        }
                        ARTICLE art = (ARTICLE)cbArticle.SelectedItem;

                        ElementLigneFactureSpot elt = new ElementLigneFactureSpot();
                        elt.Code = txtCodeArticle.Text;
                        elt.Libelle = txtlibArticle.Text; //cbArticle.Text;
                        elt.PrixTotal = Convert.ToDouble(txtPT.Text.Replace(".", ",")) ; // Math.Round(Convert.ToDouble(txtPT.Text.Replace(".", ",")),3);
                        elt.PrixUnitaire = Convert.ToDouble(txtPU.Text.Replace(".", ","));
                        elt.Qte = (float)Convert.ToDouble(txtQte.Text.Replace(".", ","));
                        elt.TVA = txtTVA.Text == "19,25 %" ? (elt.PrixTotal - (elt.PrixUnitaire * elt.Qte)) : 0;
                       // elt.CompteComptable = txtTVA.Text == "19,25 %" ? art.CCArticle : art.CCArticleEx; //txtCompteComptable.Text;
                        elt.CompteComptable = art.CCArticle;
                        elt.Unite = txtUnite.Text;
                        //recupère le codetva de l'article

                        if (txtTVA.Text == "19,25 %")
                        {
                            //if (art.CodeTVA == "TVAEX")
                            //{
                            //    throw new Exception(" Une Incohérence est détectée: selection codetva");
                            //}
                            //else
                            //{
                            //    elt.CodeTVA = art.CodeTVA;

                            //}
                            elt.CodeTVA = art.CodeTVA;
                        }
                        else
                            elt.CodeTVA = "TVAEX";

                         
                        elt.Articl = art;
                        FAMILLE_ARTICLE fa = (FAMILLE_ARTICLE)cbFamilleArticle.SelectedItem;
                        elt.FamilleArticl = fa;
                         
                        eltsLigneOS.Add(elt);

                        dataGridEltOS.ItemsSource = null;
                        dataGridEltOS.ItemsSource = eltsLigneOS;

                        txtCodeArticle.Text = ""; txtQte.Text = ""; txtUnite.Text = "";
                        txtTVA.SelectedIndex = -1; txtPU.Text = ""; txtPT.Text = "0"; cbArticle.SelectedIndex = -1;
                         
                        double _ht = Math.Abs(eltsLigneOS.Sum(r => r.PrixUnitaire * r.Qte));
                        double _tva = Math.Abs(eltsLigneOS.Sum(r => r.TVA));
                        montantHTCpteFact.Content = _ht; //Convert.ToInt32(_ht);
                        montantTVACpteFact.Content = _tva; //Convert.ToInt32(_tva);
                        montantTTCCpteFact.Content = _ht + _tva; //(Convert.ToInt32(_ht) + Convert.ToInt32(_tva));
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

        private void btnSupprimerLS_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                int index = dataGridEltOS.SelectedIndex;
                ElementLigneFactureSpot elos = eltsLigneOS[index];
                eltsLigneOS.RemoveAt(index);
                dataGridEltOS.ItemsSource = null;
                dataGridEltOS.ItemsSource = eltsLigneOS;
                double _ht = Math.Abs(eltsLigneOS.Sum(r => r.PrixUnitaire * r.Qte));
                double _tva = Math.Abs(eltsLigneOS.Sum(r => r.TVA));
                montantHTCpteFact.Content = _ht;  //Convert.ToInt32(_ht);
                montantTVACpteFact.Content = _tva; // Convert.ToInt32(_tva);
                montantTTCCpteFact.Content = _ht + _tva;  //(Convert.ToInt32(_ht) + Convert.ToInt32(_tva));
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        private void cbFamilleArticle_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            try
            { 
                if (cbFamilleArticle.Items.Count != 0)
                {
                    FAMILLE_ARTICLE fa = (FAMILLE_ARTICLE)cbFamilleArticle.SelectedItem;
                    txtCodeFamille.Text = fa.CodeFamArt.ToString();

                    articles = vsomAcc.GetArticleByFamille(fa.CodeFamArt);
                    arts = new List<string>();
                     
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

        private void btnEnregistrer_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                vsomAcc = new VSOMAccessors();

                if (operationsUser.Where(op => op.NomOp == "Finance : Avoir Spot").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin" && utilisateur.LU != "Hermann")
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
                    double _ht =Math.Round( eltsLigneOS.Sum(r => r.PrixUnitaire * r.Qte));
                    double _tva =Math.Round( eltsLigneOS.Sum(r => r.TVA), 0, MidpointRounding.AwayFromZero);
                    double _mttc = _ht + _tva;
                    AVOIR fact = vsomAcc.InsertAvoirSpot(listElements,txtDateCreation.SelectedDate.Value,txtObjetAvoir.Text.Trim(),
                        new TextRange(txtObservations.Document.ContentStart, txtObservations.Document.ContentEnd).Text,utilisateur.IdU,
                        ((CLIENT)cbClient.SelectedItem).IdClient, ((CLIENT)cbClient.SelectedItem).CodeClient, _ht,_mttc, _tva);
                    txtIdAvoir.Text = fact.IdFA.ToString();
                    btnEnregistrer.IsEnabled = false;
                    btnAjoutLS.IsEnabled = btnModifierLS.IsEnabled = btnSupprimerLS.IsEnabled = false;
                    MessageBox.Show("Opération effectuée avec succès", "Avoir Spot");

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
                MessageBox.Show("Echec de la transation : \n" + ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnAnnuler_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void txtTVA_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            try
            { 
                if (cbArticle.SelectedIndex != -1)
                {
                    if (txtTVA.Text == "19,25 %")
                    {
                       // string tva = Math.Round((0.1925 * Convert.ToDouble(txtPU.Text.Replace(".", ",")) * Convert.ToDouble(txtQte.Text.Replace(".", ","))), 3).ToString();
                       // txtPT.Text = (Math.Round(Convert.ToDouble(txtPU.Text.Replace(".", ",")) * Convert.ToDouble(txtQte.Text.Replace(".", ",")), 3) + Math.Round(Convert.ToDouble(tva), 3)).ToString();
                        string tva = (0.1925 * Convert.ToDouble(txtPU.Text.Replace(".", ",")) * Convert.ToDouble(txtQte.Text.Replace(".", ","))).ToString();
                        txtPT.Text = Convert.ToDouble(txtPU.Text.Replace(".", ",")) * Convert.ToDouble(txtQte.Text.Replace(".", ",")) +(Convert.ToDouble(tva)).ToString();
                    
                    }
                    else
                    {
                       // txtPT.Text = (Math.Round(Convert.ToDouble(txtPU.Text.Replace(".", ",")) * Convert.ToDouble(txtQte.Text.Replace(".", ",")), 3)).ToString();
                         txtPT.Text = (Convert.ToDouble(txtPU.Text.Replace(".", ",")) * Convert.ToDouble(txtQte.Text.Replace(".", ","))).ToString();
                    
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

        private void txtQte_LostFocus_1(object sender, RoutedEventArgs e)
        {
            try
            { 
                if (cbArticle.SelectedIndex != -1)
                {
                    if (txtTVA.Text == "19,25 %")
                    {
                       // string tva = Math.Round((0.1925 * Convert.ToDouble(txtPU.Text.Replace(".", ",")) * Convert.ToDouble(txtQte.Text.Replace(".", ","))), 3).ToString();
                       // txtPT.Text = (Math.Round(Convert.ToDouble(txtPU.Text.Replace(".", ",")) * Convert.ToDouble(txtQte.Text.Replace(".", ",")), 3) + Math.Round(Convert.ToDouble(tva), 3)).ToString();
                        string tva = (0.1925 * Convert.ToDouble(txtPU.Text.Replace(".", ",")) * Convert.ToDouble(txtQte.Text.Replace(".", ","))).ToString();
                        txtPT.Text = (Convert.ToDouble(txtPU.Text.Replace(".", ",")) * Convert.ToDouble(txtQte.Text.Replace(".", ",")) + Convert.ToDouble(tva)).ToString();
                    
                    }
                    else
                    {
                        //txtPT.Text = (Math.Round(Convert.ToDouble(txtPU.Text.Replace(".", ",")) * Convert.ToDouble(txtQte.Text.Replace(".", ",")), 3)).ToString();
                        txtPT.Text = (Convert.ToDouble(txtPU.Text.Replace(".", ",")) * Convert.ToDouble(txtQte.Text.Replace(".", ","))).ToString();
                    
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

        private void txtQte_PreviewTextInput_1(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9,.]+-");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void txtPU_PreviewTextInput_1(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9,.]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void cbArticle_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            try
            { 
                if (cbArticle.Items.Count != 0)
                {
                    ARTICLE art = (ARTICLE)cbArticle.SelectedItem;
                    txtCodeArticle.Text = art.CodeArticle.ToString();
                    txtlibArticle.Text = art.LibArticle;
                    txtQte.Text = "";
                    txtUnite.Text = art.LIGNE_PRIX.FirstOrDefault<LIGNE_PRIX>() != null ? art.LIGNE_PRIX.FirstOrDefault<LIGNE_PRIX>().UniteLP : "u";
                    txtTVA.SelectedIndex = (art.CodeTVA == "TVAAP" || art.CodeTVA == "TVATI" || art.CodeTVA == "TVADA") ? 1 : 0;
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

        private void txtPU_LostFocus_1(object sender, RoutedEventArgs e)
        {
            try
            { 
                if (cbArticle.SelectedIndex != -1)
                {
                    if (txtTVA.Text == "19,25 %")
                    {
                       // string tva = Math.Round((0.1925 * Convert.ToDouble(txtPU.Text.Replace(".", ",")) * Convert.ToDouble(txtQte.Text.Replace(".", ","))), 0, MidpointRounding.AwayFromZero).ToString();
                       // txtPT.Text = (Math.Round(Convert.ToDouble(txtPU.Text.Replace(".", ",")) * Convert.ToDouble(txtQte.Text.Replace(".", ",")), 0, MidpointRounding.AwayFromZero) + Math.Round(Convert.ToDouble(tva), 0, MidpointRounding.AwayFromZero)).ToString();
                         string tva = (0.1925 * Convert.ToDouble(txtPU.Text.Replace(".", ",")) * Convert.ToDouble(txtQte.Text.Replace(".", ","))).ToString();
                         txtPT.Text = (Convert.ToDouble(txtPU.Text.Replace(".", ",")) * Convert.ToDouble(txtQte.Text.Replace(".", ",")) + Convert.ToDouble(tva)).ToString();
                    
                    }
                    else
                    {
                        //txtPT.Text = (Math.Round(Convert.ToDouble(txtPU.Text.Replace(".", ",")) * Convert.ToDouble(txtQte.Text.Replace(".", ",")), 0, MidpointRounding.AwayFromZero)).ToString();
                        txtPT.Text = (Convert.ToDouble(txtPU.Text.Replace(".", ",")) * Convert.ToDouble(txtQte.Text.Replace(".", ","))).ToString();
                    
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

        private void btnCompusory_Click_1(object sender, RoutedEventArgs e)
        {
            Finance.AvoirSpotSelect frm = new AvoirSpotSelect(utilisateur,this);
            frm.ShowDialog();
        }
    }
}
