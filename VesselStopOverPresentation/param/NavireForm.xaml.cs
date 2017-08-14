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

namespace VesselStopOverPresentation
{
    /// <summary>
    /// Logique d'interaction pour NavireForm.xaml
    /// </summary>
    public partial class NavireForm : Window
    {
        private NavirePanel navirePanel;
        public NAVIRE navire { get; set; }
        private List<ARMATEUR> armateurs;
        public List<string> arms { get; set; }

        private UTILISATEUR utilisateur;
        private List<OPERATION> operationsUser;
        private VsomParameters vsp = new VsomParameters();
        public NavireForm(NavirePanel panel, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();
                VsomParameters vsprm = new VsomParameters();
                InitializeComponent();
                this.DataContext = this;
                navirePanel = panel;

                armateurs = vsprm.GetArmateursActifs();
                arms = new List<string>();
                foreach (ARMATEUR a in armateurs)
                {
                    arms.Add(a.NomArm);
                }

                navire = new NAVIRE();

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

                if (operationsUser.Where(op => op.NomOp == "Navire : Enregistrement d'un nouvel élément").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
                {
                    MessageBox.Show("Vous n'avez pas les droits nécessaires pour mettre à jour les données de base sur les navires. Veuillez contacter un administrateur", "Droits insuffisants !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (txtCode.Text.Trim() == "")
                {
                    MessageBox.Show("Veuillez saisir le code du navire", "Code navire ?", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (txtLibelle.Text.Trim() == "")
                {
                    MessageBox.Show("Veuillez saisir le nom du navire", "Nom navire ?", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (cbArmateur.SelectedIndex == -1)
                {
                    MessageBox.Show("Veuillez sélectionner l'amateur du navire", "Armateur navire ?", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    /*
                     * HA 13juin16 ajout du code radio sur l'ecran creation de navire
                     */ 
                    NAVIRE n = vsomAcc.InsertOrUpdateNavire(navire.IdNav, txtCode.Text, txtLibelle.Text, new TextRange(txtObservations.Document.ContentStart,
                        txtObservations.Document.ContentEnd).Text, armateurs.ElementAt<ARMATEUR>(cbArmateur.SelectedIndex).IdArm, txtCodeRadio.Text);
                    navirePanel.navires = vsprm.GetNaviresActifs();
                    navirePanel.dataGrid.ItemsSource = null;
                    navirePanel.dataGrid.ItemsSource = navirePanel.navires;
                    navirePanel.lblStatut.Content = navirePanel.navires.Count + " navire(s)";
                    MessageBox.Show("Enregistrement effectué avec succès.", "Enregistrement effectué !", MessageBoxButton.OK, MessageBoxImage.Information);
                    txtCode.Text = "";
                    txtLibelle.Text = ""; txtCodeRadio.Text = string.Empty;
                    navire.IdNav = 0;
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

        private void cbArmateur_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                txtCodeArmateur.Text = armateurs.ElementAt<ARMATEUR>(cbArmateur.SelectedIndex).CodeArm;
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
