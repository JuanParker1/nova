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
using Excel = Microsoft.Office.Interop.Excel;
using System.IO;
using System.IO.Compression;

namespace VesselStopOverPresentation
{
    /// <summary>
    /// Logique d'interaction pour ConteneurTCForm.xaml
    /// </summary>
    public partial class ConteneurTCForm : Window
    {
        public List<CONTENEUR_TC> conteneurs { get; set; }
        public List<string> ctrs { get; set; }

        public List<CONNAISSEMENT> connaissements { get; set; }
        public List<string> cons { get; set; }

        private List<TYPE_CONTENEUR> typesConteneurTCs;
        public List<string> typesCtrs { get; set; }

        public List<ElementFacturation> eltsFact { get; set; }

        public List<MOUVEMENT_TC> eltsMvt { get; set; }

        private ConteneurTCPanel contPanel;

        public List<OPERATION> operationsUser { get; set; }
        public UTILISATEUR utilisateur { get; set; }

        private string typeForm;

        private FormLoader formLoader;

        public CONTENEUR_TC _conteneur_tc { get; set; }
       // private VsomParameters vsp = new VsomParameters();
        private VSOMAccessors vsomAcc;
        public ConteneurTCForm(ConteneurTCPanel panel, CONTENEUR_TC ctr, UTILISATEUR user)
        {
            try
            {
                 vsomAcc = new VSOMAccessors();
                //VsomParameters vsp = new VsomParameters();
                InitializeComponent();
                this.DataContext = this;

                typesConteneurTCs = vsomAcc.GetTypesConteneurs();
                typesCtrs = new List<string>();
                foreach (TYPE_CONTENEUR t in typesConteneurTCs)
                {
                    typesCtrs.Add(t.CodeTypeCtr);
                }

                contPanel = panel;
                utilisateur = user;
                operationsUser = vsomAcc.GetOperationsUtilisateur(utilisateur.IdU);

                formLoader = new FormLoader(utilisateur);

                formLoader.LoadConteneurTCForm(this, ctr);
                _conteneur_tc = ctr;
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

        public ConteneurTCForm(string type, ConteneurTCPanel panel, UTILISATEUR user)
        {
            try
            {
                 vsomAcc = new VSOMAccessors();
               // VsomParameters vsp = new VsomParameters();
                InitializeComponent();
                this.DataContext = this;

                typesConteneurTCs = vsomAcc.GetTypesConteneurs();
                typesCtrs = new List<string>();
                foreach (TYPE_CONTENEUR t in typesConteneurTCs)
                {
                    typesCtrs.Add(t.CodeTypeCtr);
                }

                contPanel = panel;
                utilisateur = user;
                operationsUser = vsomAcc.GetOperationsUtilisateur(utilisateur.IdU);

                formLoader = new FormLoader(utilisateur);

                typeForm = type;
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
            
        }

        private void cbNumBL_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (cbNumBL.SelectedIndex != -1)
                {
                    int indexBL = cbNumBL.SelectedIndex;
                    CONNAISSEMENT con = connaissements.ElementAt<CONNAISSEMENT>(indexBL);
                    txtIdBL.Text = con.IdBL.ToString();
                    txtConsignee.Text = con.ConsigneeBL;
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

        private void txtPoidsC_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void txtCaution_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void btnEnregistrer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                
            }
            catch (HabilitationException ex)
            {
                MessageBox.Show(ex.Message, "Droits insuffisants !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        private void cbNumCtr_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                //VSOMAccessors vsomAcc = new VSOMAccessors();

                if (e.Key == Key.Return && cbNumCtr.Text.Trim() != "")
                {
                    conteneurs = vsomAcc.GetConteneurTCsByNumCtr(cbNumCtr.Text);

                    if (conteneurs.Count == 0)
                    {
                        MessageBox.Show("Il n'existe aucun conteneur portant ce numéro", "ConteneurTC introuvable", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                    else if (conteneurs.Count == 1)
                    {
                        CONTENEUR_TC ctr = conteneurs.FirstOrDefault<CONTENEUR_TC>();
                        formLoader.LoadConteneurTCForm(this, ctr);
                    }
                    else
                    {
                        ListConteneurTCForm listCtrForm = new ListConteneurTCForm(this, conteneurs, utilisateur);
                        listCtrForm.Title = "Choix multiples : Sélectionnez un conteneur";
                        listCtrForm.ShowDialog();
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

        private void cbNumBL_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                //VSOMAccessors vsomAcc = new VSOMAccessors();

                if (e.Key == Key.Return && cbNumBL.Text.Trim() != "")
                {
                    connaissements = vsomAcc.GetConnaissementByNumBL(cbNumBL.Text);

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

        private void txtPoids_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void btnDebarquer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //VSOMAccessors vsomAcc = new VSOMAccessors();

                IdentificationConteneurForm identForm = new IdentificationConteneurForm(this, utilisateur);
                CONTENEUR ctr = vsomAcc.GetConteneurImportByIdCtr(Convert.ToInt32(txtIdCtr.Text));
                identForm.txtSeal1.Text = ctr.Seal1Ctr;
                identForm.txtSeal2.Text = ctr.Seal2Ctr;
                identForm.Title = "Débarquement - Conteneur N° : " + cbNumCtr.Text;
                identForm.ShowDialog();
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        private void btnSortir_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SortirConteneurForm sortieForm = new SortirConteneurForm(this, utilisateur);
                sortieForm.Title = "Sortie - Conteneur N° : " + cbNumCtr.Text;
                sortieForm.ShowDialog();
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
                RetourConteneurForm retourForm = new RetourConteneurForm(this, utilisateur);
                retourForm.Title = "Retour - Conteneur N° : " + cbNumCtr.Text;
                retourForm.ShowDialog();
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
                //VSOMAccessors vsomAcc = new VSOMAccessors();

                ParquerConteneurForm parquerForm = new ParquerConteneurForm(this, utilisateur);
                if (vsomAcc.GetConteneurTCByIdCtr(Convert.ToInt32(txtIdCtr.Text)).IdParcRetourVide.HasValue)
                {
                    parquerForm.cbParc.SelectedItem = parquerForm.parcs.FirstOrDefault<PARC>(p => p.IdParc == vsomAcc.GetConteneurTCByIdCtr(Convert.ToInt32(txtIdCtr.Text)).IdParcRetourVide).NomParc;
                }
                parquerForm.Title = "Parquing - Conteneur N° : " + cbNumCtr.Text;
                parquerForm.ShowDialog();

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
                //VSOMAccessors vsomAcc = new VSOMAccessors();

                //if (vsomAcc.GetConteneurTCByIdCtr(Convert.ToInt32(txtIdCtr.Text)).IdParcRetourVide.HasValue)
                //{
                //    TransfReparationConteneurForm reparationForm = new TransfReparationConteneurForm(this, utilisateur);
                //    reparationForm.cbParc.SelectedItem = reparationForm.parcs.FirstOrDefault<PARC>(p => p.IdParc == vsomAcc.GetConteneurTCByIdCtr(Convert.ToInt32(txtIdCtr.Text)).IdParcRetourVide).NomParc;
                //    reparationForm.Title = "Réparation - Conteneur N° : " + cbNumCtr.Text;
                //    reparationForm.ShowDialog();
                //}
                //else
                //{
                //    MessageBox.Show("Conteneur non retourné", "Conteneur non retourné !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                //}

                TransfReparationConteneurForm reparationForm = new TransfReparationConteneurForm(this, utilisateur);
                if (vsomAcc.GetConteneurTCByIdCtr(Convert.ToInt32(txtIdCtr.Text)).IdParcRetourVide.HasValue)
                {
                    reparationForm.cbParc.SelectedItem = reparationForm.parcs.FirstOrDefault<PARC>(p => p.IdParc == vsomAcc.GetConteneurTCByIdCtr(Convert.ToInt32(txtIdCtr.Text)).IdParcRetourVide).NomParc;
                }
                reparationForm.Title = "Réparation - Conteneur N° : " + cbNumCtr.Text;
                reparationForm.ShowDialog();
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
               // VSOMAccessors vsomAcc = new VSOMAccessors();

                //if (vsomAcc.GetConteneurTCByIdCtr(Convert.ToInt32(txtIdCtr.Text)).IdParcRetourVide.HasValue)
                //{
                //    TransfHabillageConteneurForm habillageForm = new TransfHabillageConteneurForm(this, utilisateur);
                //    habillageForm.cbParc.SelectedItem = habillageForm.parcs.FirstOrDefault<PARC>(p => p.IdParc == vsomAcc.GetConteneurTCByIdCtr(Convert.ToInt32(txtIdCtr.Text)).IdParcRetourVide).NomParc;
                //    habillageForm.Title = "Habillage - Conteneur N° : " + cbNumCtr.Text;
                //    habillageForm.ShowDialog();
                //}
                //else
                //{
                //    MessageBox.Show("Conteneur non retourné", "Conteneur non retourné !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                //}

                TransfHabillageConteneurForm habillageForm = new TransfHabillageConteneurForm(this, utilisateur);
                if (vsomAcc.GetConteneurTCByIdCtr(Convert.ToInt32(txtIdCtr.Text)).IdParcRetourVide.HasValue)
                {
                    habillageForm.cbParc.SelectedItem = habillageForm.parcs.FirstOrDefault<PARC>(p => p.IdParc == vsomAcc.GetConteneurTCByIdCtr(Convert.ToInt32(txtIdCtr.Text)).IdParcRetourVide).NomParc;
                }
                habillageForm.Title = "Habillage - Conteneur N° : " + cbNumCtr.Text;
                habillageForm.ShowDialog();
                
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

        private void btnTransfEmpl_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //VSOMAccessors vsomAcc = new VSOMAccessors();

                CONTENEUR_TC contTC = vsomAcc.GetConteneurTCByIdCtr(Convert.ToInt32(txtIdCtr.Text));

                if (contTC.IdParcParquing.HasValue)
                {
                    TransfertEmplacementConteneurForm transfEmplForm = new TransfertEmplacementConteneurForm(this, contTC, utilisateur);
                    transfEmplForm.txtParcActuel.Text = transfEmplForm.parcs.FirstOrDefault<PARC>(p => p.IdParc == contTC.IdParcParquing).NomParc;
                    transfEmplForm.Title = "Transfert d'emplacement - Conteneur N° : " + cbNumCtr.Text;
                    transfEmplForm.ShowDialog();
                }
                else
                {
                    MessageBox.Show("Conteneur non parqué", "Conteneur non parqué !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
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
                //VSOMAccessors vsomAcc = new VSOMAccessors();

                List<DISPOSITION_CONTENEUR> dispositionCtr = vsomAcc.GetDemandesDispositionEnCoursByTypeCtr(this.cbTypeCtrC.Text);

                ListDispositionCtrForm listDispoForm = new ListDispositionCtrForm(this, dispositionCtr, utilisateur);
                listDispoForm.Title = "Choix multiples : Sélectionnez une demande de mise à disposition";
                listDispoForm.ShowDialog();
             
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
                //VSOMAccessors vsomAcc = new VSOMAccessors();

                if (vsomAcc.GetConteneurByIdCtr(Convert.ToInt32(txtIdCtr.Text)).CONTENEUR_TC.FirstOrDefault<CONTENEUR_TC>().IdCtrExport.HasValue)
                {
                    CONTENEUR ctr = vsomAcc.GetConteneurExportByIdCtr(vsomAcc.GetConteneurByIdCtr(Convert.ToInt32(txtIdCtr.Text)).CONTENEUR_TC.FirstOrDefault<CONTENEUR_TC>().IdCtrExport.Value);
                    ReceptionExportConteneurForm receptionForm = new ReceptionExportConteneurForm(this, ctr, utilisateur);
                    receptionForm.txtSeal1.Text = ctr.Seal1Ctr;
                    receptionForm.txtSeal2.Text = ctr.Seal2Ctr;
                    receptionForm.Title = "Retour plein export - Conteneur N° : " + ctr.NumCtr;
                    receptionForm.ShowDialog();
                }
                else
                {
                    MessageBox.Show("Conteneur non disponible", "Conteneur non disponible !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
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

        private void btnEmbarquer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                vsomAcc = new VSOMAccessors();

                if (vsomAcc.GetConteneurByIdCtr(Convert.ToInt32(txtIdCtr.Text)).CONTENEUR_TC.FirstOrDefault<CONTENEUR_TC>().IdCtrExport.HasValue)
                {
                    CONTENEUR ctr = vsomAcc.GetConteneurExportByIdCtr(vsomAcc.GetConteneurByIdCtr(Convert.ToInt32(txtIdCtr.Text)).CONTENEUR_TC.FirstOrDefault<CONTENEUR_TC>().IdCtrExport.Value);
                    EmbarquerConteneurForm embarqForm = new EmbarquerConteneurForm(this, ctr, utilisateur);
                    embarqForm.txtSeal1.Text = ctr.Seal1Ctr;
                    embarqForm.txtSeal2.Text = ctr.Seal2Ctr;
                    embarqForm.Title = "Embarquement - Conteneur N° : " + ctr.NumCtr;
                    embarqForm.ShowDialog();
                }
                else
                {
                    MessageBox.Show("Conteneur non disponible", "Conteneur non disponible !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
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

        private void btnHistInterchange_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //VSOMAccessors vsomAcc = new VSOMAccessors();

                if (operationsUser.Where(op => op.NomOp == "Conteneur : Visualisation Interchange").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
                {
                    MessageBox.Show("Vous n'avez pas les droits nécessaires pour visualiser des éléments d'interchange. Veuillez contacter un administrateur", "Droits insuffisants !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    InterchangeConteneurForm interForm = new InterchangeConteneurForm(this, Convert.ToInt32(txtIdCtr.Text));
                    interForm.Title = "Historique d'interchange - Conteneur N° " + cbNumCtr.Text;
                    interForm.ShowDialog();
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
                 vsomAcc = new VSOMAccessors();
                CONTENEUR_TC ctrTC = vsomAcc.GetConteneurTCByIdCtr(Convert.ToInt32(txtIdCtr.Text));

                if (ctrTC.IdParcParquing.HasValue)
                {
                    RetourArmateurConteneurForm retourArmateurForm = new RetourArmateurConteneurForm(this, utilisateur);
                    retourArmateurForm.txtParc.Text = ctrTC.PARC.NomParc;
                    retourArmateurForm.txtEmplacement.Text = vsomAcc.GetEmplacementByIdEmpl(ctrTC.IdEmplacementParc.Value).LigEmpl + vsomAcc.GetEmplacementByIdEmpl(ctrTC.IdEmplacementParc.Value).ColEmpl;
                    retourArmateurForm.Title = "Retour à l'armateur - Conteneur N° : " + cbNumCtr.Text;
                    retourArmateurForm.ShowDialog();
                }
                else
                {
                    MessageBox.Show("Conteneur non parqué", "Conteneur non parqué !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
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

        private void dataGridMvt_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
               // VSOMAccessors vsomAcc = new VSOMAccessors();

                if (operationsUser.Where(op => op.NomOp == "Conteneur : Modification mouvements").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
                {
                    MessageBox.Show("Vous n'avez pas les droits nécessaires pour modifier les informations sur les operations. Veuillez contacter un administrateur", "Droits insuffisants !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    //cas exeception fait pour les operation de regularisation effectue par mdm pinot
                    if (utilisateur.LU == "CPINOT")
                    {
                        conteneur._2015UpdateOperationTracking interForm = new conteneur._2015UpdateOperationTracking(this, Convert.ToInt32(txtIdCtr.Text), utilisateur);
                        interForm.Title = "Modification des operations - Conteneur N° " + cbNumCtr.Text;
                        interForm.cbOperation.SelectedItem = ((MOUVEMENT_TC)dataGridMvt.SelectedItem).TYPE_OPERATION.LibTypeOp;
                        interForm.ShowDialog();
                    }
                    else
                    {
                        UpdateOperationTrackingForm interForm = new UpdateOperationTrackingForm(this, Convert.ToInt32(txtIdCtr.Text), utilisateur);
                        interForm.Title = "Modification des operations - Conteneur N° " + cbNumCtr.Text;
                        interForm.cbOperation.SelectedItem = ((MOUVEMENT_TC)dataGridMvt.SelectedItem).TYPE_OPERATION.LibTypeOp;
                        interForm.ShowDialog();
                    }
                    
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

        private void btnConstatContradictoire_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                vsomAcc = new VSOMAccessors();
                CONTENEUR_TC ctrTC = vsomAcc.GetConteneurTCByIdCtr(Convert.ToInt32(txtIdCtr.Text));

                ConstatContradictoireForm retourArmateurForm = new ConstatContradictoireForm(this, utilisateur);
                retourArmateurForm.Title = "Constat contradictoire - Conteneur N° : " + cbNumCtr.Text;
                retourArmateurForm.ShowDialog();

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

        private void btnRestitution_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                 vsomAcc = new VSOMAccessors();
                CONTENEUR_TC ctrTC = vsomAcc.GetConteneurTCByIdCtr(Convert.ToInt32(txtIdCtr.Text));

                RestitutionConteneurForm restitutionCtrForm = new RestitutionConteneurForm(this, utilisateur);
                restitutionCtrForm.Title = "Restitution - Conteneur N° : " + cbNumCtr.Text;
                restitutionCtrForm.ShowDialog();

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

        private void btnHistMvt_Click(object sender, RoutedEventArgs e)
        {
            Excel.Application xlApp = null;
            Excel.Workbook xlWorkBook = null;
            Excel.Worksheet xlWorkSheet = null;
            Excel.Range range;

            try
            {
                 vsomAcc = new VSOMAccessors();

                CONTENEUR_TC ctrTC = vsomAcc.GetConteneurTCByIdCtr(Convert.ToInt32(txtIdCtr.Text));

                xlApp = new Excel.Application();
                xlWorkBook = xlApp.Workbooks.Open(Environment.CurrentDirectory + "//Ressources//HistoriqueConteneur.xlsx", 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
                xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

                range = xlWorkSheet.UsedRange;

                int i = 2;
                List<MOUVEMENT_TC> mvtsTC = ctrTC.MOUVEMENT_TC.OrderBy(op => op.DateMvt.Value).ToList<MOUVEMENT_TC>();

                xlWorkSheet.Columns[1].NumberFormat = "[$-00040C]jj/mm/aaaa;@";
                foreach (MOUVEMENT_TC mvt in mvtsTC)
                {
                    (range.Cells[i, 1] as Excel.Range).Value2 = mvt.DateMvt.Value;
                    (range.Cells[i, 2] as Excel.Range).Value2 = mvt.DateMvt.Value.Hour + ":" + mvt.DateMvt.Value.Minute;
                    (range.Cells[i, 3] as Excel.Range).Value2 = mvt.UTILISATEUR.NU;
                    (range.Cells[i, 4] as Excel.Range).Value2 = "1";
                    (range.Cells[i, 5] as Excel.Range).Value2 = mvt.CONTENEUR_TC.NumTC;
                    if (mvt.CONTENEUR_TC.TypeCtr == "20DV")
                    {
                        (range.Cells[i, 6] as Excel.Range).Value2 = "22G1";
                    }
                    else if (mvt.CONTENEUR_TC.TypeCtr == "40DV")
                    {
                        (range.Cells[i, 6] as Excel.Range).Value2 = "42G1";
                    }
                    else if (mvt.CONTENEUR_TC.TypeCtr == "40HC")
                    {
                        (range.Cells[i, 6] as Excel.Range).Value2 = "45G1";
                    }
                    else if (mvt.CONTENEUR_TC.TypeCtr == "40OT")
                    {
                        (range.Cells[i, 6] as Excel.Range).Value2 = "42U1";
                    }
                    else if (mvt.CONTENEUR_TC.TypeCtr == "40FL")
                    {
                        (range.Cells[i, 6] as Excel.Range).Value2 = "45P3";
                    }
                    else if (mvt.CONTENEUR_TC.TypeCtr == "20OT")
                    {
                        (range.Cells[i, 6] as Excel.Range).Value2 = "42U1";
                    }
                    else
                    {
                        (range.Cells[i, 6] as Excel.Range).Value2 = mvt.CONTENEUR_TC.TypeCtr;
                    }
                    (range.Cells[i, 7] as Excel.Range).Value2 = mvt.CONTENEUR_TC.CONTENEUR.StatutCtr.Substring(0, 1);
                    (range.Cells[i, 8] as Excel.Range).Value2 = mvt.CONTENEUR_TC.TypeCtr.Substring(0, 2);
                    (range.Cells[i, 9] as Excel.Range).Value2 = mvt.PARC.NomParc;
                    (range.Cells[i, 10] as Excel.Range).Value2 = mvt.TYPE_OPERATION.LibTypeOp;
                    (range.Cells[i, 11] as Excel.Range).Value2 = "";
                    (range.Cells[i, 12] as Excel.Range).Value2 = "";
                    (range.Cells[i, 13] as Excel.Range).Value2 = mvt.CONNAISSEMENT.ESCALE.ARMATEUR.NomArm;
                    (range.Cells[i, 14] as Excel.Range).Value2 = mvt.ESCALE.NumEsc;
                    (range.Cells[i, 15] as Excel.Range).Value2 = mvt.ESCALE.NAVIRE.NomNav;

                    i++;
                }

                range.Range[range.Cells[2, 1], range.Cells[i - 1, 15]].Columns.Borders.LineStyle = 1;
                range.Range[range.Cells[2, 1], range.Cells[i - 1, 15]].Font.Name = "Tw Cen MT";

                Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
                dlg.FileName = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\Historique Mouvement - Conteneur - " + ctrTC.NumTC + ".xlsx";
                dlg.DefaultExt = ".xlsx"; // Default file extension
                dlg.Filter = "Excel Documents (.xlsx)|*.xlsx"; // Filter files by extension

                // Show save file dialog box
                Nullable<bool> result = dlg.ShowDialog();

                string filename = "";
                // Process save file dialog box results
                if (result == true)
                {
                    // Save document
                    filename = dlg.FileName;

                    xlWorkBook.SaveAs(filename, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Excel.XlSaveAsAccessMode.xlExclusive, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                    MessageBox.Show("Edition de l'historique des mouvements terminée", "Edition de l'historique des mouvements terminée !", MessageBoxButton.OK, MessageBoxImage.Information);
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
            finally
            {
                if (xlWorkSheet != null)
                {
                    releaseObject(xlWorkSheet);
                }

                if (xlWorkBook != null)
                {
                    xlWorkBook.Close(true, Type.Missing, Type.Missing);
                    releaseObject(xlWorkBook);
                }

                bool excelWasRunning = System.Diagnostics.Process.GetProcessesByName("EXCEL.EXE").Length > 0;

                if (excelWasRunning)
                {
                    xlApp.Quit();
                    releaseObject(xlApp);
                }
            }
        }

        private void releaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception ex)
            {
                obj = null;
                //MessageBox.Show("Unable to release the Object " + ex.ToString());
            }
            finally
            {
                GC.Collect();
            }
        }

        private void ctxAjoutMvt_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //VSOMAccessors vsomAcc = new VSOMAccessors();

                if (operationsUser.Where(op => op.NomOp == "Conteneur : Ajout mouvements").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
                {
                    MessageBox.Show("Vous n'avez pas les droits nécessaires pour créer une operation. Veuillez contacter un administrateur", "Droits insuffisants !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    CreerOperationTrackingForm form = new CreerOperationTrackingForm(this, Convert.ToInt32(txtIdCtr.Text), utilisateur);
                    form.Title = "Insertion des operations - Conteneur N° " + cbNumCtr.Text;
                    form.ShowDialog();
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

        private void btnVendu_Click_1(object sender, RoutedEventArgs e)
        {
            //TODO control des droits
            VendreConteneur objfrm = new VendreConteneur(this, _conteneur_tc, utilisateur);
            objfrm.ShowDialog();
        }

        private void btnRetourVide_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                RetourVideConteneur retourForm = new RetourVideConteneur(this, utilisateur);
                retourForm.Title = "Retour - Conteneur N° : " + cbNumCtr.Text;
                retourForm.ShowDialog();
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
