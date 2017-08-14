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
    /// Logique d'interaction pour UpdateEltAvoirPartielForm.xaml
    /// </summary>
    public partial class UpdateEltAvoirPartielForm : Window
    {
        private AvoirForm avoirForm;
        public List<ElementFacturation> eltsFactAvoir { get; set; }
        private ElementFacturation selectedElt;
        public List<string> elts { get; set; }

        private UTILISATEUR utilisateur;
        private List<OPERATION> operationsUser;

        private FormLoader formLoader;
        //private VsomParameters vsp = new VsomParameters();
        private VSOMAccessors vsomAcc;
        public UpdateEltAvoirPartielForm(AvoirForm form, List<ElementFacturation> listEltsSelect, UTILISATEUR user)
        {
            try
            {
                vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

                eltsFactAvoir = listEltsSelect;
                avoirForm = form;

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
                 vsomAcc = new VSOMAccessors();

                AVOIR avoir = vsomAcc.InsertAvoirPartiel(Convert.ToInt32(avoirForm.cbNumFact.Text), eltsFactAvoir, new TextRange(avoirForm.txtObservations.Document.ContentStart, avoirForm.txtObservations.Document.ContentEnd).Text, utilisateur.IdU);

                formLoader.LoadAvoirForm(avoirForm, avoir);

                avoirForm.cbIdAvoir.IsEnabled = true;

                avoirForm.borderActions.Visibility = System.Windows.Visibility.Visible;
                avoirForm.borderEtat.Visibility = System.Windows.Visibility.Visible;
                MessageBox.Show("Avoir enregistré avec succès.", "Enregistrement effectué !", MessageBoxButton.OK, MessageBoxImage.Information);
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

        private void txtMontant_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void txtQte_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9.,]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void txtPrixUnitaire_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter)
                {
                    if (dataGridEltsFact.SelectedIndex == -1)
                    {
                        MessageBox.Show("Veuillez au préalable sélectionner un élément de facture", "Elément de facture ?", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else if (txtPrixUnitaire.Text.Trim() == "")
                    {
                        MessageBox.Show("Vous devez saisir le prix unitaire pour cet élément de facture", "Prix unitaire ?", MessageBoxButton.OK, MessageBoxImage.Information);
                        txtPrixUnitaire.Focus();
                    }
                    else if (txtQuantite.Text.Trim() == "")
                    {
                        MessageBox.Show("Vous devez saisir la quantité pour cet élément de facture", "Quantité ?", MessageBoxButton.OK, MessageBoxImage.Information);
                        txtQuantite.Focus();
                    }
                    else
                    {
                        txtQuantite.Text = txtQuantite.Text.Replace(" ", "").Replace(".", ",");
                        selectedElt.Qte = Math.Round(Convert.ToDouble(txtQuantite.Text), 3, MidpointRounding.AwayFromZero);
                        selectedElt.PrixUnitaire = Convert.ToInt32(txtPrixUnitaire.Text);
                        selectedElt.MontantHT = Math.Round(selectedElt.PrixUnitaire * selectedElt.Qte, 0, MidpointRounding.AwayFromZero);
                        selectedElt.MontantTVA = Math.Round((selectedElt.PrixUnitaire * selectedElt.Qte * 0.1925f), 0, MidpointRounding.AwayFromZero);
                        selectedElt.MontantTTC = Math.Round((selectedElt.PrixUnitaire * selectedElt.Qte * (1.1925f)), 0, MidpointRounding.AwayFromZero);

                        dataGridEltsFact.ItemsSource = null;
                        dataGridEltsFact.ItemsSource = eltsFactAvoir;
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

        private void txtQuantite_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter)
                {
                    if (dataGridEltsFact.SelectedIndex == -1)
                    {
                        MessageBox.Show("Veuillez au préalable sélectionner un élément de facture", "Elément de facture ?", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else if (txtPrixUnitaire.Text.Trim() == "")
                    {
                        MessageBox.Show("Vous devez saisir le prix unitaire pour cet élément de facture", "Prix unitaire ?", MessageBoxButton.OK, MessageBoxImage.Information);
                        txtPrixUnitaire.Focus();
                    }
                    else if (txtQuantite.Text.Trim() == "")
                    {
                        MessageBox.Show("Vous devez saisir la quantité pour cet élément de facture", "Quantité ?", MessageBoxButton.OK, MessageBoxImage.Information);
                        txtQuantite.Focus();
                    }
                    else
                    {
                        txtQuantite.Text = txtQuantite.Text.Replace(" ", "").Replace(".", ",");
                        selectedElt.Qte = Math.Round(Convert.ToDouble(txtQuantite.Text), 3, MidpointRounding.AwayFromZero);
                        selectedElt.PrixUnitaire = Convert.ToInt32(txtPrixUnitaire.Text);
                        selectedElt.MontantHT = Math.Round(selectedElt.PrixUnitaire * selectedElt.Qte, 0, MidpointRounding.AwayFromZero);
                        selectedElt.MontantTVA = Math.Round((selectedElt.PrixUnitaire * selectedElt.Qte * 0.1925f), 0, MidpointRounding.AwayFromZero);
                        selectedElt.MontantTTC = Math.Round((selectedElt.PrixUnitaire * selectedElt.Qte * (1.1925f)), 0, MidpointRounding.AwayFromZero);

                        dataGridEltsFact.ItemsSource = null;
                        dataGridEltsFact.ItemsSource = eltsFactAvoir;
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

        private void dataGridEltsFact_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (dataGridEltsFact.SelectedIndex != -1)
                {
                    selectedElt = (ElementFacturation)dataGridEltsFact.SelectedItem;
                    txtElt.Text = selectedElt.LibArticle;
                    txtPrixUnitaire.Text = selectedElt.PrixUnitaire.ToString();
                    txtQuantite.Text = selectedElt.Qte.ToString();
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
