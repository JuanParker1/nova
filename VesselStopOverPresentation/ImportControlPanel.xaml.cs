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
    /// Logique d'interaction pour ImportControlPanel.xaml
    /// </summary>
    public partial class ImportControlPanel : StackPanel
    {
        private VesselStopOverWindow vesselWindow;
        private UTILISATEUR utilisateur;
        private List<OPERATION> operationsUser;

        private VSOMAccessors vsomAcc = new VSOMAccessors();
        private VsomParameters vsp = new VsomParameters();
        public ImportControlPanel(VesselStopOverWindow window, UTILISATEUR user)
        {
            InitializeComponent();
            vesselWindow = window;
            utilisateur = user;
            operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);
            if (operationsUser.Where(op => op.NomOp == "Manifeste : Visualisation des éléments existants").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
            {
                btnManifeste.Visibility = System.Windows.Visibility.Collapsed;
            }
            if (operationsUser.Where(op => op.NomOp == "Connaissement : Visualisation des éléments existants").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
            {
                btnConnaissement.Visibility = System.Windows.Visibility.Collapsed;
            }
            if (operationsUser.Where(op => op.NomOp == "Véhicule : Visualisation mes éléments existants" || op.NomOp == "Véhicule : Visualisation les éléments existants de mon point" || op.NomOp == "Véhicule : Visualisation tous les éléments existants").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
            {
                btnVehicule.Visibility = System.Windows.Visibility.Collapsed;
            }
            if (operationsUser.Where(op => op.NomOp == "Conteneur : Visualisation mes éléments existants" || op.NomOp == "Conteneur : Visualisation les éléments existants de mon point" || op.NomOp == "Conteneur : Visualisation tous les éléments existants").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
            {
                btnConteneur.Visibility = System.Windows.Visibility.Collapsed;
            }
            if (operationsUser.Where(op => op.NomOp == "Mafi : Visualisation mes éléments existants" || op.NomOp == "Mafi : Visualisation les éléments existants de mon point" || op.NomOp == "Conteneur : Visualisation tous les éléments existants").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
            {
                btnMafi.Visibility = System.Windows.Visibility.Collapsed;
            }
            if (operationsUser.Where(op => op.NomOp == "General cargo : Visualisation des éléments existants").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
            {
                btnGeneralCargo.Visibility = System.Windows.Visibility.Collapsed;
            }
            if (operationsUser.Where(op => op.NomOp == "Cubage : Visualisation des éléments existants").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
            {
                btnCubage.Visibility = System.Windows.Visibility.Collapsed;
            }
            if (operationsUser.Where(op => op.NomOp == "Bon à enlever : Visualisation des éléments existants").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
            {
                btnBAE.Visibility = System.Windows.Visibility.Collapsed;
            }
            if (operationsUser.Where(op => op.NomOp == "Demande de livraison : Visualisation des éléments existants").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
            {
                btnDemandeLivraison.Visibility = System.Windows.Visibility.Collapsed;
            }
            if (operationsUser.Where(op => op.NomOp == "Bon de sortie : Visualisation des éléments existants").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
            {
                btnBonSortie.Visibility = System.Windows.Visibility.Collapsed;
            }
            if (operationsUser.Where(op => op.NomOp == "Demande de visite : Visualisation des éléments existants").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
            {
                btnDemandeVisite.Visibility = System.Windows.Visibility.Collapsed;
            }
            if (operationsUser.Where(op => op.NomOp == "Demande d'extention de franchise : Visualisation des éléments existants").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
            {
                btnDemandeExtension.Visibility = System.Windows.Visibility.Collapsed;
            }
            if (operationsUser.Where(op => op.NomOp == "Demande de restitution de caution : Visualisation des éléments existants").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
            {
                btnRestCaution.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private void btnEscale_Click(object sender, RoutedEventArgs e)
        {
            EscalePanel escalePanel = new EscalePanel(utilisateur);
            vesselWindow.mainPanel.Header = "Escale";
            vesselWindow.mainPanel.Content = null;
            vesselWindow.mainPanel.Content = escalePanel;
        }

        private void btnManifeste_Click(object sender, RoutedEventArgs e)
        {
            ManifestePanel manifPanel = new ManifestePanel(utilisateur);
            vesselWindow.mainPanel.Header = "Manifeste";
            vesselWindow.mainPanel.Content = null;
            vesselWindow.mainPanel.Content = manifPanel;
        }

        private void btnConnaissement_Click(object sender, RoutedEventArgs e)
        {
            ConnaissementPanel conPanel = new ConnaissementPanel(utilisateur);
            vesselWindow.mainPanel.Header = "Connaissement";
            vesselWindow.mainPanel.Content = null;
            vesselWindow.mainPanel.Content = conPanel;
        }

        private void btnVehicule_Click(object sender, RoutedEventArgs e)
        {
            VehiculePanel vehPanel = new VehiculePanel(utilisateur);
            vesselWindow.mainPanel.Header = "Vehicule";
            vesselWindow.mainPanel.Content = null;
            vesselWindow.mainPanel.Content = vehPanel;
        }

        private void btnConteneur_Click(object sender, RoutedEventArgs e)
        {
            ConteneurPanel contPanel = new ConteneurPanel(utilisateur);
            vesselWindow.mainPanel.Header = "Conteneur";
            vesselWindow.mainPanel.Content = null;
            vesselWindow.mainPanel.Content = contPanel;
        }

        private void btnMafi_Click(object sender, RoutedEventArgs e)
        {
            MafiPanel mafiPanel = new MafiPanel(utilisateur);
            vesselWindow.mainPanel.Header = "Mafi";
            vesselWindow.mainPanel.Content = null;
            vesselWindow.mainPanel.Content = mafiPanel;
        }

        private void btnGeneralCargo_Click(object sender, RoutedEventArgs e)
        {
            ConventionnelPanel convPanel = new ConventionnelPanel(utilisateur);
            vesselWindow.mainPanel.Header = "Conventionnel";
            vesselWindow.mainPanel.Content = null;
            vesselWindow.mainPanel.Content = convPanel;
        }

        private void btnCubage_Click(object sender, RoutedEventArgs e)
        {
            CubagePanel cubPanel = new CubagePanel(utilisateur);
            vesselWindow.mainPanel.Header = "Cubage";
            vesselWindow.mainPanel.Content = null;
            vesselWindow.mainPanel.Content = cubPanel;
        }

        private void btnBAE_Click(object sender, RoutedEventArgs e)
        {
            BonEnleverPanel bAEPanel = new BonEnleverPanel(utilisateur);
            vesselWindow.mainPanel.Header = "Bon à enlever";
            vesselWindow.mainPanel.Content = null;
            vesselWindow.mainPanel.Content = bAEPanel;
        }

        private void btnDemandeLivraison_Click(object sender, RoutedEventArgs e)
        {
            DemandeLivraisonPanel demandeLivraisonPanel = new DemandeLivraisonPanel(utilisateur);
            vesselWindow.mainPanel.Header = "Demande de livraison";
            vesselWindow.mainPanel.Content = null;
            vesselWindow.mainPanel.Content = demandeLivraisonPanel;
        }

        private void btnBonSortie_Click(object sender, RoutedEventArgs e)
        {
            BonSortiePanel bonSortiePanel = new BonSortiePanel(utilisateur);
            vesselWindow.mainPanel.Header = "Bon de sortie";
            vesselWindow.mainPanel.Content = null;
            vesselWindow.mainPanel.Content = bonSortiePanel;
        }

        private void btnDemandeVisite_Click(object sender, RoutedEventArgs e)
        {
            DemandeVisitePanel demandeVisitePanel = new DemandeVisitePanel(utilisateur);
            vesselWindow.mainPanel.Header = "Demande de visite";
            vesselWindow.mainPanel.Content = null;
            vesselWindow.mainPanel.Content = demandeVisitePanel;
        }

        private void btnDemandeExtension_Click(object sender, RoutedEventArgs e)
        {
            ExtensionFranchisePanel extensionFranchisePanel = new ExtensionFranchisePanel(utilisateur);
            vesselWindow.mainPanel.Header = "Extension de séjour";
            vesselWindow.mainPanel.Content = null;
            vesselWindow.mainPanel.Content = extensionFranchisePanel;
        }

        private void btnsinistre_Click(object sender, RoutedEventArgs e)
        {
            SinistrePanel sinPanel = new SinistrePanel(utilisateur);
            vesselWindow.mainPanel.Header = "Sinistre";
            vesselWindow.mainPanel.Content = null;
            vesselWindow.mainPanel.Content = sinPanel;
        }

        private void btnRestCaution_Click(object sender, RoutedEventArgs e)
        {
            DemandeRestitutionCautionPanel restCautionPanel = new DemandeRestitutionCautionPanel(utilisateur);
            vesselWindow.mainPanel.Header = "Restitution de caution";
            vesselWindow.mainPanel.Content = null;
            vesselWindow.mainPanel.Content = restCautionPanel;
        }
    }
}
