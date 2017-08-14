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
    /// Logique d'interaction pour ReportingControlPanel.xaml
    /// </summary>
    public partial class ReportingControlPanel : StackPanel
    {
        public VesselStopOverWindow vesselWindow { get; set; }
        private UTILISATEUR utilisateur;
        private List<OPERATION> operationsUser;

        private VSOMAccessors vsomAcc = new VSOMAccessors();
        private VsomParameters vsp = new VsomParameters();
        public UTILISATEUR GetUser()
        {
            return utilisateur;
        }

        public ReportingControlPanel(VesselStopOverWindow window, UTILISATEUR user)
        {
            InitializeComponent();
            vesselWindow = window;
            utilisateur = user;
            operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);
            if (operationsUser.Where(op => op.NomOp == "Report : Encaissements").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
            {
                btnPaiement.Visibility = System.Windows.Visibility.Collapsed;
            }
            if (operationsUser.Where(op => op.NomOp == "Report : Elements DIT").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
            {
                btnElementsDIT.Visibility = System.Windows.Visibility.Collapsed;
            }
            if (operationsUser.Where(op => op.NomOp == "Report : Extract SEPBC").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
            {
                btnElementsSEPBC.Visibility = System.Windows.Visibility.Collapsed;
            }
            if (operationsUser.Where(op => op.NomOp == "Report : Extract CA Synthèse").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
            {
                btnExtractCA.Visibility = System.Windows.Visibility.Collapsed;
            }
            if (operationsUser.Where(op => op.NomOp == "Report : Extract Surestaries Détention").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
            {
                btnExtractArmateur.Visibility = System.Windows.Visibility.Collapsed;
            }
            if (operationsUser.Where(op => op.NomOp == "Report : Extract Douane").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
            {
                btnExtractDouane.Visibility = System.Windows.Visibility.Collapsed;
            }
            if (operationsUser.Where(op => op.NomOp == "Report : Extract CA Détaillé").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
            {
                btnExtractCADetail.Visibility = System.Windows.Visibility.Collapsed;
            }
            if (operationsUser.Where(op => op.NomOp == "Report : Factures DIT").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
            {
                btnFacturesDIT.Visibility = System.Windows.Visibility.Collapsed;
            }
            if (operationsUser.Where(op => op.NomOp == "Report : Synthèse par Acconier").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
            {
                btnSyntheseParAcconier.Visibility = System.Windows.Visibility.Collapsed;
            }
            if (operationsUser.Where(op => op.NomOp == "Report : Synthèse SEPBC").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
            {
                btnSyntheseSEPBC.Visibility = System.Windows.Visibility.Collapsed;
            }
            if (operationsUser.Where(op => op.NomOp == "Report : Extract Parc Auto").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
            {
                btnExtractParc.Visibility = System.Windows.Visibility.Collapsed;
            }
            if (operationsUser.Where(op => op.NomOp == "Report : Extract Parc Auto Excel").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
            {
                btnExtractParcExcel.Visibility = System.Windows.Visibility.Collapsed;
            }
            if (operationsUser.Where(op => op.NomOp == "Report : Historique position").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
            {
                btnHistOccup.Visibility = System.Windows.Visibility.Collapsed;
            }
            if (operationsUser.Where(op => op.NomOp == "Report : Stock conteneurs").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
            {
                btnStockConteneur.Visibility = System.Windows.Visibility.Collapsed;
            }
            if (operationsUser.Where(op => op.NomOp == "Report : Bilan Opérations Tracking").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
            {
                btnBilanTracking.Visibility = System.Windows.Visibility.Collapsed;
            }
            if (operationsUser.Where(op => op.NomOp == "Report : Extract Véhicules entrés").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
            {
                btnVehEntres.Visibility = System.Windows.Visibility.Collapsed;
            }
            if (operationsUser.Where(op => op.NomOp == "Report : Extract Véhicules Sortis").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
            {
                btnVehSortis.Visibility = System.Windows.Visibility.Collapsed;
            }
            if (operationsUser.Where(op => op.NomOp == "Report : Extract Manutention").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
            {
                btnExtractManut.Visibility = System.Windows.Visibility.Collapsed;
            }
            if (operationsUser.Where(op => op.NomOp == "Report : Extract Séjour").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
            {
                btnExtractSejour.Visibility = System.Windows.Visibility.Collapsed;
            }
            if (operationsUser.Where(op => op.NomOp == "Report : Extract Entrées").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
            {
                btnExtractEntrees.Visibility = System.Windows.Visibility.Collapsed;
            }
            if (operationsUser.Where(op => op.NomOp == "Report : Extract Sorties").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
            {
                btnExtractSorties.Visibility = System.Windows.Visibility.Collapsed;
            }
            if (operationsUser.Where(op => op.NomOp == "Report : SOCAR").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
            {
                btnExtractSOCAR.Visibility = System.Windows.Visibility.Collapsed;
            }
            if (operationsUser.Where(op => op.NomOp == "Report : Extract Prév. Import DIT").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
            {
                btnExtractPrevImportDIT.Visibility = System.Windows.Visibility.Collapsed;
            }
            if (operationsUser.Where(op => op.NomOp == "Report : Extract Prév. Export DIT").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
            {
                btnExtractPrevExportDIT.Visibility = System.Windows.Visibility.Collapsed;
            }
            if (operationsUser.Where(op => op.NomOp == "Report : Extract Reversements").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
            {
                btnExtractDiversReversements.Visibility = System.Windows.Visibility.Collapsed;
            }
            if (operationsUser.Where(op => op.NomOp == "Report : Daily Moves").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
            {
                btnDailyMoves.Visibility = System.Windows.Visibility.Collapsed;
            }
            if (operationsUser.Where(op => op.NomOp == "Report : Extract CNCC").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
            {
                btnExtractCNCC.Visibility = System.Windows.Visibility.Collapsed;
            }
            if (operationsUser.Where(op => op.NomOp == "Report : Recap Surestaries").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
            {
                btnRecapSurestaries.Visibility = System.Windows.Visibility.Collapsed;
            }
            if (operationsUser.Where(op => op.NomOp == "Report : Recap Detention").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
            {
                btnRecapDetention.Visibility = System.Windows.Visibility.Collapsed;
            }
            if (operationsUser.Where(op => op.NomOp == "Report : Commande SAP").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
            {
                btnImprimerCommandeSAP.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private void btnPaiement_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                EtatPaiementsReport paiementsReport = new EtatPaiementsReport(this);
                vesselWindow.mainPanel.Header = "Etat des paiements";
                vesselWindow.mainPanel.Content = null;
                vesselWindow.mainPanel.Content = paiementsReport;
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        private void btnFacturesDIT_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                EtatFacturesDITReport facturesDITReport = new EtatFacturesDITReport(this);
                vesselWindow.mainPanel.Header = "Etat des factures DIT";
                vesselWindow.mainPanel.Content = null;
                vesselWindow.mainPanel.Content = facturesDITReport;
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        private void btnElementsDIT_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                EtatElementsDITReport elementsPanelReport = new EtatElementsDITReport(this);
                vesselWindow.mainPanel.Header = "Etat éléments DIT";
                vesselWindow.mainPanel.Content = null;
                vesselWindow.mainPanel.Content = elementsPanelReport;
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        private void btnElementsSEPBC_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                EtatElementsSEPBCReport elementsSEPBCReport = new EtatElementsSEPBCReport(this);
                vesselWindow.mainPanel.Header = "Etat éléments SEPBC";
                vesselWindow.mainPanel.Content = null;
                vesselWindow.mainPanel.Content = elementsSEPBCReport;
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        private void btnSyntheseParAcconier_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                EtatSyntheseParAcconiersReport syntheseParAcconiersReport = new EtatSyntheseParAcconiersReport(this);
                vesselWindow.mainPanel.Header = "Synthèse par acconier";
                vesselWindow.mainPanel.Content = null;
                vesselWindow.mainPanel.Content = syntheseParAcconiersReport;
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        private void btnSyntheseSEPBC_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                EtatSyntheseSEPBCReport syntheseSEPBCReport = new EtatSyntheseSEPBCReport(this);
                vesselWindow.mainPanel.Header = "Synthèse SEPBC";
                vesselWindow.mainPanel.Content = null;
                vesselWindow.mainPanel.Content = syntheseSEPBCReport;
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        private void btnExtractParc_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                EtatContenuParcReport contenuParcReport = new EtatContenuParcReport(this);
                vesselWindow.mainPanel.Header = "Contenu du parc auto";
                vesselWindow.mainPanel.Content = null;
                vesselWindow.mainPanel.Content = contenuParcReport;
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        private void btnVehSortis_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                EtatSortiesReport sortiesParcReport = new EtatSortiesReport(this);
                vesselWindow.mainPanel.Header = "Sorties du parc auto";
                vesselWindow.mainPanel.Content = null;
                vesselWindow.mainPanel.Content = sortiesParcReport;
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        private void btnExtractManut_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ExtractManutForm extractManutentionForm = new ExtractManutForm(this, utilisateur);
                extractManutentionForm.Title = "Edition du l'extract des véhicules pour manutention";
                extractManutentionForm.ShowDialog();
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        private void btnExtractSejour_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ExtractSejourForm extractSejourForm = new ExtractSejourForm(this, utilisateur);
                extractSejourForm.Title = "Edition du l'extract des véhicules pour séjour";
                extractSejourForm.ShowDialog();
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        private void btnExtractEntrees_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ExtractManutAllForm extractSejourForm = new ExtractManutAllForm(this, utilisateur);
                extractSejourForm.Title = "Edition du l'extract des véhicules entrés au parc";
                extractSejourForm.ShowDialog();
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        private void btnExtractSorties_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ExtractSejourAllForm extractSejourForm = new ExtractSejourAllForm(this, utilisateur);
                extractSejourForm.Title = "Edition du l'extract des véhicules sortis du parc";
                extractSejourForm.ShowDialog();
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        private void btnExtractCA_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                EtatCAReport CAReport = new EtatCAReport(this);
                vesselWindow.mainPanel.Header = "Chiffre d'affaire";
                vesselWindow.mainPanel.Content = null;
                vesselWindow.mainPanel.Content = CAReport;
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        private void btnExtractCADetail_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                EtatCADetailReport detailCAReport = new EtatCADetailReport(this);
                vesselWindow.mainPanel.Header = "Chiffre d'affaire détaillé";
                vesselWindow.mainPanel.Content = null;
                vesselWindow.mainPanel.Content = detailCAReport;
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        private void btnExtractSOCAR_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SOCARReport socarReport = new SOCARReport(this);
                vesselWindow.mainPanel.Header = "Reporting SOCAR";
                vesselWindow.mainPanel.Content = null;
                vesselWindow.mainPanel.Content = socarReport;
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        private void btnVehEntres_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                EtatEntreesReport entreesParcReport = new EtatEntreesReport(this);
                vesselWindow.mainPanel.Header = "Entrées parc auto";
                vesselWindow.mainPanel.Content = null;
                vesselWindow.mainPanel.Content = entreesParcReport;
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        private void btnExtractArmateur_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SurestDetReport surestDetReport = new SurestDetReport(this);
                vesselWindow.mainPanel.Header = "Reporting Surest. Dét.";
                vesselWindow.mainPanel.Content = null;
                vesselWindow.mainPanel.Content = surestDetReport;
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        private void btnExtractDouane_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ExtractDouaneForm extractDouaneForm = new ExtractDouaneForm(this, utilisateur);
                extractDouaneForm.Title = "Edition du l'extract des véhicules pour douane";
                extractDouaneForm.ShowDialog();
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        private void btnHistPosition_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                HistPositionReport histPositionReport = new HistPositionReport(this);
                vesselWindow.mainPanel.Header = "Historique position";
                vesselWindow.mainPanel.Content = null;
                vesselWindow.mainPanel.Content = histPositionReport;
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        private void btnExtractParcExcel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ExtractParcAutoExcelForm extractParcAutoExcelForm = new ExtractParcAutoExcelForm(this, utilisateur);
                extractParcAutoExcelForm.Title = "Edition du l'extract des véhicules parqués";
                extractParcAutoExcelForm.ShowDialog();
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        private void btnExtractPrevImportDIT_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ExtractPrevImportDITForm extractPrevImportDITForm = new ExtractPrevImportDITForm(this, utilisateur);
                extractPrevImportDITForm.Title = "Prévision d'importation conteneur";
                extractPrevImportDITForm.ShowDialog();
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        private void btnExtractPrevExportDIT_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ExtractPrevExportDITForm extractPrevExportDITForm = new ExtractPrevExportDITForm(this, utilisateur);
                extractPrevExportDITForm.Title = "Prévision d'exportation conteneur";
                extractPrevExportDITForm.ShowDialog();
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        private void btnExtractDiversReversements_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ExtractDiversReversementsForm extractDiversReversementsForm = new ExtractDiversReversementsForm(this, utilisateur);
                extractDiversReversementsForm.Title = "Edition du l'extract des divers reversements";
                extractDiversReversementsForm.ShowDialog();
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        private void btnStockConteneur_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                EtatStockConteneurReport stockConteneurReport = new EtatStockConteneurReport(this);
                vesselWindow.mainPanel.Header = "Stock de conteneurs";
                vesselWindow.mainPanel.Content = null;
                vesselWindow.mainPanel.Content = stockConteneurReport;
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        private void btnDailyMoves_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DailyMovesForm dailyMovesForm = new DailyMovesForm(this, utilisateur);
                dailyMovesForm.Title = "Daily Moves";
                dailyMovesForm.ShowDialog();
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        private void btnExtractCNCC_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ExtractCNCCForm extractVehiculesCNCCForm = new ExtractCNCCForm(this, utilisateur);
                extractVehiculesCNCCForm.Title = "Edition du l'extract des véhicules sortis CNCC";
                extractVehiculesCNCCForm.ShowDialog();
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        private void btnImprimerCommandeSAP_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CommandeSAPForm cdeSAPForm = new CommandeSAPForm(this, utilisateur);
                cdeSAPForm.Title = "Commande SAP";
                cdeSAPForm.ShowDialog();
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnBilanTracking_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                EtatBilanTrackingReport bilanTrackingReport = new EtatBilanTrackingReport(this);
                vesselWindow.mainPanel.Header = "Bilan Tracking";
                vesselWindow.mainPanel.Content = null;
                vesselWindow.mainPanel.Content = bilanTrackingReport;
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        private void btnRecapSurestaries_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                RecapSurestariesForm recapSurestariesForm = new RecapSurestariesForm(this, utilisateur);
                recapSurestariesForm.Title = "Recap Surestaries";
                recapSurestariesForm.ShowDialog();
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        private void btnRecapDetention_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                RecapDetentionForm recapDetentionForm = new RecapDetentionForm(this, utilisateur);
                recapDetentionForm.Title = "Recap Détention";
                recapDetentionForm.ShowDialog();
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
