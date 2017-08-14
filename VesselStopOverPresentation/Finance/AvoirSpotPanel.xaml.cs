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

namespace VesselStopOverPresentation.Finance
{
    /// <summary>
    /// Logique d'interaction pour AvoirSpotPanel.xaml
    /// </summary>
    public partial class AvoirSpotPanel : DockPanel
    {

        private UTILISATEUR utilisateur;
        public AvoirSpotPanel(UTILISATEUR user)
        {
            InitializeComponent();
            utilisateur = user;
        }

        private void btnNouveau_Click_1(object sender, RoutedEventArgs e)
        {
            Finance.AvoirSpot frm = new AvoirSpot(utilisateur);
            frm.Show();
        }

        private void btnRechercher_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void txtRechercher_PreviewKeyDown_1(object sender, KeyEventArgs e)
        {

        }

        private void dataGrid_MouseDoubleClick_1(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
