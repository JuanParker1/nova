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
    /// Logique d'interaction pour HistoriqueOperationsForm.xaml
    /// </summary>
    public partial class HistoriqueOperationsForm : Window
    {

        public List<HistoriqueOperation> operations { get; set; }

        private VehiculeForm vehForm;
        private ConteneurForm ctrForm;
        private MafiForm mafiForm;
        private ConventionnelForm convForm;
        private VsomParameters vsp = new VsomParameters();
        public HistoriqueOperationsForm(VehiculeForm form, int idVeh)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;
                operations = vsp.GetHistoriqueOperationsVeh(idVeh);
                vehForm = form;
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
            
        }

        public HistoriqueOperationsForm(ConteneurForm form, int idCtr)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

                operations = vsp.GetHistoriqueOperationsCtr(idCtr);
                ctrForm = form;
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        public HistoriqueOperationsForm(MafiForm form, int idCtr)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

                operations = vsp.GetHistoriqueOperationsMafi(idCtr);
                mafiForm = form;
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        public HistoriqueOperationsForm(ConventionnelForm form, int idGC)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;
                operations = vsp.GetHistoriqueOperationsVeh(idGC);
                convForm = form;
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }

        }

        private void btnFermer_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
