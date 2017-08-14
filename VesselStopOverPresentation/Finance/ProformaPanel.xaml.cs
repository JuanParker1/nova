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
    /// Logique d'interaction pour ProformaPanel.xaml
    /// </summary>
    public partial class ProformaPanel : DockPanel
    {

        public List<PROFORMA> proformas { get; set; }
        private List<OPERATION> operationsUser;
        private UTILISATEUR utilisateur;
        private VsomParameters vsp = new VsomParameters();
        public ProformaPanel(UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

                listRechercher.SelectedIndex = 0;
                //cbFiltres.SelectedIndex = 2;

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
                if (operationsUser.Where(op => op.NomOp == "Proforma : Enregistrement d'un nouvel élément").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
                {
                    MessageBox.Show("Vous n'avez pas les droits nécessaires pour créer une nouvelle proforma. Veuillez contacter un administrateur", "Droits insuffisants !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    ProformaForm profForm = new ProformaForm("Nouveau", this, utilisateur);
                    profForm.cbIdProf.IsEnabled = false;
                    profForm.Title = "Nouveau : Proforma";
                    profForm.borderActions.Visibility = System.Windows.Visibility.Collapsed;
                    profForm.borderEtat.Visibility = System.Windows.Visibility.Collapsed;
                    profForm.eltBorder.IsEnabled = false;
                    profForm.Show();
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
                    ProformaForm profForm = new ProformaForm(this, vsp.GetProformaByIdProf(((PROFORMA)dataGrid.SelectedItem).IdFP), utilisateur);
                    profForm.Show();
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
                    proformas = vsp.GetProformas();
                    dataGrid.ItemsSource = proformas;
                    lblStatut.Content = proformas.Count + " Proforma(s)";
                }
                else if (cbFiltres.SelectedIndex == 1)
                {
                    proformas = vsp.GetProformasValidees();
                    dataGrid.ItemsSource = proformas;
                    lblStatut.Content = proformas.Count + " Proforma(s)";
                }
                else if (cbFiltres.SelectedIndex == 2)
                {
                    proformas = vsp.GetProformasEnAttente();
                    dataGrid.ItemsSource = proformas;
                    lblStatut.Content = proformas.Count + " Proforma(s)";
                }
                else if (cbFiltres.SelectedIndex == 3)
                {
                    proformas = vsp.GetProformasAnnulees();
                    dataGrid.ItemsSource = proformas;
                    lblStatut.Content = proformas.Count + " Proforma(s)";
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

                if (e.Key == Key.Return && listRechercher.SelectedItem != null)
                {
                    if (listRechercher.SelectedIndex == 0)
                    {
                        int result;
                        proformas = new List<PROFORMA>();
                        PROFORMA prof = vsp.GetProformaByIdProf(Int32.TryParse(txtRechercher.Text.Trim(), out result) ? result : -1);
                        if (prof != null)
                        {
                            proformas.Add(prof);
                        }
                        dataGrid.ItemsSource = proformas;
                        lblStatut.Content = proformas.Count + " Proforma(s) trouvée(s)";
                    }
                    else if (listRechercher.SelectedIndex == 1)
                    {
                        proformas = vsp.GetProformasByNumBL(txtRechercher.Text.Trim());
                        dataGrid.ItemsSource = proformas;
                        lblStatut.Content = proformas.Count + " Proforma(s) trouvée(s)";
                    }
                    else if (listRechercher.SelectedIndex == 2)
                    {
                        proformas = vsp.GetProformasByNumEsc(txtRechercher.Text.Trim());
                        dataGrid.ItemsSource = proformas;
                        lblStatut.Content = proformas.Count + " Proforma(s) trouvée(s)";
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
    }
}
