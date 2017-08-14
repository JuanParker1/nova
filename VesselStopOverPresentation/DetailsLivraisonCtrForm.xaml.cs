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
    /// Logique d'interaction pour DetailsLivraisonCtrForm.xaml
    /// </summary>
    public partial class DetailsLivraisonCtrForm : Window
    {

        private DemandeLivraisonForm livForm;
        private List<CONTENEUR> conteneurs;
        public List<string> ctrs { get; set; }

        private UTILISATEUR utilisateur;
        private List<OPERATION> operationsUser;

        private FormLoader formLoader;

        private List<TYPE_CONTENEUR> typesConteneur;
        public List<string> typesCtr { get; set; }
        private VsomParameters vsp = new VsomParameters();
        public DetailsLivraisonCtrForm(DemandeLivraisonForm form, int idBL, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();
                VsomParameters vsp = new VsomParameters();
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
                    typesCtr.Add(t.LibTypeCtr);
                }

                livForm = form;

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
                else if (txtNumAttDedouanement.Text.Trim() == "")
                {
                    MessageBox.Show("Vous devez saisir le numéro de l'attestation de dédouanement de ce conteneur.", "Attestation de dédouanement ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    txtNumAttDedouanement.Focus();
                }
                else if (txtNumSGSAVI.Text.Trim() == "")
                {
                    MessageBox.Show("Vous devez saisir le numéro SGS (AVI) de ce conteneur.", "N° AVI ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    txtNumSGSAVI.Focus();
                }
                else if (txtNumDeclDouane.Text.Trim() == "")
                {
                    MessageBox.Show("Vous devez saisir le numéro déclaration douane de ce conteneur.", "N° Déclaration douane ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    txtNumDeclDouane.Focus();
                }
                else if (txtNumQuittanceDouane.Text.Trim() == "")
                {
                    MessageBox.Show("Vous devez saisir le numéro de quittance douane de ce conteneur.", "N° Quittance douane ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    txtNumQuittanceDouane.Focus();
                }
                else if (txtNumFactPAD.Text.Trim() == "")
                {
                    MessageBox.Show("Vous devez saisir le numéro de facture PAD de ce conteneur.", "N° Facture PAD ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    txtNumFactPAD.Focus();
                }
                else if (txtNumQuittancePAD.Text.Trim() == "")
                {
                    MessageBox.Show("Vous devez saisir le numéro quittance PAD de ce conteneur.", "N° Quittance PAD ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    txtNumQuittancePAD.Focus();
                }
                else if (txtNumBAE.Text.Trim() == "")
                {
                    MessageBox.Show("Vous devez saisir le numéro du BAE PAD de ce conteneur.", "N° BAE PAD ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    txtNumBAE.Focus();
                }
                else if (txtNumBESC.Text.Trim() == "")
                {
                    MessageBox.Show("Vous devez saisir le numéro de BESC de ce conteneur.", "N° BESC ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    txtNumBESC.Focus();
                }
                else if (txtNumSydonia.Text.Trim() == "")
                {
                    MessageBox.Show("Vous devez saisir le numéro SYDONIA de ce conteneur.", "N° Sydonia ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    txtNumSydonia.Focus();
                }
                else
                {
                    CONTENEUR ctr = vsomAcc.UpdateInfosLivraisonCtr(conteneurs.ElementAt<CONTENEUR>(cbNumCtr.SelectedIndex).IdCtr, txtEnleveur.Text, txtCNI.Text, txtTel.Text, txtNumAttDedouanement.Text, txtNumSGSAVI.Text, txtNumDeclDouane.Text, txtNumQuittanceDouane.Text, txtNumFactPAD.Text, txtNumQuittancePAD.Text, txtNumBAE.Text, txtNumBESC.Text, txtNumSydonia.Text, utilisateur.IdU);

                    conteneurs = vsp.GetConteneursImportOfConnaissement(ctr.IdBL.Value);
                    ctrs = new List<string>();
                    foreach (CONTENEUR c in conteneurs)
                    {
                        ctrs.Add(c.NumCtr);
                    }
                    cbNumCtr.ItemsSource = null;
                    cbNumCtr.ItemsSource = ctrs;
                    cbNumCtr.SelectedItem = ctr.NumCtr;

                    formLoader.LoadConnaissementForm(livForm, ctr.CONNAISSEMENT, 0);

                    MessageBox.Show("Les informations de livraison de ce conteneur ont été enregistrées", "Livraison conteneur enregistré !", MessageBoxButton.OK, MessageBoxImage.Information);
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
                if (cbNumCtr.Items.Count != 0)
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
                    txtSejour.SelectedDate = ctr.FFCtr;
                    txtSurestaries.SelectedDate = ctr.FFSCtr;
                    txtTel.Text = ctr.TelenCtr;

                    txtNumAttDedouanement.Text = ctr.NumADDCtr;
                    txtNumBAE.Text = ctr.NumAEPADCtr;
                    txtNumBESC.Text = ctr.NumBESCCtr;
                    txtNumDeclDouane.Text = ctr.NumDDCtr;
                    txtNumFactPAD.Text = ctr.NumFPADCtr;
                    txtNumQuittanceDouane.Text = ctr.NumQDCtr;
                    txtNumQuittancePAD.Text = ctr.NumQPADCtr;
                    txtNumSGSAVI.Text = ctr.NumAVICtr;
                    txtNumSydonia.Text = ctr.NumSydoniaCtr;

                    this.Title = "Détails livraison du conteneur : " + ctr.NumCtr + " - Consignée - " + ctr.CONNAISSEMENT.ConsigneeBL;
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
