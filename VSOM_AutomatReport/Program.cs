using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace VSOM_AutomatReport
{
    class Program
    {
        static void Main(string[] args)
        {
            Microsoft.Office.Interop.Excel.Application xlApp = null;
            Microsoft.Office.Interop.Excel.Workbook xlWorkBook = null;
            Microsoft.Office.Interop.Excel.Worksheet xlWorkSheet = null;
            Microsoft.Office.Interop.Excel.Range range;
            SqlConnection scon = null; SqlDataReader dr = null;
            string debut = DateTime.Today.ToShortDateString(); string fin = DateTime.Today.AddDays(1).ToShortDateString();
            try
            {
                xlApp = new Microsoft.Office.Interop.Excel.Application();
                xlWorkBook = xlApp.Workbooks.Open("C:\\VSOM_Automate\\Ressources\\templateRecapVehSortie.xlsx", Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                xlWorkSheet = (Microsoft.Office.Interop.Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);
                range = xlWorkSheet.UsedRange;
                int i = 3;

                string strcnn = @"Data Source=192.168.0.28\SVR2012;Initial Catalog=NAVY;User ID=sa;Password=P@ssw0rd2012";
                 scon= new SqlConnection(strcnn);
                scon.Open(); SqlCommand scmd;
                scmd = new SqlCommand(string.Format("select VEHICULE.DSPVeh,VEHICULE.FSVeh, VEHICULE.DSRVeh, VEHICULE.NumChassis , VEHICULE.DescVeh, BON_SORTIE.IdBS,BON_SORTIE.DateBS , (select CONNAISSEMENT.NumBL from CONNAISSEMENT where CONNAISSEMENT.IdBL=VEHICULE.IdBL) as numbl from VEHICULE inner join BON_SORTIE on VEHICULE.IdBS=BON_SORTIE.IdBS where BON_SORTIE.DateBS>='{0}' and BON_SORTIE.DateBS<'{1}' and VEHICULE.DSRVeh is null", debut, fin), scon);
                
                dr = scmd.ExecuteReader();
                //string chassi = string.Empty; string descveh = string.Empty; string numbl=string.Empty;
                //StringBuilder sb = new StringBuilder();

                #region lecture

                while (dr.Read())
                {

                    if (!(dr.IsDBNull(dr.GetOrdinal("NumChassis"))))
                    {
                        (range.Cells[i, 1] as Microsoft.Office.Interop.Excel.Range).Value2 = dr.GetString(dr.GetOrdinal("NumChassis"));
                    }

                    if (!(dr.IsDBNull(dr.GetOrdinal("DescVeh"))))
                    {
                        (range.Cells[i, 2] as Microsoft.Office.Interop.Excel.Range).Value2 = dr.GetString(dr.GetOrdinal("DescVeh"));
                    }

                    if (!(dr.IsDBNull(dr.GetOrdinal("NumBL"))))
                    {
                        (range.Cells[i, 3] as Microsoft.Office.Interop.Excel.Range).Value2 = dr.GetString(dr.GetOrdinal("NumBL"));
                    }
                    if (!(dr.IsDBNull(dr.GetOrdinal("FSVeh"))))
                    {
                        (range.Cells[i, 4] as Microsoft.Office.Interop.Excel.Range).Value2 = dr.GetDateTime(dr.GetOrdinal("FSVeh")).ToString();
                    }

                    if (!(dr.IsDBNull(dr.GetOrdinal("DSRVeh"))))
                    {
                        (range.Cells[i, 5] as Microsoft.Office.Interop.Excel.Range).Value2 = dr.GetDateTime(dr.GetOrdinal("DSRVeh")).ToString();
                    }

                    if (!(dr.IsDBNull(dr.GetOrdinal("IdBS"))))
                    {
                        (range.Cells[i, 6] as Microsoft.Office.Interop.Excel.Range).Value2 = dr.GetInt32(dr.GetOrdinal("IdBS"));
                    }

                    if (!(dr.IsDBNull(dr.GetOrdinal("DateBS"))))
                    {
                        (range.Cells[i, 7] as Microsoft.Office.Interop.Excel.Range).Value2 = dr.GetDateTime(dr.GetOrdinal("DateBS")).ToString();
                    }

                    i++;
                } 

                #endregion

                if (dr != null) dr.Close();

                  //recherche du nombre de bs scannée
                scmd = new SqlCommand(string.Format("select count(distinct(BON_SORTIE.IdBS)) as nbr from VEHICULE inner join BON_SORTIE on VEHICULE.IdBS=BON_SORTIE.IdBS where BON_SORTIE.DateBS>='{0}' and BON_SORTIE.DateBS<'{1}' and VEHICULE.DSRVeh is not null", debut, fin), scon);
                int countbs = (Int32)scmd.ExecuteScalar();
                (range.Cells[3, 9] as Microsoft.Office.Interop.Excel.Range).Value2 =countbs;

                // recherche du nombre de bs cree
                scmd = new SqlCommand(string.Format("select count(bon_sortie.idbs) as nbr from BON_SORTIE where BON_SORTIE.DateBS>='{0}' and BON_SORTIE.DateBS<'{1}'", debut, fin), scon);
                int countbs2 = (Int32)scmd.ExecuteScalar();
                (range.Cells[3, 10] as Microsoft.Office.Interop.Excel.Range).Value2 =countbs2;
                
                //sauvegarde
                string path = "C:\\VSOM_Automate\\RecapBonDeSortie_VehiculesLivrés_" + string.Format("{0:ddMMyy}", DateTime.Today) + ".xlsx";
                xlWorkBook.SaveAs(path, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlExclusive, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

                if (xlWorkSheet != null) { releaseObject(xlWorkSheet);}

                if (xlWorkBook != null) { xlWorkBook.Close(true, Type.Missing, Type.Missing); releaseObject(xlWorkBook); }

                bool excelWasRunning = System.Diagnostics.Process.GetProcessesByName("EXCEL.EXE").Length > 0;

                if (excelWasRunning) { xlApp.Quit(); releaseObject(xlApp); }
                
                //envoie email
                //string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\RecapBonDeSortie_VehiculesLivrés_" + string.Format("{0:ddMMyy}", DateTime.Today) + ".xlsx";
              

               /*old code for sending email
                * Microsoft.Office.Interop.Outlook.Application app = new Microsoft.Office.Interop.Outlook.Application();
                Microsoft.Office.Interop.Outlook.MailItem mailItem = app.CreateItem(Microsoft.Office.Interop.Outlook.OlItemType.olMailItem);
                mailItem.Subject = "Recap Sorties Vehicules (Parc Auto)";
                mailItem.To ="direction@socomarcm.net" ; mailItem.CC = "adohouannon@socomarcm.net";
                mailItem.Body = "Merci de voir le rapport des vehicules ayant fait l'objet de bon de sorties et non livré ce jour "+ debut;
                mailItem.Attachments.Add(path);
                mailItem.Importance = Microsoft.Office.Interop.Outlook.OlImportance.olImportanceHigh;
                mailItem.Display(false);
                mailItem.Send();
                */

                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("webmail.socomar-cameroun.com");

                mail.From = new MailAddress("automate@socomar-cameroun.com");
                mail.To.Add("direction@socomar-cameroun.com");
                mail.CC.Add("hermann.adohouannon@socomar-cameroun.com");
                mail.Subject = "Recap Sorties Vehicules (Parc Auto)";
                mail.Body = "Merci de voir le rapport des vehicules ayant fait l'objet de bon de sorties et non livré ce jour " + debut;
                mail.Attachments.Add(new Attachment(path));
                //SmtpServer.Port = 587;
                SmtpServer.Credentials = new System.Net.NetworkCredential("automate@socomar-cameroun.com", "Socom@r17!");
                //SmtpServer.EnableSsl = true;

                SmtpServer.Send(mail);

                Console.WriteLine("ok");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message+" "+ex.InnerException);
                //Console.ReadLine();
                using (System.IO.StreamWriter sw = new System.IO.StreamWriter( "C:\\VSOM_Automate\\log_" + string.Format("{0:ddMMyy}", DateTime.Today) + ".txt"))
                {
                    sw.WriteLine(ex.InnerException);
                    sw.WriteLine(ex.Message);
                     
                }
            }
            finally
            {
               // if (dr != null) dr.Close();
                scon.Close();
                
                
            }

            

        }

        private static void releaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception ex)
            {
                obj = null;
                //MessageBox.Show("Unable to release the Object " + ex.ToString());
            }
            finally
            {
                GC.Collect();
            }
        }
    }
}
