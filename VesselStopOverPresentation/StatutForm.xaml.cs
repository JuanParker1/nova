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
    /// Logique d'interaction pour StatutForm.xaml
    /// </summary>
    public partial class StatutForm : Window
    {

        public StatutForm(OPERATION_CONNAISSEMENT opBL)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

                txtObservations.Document.Blocks.Clear();
                txtObservations.Document.Blocks.Add(new Paragraph(new Run(opBL.AIOp)));
                lblStatut.Content = opBL.TYPE_OPERATION.LibTypeOp + " Le " + opBL.DateOp + " Par " + opBL.UTILISATEUR.NU;
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        public StatutForm(OPERATION_ESCALE opEsc)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

                txtObservations.Document.Blocks.Clear();
                txtObservations.Document.Blocks.Add(new Paragraph(new Run(opEsc.AIOp)));
                lblStatut.Content = opEsc.TYPE_OPERATION.LibTypeOp + " Le " + opEsc.DateOp + " Par " + opEsc.UTILISATEUR.NU;
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        public StatutForm(OPERATION_MANIFESTE opMan)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

                txtObservations.Document.Blocks.Clear();
                txtObservations.Document.Blocks.Add(new Paragraph(new Run(opMan.AIOp)));
                lblStatut.Content = opMan.TYPE_OPERATION.LibTypeOp + " Le " + opMan.DateOp + " Par " + opMan.UTILISATEUR.NU;
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
            try
            {
                this.Close();
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
