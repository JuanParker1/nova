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
    /// Logique d'interaction pour DetailsLivraisonMafiForm.xaml
    /// </summary>
    public partial class DetailsLivraisonMafiForm : Window
    {

        private DemandeLivraisonForm livForm;
        private List<MAFI> mafis;
        public List<string> mfs { get; set; }

        private UTILISATEUR utilisateur;
        private List<OPERATION> operationsUser;

        private FormLoader formLoader;
        private VsomParameters vsp = new VsomParameters();
        public DetailsLivraisonMafiForm(DemandeLivraisonForm form, int idBL, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

                mafis = vsp.GetMafisImportOfConnaissement(idBL);
                mfs = new List<string>();
                foreach (MAFI mf in mafis)
                {
                    mfs.Add(mf.NumMafi);
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
               // VSOMAccessors vsomAcc = new VSOMAccessors();
                VsomMarchal vsomAcc = new VsomMarchal();

                if (cbNumMafi.SelectedIndex == -1)
                {
                    MessageBox.Show("Vous devez saisir sélectionner un mafi", "Mafi ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (txtEnleveur.Text.Trim() == "")
                {
                    MessageBox.Show("Vous devez saisir le nom de l'enleveur du mafi", "Enleveur ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
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
                    MessageBox.Show("Vous devez saisir le numéro de l'attestation de dédouanement de ce mafi.", "Attestation de dédouanement ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    txtNumAttDedouanement.Focus();
                }
                else if (txtNumSGSAVI.Text.Trim() == "")
                {
                    MessageBox.Show("Vous devez saisir le numéro SGS (AVI) de ce mafi.", "N° AVI ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    txtNumSGSAVI.Focus();
                }
                else if (txtNumDeclDouane.Text.Trim() == "")
                {
                    MessageBox.Show("Vous devez saisir le numéro déclaration douane de ce mafi.", "N° Déclaration douane ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    txtNumDeclDouane.Focus();
                }
                else if (txtNumQuittanceDouane.Text.Trim() == "")
                {
                    MessageBox.Show("Vous devez saisir le numéro de quittance douane de ce mafi.", "N° Quittance douane ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    txtNumQuittanceDouane.Focus();
                }
                else if (txtNumFactPAD.Text.Trim() == "")
                {
                    MessageBox.Show("Vous devez saisir le numéro de facture PAD de ce mafi.", "N° Facture PAD ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    txtNumFactPAD.Focus();
                }
                else if (txtNumQuittancePAD.Text.Trim() == "")
                {
                    MessageBox.Show("Vous devez saisir le numéro quittance PAD de ce mafi.", "N° Quittance PAD ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    txtNumQuittancePAD.Focus();
                }
                else if (txtNumBAE.Text.Trim() == "")
                {
                    MessageBox.Show("Vous devez saisir le numéro du BAE PAD de ce mafi.", "N° BAE PAD ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    txtNumBAE.Focus();
                }
                else if (txtNumBESC.Text.Trim() == "")
                {
                    MessageBox.Show("Vous devez saisir le numéro de BESC de ce mafi.", "N° BESC ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    txtNumBESC.Focus();
                }
                else if (txtNumSydonia.Text.Trim() == "")
                {
                    MessageBox.Show("Vous devez saisir le numéro SYDONIA de ce mafi.", "N° Sydonia ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    txtNumSydonia.Focus();
                }
                else
                {
                    MAFI mafi = vsomAcc.UpdateInfosLivraisonMafi(mafis.ElementAt<MAFI>(cbNumMafi.SelectedIndex).IdMafi, txtEnleveur.Text, txtCNI.Text, txtTel.Text, txtNumAttDedouanement.Text, txtNumSGSAVI.Text, txtNumDeclDouane.Text, txtNumQuittanceDouane.Text, txtNumFactPAD.Text, txtNumQuittancePAD.Text, txtNumBAE.Text, txtNumBESC.Text, txtNumSydonia.Text, utilisateur.IdU);

                    mafis = vsp.GetMafisImportOfConnaissement(mafi.IdBL.Value);
                    mfs = new List<string>();
                    foreach (MAFI c in mafis)
                    {
                        mfs.Add(c.NumMafi);
                    }
                    cbNumMafi.ItemsSource = null;
                    cbNumMafi.ItemsSource = mfs;
                    cbNumMafi.SelectedItem = mafi.NumMafi;

                    formLoader.LoadConnaissementForm(livForm, mafi.CONNAISSEMENT, 0);

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

        private void cbNumMafi_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (cbNumMafi.Items.Count != 0)
                {
                    int indexMafi = cbNumMafi.SelectedIndex;
                    MAFI mafi = mafis.ElementAt<MAFI>(indexMafi);
                    txtTypeMafi.Text = mafi.TypeCMafi;
                    txtCNI.Text = mafi.CNIEnMafi;
                    txtDescription.Text = mafi.DescMafi;
                    txtEnleveur.Text = mafi.NomEnMafi;
                    txtFinSejour.SelectedDate = mafi.FSMafi;
                    txtIdMafi.Text = mafi.IdMafi.ToString();
                    txtMarchandise.Text = mafi.DescMses;
                    txtPoids.Text = mafi.PoidsCMafi.ToString();
                    txtSejour.SelectedDate = mafi.FFMafi;
                    txtSurestaries.SelectedDate = mafi.FFSMafi;
                    txtTel.Text = mafi.TelenMafi;

                    txtNumAttDedouanement.Text = mafi.NumADDMafi;
                    txtNumBAE.Text = mafi.NumAEPADMafi;
                    txtNumBESC.Text = mafi.NumBESCMafi;
                    txtNumDeclDouane.Text = mafi.NumDDMafi;
                    txtNumFactPAD.Text = mafi.NumFPADMafi;
                    txtNumQuittanceDouane.Text = mafi.NumQDMafi;
                    txtNumQuittancePAD.Text = mafi.NumQPADMafi;
                    txtNumSGSAVI.Text = mafi.NumAVIMafi;
                    txtNumSydonia.Text = mafi.NumSydoniaMafi;

                    this.Title = "Détails livraison du mafi : " + mafi.NumMafi + " - Consignée - " + mafi.CONNAISSEMENT.ConsigneeBL;
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
