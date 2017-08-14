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
    /// Logique d'interaction pour EscalePanel.xaml
    /// </summary>
    public partial class EscalePanel : DockPanel
    {
        public List<ESCALE> escales { get; set; }
        private List<OPERATION> operationsUser;
        private UTILISATEUR utilisateur;
        private VsomParameters vsp = new VsomParameters();
        public EscalePanel(UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;
                listRechercher.SelectedIndex = 0;
                //cbFiltres.SelectedIndex = 1;
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
                VSOMAccessors vsomAcc = new VSOMAccessors();

                if (operationsUser.Where(op => op.NomOp == "Escale : Enregistrement d'un nouvel élément").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
                {
                    MessageBox.Show("Vous n'avez pas les droits nécessaires pour créer une nouvelle escale. Veuillez contacter un administrateur", "Droits insuffisants !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    EscaleForm escaleForm = new EscaleForm("Nouveau", this, utilisateur);
                    escaleForm.Title = "Nouveau : Escale";
                    escaleForm.valeurEscTab.Visibility = System.Windows.Visibility.Collapsed;
                    //escaleForm.compteDITTab.Visibility = System.Windows.Visibility.Collapsed;
                    escaleForm.compteSEPBCTab.Visibility = System.Windows.Visibility.Collapsed;
                    escaleForm.comptePADTab.Visibility = System.Windows.Visibility.Collapsed;
                    escaleForm.compteArmateurTab.Visibility = System.Windows.Visibility.Collapsed;
                    escaleForm.Show();
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
                    ESCALE esc = vsp.GetEscaleById(((ESCALE)dataGrid.SelectedItem).IdEsc);
                    EscaleForm escaleForm = new EscaleForm(this, esc, utilisateur);
                    escaleForm.Show();
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
                VSOMAccessors vsomAcc = new VSOMAccessors();

                if (e.Key == Key.Return && listRechercher.SelectedIndex != -1)
                {
                    if (listRechercher.SelectedIndex == 0)
                    {
                        int result;
                        if (cbFiltres.SelectedIndex == 0 || cbFiltres.SelectedIndex == -1)
                        {
                            escales = vsp.GetEscalesByNumEscale(Int32.TryParse(txtRechercher.Text.Trim(), out result) ? result : -1);
                        }
                        else if (cbFiltres.SelectedIndex == 1)
                        {
                            escales = vsp.GetEscalesByNumEscale(Int32.TryParse(txtRechercher.Text.Trim(), out result) ? result : -1, "En cours");
                        }
                        else if (cbFiltres.SelectedIndex == 2)
                        {
                            escales = vsp.GetEscalesByNumEscale(Int32.TryParse(txtRechercher.Text.Trim(), out result) ? result : -1, "Clôturée");
                        }
                        dataGrid.ItemsSource = escales;
                        lblStatut.Content = escales.Count + " Escale(s) trouvée(s)";
                    }
                    else if (listRechercher.SelectedIndex == 1)
                    {
                        if (cbFiltres.SelectedIndex == 0 || cbFiltres.SelectedIndex == -1)
                        {
                            escales = vsp.GetEscalesByNumVoyage(txtRechercher.Text.Trim());
                        }
                        else if (cbFiltres.SelectedIndex == 1)
                        {
                            escales = vsp.GetEscalesByNumVoyage(txtRechercher.Text.Trim(), "En cours");
                        }
                        else if (cbFiltres.SelectedIndex == 2)
                        {
                            escales = vsp.GetEscalesByNumVoyage(txtRechercher.Text.Trim(), "Clôturée");
                        }
                        dataGrid.ItemsSource = escales;
                        lblStatut.Content = escales.Count + " Escale(s) trouvée(s)";
                    }
                    else if (listRechercher.SelectedIndex == 2)
                    {
                        if (cbFiltres.SelectedIndex == 0 || cbFiltres.SelectedIndex == -1)
                        {
                            escales = vsp.GetEscalesByCodeAcconier(txtRechercher.Text.Trim());
                        }
                        else if (cbFiltres.SelectedIndex == 1)
                        {
                            escales = vsp.GetEscalesByCodeAcconier(txtRechercher.Text.Trim(), "En cours");
                        }
                        else if (cbFiltres.SelectedIndex == 2)
                        {
                            escales = vsp.GetEscalesByCodeAcconier(txtRechercher.Text.Trim(), "Clôturée");
                        }
                        dataGrid.ItemsSource = escales;
                        lblStatut.Content = escales.Count + " Escale(s) trouvée(s)";
                    }
                    else if (listRechercher.SelectedIndex == 3)
                    {
                        if (cbFiltres.SelectedIndex == 0 || cbFiltres.SelectedIndex == -1)
                        {
                            escales = vsp.GetEscalesByCodeNavire(txtRechercher.Text.Trim());
                        }
                        else if (cbFiltres.SelectedIndex == 1)
                        {
                            escales = vsp.GetEscalesByCodeNavire(txtRechercher.Text.Trim(), "En cours");
                        }
                        else if (cbFiltres.SelectedIndex == 2)
                        {
                            escales = vsp.GetEscalesByCodeNavire(txtRechercher.Text.Trim(), "Clôturée");
                        }
                        dataGrid.ItemsSource = escales;
                        lblStatut.Content = escales.Count + " Escale(s) trouvée(s)";
                    }
                    else if (listRechercher.SelectedIndex == 4)
                    {
                        if (cbFiltres.SelectedIndex == 0 || cbFiltres.SelectedIndex == -1)
                        {
                            escales = vsp.GetEscalesByCodeArmateur(txtRechercher.Text.Trim());
                        }
                        else if (cbFiltres.SelectedIndex == 1)
                        {
                            escales = vsp.GetEscalesByCodeArmateur(txtRechercher.Text.Trim(), "En cours");
                        }
                        else if (cbFiltres.SelectedIndex == 2)
                        {
                            escales = vsp.GetEscalesByCodeArmateur(txtRechercher.Text.Trim(), "Clôturée");
                        }
                        dataGrid.ItemsSource = escales;
                        lblStatut.Content = escales.Count + " Escale(s) trouvée(s)";
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
                VSOMAccessors vsomAcc = new VSOMAccessors();

                if (cbFiltres.SelectedIndex == 0)
                {
                    escales = vsp.GetEscales();
                    dataGrid.ItemsSource = escales;
                    lblStatut.Content = escales.Count + " Escale(s)";
                }
                else if (cbFiltres.SelectedIndex == 1)
                {
                    escales = vsp.GetEscalesByStatut("En cours");
                    dataGrid.ItemsSource = escales;
                    lblStatut.Content = escales.Count + " Escale(s)";
                }
                else if (cbFiltres.SelectedIndex == 2)
                {
                    escales = vsp.GetEscalesByStatut("Clôturée");
                    dataGrid.ItemsSource = escales;
                    lblStatut.Content = escales.Count + " Escale(s)";
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
