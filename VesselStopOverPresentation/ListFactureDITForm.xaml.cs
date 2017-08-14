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
    /// Logique d'interaction pour ListFactureDITForm.xaml
    /// </summary>
    public partial class ListFactureDITForm : Window
    {
        public List<FACTURE_DIT> factures { get; set; }

        private FactureDITForm factForm;
        private List<OPERATION> operationsUser;
        private UTILISATEUR utilisateur;

        private FormLoader formLoader;
        private VsomParameters vsp = new VsomParameters();
        public ListFactureDITForm(FactureDITForm form, List<FACTURE_DIT> listFactures, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

                factures = listFactures;
                
                utilisateur = user;
                operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);

                factForm = form;

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

        private void dataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                if (dataGrid.SelectedIndex != -1)
                {
                    FACTURE_DIT fact = vsp.GetFacturesDITByIdFact(((FACTURE_DIT)dataGrid.SelectedItem).IdFactDIT);
                    formLoader.LoadFactureDITForm(factForm, fact);
                    this.Close();
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

        private void btnSelectionner_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                if (dataGrid.SelectedIndex != -1 && dataGrid.SelectedItems.Count == 1)
                {
                    formLoader.LoadFactureDITForm(factForm, vsp.GetFacturesDITByIdFact(((FACTURE_DIT)dataGrid.SelectedItem).IdFactDIT));
                    this.Close();
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
