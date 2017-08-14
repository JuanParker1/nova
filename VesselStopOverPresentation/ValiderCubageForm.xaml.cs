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
    /// Logique d'interaction pour ValiderCubageForm.xaml
    /// </summary>
    public partial class ValiderCubageForm : Window
    {

        private CubageForm cubForm;
        private List<OPERATION> operationsUser;
        private UTILISATEUR utilisateur;
        private int idVehicule;

        private FormLoader formLoader;
        //private VsomParameters vsp = new VsomParameters();
        private VSOMAccessors vsomAcc;
        public ValiderCubageForm(CubageForm form, int idVeh, UTILISATEUR user)
        {
            try
            {
                 vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

                utilisateur = user;
                idVehicule = idVeh;
                operationsUser = vsomAcc.GetOperationsUtilisateur(utilisateur.IdU);

                cubForm = form;

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

        private void btnValider_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                vsomAcc = new VSOMAccessors();
                VEHICULE veh = null;
                
                if (radioCube.IsChecked == false && radioAncien.IsChecked == false)
                {
                    MessageBox.Show("Veuillez cocher le volume à considérer", "Volume à considérer ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (txtVolCube.Text.Trim() == "")
                {
                    MessageBox.Show("Veuillez renseigner le volume cubé à considérer", "Volume cubé ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    txtVolCube.Text = txtVolCube.Text.Replace(" ", "").Replace(".", ",");
                    if (radioAncien.IsChecked == true)
                    {
                        veh = vsomAcc.ValiderCubageVehicule(idVehicule, new TextRange(txtObservations.Document.ContentStart, txtObservations.Document.ContentEnd).Text, Convert.ToDouble(txtVolAncien.Text.Replace(".", ",")), utilisateur.IdU);
                    }
                    else if (radioCube.IsChecked == true)
                    {
                        veh = vsomAcc.ValiderCubageVehicule(idVehicule, new TextRange(txtObservations.Document.ContentStart, txtObservations.Document.ContentEnd).Text, Convert.ToDouble(txtVolCube.Text), utilisateur.IdU);
                    }

                    formLoader.LoadCubageForm(cubForm, vsomAcc.GetCubageByIdCub(Convert.ToInt32(cubForm.cbIdCub.Text)));

                    MessageBox.Show("Validation du cubage du véhicule effectuée avec succès.", "Cubage validé !", MessageBoxButton.OK, MessageBoxImage.Information);

                    this.Close();
                }
            }
            catch (HabilitationException ex)
            {
                MessageBox.Show(ex.Message, "Droits insuffisants !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        private void radioCube_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                txtVolCube.IsReadOnly = false;
                txtVolCube.Background = Brushes.White;
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        private void txtVolCube_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9,.]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void radioAncien_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                txtVolCube.IsReadOnly = true;
                txtVolCube.Background = Brushes.AntiqueWhite;
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
