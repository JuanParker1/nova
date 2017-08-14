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
    /// Logique d'interaction pour MAJChassisForm.xaml
    /// </summary>
    public partial class MAJChassisForm : Window
    {

        private VehiculeForm vehForm;
        private List<OPERATION> operationsUser;
        private UTILISATEUR utilisateur;

        private VEHICULE vehicule;

        private FormLoader formLoader;
        private VsomParameters vsp = new VsomParameters();
        public MAJChassisForm(VehiculeForm form, VEHICULE veh, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

                utilisateur = user;
                operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);

                vehForm = form;

                vehicule = veh;

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

                if (txtNouveauChassis.Text.Trim() == "")
                {
                    MessageBox.Show("Veuillez saisir le nouveau numéro de chassis pour ce véhicule", "Nouveau chassis ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    VEHICULE veh = vsomAcc.UpdateNumChassis(vehicule.IdVeh, txtNouveauChassis.Text, utilisateur.IdU);

                    formLoader.LoadVehiculeForm(vehForm, veh);

                    MessageBox.Show("Le N° de chassis a été mis à jour avec succès", "N° de chassis mis à jour !", MessageBoxButton.OK, MessageBoxImage.Information);

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
