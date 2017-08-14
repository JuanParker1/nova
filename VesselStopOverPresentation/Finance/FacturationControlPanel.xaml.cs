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
using System.Windows.Navigation;
using System.Windows.Shapes;
using VesselStopOverData;

namespace VesselStopOverPresentation
{
    /// <summary>
    /// Logique d'interaction pour FacturationControlPanel.xaml
    /// </summary>
    public partial class FacturationControlPanel : StackPanel
    {
        private VesselStopOverWindow vesselWindow;
        private UTILISATEUR utilisateur;
        private List<OPERATION> operationsUser;

        private VSOMAccessors vsomAcc = new VSOMAccessors();
        VsomParameters vsp = new VsomParameters();

        public FacturationControlPanel(VesselStopOverWindow window, UTILISATEUR user)
        {
            InitializeComponent();
            vesselWindow = window;
            utilisateur = user;
            operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);
            if (operationsUser.Where(op => op.NomOp == "Proforma : Visualisation des éléments existants" || op.NomOp == "Proforma : Validation d'un élément").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
            {
                btnProforma.Visibility = System.Windows.Visibility.Collapsed;
            }
            if (operationsUser.Where(op => op.NomOp == "Facture : Visualisation des éléments existants" || op.NomOp == "Facture : Validation d'un élément").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
            {
                btnFacture.Visibility = System.Windows.Visibility.Collapsed;
            }
            //if (operationsUser.Where(op => op.NomOp == "Mise à jour des prix DIT").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
            //{
            //    btnFactureDIT.Visibility = System.Windows.Visibility.Collapsed;
            //}
            if (operationsUser.Where(op => op.NomOp == "Avoir : Visualisation des éléments existants" || op.NomOp == "Avoir : Validation d'un élément").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
            {
                btnAvoir.Visibility = System.Windows.Visibility.Collapsed;
            }
            if (operationsUser.Where(op => op.NomOp == "Paiement : Visualisation des éléments existants").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
            {
                btnPaiement.Visibility = System.Windows.Visibility.Collapsed;
            }
            if (operationsUser.Where(op => op.NomOp == "Demande de réduction : Enregistrement d'un nouvel élément").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
            {
                btnReduction.Visibility = System.Windows.Visibility.Collapsed;
            }
            if (operationsUser.Where(op => op.NomOp == "Facture spot: Enregistrement d'un nouvel élément").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
            {
                btnfactureSpot.Visibility = System.Windows.Visibility.Collapsed;
            }
            if (operationsUser.Where(op => op.NomOp == "Facture spot: Visualisation des éléments existants").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
            {
                btnfactureSpot.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private void btnProforma_Click(object sender, RoutedEventArgs e)
        {
            ProformaPanel profPanel = new ProformaPanel(utilisateur);
            vesselWindow.mainPanel.Header = "Proforma";
            vesselWindow.mainPanel.Content = null;
            vesselWindow.mainPanel.Content = profPanel;
        }

        private void btnFacture_Click(object sender, RoutedEventArgs e)
        {
            FacturePanel factPanel = new FacturePanel(utilisateur);
            vesselWindow.mainPanel.Header = "Facture";
            vesselWindow.mainPanel.Content = null;
            vesselWindow.mainPanel.Content = factPanel;
        }

        private void btnAvoir_Click(object sender, RoutedEventArgs e)
        {
            AvoirPanel avoirPanel = new AvoirPanel(utilisateur);
            vesselWindow.mainPanel.Header = "Avoir";
            vesselWindow.mainPanel.Content = null;
            vesselWindow.mainPanel.Content = avoirPanel;
        }

        private void btnPaiement_Click(object sender, RoutedEventArgs e)
        {
            PaiementPanel payPanel = new PaiementPanel(utilisateur);
            vesselWindow.mainPanel.Header = "Paiement";
            vesselWindow.mainPanel.Content = null;
            vesselWindow.mainPanel.Content = payPanel;
        }

        private void btnReduction_Click(object sender, RoutedEventArgs e)
        {
            DemandeReductionPanel reducPanel = new DemandeReductionPanel(utilisateur);
            vesselWindow.mainPanel.Header = "Demande de réduction";
            vesselWindow.mainPanel.Content = null;
            vesselWindow.mainPanel.Content = reducPanel;
        }

        private void btnFactureDIT_Click(object sender, RoutedEventArgs e)
        {
            FactureDITPanel factDITPanel = new FactureDITPanel(utilisateur);
            vesselWindow.mainPanel.Header = "Facture DIT";
            vesselWindow.mainPanel.Content = null;
            vesselWindow.mainPanel.Content = factDITPanel;
        }
 

        private void btnfactureSpot_Click_1(object sender, RoutedEventArgs e)
        {
            FactureSpotPanel fact = new FactureSpotPanel(utilisateur);
            vesselWindow.mainPanel.Header = "Facture Spot";
            vesselWindow.mainPanel.Content = null;
            vesselWindow.mainPanel.Content = fact;
        }
    }
}
