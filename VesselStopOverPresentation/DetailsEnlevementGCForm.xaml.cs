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
    /// Logique d'interaction pour DetailsEnlevementGCForm.xaml
    /// </summary>
    public partial class DetailsEnlevementGCForm : Window
    {

        private BonEnleverForm baeForm;
        private List<CONVENTIONNEL> conventionnels;
        public List<string> convs { get; set; }

        private UTILISATEUR utilisateur;
        private List<OPERATION> operationsUser;

        private FormLoader formLoader;
        private VsomParameters vsp = new VsomParameters();
        public DetailsEnlevementGCForm(BonEnleverForm form, int idBL, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

                conventionnels = vsp.GetConventionnelsOfConnaissement(idBL);
                convs = new List<string>();
                foreach (CONVENTIONNEL gc in conventionnels)
                {
                    convs.Add(gc.NumGC);
                }

                baeForm = form;

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

        private void btnEnregistrer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //VSOMAccessors vsomAcc = new VSOMAccessors();
                VsomMarchal vsomAcc = new VsomMarchal();

                if (cbNumGC.SelectedIndex == -1)
                {
                    MessageBox.Show("Vous devez saisir sélectionner un conventionnel", "Conventionnel ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (txtEnleveur.Text.Trim() == "")
                {
                    MessageBox.Show("Vous devez saisir le nom de l'enleveur du conventionnel", "Enleveur ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    txtEnleveur.Focus();
                }
                else if (txtCNI.Text.Trim() == "")
                {
                    MessageBox.Show("Vous devez saisir le numéro de la pièce d'identifié de l'enleveur.", "N° Pièce d'identité ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    txtCNI.Focus();
                }
                else if (txtTel.Text.Trim() == "")
                {
                    MessageBox.Show("Vous devez saisir le numéro de téléphone de l'enleveur.", "Téléphone ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    txtTel.Focus();
                }
                else if (!txtSortiePrev.SelectedDate.HasValue)
                {
                    MessageBox.Show("Vous devez une date de sortie prévisionnelle.", "Date de sortie previsionnelle?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    txtSortiePrev.Focus();
                }
                else if (txtNumBESC.Text.Trim() == "")
                {
                    MessageBox.Show("Vous devez saisir le numéro de BESC de ce conventionnel.", "N° de BESC ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    txtNumBESC.Focus();
                }
                else
                {
                    CONVENTIONNEL conv = vsomAcc.UpdateInfosEnlevementGC(conventionnels.ElementAt<CONVENTIONNEL>(cbNumGC.SelectedIndex).IdGC, txtSortiePrev.SelectedDate.Value, txtNumBESC.Text, txtEnleveur.Text, txtCNI.Text, txtTel.Text, utilisateur.IdU);

                    conventionnels = vsp.GetConventionnelsOfConnaissement(conv.IdBL.Value);
                    convs = new List<string>();
                    foreach (CONVENTIONNEL v in conventionnels)
                    {
                        convs.Add(v.NumGC);
                    }
                    cbNumGC.ItemsSource = null;
                    cbNumGC.ItemsSource = convs;
                    cbNumGC.SelectedItem = conv.NumGC;

                    if (conv.IdBL != 0)
                    {
                        formLoader.LoadConnaissementForm(baeForm, conv.CONNAISSEMENT);
                    }
                    else
                    {

                    }

                    MessageBox.Show("Les informations d'enlèvement de ce conventionnel ont été enregistrées", "Enlèvement conventionnel enregistré !", MessageBoxButton.OK, MessageBoxImage.Information);
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

            }
        }

        private void cbNumGC_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (cbNumGC.Items.Count != 0)
                {
                    int indexGC = cbNumGC.SelectedIndex;
                    CONVENTIONNEL conv = conventionnels.ElementAt<CONVENTIONNEL>(indexGC);
                    txtBarCode.Text = conv.BarCode;
                    txtCNI.Text = conv.CNIEnGC;
                    txtDescription.Text = conv.DescGC;
                    txtEnleveur.Text = conv.NomEnGC;
                    if (conv.DSPGC.HasValue)
                    {
                        txtSortiePrev.SelectedDate = conv.DSPGC.Value;
                    }
                    if (conv.FSGC.HasValue)
                    {
                        txtFinSejour.SelectedDate = conv.FSGC;
                    }
                    txtHautC.Text = conv.HautCGC.ToString();
                    txtIdGC.Text = conv.IdGC.ToString();
                    txtLargC.Text = conv.LargCGC.ToString();
                    txtLongC.Text = conv.LongCGC.ToString();
                    txtPoidsC.Text = conv.PoidsCGC.ToString();
                    txtTel.Text = conv.TelenGC;
                    txtVolC.Text = conv.VolCGC.ToString();
                    txtNumBESC.Text = conv.NumBESCGC;

                    this.Title = "Détails enlèvement du conventionnel : " + conv.NumGC + " - Consignée - " + conv.CONNAISSEMENT.ConsigneeBL;
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
