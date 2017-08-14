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
    /// Logique d'interaction pour PortForm.xaml
    /// </summary>
    public partial class PortForm : Window
    {
        private PortPanel portPanel;
        public PORT port { get; set; }

        private UTILISATEUR utilisateur;
        private List<OPERATION> operationsUser;
        private VsomParameters vsp = new VsomParameters();
        public PortForm(PortPanel panel, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;
                portPanel = panel;

                port = new PORT();

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

        private void btnEnregistrer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                VsomConfig vsomAcc = new VsomConfig();
               // VsomParameters vsp = new VsomParameters();
                if (operationsUser.Where(op => op.NomOp == "Port : Enregistrement d'un nouvel élément").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
                {
                    MessageBox.Show("Vous n'avez pas les droits nécessaires pour créer les données de base sur les ports. Veuillez contacter un administrateur", "Droits insuffisants !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (txtCode.Text.Trim() == "")
                {
                    MessageBox.Show("Veuillez saisir le code du port", "Code port ?", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (txtLibelle.Text.Trim() == "")
                {
                    MessageBox.Show("Veuillez saisir le nom du port", "Nom port ?", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (txtPays.Text.Trim() == "")
                {
                    MessageBox.Show("Veuillez sélectionner le pays correspondant", "Pays port ?", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    PORT p = vsomAcc.InsertOrUpdatePort(txtCode.Text, txtLibelle.Text, txtPays.Text);
                    portPanel.ports = vsp.GetPortsOrderByCode();
                    portPanel.dataGrid.ItemsSource = null;
                    portPanel.dataGrid.ItemsSource = portPanel.ports;
                    portPanel.lblStatut.Content = portPanel.ports.Count + " port(s)";
                    MessageBox.Show("Enregistrement effectué avec succès.", "Enregistrement effectué !", MessageBoxButton.OK, MessageBoxImage.Information);
                    txtCode.Text = "";
                    txtLibelle.Text = "";
                    txtPays.Text = "";
                    port.CodePort = "";
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
