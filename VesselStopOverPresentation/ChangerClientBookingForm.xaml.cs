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
    /// Logique d'interaction pour ChangerClientBookingForm.xaml
    /// </summary>
    public partial class ChangerClientBookingForm : Window
    {
        private BookingForm bookingForm;

        private List<OPERATION> operationsUser;
        private UTILISATEUR utilisateur;

        public List<CLIENT> clients { get; set; }
        public List<string> clts { get; set; }

        private FormLoader formLoader;
        //private VsomParameters vsp = new VsomParameters();
        private VSOMAccessors vsomAcc;
        public ChangerClientBookingForm(BookingForm form, UTILISATEUR user)
        {
            try
            {
                 vsomAcc = new VSOMAccessors();
               // VsomParameters vsprm = new VsomParameters();
                InitializeComponent();
                this.DataContext = this;

                clients = vsomAcc.GetClientsActifs();
                clts = new List<string>();
                foreach (CLIENT cl in clients)
                {
                    clts.Add(cl.NomClient);
                }

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

        private void cbClientNouveau_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                txtCodeClientNouveau.Text = clients.ElementAt<CLIENT>(cbClientNouveau.SelectedIndex).CodeClient;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnChangerClient_Click(object sender, RoutedEventArgs e)
        {
            try
            {
               // VSOMAccessors vsomAcc = new VSOMAccessors();

                if (cbClientNouveau.SelectedIndex == -1)
                {
                    MessageBox.Show("Veuillez sélectionner un client", "Client ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    CONNAISSEMENT book = vsomAcc.UpdateClientBooking(Convert.ToInt32(bookingForm.txtSysId.Text), clients.ElementAt<CLIENT>(cbClientNouveau.SelectedIndex).IdClient, utilisateur.IdU);

                    formLoader.LoadBookingForm(bookingForm, book);

                    MessageBox.Show("L'opération de changement de client s'est déroulée avec succès", "Booking transféré !", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();
                }
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
    }
}
