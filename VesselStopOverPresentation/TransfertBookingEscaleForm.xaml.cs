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
using Excel = Microsoft.Office.Interop.Excel;
using System.IO;
using System.IO.Compression;

namespace VesselStopOverPresentation
{
    /// <summary>
    /// Logique d'interaction pour TransfertBookingEscaleForm.xaml
    /// </summary>
    public partial class TransfertBookingEscaleForm : Window
    {

        private BookingForm bookingForm;

        private List<OPERATION> operationsUser;
        private UTILISATEUR utilisateur;

        public List<ESCALE> escales { get; set; }
        public List<Int32> escs { get; set; }

        private FormLoader formLoader;
        //private VsomParameters vsp = new VsomParameters();
        private VSOMAccessors vsomAcc;

        public TransfertBookingEscaleForm(BookingForm form, UTILISATEUR user)
        {
            try
            {
                 vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

                utilisateur = user;
                operationsUser = vsomAcc.GetOperationsUtilisateur(utilisateur.IdU);

                bookingForm = form;

                formLoader = new FormLoader(utilisateur);
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        private void cbNumEscale_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                 vsomAcc = new VSOMAccessors();

                if (e.Key == Key.Return && cbNumEscale.Text.Trim() != "")
                {
                    int result;
                    escales = vsomAcc.GetEscalesByNumEscale(Int32.TryParse(cbNumEscale.Text.Trim(), out result) ? result : -1);

                    if (escales.Count == 0)
                    {
                        MessageBox.Show("Il n'existe aucune escale portant ce numéro", "Escale introuvable", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                    else if (escales.Count == 1)
                    {
                        ESCALE esc = escales.FirstOrDefault<ESCALE>();
                        formLoader.LoadEscaleForm(this, esc);
                    }
                    else
                    {
                        ListEscaleForm listEscForm = new ListEscaleForm(this, escales, utilisateur);
                        listEscForm.Title = "Choix multiples : Sélectionnez une escale";
                        listEscForm.ShowDialog();
                    }
                }
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        private void cbNumEscale_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void btnTransferer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                vsomAcc = new VSOMAccessors();
                //VsomMarchal vsomAcc = new VsomMarchal();
                if (txtEscaleSysID.Text == "")
                {
                    MessageBox.Show("Veuillez sélectionner l'escale vers laquelle vous voulez transferer ce booking", "Escale ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    CONNAISSEMENT book = vsomAcc.TransfertBookingEscale(Convert.ToInt32(bookingForm.txtSysId.Text), Convert.ToInt32(txtEscaleSysID.Text), new TextRange(txtObservations.Document.ContentStart, txtObservations.Document.ContentEnd).Text, utilisateur.IdU);

                    formLoader.LoadBookingForm(bookingForm, book);

                    MessageBox.Show("L'opération de transfert d'escale s'est déroulée avec succès", "Booking transféré !", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();
                }

            }
            catch (HabilitationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (EnregistrementInexistant ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message + "\n" + ex.StackTrace, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
    }
}
