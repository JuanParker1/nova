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
    /// Logique d'interaction pour ListConnaissementForm.xaml
    /// </summary>
    public partial class ListConnaissementForm : Window
    {
        public List<CONNAISSEMENT> connaissemments { get; set; }

        private ConnaissementForm conForm;
        private BonEnleverForm baeForm;
        private FactureDITForm factDITForm;
        private DemandeLivraisonForm livForm;
        private DemandeRestitutionCautionForm cautionForm;
        private BonSortieForm sortieForm;
        private DemandeReductionForm reducForm;
        private ExtensionFranchiseForm extForm;
        private DemandeVisiteForm visiteForm;
        private ProformaForm profForm;
        private PaiementForm payForm;
        private OrdreServiceForm osForm;
        private VehiculeForm vehForm;
        private ConteneurForm ctrForm;
        private ConteneurTCForm ctrTCForm;
        private MafiForm mafiForm;
        private ConventionnelForm convForm;
        private List<OPERATION> operationsUser;
        private UTILISATEUR utilisateur;

        private FormLoader formLoader;
        private VsomParameters vsp = new VsomParameters();

        public ListConnaissementForm(ConnaissementForm form, List<CONNAISSEMENT> listConnaissements, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

                connaissemments = listConnaissements;
                dataGrid.ItemsSource = connaissemments;
                
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

        public ListConnaissementForm(FactureDITForm form, List<CONNAISSEMENT> listConnaissements, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

                connaissemments = listConnaissements;
                dataGrid.ItemsSource = connaissemments;

                utilisateur = user;
                operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);

                factDITForm = form;

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

        public ListConnaissementForm(BonEnleverForm form, List<CONNAISSEMENT> listConnaissements, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

                connaissemments = listConnaissements;
                dataGrid.ItemsSource = connaissemments;

                utilisateur = user;
                operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);

                baeForm = form;

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

        public ListConnaissementForm(VehiculeForm form, List<CONNAISSEMENT> listConnaissements, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

                connaissemments = listConnaissements;
                dataGrid.ItemsSource = connaissemments;

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

        public ListConnaissementForm(ConteneurForm form, List<CONNAISSEMENT> listConnaissements, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

                connaissemments = listConnaissements;
                dataGrid.ItemsSource = connaissemments;

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

        public ListConnaissementForm(ConteneurTCForm form, List<CONNAISSEMENT> listConnaissements, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

                connaissemments = listConnaissements;
                dataGrid.ItemsSource = connaissemments;

                utilisateur = user;
                operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);

                ctrTCForm = form;

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

        public ListConnaissementForm(MafiForm form, List<CONNAISSEMENT> listConnaissements, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

                connaissemments = listConnaissements;
                dataGrid.ItemsSource = connaissemments;

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

        public ListConnaissementForm(ConventionnelForm form, List<CONNAISSEMENT> listConnaissements, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

                connaissemments = listConnaissements;
                dataGrid.ItemsSource = connaissemments;

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

        public ListConnaissementForm(OrdreServiceForm form, List<CONNAISSEMENT> listConnaissements, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

                connaissemments = listConnaissements;
                dataGrid.ItemsSource = connaissemments;

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

        public ListConnaissementForm(DemandeLivraisonForm form, List<CONNAISSEMENT> listConnaissements, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

                connaissemments = listConnaissements;
                dataGrid.ItemsSource = connaissemments;

                utilisateur = user;
                operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);

                livForm = form;

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

        public ListConnaissementForm(DemandeRestitutionCautionForm form, List<CONNAISSEMENT> listConnaissements, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

                connaissemments = listConnaissements;
                dataGrid.ItemsSource = connaissemments;

                utilisateur = user;
                operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);

                cautionForm = form;

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

        public ListConnaissementForm(BonSortieForm form, List<CONNAISSEMENT> listConnaissements, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

                connaissemments = listConnaissements;
                dataGrid.ItemsSource = connaissemments;

                utilisateur = user;
                operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);

                sortieForm = form;

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

        public ListConnaissementForm(DemandeVisiteForm form, List<CONNAISSEMENT> listConnaissements, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

                connaissemments = listConnaissements;
                dataGrid.ItemsSource = connaissemments;

                utilisateur = user;
                operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);

                visiteForm = form;

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

        public ListConnaissementForm(ProformaForm form, List<CONNAISSEMENT> listConnaissements, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

                connaissemments = listConnaissements;
                dataGrid.ItemsSource = connaissemments;

                utilisateur = user;
                operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);

                profForm = form;

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

        public ListConnaissementForm(PaiementForm form, List<CONNAISSEMENT> listConnaissements, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

                connaissemments = listConnaissements;
                dataGrid.ItemsSource = connaissemments;

                utilisateur = user;
                operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);

                payForm = form;

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

        public ListConnaissementForm(DemandeReductionForm form, List<CONNAISSEMENT> listConnaissements, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

                connaissemments = listConnaissements;
                dataGrid.ItemsSource = connaissemments;

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

        public ListConnaissementForm(ExtensionFranchiseForm form, List<CONNAISSEMENT> listConnaissements, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

                connaissemments = listConnaissements;
                dataGrid.ItemsSource = connaissemments;

                utilisateur = user;
                operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);

                extForm = form;

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
                    if (conForm != null)
                    {
                        formLoader.LoadConnaissementForm(conForm, (CONNAISSEMENT)dataGrid.SelectedItem);
                        this.Close();
                    }
                    else if (baeForm != null)
                    {
                        formLoader.LoadConnaissementForm(baeForm, (CONNAISSEMENT)dataGrid.SelectedItem);
                        this.Close();
                    }
                    else if (sortieForm != null)
                    {
                        formLoader.LoadConnaissementForm(sortieForm, (CONNAISSEMENT)dataGrid.SelectedItem);
                        this.Close();
                    }
                    else if (livForm != null)
                    {
                        formLoader.LoadConnaissementForm(livForm, (CONNAISSEMENT)dataGrid.SelectedItem, 0);
                        this.Close();
                    }
                    else if (osForm != null)
                    {
                        formLoader.LoadConnaissementForm(osForm, (CONNAISSEMENT)dataGrid.SelectedItem);
                        this.Close();
                    }
                    else if (profForm != null)
                    {
                        formLoader.LoadConnaissementForm(profForm, (CONNAISSEMENT)dataGrid.SelectedItem);
                        this.Close();
                    }
                    else if (reducForm != null)
                    {
                        formLoader.LoadConnaissementForm(reducForm, (CONNAISSEMENT)dataGrid.SelectedItem);
                        this.Close();
                    }
                    else if (visiteForm != null)
                    {
                        formLoader.LoadConnaissementForm(visiteForm, (CONNAISSEMENT)dataGrid.SelectedItem, 0);
                        this.Close();
                    }
                    else if (extForm != null)
                    {
                        formLoader.LoadConnaissementForm(extForm, (CONNAISSEMENT)dataGrid.SelectedItem);
                        this.Close();
                    }
                    else if (payForm != null)
                    {
                        formLoader.LoadConnaissementForm(payForm, (CONNAISSEMENT)dataGrid.SelectedItem);
                        this.Close();
                    }
                    else if (factDITForm != null)
                    {
                        formLoader.LoadConnaissementForm(factDITForm, (CONNAISSEMENT)dataGrid.SelectedItem);
                        this.Close();
                    }
                    else if (vehForm != null)
                    {
                        formLoader.LoadConnaissementForm(vehForm, (CONNAISSEMENT)dataGrid.SelectedItem);
                        this.Close();
                    }
                    else if (ctrForm != null)
                    {
                        formLoader.LoadConnaissementForm(ctrForm, (CONNAISSEMENT)dataGrid.SelectedItem);
                        this.Close();
                    }
                    else if (ctrTCForm != null)
                    {
                        formLoader.LoadConnaissementForm(ctrTCForm, (CONNAISSEMENT)dataGrid.SelectedItem);
                        this.Close();
                    }
                    else if (mafiForm != null)
                    {
                        formLoader.LoadConnaissementForm(mafiForm, (CONNAISSEMENT)dataGrid.SelectedItem);
                        this.Close();
                    }
                    else if (convForm != null)
                    {
                        formLoader.LoadConnaissementForm(convForm, (CONNAISSEMENT)dataGrid.SelectedItem);
                        this.Close();
                    }
                    else if (cautionForm != null)
                    {
                        formLoader.LoadConnaissementForm(cautionForm, (CONNAISSEMENT)dataGrid.SelectedItem, 0);
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
                //MessageBox.Show(ex.Message + "\n" + ex.StackTrace, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnSelectionner_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dataGrid.SelectedIndex != -1 && dataGrid.SelectedItems.Count == 1)
                {
                    if (conForm != null)
                    {
                        formLoader.LoadConnaissementForm(conForm, (CONNAISSEMENT)dataGrid.SelectedItem);
                        this.Close();
                    }
                    else if (baeForm != null)
                    {
                        formLoader.LoadConnaissementForm(baeForm, (CONNAISSEMENT)dataGrid.SelectedItem);
                        this.Close();
                    }
                    else if (osForm != null)
                    {
                        formLoader.LoadConnaissementForm(osForm, (CONNAISSEMENT)dataGrid.SelectedItem);
                        this.Close();
                    }
                    else if (sortieForm != null)
                    {
                        formLoader.LoadConnaissementForm(sortieForm, (CONNAISSEMENT)dataGrid.SelectedItem);
                        this.Close();
                    }
                    else if (livForm != null)
                    {
                        formLoader.LoadConnaissementForm(livForm, (CONNAISSEMENT)dataGrid.SelectedItem, 0);
                        //livForm._con = (CONNAISSEMENT)dataGrid.SelectedItem;
                        this.Close();
                    }
                    else if (profForm != null)
                    {
                        formLoader.LoadConnaissementForm(profForm, (CONNAISSEMENT)dataGrid.SelectedItem);
                        this.Close();
                    }
                    else if (reducForm != null)
                    {
                        formLoader.LoadConnaissementForm(reducForm, (CONNAISSEMENT)dataGrid.SelectedItem);
                        this.Close();
                    }
                    else if (visiteForm != null)
                    {
                        formLoader.LoadConnaissementForm(visiteForm, (CONNAISSEMENT)dataGrid.SelectedItem, 0);
                        this.Close();
                    }
                    else if (extForm != null)
                    {
                        formLoader.LoadConnaissementForm(extForm, (CONNAISSEMENT)dataGrid.SelectedItem);
                        this.Close();
                    }
                    else if (payForm != null)
                    {
                        formLoader.LoadConnaissementForm(payForm, (CONNAISSEMENT)dataGrid.SelectedItem);
                        this.Close();
                    }
                    else if (factDITForm != null)
                    {
                        formLoader.LoadConnaissementForm(factDITForm, (CONNAISSEMENT)dataGrid.SelectedItem);
                        this.Close();
                    }
                    else if (vehForm != null)
                    {
                        formLoader.LoadConnaissementForm(vehForm, (CONNAISSEMENT)dataGrid.SelectedItem);
                        this.Close();
                    }
                    else if (ctrForm != null)
                    {
                        formLoader.LoadConnaissementForm(ctrForm, (CONNAISSEMENT)dataGrid.SelectedItem);
                        this.Close();
                    }
                    else if (ctrTCForm != null)
                    {
                        formLoader.LoadConnaissementForm(ctrTCForm, (CONNAISSEMENT)dataGrid.SelectedItem);
                        this.Close();
                    }
                    else if (mafiForm != null)
                    {
                        formLoader.LoadConnaissementForm(mafiForm, (CONNAISSEMENT)dataGrid.SelectedItem);
                        this.Close();
                    }
                    else if (convForm != null)
                    {
                        formLoader.LoadConnaissementForm(convForm, (CONNAISSEMENT)dataGrid.SelectedItem);
                        this.Close();
                    }
                    else if (cautionForm != null)
                    {
                        formLoader.LoadConnaissementForm(cautionForm, (CONNAISSEMENT)dataGrid.SelectedItem, 0);
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
