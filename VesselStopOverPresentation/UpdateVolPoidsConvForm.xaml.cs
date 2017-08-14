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
    /// Logique d'interaction pour UpdateVolPoidsConvForm.xaml
    /// </summary>
    public partial class UpdateVolPoidsConvForm : Window
    {
        private BookingForm bookForm;
        private List<ElementBookingGC> elementsConvBooks;
        public List<string> elts { get; set; }

        private UTILISATEUR utilisateur;
        private List<OPERATION> operationsUser;

        private FormLoader formLoader;
        //private VsomParameters vsomAcc = new VsomParameters();
        private VSOMAccessors vsomAcc;

        public UpdateVolPoidsConvForm(BookingForm form, int idBL, UTILISATEUR user)
        {
            try
            {
                 vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

                elementsConvBooks = vsomAcc.GetConventionnelsOfBooking(idBL);
                elts = new List<string>();
                foreach (ElementBookingGC elt in elementsConvBooks)
                {
                    elts.Add(elt.NumGC + " - " + elt.Description);
                }

                bookForm = form;

                utilisateur = user;
                operationsUser = vsomAcc.GetOperationsUtilisateur(utilisateur.IdU);

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

        private void btnEnregistrer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //VSOMAccessors vsomAcc = new VSOMAccessors();
                //VsomMarchal vsomAcc = new VsomMarchal();
                if (cbElt.SelectedIndex == -1)
                {
                    MessageBox.Show("Vous devez saisir sélectionner un conventionnel", "Conventionnel ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (txtQteB.Text.Trim() == "")
                {
                    MessageBox.Show("Vous devez saisir la quantité bookée", "Qté bookée ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    txtQteB.Focus();
                }
                else if (txtQteR.Text.Trim() == "")
                {
                    MessageBox.Show("Vous devez saisir la quantité réceptionnée", "Qté réceptionnée ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    txtQteR.Focus();
                }
                else if (txtNumItem.Text.Trim() == "")
                {
                    MessageBox.Show("Vous devez saisir la quantité embarquée", "Qté embarquée ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    txtQteB.Focus();
                }
                else if (txtPoidsB.Text.Trim() == "")
                {
                    MessageBox.Show("Vous devez saisir le poids booké", "Poids booké ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    txtPoidsB.Focus();
                }
                else if (txtPoidsR.Text.Trim() == "")
                {
                    MessageBox.Show("Vous devez saisir le poids réceptionné", "Poids réceptionné ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    txtPoidsR.Focus();
                }
                else if (txtPoidsE.Text.Trim() == "")
                {
                    MessageBox.Show("Vous devez saisir le poids embarqué", "Poids embarqué ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    txtPoidsE.Focus();
                }
                else if (txtVolB.Text.Trim() == "")
                {
                    MessageBox.Show("Vous devez saisir le volume booké", "Volume booké ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    txtVolB.Focus();
                }
                else if (txtVolR.Text.Trim() == "")
                {
                    MessageBox.Show("Vous devez saisir le volume réceptionné", "Volume réceptionné ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    txtVolR.Focus();
                }
                else if (txtVolE.Text.Trim() == "")
                {
                    MessageBox.Show("Vous devez saisir le volume embarqué", "Volume embarqué ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    txtVolE.Focus();
                }
                else
                {
                    //txtQuantite.Text = txtQuantite.Text.Replace(" ", "").Replace(".", ",");

                    ElementBookingGC elt = vsomAcc.UpdateVolPoidsConvBooking(elementsConvBooks.ElementAt<ElementBookingGC>(cbElt.SelectedIndex).IdGC, Convert.ToInt16(txtQteB.Text), Convert.ToInt16(txtQteR.Text), Convert.ToInt16(txtNumItem.Text), Convert.ToDouble(txtPoidsB.Text.Replace(" ", "").Replace(".", ",")), Convert.ToDouble(txtPoidsR.Text.Replace(" ", "").Replace(".", ",")), Convert.ToDouble(txtPoidsE.Text.Replace(" ", "").Replace(".", ",")), Convert.ToDouble(txtVolB.Text.Replace(" ", "").Replace(".", ",")), Convert.ToDouble(txtVolR.Text.Replace(" ", "").Replace(".", ",")), Convert.ToDouble(txtVolE.Text.Replace(" ", "").Replace(".", ",")), utilisateur.IdU);
                    CONVENTIONNEL conv = vsomAcc.GetConventionnelExportByIdGC(elt.IdGC);
                    elementsConvBooks = vsomAcc.GetConventionnelsOfBooking(conv.IdBL.Value);
                    elts = new List<string>();
                    foreach (ElementBookingGC el in elementsConvBooks)
                    {
                        elts.Add(el.NumGC + " - " + el.Description);
                    }
                    cbElt.ItemsSource = null;
                    cbElt.ItemsSource = elts;
                    cbElt.SelectedItem = elt.NumGC + " - " + elt.Description;

                    formLoader.LoadBookingForm(bookForm, vsomAcc.GetConnaissementByIdBL(conv.IdBL.Value));

                    MessageBox.Show("Ce conventionnel a été mis à jour avec succès", "Conventionnel mis à jour !", MessageBoxButton.OK, MessageBoxImage.Information);
                }
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
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void cbElt_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                //VSOMAccessors vsomAcc = new VSOMAccessors();

                if (cbElt.SelectedIndex != -1)
                {
                    int indexElt = cbElt.SelectedIndex;
                    CONVENTIONNEL gc = vsomAcc.GetConventionnelExportByIdGC(elementsConvBooks.ElementAt<ElementBookingGC>(indexElt).IdGC);

                    txtQteB.Text = gc.QteBGC.ToString();
                    txtQteR.Text = gc.QteRGC.ToString();
                    txtNumItem.Text = gc.NumItem.ToString();

                    txtPoidsB.Text = gc.PoidsMGC.ToString();
                    txtPoidsR.Text = gc.PoidsRGC.ToString();
                    txtPoidsE.Text = gc.PoidsCGC.ToString();

                    txtVolB.Text = gc.VolMGC.ToString();
                    txtVolR.Text = gc.VolRGC.ToString();
                    txtVolE.Text = gc.VolCGC.ToString();
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

        private void txtQte_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void txtPoidsVol_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9.,]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
