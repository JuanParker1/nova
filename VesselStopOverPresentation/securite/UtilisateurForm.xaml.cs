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
    /// Logique d'interaction pour IdentificationConteneurForm.xaml
    /// </summary>
    public partial class UtilisateurForm : Window
    {

        public List<OPERATION> operations { get; set; }

        private UtilisateurPanel userPanel;
        private List<OPERATION> operationsUser;
        private UTILISATEUR utilisateur;

        public UTILISATEUR userOfForm { get; set; }
        public List<OPERATION> operationOfUser { get; set; }

        private List<ACCONIER> acconiers;
        public List<string> accs { get; set; }

        private List<PARC> parcs;
        public List<string> prcs { get; set; }

        private FormLoader formLoader;
        //private VsomParameters vsomAcc = new VsomParameters();
        private VSOMAccessors vsomAcc;
        public UtilisateurForm(UtilisateurPanel panel, UTILISATEUR user)
        {
            try
            {
                 vsomAcc = new VSOMAccessors();
                //VsomParameters vsomAccrm = new VsomParameters();
                InitializeComponent();
                this.DataContext = this;

                acconiers = vsomAcc.GetAcconiersActifs();
                accs = new List<string>();
                foreach (ACCONIER acc in acconiers)
                {
                    accs.Add(acc.NomAcc);
                }

                parcs = vsomAcc.GetParcs("C");
                prcs = new List<string>();
                foreach (PARC p in parcs)
                {
                    prcs.Add(p.NomParc);
                }

                userPanel = panel;

                userOfForm = new UTILISATEUR();

                operations = vsomAcc.GetOperations();

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
                VsomConfig vsom = new VsomConfig();

                if (operationsUser.Where(op => op.NomOp == "Opération sur les comptes utilisateurs").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
                {
                    MessageBox.Show("Vous n'avez pas les droits nécessaires pour mettre à jour les données de base utilisateur. Veuillez contacter un administrateur", "Droits insuffisants !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (txtLogin.Text.Trim() == "")
                {
                    MessageBox.Show("Veuillez saisir le login de l'utilisateur", "Login ?", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (txtNomUser.Text.Trim() == "")
                {
                    MessageBox.Show("Veuillez saisir le nom complet de l'utilisateur", "Nom complet ?", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (txtPassword.Password != txtPasswordConfirmation.Password || txtPassword.Password == "")
                {
                    MessageBox.Show("Veuillez saisir le mot de passe de l'utilisteur. Attention les mots de passe doivent être identiques", "Mot de passe ?", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (cbAcconier.SelectedIndex == -1)
                {
                    MessageBox.Show("Veuillez sélectionner l'acconier auquel est rattaché cet utilisateur", "Acconier ?", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                //else if (cbParc.SelectedIndex == -1)
                //{
                //    MessageBox.Show("Veuillez sélectionner le parc à conteneur auquel est rattaché cet utilisateur", "Parc ?", MessageBoxButton.OK, MessageBoxImage.Information);
                //}
                else
                {
                    UTILISATEUR u = vsom.InsertOrUpdateUtilisateur(userOfForm.IdU, txtLogin.Text, txtPassword.Password, txtNomUser.Text, 
                                    cbCaisse.Text, acconiers.ElementAt<ACCONIER>(cbAcconier.SelectedIndex).IdAcc, 
                                    cbParc.SelectedIndex != -1 ? parcs.ElementAt<PARC>(cbParc.SelectedIndex).IdParc : -1, 
                                    chkStatut.IsChecked == true ? "A" : "I", 
                                    new TextRange(txtObservations.Document.ContentStart, txtObservations.Document.ContentEnd).Text, 
                                    operationOfUser == null ? new List<OPERATION>() : operationOfUser, utilisateur.IdU);

                    formLoader.LoadUtilisateurForm(this, u);
                    MessageBox.Show("Enregistrement effectué avec succès.", "Enregistrement effectué !", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnAjout_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dataGridOperations.SelectedItems.Count == 0)
                {
                    MessageBox.Show("Veuillez sélectionner au moins un droit à ajouter à cet utilisateur", "Droits ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    foreach(OPERATION op in dataGridOperations.SelectedItems.OfType<OPERATION>().ToList<OPERATION>())
                    {
                        OPERATION elt = new OPERATION();
                        elt.NomOp = op.NomOp;
                        elt.IdOp = op.IdOp;

                        if (operationOfUser == null)
                        {
                            operationOfUser = new List<OPERATION>();
                        }

                        if (!operationOfUser.Exists(o => o.IdOp == elt.IdOp))
                        {
                            operationOfUser.Add(elt);
                        }
                    }

                    dataGridOperationsUser.ItemsSource = null;
                    dataGridOperationsUser.ItemsSource = operationOfUser;
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

        private void btnSupprimer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dataGridOperationsUser.SelectedItems.Count == 0)
                {
                    MessageBox.Show("Veuillez sélectionner au moins un droit à retirer à cet utilisateur", "Droits ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {

                    foreach (OPERATION op in dataGridOperationsUser.SelectedItems.OfType<OPERATION>().ToList<OPERATION>())
                    {
                        operationOfUser.Remove(op);
                    }

                    dataGridOperationsUser.ItemsSource = null;
                    dataGridOperationsUser.ItemsSource = operationOfUser;
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

        private void btnAjoutAll_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (OPERATION op in dataGridOperations.Items.OfType<OPERATION>().ToList<OPERATION>())
                {
                    OPERATION elt = new OPERATION();
                    elt.NomOp = op.NomOp;
                    elt.IdOp = op.IdOp;

                    if (operationOfUser == null)
                    {
                        operationOfUser = new List<OPERATION>();
                    }

                    if (!operationOfUser.Exists(o => o.IdOp == elt.IdOp))
                    {
                        operationOfUser.Add(elt);
                    }
                }

                dataGridOperationsUser.ItemsSource = null;
                dataGridOperationsUser.ItemsSource = operationOfUser;
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
