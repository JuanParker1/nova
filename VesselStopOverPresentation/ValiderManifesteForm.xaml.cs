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
    /// Logique d'interaction pour ValiderManifesteForm.xaml
    /// </summary>
    public partial class ValiderManifesteForm : Window
    {

        private ManifesteForm manForm;
        private List<OPERATION> operationsUser;
        private UTILISATEUR utilisateur;

        private FormLoader formLoader;
       // private VsomParameters vsp = new VsomParameters();
        private VSOMAccessors vsomAcc;
        public ValiderManifesteForm(ManifesteForm form, UTILISATEUR user)
        {
            try
            {

                InitializeComponent();
               // using (var ctx = new VSOMClassesDataContext())
               // {
                    vsomAcc = new VSOMAccessors();

                    this.DataContext = this;

                    utilisateur = user;
                    operationsUser = vsomAcc.GetOperationsUtilisateur(utilisateur.IdU);

                    manForm = form;

                    formLoader = new FormLoader(utilisateur);
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

        private void btnValider_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //using (var ctx = new VSOMClassesDataContext())
                //{
                   vsomAcc = new VSOMAccessors();
                    MANIFESTE man = vsomAcc.ValiderManifeste(vsomAcc.GetManifesteByIdMan(Convert.ToInt32(manForm.cbIdMan.Text)).IdMan, new TextRange(txtObservations.Document.ContentStart, txtObservations.Document.ContentEnd).Text, utilisateur.IdU);

                    //Raffraîchir les informations
                    formLoader.LoadManifesteForm(manForm, man);
                //}
                MessageBox.Show("Manifeste validé : Les éléments de facturation ont été générés avec succès", "Connaissement validé !", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            catch (HabilitationException ex)
            {
                MessageBox.Show(ex.Message, "Droits insuffisants !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            catch (ManifesteException ex)
            {
                MessageBox.Show(ex.Message, "Manifeste validé !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n Détail :" + ex.StackTrace, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
