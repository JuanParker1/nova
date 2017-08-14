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
using System.Text.RegularExpressions;

namespace VesselStopOverPresentation
{
    /// <summary>
    /// Logique d'interaction pour ArmateurForm.xaml
    /// </summary>
    public partial class ArmateurForm : Window
    {
        private ArmateurPanel armateurPanel;
        public ARMATEUR armateur { get; set; }

        public List<CLIENT> clients { get; set; }
        public List<string> clts { get; set; }

        private UTILISATEUR utilisateur;
        private List<OPERATION> operationsUser;
        private VsomParameters vsp = new VsomParameters();

        public ArmateurForm(ArmateurPanel panel, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();
                //VsomParameters vsprm = new VsomParameters();
                InitializeComponent();
                this.DataContext = this;
                armateurPanel = panel;

                clients = vsp.GetClientsActifs();
                clts = new List<string>();
                foreach (CLIENT cl in clients)
                {
                    clts.Add(cl.NomClient);
                }

                armateur = new ARMATEUR();

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

        private void btnEnregistrer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                VsomConfig vsomAcc = new VsomConfig();
                VsomParameters vsprm = new VsomParameters();

                if (operationsUser.Where(op => op.NomOp == "Armateur : Enregistrement d'un nouvel élément").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
                {
                    MessageBox.Show("Vous n'avez pas les droits nécessaires pour mettre à jour les données de base armateur. Veuillez contacter un administrateur", "Droits insuffisants !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (txtCode.Text.Trim() == "")
                {
                    MessageBox.Show("Veuillez saisir le code du navire", "Code navire ?", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (txtLibelle.Text.Trim() == "")
                {
                    MessageBox.Show("Veuillez saisir le nom du navire", "Nom navire ?", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (cbClientArmateur.SelectedIndex == -1)
                {
                    MessageBox.Show("Veuillez saisir le compte comptable de l'armateur", "Compte armateur ?", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    ARMATEUR a = vsomAcc.InsertOrUpdateArmateur(armateur.IdArm, txtCode.Text, txtLibelle.Text, new TextRange(txtAdresse.Document.ContentStart, txtAdresse.Document.ContentEnd).Text, txtCompteArmateur.Text, new TextRange(txtObservations.Document.ContentStart, txtObservations.Document.ContentEnd).Text);
                    armateurPanel.armateurs = vsprm.GetArmateursActifs();
                    armateurPanel.dataGrid.ItemsSource = null;
                    armateurPanel.dataGrid.ItemsSource = armateurPanel.armateurs;
                    armateurPanel.lblStatut.Content = armateurPanel.armateurs.Count + " armateur(s)";
                    MessageBox.Show("Enregistrement effectué avec succès.", "Enregistrement effectué !", MessageBoxButton.OK, MessageBoxImage.Information);
                    txtCode.Text = "";
                    txtLibelle.Text = "";
                    txtAdresse.Document.Blocks.Clear();
                    txtObservations.Document.Blocks.Clear();
                    armateur.IdArm = 0;
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

        private void cbClient_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                txtCompteArmateur.Text = clients.ElementAt<CLIENT>(cbClientArmateur.SelectedIndex).CodeClient;
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
