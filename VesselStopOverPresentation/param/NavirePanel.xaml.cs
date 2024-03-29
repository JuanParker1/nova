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
using System.Windows.Navigation;
using System.Windows.Shapes;
using VesselStopOverData;

namespace VesselStopOverPresentation
{
    /// <summary>
    /// Logique d'interaction pour NavirePanel.xaml
    /// </summary>
    public partial class NavirePanel : DockPanel
    {

        private UTILISATEUR utilisateur;
        private List<OPERATION> operationsUser;
        public List<NAVIRE> navires { get; set; }
        private VsomParameters vsp = new VsomParameters();
        public NavirePanel(UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;
                cbFiltres.SelectedIndex = 0;
                listRechercher.SelectedIndex = 0;

                utilisateur = user;
                operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        private void cbFiltres_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();
                VsomParameters vsp = new VsomParameters();
                if (cbFiltres.SelectedIndex == 0)
                {
                    navires = vsp.GetNaviresActifs();
                    dataGrid.ItemsSource = navires;
                    lblStatut.Content = navires.Count + " Navire(s)";
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

        private void txtRechercher_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();
                VsomParameters vsp = new VsomParameters();
                if (e.Key == Key.Return && listRechercher.SelectedItem != null)
                {
                    if (listRechercher.SelectedIndex == 0)
                    {
                        navires = vsp.GetNaviresByCodeNav(txtRechercher.Text.Trim());
                        dataGrid.ItemsSource = navires;
                        lblStatut.Content = navires.Count + " Navire(s) trouvé(s)";
                    }
                    else if (listRechercher.SelectedIndex == 1)
                    {
                        navires = vsp.GetNaviresByNomNav(txtRechercher.Text.Trim());
                        dataGrid.ItemsSource = navires;
                        lblStatut.Content = navires.Count + " Navire(s) trouvé(s)";
                    }
                }
                else if (e.Key == Key.Escape)
                {
                    txtRechercher.Text = null;
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

        private void btnNouveau_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (operationsUser.Where(op => op.NomOp == "Navire : Enregistrement d'un nouvel élément").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
                {
                    MessageBox.Show("Vous n'avez pas les droits nécessaires pour créer un nouveau navire. Veuillez contacter un administrateur", "Droits insuffisants !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    NavireForm navireForm = new NavireForm(this, utilisateur);
                    navireForm.Title = "Nouveau : Navire";
                    navireForm.Show();
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

        private void dataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (dataGrid.SelectedIndex != -1)
                {
                    NavireForm navireForm = new NavireForm(this, utilisateur);
                    NAVIRE n = (NAVIRE)dataGrid.SelectedItem;
                    navireForm.navire = n;
                    navireForm.txtCode.Text = n.CodeNav;
                    navireForm.txtLibelle.Text = n.NomNav;
                    navireForm.cbArmateur.SelectedItem = n.ARMATEUR.NomArm;
                    navireForm.Title = "Navire : " + n.NomNav;
                    navireForm.lblStatut.Content = n.StatutNav == "A" ? "Actif" : "Inactif";
                    //HA 13juin16 ajout code radio navire
                    navireForm.txtCodeRadio.Text = n.CodeRadio;

                    navireForm.Show();
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

        private void btnAnnulerRecherche_Click(object sender, RoutedEventArgs e)
        {
            txtRechercher.Text = null;
        }
    }
}
