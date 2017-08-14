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

namespace VesselStopOverPresentation.Finance
{
    /// <summary>
    /// Logique d'interaction pour AvoirCustom.xaml
    /// </summary>
    public partial class AvoirCustom : Window
    {
        private List<CLIENT> clients;
        public List<string> clts { get; set; }
        private List<OPERATION> operationsUser;
        private UTILISATEUR utilisateur;

        public AvoirPanel avoirPanel { get; set; }
        public List<ElementLigneFactureSpot> eltsFact { get; set; }
        private FormLoader formLoader; 
        private VSOMAccessors vsomAcc;
       public AVOIR _avoir;
        public AvoirCustom(AvoirPanel panel , AVOIR avoir , UTILISATEUR user)
        {
            try
            {
                InitializeComponent();
                vsomAcc = new VSOMAccessors();
                clients = vsomAcc.GetClientsActifs();
                clts = new List<string>();
                foreach (CLIENT clt in clients)
                {
                    clts.Add(clt.NomClient);
                }

                utilisateur = user;
                operationsUser = vsomAcc.GetOperationsUtilisateur(utilisateur.IdU);

                formLoader = new FormLoader(utilisateur);
                _avoir = avoir;
                formLoader.LoadAvoirForm(this, avoir);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnImprimer_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                AvoirReport avoirReport = new AvoirReport(this);
                avoirReport.Title = "Impression de la facture avoir : " + cbIdAvoir.Text;
                avoirReport.Show();
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void cbClient_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
