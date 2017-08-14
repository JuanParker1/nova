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
    /// Logique d'interaction pour CalculerSejourConventionnelForm.xaml
    /// </summary>
    public partial class CalculerSejourConventionnelForm : Window
    {

        private ConventionnelForm convForm;

        private List<OPERATION> operationsUser;
        private UTILISATEUR utilisateur;

        private FormLoader formLoader;
        //private VsomParameters vsp = new VsomParameters();
        private VSOMAccessors vsomAcc;
        public CalculerSejourConventionnelForm(ConventionnelForm form, UTILISATEUR user)
        {
            try
            {
                 vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

                utilisateur = user;
                operationsUser = vsomAcc.GetOperationsUtilisateur(utilisateur.IdU);

                convForm = form;

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

        private void btnCalculer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                vsomAcc = new VSOMAccessors();

                if (!txtDateFinSejour.SelectedDate.HasValue)
                {
                    MessageBox.Show("Veuillez entrer une date de fin de séjour", "Date de fin de séjour ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if(txtDateFinSejour.SelectedDate.Value < vsomAcc.GetConventionnelByIdGC(Convert.ToInt32(convForm.txtIdGC.Text)).ESCALE.DRAEsc)
                {
                    MessageBox.Show("La date de fin de séjour de ce véhicule ne peut pas être inférieure ou égale à la date réelle d'arrivée du navire", "Date de fin de séjour incohérente", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (txtDateFinSejour.SelectedDate.Value < DateTime.Now.Date && utilisateur.LU != "Admin")
                {
                    MessageBox.Show("La date de fin de séjour saisie est inférieure à la date du jour.", "Date de fin de séjour incohérente", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    CONVENTIONNEL conv = vsomAcc.CalculerSejourConventionnel(Convert.ToInt32(convForm.txtIdGC.Text), txtDateFinSejour.SelectedDate.Value, utilisateur.IdU);

                    formLoader.LoadConventionnelForm(convForm, conv);

                    MessageBox.Show("L'opération de facturation du séjour s'est déroulé avec succès, consultez le journal des éléments de facturation", "Facturation de séjour réussie !", MessageBoxButton.OK, MessageBoxImage.Information);

                    this.Close();
                }
            }
            catch (FacturationException vehex)
            {
                MessageBox.Show(vehex.Message, "General Cargo identifié !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
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
