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
    /// Logique d'interaction pour NoteForm.xaml
    /// </summary>
    public partial class NoteForm : Window
    {

        private EscaleForm escForm;
        private ManifesteForm manForm;
        private ConnaissementForm conForm;
        private BookingForm bookForm;
        private VehiculeForm vehForm;
        private ConteneurForm ctrForm;
        private MafiForm mafiForm;
        private ConventionnelForm convForm;
        private DemandeReductionForm reducForm;

        private List<OPERATION> operationsUser;
        private UTILISATEUR utilisateur;

        private FormLoader formLoader;
        private VsomParameters vsp = new VsomParameters();
         
        public NoteForm(string type, EscaleForm form, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

                utilisateur = user;
                operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);

                escForm = form;

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

        public NoteForm(EscaleForm form, NOTE note, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

                utilisateur = user;
                operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);

                escForm = form;

                txtTitreNote.Text = note.TitreNote;
                txtObservations.Document.Blocks.Clear();
                txtObservations.Document.Blocks.Add(new Paragraph(new Run(note.DescNote)));
                btnEnregistrer.IsEnabled = false;
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        public NoteForm(string type, ManifesteForm form, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

                utilisateur = user;
                operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);

                manForm = form;

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

        public NoteForm(ManifesteForm form, NOTE note, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

                utilisateur = user;
                operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);

                manForm = form;

                txtTitreNote.Text = note.TitreNote;
                txtObservations.Document.Blocks.Clear();
                txtObservations.Document.Blocks.Add(new Paragraph(new Run(note.DescNote)));
                btnEnregistrer.IsEnabled = false;
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        public NoteForm(string type, ConnaissementForm form, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

                utilisateur = user;
                operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);

                conForm = form;

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

        public NoteForm(ConnaissementForm form, NOTE note, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

                utilisateur = user;
                operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);

                conForm = form;

                txtTitreNote.Text = note.TitreNote;
                txtObservations.Document.Blocks.Clear();
                txtObservations.Document.Blocks.Add(new Paragraph(new Run(note.DescNote)));
                btnEnregistrer.IsEnabled = false;
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        public NoteForm(string type, BookingForm form, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

                utilisateur = user;
                operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);

                bookForm = form;

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

        public NoteForm(BookingForm form, NOTE note, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

                utilisateur = user;
                operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);

                bookForm = form;

                txtTitreNote.Text = note.TitreNote;
                txtObservations.Document.Blocks.Clear();
                txtObservations.Document.Blocks.Add(new Paragraph(new Run(note.DescNote)));
                btnEnregistrer.IsEnabled = false;
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        public NoteForm(string type, VehiculeForm form, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

                utilisateur = user;
                operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);

                vehForm = form;

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

        public NoteForm(VehiculeForm form, NOTE note, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

                utilisateur = user;
                operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);

                vehForm = form;

                txtTitreNote.Text = note.TitreNote;
                txtObservations.Document.Blocks.Clear();
                txtObservations.Document.Blocks.Add(new Paragraph(new Run(note.DescNote)));
                btnEnregistrer.IsEnabled = false;
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        public NoteForm(string type, ConteneurForm form, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

                utilisateur = user;
                operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);

                ctrForm = form;

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

        public NoteForm(ConteneurForm form, NOTE note, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

                utilisateur = user;
                operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);

                ctrForm = form;

                txtTitreNote.Text = note.TitreNote;
                txtObservations.Document.Blocks.Clear();
                txtObservations.Document.Blocks.Add(new Paragraph(new Run(note.DescNote)));
                btnEnregistrer.IsEnabled = false;
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        public NoteForm(string type, ConventionnelForm form, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

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

        public NoteForm(ConventionnelForm form, NOTE note, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

                utilisateur = user;
                operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);

                convForm = form;

                txtTitreNote.Text = note.TitreNote;
                txtObservations.Document.Blocks.Clear();
                txtObservations.Document.Blocks.Add(new Paragraph(new Run(note.DescNote)));
                btnEnregistrer.IsEnabled = false;
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        public NoteForm(string type, MafiForm form, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

                utilisateur = user;
                operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);

                mafiForm = form;

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

        public NoteForm(MafiForm form, NOTE note, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

                utilisateur = user;
                operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);

                mafiForm = form;

                txtTitreNote.Text = note.TitreNote;
                txtObservations.Document.Blocks.Clear();
                txtObservations.Document.Blocks.Add(new Paragraph(new Run(note.DescNote)));
                btnEnregistrer.IsEnabled = false;
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        public NoteForm(string type, DemandeReductionForm form, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

                utilisateur = user;
                operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);

                reducForm = form;

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

        public NoteForm(DemandeReductionForm form, NOTE note, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

                utilisateur = user;
                operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);

                reducForm = form;

                txtTitreNote.Text = note.TitreNote;
                txtObservations.Document.Blocks.Clear();
                txtObservations.Document.Blocks.Add(new Paragraph(new Run(note.DescNote)));
                btnEnregistrer.IsEnabled = false;
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
                VsomNotes vsomAcc = new VsomNotes();

                if (new TextRange(txtObservations.Document.ContentStart, txtObservations.Document.ContentEnd).Text.Trim() == "")
                {
                    MessageBox.Show("Vous ne pouvez pas créer de note vide", "Description ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    if (escForm != null)
                    {
                        NOTE note = vsomAcc.InsertNoteEscale(Convert.ToInt32(escForm.txtEscaleSysID.Text), txtTitreNote.Text, new TextRange(txtObservations.Document.ContentStart, txtObservations.Document.ContentEnd).Text, utilisateur.IdU);
                        MessageBox.Show("Note enregistrée !", "Note enregistrée !", MessageBoxButton.OK, MessageBoxImage.Information);
                        formLoader.LoadEscaleForm(escForm, note.ESCALE);
                    }
                    else if (manForm != null)
                    {
                        NOTE note = vsomAcc.InsertNoteManifeste(Convert.ToInt32(manForm.cbIdMan.Text), txtTitreNote.Text, new TextRange(txtObservations.Document.ContentStart, txtObservations.Document.ContentEnd).Text, utilisateur.IdU);
                        MessageBox.Show("Note enregistrée !", "Note enregistrée !", MessageBoxButton.OK, MessageBoxImage.Information);
                        formLoader.LoadManifesteForm(manForm, note.MANIFESTE);
                    }
                    else if (conForm != null)
                    {
                        NOTE note = vsomAcc.InsertNoteConnaissement(Convert.ToInt32(conForm.txtIdBL.Text), txtTitreNote.Text, new TextRange(txtObservations.Document.ContentStart, txtObservations.Document.ContentEnd).Text, utilisateur.IdU);
                        MessageBox.Show("Note enregistrée !", "Note enregistrée !", MessageBoxButton.OK, MessageBoxImage.Information);
                        formLoader.LoadConnaissementForm(conForm, note.CONNAISSEMENT);
                    }
                    else if (bookForm != null)
                    {
                        NOTE note = vsomAcc.InsertNoteBooking(Convert.ToInt32(bookForm.txtSysId.Text), txtTitreNote.Text, new TextRange(txtObservations.Document.ContentStart, txtObservations.Document.ContentEnd).Text, utilisateur.IdU);
                        MessageBox.Show("Note enregistrée !", "Note enregistrée !", MessageBoxButton.OK, MessageBoxImage.Information);
                        formLoader.LoadBookingForm(bookForm, note.CONNAISSEMENT);
                    }
                    else if (vehForm != null)
                    {
                        NOTE note = vsomAcc.InsertNoteVehicule(Convert.ToInt32(vehForm.txtIdChassis.Text), txtTitreNote.Text, new TextRange(txtObservations.Document.ContentStart, txtObservations.Document.ContentEnd).Text, utilisateur.IdU);
                        MessageBox.Show("Note enregistrée !", "Note enregistrée !", MessageBoxButton.OK, MessageBoxImage.Information);
                        formLoader.LoadVehiculeForm(vehForm, note.VEHICULE);
                    }
                    else if (ctrForm != null)
                    {
                        NOTE note = vsomAcc.InsertNoteConteneur(Convert.ToInt32(ctrForm.txtIdCtr.Text), txtTitreNote.Text, new TextRange(txtObservations.Document.ContentStart, txtObservations.Document.ContentEnd).Text, utilisateur.IdU);
                        MessageBox.Show("Note enregistrée !", "Note enregistrée !", MessageBoxButton.OK, MessageBoxImage.Information);
                        formLoader.LoadConteneurForm(ctrForm, note.CONTENEUR);
                    }
                    else if (mafiForm != null)
                    {
                        NOTE note = vsomAcc.InsertNoteConteneur(Convert.ToInt32(mafiForm.txtIdMafi.Text), txtTitreNote.Text, new TextRange(txtObservations.Document.ContentStart, txtObservations.Document.ContentEnd).Text, utilisateur.IdU);
                        MessageBox.Show("Note enregistrée !", "Note enregistrée !", MessageBoxButton.OK, MessageBoxImage.Information);
                        formLoader.LoadMafiForm(mafiForm, note.MAFI);
                    }
                    else if (convForm != null)
                    {
                        NOTE note = vsomAcc.InsertNoteConventionnel(Convert.ToInt32(convForm.txtIdGC.Text), txtTitreNote.Text, new TextRange(txtObservations.Document.ContentStart, txtObservations.Document.ContentEnd).Text, utilisateur.IdU);
                        MessageBox.Show("Note enregistrée !", "Note enregistrée !", MessageBoxButton.OK, MessageBoxImage.Information);
                        formLoader.LoadConventionnelForm(convForm, note.CONVENTIONNEL);
                    }
                    else if (reducForm != null)
                    {
                        NOTE note = vsomAcc.InsertNoteReduction(Convert.ToInt32(reducForm.cbIdRed.Text), txtTitreNote.Text, new TextRange(txtObservations.Document.ContentStart, txtObservations.Document.ContentEnd).Text, utilisateur.IdU);
                        MessageBox.Show("Note enregistrée !", "Note enregistrée !", MessageBoxButton.OK, MessageBoxImage.Information);
                        formLoader.LoadDemandeReductionForm(reducForm, note.DEMANDE_REDUCTION);
                    }
                    
                    this.Close();
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
        
    }
}
