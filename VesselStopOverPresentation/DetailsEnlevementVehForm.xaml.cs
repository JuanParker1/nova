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
    /// Logique d'interaction pour DetailsEnlevementVehForm.xaml
    /// </summary>
    public partial class DetailsEnlevementVehForm : Window
    {

        private BonEnleverForm baeForm;
        private List<VEHICULE> vehicules;
        public List<string> vehs { get; set; }

        private UTILISATEUR utilisateur;
        private List<OPERATION> operationsUser;

        private FormLoader formLoader;
        private VsomParameters vsp = new VsomParameters();
        public DetailsEnlevementVehForm(BonEnleverForm form, int idBL, UTILISATEUR user)
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
                else if (!txtSortiePrev.SelectedDate.HasValue)
                {
                    MessageBox.Show("Vous devez une date de sortie prévisionnelle.", "Date de sortie previsionnelle?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    txtSortiePrev.Focus();
                }
                else if (txtNumBESC.Text.Trim() == "")
                {
                    MessageBox.Show("Vous devez saisir le numéro de BESC de ce véhicule.", "N° de BESC ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    txtNumBESC.Focus();
                }
                else
                {
                    VEHICULE veh = vsomAcc.UpdateInfosEnlevementVeh(vehicules.ElementAt<VEHICULE>(cbNumChassis.SelectedIndex).IdVeh, txtSortiePrev.SelectedDate.Value, txtNumBESC.Text, txtEnleveur.Text, txtCNI.Text, txtTel.Text, utilisateur.IdU);

                    vehicules = vsp.GetVehiculesOfConnaissement(veh.IdBL.Value);
                    vehs = new List<string>();
                    foreach (VEHICULE v in vehicules)
                    {
                        vehs.Add(v.NumChassis);
                    }
                    cbNumChassis.ItemsSource = null;
                    cbNumChassis.ItemsSource = vehs;
                    cbNumChassis.SelectedItem = veh.NumChassis;

                    if (veh.IdBL != 0)
                    {
                        formLoader.LoadConnaissementForm(baeForm, veh.CONNAISSEMENT);
                    }
                    else
                    {

                    }

                    MessageBox.Show("Les informations d'enlèvement de ce véhicule ont été enregistrées", "Enlèvement véhicule enregistré !", MessageBoxButton.OK, MessageBoxImage.Information);
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
                    if (veh.DSPVeh.HasValue)
                    {
                        txtSortiePrev.SelectedDate = veh.DSPVeh.Value;
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
                    txtNumBESC.Text = veh.NumBESCVeh;

                    this.Title = "Détails enlèvement du véhicule : " + veh.NumChassis + " - Consignée - " + veh.CONNAISSEMENT.ConsigneeBL;
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
