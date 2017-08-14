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
    /// Logique d'interaction pour DITForm.xaml
    /// </summary>
    public partial class DITForm : Window
    {
        private ConteneurForm contForm;
        private FactureDITForm factDITForm;
        private List<ELEMENT_FACTURATION> elements;
        public List<string> elts { get; set; }

        private UTILISATEUR utilisateur;
        private List<OPERATION> operationsUser;

        private FormLoader formLoader;
        private VsomParameters vsp = new VsomParameters();
        public DITForm(ConteneurForm form, int idCtr, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

                elements = vsp.GetElementDITByIdCtr(idCtr);
                elts = new List<string>();
                foreach (ELEMENT_FACTURATION elt in elements)
                {
                    elts.Add(elt.LibEF);
                }

                contForm = form;

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

        public DITForm(FactureDITForm form, int idBL, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

                elements = vsp.GetElementFactDITByIdBL(idBL);
                elts = new List<string>();
                foreach (ELEMENT_FACTURATION elt in elements)
                {
                    elts.Add(elt.LibEF);
                }

                factDITForm = form;

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
                VSOMAccessors vsomAcc = new VSOMAccessors();

                if (cbElt.SelectedIndex == -1)
                {
                    MessageBox.Show("Vous devez saisir sélectionner un élément de facturation", "Elément de facturation ?", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (txtMontantDIT.Text.Trim() == "")
                {
                    MessageBox.Show("Vous devez saisir le montant tel que vous l'auriez fait dans le système du DIT", "Montant DIT ?", MessageBoxButton.OK, MessageBoxImage.Information);
                    txtMontantDIT.Focus();
                }
                else
                {
                    ELEMENT_FACTURATION elt = vsomAcc.UpdateEltFactDIT(elements.ElementAt<ELEMENT_FACTURATION>(cbElt.SelectedIndex).IdJEF, Convert.ToDouble(txtMontantDIT.Text), utilisateur.IdU);

                    if (contForm != null)
                    {
                        elements = vsp.GetElementDITByIdCtr(elt.IdCtr.Value);
                    }
                    else if(factDITForm != null)
                    {
                        elements = vsp.GetElementFactDITByIdBL(elt.IdBL.Value);
                    }
                    elts = new List<string>();
                    foreach (ELEMENT_FACTURATION el in elements)
                    {
                        elts.Add(elt.LibEF);
                    }
                    cbElt.ItemsSource = null;
                    cbElt.ItemsSource = elts;

                    if (contForm != null)
                    {
                        formLoader.LoadConteneurForm(contForm, vsp.GetConteneurImportByIdCtr(elt.IdCtr.Value));
                    }
                    else
                    {
                        formLoader.LoadConnaissementForm(factDITForm, elt.CONNAISSEMENT);
                    }

                    MessageBox.Show("Le montant DIT a été mis à jour sur cette ligne de facturation", "Ligne de facturation mise à jour !", MessageBoxButton.OK, MessageBoxImage.Information);
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

        private void cbElt_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (cbElt.SelectedIndex != -1)
                {
                    int indexElt = cbElt.SelectedIndex;
                    ELEMENT_FACTURATION elt = elements.ElementAt<ELEMENT_FACTURATION>(indexElt);
                    txtMontantSOC.Text = (elt.PUEF * elt.QTEEF).ToString();
                    txtMontantDIT.Text = elt.PTDIT.HasValue ? elt.PTDIT.ToString() : "";
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

        private void txtMontantDIT_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
