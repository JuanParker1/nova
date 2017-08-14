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
using System.Windows.Navigation;
using System.Windows.Shapes;
using VesselStopOverData;

namespace VesselStopOverPresentation
{
    /// <summary>
    /// Logique d'interaction pour ConteneurTCPanel.xaml
    /// </summary>
    public partial class ConteneurTCPanel : DockPanel
    {
        private List<OPERATION> operationsUser;
        private UTILISATEUR utilisateur;
        private VsomParameters vsp = new VsomParameters();
        public ConteneurTCPanel(UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;
                listRechercher.SelectedIndex = 0;
                //cbFiltres.SelectedIndex = 1;
                //lblStatut.Content = conteneurs.Count + " Conteneur(s)";

                actionsBorder.Visibility = System.Windows.Visibility.Collapsed;

                utilisateur = user;
                operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
            
        }

        public List<CONTENEUR_TC> conteneurs { get; set; }

        private void dataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();
                CONTENEUR_TC ctr = vsp.GetConteneurTCByIdTC(((CONTENEUR_TC)dataGrid.SelectedItem).IdTC);
                if (dataGrid.SelectedIndex != -1)
                {
                    ConteneurTCForm contForm = new ConteneurTCForm(this, ctr, utilisateur);
                    contForm.Show();
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

        private void cbFiltres_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                if (cbFiltres.SelectedIndex == 0)
                {
                    conteneurs = vsp.GetConteneursTC();
                }
                else if (cbFiltres.SelectedIndex == 1)
                {
                    conteneurs = vsp.GetConteneurTCsByStatut("Manifesté");
                }
                else if (cbFiltres.SelectedIndex == 2)
                {
                    conteneurs = vsp.GetConteneurTCsByStatut("Débarqué");
                }
                else if (cbFiltres.SelectedIndex == 3)
                {
                    conteneurs = vsp.GetConteneurTCsByStatut("Sorti");
                }
                else if (cbFiltres.SelectedIndex == 4)
                {
                    conteneurs = vsp.GetConteneurTCsByStatut("Retourné");
                }
                else if (cbFiltres.SelectedIndex == 5)
                {
                    conteneurs = vsp.GetConteneurTCsByStatut("Parqué");
                }
                else if (cbFiltres.SelectedIndex == 6)
                {
                    conteneurs = vsp.GetConteneurTCsByStatut("En réparation");
                }
                else if (cbFiltres.SelectedIndex == 7)
                {
                    conteneurs = vsp.GetConteneurTCsByStatut("En nettoyage");
                }
                else if (cbFiltres.SelectedIndex == 8)
                {
                    conteneurs = vsp.GetConteneurTCsByStatut("Mis à disposition");
                }
                else if (cbFiltres.SelectedIndex == 9)
                {
                    conteneurs = vsp.GetConteneurTCsByStatut("Cargo Loading");
                }
                else if (cbFiltres.SelectedIndex == 10)
                {
                    conteneurs = vsp.GetConteneurTCsByStatut("Cargo Loaded");
                }
                else if (cbFiltres.SelectedIndex == 11)
                {
                    conteneurs = vsp.GetConteneurTCsByStatut("Retourné à l'armateur");
                }
                dataGrid.ItemsSource = conteneurs;
                lblStatut.Content = conteneurs.Count + " Conteneur(s)";
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        private void txtRechercher_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                if (e.Key == Key.Return && listRechercher.SelectedItem != null)
                {
                    if (listRechercher.SelectedIndex == 0)
                    {
                        conteneurs = vsp.GetConteneurTCsByNumCtr(txtRechercher.Text.Trim());
                    }
                    else if (listRechercher.SelectedIndex == 1)
                    {
                        conteneurs = vsp.GetConteneurTCsByNumBL(txtRechercher.Text.Trim());
                    }
                    else if (listRechercher.SelectedIndex == 2)
                    {
                        int result;
                        conteneurs = vsp.GetConteneurTCsByNumEscale(Int32.TryParse(txtRechercher.Text.Trim(), out result) ? result : -1);
                    }
                    else if (listRechercher.SelectedIndex == 3)
                    {
                        conteneurs = vsp.GetConteneurTCsByDescription(txtRechercher.Text.Trim());
                    }
                    else if (listRechercher.SelectedIndex == 4)
                    {
                        conteneurs = vsp.GetConteneurTCsByParc(txtRechercher.Text.Trim());
                    }

                    dataGrid.ItemsSource = conteneurs;
                    lblStatut.Content = conteneurs.Count + " ConteneurTC(s) trouvé(s)";
                }
                else if (e.Key == Key.Escape)
                {
                    txtRechercher.Text = null;
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

        private void btnNouveau_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (operationsUser.Where(op => op.NomOp == "Conteneur : Enregistrement d'un nouvel élément").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
                {
                    MessageBox.Show("Vous n'avez pas les droits nécessaires pour créer un nouveau conteneur. Veuillez contacter un administrateur", "Droits insuffisants !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    ConteneurTCForm contForm = new ConteneurTCForm("Nouveau", this, utilisateur);
                    contForm.cbNumCtr.IsEditable = true;
                    //contForm.actionsBorder.Visibility = System.Windows.Visibility.Collapsed;
                    contForm.txtDescription.IsReadOnly = false;
                    contForm.txtDescription.Background = Brushes.White;
                    contForm.txtDescMses.IsReadOnly = false;
                    contForm.txtDescMses.Background = Brushes.White;
                    contForm.txtIMDGCode.IsReadOnly = false;
                    contForm.txtIMDGCode.Background = Brushes.White;
                    contForm.txtPoids.IsReadOnly = false;
                    contForm.txtPoids.Background = Brushes.White;
                    contForm.cbEtatM.IsEnabled = true;
                    contForm.cbTypeCtrM.IsEnabled = true;
                    contForm.cbNumBL.IsEditable = true;
                    contForm.cbNumBL.IsEnabled = true;
                    contForm.groupInfosCub.IsEnabled = false;
                    contForm.Title = "Nouveau : Conteneur";
                    contForm.Show();
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

        private void btnDebarquer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                if (dataGrid.SelectedIndex != -1 && dataGrid.SelectedItems.Count == 1)
                {
                    CONTENEUR_TC ctr = vsp.GetConteneurTCByIdCtr(((CONTENEUR_TC)dataGrid.SelectedItem).IdCtr.Value);
                    IdentificationConteneurForm identForm = new IdentificationConteneurForm(this, ctr, utilisateur);
                    identForm.txtSeal1.Text = ctr.CONTENEUR.Seal1Ctr;
                    identForm.txtSeal2.Text = ctr.CONTENEUR.Seal2Ctr;
                    identForm.Title = "Débarquement - Conteneur N° : " + ctr.NumTC;
                    identForm.ShowDialog();
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

        private void btnSortir_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                if (dataGrid.SelectedIndex != -1 && dataGrid.SelectedItems.Count == 1)
                {
                    CONTENEUR_TC ctr = vsp.GetConteneurTCByIdCtr(((CONTENEUR_TC)dataGrid.SelectedItem).IdCtr.Value);
                    SortirConteneurForm sortieForm = new SortirConteneurForm(this, ctr, utilisateur);
                    sortieForm.Title = "Sortie - Conteneur N° : " + ctr.NumTC;
                    sortieForm.ShowDialog();
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

        private void btnRetour_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                if (dataGrid.SelectedIndex != -1 && dataGrid.SelectedItems.Count == 1)
                {
                    CONTENEUR_TC ctr = vsp.GetConteneurTCByIdCtr(((CONTENEUR_TC)dataGrid.SelectedItem).IdCtr.Value);
                    RetourConteneurForm retourForm = new RetourConteneurForm(this, ctr, utilisateur);
                    retourForm.Title = "Retour - Conteneur N° : " + ctr.NumTC;
                    retourForm.ShowDialog();
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

        private void btnParquer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                if (dataGrid.SelectedIndex != -1 && dataGrid.SelectedItems.Count == 1)
                {
                    CONTENEUR_TC ctr = vsp.GetConteneurTCByIdCtr(((CONTENEUR_TC)dataGrid.SelectedItem).IdCtr.Value);
                    ParquerConteneurForm parquerForm = new ParquerConteneurForm(this, ctr, utilisateur);
                    if (ctr.IdParcRetourVide.HasValue)
                    {
                        parquerForm.cbParc.SelectedItem = parquerForm.parcs.FirstOrDefault<PARC>(p => p.IdParc == ctr.IdParcRetourVide).NomParc;
                    }
                    parquerForm.Title = "Parquing - Conteneur N° : " + ctr.NumTC;
                    parquerForm.ShowDialog();
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

        private void btnTransfRep_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                if (dataGrid.SelectedIndex != -1 && dataGrid.SelectedItems.Count == 1)
                {
                    CONTENEUR_TC ctr = vsp.GetConteneurTCByIdCtr(((CONTENEUR_TC)dataGrid.SelectedItem).IdCtr.Value);
                    TransfReparationConteneurForm reparationForm = new TransfReparationConteneurForm(this, ctr, utilisateur);
                    if (ctr.IdParcRetourVide.HasValue)
                    {
                        reparationForm.cbParc.SelectedItem = reparationForm.parcs.FirstOrDefault<PARC>(p => p.IdParc == ctr.IdParcRetourVide).NomParc;
                    }
                    reparationForm.Title = "Réparation - Conteneur N° : " + ctr.NumTC;
                    reparationForm.ShowDialog();
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

        private void btnTransfHabillage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                if (dataGrid.SelectedIndex != -1 && dataGrid.SelectedItems.Count == 1)
                {
                    CONTENEUR_TC ctr = vsp.GetConteneurTCByIdCtr(((CONTENEUR_TC)dataGrid.SelectedItem).IdCtr.Value);
                    TransfHabillageConteneurForm habillageForm = new TransfHabillageConteneurForm(this, ctr, utilisateur);
                    if (ctr.IdParcRetourVide.HasValue)
                    {
                        habillageForm.cbParc.SelectedItem = habillageForm.parcs.FirstOrDefault<PARC>(p => p.IdParc == ctr.IdParcRetourVide).NomParc;
                    }
                    habillageForm.Title = "Habillage - Conteneur N° : " + ctr.NumTC;
                    habillageForm.ShowDialog();
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

        private void btnMiseDispoVide_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                if (dataGrid.SelectedIndex != -1 && dataGrid.SelectedItems.Count == 1)
                {
                    CONTENEUR_TC ctr = vsp.GetConteneurTCByIdCtr(((CONTENEUR_TC)dataGrid.SelectedItem).IdCtr.Value);

                    List<DISPOSITION_CONTENEUR> dispositionCtr = vsp.GetDemandesDispositionEnCoursByTypeCtr(ctr.TypeCtr);

                    ListDispositionCtrForm listDispoForm = new ListDispositionCtrForm(this, ctr, dispositionCtr, utilisateur);
                    listDispoForm.Title = "Choix multiples : Sélectionnez une demande de mise à disposition";
                    listDispoForm.ShowDialog();
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

        private void btnRetourPlein_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                if (dataGrid.SelectedIndex != -1 && dataGrid.SelectedItems.Count == 1)
                {
                    CONTENEUR ctr = vsp.GetConteneurExportByIdCtr(((CONTENEUR_TC)dataGrid.SelectedItem).IdCtrExport.Value);
                    ReceptionExportConteneurForm receptionForm = new ReceptionExportConteneurForm(this, ctr, utilisateur);
                    receptionForm.txtSeal1.Text = ctr.Seal1Ctr;
                    receptionForm.txtSeal2.Text = ctr.Seal2Ctr;
                    receptionForm.Title = "Retour plein export - Conteneur N° : " + ctr.NumCtr;
                    receptionForm.ShowDialog();
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

        private void btnEmbarquer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                if (dataGrid.SelectedIndex != -1 && dataGrid.SelectedItems.Count == 1)
                {
                    CONTENEUR ctr = vsp.GetConteneurExportByIdCtr(((CONTENEUR_TC)dataGrid.SelectedItem).IdCtrExport.Value);
                    EmbarquerConteneurForm embarqForm = new EmbarquerConteneurForm(this, ctr, utilisateur);
                    embarqForm.txtSeal1.Text = ctr.Seal1Ctr;
                    embarqForm.txtSeal2.Text = ctr.Seal2Ctr;
                    embarqForm.Title = "Embarquement - Conteneur N° : " + ctr.NumCtr;
                    embarqForm.ShowDialog();
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

        private void btnRetourArmateur_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                if (dataGrid.SelectedIndex != -1 && dataGrid.SelectedItems.Count == 1)
                {
                    CONTENEUR_TC ctrTC = vsp.GetConteneurTCByIdCtr(((CONTENEUR_TC)dataGrid.SelectedItem).IdCtr.Value);
                    RetourArmateurConteneurForm retourArmateurForm = new RetourArmateurConteneurForm(this, ctrTC, utilisateur);
                    retourArmateurForm.txtParc.Text = ctrTC.PARC.NomParc;
                    retourArmateurForm.txtEmplacement.Text = vsp.GetEmplacementByIdEmpl(ctrTC.IdEmplacementParc.Value).LigEmpl + vsp.GetEmplacementByIdEmpl(ctrTC.IdEmplacementParc.Value).ColEmpl;
                    retourArmateurForm.Title = "Retour à l'armateur - Conteneur N° : " + ctrTC.NumTC;
                    retourArmateurForm.ShowDialog();
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

        private void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                if (dataGrid.SelectedIndex != -1 && dataGrid.SelectedItems.Count == 1)
                {
                    CONTENEUR_TC ctr = vsp.GetConteneurTCByIdCtr(((CONTENEUR_TC)dataGrid.SelectedItem).IdTC);
                    actionsBorder.Visibility = System.Windows.Visibility.Visible;

                    if (ctr.IdCtrExport.HasValue)
                    {
                        btnRetourPlein.IsEnabled = true;
                        btnEmbarquer.IsEnabled = true;
                    }
                    else
                    {
                        btnRetourPlein.IsEnabled = false;
                        btnEmbarquer.IsEnabled = false;
                    }

                    if (ctr.IdParcParquing.HasValue)
                    {
                        btnRetourArmateur.IsEnabled = true;
                    }
                    else
                    {
                        btnRetourArmateur.IsEnabled = false;
                    }

                    if (ctr.StatutTC == "Cargo Loaded")
                    {
                        btnRestitution.IsEnabled = true;
                    }
                    else
                    {
                        btnRestitution.IsEnabled = false;
                    }
                }
                else
                {
                    actionsBorder.Visibility = System.Windows.Visibility.Collapsed;
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

        private void btnTransfEmpl_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                if (dataGrid.SelectedIndex != -1 && dataGrid.SelectedItems.Count == 1)
                {
                    CONTENEUR_TC ctrTC = vsp.GetConteneurTCByIdCtr(((CONTENEUR_TC)dataGrid.SelectedItem).IdCtr.Value);
                    TransfertEmplacementConteneurForm transfertForm = new TransfertEmplacementConteneurForm(this, ctrTC, utilisateur);
                    transfertForm.Title = "Transfert - Conteneur N° : " + ctrTC.NumTC;
                    transfertForm.ShowDialog();
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

        private void btnConstatContradictoire_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                if (dataGrid.SelectedIndex != -1 && dataGrid.SelectedItems.Count == 1)
                {
                    CONTENEUR_TC ctrTC = vsp.GetConteneurTCByIdCtr(((CONTENEUR_TC)dataGrid.SelectedItem).IdCtr.Value);
                    ConstatContradictoireForm constatContradictoireForm = new ConstatContradictoireForm(this, ctrTC, utilisateur);
                    constatContradictoireForm.Title = "Constat contradictoire - Conteneur N° : " + ctrTC.NumTC;
                    constatContradictoireForm.ShowDialog();
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

        private void btnRestitution_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                if (dataGrid.SelectedIndex != -1 && dataGrid.SelectedItems.Count == 1)
                {
                    CONTENEUR_TC ctrTC = vsp.GetConteneurTCByIdCtr(((CONTENEUR_TC)dataGrid.SelectedItem).IdCtr.Value);
                    RestitutionConteneurForm restitutionConteneurForm = new RestitutionConteneurForm(this, ctrTC, utilisateur);
                    restitutionConteneurForm.Title = "Restitution - Conteneur N° : " + ctrTC.NumTC;
                    restitutionConteneurForm.ShowDialog();
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
