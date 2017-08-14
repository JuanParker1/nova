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
    /// Logique d'interaction pour MAJDateDepotForm.xaml
    /// </summary>
    public partial class MAJDateDepotForm : Window
    {

        private DemandeLivraisonForm livForm;
        private List<OPERATION> operationsUser;
        private UTILISATEUR utilisateur;

        private FormLoader formLoader;
        private VsomParameters vsp = new VsomParameters();
        public MAJDateDepotForm(DemandeLivraisonForm form, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

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

        private void btnMAJDateDepot_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //VSOMAccessors vsomAcc = new VSOMAccessors();
                VsomMarchal vsomAcc = new VsomMarchal();

                if (!txtDateDepot.SelectedDate.HasValue)
                {
                    MessageBox.Show("Veuillez entrer une date de depôt du dossier physique", "Date de dépôt ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    DEMANDE_LIVRAISON demandeLivraison = vsomAcc.MAJDateDepotLivraison(Convert.ToInt32(livForm.cbIdDL.Text), txtDateDepot.SelectedDate.Value, utilisateur.IdU);

                    formLoader.LoadDemandeLivraisonForm(livForm, demandeLivraison);

                    MessageBox.Show("Date de dépôt mise à jour avec succès.", "Date de dépôt mise à jour !", MessageBoxButton.OK, MessageBoxImage.Information);

                    this.Close();
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
