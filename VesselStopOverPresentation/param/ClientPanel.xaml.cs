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
    /// Logique d'interaction pour ClientPanel.xaml
    /// </summary>
    public partial class ClientPanel : DockPanel
    {
        public List<CLIENT> clients { get; set; }

        private List<OPERATION> operationsUser;
        private UTILISATEUR utilisateur;
        //private VsomParameters vsp = new VsomParameters();
        private VSOMAccessors vsomAcc;
        public ClientPanel(UTILISATEUR user)
        {
            try
            {
                 vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;
                cbFiltres.SelectedIndex = 0;
                listRechercher.SelectedIndex = 0;
                lblStatut.Content = clients.Count + " Client(s)";
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
                ClientForm clientForm = new ClientForm("Nouveau", this, utilisateur);
                clientForm.cbCodeClient.IsEditable = true;
                clientForm.Title = "Nouveau : Client";
                clientForm.Show();
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
                if (dataGrid.SelectedIndex != -1)
                {
                    ClientForm clientForm = new ClientForm(this, utilisateur);
                    CLIENT client = ((CLIENT)dataGrid.SelectedItem);
                    clientForm.cbCodeClient.SelectedItem = client.CodeClient;
                    clientForm.client = client;
                    clientForm.Show();
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

        private void txtRechercher_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                //VSOMAccessors vsomAcc = new VSOMAccessors();
                //VsomParameters vsp = new VsomParameters();
                if (e.Key == Key.Return && listRechercher.SelectedIndex != -1)
                {
                    if (listRechercher.SelectedIndex == 0)
                    {
                        clients = vsomAcc.GetClientByCode(txtRechercher.Text.Trim());
                        dataGrid.ItemsSource = clients;
                        lblStatut.Content = clients.Count + " Client(s) trouvé(s)";
                    }
                    else if (listRechercher.SelectedIndex == 1)
                    {
                        clients = vsomAcc.GetClientByNom(txtRechercher.Text.Trim());
                        dataGrid.ItemsSource = clients;
                        lblStatut.Content = clients.Count + " Client(s) trouvé(s)";
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

        private void btnAnnulerRecherche_Click(object sender, RoutedEventArgs e)
        {
            txtRechercher.Text = null;
        }

        private void cbFiltres_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                //VSOMAccessors vsomAcc = new VSOMAccessors();
                //VsomParameters vsp = new VsomParameters();
                if (cbFiltres.SelectedIndex == 0)
                {
                    clients = vsomAcc.GetClientsActifs();
                    dataGrid.ItemsSource = clients;
                    lblStatut.Content = clients.Count + " Client(s)";
                }
                else if (cbFiltres.SelectedIndex == 1)
                {
                    clients = vsomAcc.GetClientsActifs();
                    dataGrid.ItemsSource = clients;
                    lblStatut.Content = clients.Count + " Client(s)";
                }
                else if (cbFiltres.SelectedIndex == 2)
                {
                    //clients = vsomAcc.GetClients();
                    dataGrid.ItemsSource = null;
                    lblStatut.Content = "0 Client(s)";
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

        private void btncondif_Click_1(object sender, RoutedEventArgs e)
        {
            Finance.ClientCondForm frm = new Finance.ClientCondForm(utilisateur,false);
            frm.Show();
        }
    }
}
