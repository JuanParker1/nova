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
    /// Logique d'interaction pour SortirMafiForm.xaml
    /// </summary>
    public partial class SortirMafiForm : Window
    {
        private MafiForm mafiForm;
        private MafiPanel mafiPanel;
        private MAFI mafi;
        private List<OPERATION> operationsUser;
        private UTILISATEUR utilisateur;

        private FormLoader formLoader;
        private VsomParameters vsp = new VsomParameters();
        public SortirMafiForm(MafiForm form, UTILISATEUR user)
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

                //if (utilisateur.LU != "Admin")
                //{
                //    txtDateSortie.IsEnabled = false;
                //}

                if (operationsUser.Where(op => op.NomOp == "Field : Autorisation sur date").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
                {
                    txtDateSortie.IsEnabled = false;
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

        public SortirMafiForm(MafiPanel panel, MAFI mf, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

                mafi = mf;

                utilisateur = user;
                operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);

                mafiPanel = panel;

                formLoader = new FormLoader(utilisateur);

                //if (utilisateur.LU != "Admin")
                //{
                //    txtDateSortie.IsEnabled = false;
                //}

                if (operationsUser.Where(op => op.NomOp == "Field : Autorisation sur date").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
                {
                    txtDateSortie.IsEnabled = false;
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
                //VSOMAccessors vsomAcc = new VSOMAccessors();
                VsomMarchal vsomAcc = new VsomMarchal();

                if (!txtDateSortie.SelectedDate.HasValue)
                {
                    MessageBox.Show("Veuillez entrer une date de sortie", "Echec de l'opération", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    MAFI mf = null;
                    if (mafiForm != null)
                    {
                        mf = vsomAcc.SortirMafi(Convert.ToInt32(mafiForm.txtIdMafi.Text), txtDateSortie.SelectedDate.Value, new TextRange(txtObservations.Document.ContentStart, txtObservations.Document.ContentEnd).Text, utilisateur.IdU);
                        //Raffraîchir les informations

                        formLoader.LoadMafiForm(mafiForm, mf);
                    }
                    else if (mafiPanel != null)
                    {
                        mf = vsomAcc.SortirMafi(((CONTENEUR)mafiPanel.dataGrid.SelectedItem).IdCtr, txtDateSortie.SelectedDate.Value, new TextRange(txtObservations.Document.ContentStart, txtObservations.Document.ContentEnd).Text, utilisateur.IdU);
                        //Raffraîchir les informations
                        //mafiPanel.dataGrid.ItemsSource = vsomAcc.GetConteneursImport();
                    }

                    MessageBox.Show("L'opération de sortie de mafi s'est déroulée avec succès, consultez le journal des éléments de facturation", "Conteneur identifié !", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (HabilitationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (IdentificationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération", MessageBoxButton.OK, MessageBoxImage.Error);
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
