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
    /// Logique d'interaction pour CalculerStationnementMafiForm.xaml
    /// </summary>
    public partial class CalculerStationnementMafiForm : Window
    {

        private MafiForm mafiForm;
        private List<OPERATION> operationsUser;
        private UTILISATEUR utilisateur;

        private FormLoader formLoader;
        //private VsomParameters vsp = new VsomParameters();
        private VSOMAccessors vsomAcc;
        public CalculerStationnementMafiForm(MafiForm form, UTILISATEUR user)
        {
            try
            {
                 vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

                utilisateur = user;
                operationsUser = vsomAcc.GetOperationsUtilisateur(utilisateur.IdU);

                mafiForm = form;

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

                MAFI m = vsomAcc.GetMafiImportByIdMafi(Convert.ToInt32(mafiForm.txtIdMafi.Text));

                if (!txtDateFinStationnement.SelectedDate.HasValue)
                {
                    MessageBox.Show("Veuillez entrer une date de fin de stationnement", "Date de fin de stationnement ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (txtDateFinStationnement.SelectedDate.Value < vsomAcc.GetMafiImportByIdMafi(Convert.ToInt32(mafiForm.txtIdMafi.Text)).ESCALE.DRAEsc)
                {
                    MessageBox.Show("La date de fin de stationnement de ce mafi ne peut pas être inférieure ou égale à la date réelle d'arrivée du navire", "Date de fin de stationnement incohérente", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (txtDateFinStationnement.SelectedDate.Value < DateTime.Now.Date && utilisateur.LU != "Admin")
                {
                    MessageBox.Show("La date saisie pour le calcul des surestaries sur ce conteneur est inférieure à la date du jour.", "Date de fin du surestaries incohérente", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (txtDateFinStationnement.SelectedDate.Value < m.FSMafi.Value.Date)
                {
                    MessageBox.Show("Les surestaries et stationnement ont déjà été calculés à cette date.", "Date de fin incohérente", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    MAFI mafi = vsomAcc.CalculerStationnementMafi(Convert.ToInt32(mafiForm.txtIdMafi.Text), txtDateFinStationnement.SelectedDate.Value, utilisateur.IdU);

                    formLoader.LoadMafiForm(mafiForm, mafi);

                    MessageBox.Show("L'opération de facturation du stationnement s'est déroulée avec succès, consultez le journal des éléments de facturation", "Facturation du stationnement réussie !", MessageBoxButton.OK, MessageBoxImage.Information);

                    this.Close();
                }
            }
            catch (FacturationException gpex)
            {
                MessageBox.Show(gpex.Message, "Rôle gestionnaire de parc non défini !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            catch (IdentificationException vehex)
            {
                MessageBox.Show(vehex.Message, "Conteneur non identifié !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
            }
        }
    }
}
