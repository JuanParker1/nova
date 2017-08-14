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
namespace VesselStopOverPresentation.Remote
{
    /// <summary>
    /// Logique d'interaction pour QuotationsPanel.xaml
    /// </summary>
    public partial class QuotationsPanel : DockPanel
    {
        public List<RMVSOM_Views> _list;
        UTILISATEUR _user;
        public QuotationsPanel(UTILISATEUR usr)
        {
            InitializeComponent();
            _list = new List<RMVSOM_Views>();
            _user = usr;
        }

        private void dataGrid_MouseDoubleClick_1(object sender, MouseButtonEventArgs e)
        {
            try
            {
                //VSOMAccessors vsomAcc = new VSOMAccessors();
               /* if (vsp.GetOperationsUtilisateur(_user.IdU).Where(op => op.NomOp == "Facture spot: Visualisation des éléments existants").FirstOrDefault<OPERATION>() == null && _user.LU != "Admin")
                {
                    MessageBox.Show("Vous n'avez pas les droits nécessaires pour effectuer cette action. Veuillez contacter un administrateur", "Remote VSOM");
                }
                else
                {*/
                    if (dataGrid.SelectedIndex != -1)
                    {
                        RMVSOM_Views fact = (RMVSOM_Views)dataGrid.SelectedItem;
                        QuotationForm frm = new QuotationForm(this, fact, _user);
                        frm.Show();
                    }
               /* }*/
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnRechercher_Click_1(object sender, RoutedEventArgs e)
        {

            if (cbFiltres.SelectedIndex == -1)
            {
                dataGrid.ItemsSource = null;  DateTime fin = txtfin.SelectedDate.Value;
                fin= fin.AddHours(23); fin=fin.AddMinutes(59);
                dataGrid.ItemsSource = new RMVSOM_Marchal().GetList(String.Empty, txtdebut.SelectedDate.Value, fin); 
            }
            else
            {
                dataGrid.ItemsSource = null; DateTime fin = txtfin.SelectedDate.Value;
                fin = fin.AddHours(23); fin = fin.AddMinutes(59);
                dataGrid.ItemsSource = new RMVSOM_Marchal().GetList(cbFiltres.Text, txtdebut.SelectedDate.Value, fin); 
            }
        }

        private void txtRechercher_PreviewKeyDown_1(object sender, KeyEventArgs e)
        {

        }
    }
}
