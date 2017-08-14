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
    /// Logique d'interaction pour ListVehiculeForm.xaml
    /// </summary>
    public partial class ListVehiculeForm : Window
    {
        public List<VEHICULE> vehicules { get; set; }

        private VehiculeForm vehForm;
        private SinistreForm sinForm;
        private List<OPERATION> operationsUser;
        private UTILISATEUR utilisateur;

        private FormLoader formLoader;
        private VsomParameters vsp = new VsomParameters();
        public ListVehiculeForm(VehiculeForm form, List<VEHICULE> listVehicules, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

                vehicules = listVehicules;
                dataGrid.ItemsSource = vehicules;
                
                utilisateur = user;
                operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);

                vehForm = form;

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

        public ListVehiculeForm(SinistreForm form, List<VEHICULE> listVehicules, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

                vehicules = listVehicules;
                dataGrid.ItemsSource = vehicules;

                utilisateur = user;
                operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);

                sinForm = form;

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
                if (dataGrid.SelectedIndex != -1)
                {
                    if (vehForm != null)
                    {
                        formLoader.LoadVehiculeForm(vehForm, (VEHICULE)dataGrid.SelectedItem);
                    }
                    else if (sinForm != null)
                    {
                        formLoader.LoadVehiculeForm(sinForm, (VEHICULE)dataGrid.SelectedItem);
                    }
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
                if (dataGrid.SelectedIndex != -1 && dataGrid.SelectedItems.Count == 1)
                {
                    if (vehForm != null)
                    {
                        formLoader.LoadVehiculeForm(vehForm, (VEHICULE)dataGrid.SelectedItem);
                    }
                    else if (sinForm != null)
                    {
                        formLoader.LoadVehiculeForm(sinForm, (VEHICULE)dataGrid.SelectedItem);
                    }
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
