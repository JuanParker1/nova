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
    /// Logique d'interaction pour ParcAutoControlPanel.xaml
    /// </summary>
    public partial class ParcAutoControlPanel : StackPanel
    {
        private VesselStopOverWindow vesselWindow;
        private UTILISATEUR utilisateur;
        private List<OPERATION> operationsUser;

        private VSOMAccessors vsomAcc ;//= new VSOMAccessors();
        private VsomParameters vsp;// = new VsomParameters();
        public ParcAutoControlPanel(VesselStopOverWindow window, UTILISATEUR user)
        {
            InitializeComponent();
            //using (var ctx = new VSOMClassesDataContext())
            //{
                vsp = new VsomParameters();
                vesselWindow = window;
                utilisateur = user;
                operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);
                if (operationsUser.Where(op => op.NomOp == "Connaissement : Visualisation des éléments existants").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
                {
                    btnConnaissement.Visibility = System.Windows.Visibility.Collapsed;
                }
                if (operationsUser.Where(op => op.NomOp == "Véhicule : Visualisation mes éléments existants" || op.NomOp == "Véhicule : Visualisation les éléments existants de mon point" || op.NomOp == "Véhicule : Visualisation tous les éléments existants").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
                {
                    btnVehicule.Visibility = System.Windows.Visibility.Collapsed;
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

                if (operationsUser.Where(op => op.NomOp == "Quotations : Manipulation").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
                {
                    btnQuotation.Visibility = System.Windows.Visibility.Collapsed;
                }

                if (user.IdAcc != 1)
                {
                    btnConnaissement.Visibility = System.Windows.Visibility.Collapsed;
                    btnEmplacement.Visibility = System.Windows.Visibility.Collapsed;
                }
            //}
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

        private void btnEmplacement_Click(object sender, RoutedEventArgs e)
        {
            EmplacementPanel emplacementPanel = new EmplacementPanel(utilisateur);
            vesselWindow.mainPanel.Header = "Emplacement";
            vesselWindow.mainPanel.Content = null;
            vesselWindow.mainPanel.Content = emplacementPanel;
        }

        private void btnRepportBAE_Click_1(object sender, RoutedEventArgs e)
        {
            ParcAutoBAERepport pnl = new ParcAutoBAERepport(utilisateur);
            vesselWindow.mainPanel.Header = "Report BAE ";
            vesselWindow.mainPanel.Content = null;
            vesselWindow.mainPanel.Content = pnl;
        }

        private void btnQuotation_Click_1(object sender, RoutedEventArgs e)
        {
            Remote.QuotationsPanel emplacementPanel = new Remote.QuotationsPanel(utilisateur);
            vesselWindow.mainPanel.Header = "Quotations";
            vesselWindow.mainPanel.Content = null;
            vesselWindow.mainPanel.Content = emplacementPanel;
        }
    }
}
