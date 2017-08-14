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
namespace VesselStopOverPresentation.Finance
{
    /// <summary>
    /// Logique d'interaction pour NovaToSAGE.xaml
    /// </summary>
    public partial class NovaToSAGE : Window
    {
        public NovaToSAGE()
        {
            InitializeComponent();
            txtDebut.SelectedDate = DateTime.Today;
            txtFin.SelectedDate = DateTime.Today;
        }

        private void BtnExtract_Click_1(object sender, RoutedEventArgs e)
        {
            if (txtFin.SelectedDate.Value < txtDebut.SelectedDate.Value)
                MessageBox.Show("Veuillez verifier la période sélectionnée!!!", "Extraction NOVA");
            else
            {
                try
                {
                    BtnExtract.IsEnabled = false;
                    this.Cursor = Cursors.Wait;
                    VsomSAGE vs;
                    vs = new VsomSAGE();
                    string rep = vs.NOVAReport(txtDebut.SelectedDate.Value, txtFin.SelectedDate.Value);
                    //vs.NOVA_TRANSACTIONS();

                    MessageBox.Show("Opération effectuée.Consulter les fichiers export");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Une erreur est survenue lors de l'extraction " + ex.Message+" \n"+ex.StackTrace, "Finance Journal Comptable");
                }
                finally
                {
                    BtnExtract.IsEnabled = true;
                    this.Cursor = Cursors.Arrow;
                }
            }
        }
    }
}
