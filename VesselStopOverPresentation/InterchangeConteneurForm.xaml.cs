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
using System.Text.RegularExpressions;

namespace VesselStopOverPresentation
{
    /// <summary>
    /// Logique d'interaction pour InterchangeConteneurForm.xaml
    /// </summary>
    public partial class InterchangeConteneurForm : Window
    {

        public List<HistoriqueInterchange> interchanges { get; set; }

        private ConteneurForm ctrForm;
        private ConteneurTCForm ctrTCForm;
        private CONTENEUR conteneur;
        private VsomParameters vsp = new VsomParameters();
        public InterchangeConteneurForm(ConteneurForm form, int idCtr)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;
                interchanges = vsp.GetInterchangeCtr(idCtr);

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

        public InterchangeConteneurForm(ConteneurTCForm form, int idCtr)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;
                interchanges = vsp.GetInterchangeCtr(idCtr);

                ctrTCForm = form;
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
