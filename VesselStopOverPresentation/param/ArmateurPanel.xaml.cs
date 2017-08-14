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
    /// Logique d'interaction pour ArmateurPanel.xaml
    /// </summary>
    public partial class ArmateurPanel : DockPanel
    {

        private UTILISATEUR utilisateur;
        private List<OPERATION> operationsUser;
        public List<ARMATEUR> armateurs { get; set; }
        private VsomParameters vsp = new VsomParameters();

        public ArmateurPanel(UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;
                cbFiltres.SelectedIndex = 0;
                listRechercher.SelectedIndex = 0;

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

        private void cbFiltres_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();
                VsomParameters vsp = new VsomParameters();

                if (cbFiltres.SelectedIndex == 0)
                {
                    armateurs = vsp.GetArmateursActifs();
                    dataGrid.ItemsSource = armateurs;
                    lblStatut.Content = armateurs.Count + " Armateur(s)";
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
                VsomParameters vsp = new VsomParameters();
                if (e.Key == Key.Return && listRechercher.SelectedItem != null)
                {
                    if (listRechercher.SelectedIndex == 0)
                    {
                        armateurs = vsp.GetArmateursByCodeArm(txtRechercher.Text.Trim());
                        dataGrid.ItemsSource = armateurs;
                        lblStatut.Content = armateurs.Count + " Armateur(s) trouvé(s)";
                    }
                    else if (listRechercher.SelectedIndex == 1)
                    {
                        armateurs = vsp.GetArmateursByNomArm(txtRechercher.Text.Trim());
                        dataGrid.ItemsSource = armateurs;
                        lblStatut.Content = armateurs.Count + " Armateur(s) trouvé(s)";
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

        private void btnNouveau_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (operationsUser.Where(op => op.NomOp == "Armateur : Enregistrement d'un nouvel élément").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
                {
                    MessageBox.Show("Vous n'avez pas les droits nécessaires pour créer un nouvel armateur. Veuillez contacter un administrateur", "Droits insuffisants !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    ArmateurForm armateurForm = new ArmateurForm(this, utilisateur);
                    armateurForm.Title = "Nouveau : Armateur";
                    armateurForm.Show();
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
                if (dataGrid.SelectedIndex != -1)
                {
                    ArmateurForm armateurForm = new ArmateurForm(this, utilisateur);
                    ARMATEUR a = (ARMATEUR)dataGrid.SelectedItem;
                    armateurForm.armateur = a;
                    armateurForm.txtCode.Text = a.CodeArm;
                    armateurForm.txtLibelle.Text = a.NomArm;
                    armateurForm.txtAdresse.Document.Blocks.Clear();
                    armateurForm.txtAdresse.Document.Blocks.Add(new Paragraph(new Run(a.AdresseArm)));
                    armateurForm.txtObservations.Document.Blocks.Clear();
                    armateurForm.txtObservations.Document.Blocks.Add(new Paragraph(new Run(a.AIArm)));
                    if (armateurForm.clients.FirstOrDefault<CLIENT>(cl => cl.CodeClient == a.CCArm) != null)
                    {
                        armateurForm.cbClientArmateur.SelectedItem = armateurForm.clients.FirstOrDefault<CLIENT>(cl => cl.CodeClient == a.CCArm).NomClient;
                    }
                    armateurForm.Title = "Armateur : " + a.NomArm;
                    armateurForm.lblStatut.Content = a.StatutArm == "A" ? "Actif" : "Inactif";
                    armateurForm.Show();
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

        private void mnActive_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void mnDesactive_Click_1(object sender, RoutedEventArgs e)
        {

        }
    }
}
