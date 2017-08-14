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

namespace VesselStopOverPresentation
{
    /// <summary>
    /// Logique d'interaction pour RetourVideConteneur.xaml
    /// </summary>
    public partial class RetourVideConteneur : Window
    {
        private List<PARC> parcs;
        public List<string> prcs { get; set; }
        private ConteneurTCForm ctrTCForm;
        private List<OPERATION> operationsUser;
        private UTILISATEUR utilisateur; 
        private FormLoader formLoader;
        private VsomParameters vsp = new VsomParameters();
        public RetourVideConteneur(ConteneurTCForm form, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;
                 
                parcs = vsp.GetParcs("C");
                prcs = new List<string>();
                foreach (PARC p in parcs)
                {
                    prcs.Add(p.NomParc);
                }

                utilisateur = user;
                operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);

                ctrTCForm = form;

                formLoader = new FormLoader(utilisateur);

                //if (utilisateur.LU != "Admin")
                //{
                //    txtDateRetour.IsEnabled = false;
                //}

                if (operationsUser.Where(op => op.NomOp == "Field : Autorisation sur date").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
                {
                    txtDateRetour.IsEnabled = false;
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

        private void btnRetour_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //VSOMAccessors vsomAcc = new VSOMAccessors();
                VsomMarchal vsomAcc = new VsomMarchal();

                if (!txtDateRetour.SelectedDate.HasValue)
                {
                    MessageBox.Show("Veuillez entrer une date de retour", "Echec de l'opération", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (cbParc.SelectedIndex == -1)
                {
                    MessageBox.Show("Veuillez sélectionner le parc où est retouné ce conteneur", "Parc ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    //TODO enregistrement retour vide
                    DateTime dretour = txtDateRetour.SelectedDate.Value;
                   dretour= dretour.AddHours(11);
                   dretour = dretour.AddMinutes(59);
                  CONTENEUR_TC  ctr = vsomAcc.RetourVideConteneur(Convert.ToInt32(ctrTCForm.txtIdCtr.Text), dretour, 
                        parcs.ElementAt<PARC>(cbParc.SelectedIndex).IdParc,  new TextRange(txtObservations.Document.ContentStart, txtObservations.Document.ContentEnd).Text, utilisateur.IdU);

                  MessageBox.Show("L'opération de retour vide non empoté de conteneur s'est déroulée avec succès.", "Conteneur retourné !", MessageBoxButton.OK, MessageBoxImage.Information);
                  
                    //TODO: Raffraichissement mouvement
                  this.Close();
                  formLoader.LoadConteneurTCForm(ctrTCForm, ctr);
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
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
