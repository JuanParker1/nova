﻿using System;
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
    /// Logique d'interaction pour AjoutVehiculeCubageForm.xaml
    /// </summary>
    public partial class AjoutVehiculeCubageForm : Window
    {
        public List<VehiculeCubage> vehicules { get; set; }

        private CubageForm cubForm;

        private List<OPERATION> operationsUser;
        private UTILISATEUR utilisateur;

        private FormLoader formLoader;
        //private VsomParameters vsp = new VsomParameters();
        private VSOMAccessors vsomAcc;
        public AjoutVehiculeCubageForm(CubageForm form, List<VehiculeCubage> listVehsCub, UTILISATEUR user)
        {
            try
            {
                 vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

                vehicules = listVehsCub;
                dataGrid.ItemsSource = vehicules;
                
                utilisateur = user;
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

        private void btnAjouter_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //VSOMAccessors vsomAcc = new VSOMAccessors();
                //VsomMarchal vsomAcc = new VsomMarchal();

                if (dataGrid.SelectedItems.Count == 0)
                {
                    MessageBox.Show("Veuillez sélectionner au moins un véhicule", "Véhicules", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    CUBAGE cub = vsomAcc.AjoutVehiculesCubage(vsomAcc.GetCubageByIdCub(Convert.ToInt32(cubForm.cbIdCub.Text)).IdCubage, dataGrid.SelectedItems.OfType<VehiculeCubage>().ToList<VehiculeCubage>(), utilisateur.IdU);

                    formLoader.LoadCubageForm(cubForm, cub);

                    MessageBox.Show("Véhicules ajoutés avec succès", "Véhicules ajoutés !", MessageBoxButton.OK, MessageBoxImage.Information);

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
