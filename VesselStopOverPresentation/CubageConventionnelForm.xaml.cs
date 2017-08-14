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
    /// Logique d'interaction pour CubageConventionnelForm.xaml
    /// </summary>
    public partial class CubageConventionnelForm : Window
    {

        private ConventionnelForm convForm;
        private ConventionnelPanel convPanel;
        private CubageForm cubForm;

        private CONVENTIONNEL conventionnel;

        private List<TYPE_CONVENTIONNEL> typesConventionnels;
        public List<string> tps { get; set; }

        private List<OPERATION> operationsUser;
        private UTILISATEUR utilisateur;

        private FormLoader formLoader;
        private VsomParameters vsp = new VsomParameters();

        public CubageConventionnelForm(ConventionnelForm form, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();
                //VsomParameters vsprm = new VsomParameters();

                InitializeComponent();
                this.DataContext = this;

                typesConventionnels = vsp.GetTypesConventionnelsImport();
                tps = new List<string>();
                foreach (TYPE_CONVENTIONNEL t in typesConventionnels)
                {
                    tps.Add(t.CodeTypeGC);
                }

                utilisateur = user;
                operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);

                convForm = form;

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

        public CubageConventionnelForm(ConventionnelPanel panel, CONVENTIONNEL conv, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();
               // VsomParameters vsprm = new VsomParameters();
                InitializeComponent();
                this.DataContext = this;

                typesConventionnels = vsp.GetTypesConventionnelsImport();
                tps = new List<string>();
                foreach (TYPE_CONVENTIONNEL t in typesConventionnels)
                {
                    tps.Add(t.CodeTypeGC);
                }

                conventionnel = conv;

                utilisateur = user;
                operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);

                convPanel = panel;

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

        public CubageConventionnelForm(CubageForm form, CONVENTIONNEL conv, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();
                //VsomParameters vsprm = new VsomParameters();
                InitializeComponent();
                this.DataContext = this;

                typesConventionnels = vsp.GetTypesConventionnelsImport();
                tps = new List<string>();
                foreach (TYPE_CONVENTIONNEL t in typesConventionnels)
                {
                    tps.Add(t.CodeTypeGC);
                }

                conventionnel = conv;

                utilisateur = user;
                operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);

                cubForm = form;

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

        private void btnCuber_Click(object sender, RoutedEventArgs e)
        {
            try
            {
               // VSOMAccessors vsomAcc = new VSOMAccessors();
                VsomMarchal vsomAcc = new VsomMarchal();

                if (txtLongC.Text == "" && txtLargC.Text == "" && txtHautC.Text == "" && txtVolC.Text == "")
                {
                    MessageBox.Show("Veuillez remplir les dimensions du véhicule", "Dimensions", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    if (convForm != null)
                    {
                        CONVENTIONNEL conv = vsomAcc.CuberConventionnel(Convert.ToInt32(convForm.txtIdGC.Text), Convert.ToDouble(txtLongC.Text.Replace(".", ",")), Convert.ToDouble(txtLargC.Text.Replace(".", ",")), Convert.ToDouble(txtHautC.Text.Replace(".", ",")), Convert.ToDouble(txtVolC.Text.Replace(".", ",")), new TextRange(txtObservations.Document.ContentStart, txtObservations.Document.ContentEnd).Text, utilisateur.IdU);
                        //Raffraîchir les informations
                        formLoader.LoadConventionnelForm(convForm, conv);
                    }
                    else if (convPanel != null)
                    {
                        CONVENTIONNEL conv = vsomAcc.CuberConventionnel(conventionnel.IdGC, Convert.ToDouble(txtLongC.Text.Replace(".", ",")), Convert.ToDouble(txtLargC.Text.Replace(".", ",")), Convert.ToDouble(txtHautC.Text.Replace(".", ",")), Convert.ToDouble(txtVolC.Text.Replace(".", ",")), new TextRange(txtObservations.Document.ContentStart, txtObservations.Document.ContentEnd).Text, utilisateur.IdU);
                        if (convPanel.cbFiltres.SelectedIndex != 0)
                        {
                            convPanel.cbFiltres.SelectedIndex = 0;
                        }
                        else
                        {
                            convPanel.conventionnels = vsp.GetConventionnelsImport();
                            convPanel.dataGrid.ItemsSource = convPanel.conventionnels;
                        }
                    }

                    MessageBox.Show("L'opération de cubage s'est déroulée avec succès, consultez le journal des éléments de facturation", "General cargo cubé !", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                
            }
            catch (HabilitationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (EnregistrementInexistant ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        private void txtDim_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9,.]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void txtDim_LostFocus(object sender, RoutedEventArgs e)
        {
            // vérifier qu'il y a des valeurs dans les champs
            try
            {
                if (txtLongC.Text != "" && txtLargC.Text != "" && txtHautC.Text != "")
                {
                    txtVolC.Text = Math.Round((Convert.ToDouble(txtLongC.Text.Replace(".", ",")) * Convert.ToDouble(txtLargC.Text.Replace(".", ",")) * Convert.ToDouble(txtHautC.Text.Replace(".", ","))), 3).ToString();
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
