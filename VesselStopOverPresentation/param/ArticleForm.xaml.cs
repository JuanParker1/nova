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
    /// Logique d'interaction pour ArticleForm.xaml
    /// </summary>
    public partial class ArticleForm : Window
    {
        private UTILISATEUR _user { get; set; }

        private List<OPERATION> operationsUser;
        public List<ARTICLE> articles { get; set; }
        List<FAMILLE_ARTICLE> famille { get; set; }
        ArticlePanel _panel;
        ARTICLE articl;
        private List<CODE_TVA> _lstCodeTVA;
        private VsomParameters vsomAcc;
       private VsomConfig vsomAc;
        public ArticleForm(ArticlePanel panel, UTILISATEUR user,ARTICLE _article)
        {
            try
            {
                 vsomAcc = new VsomMarchal();
                 vsomAc = new VsomConfig();

                //VsomParameters vsp = new VsomParameters();
                InitializeComponent();
                this.DataContext = this;
                _user = user; _panel = panel;
                operationsUser = vsomAcc.GetOperationsUtilisateur(_user.IdU);
                famille = vsomAcc.GetFamillesArticles(); cbFamille.ItemsSource = famille; cbFamille.DisplayMemberPath = "LibFamArt";
                //chargement des code_tva 
                _lstCodeTVA = vsomAcc.GetCodesTVA();
                codeTVA.ItemsSource = _lstCodeTVA;
                codeTVA.DisplayMemberPath = "CodeTVA"; 
                codeTVA.SelectedIndex = 0;
                articl = null;
                if (_article != null)
                {
                    articl = _article;
                    int i = famille.FindIndex(r => r.CodeFamArt == _article.FAMILLE_ARTICLE.CodeFamArt);
                    cbFamille.SelectedIndex = i;

                    txtCode.Text = _article.CodeArticle.ToString(); txtCode.IsEnabled = false;
                    txtLibelle.Text = _article.LibArticle; txtCCArticleEx.Text = _article.CCArticleEx;
                    txtCCArticle.Text = _article.CCArticle;
                     
                    codeTVA.SelectedItem = _article.CODE_TVA; 
                    LIGNE_PRIX lp = _article.LIGNE_PRIX.FirstOrDefault<LIGNE_PRIX>();
                    txtPU.Text = lp.PU1LP.Value.ToString();
                    txtUnite.Text = lp.UniteLP;
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

        private void btnEnregistrer_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                //VsomConfig vsomAcc = new VsomConfig();
                //VsomParameters vsp = new VsomParameters();
                int result;
                int id = Int32.TryParse(txtCode.Text.Trim(), out result) ? result : -1;
                int pu = Int32.TryParse(txtPU.Text.Trim(), out result) ? result : -1;
                if (operationsUser.Where(op => op.NomOp == "Articles: Opération").FirstOrDefault<OPERATION>() == null && _user.LU != "Admin")
                {
                    MessageBox.Show("Vous n'avez pas les droits nécessaires pour cette action. Veuillez contacter un administrateur", "Droits insuffisants !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (txtCode.Text.Trim() == "" || id==-1)
                {
                    MessageBox.Show("Veuillez saisir le code", "Code Article ?", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (txtLibelle.Text.Trim() == "")
                {
                    MessageBox.Show("Veuillez saisir le libelle ", "Article ?", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (txtCCArticle.Text.Trim() == "")
                {
                    MessageBox.Show("Veuillez saisir le compte comptable ", "Article ?", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (cbFamille.SelectedIndex == -1)
                {
                    MessageBox.Show("Veuillez selectionnet la famille d'article ", "Article ?", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (txtPU.Text.Trim() == "" || pu==-1)
                {
                    MessageBox.Show("Veuillez saisir le prix unitaire de l'article ", "Article ?", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (txtUnite.Text.Trim() == "")
                {
                    MessageBox.Show("Veuillez saisir l'unité de mesure de l'article ", "Article ?", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    if (articl == null)
                    {
                        vsomAc.InsertArticle(((FAMILLE_ARTICLE)cbFamille.SelectedItem).CodeFamArt, int.Parse(txtCode.Text.Trim()), txtLibelle.Text.Trim(),
                            txtCCArticle.Text.Trim(), txtCCArticleEx.Text.Trim(), codeTVA.Text,pu,txtUnite.Text.Trim());
                    }
                    else
                    {
                        vsomAc.UpdateArticle(((FAMILLE_ARTICLE)cbFamille.SelectedItem).CodeFamArt, int.Parse(txtCode.Text.Trim()), txtLibelle.Text.Trim(),
                                txtCCArticle.Text.Trim(), txtCCArticleEx.Text.Trim(), codeTVA.Text, pu, txtUnite.Text.Trim());
                    }
                        MessageBox.Show("Opération effectuée", "Article");

                        articles = vsomAcc.GetArticleAll();
                        _panel.dataGrid.ItemsSource = articles;
                        this.Close();

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

        private void txtCode_TextInput_1(object sender, TextCompositionEventArgs e)
        {

        }

        private void txtCode_PreviewTextInput_1(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void txtPU_PreviewTextInput_1(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
