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
    /// Logique d'interaction pour FactureSpotPanel.xaml
    /// </summary>
    public partial class FactureSpotPanel : DockPanel
    {
        UTILISATEUR _user; private VsomParameters vsp = new VsomParameters();
        public List<FACTURE> factures { get; set; }
        public FactureSpotPanel(UTILISATEUR user)
        {
            InitializeComponent();
            _user = user;
        }

        private void btnNouveau_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                VsomParameters vsp = new VsomParameters();
                if (vsp.GetOperationsUtilisateur(_user.IdU).Where(op => op.NomOp == "Facture spot: Enregistrement d'un nouvel élément").FirstOrDefault<OPERATION>() == null && _user.LU != "Admin")
                {
                    MessageBox.Show("Vous n'avez pas les droits nécessaires pour créer une facture spot. Veuillez contacter un administrateur", "Facture Spot");
                }
                else
                {
                    FactureSpot fact = new FactureSpot(this, _user);
                    fact.Show();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Echec de l'opération : \n " + ex.Message, "Facture Spot : Nouveau");
            }
        }


        private void btnRecherche_Click(object sender, RoutedEventArgs e)
        {
            try { 
                 //VSOMAccessors vsom = new VSOMAccessors();
                DateTime fin = txtfin.SelectedDate.Value;
                fin= fin.AddHours(23); fin=fin.AddMinutes(59);
                 factures = vsp.GetFactureSpot(txtdebut.SelectedDate.Value,fin);
                 dataGrid.ItemsSource = null;
                 dataGrid.ItemsSource = factures;
                 lblStatut.Content = factures.Count + " Facture(s) trouvée(s)";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Echec de l'opération. \n " + ex.Message, "Facture Spot : recherche");
            }
        }

        private void txtRechercher_PreviewKeyDown(object sender, RoutedEventArgs e)
        {
            try {
                //VSOMAccessors vsom = new VSOMAccessors();
                factures = new List<FACTURE>(); int result;
                FACTURE fact = vsp.GetFactureSpotByIdDocSAP(Int32.TryParse(txtRechercher.Text.Trim(), out result) ? result : -1);
                if (fact != null)
                {
                    factures.Add(fact);
                }

                dataGrid.ItemsSource = null;
                dataGrid.ItemsSource = factures;
                lblStatut.Content = factures.Count + " Facture(s) trouvée(s)";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Echec de l'opération. \n " + ex.Message, "Facture Spot : recherche");
            }
        }

        private void dataGrid_MouseDoubleClick_1(object sender, MouseButtonEventArgs e)
        {
            try
            {
                //VSOMAccessors vsomAcc = new VSOMAccessors();
                if (vsp.GetOperationsUtilisateur(_user.IdU).Where(op => op.NomOp == "Facture spot: Visualisation des éléments existants").FirstOrDefault<OPERATION>() == null && _user.LU != "Admin")
                {
                    MessageBox.Show("Vous n'avez pas les droits nécessaires pour effectuer cette action. Veuillez contacter un administrateur", "Facture Spot");
                }
                else
                {
                    if (dataGrid.SelectedIndex != -1)
                    {
                        FACTURE fact = (FACTURE)dataGrid.SelectedItem;
                        FactureSpot factForm = new FactureSpot(this, fact, _user);
                        factForm.Show();
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

    }
}
