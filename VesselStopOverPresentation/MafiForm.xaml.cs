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
    /// Logique d'interaction pour MafiForm.xaml
    /// </summary>
    public partial class MafiForm : Window
    {
        public List<MAFI> mafis { get; set; }
        public List<string> mfs { get; set; }

        public List<CONNAISSEMENT> connaissements { get; set; }
        public List<string> cons { get; set; }

        private List<TYPE_CONTENEUR> typesConteneurs;
        public List<string> typesCtrs { get; set; }

        private List<TYPE_CONVENTIONNEL> typesConventionnels;
        public List<string> tps { get; set; }

        public List<ElementFacturation> eltsFact { get; set; }

        private MafiPanel mafiPanel;
        private ManifesteForm manifForm;
        private ConnaissementForm connaissementForm;
        private DemandeRestitutionCautionForm cautionForm;

        public List<OPERATION> operationsUser { get; set; }
        public UTILISATEUR utilisateur { get; set; }

        private string typeForm;

        private FormLoader formLoader;

        private VsomParameters vsp = new VsomParameters();
        public MafiForm(MafiPanel panel, MAFI mf, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();
               // VsomParameters vsp = new VsomParameters();
                InitializeComponent();
                this.DataContext = this;

                typesConteneurs = vsp.GetTypesConteneurs();
                typesCtrs = new List<string>();
                foreach (TYPE_CONTENEUR t in typesConteneurs)
                {
                    typesCtrs.Add(t.CodeTypeCtr);
                }

                mafiPanel = panel;
                utilisateur = user;
                operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);

                formLoader = new FormLoader(utilisateur);

                formLoader.LoadMafiForm(this, mf);

            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
            
        }

        public MafiForm(string type, MafiPanel panel, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();
                VsomParameters vsp = new VsomParameters();
                InitializeComponent();
                this.DataContext = this;

                typesConteneurs = vsp.GetTypesConteneurs();
                typesCtrs = new List<string>();
                foreach (TYPE_CONTENEUR t in typesConteneurs)
                {
                    typesCtrs.Add(t.CodeTypeCtr);
                }

                mafiPanel = panel;
                utilisateur = user;
                operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);

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

        public MafiForm(ManifesteForm form, MAFI mf, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();
                VsomParameters vsp = new VsomParameters();
                InitializeComponent();
                this.DataContext = this;

                typesConteneurs = vsp.GetTypesConteneurs();
                typesCtrs = new List<string>();
                foreach (TYPE_CONTENEUR t in typesConteneurs)
                {
                    typesCtrs.Add(t.CodeTypeCtr);
                }

                manifForm = form;
                utilisateur = user;
                operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);

                formLoader = new FormLoader(utilisateur);

                formLoader.LoadMafiForm(this, mf);
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
            
        }

        public MafiForm(ConnaissementForm form, MAFI mf, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();
                VsomParameters vsp = new VsomParameters();
                InitializeComponent();
                this.DataContext = this;

                typesConteneurs = vsp.GetTypesConteneurs();
                typesCtrs = new List<string>();
                foreach (TYPE_CONTENEUR t in typesConteneurs)
                {
                    typesCtrs.Add(t.CodeTypeCtr);
                }

                connaissementForm = form;
                utilisateur = user;
                operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);

                formLoader = new FormLoader(utilisateur);

                formLoader.LoadMafiForm(this, mf);
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

        private void btnIdentifier_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                IdentificationMafiForm identForm = new IdentificationMafiForm(this, utilisateur);
                MAFI mf = vsp.GetMafiImportByIdMafi(Convert.ToInt32(txtIdMafi.Text));
                identForm.Title = "Identification - Mafi N° : " + cbNumMafi.Text;
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
                //VSOMAccessors vsomAcc = new VSOMAccessors();
                VsomMarchal vsomAcc = new VsomMarchal();

                if (typeForm != "Nouveau")
                {
                    if (cbNumMafi.Text.Trim() == "")
                    {
                        MessageBox.Show("Il faut saisir le numéro de ce mafi", "N° du Mafi?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                    else if (txtDescription.Text.Trim() == "")
                    {
                        MessageBox.Show("Vous devez renseigner une description pour ce mafi", "Description du mafi ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                    else if (txtDescMses.Text.Trim() == "")
                    {
                        MessageBox.Show("Vous devez renseigner une description pour les marchandises que transporte ce mafi", "Description des marchandises ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                    else if (txtTypeMMafi.Text.Trim() == "")
                    {
                        MessageBox.Show("Vous devez renseigner le type de mafi", "Type du mafi ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                    else if (txtTypeCMafi.Text.Trim() == "")
                    {
                        MessageBox.Show("Vous devez renseigner le type de mafi après identification", "Type du mafi ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                    else if (txtPoidsC.Text.Trim() == "")
                    {
                        MessageBox.Show("Vous ne pouvez pas enregistrer ce mafi sans saisir son poids réel", "Poids réel du mafi ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                    else if (txtVolMafi.Text.Trim() == "")
                    {
                        MessageBox.Show("Vous ne pouvez pas enregistrer ce mafi sans saisir son volume réel", "Volume réel du mafi ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                    else
                    {
                        txtPoids.Text = txtPoids.Text.Replace(" ", "").Replace(".", ",");
                        txtPoidsC.Text = txtPoidsC.Text.Replace(" ", "").Replace(".", ",");
                        txtVolMafi.Text = txtVolMafi.Text.Replace(" ", "").Replace(".", ",");
                        MAFI mf = vsomAcc.UpdateMafi(Convert.ToInt32(txtIdMafi.Text), cbNumMafi.Text, txtDescription.Text, txtIMDGCode.Text, txtDescMses.Text, txtTypeMMafi.Text, txtTypeCMafi.Text, Convert.ToInt32(txtPoidsC.Text), Convert.ToDouble(txtVolMafi.Text), utilisateur.IdU);

                        formLoader.LoadMafiForm(this, mf);

                        MessageBox.Show("Mise à jour effectuée avec succès", "Modification validée !", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                else
                {
                    if (txtIdBL.Text == "")
                    {
                        MessageBox.Show("Veuillez spécifier un BL pour la création d'un mafi", "Sélectionnez un BL ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                    else if (cbNumMafi.Text.Trim() == "")
                    {
                        MessageBox.Show("Il faut saisir le numéro de ce mafi", "N° du mafi?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                    else if (txtDescription.Text.Trim() == "")
                    {
                        MessageBox.Show("Vous devez renseigner une description pour ce mafi", "Description du mafi ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                    else if (txtDescMses.Text.Trim() == "")
                    {
                        MessageBox.Show("Vous devez renseigner une description pour les marchandises que transporte ce mafi", "Description des marchandises ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                    else if (txtTypeMMafi.Text.Trim() == "")
                    {
                        MessageBox.Show("Vous devez renseigner le type de mafi", "Type du mafi ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                    else
                    {
                        txtPoids.Text = txtPoids.Text.Replace(" ", "").Replace(".", ",");
                        txtPoidsC.Text = txtPoidsC.Text.Replace(" ", "").Replace(".", ",");
                        txtVolMafi.Text = txtVolMafi.Text.Replace(" ", "").Replace(".", ",");
                        MAFI mf = vsomAcc.InsertMafi(Convert.ToInt32(txtIdBL.Text), cbNumMafi.Text, txtDescription.Text, txtDescMses.Text, txtIMDGCode.Text, txtTypeMMafi.Text, Convert.ToInt32(txtPoids.Text), Convert.ToDouble(txtVolMafi.Text), utilisateur.IdU);

                        formLoader.LoadMafiForm(this, mf);
                        typeForm = "";
                        MessageBox.Show("Mafi crée avec succès", "Mafi enregistré !", MessageBoxButton.OK, MessageBoxImage.Information);
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
                CalculerStationnementMafiForm calcForm = new CalculerStationnementMafiForm(this, utilisateur);
                calcForm.Title = "Calcul du stationnement - Mafi N° : " + cbNumMafi.Text;
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

        private void btnHistOps_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                HistoriqueOperationsForm opForm = new HistoriqueOperationsForm(this, Convert.ToInt32(txtIdMafi.Text));
                opForm.Title = "Historique des opérations - Mafi N° " + cbNumMafi.Text;
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

        private void cbNumMafi_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                if (e.Key == Key.Return && cbNumMafi.Text.Trim() != "")
                {
                    mafis = vsp.GetMafisImportByNumMafi(cbNumMafi.Text);

                    if (mafis.Count == 0)
                    {
                        MessageBox.Show("Il n'existe aucun mafi portant ce numéro", "Mafi introuvable", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                    else if (mafis.Count == 1)
                    {
                        MAFI mf = mafis.FirstOrDefault<MAFI>();
                        formLoader.LoadMafiForm(this, mf);
                    }
                    else
                    {
                        ListMafiForm listMafiForm = new ListMafiForm(this, mafis, utilisateur);
                        listMafiForm.Title = "Choix multiples : Sélectionnez un mafi";
                        listMafiForm.ShowDialog();
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

        //private void radioLigneMar_Checked(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        VSOMAccessors vsomAcc = new VSOMAccessors();

        //        CONTENEUR ctr = vsp.GetConteneurImportByIdCtr(Convert.ToInt32(txtIdMafi.Text));

        //        if (!ctr.IdPay.HasValue)
        //        {
        //            if (ctr.CONNAISSEMENT.DestBL == "DLA")
        //            {
        //                txtCaution.Text = ctr.TYPE_CONTENEUR.PUDLA.ToString();
        //            }
        //            else if (ctr.CONNAISSEMENT.DestBL == "CMR")
        //            {
        //                txtCaution.Text = ctr.TYPE_CONTENEUR.PUCMR.ToString();
        //            }
        //            else if (ctr.CONNAISSEMENT.DestBL == "HINT")
        //            {
        //                txtCaution.Text = ctr.TYPE_CONTENEUR.PUINT.ToString();
        //            }
        //        }
        //        else
        //        {
        //            //MessageBox.Show("Mise à jour impossible, la caution a déjà été payée", "Mise à jour du propriétaire impossible", MessageBoxButton.OK, MessageBoxImage.Exclamation);
        //        }
        //    }
        //    catch (ApplicationException ex)
        //    {
        //        MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //}

        //private void radioConsignee_Checked(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        VSOMAccessors vsomAcc = new VSOMAccessors();

        //        CONTENEUR ctr = vsp.GetConteneurImportByIdCtr(Convert.ToInt32(txtIdMafi.Text));

        //        if (!ctr.IdPay.HasValue)
        //        {
        //            txtCaution.Text = "0";
        //        }
        //        else
        //        {
        //            //MessageBox.Show("Mise à jour impossible, la caution a déjà été payée", "Mise à jour du propriétaire impossible", MessageBoxButton.OK, MessageBoxImage.Exclamation);
        //        }
        //    }
        //    catch (ApplicationException ex)
        //    {
        //        MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //}

        private void cbNumBL_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                if (e.Key == Key.Return && cbNumBL.Text.Trim() != "")
                {
                    connaissements = vsp.GetConnaissementByNumBL(cbNumBL.Text);

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

        private void listNotes_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (listNotes.SelectedIndex != -1)
                {
                    NOTE note = (NOTE)listNotes.SelectedItem;
                    NoteForm noteForm = new NoteForm(this, note, utilisateur);
                    noteForm.Title = "Note - " + note.IdNote + " - Mafi - " + note.MAFI.NumMafi;
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

        private void btnSortir_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SortirMafiForm sortieForm = new SortirMafiForm(this, utilisateur);
                sortieForm.Title = "Sortie - Mafi N° : " + cbNumMafi.Text;
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

        private void btnAjoutNote_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                NoteForm noteForm = new NoteForm("Nouveau", this, utilisateur);
                noteForm.Title = "Création de note - Mafi " + cbNumMafi.Text;
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

        private void dataGridEltsFact_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (dataGridEltsFact.SelectedIndex != -1)
                {
                    UpdateEltMafiForm updateEltForm = new UpdateEltMafiForm(this, Convert.ToInt32(txtIdMafi.Text), utilisateur);
                    updateEltForm.cbElt.SelectedItem = ((ElementFacturation)dataGridEltsFact.SelectedItem).LibArticle;
                    updateEltForm.Title = "Eléments de facturation - Mafi N° " + cbNumMafi.Text;
                    updateEltForm.ShowDialog();
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

        private void txtVolMafi_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9.,]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
