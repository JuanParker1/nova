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
    /// Logique d'interaction pour ConnaissementPanel.xaml
    /// </summary>
    public partial class ConnaissementPanel : DockPanel
    {
        public List<CONNAISSEMENT> connaissements { get; set; }
        private List<OPERATION> operationsUser;
        private UTILISATEUR utilisateur;
        private VsomParameters vsp = new VsomParameters();

        public ConnaissementPanel(UTILISATEUR user)
        {
            try
            {

                InitializeComponent();
                //using (var ctx = new VSOMClassesDataContext())
                //{
                    VSOMAccessors vsomAcc = new VSOMAccessors();

                    this.DataContext = this;
                    listRechercher.SelectedIndex = 0;
                    //cbFiltres.SelectedIndex = 0;
                    //lblStatut.Content = connaissements.Count + " Connaissement(s)";

                    utilisateur = user;
                    operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);
                    //vsomAcc.CorrectionStatut();
                    //vsomAcc.CorrectionGesparc();
                    //vsomAcc.CorrigerReceptionVehiculeGesparc();
                    //vsomAcc.CorrectionOperationsArmateur();
                    //vsomAcc.CorrectionFactures();
                    //vsomAcc.Correction();
                    //vsomAcc.CorrectionSAP();
                    //vsomAcc.CorrectionRefPaiement();
                    //vsomAcc.CorrectionAgencyFees();
                    //vsomAcc.CorrectionCompteComptables();
                    //vsomAcc.CorrectionVolBL();
                    //vsomAcc.CorrectionISPS();
                    //vsomAcc.CorrectionCompteEscale();
                    //vsomAcc.CorrectionSummaryNDS();
                    //vsomAcc.CorrectionReportingNDS();
                    //vsomAcc.CorrectionSummaryKARIM();
                    //vsomAcc.CorrectionInterchange();
                    //vsomAcc.CorrectionGestionnaireParcAuto();
                //}
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
            }
            
        }

        private void btnNouveau_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (operationsUser.Where(op => op.NomOp == "Connaissement : Modification des informations de base").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
                {
                    MessageBox.Show("Vous n'avez pas les droits nécessaires pour créer un nouveau connaissement. Veuillez contacter un administrateur", "Droits insuffisants !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    ConnaissementForm conForm = new ConnaissementForm("Nouveau", this, utilisateur);
                    conForm.cbNumBL.IsEditable = true;
                    conForm.cbEscale.IsEditable = true;
                    conForm.txtPortProv.IsReadOnly = false;
                    conForm.txtPortProv.Background = Brushes.White;
                    conForm.groupInfosChargeur.IsEnabled = false;
                    conForm.Title = "Nouveau : Connaissement";
                    conForm.actionsBorder.Visibility = System.Windows.Visibility.Collapsed;
                    conForm.Show();
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

        private void dataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                //using (var ctx = new VSOMClassesDataContext())
                //{
                    //VSOMAccessors vs = new VSOMAccessors(ctx);
                    VSOMAccessors vsomAcc = new VSOMAccessors();

                    if (dataGrid.SelectedIndex != -1)
                    {
                        CONNAISSEMENT con = vsp.GetConnaissementByIdBL(((CONNAISSEMENT)dataGrid.SelectedItem).IdBL);
                        if (con.StatutBL == "Del")
                        {
                            throw new ApplicationException("Ce connaissement n'est pas disponible pour traitement");
                        }
                        ConnaissementForm conForm = new ConnaissementForm(this, con, utilisateur);
                        conForm.Show();
                    }
                //}
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
            
        }

        private void btnAnnulerRecherche_Click(object sender, RoutedEventArgs e)
        {
            txtRechercher.Text = null;
        }

        private void txtRechercher_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
               // using (var ctx = new VSOMClassesDataContext())
               // {
                   // VSOMAccessors vs = new VSOMAccessors(ctx);
                    VSOMAccessors vsomAcc = new VSOMAccessors();

                    if (e.Key == Key.Return && listRechercher.SelectedItem != null)
                    {
                        if (listRechercher.SelectedIndex == 0)
                        {
                            if (cbFiltres.SelectedIndex == 0 || cbFiltres.SelectedIndex == -1)
                            {
                                connaissements = vsp.GetConnaissementByNumBLAll(txtRechercher.Text.Trim());
                            }
                            else if (cbFiltres.SelectedIndex == 1)
                            {
                                connaissements = vsp.GetConnaissementByNumBLAll(txtRechercher.Text.Trim(), "Non initié");
                            }
                            else if (cbFiltres.SelectedIndex == 2)
                            {
                                connaissements = vsp.GetConnaissementByNumBLAll(txtRechercher.Text.Trim(), "Initié");
                            }
                            else if (cbFiltres.SelectedIndex == 3)
                            {
                                connaissements = vsp.GetConnaissementByNumBLAll(txtRechercher.Text.Trim(), "Traité");
                            }
                            else if (cbFiltres.SelectedIndex == 4)
                            {
                                connaissements = vsp.GetConnaissementByNumBLAll(txtRechercher.Text.Trim(), "Manifesté");
                            }
                            else if (cbFiltres.SelectedIndex == 5)
                            {
                                connaissements = vsp.GetConnaissementsAFacturer(txtRechercher.Text.Trim());
                            }
                            else if (cbFiltres.SelectedIndex == 6)
                            {
                                connaissements = vsp.GetConnaissementPourEnlevement(txtRechercher.Text.Trim());
                            }
                            else if (cbFiltres.SelectedIndex == 7)
                            {
                                connaissements = vsp.GetConnaissementPourLivraison(txtRechercher.Text.Trim());
                            }
                            else if (cbFiltres.SelectedIndex == 8)
                            {
                                connaissements = vsp.GetConnaissementPourSortie(txtRechercher.Text.Trim());
                            }
                            else if (cbFiltres.SelectedIndex == 9)
                            {
                                connaissements = vsp.GetConnaissementByNumBLAll(txtRechercher.Text.Trim(), "Accompli");
                            }
                            else if (cbFiltres.SelectedIndex == 10)
                            {
                                connaissements = vsp.GetConnaissementByNumBLAll(txtRechercher.Text.Trim(), "Cloturé");
                            }
                            else if (cbFiltres.SelectedIndex == 11)
                            {
                                connaissements = vsp.GetConnaissementSOCARByNumBL(txtRechercher.Text.Trim());
                            }
                            dataGrid.ItemsSource = connaissements;
                            lblStatut.Content = connaissements.Count + " Connaissement(s) trouvé(s)";
                        }
                        else if (listRechercher.SelectedIndex == 1)
                        {
                            int result;
                            if (cbFiltres.SelectedIndex == 0 || cbFiltres.SelectedIndex == -1)
                            {
                                connaissements = vsp.GetConnaissementByNumEscale(Int32.TryParse(txtRechercher.Text.Trim(), out result) ? result : -1);
                            }
                            else if (cbFiltres.SelectedIndex == 1)
                            {
                                connaissements = vsp.GetConnaissementByNumEscale(Int32.TryParse(txtRechercher.Text.Trim(), out result) ? result : -1, "Non initié");
                            }
                            else if (cbFiltres.SelectedIndex == 2)
                            {
                                connaissements = vsp.GetConnaissementByNumEscale(Int32.TryParse(txtRechercher.Text.Trim(), out result) ? result : -1, "Initié");
                            }
                            else if (cbFiltres.SelectedIndex == 3)
                            {
                                connaissements = vsp.GetConnaissementByNumEscale(Int32.TryParse(txtRechercher.Text.Trim(), out result) ? result : -1, "Traité");
                            }
                            else if (cbFiltres.SelectedIndex == 4)
                            {
                                connaissements = vsp.GetConnaissementByNumEscale(Int32.TryParse(txtRechercher.Text.Trim(), out result) ? result : -1, "Manifesté");
                            }
                            else if (cbFiltres.SelectedIndex == 5)
                            {
                                connaissements = vsp.GetConnaissementsAFacturerByEscale(Int32.TryParse(txtRechercher.Text.Trim(), out result) ? result : -1);
                            }
                            else if (cbFiltres.SelectedIndex == 6)
                            {
                                connaissements = vsp.GetConnaissementPourEnlevementByEscale(Int32.TryParse(txtRechercher.Text.Trim(), out result) ? result : -1);
                            }
                            else if (cbFiltres.SelectedIndex == 7)
                            {
                                connaissements = vsp.GetConnaissementPourLivraisonByEscale(Int32.TryParse(txtRechercher.Text.Trim(), out result) ? result : -1);
                            }
                            else if (cbFiltres.SelectedIndex == 8)
                            {
                                connaissements = vsp.GetConnaissementPourSortieByEscale(Int32.TryParse(txtRechercher.Text.Trim(), out result) ? result : -1);
                            }
                            else if (cbFiltres.SelectedIndex == 9)
                            {
                                connaissements = vsp.GetConnaissementByNumEscale(Int32.TryParse(txtRechercher.Text.Trim(), out result) ? result : -1, "Accompli");
                            }
                            else if (cbFiltres.SelectedIndex == 10)
                            {
                                connaissements = vsp.GetConnaissementByNumEscale(Int32.TryParse(txtRechercher.Text.Trim(), out result) ? result : -1, "Cloturé");
                            }
                            else if (cbFiltres.SelectedIndex == 11)
                            {
                                connaissements = vsp.GetConnaissementSOCARByNumEscale(Int32.TryParse(txtRechercher.Text.Trim(), out result) ? result : -1);
                            }
                            dataGrid.ItemsSource = connaissements;
                            lblStatut.Content = connaissements.Count + " Connaissement(s) trouvé(s)";
                        }
                        else if (listRechercher.SelectedIndex == 2)
                        {
                            if (cbFiltres.SelectedIndex == 0 || cbFiltres.SelectedIndex == -1)
                            {
                                connaissements = vsp.GetConnaissementByConsignee(txtRechercher.Text.Trim());
                            }
                            else if (cbFiltres.SelectedIndex == 1)
                            {
                                connaissements = vsp.GetConnaissementByConsignee(txtRechercher.Text.Trim(), "Non initié");
                            }
                            else if (cbFiltres.SelectedIndex == 2)
                            {
                                connaissements = vsp.GetConnaissementByConsignee(txtRechercher.Text.Trim(), "Initié");
                            }
                            else if (cbFiltres.SelectedIndex == 3)
                            {
                                connaissements = vsp.GetConnaissementByConsignee(txtRechercher.Text.Trim(), "Traité");
                            }
                            else if (cbFiltres.SelectedIndex == 4)
                            {
                                connaissements = vsp.GetConnaissementByConsignee(txtRechercher.Text.Trim(), "Manifesté");
                            }
                            else if (cbFiltres.SelectedIndex == 5)
                            {
                                connaissements = vsp.GetConnaissementsAFacturerByConsignee(txtRechercher.Text.Trim());
                            }
                            else if (cbFiltres.SelectedIndex == 6)
                            {
                                connaissements = vsp.GetConnaissementPourEnlevementByConsignee(txtRechercher.Text.Trim());
                            }
                            else if (cbFiltres.SelectedIndex == 7)
                            {
                                connaissements = vsp.GetConnaissementPourLivraisonByConsignee(txtRechercher.Text.Trim());
                            }
                            else if (cbFiltres.SelectedIndex == 8)
                            {
                                connaissements = vsp.GetConnaissementPourSortieByConsignee(txtRechercher.Text.Trim());
                            }
                            else if (cbFiltres.SelectedIndex == 9)
                            {
                                connaissements = vsp.GetConnaissementByConsignee(txtRechercher.Text.Trim(), "Accompli");
                            }
                            else if (cbFiltres.SelectedIndex == 10)
                            {
                                connaissements = vsp.GetConnaissementByConsignee(txtRechercher.Text.Trim(), "Cloturé");
                            }
                            else if (cbFiltres.SelectedIndex == 11)
                            {
                                connaissements = vsp.GetConnaissementSOCARByConsignee(txtRechercher.Text.Trim());
                            }
                            dataGrid.ItemsSource = connaissements;
                            lblStatut.Content = connaissements.Count + " Connaissement(s) trouvé(s)";
                        }
                    }
                    else if (e.Key == Key.Escape)
                    {
                        txtRechercher.Text = null;
                    }
                //}
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
               // using (var ctx = new VSOMClassesDataContext())
               // {
                    //VSOMAccessors vs = new VSOMAccessors(ctx);
                    VSOMAccessors vsomAcc = new VSOMAccessors();

                    if (cbFiltres.SelectedIndex == 0)
                    {
                        connaissements = vsp.GetConnaissementsImport();
                        dataGrid.ItemsSource = connaissements;
                        lblStatut.Content = connaissements.Count + " Connaissement(s)";
                    }
                    else if (cbFiltres.SelectedIndex == 1)
                    {
                        connaissements = vsp.GetConnaissementByStatut("Non initié");
                        dataGrid.ItemsSource = connaissements;
                        lblStatut.Content = connaissements.Count + " Connaissement(s)";
                    }
                    else if (cbFiltres.SelectedIndex == 2)
                    {
                        connaissements = vsp.GetConnaissementByStatut("Initié");
                        dataGrid.ItemsSource = connaissements;
                        lblStatut.Content = connaissements.Count + " Connaissement(s)";
                    }
                    else if (cbFiltres.SelectedIndex == 3)
                    {
                        connaissements = vsp.GetConnaissementByStatut("Traité");
                        dataGrid.ItemsSource = connaissements;
                        lblStatut.Content = connaissements.Count + " Connaissement(s)";
                    }
                    else if (cbFiltres.SelectedIndex == 4)
                    {
                        connaissements = vsp.GetConnaissementByStatut("Manifesté");
                        dataGrid.ItemsSource = connaissements;
                        lblStatut.Content = connaissements.Count + " Connaissement(s)";
                    }
                    else if (cbFiltres.SelectedIndex == 5)
                    {
                        connaissements = vsp.GetConnaissementsAFacturer();
                        dataGrid.ItemsSource = connaissements;
                        lblStatut.Content = connaissements.Count + " Connaissement(s)";
                    }
                    else if (cbFiltres.SelectedIndex == 6)
                    {
                        connaissements = vsp.GetConnaissementPourEnlevement();
                        dataGrid.ItemsSource = connaissements;
                        lblStatut.Content = connaissements.Count + " Connaissement(s)";
                    }
                    else if (cbFiltres.SelectedIndex == 7)
                    {
                        connaissements = vsp.GetConnaissementPourLivraison();
                        dataGrid.ItemsSource = connaissements;
                        lblStatut.Content = connaissements.Count + " Connaissement(s)";
                    }
                    else if (cbFiltres.SelectedIndex == 8)
                    {
                        connaissements = vsp.GetConnaissementPourSortie();
                        dataGrid.ItemsSource = connaissements;
                        lblStatut.Content = connaissements.Count + " Connaissement(s)";
                    }
                    else if (cbFiltres.SelectedIndex == 9)
                    {
                        connaissements = vsp.GetConnaissementByStatut("Accompli");
                        dataGrid.ItemsSource = connaissements;
                        lblStatut.Content = connaissements.Count + " Connaissement(s)";
                    }
                    else if (cbFiltres.SelectedIndex == 10)
                    {
                        connaissements = vsp.GetConnaissementByStatut("Cloturé");
                        dataGrid.ItemsSource = connaissements;
                        lblStatut.Content = connaissements.Count + " Connaissement(s)";
                    }
               // }
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
