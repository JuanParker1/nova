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
    /// Logique d'interaction pour AcconierForm.xaml
    /// </summary>
    public partial class AcconierForm : Window
    {
        private AcconierPanel acconierPanel;
        public ACCONIER acconier { get; set; }

        private UTILISATEUR utilisateur;
        private List<OPERATION> operationsUser;
       // private VsomParameters vsp = new VsomParameters();
        private VSOMAccessors vsomAcc;
        public AcconierForm(AcconierPanel panel, UTILISATEUR user)
        {
            try
            {
                 vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;
                acconierPanel = panel;

                acconier = new ACCONIER();

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

        private void btnEnregistrer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                VsomConfig vsom = new VsomConfig();
                //VsomParameters vsp = new VsomParameters();

                if (operationsUser.Where(op => op.NomOp == "Acconier : Enregistrement d'un nouvel élément").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
                {
                    MessageBox.Show("Vous n'avez pas les droits nécessaires pour mettre à jour les données de base acconier. Veuillez contacter un administrateur", "Droits insuffisants !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (txtCode.Text.Trim() == "")
                {
                    MessageBox.Show("Veuillez saisir le code de l'acconier", "Code acconier ?", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (txtLibelle.Text.Trim() == "")
                {
                    MessageBox.Show("Veuillez saisir le nom de l'acconier", "Nom acconier ?", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    ACCONIER a = vsom.InsertOrUpdateAcconier(acconier.IdAcc, txtCode.Text, txtLibelle.Text, new TextRange(txtObservations.Document.ContentStart, txtObservations.Document.ContentEnd).Text);
                    acconierPanel.acconiers = vsomAcc.GetAcconiersActifs();
                    acconierPanel.dataGrid.ItemsSource = null;
                    acconierPanel.dataGrid.ItemsSource = acconierPanel.acconiers;
                    acconierPanel.lblStatut.Content = acconierPanel.acconiers.Count + " acconier(s)";
                    MessageBox.Show("Enregistrement effectué avec succès.", "Enregistrement effectué !", MessageBoxButton.OK, MessageBoxImage.Information);
                    txtCode.Text = "";
                    txtLibelle.Text = "";
                    acconier.IdAcc = 0;
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

        //private void txtCCAcc_PreviewTextInput(object sender, TextCompositionEventArgs e)
        //{
        //    Regex regex = new Regex("[^0-9]+");
        //    e.Handled = regex.IsMatch(e.Text);
        //}
    }
}
