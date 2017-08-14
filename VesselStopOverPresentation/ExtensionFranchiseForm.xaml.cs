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
    /// Logique d'interaction pour ExtensionFranchiseForm.xaml
    /// </summary>
    public partial class ExtensionFranchiseForm : Window
    {
        public List<CONNAISSEMENT> connaissements { get; set; }
        public List<string> cons { get; set; }

        public List<CLIENT> clients { get; set; }
        public List<string> clts { get; set; }

        public List<ESCALE> escales { get; set; }
        public List<Int32> escs { get; set; }

        public List<EXTENSION_FRANCHISE> demandesExtention { get; set; }
        public List<Int32> exts { get; set; }

        public List<ElementFacturation> eltsFact { get; set; }

        public List<NAVIRE> navires { get; set; }
        public List<string> navs { get; set; }

        public List<VEHICULE> vehicules { get; set; }
        public List<CONTENEUR> conteneurs { get; set; }
        public List<CONVENTIONNEL> conventionnels { get; set; }

        private UTILISATEUR utilisateur;
        private List<OPERATION> operationsUser;

        public ExtensionFranchisePanel extensionPanel { get; set; }

        private FormLoader formLoader;
       // private VsomParameters vsp = new VsomParameters();
        private VSOMAccessors vsomAcc;
        public ExtensionFranchiseForm(ExtensionFranchisePanel panel, EXTENSION_FRANCHISE ext, UTILISATEUR user)
        {
            try
            {
                 vsomAcc = new VSOMAccessors();
                //VsomParameters vsp = new VsomParameters();
                InitializeComponent();
                this.DataContext = this;

                //demandesExtention = vsomAcc.GetExtensionsFranchise();
                //exts = new List<Int32>();
                //foreach (EXTENSION_FRANCHISE ext in demandesExtention)
                //{
                //    exts.Add(ext.IdDEXT);
                //}

                clients = vsomAcc.GetClientsActifs();
                clts = new List<string>();
                foreach (CLIENT clt in clients)
                {
                    clts.Add(clt.NomClient);
                }

                navires = vsomAcc.GetNaviresActifs();
                navs = new List<string>();
                foreach (NAVIRE nav in navires)
                {
                    navs.Add(nav.NomNav);
                }

                extensionPanel = panel;

                utilisateur = user;
                operationsUser = vsomAcc.GetOperationsUtilisateur(utilisateur.IdU);

                formLoader = new FormLoader(utilisateur);

                formLoader.LoadExtensionFranchiseForm(this, ext);
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
            
        }

        public ExtensionFranchiseForm(string type, ExtensionFranchisePanel panel, UTILISATEUR user)
        {
            try
            {
                 vsomAcc = new VSOMAccessors();
                //VsomParameters vsp = new VsomParameters();
                InitializeComponent();
                this.DataContext = this;

                clients = vsomAcc.GetClientsActifs();
                clts = new List<string>();
                foreach (CLIENT clt in clients)
                {
                    clts.Add(clt.NomClient);
                }

                navires = vsomAcc.GetNaviresActifs();
                navs = new List<string>();
                foreach (NAVIRE nav in navires)
                {
                    navs.Add(nav.NomNav);
                }

                extensionPanel = panel;

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

        private void cbClient_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (cbClient.SelectedIndex != -1)
                {
                    txtCodeClient.Text = clients.ElementAt<CLIENT>(cbClient.SelectedIndex).CodeClient;
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

        private void cbNumEsc_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (cbNumEsc.SelectedIndex != -1)
                {
                    txtNumVoy.Text = escales.ElementAt<ESCALE>(cbNumEsc.SelectedIndex).NumVoySCR;
                    cbNavire.SelectedItem = escales.ElementAt<ESCALE>(cbNumEsc.SelectedIndex).NAVIRE.NomNav;
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

        private void btnEnregistrer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                 vsomAcc = new VSOMAccessors();
                //VsomMarchal vsomAcc = new VsomMarchal();
                if (cbIdExt.IsEnabled == false && txtIdBL.Text != "")
                {
                    CONNAISSEMENT bl = vsomAcc.GetConnaissementByIdBL(Convert.ToInt32(txtIdBL.Text));

                    if (operationsUser.Where(op => op.NomOp == "Demande d'extention de franchise : Enregistrement d'un nouvel élément").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
                    {
                        MessageBox.Show("Vous n'avez pas les droits nécessaires pour créer une nouvelle demande d'extension de franchise. Veuillez contacter un administrateur", "Droits insuffisants !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                    else if (txtNbSej.Text.Trim() == "")
                    {
                        MessageBox.Show("Nombre de jours d'extension de séjour ?", "Nombre de jours de séjour ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                    else if (txtNbSures.Text.Trim() == "" )
                    {
                        MessageBox.Show("Nombre de jours d'extension de surestaries ?", "Nombre de jours surestaries ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                    else if (txtNbDet.Text.Trim() == "")
                    {
                        MessageBox.Show("Nombre de jours d'extension de détention ?", "Nombre de jours détention ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                    else
                    {
                        EXTENSION_FRANCHISE ddeExtension = vsomAcc.InsertExtensionFranchise(Convert.ToInt32(txtIdBL.Text), Convert.ToInt32(txtNbSej.Text), Convert.ToInt32(txtNbSures.Text), Convert.ToInt32(txtNbDet.Text), new TextRange(txtObservations.Document.ContentStart, txtObservations.Document.ContentEnd).Text, utilisateur.IdU);

                        if (extensionPanel.cbFiltres.SelectedIndex != 2)
                        {
                            extensionPanel.cbFiltres.SelectedIndex = 2;
                        }
                        else
                        {
                            extensionPanel.extensions = vsomAcc.GetExtensionsFranchiseEnAttente();
                            extensionPanel.dataGrid.ItemsSource = extensionPanel.extensions;
                            extensionPanel.lblStatut.Content = extensionPanel.extensions.Count + " Demande(s) d'extension de franchise";
                        }

                        formLoader.LoadExtensionFranchiseForm(this, ddeExtension);

                        borderActions.Visibility = System.Windows.Visibility.Visible;
                        btnValider.Visibility = System.Windows.Visibility.Visible;

                        MessageBox.Show("Enregistrement effectué avec succès.", "Enregistrement effectué !", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
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

        private void btnValider_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (operationsUser.Where(op => op.NomOp == "Bon à enlever : Validation d'un élément").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
                {
                    MessageBox.Show("Vous n'avez pas les droits nécessaires pour valider un BAE. Veuillez contacter un administrateur", "Droits insuffisants !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    ValiderExtensionForm validForm = new ValiderExtensionForm(this, utilisateur);
                    validForm.Title = "Validation extension : " + cbIdExt.Text + " - Consignée " + txtConsignee.Text;
                    validForm.ShowDialog();
                }
            }
            catch (EnregistrementInexistant ex)
            {
                MessageBox.Show(ex.Message, "Enregistrement inexistant !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        private void cbNumBL_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
               vsomAcc = new VSOMAccessors();

                if (e.Key == Key.Return && cbNumBL.Text.Trim() != "")
                {
                    connaissements = vsomAcc.GetConnaissementPourExtensionByNumBL(cbNumBL.Text);

                    if (connaissements.Count == 0)
                    {
                        MessageBox.Show("Il n'existe aucun connaissement portant ce numéro", "Connaissement introuvable", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                    else if (connaissements.Count == 1)
                    {
                        CONNAISSEMENT con = connaissements.FirstOrDefault<CONNAISSEMENT>();
                        formLoader.LoadConnaissementForm(this, con);
                    }
                    else
                    {
                        ListConnaissementForm listConForm = new ListConnaissementForm(this, connaissements, utilisateur);
                        listConForm.Title = "Choix multiples : Sélectionnez un connaissement";
                        listConForm.ShowDialog();
                    }
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

        private void txtNb_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
