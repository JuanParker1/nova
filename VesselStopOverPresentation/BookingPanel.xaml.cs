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

namespace VesselStopOverPresentation
{
    /// <summary>
    /// Logique d'interaction pour BookingPanel.xaml
    /// </summary>
    public partial class BookingPanel : DockPanel
    {
        public List<CONNAISSEMENT> bookings { get; set; }
        private List<OPERATION> operationsUser;
        private UTILISATEUR utilisateur;
        //private VsomParameters vsp = new VsomParameters();
        private VSOMAccessors vsomAcc;
        public BookingPanel(UTILISATEUR user)
        {
            try
            {
                 vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;
                listRechercher.SelectedIndex = 0;
                //cbFiltres.SelectedIndex = 0;

                utilisateur = user;
                operationsUser = vsomAcc.GetOperationsUtilisateur(utilisateur.IdU);
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
            
        }

        private void btnNouveau_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (operationsUser.Where(op => op.NomOp == "Booking : Enregistrement d'un nouvel élément").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
                {
                    MessageBox.Show("Vous n'avez pas les droits nécessaires pour créer un nouveau booking. Veuillez contacter un administrateur", "Droits insuffisants !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    BookingForm bookingForm = new BookingForm("Nouveau", this, utilisateur);
                    bookingForm.cbNumBooking.IsEditable = true;
                    bookingForm.Title = "Nouveau : Booking";
                    bookingForm.actionsBorder.Visibility = System.Windows.Visibility.Collapsed;
                    bookingForm.borderEtat.Visibility = System.Windows.Visibility.Collapsed;
                    bookingForm.Show();
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

        private void dataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                //VSOMAccessors vsomAcc = new VSOMAccessors();

                if (dataGrid.SelectedIndex != -1)
                {
                    CONNAISSEMENT con = vsomAcc.GetBookingByIdBooking(((CONNAISSEMENT)dataGrid.SelectedItem).IdBL);
                    BookingForm bookingForm = new BookingForm(this, con, utilisateur);
                    bookingForm.Show();
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

        private void btnAnnulerRecherche_Click(object sender, RoutedEventArgs e)
        {
            txtRechercher.Text = null;
        }

        private void txtRechercher_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                //VSOMAccessors vsomAcc = new VSOMAccessors();

                if (e.Key == Key.Return && listRechercher.SelectedItem != null)
                {
                    if (listRechercher.SelectedIndex == 0)
                    {
                        bookings = vsomAcc.GetBookingAllByNumBooking(txtRechercher.Text.Trim());
                        dataGrid.ItemsSource = bookings;
                        lblStatut.Content = bookings.Count + " Booking(s) trouvé(s)";
                    }
                    else if (listRechercher.SelectedIndex == 1)
                    {
                        int result;
                        bookings = vsomAcc.GetBookingByNumEscale(Int32.TryParse(txtRechercher.Text.Trim(), out result) ? result : -1);
                        dataGrid.ItemsSource = bookings;
                        lblStatut.Content = bookings.Count + " Booking(s) trouvé(s)";
                    }
                    else if (listRechercher.SelectedIndex == 2)
                    {
                        bookings = vsomAcc.GetBookingAllByConsignee(txtRechercher.Text.Trim());
                        dataGrid.ItemsSource = bookings;
                        lblStatut.Content = bookings.Count + " Booking(s) trouvé(s)";
                    }
                    else if (listRechercher.SelectedIndex == 3)
                    {
                        bookings = vsomAcc.GetBookingAllByShipper(txtRechercher.Text.Trim());
                        dataGrid.ItemsSource = bookings;
                        lblStatut.Content = bookings.Count + " Booking(s) trouvé(s)";
                    }
                }
                else if (e.Key == Key.Escape)
                {
                    txtRechercher.Text = null;
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

        private void cbFiltres_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                //VSOMAccessors vsomAcc = new VSOMAccessors();

                if (cbFiltres.SelectedIndex == 0)
                {
                    bookings = vsomAcc.GetBookingByStatut("Booking");
                    dataGrid.ItemsSource = bookings;
                    lblStatut.Content = bookings.Count + " Booking(s)";
                }
                else if (cbFiltres.SelectedIndex == 1)
                {
                    bookings = vsomAcc.GetBookingByStatut("Clearance");
                    dataGrid.ItemsSource = bookings;
                    lblStatut.Content = bookings.Count + " Booking(s)";
                }
                else if (cbFiltres.SelectedIndex == 2)
                {
                    bookings = vsomAcc.GetBookingByStatut("Final Booking");
                    dataGrid.ItemsSource = bookings;
                    lblStatut.Content = bookings.Count + " Booking(s)";
                }
                else if (cbFiltres.SelectedIndex == 3)
                {
                    bookings = vsomAcc.GetBookingByStatut("Cargo Loading");
                    dataGrid.ItemsSource = bookings;
                    lblStatut.Content = bookings.Count + " Booking(s)";
                }
                else if (cbFiltres.SelectedIndex == 4)
                {
                    bookings = vsomAcc.GetBookingByStatut("Cargo Loaded");
                    dataGrid.ItemsSource = bookings;
                    lblStatut.Content = bookings.Count + " Booking(s)";
                }
                else if (cbFiltres.SelectedIndex == 5)
                {
                    bookings = vsomAcc.GetBookingByStatut("BL généré");
                    dataGrid.ItemsSource = bookings;
                    lblStatut.Content = bookings.Count + " Booking(s)";
                }
                else if (cbFiltres.SelectedIndex == 6)
                {
                    bookings = vsomAcc.GetBookingByStatut("Annulé");
                    dataGrid.ItemsSource = bookings;
                    lblStatut.Content = bookings.Count + " Booking(s)";
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
    }
}
