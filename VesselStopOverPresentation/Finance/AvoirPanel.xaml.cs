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
    /// Logique d'interaction pour AvoirPanel.xaml
    /// </summary>
    public partial class AvoirPanel : DockPanel
    {

        public List<AVOIR> avoirs { get; set; }
        private List<OPERATION> operationsUser;
        private UTILISATEUR utilisateur;
        //private VsomParameters vsp = new VsomParameters();
        private VSOMAccessors vsomAcc;
        public AvoirPanel(UTILISATEUR user)
        {
            try
            {
                 vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

                listRechercher.SelectedIndex = 0;
                cbFiltres.SelectedIndex = 0;

                utilisateur = user;
                operationsUser = vsomAcc.GetOperationsUtilisateur(utilisateur.IdU);
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
            
        }

        private void btnNouveau_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (operationsUser.Where(op => op.NomOp == "Avoir : Enregistrement d'un nouvel élément").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
                {
                    MessageBox.Show("Vous n'avez pas les droits nécessaires pour créer un nouvel avoir. Veuillez contacter un administrateur", "Droits insuffisants !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    AvoirForm avoirForm = new AvoirForm("Nouveau", this, utilisateur);
                    avoirForm.cbIdAvoir.IsEnabled = false;
                    avoirForm.Title = "Nouveau : Avoir";
                    avoirForm.borderActions.Visibility = System.Windows.Visibility.Collapsed;
                    avoirForm.borderEtat.Visibility = System.Windows.Visibility.Collapsed;
                    avoirForm.Show();
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

        private void dataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                //VSOMAccessors vsomAcc = new VSOMAccessors();
                if (dataGrid.SelectedIndex != -1)
                {
                    AVOIR av = (AVOIR)dataGrid.SelectedItem;
                    if (av.TypeAvoir == "Spot")
                    {
                        Finance.AvoirCustom frm = new Finance.AvoirCustom(this, av, utilisateur);
                        frm.Show();
                    }

                    if (av.FACTURE.IdFP == null) //avoir facture spot
                    {
                        Finance.AvoirCustom frm = new Finance.AvoirCustom(this, av, utilisateur);
                        frm.Show();
                    }
                    else
                    {
                        AvoirForm avoirForm = new AvoirForm(this, vsomAcc.GetAvoirByIdAvoir(((AVOIR)dataGrid.SelectedItem).IdFA), utilisateur);
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

        private void cbFiltres_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
             //   VSOMAccessors vsomAcc = new VSOMAccessors();

                if (cbFiltres.SelectedIndex == 0)
                {
                    avoirs = vsomAcc.GetAvoirsCurrentYear();
                    dataGrid.ItemsSource = avoirs;
                    lblStatut.Content = avoirs.Count + " Avoir(s)";
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
    }
}
