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
    /// Logique d'interaction pour ConteneurForm.xaml
    /// </summary>
    public partial class ConteneurForm : Window
    {
        public List<CONTENEUR> conteneurs { get; set; }
        public List<string> ctrs { get; set; }

        public List<CONNAISSEMENT> connaissements { get; set; }
        public List<string> cons { get; set; }

        private List<TYPE_CONTENEUR> typesConteneurs;
        public List<string> typesCtrs { get; set; }

        public List<ElementFacturation> eltsFact { get; set; }

        private ConteneurPanel contPanel;
        private ConteneurExportPanel contExpPanel;
        private ManifesteForm manifForm;
        private ConnaissementForm connaissementForm;
        private DemandeRestitutionCautionForm cautionForm;

        public List<OPERATION> operationsUser { get; set; }
        public UTILISATEUR utilisateur { get; set; }

        private string typeForm;

        private FormLoader formLoader;
        private VSOMAccessors vsomAcc;
        public ConteneurForm(ConteneurPanel panel, CONTENEUR ctr, UTILISATEUR user)
        {
            try
            {
                 vsomAcc = new VSOMAccessors();
               // VsomParameters vsp = new VsomParameters();
                InitializeComponent();
                this.DataContext = this;

                typesConteneurs = vsomAcc.GetTypesConteneurs();
                typesCtrs = new List<string>();
                foreach (TYPE_CONTENEUR t in typesConteneurs)
                {
                    typesCtrs.Add(t.CodeTypeCtr);
                }

                contPanel = panel;
                utilisateur = user;
                operationsUser = vsomAcc.GetOperationsUtilisateur(utilisateur.IdU);

                formLoader = new FormLoader(utilisateur);

                formLoader.LoadConteneurForm(this, ctr);

            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
            
        }

        public ConteneurForm(ConteneurExportPanel panel, CONTENEUR ctr, UTILISATEUR user)
        {
            try
            {
                 vsomAcc = new VSOMAccessors();
                //VsomParameters vsp = new VsomParameters();
                InitializeComponent();
                this.DataContext = this;

                typesConteneurs = vsomAcc.GetTypesConteneurs();
                typesCtrs = new List<string>();
                foreach (TYPE_CONTENEUR t in typesConteneurs)
                {
                    typesCtrs.Add(t.CodeTypeCtr);
                }

                contExpPanel = panel;
                utilisateur = user;
                operationsUser = vsomAcc.GetOperationsUtilisateur(utilisateur.IdU);

                formLoader = new FormLoader(utilisateur);

                formLoader.LoadConteneurForm(this, ctr);

            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }

        }

        public ConteneurForm(string type, ConteneurPanel panel, UTILISATEUR user)
        {
            try
            {
                 vsomAcc = new VSOMAccessors();
                //VsomParameters vsp = new VsomParameters();
                InitializeComponent();
                this.DataContext = this;

                typesConteneurs = vsomAcc.GetTypesConteneurs();
                typesCtrs = new List<string>();
                foreach (TYPE_CONTENEUR t in typesConteneurs)
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

        public ConteneurForm(ManifesteForm form, CONTENEUR ctr, UTILISATEUR user)
        {
            try
            {
                 vsomAcc = new VSOMAccessors();
                //VsomParameters vsp = new VsomParameters();
                InitializeComponent();
                this.DataContext = this;

                typesConteneurs = vsomAcc.GetTypesConteneurs();
                typesCtrs = new List<string>();
                foreach (TYPE_CONTENEUR t in typesConteneurs)
                {
                    typesCtrs.Add(t.CodeTypeCtr);
                }

                manifForm = form;
                utilisateur = user;
                operationsUser = vsomAcc.GetOperationsUtilisateur(utilisateur.IdU);

                formLoader = new FormLoader(utilisateur);

                formLoader.LoadConteneurForm(this, ctr);
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
            
        }

        public ConteneurForm(DemandeRestitutionCautionForm form, CONTENEUR ctr, UTILISATEUR user)
        {
            try
            {
                 vsomAcc = new VSOMAccessors();
                //VsomParameters vsp = new VsomParameters();
                InitializeComponent();
                this.DataContext = this;

                typesConteneurs = vsomAcc.GetTypesConteneurs();
                typesCtrs = new List<string>();
                foreach (TYPE_CONTENEUR t in typesConteneurs)
                {
                    typesCtrs.Add(t.CodeTypeCtr);
                }

                cautionForm = form;
                utilisateur = user;
                operationsUser = vsomAcc.GetOperationsUtilisateur(utilisateur.IdU);

                formLoader = new FormLoader(utilisateur);

                formLoader.LoadConteneurForm(this, ctr);
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }

        }

        public ConteneurForm(ConnaissementForm form, CONTENEUR ctr, UTILISATEUR user)
        {
            try
            {
                 vsomAcc = new VSOMAccessors();
                //VsomParameters vsp = new VsomParameters();
                InitializeComponent();
                this.DataContext = this;

                typesConteneurs = vsomAcc.GetTypesConteneurs();
                typesCtrs = new List<string>();
                foreach (TYPE_CONTENEUR t in typesConteneurs)
                {
                    typesCtrs.Add(t.CodeTypeCtr);
                }

                connaissementForm = form;
                utilisateur = user;
                operationsUser = vsomAcc.GetOperationsUtilisateur(utilisateur.IdU);

                formLoader = new FormLoader(utilisateur);

                formLoader.LoadConteneurForm(this, ctr);
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

        private void btnDIT_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DITForm dITForm = new DITForm(this, Convert.ToInt32(txtIdCtr.Text), utilisateur);
                dITForm.Title = "Eléments de facturation DIT - Ctr N° " + cbNumCtr.Text;
                dITForm.ShowDialog();
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
            
        }

        private void btnIdentifier_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                 vsomAcc = new VSOMAccessors();
                
             //VsomParameters vsp = new VsomParameters();
                IdentificationConteneurForm identForm = new IdentificationConteneurForm(this, utilisateur);
                CONTENEUR ctr = vsomAcc.GetConteneurImportByIdCtr(Convert.ToInt32(txtIdCtr.Text));
                identForm.txtSeal1.Text = ctr.Seal1Ctr;
                identForm.txtSeal2.Text = ctr.Seal2Ctr;
                identForm.Title = "Identification - Conteneur N° : " + cbNumCtr.Text;
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

        private void btnEnregistrer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                 vsomAcc = new VSOMAccessors();
                // vsomAcc = new VsomMarchal();

                if (typeForm != "Nouveau")
                {
                    if (cbNumCtr.Text.Trim() == "")
                    {
                        MessageBox.Show("Il faut saisir le numéro de ce conteneur", "N° du conteneur?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                    else if (txtDescription.Text == "")
                    {
                        MessageBox.Show("Vous devez renseigner une description pour ce conteneur", "Description du conteneur ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                    else if (txtDescMses.Text == "")
                    {
                        MessageBox.Show("Vous devez renseigner une description pour les marchandises que transporte ce conteneur", "Description des marchandises ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                    else if (cbTypeCtrM.SelectedIndex == -1)
                    {
                        MessageBox.Show("Vous devez renseigner le type de conteneur", "Type du conteneur ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                    else if (cbTypeCtrC.SelectedIndex == -1)
                    {
                        MessageBox.Show("Vous devez renseigner le type de conteneur après identification", "Type du conteneur ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                    else if (txtPoidsC.Text == "0")
                    {
                        MessageBox.Show("Vous ne pouvez pas enregistrer ce conteneur sans saisir son poids réel", "Poids réel du conteneur ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                    else
                    {

                        string typeMses;
                        if (cbTypeMses.SelectedIndex == 0)
                        {
                            typeMses = "N";
                        }
                        else if (cbTypeMses.SelectedIndex == 1)
                        {
                            typeMses = "P";
                        }
                        else
                        {
                            typeMses = "D";
                        }
                        CONTENEUR ctr = vsomAcc.UpdateConteneur(Convert.ToInt32(txtIdCtr.Text), cbNumCtr.Text, radioLigneMar.IsChecked == true ? 1 : 2, txtDescription.Text, txtIMDGCode.Text, txtDescMses.Text, typeMses, txtSeal1.Text, txtSeal2.Text, cbEtatM.SelectedIndex == 0 ? "F" : "E", cbTypeCtrM.Text, cbTypeCtrC.Text, Convert.ToInt32(txtPoidsC.Text), utilisateur.IdU);

                        formLoader.LoadConteneurForm(this, ctr);

                        MessageBox.Show("Mise à jour effectuée avec succès", "Modification validée !", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                else
                {
                    if (txtIdBL.Text == "")
                    {
                        MessageBox.Show("Veuillez spécifier un BL pour la création d'un conteneur", "Sélectionnez un BL ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                    else if (cbNumCtr.Text.Trim() == "")
                    {
                        MessageBox.Show("Il faut saisir le numéro de ce conteneur", "N° du conteneur?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                    else if (txtDescription.Text == "")
                    {
                        MessageBox.Show("Vous devez renseigner une description pour ce conteneur", "Description du conteneur ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                    else if (txtDescMses.Text == "")
                    {
                        MessageBox.Show("Vous devez renseigner une description pour les marchandises que transporte ce conteneur", "Description des marchandises ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                    else if (cbTypeCtrM.SelectedIndex == -1)
                    {
                        MessageBox.Show("Vous devez renseigner le type de conteneur", "Type du conteneur ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                    else
                    {
                        txtPoids.Text = txtPoids.Text.Replace(" ", "");
                        CONTENEUR ctr = vsomAcc.InsertConteneur(Convert.ToInt32(txtIdBL.Text), cbNumCtr.Text, txtDescription.Text, txtDescMses.Text, txtIMDGCode.Text, cbTypeCtrM.Text, cbEtatM.SelectedIndex == 0 ? "F" : "E", Convert.ToInt32(txtPoids.Text), txtSeal1.Text, txtSeal2.Text, utilisateur.IdU);

                        formLoader.LoadConteneurForm(this, ctr);
                        typeForm = "";
                        MessageBox.Show("Conteneur crée avec succès", "Conteneur enregistré !", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                
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

        private void btnCalcStationnement_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CalculerStationnementCtrForm calcForm = new CalculerStationnementCtrForm(this, utilisateur);
                calcForm.Title = "Calcul des surestaries - Conteneur N° : " + cbNumCtr.Text;
                calcForm.ShowDialog();
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        private void btnProgVisiteDouane_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ProgVisiteDouaneCtrForm progVisiteDouaneForm = new ProgVisiteDouaneCtrForm(this, utilisateur);
                progVisiteDouaneForm.Title = "Programmation de la visite douane - Conteneur N° : " + cbNumCtr.Text;
                progVisiteDouaneForm.ShowDialog();
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        private void btnProgDoubleRelevage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ProgDoubleRelevageCtrForm progDoubleRelevageForm = new ProgDoubleRelevageCtrForm(this, utilisateur);
                progDoubleRelevageForm.Title = "Programmation du double relevage - Conteneur N° : " + cbNumCtr.Text;
                progDoubleRelevageForm.ShowDialog();
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        private void btnHistOps_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                HistoriqueOperationsForm opForm = new HistoriqueOperationsForm(this, Convert.ToInt32(txtIdCtr.Text));
                opForm.Title = "Historique des opérations - Conteneur N° " + cbNumCtr.Text;
                opForm.ShowDialog();
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
                
                 VsomParameters vsp = new VsomParameters();
                if (e.Key == Key.Return && cbNumCtr.Text.Trim() != "")
                {
                    conteneurs = vsomAcc.GetConteneursImportByNumCtr(cbNumCtr.Text);

                    if (conteneurs.Count == 0)
                    {
                        MessageBox.Show("Il n'existe aucun conteneur portant ce numéro", "Conteneur introuvable", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                    else if (conteneurs.Count == 1)
                    {
                        CONTENEUR ctr = conteneurs.FirstOrDefault<CONTENEUR>();
                        formLoader.LoadConteneurForm(this, ctr);
                    }
                    else
                    {
                        ListConteneurForm listCtrForm = new ListConteneurForm(this, conteneurs, utilisateur);
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

        private void radioLigneMar_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                //VSOMAccessors vsomAcc = new VSOMAccessors();
                
                 //VsomParameters vsp = new VsomParameters();
                CONTENEUR ctr = vsomAcc.GetConteneurImportByIdCtr(Convert.ToInt32(txtIdCtr.Text));

                if (!ctr.IdPay.HasValue)
                {
                    if (ctr.CONNAISSEMENT.DestBL == "DLA")
                    {
                        txtCaution.Text = ctr.TYPE_CONTENEUR.PUDLA.ToString();
                    }
                    else if (ctr.CONNAISSEMENT.DestBL == "CMR")
                    {
                        txtCaution.Text = ctr.TYPE_CONTENEUR.PUCMR.ToString();
                    }
                    else if (ctr.CONNAISSEMENT.DestBL == "HINT")
                    {
                        txtCaution.Text = ctr.TYPE_CONTENEUR.PUINT.ToString();
                    }
                }
                else
                {
                    //MessageBox.Show("Mise à jour impossible, la caution a déjà été payée", "Mise à jour du propriétaire impossible", MessageBoxButton.OK, MessageBoxImage.Exclamation);
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

        private void radioConsignee_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                //VSOMAccessors vsomAcc = new VSOMAccessors();
                
             //VsomParameters vsp = new VsomParameters();
                CONTENEUR ctr = vsomAcc.GetConteneurImportByIdCtr(Convert.ToInt32(txtIdCtr.Text));

                if (!ctr.IdPay.HasValue)
                {
                    txtCaution.Text = "0";
                }
                else
                {
                    //MessageBox.Show("Mise à jour impossible, la caution a déjà été payée", "Mise à jour du propriétaire impossible", MessageBoxButton.OK, MessageBoxImage.Exclamation);
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
                
             //VsomParameters vsp = new VsomParameters();
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

        private void listNotes_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (listNotes.SelectedIndex != -1)
                {
                    NOTE note = (NOTE)listNotes.SelectedItem;
                    NoteForm noteForm = new NoteForm(this, note, utilisateur);
                    noteForm.Title = "Note - " + note.IdNote + " - Ctr - " + note.CONTENEUR.NumCtr;
                    noteForm.lblStatut.Content = "Note crée le : " + note.DateNote + " par " + note.UTILISATEUR.NU;
                    noteForm.ShowDialog();
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

        private void cbTypeMses_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
              //  VSOMAccessors vsomAcc = new VSOMAccessors();
                
             //VsomParameters vsp = new VsomParameters();
                if (cbTypeMses.SelectedIndex > 2 && vsomAcc.GetConteneurByIdCtr(Convert.ToInt32(txtIdCtr.Text)).SensCtr == "I")
                {
                    MessageBox.Show("Type de marchandise non valide pour un conteneur import", "Echec de la selection !", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else if (cbTypeMses.SelectedIndex < 3 && vsomAcc.GetConteneurByIdCtr(Convert.ToInt32(txtIdCtr.Text)).SensCtr == "E")
                {
                    MessageBox.Show("Type de marchandise non valide pour un conteneur export", "Echec de la selection !", MessageBoxButton.OK, MessageBoxImage.Error);
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

        private void btnAjoutNote_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                NoteForm noteForm = new NoteForm("Nouveau", this, utilisateur);
                noteForm.Title = "Création de note - Conteneur " + cbNumCtr.Text;
                noteForm.lblStatut.Content = "Note crée par : " + utilisateur.NU;
                noteForm.ShowDialog();
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
    }
}
