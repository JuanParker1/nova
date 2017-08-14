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
namespace VesselStopOverPresentation.conteneur
{
    /// <summary>
    /// Logique d'interaction pour _2015UpdateOperationTracking.xaml
    /// </summary>
    public partial class _2015UpdateOperationTracking : Window
    {
        private ConteneurTCForm ctrTCForm;

        private List<MOUVEMENT_TC> operations;
        public List<string> ops { get; set; }

        private List<UTILISATEUR> utilisateurs;
        public List<string> users { get; set; }

        public List<PARC> parcs { get; set; }
        public List<string> prcs { get; set; }

        private List<EMPLACEMENT> emplacements;
        public List<string> empls { get; set; }

        private UTILISATEUR utilisateur;
        private List<OPERATION> operationsUser;

        private FormLoader formLoader;
        //private VsomParameters vsomAcc = new VsomParameters();
        private VSOMAccessors vsomAcc;

        public _2015UpdateOperationTracking(ConteneurTCForm form, int idCtr, UTILISATEUR user)
        {
            try
            {
                vsomAcc = new VSOMAccessors();
                //VsomParameters vsomAcc = new VsomParameters();
                InitializeComponent();
                this.DataContext = this;

                operations = vsomAcc.GetConteneurByIdCtr(idCtr).CONTENEUR_TC.FirstOrDefault<CONTENEUR_TC>().MOUVEMENT_TC.ToList<MOUVEMENT_TC>();
                ops = new List<string>();
                foreach (MOUVEMENT_TC mvt in operations)
                {
                    ops.Add(mvt.TYPE_OPERATION.LibTypeOp);
                }

                utilisateurs = vsomAcc.GetUtilisateurs();
                users = new List<string>();
                foreach (UTILISATEUR usr in utilisateurs)
                {
                    users.Add(usr.NU);
                }

                parcs = vsomAcc.GetParcs("C");
                prcs = new List<string>();
                foreach (PARC p in parcs)
                {
                    prcs.Add(p.NomParc);
                }

                ctrTCForm = form;

                utilisateur = user;
                operationsUser = vsomAcc.GetOperationsUtilisateur(utilisateur.IdU);

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

        private void btnEnregistrer_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                vsomAcc = new VSOMAccessors();
                //VsomMarchal vsomAcc = new VsomMarchal();

                if (cbOperation.SelectedIndex == -1)
                {
                    MessageBox.Show("Vous devez saisir sélectionner un élément de facturation", "Elément de facturation ?", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (!txtDate.SelectedDate.HasValue)
                {
                    MessageBox.Show("Vous devez saisir la date de l'operation", "Date ?", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (cbUtilisateur.SelectedIndex == -1)
                {
                    MessageBox.Show("Vous devez sélectionner l'utilisateur qui a effectué l'operation", "Utilisateur ?", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (cbParc.SelectedIndex == -1)
                {
                    MessageBox.Show("Vous devez sélectionner le parc dans lequel a été effectué l'operation", "Parc ?", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                //else if (cbStatut.SelectedIndex == -1)
                //{
                //    MessageBox.Show("Vous devez sélectionner un statut", "Statut ?", MessageBoxButton.OK, MessageBoxImage.Information);
                //}
                else
                {
                    MOUVEMENT_TC mvt = vsomAcc._2015UpdateOperationTracking(operations.ElementAt<MOUVEMENT_TC>(cbOperation.SelectedIndex).IdMvt, txtDate.SelectedDate.Value,utilisateur.IdU , parcs.ElementAt<PARC>(cbParc.SelectedIndex).IdParc);

                    operations = mvt.CONTENEUR_TC.MOUVEMENT_TC.ToList<MOUVEMENT_TC>();
                    ops = new List<string>();
                    foreach (MOUVEMENT_TC mv in operations)
                    {
                        ops.Add(mv.TYPE_OPERATION.LibTypeOp);
                    }
                    cbOperation.ItemsSource = null;
                    cbOperation.ItemsSource = ops;
                    cbOperation.SelectedItem = mvt.TYPE_OPERATION.LibTypeOp;

                    formLoader.LoadConteneurTCForm(ctrTCForm, mvt.CONTENEUR_TC);

                    MessageBox.Show("Operation mise à jour avec succès", "Operation mise à jour !", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (EnregistrementInexistant ei)
            {
                MessageBox.Show(ei.Message, "Enregistrement inexistant", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void cbParc_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {

        }

        private void cbOperation_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (cbOperation.SelectedIndex != -1)
                {
                    int indexElt = cbOperation.SelectedIndex;
                    MOUVEMENT_TC elt = operations.ElementAt<MOUVEMENT_TC>(indexElt);
                    txtDate.SelectedDate = elt.DateMvt;
                    cbUtilisateur.SelectedItem = elt.UTILISATEUR.NU;
                    cbParc.SelectedItem = elt.PARC.NomParc;
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
