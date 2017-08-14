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
    /// Logique d'interaction pour ConnexionForm.xaml
    /// </summary>
    public partial class ConnexionForm : Window
    {
        public ConnexionForm()
        {
            InitializeComponent();
            txtCompte.Focus();
        }

       // private VSOMAccessors vsomAcc = new VSOMAccessors();
        private VsomSecurity vsomAcc = new VsomSecurity();

        private void txtPassword_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Return && txtCompte.Text.Trim() != "" && txtPassword.Password.Trim() != "")
                {
                    UTILISATEUR user = vsomAcc.SeConnecter(txtCompte.Text, txtPassword.Password);
                    Container mainForm = new Container();
                    VesselStopOverWindow vesselPage = new VesselStopOverWindow(user);
                    mainForm.NavigationService.Navigate(vesselPage);
                    this.Hide();
                    mainForm.Title = user.LU + " - NOVA App : Vessel StopOver Management System";
                    //vsomAcc.CorrectionAgencyFees();
                    mainForm.Show();
                }
            }
            catch (ConnexionException ex)
            {
                lblStatut.Content = "Paramètres incorrects";
                MessageBox.Show(ex.Message, "Echec de connexion", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                txtCompte.Focus();
                txtCompte.SelectAll();
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération 2 !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void txtPassword_GotFocus(object sender, RoutedEventArgs e)
        {
            txtPassword.SelectAll();
        }
    }
}
