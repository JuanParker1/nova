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
    /// Logique d'interaction pour AnnulerFactureSpot.xaml
    /// </summary>
    public partial class AnnulerFactureSpot : Window
    {
        FactureSpot _form;
        FACTURE _facture;
        UTILISATEUR _user;
        FormLoader fl;
        private List<OPERATION> operationsUser;
       // private VsomParameters vsp = new VsomParameters();
        private VSOMAccessors vsomAcc;
        public AnnulerFactureSpot(FactureSpot form ,FACTURE facture, UTILISATEUR user )
        {
            try
            {
                 vsomAcc = new VSOMAccessors();
                //VsomParameters vsp = new VsomParameters();

                InitializeComponent();
                _form = form;
                _facture = facture;
                _user = user;
                operationsUser = vsomAcc.GetOperationsUtilisateur(_user.IdU);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Echec de l'opération : \n " + ex.Message, "Annulation Facture Spot");
                this.Close();
            }
        }

        private void btnAnnuler_Click_1(object sender, RoutedEventArgs e)
        {
            if (operationsUser.Where(op => op.NomOp == "Facture spot: Annulation").FirstOrDefault<OPERATION>() == null && _user.LU != "Admin")
            {
                MessageBox.Show("Vous n'avez pas les droits nécessaires pour effectuer cette action. Veuillez contacter un administrateur", "Droits insuffisants !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            else  
            {
                try
                {
                    //VSOMAccessors vsomAcc = new VSOMAccessors();
                    AVOIR avoir = vsomAcc.InsertAvoirFactureSpot((int)_facture.IdDocSAP,
                                          new TextRange(txtObservations.Document.ContentStart, txtObservations.Document.ContentEnd).Text,
                                         _user.IdU);

                    FACTURE _f = vsomAcc.GetFactureSpotByIdDocSAP((int)_facture.IdDocSAP);
                    fl = new FormLoader(_user);
                    fl.LoadFactureSpotForm(_form, _f);
                    if (avoir != null)
                        MessageBox.Show("Facture Annulée : Avoir N° " + avoir.IdDocSAP);

                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Avoir Facture");
                }
            }
        }
    }
}
