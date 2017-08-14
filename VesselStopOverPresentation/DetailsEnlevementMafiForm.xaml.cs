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
    /// Logique d'interaction pour DetailsEnlevementMafiForm.xaml
    /// </summary>
    public partial class DetailsEnlevementMafiForm : Window
    {

        private BonEnleverForm baeForm;
        private List<MAFI> mafis;
        public List<string> mfs { get; set; }

        private UTILISATEUR utilisateur;
        private List<OPERATION> operationsUser;

        private FormLoader formLoader;
        private VsomParameters vsp = new VsomParameters();
        public DetailsEnlevementMafiForm(BonEnleverForm form, int idBL, UTILISATEUR user)
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
                else if (!txtSortiePrev.SelectedDate.HasValue)
                {
                    MessageBox.Show("Vous devez une date de sortie prévisionnelle.", "Date de sortie previsionnelle?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    txtSortiePrev.Focus();
                }
                else if (txtNumBESC.Text.Trim() == "")
                {
                    MessageBox.Show("Vous devez saisir le numéro de BESC de ce mafi.", "N° de BESC ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    txtNumBESC.Focus();
                }
                else
                {
                    MAFI mafi = vsomAcc.UpdateInfosEnlevementMafi(mafis.ElementAt<MAFI>(cbNumMafi.SelectedIndex).IdMafi, txtSortiePrev.SelectedDate.Value, txtNumBESC.Text, txtEnleveur.Text, txtCNI.Text, txtTel.Text, utilisateur.IdU);

                    mafis = vsp.GetMafisImportOfConnaissement(mafi.IdBL.Value);
                    mfs = new List<string>();
                    foreach (MAFI c in mafis)
                    {
                        mfs.Add(c.NumMafi);
                    }
                    cbNumMafi.ItemsSource = null;
                    cbNumMafi.ItemsSource = mfs;
                    cbNumMafi.SelectedItem = mafi.NumMafi;

                    formLoader.LoadConnaissementForm(baeForm, mafi.CONNAISSEMENT);

                    MessageBox.Show("Les informations d'enlèvement de ce mafi ont été enregistrées", "Enlèvement mafi enregistré !", MessageBoxButton.OK, MessageBoxImage.Information);
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
                if (cbNumMafi.SelectedIndex != -1)
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
                    if (mafi.FFMafi.HasValue)
                    {
                        txtFranSej.SelectedDate = mafi.FFMafi.Value;
                    }
                    if (mafi.DSPMafi.HasValue)
                    {
                        txtSortiePrev.SelectedDate = mafi.DSPMafi.Value;
                    }
                    txtSurestaries.SelectedDate = mafi.FFSMafi;
                    txtTel.Text = mafi.TelenMafi;
                    txtNumBESC.Text = mafi.NumBESCMafi;

                    this.Title = "Détails enlèvement du mafi : " + mafi.NumMafi + " - Consignée - " + mafi.CONNAISSEMENT.ConsigneeBL;
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
