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
    /// Logique d'interaction pour EclaterBLExportForm.xaml
    /// </summary>
    public partial class EclaterBLExportForm : Window
    {
        private BookingForm bookForm;
        public List<ElementBookingGC> eltsBookingGC { get; set; }
        public List<ElementBookingCtr> eltsBookingCtr { get; set; }

        private UTILISATEUR utilisateur;
        private List<OPERATION> operationsUser;

        private FormLoader formLoader;
        private VsomParameters vsp = new VsomParameters();
        public EclaterBLExportForm(BookingForm form, int idBooking, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

                CONNAISSEMENT booking = vsp.GetBookingByIdBooking(idBooking);

                eltsBookingGC = vsp.GetConventionnelsOfBooking(booking.IdBL);
                dataGridGC.ItemsSource = null;
                dataGridGC.ItemsSource = form.eltsBookingGC;

                eltsBookingCtr = vsp.GetConteneursOfBooking(booking.IdBL);
                dataGridConteneurs.ItemsSource = null;
                dataGridConteneurs.ItemsSource = form.eltsBookingCtr;

                bookForm = form;

                utilisateur = user;
                operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);

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

        private void btnEclaterBL_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //VSOMAccessors vsomAcc = new VSOMAccessors();
                VsomMarchal vsomAcc = new VsomMarchal();
                CONNAISSEMENT newBooking = vsomAcc.EclaterBLExport(Convert.ToInt32(bookForm.txtSysId.Text), dataGridConteneurs.SelectedItems.OfType<ElementBookingCtr>().ToList<ElementBookingCtr>(), dataGridGC.SelectedItems.OfType<ElementBookingGC>().ToList<ElementBookingGC>(), utilisateur.IdU);
                BookingForm bookingForm = new BookingForm(this, newBooking, utilisateur);
                bookingForm.Show();

                CONNAISSEMENT ancienBooking = vsp.GetBookingByIdBooking(Convert.ToInt32(bookForm.txtSysId.Text));
                formLoader.LoadBookingForm(bookForm, ancienBooking);
                
                MessageBox.Show("BL export éclaté avec succès", "BL export éclaté !", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            catch (EnregistrementInexistant ei)
            {
                MessageBox.Show(ei.Message, "Enregistrement inexistant", MessageBoxButton.OK, MessageBoxImage.Exclamation);
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
