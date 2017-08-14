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
using System.Windows.Navigation;
using System.Windows.Shapes;
using VesselStopOverData;
namespace VesselStopOverPresentation
{
    /// <summary>
    /// Logique d'interaction pour FinanceControlPanel.xaml
    /// </summary>
    public partial class FinanceControlPanel : StackPanel
    {
        private VSOMAccessors vsomAcc = new VSOMAccessors();
        public UTILISATEUR utilisateur { get; set; }
        private List<OPERATION> operationsUser;
        private VesselStopOverWindow vesselWindow;
        public FinanceControlPanel(VesselStopOverWindow window, UTILISATEUR user)
        {
            InitializeComponent();
            utilisateur = user;
            vesselWindow = window;
            operationsUser = vsomAcc.GetOperationsUtilisateur(user.IdU);
            if (operationsUser.Where(op => op.NomOp == "Finance : Extraction Journal Comptable").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
            {
                btnExtract.Visibility = System.Windows.Visibility.Collapsed;
                btnSageUnlockUser.Visibility = System.Windows.Visibility.Collapsed;
            }
            if (operationsUser.Where(op => op.NomOp == "Finance : Avoir Spot").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin" && utilisateur.LU != "Hermann")
            {
                btnAvoirSpot.Visibility = System.Windows.Visibility.Collapsed;
                btnCentralisationPaie.Visibility = System.Windows.Visibility.Collapsed; 
                btnDotationDeCharge.Visibility = System.Windows.Visibility.Collapsed;
                btnAppointement.Visibility = System.Windows.Visibility.Collapsed;
            }

            
        }

        private void btnExtract_Click_1(object sender, RoutedEventArgs e)
        {
            VesselStopOverPresentation.Finance.NovaToSAGE frm = new Finance.NovaToSAGE();
            frm.ShowDialog();
        }

        private void btnAvoirSpot_Click_1(object sender, RoutedEventArgs e)
        {
            Finance.AvoirSpotPanel aPanel = new Finance.AvoirSpotPanel(utilisateur);
            vesselWindow.mainPanel.Header = "Avoir Spot";
            vesselWindow.mainPanel.Content = null;
            vesselWindow.mainPanel.Content = aPanel;
        }

        /// <summary>
        /// sage paie
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void btnEscaleMAJ_Click_1(object sender, RoutedEventArgs e)
        {
            Sage.CentralisationPaie frm = new Sage.CentralisationPaie();
            frm.ShowDialog();

            //try
            //{
            //    //VSOMAccessors vs = new VSOMAccessors();
            //    int res = vsomAcc.NaplePartialCreditNote();
            //    MessageBox.Show(res.ToString(), "message");
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message, "meessage");
            //}

            ////Microsoft.Office.Interop.Excel.Application xlApp = null;
            ////Microsoft.Office.Interop.Excel.Workbook xlWorkBook = null;
            ////Microsoft.Office.Interop.Excel.Worksheet xlWorkSheet = null;
            ////Microsoft.Office.Interop.Excel.Range range;
            ////double credit = 0; double debit = 0;

            ////int rCnt = 7;
            ////string lign = string.Empty;
            ////try
            ////{
            ////    //VSOMAccessors vsomAcc = new VSOMAccessors();
            ////    //VsomMarchal vsm = new VsomMarchal();

            ////    Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            ////    dlg.DefaultExt = ".xlsx";
            ////    dlg.Filter = "Infos DIT (*.xls)|*.xls|Infos DIT (*.xlsx)|*.xlsx";
            ////    Nullable<bool> result = dlg.ShowDialog();
            ////    string filename = "";
            ////    if (result == true)
            ////    {
            ////        filename = dlg.FileName;
            ////    }
            ////    else
            ////    {
            ////        return;
            ////    }
                
            ////    if (filename != "")
            ////    {
            ////        xlApp = new Microsoft.Office.Interop.Excel.Application();
            ////        xlWorkBook = xlApp.Workbooks.Open(filename, 1, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
            ////        xlWorkSheet = (Microsoft.Office.Interop.Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

            ////        range = xlWorkSheet.UsedRange;
            ////        string a = string.Empty; string b = string.Empty; string c = string.Empty; string d = string.Empty;

            ////        string _path = System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + String.Format("\\sage_centralisation_paie_{0}.PNM", string.Format("{0:ddMMyy}", DateTime.Today));

            ////        List<NOVA_SAGE2> sage = new List<NOVA_SAGE2>();
            ////        string custcode = string.Empty;
            ////        for (rCnt = 2; rCnt < range.Cells.Rows.Count; rCnt++) //range.Cells.Rows.Count;
            ////        {
            ////            lign = rCnt.ToString();

            ////            if (lign == "212")
            ////            { }

            ////            a = (range.Cells[rCnt, 1] as Microsoft.Office.Interop.Excel.Range).Value2 == null ? "" : (range.Cells[rCnt, 1] as Microsoft.Office.Interop.Excel.Range).Value2.ToString();
            ////            b = (range.Cells[rCnt, 2] as Microsoft.Office.Interop.Excel.Range).Value2 == null ? "" : (range.Cells[rCnt, 2] as Microsoft.Office.Interop.Excel.Range).Value2.ToString();
            ////            d = (range.Cells[rCnt, 3] as Microsoft.Office.Interop.Excel.Range).Value2 == null ? "" : (range.Cells[rCnt, 3] as Microsoft.Office.Interop.Excel.Range).Value2.ToString();
            ////            c = (range.Cells[rCnt, 4] as Microsoft.Office.Interop.Excel.Range).Value2 == null ? "" : (range.Cells[rCnt, 4] as Microsoft.Office.Interop.Excel.Range).Value2.ToString();
            ////            string[] code = a.Split('-');

            ////            if (a.StartsWith("4211100 - SALARI") || a.StartsWith("4212100 - SALARI") || a.StartsWith("4221100 - SALARI"))
            ////            {

            ////                custcode = code[1].Trim(); ;
            ////            }
            ////            else
            ////            {
            ////                custcode = "";
            ////            }
            ////           // custcode =  code[0];
            ////            if (c.Trim() == "")
            ////            {
            ////                sage.Add(new NOVA_SAGE2
            ////                {
            ////                    Code = "860",
            ////                    CodeTitle = code[0].Trim(),
            ////                    CustomerCode = custcode,
            ////                    DatePay = string.Format("{0:ddMMyy}", DateTime.Today.AddDays(-13)),
            ////                    DebitCredit = "D",
            ////                    Description = "Paie Mai 2017",
            ////                    FC = "OD",
            ////                    GrossAmount =  Math.Round(Convert.ToDouble(d), 0, MidpointRounding.AwayFromZero).ToString(),
            ////                    InvoiceDate = string.Format("{0:ddMMyy}", DateTime.Today.AddDays(-13)),
            ////                    InvoiceNumber = b,
            ////                    N = "N",
            ////                    PayType = "S",
            ////                    X = custcode == "" ? "G" : "X"
            ////                });
            ////                debit += Math.Round(Convert.ToDouble(d), 0, MidpointRounding.AwayFromZero);
            ////            }

            ////            if (d.Trim() == "")
            ////            {

            ////                sage.Add(new NOVA_SAGE2
            ////                {
            ////                    Code = "860",
            ////                    CodeTitle = code[0].Trim(),
            ////                    CustomerCode = custcode,
            ////                    DatePay = string.Format("{0:ddMMyy}", DateTime.Today.AddDays(-3)),
            ////                    DebitCredit = "C",
            ////                    Description = "Paie Mai 2017",
            ////                    FC = "OD",
            ////                    GrossAmount = Math.Round(Convert.ToDouble(c), 0, MidpointRounding.AwayFromZero).ToString(),
            ////                    InvoiceDate = string.Format("{0:ddMMyy}", DateTime.Today.AddDays(-3)),
            ////                    InvoiceNumber = b,
            ////                    N = "N",
            ////                    PayType = "S",
            ////                    X = custcode == "" ? "G" : "X"
            ////                });
            ////                credit += Math.Round(Convert.ToDouble(c), 0, MidpointRounding.AwayFromZero);
            ////            }


            ////        }

            ////        using (System.IO.StreamWriter sw = new System.IO.StreamWriter(_path))
            ////        {
            ////            sw.WriteLine("SOCOMAR - NOVA");
            ////            foreach (NOVA_SAGE2 ns in sage)
            ////            {
            ////                if (ns.CodeTitle == null || ns.GrossAmount.Trim() == "0")
            ////                { }
            ////                else
            ////                {
            ////                    sw.WriteLine(string.Format("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}{11}{12}",
            ////                           ns.Code, ns.InvoiceDate, ns.FC, ns.CodeTitle, ns.X, ns.CustomerCode, ns.InvoiceNumber, ns.Description, ns.PayType, ns.DatePay, ns.DebitCredit,
            ////                           ns.GrossAmount, ns.N));
            ////                }
            ////            }
            ////        }
            ////    }

            ////    MessageBox.Show("Opération effectuée. Veuillez consulter le fichier généré. \n debit : " + debit + " credit : " + credit, "paie");
            ////}
            ////catch (Exception ex)
            ////{
            ////    MessageBox.Show(lign + "error : " + ex.Message + " " + ex.StackTrace, "error");
            ////}
        }

        private void btnDotationDeCharge_Click_1(object sender, RoutedEventArgs e)
        {
            Sage.DotationCharge frm = new Sage.DotationCharge();
            frm.ShowDialog();
            return;

           
        }

        private void btnSageUnlockUser_Click_1(object sender, RoutedEventArgs e)
        {
            Sage.UserSession frm = new Sage.UserSession();
            frm.ShowDialog();
        }

        private void btnAppointement_Click_1(object sender, RoutedEventArgs e)
        {
            Sage.Appointement frm = new Sage.Appointement();
            frm.ShowDialog();
            return;
        }
    }
}
