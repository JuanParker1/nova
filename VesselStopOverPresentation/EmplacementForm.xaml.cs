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
    /// Logique d'interaction pour EmplacementForm.xaml
    /// </summary>
    public partial class EmplacementForm : Window
    {
        private List<PARC> parcs;
        private UTILISATEUR utilisateur;
        private EmplacementPanel emplanel;
        private List<OPERATION> operationsUser;

        public EmplacementForm(string type,EmplacementPanel panel , UTILISATEUR user)
        {
            VSOMAccessors vsomAcc = new VSOMAccessors();

            InitializeComponent();
            this.DataContext = this;

           // parcs = vsp.GetParcs("V");
            utilisateur = user;
           // operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);
            emplanel = panel;
            txtParc.Text = "Parc principal";
            
        }

        private void btnEnregistrer_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                VsomConfig vsomAcc = new VsomConfig();

                int result;
               int index1 = Int32.TryParse(txtColDeb.Text.Trim(), out result) ? result : -1;
               int index2 = Int32.TryParse(txtColFin.Text.Trim(), out result) ? result : -1;
               if (txtLign.Text.Trim().Length==0 || index1 == -1 || index2 == -1)
               {
                   MessageBox.Show("Données  incorrectes", "Echec de l'operation", MessageBoxButton.OK, MessageBoxImage.Error);
               }
               else
               {
                  string emp= vsomAcc.InsertEmplacement(txtLign.Text.Trim(), index1, index2, utilisateur.IdU);
                   string msg=string.Empty;
                   if (emp.Length != 0) msg = string.Format("Emplacement(s) enregistré(s). \n Certains  emplacements  existaient déja : {0} \n", emp);
                   else msg = "Emplacement enregistré";

                   MessageBoxResult mbr=  MessageBox.Show(msg+" \n Enregistrer un autre emplacement?","Enregistrement Emplacement", 
                      MessageBoxButton.YesNo, MessageBoxImage.Question);
                   if (mbr == MessageBoxResult.Yes)
                   {
                       txtColDeb.Text = string.Empty; txtColFin.Text = string.Empty; txtLign.Text = string.Empty;
                       txtLign.Focus();
                   }
                   else
                   {
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
                MessageBox.Show(ex.Message, "Echec de l'opération !!!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
