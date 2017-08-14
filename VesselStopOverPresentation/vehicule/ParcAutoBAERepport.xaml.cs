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
using System.Windows.Navigation;
using System.Windows.Shapes;
using VesselStopOverData;

namespace VesselStopOverPresentation
{
    /// <summary>
    /// Logique d'interaction pour ParcAutoBAERepport.xaml
    /// </summary>
    public partial class ParcAutoBAERepport : DockPanel
    {
        public List<BON_ENLEVEMENT> bonsEnlever { get; set; }
        private UTILISATEUR utilisateur;
        private List<OPERATION> operationsUser;

        public List<BON_SORTIE> bonsSortie { get; set; }
        public List<DEMANDE_LIVRAISON> demandesLivraison { get; set; }
        private VsomParameters vsp = new VsomParameters();
        public ParcAutoBAERepport( UTILISATEUR user)
        {

            InitializeComponent();
            utilisateur = user;
            //cbFiltres.SelectedIndex = 0;
            dataGrid.Visibility = System.Windows.Visibility.Collapsed;
            dataGridLivraison.Visibility = System.Windows.Visibility.Collapsed;
            dataGridSortie.Visibility = System.Windows.Visibility.Collapsed;

        }

        private void cbFiltres_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (cbFiltres.SelectedIndex == 0)
            {
                dataGrid.Visibility = System.Windows.Visibility.Visible;
                dataGridLivraison.Visibility = System.Windows.Visibility.Collapsed;
                dataGridSortie.Visibility = System.Windows.Visibility.Collapsed;

            }

            if (cbFiltres.SelectedIndex == 1)
            {
                dataGrid.Visibility = System.Windows.Visibility.Collapsed;
                dataGridLivraison.Visibility = System.Windows.Visibility.Collapsed;
                dataGridSortie.Visibility = System.Windows.Visibility.Visible;
            }

            if (cbFiltres.SelectedIndex == 2)
            {
                dataGrid.Visibility = System.Windows.Visibility.Collapsed;
                dataGridLivraison.Visibility = System.Windows.Visibility.Visible;
                dataGridSortie.Visibility = System.Windows.Visibility.Collapsed;
            }

            if (cbFiltres.SelectedIndex == 3)
            {
                dataGrid.Visibility = System.Windows.Visibility.Collapsed;
                dataGridLivraison.Visibility = System.Windows.Visibility.Visible;
                dataGridSortie.Visibility = System.Windows.Visibility.Collapsed;
            }

        }

        private void btnRecherche_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                if (!txtdebut.SelectedDate.HasValue)
                {
                    MessageBox.Show("Veuillez entrer la periode de debut", "Echec de l'opération", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (!txtfin.SelectedDate.HasValue)
                {
                    MessageBox.Show("Veuillez entrer la période de fin", "Echec de l'opération", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    DateTime debut = txtdebut.SelectedDate.Value;
                    DateTime fin = txtfin.SelectedDate.Value;

                    fin = fin.AddHours(23); fin = fin.AddMinutes(59);
                    if (cbFiltres.SelectedIndex == 0)
                    {
                        bonsEnlever = vsp.GetBonsEnleverParcAuto(debut, fin);
                        dataGrid.ItemsSource = bonsEnlever;
                        lblStatut.Content = bonsEnlever.Count + " Bon(s) à enlever validé(s)";
                    }

                    if (cbFiltres.SelectedIndex == 1) //sortie
                    {
                        bonsSortie = vsp.GetBonsSortieVeh(debut,fin);
                        dataGridSortie.ItemsSource = bonsSortie;
                        lblStatut.Content = bonsSortie.Count + " Bon(s) de sortie validé(s)";
                    }

                    if (cbFiltres.SelectedIndex == 2)//livraison cree
                    {
                        demandesLivraison = vsp.GetDemandesLivraison(debut,fin);
                        dataGridLivraison.ItemsSource = demandesLivraison;
                        lblStatut.Content = demandesLivraison.Count + " Demande(s) de livraison enregistrée(s)";
                    }

                    if (cbFiltres.SelectedIndex == 3)//livraison validee
                    {
                        demandesLivraison = vsp.GetDemandesLivraisonValides();
                        dataGridLivraison.ItemsSource = demandesLivraison;
                        lblStatut.Content = demandesLivraison.Count + " Demande(s) de livraison validée(s)";
                    }
                    

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
