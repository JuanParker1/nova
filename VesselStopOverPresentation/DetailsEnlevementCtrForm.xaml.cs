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
    /// Logique d'interaction pour DetailsEnlevementCtrForm.xaml
    /// </summary>
    public partial class DetailsEnlevementCtrForm : Window
    {

        private BonEnleverForm baeForm;
        private List<CONTENEUR> conteneurs;
        public List<string> ctrs { get; set; }

        private UTILISATEUR utilisateur;
        private List<OPERATION> operationsUser;

        private FormLoader formLoader;

        private List<TYPE_CONTENEUR> typesConteneur;
        public List<string> typesCtr { get; set; }
        private VsomParameters vsp = new VsomParameters();
        public DetailsEnlevementCtrForm(BonEnleverForm form, int idBL, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();
                //VsomParameters vsp = new VsomParameters();
                InitializeComponent();
                this.DataContext = this;

                conteneurs = vsp.GetConteneursImportOfConnaissement(idBL);
                ctrs = new List<string>();
                foreach (CONTENEUR ctr in conteneurs)
                {
                    ctrs.Add(ctr.NumCtr);
                }

                typesConteneur = vsp.GetTypesConteneurs();
                typesCtr = new List<string>();
                foreach (TYPE_CONTENEUR t in typesConteneur)
                {
                    typesCtr.Add(t.CodeTypeCtr);
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

                if (cbNumCtr.SelectedIndex == -1)
                {
                    MessageBox.Show("Vous devez saisir sélectionner un conteneur", "Conteneur ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (txtEnleveur.Text.Trim() == "")
                {
                    MessageBox.Show("Vous devez saisir le nom de l'enleveur du conteneur", "Enleveur ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
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
                    MessageBox.Show("Vous devez saisir le numéro de BESC de ce conteneur.", "N° de BESC ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    txtNumBESC.Focus();
                }
                else
                {
                    CONTENEUR ctr = vsomAcc.UpdateInfosEnlevementCtr(conteneurs.ElementAt<CONTENEUR>(cbNumCtr.SelectedIndex).IdCtr, txtSortiePrev.SelectedDate.Value, txtNumBESC.Text, txtEnleveur.Text, txtCNI.Text, txtTel.Text, utilisateur.IdU);

                    conteneurs = vsp.GetConteneursImportOfConnaissement(ctr.IdBL.Value);
                    ctrs = new List<string>();
                    foreach (CONTENEUR c in conteneurs)
                    {
                        ctrs.Add(c.NumCtr);
                    }
                    cbNumCtr.ItemsSource = null;
                    cbNumCtr.ItemsSource = ctrs;
                    cbNumCtr.SelectedItem = ctr.NumCtr;

                    formLoader.LoadConnaissementForm(baeForm, ctr.CONNAISSEMENT);

                    MessageBox.Show("Les informations d'enlèvement de ce conteneur ont été enregistrées", "Enlèvement conteneur enregistré !", MessageBoxButton.OK, MessageBoxImage.Information);
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

        private void cbNumCtr_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (cbNumCtr.SelectedIndex != -1)
                {
                    int indexCtr = cbNumCtr.SelectedIndex;
                    CONTENEUR ctr = conteneurs.ElementAt<CONTENEUR>(indexCtr);
                    cbTypeCtr.SelectedItem = ctr.TypeCCtr;
                    txtCNI.Text = ctr.CNIEnCtr;
                    txtDescription.Text = ctr.DescCtr;
                    txtEnleveur.Text = ctr.NomEnCtr;
                    txtFinSejour.SelectedDate = ctr.FSCtr;
                    txtIdCtr.Text = ctr.IdCtr.ToString();
                    txtMarchandise.Text = ctr.DescMses;
                    txtPoids.Text = ctr.PoidsCCtr.ToString();
                    txtSeal1.Text = ctr.Seal1Ctr;
                    txtSeal2.Text = ctr.Seal2Ctr;
                    if (ctr.DSPCtr.HasValue)
                    {
                        txtSortiePrev.SelectedDate = ctr.DSPCtr.Value;
                    }
                    txtSurestaries.SelectedDate = ctr.FFSCtr;
                    txtTel.Text = ctr.TelenCtr;
                    txtNumBESC.Text = ctr.NumBESCCtr;

                    this.Title = "Détails enlèvement du conteneur : " + ctr.NumCtr + " - Consignée - " + ctr.CONNAISSEMENT.ConsigneeBL;
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
