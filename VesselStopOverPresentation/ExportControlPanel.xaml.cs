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
    /// Logique d'interaction pour ExportControlPanel.xaml
    /// </summary>
    public partial class ExportControlPanel : StackPanel
    {
        private VesselStopOverWindow vesselWindow;
        private UTILISATEUR utilisateur;

        private List<OPERATION> operationsUser;

        private VSOMAccessors vsomAcc = new VSOMAccessors();
        VsomParameters vsp = new VsomParameters();

        public ExportControlPanel(VesselStopOverWindow window, UTILISATEUR user)
        {
            InitializeComponent();
            vesselWindow = window;
            utilisateur = user;
            operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);
            if (operationsUser.Where(op => op.NomOp == "Booking : Visualisation des éléments existants").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
            {
                btnBooking.Visibility = System.Windows.Visibility.Collapsed;
            }
            if (operationsUser.Where(op => op.NomOp == "Booking : Ajout d'information de clearance").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
            {
                btnClearance.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private void btnBooking_Click(object sender, RoutedEventArgs e)
        {
            BookingPanel bookingPanel = new BookingPanel(utilisateur);
            bookingPanel.cbFiltres.SelectedIndex = 0;
            vesselWindow.mainPanel.Header = "Booking";
            vesselWindow.mainPanel.Content = null;
            vesselWindow.mainPanel.Content = bookingPanel;
        }

        private void btnConteneur_Click(object sender, RoutedEventArgs e)
        {
            ConteneurExportPanel ctrPanel = new ConteneurExportPanel(utilisateur);
            vesselWindow.mainPanel.Header = "Conteneur export";
            vesselWindow.mainPanel.Content = null;
            vesselWindow.mainPanel.Content = ctrPanel;
        }

        private void btnConventionnel_Click(object sender, RoutedEventArgs e)
        {
            ConventionnelExportPanel convPanel = new ConventionnelExportPanel(utilisateur);
            vesselWindow.mainPanel.Header = "Conventionnel export";
            vesselWindow.mainPanel.Content = null;
            vesselWindow.mainPanel.Content = convPanel;
        }

        private void btnClearance_Click(object sender, RoutedEventArgs e)
        {
            BookingPanel clrPanel = new BookingPanel(utilisateur);
            clrPanel.cbFiltres.SelectedIndex = 1;
            vesselWindow.mainPanel.Header = "Clearance";
            vesselWindow.mainPanel.Content = null;
            vesselWindow.mainPanel.Content = clrPanel;
        }

        private void btnFinalBooking_Click(object sender, RoutedEventArgs e)
        {
            BookingPanel cargoLoadingPanel = new BookingPanel(utilisateur);
            cargoLoadingPanel.cbFiltres.SelectedIndex = 2;
            vesselWindow.mainPanel.Header = "Cargo Loading";
            vesselWindow.mainPanel.Content = null;
            vesselWindow.mainPanel.Content = cargoLoadingPanel;
        }

        private void btnCargoLoading_Click(object sender, RoutedEventArgs e)
        {
            BookingPanel cargoLoadingPanel = new BookingPanel(utilisateur);
            cargoLoadingPanel.cbFiltres.SelectedIndex = 3;
            vesselWindow.mainPanel.Header = "Cargo Loading";
            vesselWindow.mainPanel.Content = null;
            vesselWindow.mainPanel.Content = cargoLoadingPanel;
        }

        private void btnCargoLoaded_Click(object sender, RoutedEventArgs e)
        {
            BookingPanel cargoLoadedPanel = new BookingPanel(utilisateur);
            cargoLoadedPanel.cbFiltres.SelectedIndex = 4;
            vesselWindow.mainPanel.Header = "Cargo Loaded";
            vesselWindow.mainPanel.Content = null;
            vesselWindow.mainPanel.Content = cargoLoadedPanel;
        }

        private void btnBillOfLading_Click(object sender, RoutedEventArgs e)
        {
            BookingPanel billOfLadingPanel = new BookingPanel(utilisateur);
            billOfLadingPanel.cbFiltres.SelectedIndex = 5;
            vesselWindow.mainPanel.Header = "Bill of Lading";
            vesselWindow.mainPanel.Content = null;
            vesselWindow.mainPanel.Content = billOfLadingPanel;
        }

        private void btnManifest_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ManifestExportForm manifestExportForm = new ManifestExportForm(this, utilisateur);
                manifestExportForm.Title = "Edition du manifeste export";
                manifestExportForm.ShowDialog();
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
