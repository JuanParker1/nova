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
using Excel = Microsoft.Office.Interop.Excel;

namespace VesselStopOverPresentation
{
    /// <summary>
    /// Logique d'interaction pour LoadEmplacementsForm.xaml
    /// </summary>
    public partial class LoadEmplacementsForm : Window
    {
        public LoadEmplacementsForm()
        {
            InitializeComponent();
        }

        private void btnSelectFile_Click(object sender, RoutedEventArgs e)
        {
            Excel.Application xlApp = null;
            Excel.Workbook xlWorkBook = null;
            Excel.Worksheet xlWorkSheet = null;
            Excel.Range range;

            try
            {
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

                dlg.DefaultExt = ".xlsx";
                dlg.Filter = "Excel Attribution emplacement (*.xlsx)|*.xlsx";
                Nullable<bool> result = dlg.ShowDialog();
                if (result == true)
                {
                    string filename = dlg.FileName;
                    txtCheminFichier.Text = filename;
                }
                else
                {
                    return;
                }

                //VSOMAccessors vsomAcc = new VSOMAccessors();
                VsomMarchal vsomAcc = new VsomMarchal();
                int rCnt = 2;

                xlApp = new Excel.Application();
                xlWorkBook = xlApp.Workbooks.Open(txtCheminFichier.Text, 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
                xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

                range = xlWorkSheet.UsedRange;

                while (((string)(range.Cells[rCnt, 1] as Excel.Range).Value2) != null)
                {

                    string numChassis = ((string)(range.Cells[rCnt, 1] as Excel.Range).Value2);
                    string numBL = ((string)(range.Cells[rCnt, 2] as Excel.Range).Value2);
                    string date = ((string)(range.Cells[rCnt, 4] as Excel.Range).Value2);
                    DateTime dateEntree = new DateTime(Convert.ToInt32(date.Substring(6, 4)), Convert.ToInt32(date.Substring(3, 2)), Convert.ToInt32(date.Substring(0, 2)));
                    string ligPos = ((string)(range.Cells[rCnt, 6] as Excel.Range).Value2).Substring(0, 2);
                    string colPos = ((string)(range.Cells[rCnt, 6] as Excel.Range).Value2).Substring(2, 2);

                    vsomAcc.ReceptionnerVehiculeGesparc(numChassis, numBL, dateEntree, ligPos, colPos, 1);

                    rCnt++;
                }

                MessageBox.Show("Opération termninée");
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                
            }
            finally
            {
                xlWorkBook.Close(true, null, null);
                xlApp.Quit();

                releaseObject(xlWorkSheet);
                releaseObject(xlWorkBook);
                releaseObject(xlApp);
            }
        }

        private void releaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception ex)
            {
                obj = null;
                MessageBox.Show("Unable to release the Object " + ex.ToString());
            }
            finally
            {
                GC.Collect();
            }
        }
    }
}
