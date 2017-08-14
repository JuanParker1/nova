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
    /// Logique d'interaction pour DetailsLivraisonGCForm.xaml
    /// </summary>
    public partial class DetailsLivraisonGCForm : Window
    {

        private DemandeLivraisonForm livForm;
        private List<CONVENTIONNEL> conventionnels;
        public List<string> convs { get; set; }

        private UTILISATEUR utilisateur;
        private List<OPERATION> operationsUser;

        private FormLoader formLoader;
        private VsomParameters vsp = new VsomParameters();
        public DetailsLivraisonGCForm(DemandeLivraisonForm form, int idBL, UTILISATEUR user)
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
                else if (txtNumAttDedouanement.Text.Trim() == "")
                {
                    MessageBox.Show("Vous devez saisir le numéro de l'attestation de dédouanement de ce conventionnel.", "Attestation de dédouanement ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    txtNumAttDedouanement.Focus();
                }
                else if (txtNumSGSCivio.Text.Trim() == "")
                {
                    MessageBox.Show("Vous devez saisir le numéro SGS (CIVIO) de ce conventionnel.", "N° CIVIO ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    txtNumSGSCivio.Focus();
                }
                else if (txtNumDeclDouane.Text.Trim() == "")
                {
                    MessageBox.Show("Vous devez saisir le numéro déclaration douane de ce conventionnel.", "N° Déclaration douane ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    txtNumDeclDouane.Focus();
                }
                else if (txtNumQuittanceDouane.Text.Trim() == "")
                {
                    MessageBox.Show("Vous devez saisir le numéro de quittance douane de ce conventionnel.", "N° Quittance douane ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    txtNumQuittanceDouane.Focus();
                }
                else if (txtNumFactPAD.Text.Trim() == "")
                {
                    MessageBox.Show("Vous devez saisir le numéro de facture PAD de ce conventionnel.", "N° Facture PAD ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    txtNumFactPAD.Focus();
                }
                else if (txtNumQuittancePAD.Text.Trim() == "")
                {
                    MessageBox.Show("Vous devez saisir le numéro quittance PAD de ce conventionnel.", "N° Quittance PAD ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    txtNumQuittancePAD.Focus();
                }
                else if (txtNumBAE.Text.Trim() == "")
                {
                    MessageBox.Show("Vous devez saisir le numéro du BAE PAD de ce conventionnel.", "N° BAE PAD ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    txtNumBAE.Focus();
                }
                else if (txtNumBESC.Text.Trim() == "")
                {
                    MessageBox.Show("Vous devez saisir le numéro de BESC de ce conventionnel.", "N° BESC ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    txtNumBESC.Focus();
                }
                else if (txtNumSydonia.Text.Trim() == "")
                {
                    MessageBox.Show("Vous devez saisir le numéro SYDONIA de ce conventionnel.", "N° Sydonia ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    txtNumSydonia.Focus();
                }
                else
                {
                    CONVENTIONNEL conv = vsomAcc.UpdateInfosLivraisonGC(conventionnels.ElementAt<CONVENTIONNEL>(cbNumGC.SelectedIndex).IdGC, txtEnleveur.Text, txtCNI.Text, txtTel.Text, txtNumAttDedouanement.Text, txtNumSGSCivio.Text, txtNumDeclDouane.Text, txtNumQuittanceDouane.Text, txtNumFactPAD.Text, txtNumQuittancePAD.Text, txtNumBAE.Text, txtNumBESC.Text, txtNumSydonia.Text, utilisateur.IdU);

                    conventionnels = vsp.GetConventionnelsOfConnaissement(conv.IdBL.Value);
                    convs = new List<string>();
                    foreach (CONVENTIONNEL c in conventionnels)
                    {
                        convs.Add(c.NumGC);
                    }
                    cbNumGC.ItemsSource = null;
                    cbNumGC.ItemsSource = convs;
                    cbNumGC.SelectedItem = conv.NumGC;

                    formLoader.LoadConnaissementForm(livForm, conv.CONNAISSEMENT, 0);

                    MessageBox.Show("Les informations de livraison de ce conventionnel ont été enregistrées", "Livraison conventionnel enregistré !", MessageBoxButton.OK, MessageBoxImage.Information);
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
                    CONVENTIONNEL veh = conventionnels.ElementAt<CONVENTIONNEL>(indexGC);
                    txtBarCode.Text = veh.BarCode;
                    txtCNI.Text = veh.CNIEnGC;
                    txtDescription.Text = veh.DescGC;
                    txtEnleveur.Text = veh.NomEnGC;
                    if (veh.FFGC.HasValue)
                    {
                        txtFinFranchise.SelectedDate = veh.FFGC.Value;
                    }
                    if (veh.FSGC.HasValue)
                    {
                        txtFinSejour.SelectedDate = veh.FSGC;
                    }
                    txtHautC.Text = veh.HautCGC.ToString();
                    txtIdGC.Text = veh.IdGC.ToString();
                    txtLargC.Text = veh.LargCGC.ToString();
                    txtLongC.Text = veh.LongCGC.ToString();
                    txtPoidsC.Text = veh.PoidsCGC.ToString();
                    txtTel.Text = veh.TelenGC;
                    txtVolC.Text = veh.VolCGC.ToString();

                    txtNumAttDedouanement.Text = veh.NumADDGC;
                    txtNumBAE.Text = veh.NumAEPADGC;
                    txtNumBESC.Text = veh.NumBESCGC;
                    txtNumDeclDouane.Text = veh.NumDDGC;
                    txtNumFactPAD.Text = veh.NumFPADGC;
                    txtNumQuittanceDouane.Text = veh.NumQDGC;
                    txtNumQuittancePAD.Text = veh.NumQPADGC;
                    txtNumSGSCivio.Text = veh.NumCIVIOGC;
                    txtNumSydonia.Text = veh.NumSydoniaGC;

                    this.Title = "Détails livraison du conventionnel : " + veh.NumGC + " - Consignée - " + veh.CONNAISSEMENT.ConsigneeBL;
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
