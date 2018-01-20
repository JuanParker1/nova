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
    /// Logique d'interaction pour CalculerSejourVehiculeForm.xaml
    /// </summary>
    public partial class CalculerSejourVehiculeForm : Window
    {

        private VehiculeForm vehForm;

        private List<OPERATION> operationsUser;
        private UTILISATEUR utilisateur;

        private FormLoader formLoader;
        //private VsomParameters vsp = new VsomParameters();
        private VSOMAccessors vsomAcc;
        string _year;
        public CalculerSejourVehiculeForm(VehiculeForm form, UTILISATEUR user)
        {
            try
            {
                 vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

                utilisateur = user;
                operationsUser = vsomAcc.GetOperationsUtilisateur(utilisateur.IdU);

                vehForm = form;
                _year = "2018";
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

        public CalculerSejourVehiculeForm(VehiculeForm form, UTILISATEUR user , string annee)
        {

            try
            {
                vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

                utilisateur = user;
                operationsUser = vsomAcc.GetOperationsUtilisateur(utilisateur.IdU);

                vehForm = form;
                _year = "2017";
                formLoader = new FormLoader(utilisateur);
                txtDateFinSejour.SelectedDate = DateTime.Parse("31/12/2017");
                txtDateFinSejour.IsEnabled = false;
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        private void btnCalculer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
               vsomAcc = new VSOMAccessors();

                VEHICULE v = vsomAcc.GetVehiculeByIdVeh(Convert.ToInt32(vehForm.txtIdChassis.Text));

                if (!txtDateFinSejour.SelectedDate.HasValue)
                {
                    MessageBox.Show("Veuillez entrer une date de fin de séjour", "Date de fin de séjour ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if(txtDateFinSejour.SelectedDate.Value < v.ESCALE.DRAEsc)
                {
                    MessageBox.Show("La date de fin de séjour de ce véhicule ne peut pas être inférieure ou égale à la date réelle d'arrivée du navire", "Date de fin de séjour incohérente", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
               /* else if (txtDateFinSejour.SelectedDate.Value < DateTime.Now.Date && utilisateur.LU != "Admin")
                {
                    MessageBox.Show("La date de fin de séjour saisie est inférieure à la date du jour.", "Date de fin de séjour incohérente", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }*/
                else if (txtDateFinSejour.SelectedDate.Value < v.FSVeh.Value.Date)
                {
                    MessageBox.Show("Le séjour a déjà été calculé à cette date.", "Date de fin de séjour incohérente", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    VEHICULE veh = null;
                    if (_year == "2018")
                    {
                         veh = vsomAcc.CalculerSejourVehicule(Convert.ToInt32(vehForm.txtIdChassis.Text), txtDateFinSejour.SelectedDate.Value, utilisateur.IdU);
                    }
                    else
                    {
                         veh = vsomAcc.CalculerSejourVehicule_2017(Convert.ToInt32(vehForm.txtIdChassis.Text), txtDateFinSejour.SelectedDate.Value, utilisateur.IdU);
                    
                    }
                    formLoader.LoadVehiculeForm(vehForm, veh);

                    MessageBox.Show("L'opération de calcul du séjour s'est déroulé avec succès, consultez le journal des éléments de facturation", "Calcul de séjour réussie !", MessageBoxButton.OK, MessageBoxImage.Information);

                    this.Close();
                }
            }
            catch (FacturationException vehex)
            {
                MessageBox.Show(vehex.Message, "Véhicule non identifié !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
    }
}
