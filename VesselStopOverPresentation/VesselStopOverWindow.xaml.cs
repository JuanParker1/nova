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
    /// Logique d'interaction pour VesselStopOverWindow.xaml
    /// </summary>
    public partial class VesselStopOverWindow : Page
    {
        public UTILISATEUR utilisateur { get; set; }

        private VSOMAccessors vsomAcc = new VSOMAccessors();

        public VesselStopOverWindow(UTILISATEUR user)
        {
            if (user.IdAcc == 1)
            {
                InitializeComponent();
                //DashboardPanel dashBoardPanel = new DashboardPanel(user);
                ExploitationControlPanel exploitPanel = new ExploitationControlPanel(this, user);
                //groupTreeView.Content = null;
                //mainPanel.Content = null;
                scrollViewer.Content = exploitPanel;
                controlHeader.Content = "Activités d'escale";
                //mainPanel.Content = dashBoardPanel;
                //controlHeader.Content = "Tableau de bord";
                lblDate.Content = "Heure de connexion : " + DateTime.Now;
                lblSociete.Content = "SOCOMAR";
                utilisateur = user;
                lblUser.Content = user.NU;
                //btnReporting.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                InitializeComponent();

                ParcAutoControlPanel parcPanel = new ParcAutoControlPanel(this, user);
                scrollViewer.Content = parcPanel;
                controlHeader.Content = "Activités du parc auto";

                btnAdministration.Visibility = System.Windows.Visibility.Collapsed;
                btnDashboard.Visibility = System.Windows.Visibility.Collapsed;
                btnExploitation.Visibility = System.Windows.Visibility.Collapsed;
                btnExport.Visibility = System.Windows.Visibility.Collapsed;
                btnFacturation.Visibility = System.Windows.Visibility.Collapsed;
                btnImport.Visibility = System.Windows.Visibility.Collapsed;
                btnReporting.Visibility = System.Windows.Visibility.Collapsed;

                lblDate.Content = "Heure de connexion : " + DateTime.Now;
                lblSociete.Content = "SOCOMAR";
                utilisateur = user;
                lblUser.Content = user.NU;
            }
        }

        private void btnExploitation_Click(object sender, RoutedEventArgs e)
        {
            ExploitationControlPanel exploitPanel = new ExploitationControlPanel(this, utilisateur);
            scrollViewer.Content = null;
            scrollViewer.Content = exploitPanel;
            controlHeader.Content = "Activités d'escale";
        }

        private void btnAdministration_Click(object sender, RoutedEventArgs e)
        {
            AdministrationControlPanel admPanel = new AdministrationControlPanel(this, utilisateur);
            scrollViewer.Content = null;
            scrollViewer.Content = admPanel;
            controlHeader.Content = "Administration";
        }

        private void btnImport_Click(object sender, RoutedEventArgs e)
        {
            ImportControlPanel importPanel = new ImportControlPanel(this, utilisateur);
            scrollViewer.Content = null;
            scrollViewer.Content = importPanel;
            controlHeader.Content = "Activités d'importation";
        }

        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            ExportControlPanel expPanel = new ExportControlPanel(this, utilisateur);
            scrollViewer.Content = null;
            scrollViewer.Content = expPanel;
            controlHeader.Content = "Activités d'exportation";
        }

        private void btnFacturation_Click(object sender, RoutedEventArgs e)
        {
            FacturationControlPanel factPanel = new FacturationControlPanel(this, utilisateur);
            scrollViewer.Content = null;
            scrollViewer.Content = factPanel;
            controlHeader.Content = "Facturation";
        }

        private void btnDashboard_Click(object sender, RoutedEventArgs e)
        {
            //DashboardPanel dashBoardPanel = new DashboardPanel(utilisateur);
            //mainPanel.Header = "Tableau de bord";
            //mainPanel.Content = null;
            //mainPanel.Content = dashBoardPanel;
        }

        private void btnParc_Click(object sender, RoutedEventArgs e)
        {
            ParcAutoControlPanel parcPanel = new ParcAutoControlPanel(this, utilisateur);
            scrollViewer.Content = null;
            scrollViewer.Content = parcPanel;
            controlHeader.Content = "Activités du parc auto";
        }

        private void btnReporting_Click(object sender, RoutedEventArgs e)
        {
            ReportingControlPanel reportPanel = new ReportingControlPanel(this, utilisateur);
            scrollViewer.Content = null;
            scrollViewer.Content = reportPanel;
            controlHeader.Content = "Reporting";
        }

        private void btnTracking_Click(object sender, RoutedEventArgs e)
        {
            TrackingControlPanel trackingPanel = new TrackingControlPanel(this, utilisateur);
            scrollViewer.Content = null;
            scrollViewer.Content = trackingPanel;
            controlHeader.Content = "Activités tracking conteneur";
        }

        private void btnFinance_Click_1(object sender, RoutedEventArgs e)
        {
            FinanceControlPanel admPanel = new FinanceControlPanel(this, utilisateur);
            scrollViewer.Content = null;
            scrollViewer.Content = admPanel;
            controlHeader.Content = "Finance";
        }
    }
}
