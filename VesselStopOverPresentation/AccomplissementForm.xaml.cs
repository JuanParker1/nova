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
    /// Logique d'interaction pour AccomplissementForm.xaml
    /// </summary>
    public partial class AccomplissementForm : Window
    {
        private ConnaissementForm conForm;

        private CONNAISSEMENT connaissement;
        private List<OPERATION> operationsUser;
        private UTILISATEUR utilisateur;

        private FormLoader formLoader;
        //private VsomParameters vsp = new VsomParameters();
        private VSOMAccessors vsomAcc;
        public AccomplissementForm(ConnaissementForm form, CONNAISSEMENT con, UTILISATEUR user)
        {
            try
            {
                 vsomAcc = new VSOMAccessors();

                InitializeComponent();
                connaissement = con;
                txtNomMandataire.Text = connaissement.NomManBL;
                txtCNI.Text = connaissement.CNIManBL;
                txtDelivreeLe.SelectedDate = connaissement.DDCNIManBL.HasValue ? connaissement.DDCNIManBL : null;
                txtDelivreeA.Text = connaissement.LDCNIManBL;
                txtTelephone.Text = connaissement.PhoneManBL;
                txtEmail.Text = connaissement.EmailBL;
                txtNumContribuable.Text = connaissement.NContribBL;
                txtObservations.Document.Blocks.Clear();
                txtObservations.Document.Blocks.Add(new Paragraph(new Run(connaissement.AIBL)));
                lblStatut.Content = connaissement.DateAccBL.HasValue ? "Date d'accomplissement : " + connaissement.DateAccBL : "";
                if (connaissement.StatutBL == "Cloturé")
                {
                    btnEnregistrer.IsEnabled = false;
                }
                conForm = form;

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
                //VsomMarchal vsomAcc = new VsomMarchal();

                if (txtNomMandataire.Text.Trim() == "")
                {
                    MessageBox.Show("Vous devez saisir le nom du mandataire de ce connaissement", "Mandataire ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    txtNomMandataire.Focus();
                }
                else if (txtCNI.Text.Trim() == "")
                {
                    MessageBox.Show("Vous devez saisir le numéro de la pièce d'identifié de ce mandataire.", "N° Pièce d'identité ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    txtCNI.Focus();
                }
                else if (txtTelephone.Text.Trim() == "")
                {
                    MessageBox.Show("Vous devez saisir le numéro de téléphone de ce mandataire.", "Téléphone ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    txtTelephone.Focus();
                }
                else
                {
                    connaissement = vsomAcc.AccomplirBL(connaissement.IdBL, txtNomMandataire.Text, txtCNI.Text, txtDelivreeLe.SelectedDate.Value, txtDelivreeA.Text, txtTelephone.Text, txtEmail.Text, txtNumContribuable.Text, new TextRange(txtObservations.Document.ContentStart, txtObservations.Document.ContentEnd).Text, utilisateur.IdU);

                    formLoader.LoadConnaissementForm(conForm, connaissement);

                    MessageBox.Show("Les informations sur le mandataire ont été mise mises à jour avec succès. Le connaissement est à présent accompli.", "Connaissement accompli !", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (EnregistrementInexistant ex)
            {
                MessageBox.Show(ex.Message, "Enregistrement inexistant !", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        private void txtCNI_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
