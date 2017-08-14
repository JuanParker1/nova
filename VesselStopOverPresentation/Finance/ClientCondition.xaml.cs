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
    /// Logique d'interaction pour ClientCondition.xaml
    /// </summary>
    public partial class ClientCondition : DockPanel
    {
        UTILISATEUR _user;
        public ClientCondition(UTILISATEUR user)
        {
            InitializeComponent();
            user = _user;
        }

        private void txtRechercher_PreviewKeyDown_1(object sender, KeyEventArgs e)
        {

        }

        private void btnNouveau_Click_1(object sender, RoutedEventArgs e)
        {
            Finance.ClientCondForm frm = new ClientCondForm(_user,true);
            frm.Show();
        }

        private void dataGrid_MouseDoubleClick_1(object sender, MouseButtonEventArgs e)
        {

        }

        private void btnRechercher_Click_1(object sender, RoutedEventArgs e)
        {

        }
    }
}
