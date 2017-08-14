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
    /// Logique d'interaction pour ConventionnelForm.xaml
    /// </summary>
    public partial class ConventionnelForm : Window
    {
        public List<CONVENTIONNEL> conventionnels { get; set; }
        public List<string> convs { get; set; }

        public List<CONNAISSEMENT> connaissements { get; set; }
        public List<string> cons { get; set; }

        private List<TYPE_CONVENTIONNEL> typesConventionnels;
        public List<string> tps { get; set; }

        public List<ElementFacturation> eltsFact { get; set; }

        public ConventionnelPanel convPanel { get; set; }
        public CubageForm cubForm { get; set; }
        private ManifesteForm manifForm;
        private ConnaissementForm connaissementForm;

        private List<OPERATION> operationsUser;
        private UTILISATEUR utilisateur;

        private FormLoader formLoader;
        private VsomParameters vsp = new VsomParameters();
        private string typeForm;

        public ConventionnelForm(ConventionnelPanel panel, CONVENTIONNEL conv, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();
               // VsomParameters vsp = new VsomParameters();
                InitializeComponent();
                this.DataContext = this;

                typesConventionnels = vsp.GetTypesConventionnelsImport();
                tps = new List<string>();
                foreach (TYPE_CONVENTIONNEL t in typesConventionnels)
                {
                    tps.Add(t.LibTypeGC);
                }

                convPanel = panel;

                utilisateur = user;
                operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);

                formLoader = new FormLoader(utilisateur);

                formLoader.LoadConventionnelForm(this, conv);
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
            
        }

        public ConventionnelForm(CubageForm form, CONVENTIONNEL conv, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();
                //VsomParameters vsp = new VsomParameters();
                InitializeComponent();
                this.DataContext = this;

                typesConventionnels = vsp.GetTypesConventionnelsImport();
                tps = new List<string>();
                foreach (TYPE_CONVENTIONNEL t in typesConventionnels)
                {
                    tps.Add(t.LibTypeGC);
                }

                cubForm = form;

                utilisateur = user;
                operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);

                formLoader = new FormLoader(utilisateur);

                formLoader.LoadConventionnelForm(this, conv);
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }

        }

        public ConventionnelForm(ManifesteForm form, CONVENTIONNEL conv, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();
               // VsomParameters vsp = new VsomParameters();
                InitializeComponent();
                this.DataContext = this;

                typesConventionnels = vsp.GetTypesConventionnelsImport();
                tps = new List<string>();
                foreach (TYPE_CONVENTIONNEL t in typesConventionnels)
                {
                    tps.Add(t.LibTypeGC);
                }

                manifForm = form;

                utilisateur = user;
                operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);

                formLoader = new FormLoader(utilisateur);

                formLoader.LoadConventionnelForm(this, conv);
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
            
        }

        public ConventionnelForm(ConnaissementForm form, CONVENTIONNEL conv, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();
               // VsomParameters vsp = new VsomParameters();
                InitializeComponent();
                this.DataContext = this;

                typesConventionnels = vsp.GetTypesConventionnelsImport();
                tps = new List<string>();
                foreach (TYPE_CONVENTIONNEL t in typesConventionnels)
                {
                    tps.Add(t.LibTypeGC);
                }

                connaissementForm = form;
                utilisateur = user;
                operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);

                formLoader = new FormLoader(utilisateur);

                formLoader.LoadConventionnelForm(this, conv);
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
            
        }

        public ConventionnelForm(string type, ConventionnelPanel panel, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();
               // VsomParameters vsp = new VsomParameters();
                InitializeComponent();
                this.DataContext = this;

                typesConventionnels = vsp.GetTypesConventionnelsImport();
                tps = new List<string>();
                foreach (TYPE_CONVENTIONNEL t in typesConventionnels)
                {
                    tps.Add(t.LibTypeGC);
                }

                convPanel = panel;

                utilisateur = user;
                operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);

                typeForm = type;

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

        private void btnEnregistrer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //VSOMAccessors vsomAcc = new VSOMAccessors();
                VsomMarchal vsomAcc = new VsomMarchal();

                if (cbNumGC.Text == "")
                {
                    MessageBox.Show("Il faut saisir le numéro de chassis de ce general cargo", "N° GC du conventionnel?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (txtDescription.Text == "")
                {
                    MessageBox.Show("Il faut saisir la description ou la marque de ce general cargo", "Description du conventionnel ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (txtLongC.Text == "0")
                {
                    if (MessageBox.Show("La longueur cubée est nulle, voulez-vous la modifier maintenant ?", "Longueur cubée ?", MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
                    {
                        txtLongC.SelectAll();
                        txtLongC.Focus();
                    }
                }
                else if (txtLargC.Text == "0")
                {
                    if (MessageBox.Show("La largeur cubée est nulle, voulez-vous la modifier maintenant ?", "Largeur cubée ?", MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
                    {
                        txtLargC.SelectAll();
                        txtLargC.Focus();
                    }
                }
                else if (txtHautC.Text == "0")
                {
                    if (MessageBox.Show("La hauteur cubée est nulle, voulez-vous la modifier maintenant ?", "Hauteur cubée ?", MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
                    {
                        txtHautC.SelectAll();
                        txtHautC.Focus();
                    }
                }
                else if (txtVolC.Text == "0")
                {
                    MessageBox.Show("Vous ne pouvez pas enregistrer un general cargo avec un cubé volume nul", "Volume cubé ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (txtPoidsC.Text == "0")
                {
                    if (MessageBox.Show("Le poids réel est nul, voulez-vous le modifier maintenant ?", "Poids réel ?", MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
                    {
                        txtPoidsC.SelectAll();
                        txtPoidsC.Focus();
                    }
                }
                else if(typeForm != "Nouveau")
                {
                    if (txtLongC.Text == "0")
                    {
                        if (MessageBox.Show("La longueur cubée est nulle, voulez-vous la modifier maintenant ?", "Longueur cubée ?", MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
                        {
                            txtLongC.SelectAll();
                            txtLongC.Focus();
                        }
                    }
                    else if (txtLargC.Text == "0")
                    {
                        if (MessageBox.Show("La largeur cubée est nulle, voulez-vous la modifier maintenant ?", "Largeur cubée ?", MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
                        {
                            txtLargC.SelectAll();
                            txtLargC.Focus();
                        }
                    }
                    else if (txtHautC.Text == "0")
                    {
                        if (MessageBox.Show("La hauteur cubée est nulle, voulez-vous la modifier maintenant ?", "Hauteur cubée ?", MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
                        {
                            txtHautC.SelectAll();
                            txtHautC.Focus();
                        }
                    }
                    else if (txtVolC.Text == "0")
                    {
                        MessageBox.Show("Vous ne pouvez pas enregistrer un general cargo avec un cubé volume nul", "Volume cubé ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                    else if (txtPoidsC.Text == "0")
                    {
                        if (MessageBox.Show("Le poids réel est nul, voulez-vous le modifier maintenant ?", "Poids réel ?", MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
                        {
                            txtPoidsC.SelectAll();
                            txtPoidsC.Focus();
                        }
                    }
                    else
                    {
                        CONVENTIONNEL conv = null;
                        if (groupInfosManif.IsEnabled == true)
                        {
                            conv = vsomAcc.UpdateConventionnel(Convert.ToInt32(txtIdGC.Text), cbNumGC.Text, txtDescription.Text, typesConventionnels.ElementAt<TYPE_CONVENTIONNEL>(cbTypeConvM.SelectedIndex).CodeTypeGC, Convert.ToDouble(txtPoidsM.Text.Replace(".", ",")), Convert.ToDouble(txtVolM.Text.Replace(".", ",")), Convert.ToDouble(txtLongM.Text.Replace(".", ",")), Convert.ToDouble(txtLargM.Text.Replace(".", ",")), Convert.ToDouble(txtHautM.Text.Replace(".", ",")), utilisateur.IdU);
                        }

                        conv = vsomAcc.UpdateConventionnel(Convert.ToInt32(txtIdGC.Text), typesConventionnels.ElementAt<TYPE_CONVENTIONNEL>(cbTypeConvC.SelectedIndex).CodeTypeGC, Convert.ToDouble(txtPoidsC.Text.Replace(".", ",")), Convert.ToDouble(txtVolC.Text.Replace(".", ",")), Convert.ToDouble(txtLongC.Text.Replace(".", ",")), Convert.ToDouble(txtLargC.Text.Replace(".", ",")), Convert.ToDouble(txtHautC.Text.Replace(".", ",")), utilisateur.IdU);

                        formLoader.LoadConventionnelForm(this, conv);

                        MessageBox.Show("Mise à jour effectuée avec succès", "Modification validée !", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                else if (typeForm == "Nouveau")
                {
                    if (txtVolM.Text == "0")
                    {
                        MessageBox.Show("Vous ne pouvez pas enregistrer un general cargo avec un volume manifesté volume nul", "Volume manifesté ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                    else if (txtPoidsM.Text == "0")
                    {
                        MessageBox.Show("Vous ne pouvez pas enregistrer un general cargo avec un poids manifesté volume nul", "Volume manifesté ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                    else if (cbTypeConvM.SelectedIndex == -1)
                    {
                        MessageBox.Show("VVeuillez sélectionner un type de conventionnel", "Type de conventionnel ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                    else
                    {
                        CONVENTIONNEL conv = vsomAcc.InsertConventionnel(Convert.ToInt32(txtIdBL.Text), cbNumGC.Text, txtDescription.Text, typesConventionnels.ElementAt<TYPE_CONVENTIONNEL>(cbTypeConvM.SelectedIndex).CodeTypeGC, txtBarCode.Text, Convert.ToInt32(txtPoidsM.Text.Replace(".", ",")), Convert.ToDouble(txtVolM.Text.Replace(".", ",")), Convert.ToDouble(txtLongM.Text.Replace(".", ",")), Convert.ToDouble(txtLargM.Text.Replace(".", ",")), Convert.ToDouble(txtHautM.Text.Replace(".", ",")), utilisateur.IdU);

                        formLoader.LoadConventionnelForm(this, conv);
                        typeForm = "";
                        MessageBox.Show("General cargo enregistré avec succès", "General cargo crée !", MessageBoxButton.OK, MessageBoxImage.Information);
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
                //MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
            }
        }

        private void btnHistOps_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                HistoriqueOperationsForm opForm = new HistoriqueOperationsForm(this, Convert.ToInt32(txtIdGC.Text));
                opForm.Title = "Historique des opérations - GC N° " + cbNumGC.Text;
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

        private void btnCalculerSejour_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                CalculerSejourConventionnelForm calcForm = new CalculerSejourConventionnelForm(this, utilisateur);
                calcForm.Title = "Calcul du séjour séjour - Chassis N° : " + cbNumGC.Text;
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

        private void txtDim_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9,.]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void txtDimC_LostFocus(object sender, RoutedEventArgs e)
        {
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

        private void txtDimM_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                if (txtLongM.Text != "" && txtLargM.Text != "" && txtHautM.Text != "")
                {
                    txtVolM.Text = Math.Round((Convert.ToDouble(txtLongM.Text.Replace(".", ",")) * Convert.ToDouble(txtLargM.Text.Replace(".", ",")) * Convert.ToDouble(txtHautM.Text.Replace(".", ","))), 3).ToString();
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
                    noteForm.Title = "Note - " + note.IdNote + " - GC - " + note.CONVENTIONNEL.NumGC;
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

        private void btnIdentifier_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                IdentificationConventionnelForm identForm = new IdentificationConventionnelForm(this, utilisateur);
                identForm.Title = "Identification - General Cargo N° : " + cbNumGC.Text;
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

        private void btnCuber_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CubageConventionnelForm cubForm = new CubageConventionnelForm(this, utilisateur);
                cubForm.Title = "Cubage - Geneal Cargo N° : " + cbNumGC.Text;
                cubForm.ShowDialog();
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        private void btnReceptionner_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                ReceptionConventionnelForm receptionForm = new ReceptionConventionnelForm(this, vsp.GetConventionnelByIdGC(Convert.ToInt32(txtIdGC.Text)), utilisateur);
                receptionForm.Title = "Réception - General Cargo N° : " + cbNumGC.Text;
                receptionForm.ShowDialog();
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
                SortirConventionnelForm sortieForm = new SortirConventionnelForm(this, utilisateur);
                sortieForm.Title = "Sortie - General Cargo N° : " + cbNumGC.Text;
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

        private void btnMAJNumGC_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                MAJNumGCForm mAJNumGCForm = new MAJNumGCForm(this, vsp.GetConventionnelByIdGC(Convert.ToInt32(txtIdGC.Text)), utilisateur);
                mAJNumGCForm.txtAncienGC.Text = cbNumGC.Text;
                mAJNumGCForm.Title = "Mise à jour du general cargo - GC N° : " + cbNumGC.Text;
                mAJNumGCForm.ShowDialog();
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        private void cbNumGC_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                VsomParameters vsomAcc = new VsomParameters();

                if (e.Key == Key.Return && cbNumGC.Text.Trim() != "")
                {
                    conventionnels = vsp.GetConventionnelsByNumGC(cbNumGC.Text);

                    if (conventionnels.Count == 0)
                    {
                        MessageBox.Show("Il n'existe aucun general cargo portant ce numéro", "General cargo introuvable", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                    else if (conventionnels.Count == 1)
                    {
                        CONVENTIONNEL conv = conventionnels.FirstOrDefault<CONVENTIONNEL>();
                        formLoader.LoadConventionnelForm(this, conv);
                    }
                    else
                    {
                        ListConventionnelForm listConvForm = new ListConventionnelForm(this, conventionnels, utilisateur);
                        listConvForm.Title = "Choix multiples : Sélectionnez un véhicule";
                        listConvForm.ShowDialog();
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

        private void dataGridEltsFact_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (dataGridEltsFact.SelectedIndex != -1)
                {
                    UpdateEltGCForm updateEltForm = new UpdateEltGCForm(this, Convert.ToInt32(txtIdGC.Text), utilisateur);
                    updateEltForm.cbElt.SelectedItem = ((ElementFacturation)dataGridEltsFact.SelectedItem).LibArticle;
                    updateEltForm.Title = "Eléments de facturation - GC N° " + cbNumGC.Text;
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

        private void btnAjoutNote_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                NoteForm noteForm = new NoteForm("Nouveau", this, utilisateur);
                noteForm.Title = "Création de note - Conventionnel " + cbNumGC.Text;
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
    }
}
