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
    /// Logique d'interaction pour MAJNumGCForm.xaml
    /// </summary>
    public partial class MAJNumGCForm : Window
    {

        private ConventionnelForm convForm;
        private List<OPERATION> operationsUser;
        private UTILISATEUR utilisateur;

        private CONVENTIONNEL conventionnel;

        private FormLoader formLoader;
        private VsomParameters vsp = new VsomParameters();
        public MAJNumGCForm(ConventionnelForm form, CONVENTIONNEL conv, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

                utilisateur = user;
                operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);

                convForm = form;

                conventionnel = conv;

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

        private void btnMAJ_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //VSOMAccessors vsomAcc = new VSOMAccessors();
                VsomMarchal vsomAcc = new VsomMarchal();
                if (txtNouveauGC.Text.Trim() == "")
                {
                    MessageBox.Show("Veuillez saisir le nouveau numéro de general cargo pour ce conventionnel", "Nouveau N° GC ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    CONVENTIONNEL conv = vsomAcc.UpdateNumGC(conventionnel.IdGC, txtNouveauGC.Text, utilisateur.IdU);

                    formLoader.LoadConventionnelForm(convForm, conv);

                    MessageBox.Show("Le N° de general cargo a été mis à jour avec succès", "N° de GC mis à jour !", MessageBoxButton.OK, MessageBoxImage.Information);

                    this.Close();
                }
            }
            catch (EnregistrementInexistant ei)
            {
                MessageBox.Show(ei.Message, "Enregistrement inexistant", MessageBoxButton.OK, MessageBoxImage.Exclamation);
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
