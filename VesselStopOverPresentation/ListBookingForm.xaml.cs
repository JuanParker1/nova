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
    /// Logique d'interaction pour ListBookingForm.xaml
    /// </summary>
    public partial class ListBookingForm : Window
    {
        public List<CONNAISSEMENT> bookings { get; set; }

        private BookingForm bookForm;
        private TransfertBookingGCForm transfGCForm;
        private TransfertBookingCtrForm transfCtrForm;

        private List<OPERATION> operationsUser;
        private UTILISATEUR utilisateur;

        private FormLoader formLoader;
        private VsomParameters vsp = new VsomParameters();
        public ListBookingForm(BookingForm form, List<CONNAISSEMENT> listBookings, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

                bookings = listBookings;
                dataGrid.ItemsSource = bookings;
                
                utilisateur = user;
                operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);

                bookForm = form;

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

        public ListBookingForm(TransfertBookingGCForm form, List<CONNAISSEMENT> listBookings, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

                bookings = listBookings;
                dataGrid.ItemsSource = bookings;

                utilisateur = user;
                operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);

                transfGCForm = form;

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

        public ListBookingForm(TransfertBookingCtrForm form, List<CONNAISSEMENT> listBookings, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

                bookings = listBookings;
                dataGrid.ItemsSource = bookings;

                utilisateur = user;
                operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);

                transfCtrForm = form;

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

        private void dataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (dataGrid.SelectedIndex != -1)
                {
                    if (bookForm != null)
                    {
                        formLoader.LoadBookingForm(bookForm, (CONNAISSEMENT)dataGrid.SelectedItem);
                        this.Close();
                    }
                    else if (transfGCForm != null)
                    {
                        formLoader.LoadBookingForm(transfGCForm, (CONNAISSEMENT)dataGrid.SelectedItem);
                        this.Close();
                    }
                    else if (transfCtrForm != null)
                    {
                        formLoader.LoadBookingForm(transfCtrForm, (CONNAISSEMENT)dataGrid.SelectedItem);
                        this.Close();
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

        private void btnSelectionner_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dataGrid.SelectedIndex != -1 && dataGrid.SelectedItems.Count == 1)
                {
                    if (bookForm != null)
                    {
                        formLoader.LoadBookingForm(bookForm, (CONNAISSEMENT)dataGrid.SelectedItem);
                        this.Close();
                    }
                    else if (transfGCForm != null)
                    {
                        formLoader.LoadBookingForm(transfGCForm, (CONNAISSEMENT)dataGrid.SelectedItem);
                        this.Close();
                    }
                    else if (transfCtrForm != null)
                    {
                        formLoader.LoadBookingForm(transfCtrForm, (CONNAISSEMENT)dataGrid.SelectedItem);
                        this.Close();
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
    }
}
