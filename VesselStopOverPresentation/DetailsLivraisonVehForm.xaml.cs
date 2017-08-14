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
    /// Logique d'interaction pour DetailsLivraisonVehForm.xaml
    /// </summary>
    public partial class DetailsLivraisonVehForm : Window
    {

        private DemandeLivraisonForm livForm;
        private List<VEHICULE> vehicules;
        public List<string> vehs { get; set; }

        private UTILISATEUR utilisateur;
        private List<OPERATION> operationsUser;

        private FormLoader formLoader;

        private VsomParameters vsp = new VsomParameters();

        public DetailsLivraisonVehForm(DemandeLivraisonForm form, int idBL, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

                vehicules = vsp.GetVehiculesOfConnaissement(idBL);
                vehs = new List<string>();
                foreach (VEHICULE veh in vehicules)
                {
                    vehs.Add(veh.NumChassis);
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

                if (cbNumChassis.SelectedIndex == -1)
                {
                    MessageBox.Show("Vous devez saisir sélectionner un véhicule", "Véhicule ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (txtEnleveur.Text.Trim() == "")
                {
                    MessageBox.Show("Vous devez saisir le nom de l'enleveur du véhicule", "Enleveur ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
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
                    MessageBox.Show("Vous devez saisir le numéro de l'attestation de dédouanement de ce véhicule.", "Attestation de dédouanement ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    txtNumAttDedouanement.Focus();
                }
                else if (txtNumSGSCivio.Text.Trim() == "")
                {
                    MessageBox.Show("Vous devez saisir le numéro SGS (CIVIO) de ce véhicule.", "N° CIVIO ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    txtNumSGSCivio.Focus();
                }
                else if (txtNumDeclDouane.Text.Trim() == "")
                {
                    MessageBox.Show("Vous devez saisir le numéro déclaration douane de ce véhicule.", "N° Déclaration douane ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    txtNumDeclDouane.Focus();
                }
                else if (txtNumQuittanceDouane.Text.Trim() == "")
                {
                    MessageBox.Show("Vous devez saisir le numéro de quittance douane de ce véhicule.", "N° Quittance douane ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    txtNumQuittanceDouane.Focus();
                }
                else if (txtNumFactPAD.Text.Trim() == "")
                {
                    MessageBox.Show("Vous devez saisir le numéro de facture PAD de ce véhicule.", "N° Facture PAD ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    txtNumFactPAD.Focus();
                }
                else if (txtNumQuittancePAD.Text.Trim() == "")
                {
                    MessageBox.Show("Vous devez saisir le numéro quittance PAD de ce véhicule.", "N° Quittance PAD ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    txtNumQuittancePAD.Focus();
                }
                else if (txtNumBAE.Text.Trim() == "")
                {
                    MessageBox.Show("Vous devez saisir le numéro du BAE PAD de ce véhicule.", "N° BAE PAD ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    txtNumBAE.Focus();
                }
                else if (txtNumBESC.Text.Trim() == "")
                {
                    MessageBox.Show("Vous devez saisir le numéro de BESC de ce véhicule.", "N° BESC ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    txtNumBESC.Focus();
                }
                else if (txtNumSydonia.Text.Trim() == "")
                {
                    MessageBox.Show("Vous devez saisir le numéro SYDONIA de ce véhicule.", "N° Sydonia ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    txtNumSydonia.Focus();
                }
                else
                {
                    VEHICULE veh = vsomAcc.UpdateInfosLivraisonVeh(vehicules.ElementAt<VEHICULE>(cbNumChassis.SelectedIndex).IdVeh, txtEnleveur.Text, txtCNI.Text, txtTel.Text, txtNumAttDedouanement.Text, txtNumSGSCivio.Text, txtNumDeclDouane.Text, txtNumQuittanceDouane.Text, txtNumFactPAD.Text, txtNumQuittancePAD.Text, txtNumBAE.Text, txtNumBESC.Text, txtNumSydonia.Text, utilisateur.IdU);

                    vehicules = vsp.GetVehiculesOfConnaissement(veh.IdBL.Value);
                    vehs = new List<string>();
                    foreach (VEHICULE v in vehicules)
                    {
                        vehs.Add(v.NumChassis);
                    }
                    cbNumChassis.ItemsSource = null;
                    cbNumChassis.ItemsSource = vehs;
                    cbNumChassis.SelectedItem = veh.NumChassis;

                    formLoader.LoadConnaissementForm(livForm, veh.CONNAISSEMENT, 0);

                    MessageBox.Show("Les informations de livraison de ce véhicule ont été enregistrées", "Livraison véhicule enregistré !", MessageBoxButton.OK, MessageBoxImage.Information);
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

        private void cbNumChassis_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (cbNumChassis.Items.Count != 0)
                {
                    int indexVeh = cbNumChassis.SelectedIndex;
                    VEHICULE veh = vehicules.ElementAt<VEHICULE>(indexVeh);
                    txtBarCode.Text = veh.BarCode;
                    txtCNI.Text = veh.CNIEnVeh;
                    txtDescription.Text = veh.DescVeh;
                    txtEnleveur.Text = veh.NomEnVeh;
                    txtEtatC.Text = veh.StatutCVeh;
                    if (veh.FFVeh.HasValue)
                    {
                        txtFinFranchise.SelectedDate = veh.FFVeh.Value;
                    }
                    if (veh.FSVeh.HasValue)
                    {
                        txtFinSejour.SelectedDate = veh.FSVeh;
                    }
                    txtHautC.Text = veh.HautCVeh.ToString();
                    txtIdChassis.Text = veh.IdVeh.ToString();
                    txtLargC.Text = veh.LargCVeh.ToString();
                    txtLongC.Text = veh.LongCVeh.ToString();
                    txtPoidsC.Text = veh.PoidsCVeh.ToString();
                    txtTel.Text = veh.TelenVeh;
                    txtVolC.Text = veh.VolCVeh.ToString();

                    txtNumAttDedouanement.Text = veh.NumADDVeh;
                    txtNumBAE.Text = veh.NumAEPADVeh;
                    txtNumBESC.Text = veh.NumBESCVeh;
                    txtNumDeclDouane.Text = veh.NumDDVeh;
                    txtNumFactPAD.Text = veh.NumFPADVeh;
                    txtNumQuittanceDouane.Text = veh.NumQDVeh;
                    txtNumQuittancePAD.Text = veh.NumQPADVeh;
                    txtNumSGSCivio.Text = veh.NumCIVIOveh;
                    txtNumSydonia.Text = veh.NumSydoniaVeh;

                    this.Title = "Détails livraison du véhicule : " + veh.NumChassis + " - Consignée - " + veh.CONNAISSEMENT.ConsigneeBL;
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
