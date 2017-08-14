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
    /// Logique d'interaction pour TransfertBookingCtrForm.xaml
    /// </summary>
    public partial class TransfertBookingCtrForm : Window
    {
        private BookingForm bookingForm;

        private List<OPERATION> operationsUser;
        private UTILISATEUR utilisateur;

        public List<CONNAISSEMENT> bookings { get; set; }
        public List<string> books { get; set; }

        private ElementBookingCtr eltCtr;

        private FormLoader formLoader;
        //private VsomParameters vsp = new VsomParameters();
        private VSOMAccessors vsomAcc;

        public TransfertBookingCtrForm(BookingForm form, ElementBookingCtr ctr, UTILISATEUR user)
        {
            try
            {
                 vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

                utilisateur = user;
                operationsUser = vsomAcc.GetOperationsUtilisateur(utilisateur.IdU);

                bookingForm = form;

                eltCtr = ctr;

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

        private void cbNumBooking_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                //VSOMAccessors vsomAcc = new VSOMAccessors();

                if (e.Key == Key.Return && cbNumBooking.Text.Trim() != "")
                {
                    bookings = vsomAcc.GetBookingByNumBooking(cbNumBooking.Text.Trim());

                    if (bookings.Count == 0)
                    {
                        MessageBox.Show("Il n'existe aucun booking portant ce numéro", "Booking introuvable", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                    else if (bookings.Count == 1)
                    {
                        CONNAISSEMENT book = bookings.FirstOrDefault<CONNAISSEMENT>();
                        formLoader.LoadBookingForm(this, book);
                    }
                    else
                    {
                        ListBookingForm listBookForm = new ListBookingForm(this, bookings, utilisateur);
                        listBookForm.Title = "Choix multiples : Sélectionnez un booking";
                        listBookForm.ShowDialog();
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

        private void btnTransferer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //VSOMAccessors vsomAcc = new VSOMAccessors();
                //VsomMarchal vsomAcc = new VsomMarchal();

                if (txtIdBL.Text == "")
                {
                    MessageBox.Show("Veuillez sélectionner le booking vers lequel vous voulez transferer ce conteneur", "Booking ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    CONNAISSEMENT book = vsomAcc.TransfertBookingCtr(eltCtr.IdCtr, Convert.ToInt32(txtIdBL.Text), new TextRange(txtObservations.Document.ContentStart, txtObservations.Document.ContentEnd).Text, utilisateur.IdU);

                    formLoader.LoadBookingForm(bookingForm, book);

                    MessageBox.Show("L'opération de transfert de booking s'est déroulée avec succès", "Conteneur transféré !", MessageBoxButton.OK, MessageBoxImage.Information);
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
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
    }
}
