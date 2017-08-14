using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;

namespace signature
{
    class Program
    {
        static void Main(string[] args)
        {

            string strcnn = @"Data Source=192.168.0.28\SVR2012;Initial Catalog=INHOUSEAPP;User ID=sa;Password=P@ssw0rd2012";
            SqlConnection scon = new SqlConnection(strcnn);
            SqlDataReader dr = null;
            try
            {
                Console.WriteLine("----Genration signature----");
                string appDataDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Microsoft\\Signatures";
                //Console.WriteLine("Source  : "+appDataDir);
                string signature = string.Empty;
                DirectoryInfo diInfo = new DirectoryInfo(appDataDir);
                //Console.WriteLine("DiInfo : "+diInfo);

                if (diInfo.Exists)
                {
                    Console.WriteLine("Directory Accès OK");

                    FileInfo[] fiSignature = diInfo.GetFiles("*.htm");

                    Console.WriteLine(fiSignature.Length+" Signature(s) found");

                    string path = string.Empty;
                    if (fiSignature.Length > 0)
                    {
                        int nbr = 0;
                        while (nbr < fiSignature.Length)
                        {
                            if (fiSignature[nbr].Name.Equals("socomar.htm")) { path = fiSignature[nbr].FullName; break; } nbr += 1;
                        }
                        //StreamReader sr = new StreamReader(path, Encoding.Default);
                        //signature = sr.ReadToEnd();

                        if (path == string.Empty)
                        {
                            Console.WriteLine("Aucune signature du nom de socomar n'est trouvé sur votre machine.");
                            path = fiSignature[0].FullName;
                            Path.GetFileNameWithoutExtension(path);
                            Console.WriteLine("Votre signature par defaut devra être : " + Path.GetFileNameWithoutExtension(path));
                        }


                        scon.Open(); SqlCommand scmd;
                        Console.WriteLine("Saisir votre mail puis valider avec la touche entrée");
                        string mail = Console.ReadLine();
                        if (mail == string.Empty)
                        {
                            while (mail == string.Empty)
                            {
                                Console.WriteLine("Saisir votre mail puis valider");
                                mail = Console.ReadLine();
                            }
                        }

                        scmd = new SqlCommand(string.Format("select * from MAILING where Adressemessagerie='{0}'", mail), scon);

                        dr = scmd.ExecuteReader();
                        string Titre1 = string.Empty; string Prenom = string.Empty; string Deuxiemeprenom = string.Empty;
                        string Nom = string.Empty; string Telmobile = string.Empty;
                        StringBuilder sb = new StringBuilder();

                        while (dr.Read())
                        {

                            if (!(dr.IsDBNull(dr.GetOrdinal("Titre1"))))
                            {
                                Titre1 = dr.GetString(dr.GetOrdinal("Titre1"));
                            }

                            if (!(dr.IsDBNull(dr.GetOrdinal("Prenom"))))
                            {
                                Prenom = dr.GetString(dr.GetOrdinal("Prenom"));
                            }

                            if (!(dr.IsDBNull(dr.GetOrdinal("Deuxiemeprenom"))))
                            {
                                Deuxiemeprenom = dr.GetString(dr.GetOrdinal("Deuxiemeprenom"));
                            }
                            if (!(dr.IsDBNull(dr.GetOrdinal("Nom"))))
                            {
                                Nom = dr.GetString(dr.GetOrdinal("Nom"));
                            }
                            if (!(dr.IsDBNull(dr.GetOrdinal("Telmobile"))))
                            {
                                Telmobile = dr.GetString(dr.GetOrdinal("Telmobile"));
                            }

                        }

                        if (Nom == string.Empty)
                        {
                            Console.WriteLine("Cette adresse n'est pas définie. ");
                            //Console.ReadKey();
                        }
                        else
                        {

                            string template = " <table border='0' cellpadding='0' cellspacing='0' style='background:none; border-width:0px; border:0px; margin:0; padding:0'><tbody><tr><td style='padding-top: 0; padding-bottom: 0; padding-left: 0; padding-right: 7px; border-top: 0; border-bottom: 0: border-left: 0; border-right: solid 3px #E45618' valign='top'><a href='http://socomar-cameroun.com'><img src='http://socomar-grimaldi.com/wp-content/uploads/2017/email-img.jpg' /></a></td><td style='padding-top: 0; padding-bottom: 0; padding-left: 12px; padding-right: 0;'><table border='0' cellpadding='0' cellspacing='0' style='background:none; border-width:0px; border:0px; margin:0; padding:0'>" +
      "<tbody><tr><td colspan='2' style='color:#3949ab;'><strong>" + Nom + " " + Prenom + " " + Deuxiemeprenom + "</strong></td></tr> <tr> <td colspan='2' style='color:#3949ab;'><em>" + Titre1 + "</em></td> </tr> <tr> <td colspan='2' style='color:#3949ab;'><strong>SOCOMAR</strong></td> </tr>" +
      "<tr><td style='vertical-align:top; width:50px'><span style='color:#cc3333'>Office:</span></td><td style='vertical-align:top; color:#3949ab;'>(237) 233 424 550&nbsp;&nbsp; <span style='color:#cc3333'>Mobile:&nbsp;</span> " + Telmobile + "</td>" +
      "</tr> <tr> <td colspan='2' style='color:#3949ab;'>BP : 12351 Douala Cameroun </td>  </tr><tr> <td style='vertical-align:top; width:58px; color:#3949ab;'>Adresse:</td> <td style='vertical-align:top; color:#3949ab;'>Douala, Avenue de L&#39;UDEAC, Zone Portuaire</td></tr> </tbody> </table> </td> </tr> </tbody></table> </a>";
                              
                            /*string template2 = "<div style='float: left;   width: 200px; height: 100px; vertical-align: top;'><a href='www.socomar-cameroun.com'><img alt='socomar' src='http://socomar-cameroun.com/wp-content/uploads/2017/email-img.jpg' /></a></div><div style='float: right; color: #000;  width:340px; height: 100px; padding-left: 3px; padding-top : 7px; vertical-align: top;'>"+
                                "<div > <label>" + Nom + " " + Prenom + " " + Deuxiemeprenom + " </label> <br/> <em>" + Titre1 + "</em> <br/><strong>SOCOMAR</strong> </div> <div style='width:330px;'><div style='float: left; vertical-align: top margin-right:2px; width:50%;'> <label> <span style='color:#cc3333'>Tel:&nbsp;</span> (237) 33 42 49 36 </label> </div>";
if(Telmobile!=string.Empty)
 template2 = template2+ "<div style=' float: left; vertical-align: top margin-right:2px; width:50%;'> <span style='color:#cc3333'>M:&nbsp;</span> (237)"+ Telmobile+"</label></div>";
 template2 = template2+"</div> <div style='margin-right:5px; width:100%;'> <span style='color:#cc3333'>BP:&nbsp;</span> 12351 Douala Cameroun </div><div style='margin-right:5px; width:100%;'> <span style='color:#cc3333'>Adr:&nbsp;</span> Douala, Avenue de L'UDEAC, Zone Portuaire </div><div style='margin-right:5px; width:100%;'> <span style='color:#cc3333'>Web:&nbsp;</span> www.socomar-cameroun.com </div></div></div>";
*/

                            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(path))
                            {
                                sw.WriteLine(template.ToString());
                            }
                            string signname = Path.GetFileNameWithoutExtension(path);

                            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(appDataDir + "\\" + signname + ".txt"))
                            {
                                sw.WriteLine("");
                            }

                            Console.WriteLine("Signature définie avec succès");
                           // Console.ReadKey();
                        }




                        //if (!string.IsNullOrEmpty(signature))
                        //{
                        //    string fileName = fiSignature[0].Name.Replace(fiSignature[0].Extension, string.Empty);
                        //    signature = signature.Replace(fileName + "_files/", appDataDir + "/" + fileName + "_files/");
                        //}
                    }
                    else 
                    {
                        Console.WriteLine("Aucune signature n'est configurée. Configurer une signature et Reessayer.");
                        
                    }

                }
                else
                {
                    Console.WriteLine("Emplacement non accèsible");
                   // Console.ReadKey();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
               // Console.ReadKey();
            }
            finally
            {
                if (dr != null) dr.Close();
                scon.Close();
            }
            Console.WriteLine("Valider pour quitter");
            Console.ReadKey(); 
        }
    }
}
