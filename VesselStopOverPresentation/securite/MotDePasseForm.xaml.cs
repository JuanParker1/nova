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
    /// Logique d'interaction pour MotDePasseForm.xaml
    /// </summary>
    public partial class MotDePasseForm : Window
    {
        private VSOMAccessors vsomAcc = new VSOMAccessors();

        private List<OPERATION> operationsUser;
        private UTILISATEUR utilisateur;

        public AdministrationControlPanel administrationPanel { get; set; }

        private FormLoader formLoader;
        private VsomParameters vsp = new VsomParameters();
        public MotDePasseForm(AdministrationControlPanel admPanel, UTILISATEUR user)
        {
            try
            {
                InitializeComponent();
                txtCompte.Text = user.LU;

                utilisateur = user;
                operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);

                administrationPanel = admPanel;

                formLoader = new FormLoader(utilisateur);
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
               // VSOMAccessors vsomAcc = new VSOMAccessors();
                 VsomSecurity vsomAcc = new VsomSecurity();

                if (txtPasswordAncien.Password == "")
                {
                    MessageBox.Show("Veuillez entrer votre mot de passe actuel", "Mot de passe actuel ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (txtNouveauPassword.Password == "")
                {
                    MessageBox.Show("Veuillez entrer votre nouveau mot de passe", "Nouveau mot de passe ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (txtNouveauPasswordConfirm.Password == "")
                {
                    MessageBox.Show("Veuillez confirmer le nouveau mot de passe", "Confirmation nouveau mot de passe ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    if (txtPasswordAncien.Password != utilisateur.MPU)
                    {
                        MessageBox.Show("Votre ancien mot de passe ne correspond pas", "Ancien mot de passe erroné ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                    else if (txtNouveauPassword.Password != txtNouveauPasswordConfirm.Password)
                    {
                        MessageBox.Show("Les nouveaux mots de passe que vous avez saisis ne sont pas identiques", "Les nouveaux mots de passe ne sont pas identiques ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                    else
                    {
                        UTILISATEUR user = vsomAcc.UpdatePassword(utilisateur.IdU, txtNouveauPassword.Password);

                        utilisateur = user;
                        administrationPanel.utilisateur = user;

                        MessageBox.Show("Votre mot de passe a été mis à jour avec succès", "Mot de passe mis à jour !", MessageBoxButton.OK, MessageBoxImage.Information);
                        this.Close();
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
    }
}
