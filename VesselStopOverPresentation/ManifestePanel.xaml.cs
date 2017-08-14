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
    /// Logique d'interaction pour ManifestePanel.xaml
    /// </summary>
    public partial class ManifestePanel : DockPanel
    {
        public List<MANIFESTE> manifestes { get; set; }
        private List<OPERATION> operationsUser;
        private UTILISATEUR utilisateur;
        private VsomParameters vsp = new VsomParameters();
        public ManifestePanel(UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;
                listRechercher.SelectedIndex = 0;
                //cbFiltres.SelectedIndex = 0;
                utilisateur = user;
                operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);
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
                if (operationsUser.Where(op => op.NomOp == "Manifeste : Enregistrement d'un nouvel élément").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
                {
                    MessageBox.Show("Vous n'avez pas les droits nécessaires pour créer un nouveau manifeste. Veuillez contacter un administrateur", "Droits insuffisants !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    ManifesteForm manifForm = new ManifesteForm("Nouveau", this, utilisateur);
                    manifForm.cbIdMan.IsEnabled = false;
                    manifForm.cbNumEscale.IsEditable = true;
                    manifForm.Title = "Nouveau : Manifeste";
                    manifForm.conTab.IsSelected = true;
                    manifForm.compteManTab.Visibility = System.Windows.Visibility.Collapsed;
                    //manifForm.compteDITTab.Visibility = System.Windows.Visibility.Collapsed;
                    manifForm.Show();
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
                VSOMAccessors vsomAcc = new VSOMAccessors();

                if (dataGrid.SelectedIndex != -1)
                {
                    MANIFESTE man = vsp.GetManifesteByIdMan(((MANIFESTE)dataGrid.SelectedItem).IdMan);
                    ManifesteForm manifesteForm = new ManifesteForm(this, man, utilisateur);
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

        private void btnAnnulerRecherche_Click(object sender, RoutedEventArgs e)
        {
            txtRechercher.Text = null;
        }

        private void txtRechercher_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                if (e.Key == Key.Return && listRechercher.SelectedItem != null)
                {
                    if (listRechercher.SelectedIndex == 0)
                    {
                        int result;
                        if (cbFiltres.SelectedIndex == 0 || cbFiltres.SelectedIndex == -1)
                        {
                            manifestes = vsp.GetManifestesByNumEscale(Int32.TryParse(txtRechercher.Text.Trim(), out result) ? result : -1);
                        }
                        else if (cbFiltres.SelectedIndex == 1)
                        {
                            manifestes = vsp.GetManifestesByNumEscale(Int32.TryParse(txtRechercher.Text.Trim(), out result) ? result : -1, "En cours");
                        }
                        else if (cbFiltres.SelectedIndex == 2)
                        {
                            manifestes = vsp.GetManifestesByNumEscale(Int32.TryParse(txtRechercher.Text.Trim(), out result) ? result : -1, "Clôturé");
                        }
                        dataGrid.ItemsSource = manifestes;
                        lblStatut.Content = manifestes.Count + " Manifestes trouvé(s)";
                    }
                    else if (listRechercher.SelectedIndex == 1)
                    {
                        if (cbFiltres.SelectedIndex == 0 || cbFiltres.SelectedIndex == -1)
                        {
                            manifestes = vsp.GetManifestesByCodeAcconier(txtRechercher.Text.Trim());
                        }
                        else if (cbFiltres.SelectedIndex == 1)
                        {
                            manifestes = vsp.GetManifestesByCodeAcconier(txtRechercher.Text.Trim(), "En cours");
                        }
                        else if (cbFiltres.SelectedIndex == 2)
                        {
                            manifestes = vsp.GetManifestesByCodeAcconier(txtRechercher.Text.Trim(), "Clôturé");
                        }
                        dataGrid.ItemsSource = manifestes;
                        lblStatut.Content = manifestes.Count + " Manifestes trouvé(s)";
                    }
                    else if (listRechercher.SelectedIndex == 2)
                    {
                        if (cbFiltres.SelectedIndex == 0 || cbFiltres.SelectedIndex == -1)
                        {
                            manifestes = vsp.GetManifestesByCodeNavire(txtRechercher.Text.Trim());
                        }
                        else if (cbFiltres.SelectedIndex == 1)
                        {
                            manifestes = vsp.GetManifestesByCodeNavire(txtRechercher.Text.Trim(), "En cours");
                        }
                        else if (cbFiltres.SelectedIndex == 2)
                        {
                            manifestes = vsp.GetManifestesByCodeNavire(txtRechercher.Text.Trim(), "Clôturé");
                        }
                        dataGrid.ItemsSource = manifestes;
                        lblStatut.Content = manifestes.Count + " Manifestes trouvé(s)";
                    }
                    else if (listRechercher.SelectedIndex == 3)
                    {
                        if (cbFiltres.SelectedIndex == 0 || cbFiltres.SelectedIndex == -1)
                        {
                            manifestes = vsp.GetManifestesByCodeArmateur(txtRechercher.Text.Trim());
                        }
                        else if (cbFiltres.SelectedIndex == 1)
                        {
                            manifestes = vsp.GetManifestesByCodeArmateur(txtRechercher.Text.Trim(), "En cours");
                        }
                        else if (cbFiltres.SelectedIndex == 2)
                        {
                            manifestes = vsp.GetManifestesByCodeArmateur(txtRechercher.Text.Trim(), "clôturé");
                        }
                        dataGrid.ItemsSource = manifestes;
                        lblStatut.Content = manifestes.Count + " Manifestes trouvé(s)";
                    }
                    else if (listRechercher.SelectedIndex == 4)
                    {
                        if (cbFiltres.SelectedIndex == 0 || cbFiltres.SelectedIndex == -1)
                        {
                            manifestes = vsp.GetManifestesByCodePort(txtRechercher.Text.Trim());
                        }
                        else if (cbFiltres.SelectedIndex == 1)
                        {
                            manifestes = vsp.GetManifestesByCodePort(txtRechercher.Text.Trim(), "En cours");
                        }
                        else if (cbFiltres.SelectedIndex == 2)
                        {
                            manifestes = vsp.GetManifestesByCodePort(txtRechercher.Text.Trim(), "Clôturé");
                        }
                        dataGrid.ItemsSource = manifestes;
                        lblStatut.Content = manifestes.Count + " Manifestes trouvé(s)";
                    }
                }
                else if (e.Key == Key.Escape)
                {
                    txtRechercher.Text = null;
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
                VSOMAccessors vsomAcc = new VSOMAccessors();

                if (cbFiltres.SelectedIndex == 0)
                {
                    manifestes = vsp.GetManifestes();
                    dataGrid.ItemsSource = manifestes;
                    lblStatut.Content = manifestes.Count + " Manifeste(s)";
                }
                else if (cbFiltres.SelectedIndex == 1)
                {
                    manifestes = vsp.GetManifestesByStatut("En cours");
                    dataGrid.ItemsSource = manifestes;
                    lblStatut.Content = manifestes.Count + " Manifeste(s) en cours(s)";
                }
                else if (cbFiltres.SelectedIndex == 2)
                {
                    manifestes = vsp.GetManifestesByStatut("Clôturé");
                    dataGrid.ItemsSource = manifestes;
                    lblStatut.Content = manifestes.Count + " Manifeste(s) clôturé(s)";
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
