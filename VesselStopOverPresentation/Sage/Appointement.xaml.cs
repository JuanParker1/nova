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

namespace VesselStopOverPresentation.Sage
{
    /// <summary>
    /// Logique d'interaction pour Appointement.xaml
    /// </summary>
    public partial class Appointement : Window
    {
        private string filename = string.Empty;
       private double credit = 0; private double debit = 0;
        public Appointement()
        {
            InitializeComponent();
        }

        private void txtfile_MouseDoubleClick_1(object sender, MouseButtonEventArgs e)
        {
         
        }

        private void btnValider_Click_1(object sender, RoutedEventArgs e)
        {
            if (txtDateDebut.SelectedDate.HasValue == false || txtlib.Text.Trim().Length==0 || txtRef.Text.Trim().Length==0) 
            { MessageBox.Show("Les champs date, libelle, reference sont obligatoires", "Appointement paie", 
                MessageBoxButton.OK, MessageBoxImage.Information); return; }
            try
            {

                if (filename != "")
                {
                    Microsoft.Office.Interop.Excel.Application xlApp = null;
                    Microsoft.Office.Interop.Excel.Workbook xlWorkBook = null;
                    Microsoft.Office.Interop.Excel.Worksheet xlWorkSheet = null;
                    Microsoft.Office.Interop.Excel.Range range;


                    int rCnt = 7;
                    string lign = string.Empty;

                    xlApp = new Microsoft.Office.Interop.Excel.Application();
                    xlWorkBook = xlApp.Workbooks.Open(filename, 1, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
                    xlWorkSheet = (Microsoft.Office.Interop.Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

                    range = xlWorkSheet.UsedRange;
                    string a = string.Empty; string b = string.Empty; string c = string.Empty; string d = string.Empty;

                    string _path = System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + String.Format("\\sage_appointement_paie_{0}.PNM", string.Format("{0:ddMMyy}", txtDateDebut.SelectedDate.Value));

                    List<NOVA_SAGE2> sage = new List<NOVA_SAGE2>();
                    string custcode = string.Empty;
                    for (rCnt = 2; rCnt <= range.Cells.Rows.Count; rCnt++) //range.Cells.Rows.Count;
                    {
                        lign = rCnt.ToString();

                        if (lign == "210")
                        { }

                        a = (range.Cells[rCnt, 1] as Microsoft.Office.Interop.Excel.Range).Value2 == null ? "" : (range.Cells[rCnt, 1] as Microsoft.Office.Interop.Excel.Range).Value2.ToString();
                        b = (range.Cells[rCnt, 2] as Microsoft.Office.Interop.Excel.Range).Value2 == null ? "" : (range.Cells[rCnt, 2] as Microsoft.Office.Interop.Excel.Range).Value2.ToString();
                        d = (range.Cells[rCnt, 4] as Microsoft.Office.Interop.Excel.Range).Value2 == null ? "" : (range.Cells[rCnt, 4] as Microsoft.Office.Interop.Excel.Range).Value2.ToString();
                        c = (range.Cells[rCnt, 5] as Microsoft.Office.Interop.Excel.Range).Value2 == null ? "" : (range.Cells[rCnt, 5] as Microsoft.Office.Interop.Excel.Range).Value2.ToString();
                        string[] code = a.Split('-');


                        //custcode =  code[0];
                        if (c.Trim() == "")
                        {
                            sage.Add(new NOVA_SAGE2
                            {
                                Code = "503",
                                CodeTitle = a.Trim(),
                                CustomerCode = b,
                                DatePay = string.Format("{0:ddMMyy}", txtDateDebut.SelectedDate.Value),
                                DebitCredit = "D",
                                Description = txtlib.Text.Trim(),
                                FC = "OD",
                                GrossAmount = Math.Round(Convert.ToDouble(d), 0, MidpointRounding.AwayFromZero).ToString(),
                                InvoiceDate = string.Format("{0:ddMMyy}", txtDateDebut.SelectedDate.Value),
                                InvoiceNumber = txtRef.Text.Trim(),
                                N = "N",
                                PayType = "S",
                                X = "X"
                            });
                            debit += Math.Round(Convert.ToDouble(d), 0, MidpointRounding.AwayFromZero);
                        }

                        if (d.Trim() == "")
                        {

                            sage.Add(new NOVA_SAGE2
                            {
                                Code = "503",
                                CodeTitle = a.Trim(),
                                CustomerCode = b,
                                DatePay = string.Format("{0:ddMMyy}", txtDateDebut.SelectedDate.Value),
                                DebitCredit = "C",
                                Description = txtlib.Text.Trim() ,
                                FC = "OD",
                                GrossAmount = Math.Round(Convert.ToDouble(c), 0, MidpointRounding.AwayFromZero).ToString(),
                                InvoiceDate = string.Format("{0:ddMMyy}", txtDateDebut.SelectedDate.Value),
                                InvoiceNumber = txtRef.Text.Trim(),
                                N = "N",
                                PayType = "S",
                                X = "G"
                            });
                            credit += Math.Round(Convert.ToDouble(c), 0, MidpointRounding.AwayFromZero);
                        }

                        txtDebit.Text = debit.ToString();
                        txtcredit.Text = credit.ToString();
                    }

                    using (System.IO.StreamWriter sw = new System.IO.StreamWriter(_path))
                    {
                        sw.WriteLine("SOCOMAR - NOVA");
                        foreach (NOVA_SAGE2 ns in sage)
                        {
                            if (ns.CodeTitle == null || ns.GrossAmount.Trim() == "0")
                            { }
                            else
                            {
                                sw.WriteLine(string.Format("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}{11}{12}",
                                       ns.Code, ns.InvoiceDate, ns.FC, ns.CodeTitle, ns.X, ns.CustomerCode, ns.InvoiceNumber, ns.Description, ns.PayType, ns.DatePay, ns.DebitCredit,
                                       ns.GrossAmount, ns.N));
                            }
                        }
                    }

                    MessageBox.Show("Opération effectuée. Veuillez consulter le fichier sur votre bureau", "Appointement");
                    
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Echec du traitement du fichier. \n" + ex.Message, "Appointement");
            }

        }

        private void txtfile_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {


                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

                dlg.DefaultExt = ".xlsx";
                dlg.Filter = "Fichiers (*.xls)|*.xls|Fichiers (*.xlsx)|*.xlsx";
                Nullable<bool> result = dlg.ShowDialog();

                if (result == true)
                {
                    filename = dlg.FileName;
                    txtfile.Content = dlg.SafeFileName;
                }
                else
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Echec de la selection du fichier \n" + ex.Message, "Appointement");
            }
        }
    }
}
