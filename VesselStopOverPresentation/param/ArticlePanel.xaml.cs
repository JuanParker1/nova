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
    /// Logique d'interaction pour ArticlePanel.xaml
    /// </summary>
    public partial class ArticlePanel : DockPanel
    {
        private UTILISATEUR _user { get; set; }
        
        private List<OPERATION> operationsUser;
        public List<ARTICLE> articles { get; set; }
        List<FAMILLE_ARTICLE> famille { get; set; }
        VsomParameters vsomAcc;
        VsomConfig vc;
        public ArticlePanel(UTILISATEUR user)
        {
            try
            {
                vsomAcc = new VsomParameters();
                 vc = new VsomConfig();
               // VsomParameters vsprm = new VsomParameters();
                InitializeComponent();
                this.DataContext = this; 
                listRechercher.SelectedIndex = 0;

                _user = user;
                //operationsUser = vsp.GetOperationsUtilisateur(_user.IdU);
                articles = vsomAcc.GetArticleAll();
                dataGrid.ItemsSource = articles;
                famille = vsomAcc.GetFamillesArticles();
                cbFamille.ItemsSource = famille; cbFamille.DisplayMemberPath = "LibFamArt";
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

        private void cbFamille_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            if (cbFamille.SelectedIndex != -1)
            {
                try
                {
                    //VsomParameters vsomAcc = new VsomParameters();
                    FAMILLE_ARTICLE fa = (FAMILLE_ARTICLE)cbFamille.SelectedItem;
                    articles = vsomAcc.GetArticleByFamille(fa.CodeFamArt); dataGrid.ItemsSource = articles;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public void Refresh()
        {
            //listRechercher.SelectedIndex = 0; 
            articles = vsomAcc.GetArticleAll();
            dataGrid.ItemsSource = null;
            dataGrid.ItemsSource = articles;
        }


        private void dataGrid_MouseDoubleClick_1(object sender, MouseButtonEventArgs e)
        {
            if (dataGrid.SelectedIndex != -1)
            {
                ARTICLE a = (ARTICLE)dataGrid.SelectedItem;
                ArticleForm frm = new ArticleForm(this, _user, a);
                frm.ShowDialog();
            }
        }

        private void btnNouveau_Click_1(object sender, RoutedEventArgs e)
        {
            ArticleForm frm = new ArticleForm(this, _user,null);
            frm.ShowDialog();
        }

        private void txtRechercher_PreviewKeyDown_1(object sender, KeyEventArgs e)
        {
            try
            {
                //VSOMAccessors vsomAcc = new VSOMAccessors(); 
               // VsomParameters vsp = new VsomParameters();
                if (listRechercher.SelectedIndex == 0)
                {
                    int result;
                    int id = Int32.TryParse(txtRechercher.Text, out result) ? result : -1;
                    articles = vsomAcc.GetArticleByCodeArticle(id); dataGrid.ItemsSource = articles;
                }

                if (listRechercher.SelectedIndex == 1)
                {

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void mnDelEmpl_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void mnActive_Click_1(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedIndex != -1)
            {
                ARTICLE art = (ARTICLE)dataGrid.SelectedItem;
                if (art.Statut == "I")
                {
                    if (MessageBox.Show("Voulez-vous activer cet article?", "Articles", MessageBoxButton.YesNo, MessageBoxImage.Question)
                        == MessageBoxResult.Yes)
                    {
                        try
                        { 
                            vc.ActiveArticle(art.CodeArticle, "");
                            MessageBox.Show("Article modifié", "Article");
                           List<ARTICLE> article =vsomAcc.GetArticleAll();
                            dataGrid.ItemsSource = null;
                            dataGrid.ItemsSource = article;// articles;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Echec de la modification : " + ex.Message, "Articles");
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Impossible d'effectuer cette action. Article actif","Articles");
                }
            }
        }

        private void mnDesactive_Click_1(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedIndex != -1)
            {
                ARTICLE art = (ARTICLE)dataGrid.SelectedItem;
                if (art.Statut == "A")
                {
                    if (MessageBox.Show("Voulez-vous desactiver cet article?", "Articles", MessageBoxButton.YesNo, MessageBoxImage.Question)
                        == MessageBoxResult.Yes)
                    {
                        try
                        {
                            vc.DesactiveArticle(art.CodeArticle, "");
                            MessageBox.Show("Article modifié", "Article");
                            //articles = vsomAcc.GetArticleAll();
                            dataGrid.ItemsSource = null;
                            dataGrid.ItemsSource = vsomAcc.GetArticleAll(); //articles;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Echec de la modification : " + ex.Message, "Articles");
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Impossible d'effectuer cette action. Article inactif", "Articles");
                }
            }
        }

        private void dataGrid_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            if (dataGrid.SelectedIndex != -1)
            {
                ARTICLE art = (ARTICLE)dataGrid.SelectedItem;
                if (art.Statut == "A")
                {
                    mnActive.IsEnabled = false;
                    mnDesactive.IsEnabled = true;
                }
                else
                {
                    mnDesactive.IsEnabled = false; mnActive.IsEnabled = true;
                }
            }
        }
    }
}
