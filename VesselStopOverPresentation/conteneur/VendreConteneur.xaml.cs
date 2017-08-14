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
namespace VesselStopOverPresentation
{
    /// <summary>
    /// Logique d'interaction pour VendreConteneur.xaml
    /// </summary>
    public partial class VendreConteneur : Window
    {
        private CONTENEUR_TC _ctr {get;set;}
        private ConteneurTCForm _tc { get; set; }
        private UTILISATEUR _user { get; set; }
        public VendreConteneur(ConteneurTCForm tc , CONTENEUR_TC ctr , UTILISATEUR user)
        {

            InitializeComponent();
            _ctr = ctr; _user = user; tc = _tc;
        }

        private void btnEnreg_Click_1(object sender, RoutedEventArgs e)
        {
            VSOMAccessors vsomAcc = new VSOMAccessors();

            if (!txtDateVente.SelectedDate.HasValue)
            {
                MessageBox.Show("Veuillez entrer la date de vente", "Echec de l'opération", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            else if (txtClient.Text.Trim().Length == 0)
            {
                MessageBox.Show("Veuillez entrer le client", "Echec de l'opération", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            
            }
            else
            {
                //vsomAcc.VendreConteneur(_ctr, txtDateVente.SelectedDate.Value, txtClient.Text.Trim(), 
                  //    new TextRange(txtObservations.Document.ContentStart, txtObservations.Document.ContentEnd).Text, _user.IdU);
            }

        }
    }
}
