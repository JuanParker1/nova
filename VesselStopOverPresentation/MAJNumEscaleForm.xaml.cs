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
    /// Logique d'interaction pour MAJNumEscaleForm.xaml
    /// </summary>
    public partial class MAJNumEscaleForm : Window
    {

        private EscaleForm escForm;
        private List<OPERATION> operationsUser;
        private UTILISATEUR utilisateur;

        private ESCALE escale;

        private FormLoader formLoader;
        private VsomParameters vsp = new VsomParameters();
        public MAJNumEscaleForm(EscaleForm form, ESCALE esc, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

                utilisateur = user;
                operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);

                escForm = form;

                escale = esc;

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

        private void btnMAJ_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //VSOMAccessors vsomAcc = new VSOMAccessors();
                VsomMarchal vsomAcc = new VsomMarchal();

                if (txtNouveauNumEscale.Text.Trim() == "")
                {
                    MessageBox.Show("Veuillez saisir le nouveau numéro d'escale pour cette escale", "Nouveau N° escale ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    ESCALE esc = vsomAcc.UpdateNumEscale(escale.IdEsc, Convert.ToInt32(txtNouveauNumEscale.Text), utilisateur.IdU);

                    formLoader.LoadEscaleForm(escForm, esc);

                    MessageBox.Show("Le N° d'escale a été mis à jour avec succès", "N° d'escale mis à jour !", MessageBoxButton.OK, MessageBoxImage.Information);

                    this.Close();
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

            }
        }
    }
}
