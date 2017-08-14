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
    /// Logique d'interaction pour ClientCondForm.xaml
    /// </summary>
    public partial class ClientCondForm : Window
    {
        ClientCondFin ccf; 
        private VsomMarchal vsomAcc;
        public List<CLIENT> _client { get; set; }
        public ClientCondForm(UTILISATEUR user, bool read)
        {
            InitializeComponent();
            ccf = new ClientCondFin();
            this.DataContext = ccf;
            vsomAcc = new VsomMarchal();
            _client = vsomAcc.GetClientsActifs();
            cbClient.ItemsSource = null; cbClient.ItemsSource = _client;
            cbClient.DisplayMemberPath = "NomClient";
            enregistrerBorder.Visibility = read == true ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
        }

        private void btnEnregistrer_Click_1(object sender, RoutedEventArgs e)
        {
            if (cbClient.SelectedIndex == -1)
            {
                MessageBox.Show("Veuillez selectionner un client");
                return;
            }

            if (txtCautTypeImp.SelectedIndex == -1)
            {
                MessageBox.Show("Veuillez selectionner le type de caution");
                return;
            }

            //else
            //{
                try
                {
                    CLIENT cli = (CLIENT)cbClient.SelectedItem;
                    CLIENT_FIN_COND cfc_imp = new CLIENT_FIN_COND();

                    #region import
                    List<CLIENT_M_PAY> cmp_mpay_imp = new List<CLIENT_M_PAY>();

                    cfc_imp.SENS = "I";
                    cfc_imp.IDCLIENT = cli.IdClient;

                    cfc_imp.CAUTION_MT = ccf.caution_montant_import;
                    cfc_imp.CAUTION_T = txtCautTypeImp.Text; //ccf.Caution_type_import;
                    cmp_mpay_imp.Add(new CLIENT_M_PAY
                    {
                        IDCLIENT = cli.IdClient,
                        SENS = "I",
                        STATUS = "Actif",
                        TYPE = "Caut",
                        CASH = caut_cash_imp.IsChecked.Value,
                        CB = caut_cb_imp.IsChecked.Value,
                        CC = caut_cc_imp.IsChecked.Value,
                        WT = caut_vir_imp.IsChecked.Value,
                        CS = caut_cs_imp.IsChecked.Value
                    });

                    cfc_imp.CONV_MANUT = ccf.Conv_manut_import;
                    cfc_imp.CONV_OP_DIVERS = ccf.Conv_od_import;
                    cfc_imp.CONV_SEJOUR = ccf.Conv_sejour_import;

                    cfc_imp.CTR_DET = ccf.Detention_import;
                    cfc_imp.CTR_FRET = ccf.fret_import;
                    cfc_imp.CTR_OP_DIVERS = ccf.Operation_divers_import;
                    cfc_imp.CTR_REP = ccf.Reparation_import;
                    cfc_imp.CTR_SUR = ccf.Surestarie_import;
                    cfc_imp.CTR_VAL_RSDL = ccf.Valeur_residuel_import;
                    cfc_imp.DOSSIER = ccf.Frais_Dossier_import;
                    cmp_mpay_imp.Add(new CLIENT_M_PAY
                    {
                        IDCLIENT = cli.IdClient,
                        SENS = "I",
                        STATUS = "Actif",
                        TYPE = "Ctr",
                        CASH = ctr_cash_imp.IsChecked.Value,
                        CB = ctr_cb_imp.IsChecked.Value,
                        CC = ctr_cc_imp.IsChecked.Value,
                        WT = ctr_vir_imp.IsChecked.Value,
                        CS = ctr_cs_imp.IsChecked.Value
                    });

                    cfc_imp.VEH_FRET = ccf.Veh_fret_import;
                    cfc_imp.VEH_MANUT = ccf.Veh_manut_import;
                    cfc_imp.VEH_OP_DIVERS = ccf.Veh_od_import;
                    cfc_imp.VEH_PG = ccf.Veh_pg_import;
                    cfc_imp.VEH_SEJOUR = ccf.Veh_sejour_import;
                    //cmp_mpay_imp.Add(new CLIENT_M_PAY
                    //{
                    //    IDCLIENT = cli.IdClient,
                    //    SENS = "I",
                    //    STATUS = "Actif",
                    //    TYPE = "Veh",
                    //    CASH = veh_cash_imp.IsChecked.Value,
                    //    CB = veh_cb_imp.IsChecked.Value,
                    //    CC = veh_cc_imp.IsChecked.Value,
                    //    WT = veh_vir_imp.IsChecked.Value
                    //});

                    cfc_imp.PLAFOND = ccf.Plafond_import;

                    #endregion

                    #region export
                    CLIENT_FIN_COND cfc_exp = new CLIENT_FIN_COND();
                    List<CLIENT_M_PAY> cmp_mpay_exp = new List<CLIENT_M_PAY>();

                    cfc_exp.SENS = "E";
                    cfc_exp.IDCLIENT = cli.IdClient;

                    cfc_exp.CAUTION_MT = ccf.Caution_montant_export;
                    cfc_exp.CAUTION_T = ccf.Caution_type_import;
                    cmp_mpay_exp.Add(new CLIENT_M_PAY
                    {
                        IDCLIENT = cli.IdClient,
                        SENS = "E",
                        STATUS = "Actif",
                        TYPE = "Caut",
                        CASH =false,// caut_cash_exp.IsChecked.Value,
                        CB = false, //caut_cb_exp.IsChecked.Value,
                        CC =false, // caut_cc_exp.IsChecked.Value,
                        WT =false,// caut_vir_exp.IsChecked.Value,
                        CS =false // caut_cs_exp.IsChecked.Value
                    });

                    cfc_exp.CONV_MANUT = ccf.Conv_manut_export;
                    cfc_exp.CONV_OP_DIVERS = ccf.Conv_od_export;
                    cfc_exp.CONV_SEJOUR = ccf.Conv_sejour_export;

                    cfc_exp.CTR_DET = ccf.Detention_export;
                    cfc_exp.CTR_FRET = ccf.fret_export;
                    cfc_exp.CTR_OP_DIVERS = ccf.Operation_divers_export;
                    cfc_exp.CTR_REP = ccf.Reparation_export;
                    cfc_exp.CTR_SUR = ccf.Surestarie_export;
                    cfc_exp.CTR_VAL_RSDL = ccf.Valeur_residuel_export;
                    cfc_exp.DOSSIER = ccf.Frais_Dossier_export;
                    cmp_mpay_exp.Add(new CLIENT_M_PAY
                    {
                        IDCLIENT = cli.IdClient,
                        SENS = "E",
                        STATUS = "Actif",
                        TYPE = "Ctr",
                        CASH = ctr_cash_exp.IsChecked.Value,
                        CB = ctr_cb_exp.IsChecked.Value,
                        CC = ctr_cc_exp.IsChecked.Value,
                        WT = ctr_vir_exp.IsChecked.Value,
                        CS = ctr_cs_exp.IsChecked.Value
                    });

                    cfc_exp.VEH_FRET = ccf.Veh_fret_export;
                    cfc_exp.VEH_MANUT = ccf.Veh_manut_export;
                    cfc_exp.VEH_OP_DIVERS = ccf.Veh_od_export;
                    cfc_exp.VEH_PG = ccf.Veh_pg_export;
                    cfc_exp.VEH_SEJOUR = ccf.Veh_sejour_export;
                    //cmp_mpay_exp.Add(new CLIENT_M_PAY
                    //{
                    //    IDCLIENT = cli.IdClient,
                    //    SENS = "E",
                    //    STATUS = "Actif",
                    //    TYPE = "Veh",
                    //    CASH = veh_cash_exp.IsChecked.Value,
                    //    CB = veh_cb_exp.IsChecked.Value,
                    //    CC = veh_cc_exp.IsChecked.Value,
                    //    WT = veh_vir_exp.IsChecked.Value
                    //});
                    cfc_exp.PLAFOND = ccf.Plafond_export;
                    cfc_exp.COVVD = ccf.Covvd;
                    #endregion

                    vsomAcc.InsertClientFinancialCondition(cfc_imp, cfc_exp, cmp_mpay_imp, cmp_mpay_exp);
                    MessageBox.Show("Opération effectuée", "Condition financière client");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Echec de l'opération : " + ex.Message, "Condition financière client");
                }

            //}
        }

        private void cbClient_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            if (cbClient.SelectedIndex != -1)
            {
                CLIENT c = (CLIENT)cbClient.SelectedItem;
                txtCodeClient.Text = c.CodeClient;
                try
                { 
                  //charger la config du client
                    ccf = vsomAcc.LoadClientFinancialCondition(c.IdClient);
                    List<CLIENT_M_PAY> _lst = vsomAcc.LoadClientMPAY(c.IdClient);

                    txtCautTypeImp.SelectedIndex = ccf.Caution_type_import=="Classique" ? 1 : 0;
                    if (_lst.Count != 0)
                    {
                        //conteneur
                        CLIENT_M_PAY ctr_e = _lst.Single(s => s.SENS == "E" && s.TYPE == "Ctr");
                        CLIENT_M_PAY ctr_i = _lst.Single(s => s.SENS == "I" && s.TYPE == "Ctr");
                        ctr_cash_exp.IsChecked = ctr_e.CASH;
                        ctr_cb_exp.IsChecked = ctr_e.CB;
                        ctr_cc_exp.IsChecked = ctr_e.CC;
                        ctr_vir_exp.IsChecked = ctr_e.WT;
                        ctr_cs_exp.IsChecked = ctr_e.CS;

                        ctr_cash_imp.IsChecked = ctr_i.CASH;
                        ctr_cb_imp.IsChecked = ctr_i.CB;
                        ctr_cc_imp.IsChecked = ctr_i.CC;
                        ctr_vir_imp.IsChecked = ctr_i.WT;
                        ctr_cs_imp.IsChecked = ctr_i.CS;
                        //veh
                        //CLIENT_M_PAY veh_e = _lst.Single(s => s.SENS == "E" && s.TYPE == "Veh");
                        //CLIENT_M_PAY veh_i = _lst.Single(s => s.SENS == "I" && s.TYPE == "Veh");

                        //caution
                        CLIENT_M_PAY caut_e = _lst.Single(s => s.SENS == "E" && s.TYPE == "Caut");
                        CLIENT_M_PAY caut_i = _lst.Single(s => s.SENS == "I" && s.TYPE == "Caut");
                        txtCautTypeImp.Text = ccf.Caution_type_import; 
                        caut_cash_imp.IsChecked = caut_i.CASH;
                        caut_cb_imp.IsChecked = caut_i.CB;
                        caut_cc_imp.IsChecked = caut_i.CC;
                        caut_vir_imp.IsChecked = caut_i.WT;
                        caut_cs_imp.IsChecked = caut_i.CS;

                        caut_cash_exp.IsChecked = caut_e.CASH;
                        caut_cb_exp.IsChecked = caut_e.CB;
                        caut_cc_exp.IsChecked = caut_e.CC;
                        caut_vir_exp.IsChecked = caut_e.WT;
                        caut_cs_exp.IsChecked = caut_e.CS;
                    }
                    else
                    {
                        caut_cash_imp.IsChecked = false;
                        caut_cb_imp.IsChecked = false;
                        caut_cc_imp.IsChecked = false;
                        caut_vir_imp.IsChecked = false;
                        caut_cs_imp.IsChecked = false;

                        caut_cash_exp.IsChecked = false;
                        caut_cb_exp.IsChecked = false;
                        caut_cc_exp.IsChecked = false;
                        caut_vir_exp.IsChecked = false;
                        caut_cs_exp.IsChecked = false;

                        ctr_cash_exp.IsChecked = false;
                        ctr_cb_exp.IsChecked = false;
                        ctr_cc_exp.IsChecked = false;
                        ctr_vir_exp.IsChecked = false;
                        ctr_cs_exp.IsChecked = false;

                        ctr_cash_imp.IsChecked = false;
                        ctr_cb_imp.IsChecked = false;
                        ctr_cc_imp.IsChecked = false;
                        ctr_vir_imp.IsChecked = false;
                        ctr_cs_imp.IsChecked = false;
                    }
                    //
                    this.DataContext = ccf;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Les conditons ne sont pas préchargées", "Conditions Financières", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
