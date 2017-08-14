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
    /// Logique d'interaction pour ListEscaleForm.xaml
    /// </summary>
    public partial class ListEscaleForm : Window
    {
        public List<ESCALE> escales { get; set; }

        private EscaleForm escForm;
        private OrdreServiceForm osForm;
        private BookingForm bookForm;
        private ConnaissementForm conForm;
        private CubageForm cubForm;
        private ManifestExportForm manExpForm;
        private RapportDebarquementForm rapDebForm;
        private RapportEmbarquementForm rapEmbForm;
        private RapportDebarquementExcelForm rapDebExcelForm;
        private RapportEmbarquementExcelForm rapEmbExcelForm;
        private DischargingReportExcelForm dischargingRepExcelForm;
        private LoadingReportExcelForm loadingRepExcelForm;
        private ExtractPrevImportDITForm extractPrevImportDITForm;
        private ExtractPrevExportDITForm extractPrevExportDITForm;
        private TransfertBookingEscaleForm transfertBookingEscaleForm;
        private ManifesteForm manForm;
        private List<OPERATION> operationsUser;
        private UTILISATEUR utilisateur;

        private FormLoader formLoader;
        private VsomParameters vsp = new VsomParameters();
        public ListEscaleForm(EscaleForm form, List<ESCALE> listEscales, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

                escales = listEscales;
                
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

        public ListEscaleForm(OrdreServiceForm form, List<ESCALE> listEscales, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

                escales = listEscales;

                utilisateur = user;
                operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);

                osForm = form;

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

        public ListEscaleForm(BookingForm form, List<ESCALE> listEscales, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

                escales = listEscales;

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

        public ListEscaleForm(CubageForm form, List<ESCALE> listEscales, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

                escales = listEscales;

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

        public ListEscaleForm(ManifestExportForm form, List<ESCALE> listEscales, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

                escales = listEscales;

                utilisateur = user;
                operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);

                manExpForm = form;

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

        public ListEscaleForm(RapportDebarquementForm form, List<ESCALE> listEscales, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

                escales = listEscales;

                utilisateur = user;
                operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);

                rapDebForm = form;

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

        public ListEscaleForm(RapportEmbarquementForm form, List<ESCALE> listEscales, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

                escales = listEscales;

                utilisateur = user;
                operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);

                rapEmbForm = form;

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

        public ListEscaleForm(RapportDebarquementExcelForm form, List<ESCALE> listEscales, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

                escales = listEscales;

                utilisateur = user;
                operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);

                rapDebExcelForm = form;

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

        public ListEscaleForm(DischargingReportExcelForm form, List<ESCALE> listEscales, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

                escales = listEscales;

                utilisateur = user;
                operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);

                dischargingRepExcelForm = form;

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

        public ListEscaleForm(LoadingReportExcelForm form, List<ESCALE> listEscales, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

                escales = listEscales;

                utilisateur = user;
                operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);

                loadingRepExcelForm = form;

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

        public ListEscaleForm(RapportEmbarquementExcelForm form, List<ESCALE> listEscales, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

                escales = listEscales;

                utilisateur = user;
                operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);

                rapEmbExcelForm = form;

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

        public ListEscaleForm(ExtractPrevImportDITForm form, List<ESCALE> listEscales, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

                escales = listEscales;

                utilisateur = user;
                operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);

                extractPrevImportDITForm = form;

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

        public ListEscaleForm(ExtractPrevExportDITForm form, List<ESCALE> listEscales, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

                escales = listEscales;

                utilisateur = user;
                operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);

                extractPrevExportDITForm = form;

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

        public ListEscaleForm(TransfertBookingEscaleForm form, List<ESCALE> listEscales, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

                escales = listEscales;

                utilisateur = user;
                operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);

                transfertBookingEscaleForm = form;

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

        public ListEscaleForm(ManifesteForm form, List<ESCALE> listEscales, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

                escales = listEscales;

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

        public ListEscaleForm(ConnaissementForm form, List<ESCALE> listEscales, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

                escales = listEscales;

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

        private void dataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (dataGrid.SelectedIndex != -1)
                {
                    if (escForm != null)
                    {
                        formLoader.LoadEscaleForm(escForm, (ESCALE)dataGrid.SelectedItem);
                        this.Close();
                    }
                    else if (osForm != null)
                    {
                        formLoader.LoadEscaleForm(osForm, (ESCALE)dataGrid.SelectedItem);
                        this.Close();
                    }
                    else if (bookForm != null)
                    {
                        formLoader.LoadEscaleForm(bookForm, (ESCALE)dataGrid.SelectedItem);
                        this.Close();
                    }
                    else if (conForm != null)
                    {
                        formLoader.LoadEscaleForm(conForm, (ESCALE)dataGrid.SelectedItem);
                        this.Close();
                    }
                    else if (cubForm != null)
                    {
                        formLoader.LoadEscaleForm(cubForm, (ESCALE)dataGrid.SelectedItem);
                        this.Close();
                    }
                    else if (manExpForm != null)
                    {
                        formLoader.LoadEscaleForm(manExpForm, (ESCALE)dataGrid.SelectedItem);
                        this.Close();
                    }
                    else if (rapDebForm != null)
                    {
                        formLoader.LoadEscaleForm(rapDebForm, (ESCALE)dataGrid.SelectedItem);
                        this.Close();
                    }
                    else if (rapEmbForm != null)
                    {
                        formLoader.LoadEscaleForm(rapEmbForm, (ESCALE)dataGrid.SelectedItem);
                        this.Close();
                    }
                    else if (dischargingRepExcelForm != null)
                    {
                        formLoader.LoadEscaleForm(dischargingRepExcelForm, (ESCALE)dataGrid.SelectedItem);
                        this.Close();
                    }
                    else if (loadingRepExcelForm != null)
                    {
                        formLoader.LoadEscaleForm(loadingRepExcelForm, (ESCALE)dataGrid.SelectedItem);
                        this.Close();
                    }
                    else if (extractPrevImportDITForm != null)
                    {
                        formLoader.LoadEscaleForm(extractPrevImportDITForm, (ESCALE)dataGrid.SelectedItem);
                        this.Close();
                    }
                    else if (extractPrevExportDITForm != null)
                    {
                        formLoader.LoadEscaleForm(extractPrevExportDITForm, (ESCALE)dataGrid.SelectedItem);
                        this.Close();
                    }
                    else if (transfertBookingEscaleForm != null)
                    {
                        formLoader.LoadEscaleForm(transfertBookingEscaleForm, (ESCALE)dataGrid.SelectedItem);
                        this.Close();
                    }
                    else if (manForm != null)
                    {
                        formLoader.LoadEscaleForm(manForm, (ESCALE)dataGrid.SelectedItem);
                        this.Close();
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

        private void btnSelectionner_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dataGrid.SelectedIndex != -1 && dataGrid.SelectedItems.Count == 1)
                {
                    if (escForm != null)
                    {
                        formLoader.LoadEscaleForm(escForm, (ESCALE)dataGrid.SelectedItem);
                        this.Close();
                    }
                    else if (osForm != null)
                    {
                        formLoader.LoadEscaleForm(osForm, (ESCALE)dataGrid.SelectedItem);
                        this.Close();
                    }
                    else if (bookForm != null)
                    {
                        formLoader.LoadEscaleForm(bookForm, (ESCALE)dataGrid.SelectedItem);
                        this.Close();
                    }
                    else if (conForm != null)
                    {
                        formLoader.LoadEscaleForm(conForm, (ESCALE)dataGrid.SelectedItem);
                        this.Close();
                    }
                    else if (cubForm != null)
                    {
                        formLoader.LoadEscaleForm(cubForm, (ESCALE)dataGrid.SelectedItem);
                        this.Close();
                    }
                    else if (manExpForm != null)
                    {
                        formLoader.LoadEscaleForm(manExpForm, (ESCALE)dataGrid.SelectedItem);
                        this.Close();
                    }
                    else if (rapDebForm != null)
                    {
                        formLoader.LoadEscaleForm(rapDebForm, (ESCALE)dataGrid.SelectedItem);
                        this.Close();
                    }
                    else if (rapEmbForm != null)
                    {
                        formLoader.LoadEscaleForm(rapEmbForm, (ESCALE)dataGrid.SelectedItem);
                        this.Close();
                    }
                    else if (dischargingRepExcelForm != null)
                    {
                        formLoader.LoadEscaleForm(dischargingRepExcelForm, (ESCALE)dataGrid.SelectedItem);
                        this.Close();
                    }
                    else if (loadingRepExcelForm != null)
                    {
                        formLoader.LoadEscaleForm(loadingRepExcelForm, (ESCALE)dataGrid.SelectedItem);
                        this.Close();
                    }
                    else if (extractPrevImportDITForm != null)
                    {
                        formLoader.LoadEscaleForm(extractPrevImportDITForm, (ESCALE)dataGrid.SelectedItem);
                        this.Close();
                    }
                    else if (extractPrevExportDITForm != null)
                    {
                        formLoader.LoadEscaleForm(extractPrevExportDITForm, (ESCALE)dataGrid.SelectedItem);
                        this.Close();
                    }
                    else if (transfertBookingEscaleForm != null)
                    {
                        formLoader.LoadEscaleForm(transfertBookingEscaleForm, (ESCALE)dataGrid.SelectedItem);
                        this.Close();
                    }
                    else if (manForm != null)
                    {
                        formLoader.LoadEscaleForm(manForm, (ESCALE)dataGrid.SelectedItem);
                        this.Close();
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
    }
}
