using System;
using System.Collections.Generic;
using System.Linq;
using System.Text; 
using System.Data.Linq; 
using System.Configuration;
using VesselStopOverData;
using System.Net;
using System.IO;
using System.Runtime;
using System.Transactions;
namespace VsomRemoteSVC
{
    class Program
    {
        static void Main(string[] args)
        {

            #region connaaissement

            using (var transaction = new System.Transactions.TransactionScope())
            {
                string url = System.Configuration.ConfigurationManager.AppSettings["url"];
                VSOMClassesDataContext dcAcc = new VSOMClassesDataContext();

                List<RMT_CONNAISSEMENT> lst = dcAcc.GetTable<RMT_CONNAISSEMENT>().ToList<RMT_CONNAISSEMENT>();
                List<int> lstdisct = lst.Select(p => p.SysId).Distinct().ToList();
                int nbrtraitement = 0;
                StringBuilder sb = new StringBuilder();
                foreach (int ind in lstdisct)
                {

                    try
                    {
                        CONNAISSEMENT con = (from m in dcAcc.GetTable<CONNAISSEMENT>() where m.IdBL == ind select m).FirstOrDefault();
                        if (con != null)
                        {
                            HttpWebRequest httpWebRequest = null;
                            StringBuilder text = new StringBuilder();
                            //si le connaissement n'a pas de facture, on envoie ses element de facture actuelle
                            int nbrfact = (from m in dcAcc.GetTable<FACTURE>() from mp in dcAcc.GetTable<PROFORMA>() where m.IdFP == mp.IdFP && mp.IdBL == con.IdBL select m).Count();
                            if (nbrfact == 0)
                            {
                                VEHICULE veh = (from m in dcAcc.GetTable<VEHICULE>() where m.IdBL == con.IdBL select m).First<VEHICULE>();
                                #region envoie connaissement
                                httpWebRequest = (HttpWebRequest)WebRequest.Create(url + "SBL/");
                                httpWebRequest.Method = WebRequestMethods.Http.Post;
                                httpWebRequest.Accept = "application/json";
                                httpWebRequest.UserAgent = ".NET Framework Client";
                                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                                {
                                    Json_CONNAISSEMENT obj = new Json_CONNAISSEMENT();
                                    obj.adr = con.AdresseBL; obj.client = con.IdClient.ToString(); obj.consignee = con.ConsigneeBL;
                                    obj.id = con.IdBL.ToString(); obj.notify = con.NotifyBL; obj.num = con.NumBL; obj.chassis = veh.NumChassis;
                                    MemoryStream strem = new MemoryStream();
                                    System.Runtime.Serialization.Json.DataContractJsonSerializer ser = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(Json_CONNAISSEMENT));
                                    ser.WriteObject(strem, obj);
                                    strem.Position = 0;
                                    StreamReader sr = new StreamReader(strem);
                                    streamWriter.Write(sr.ReadToEnd());
                                    streamWriter.Flush();
                                }
                                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                                {
                                    text.AppendLine(streamReader.ReadToEnd());
                                }
                                #endregion

                                //recuperation element facture vehicule et bl
                                #region element de factureation du bl
                                List<ELEMENT_FACTURATION> listef = (from m in dcAcc.GetTable<ELEMENT_FACTURATION>() where m.IdBL == con.IdBL && (m.EltFacture == "BL" || m.EltFacture == "Veh") select m).ToList<ELEMENT_FACTURATION>();
                                List<Json_ELEMENT> lstjson = new List<Json_ELEMENT>();
                                foreach (ELEMENT_FACTURATION elt in listef)
                                {
                                    Json_ELEMENT je = new Json_ELEMENT();
                                    je.bl = elt.IdBL.ToString(); je.codeart = elt.CodeArticle; je.elt = elt.EltFacture; je.id = elt.IdJEF.ToString();
                                    je.idfd = ""; je.lib = elt.LibEF; je.pu = elt.PUEF.Value.ToString(); je.qte = elt.QTEEF.Value.ToString().Replace(',', '.') ; je.statut = elt.StatutEF;
                                    je.taux = elt.TauxTVA.ToString(); je.unite = elt.UnitEF; je.codetva = elt.CodeTVA;
                                    lstjson.Add(je);
                                }

                                httpWebRequest = (HttpWebRequest)WebRequest.Create(url + "SL/");
                                httpWebRequest.Method = WebRequestMethods.Http.Post;
                                httpWebRequest.Accept = "application/json";
                                httpWebRequest.UserAgent = ".NET Framework Client";
                                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                                {
                                    MemoryStream strem = new MemoryStream();
                                    System.Runtime.Serialization.Json.DataContractJsonSerializer ser = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(List<Json_ELEMENT>));
                                    ser.WriteObject(strem, lstjson);
                                    strem.Position = 0;
                                    StreamReader sr = new StreamReader(strem);
                                    string jlst = sr.ReadToEnd();
                                    streamWriter.Write(jlst);
                                    sb.AppendLine(jlst);
                                    streamWriter.Flush();
                                }
                                var httpResponse2 = (HttpWebResponse)httpWebRequest.GetResponse();
                                using (var streamReader = new StreamReader(httpResponse2.GetResponseStream()))
                                {
                                    text.AppendLine(streamReader.ReadToEnd());
                                }

                                #endregion
                            }
                            else
                            {
                                #region envoie connaissement
                                VEHICULE veh = (from m in dcAcc.GetTable<VEHICULE>() where m.IdBL == con.IdBL select m).First<VEHICULE>();

                                httpWebRequest = (HttpWebRequest)WebRequest.Create(url + "SBL/");
                                httpWebRequest.Method = WebRequestMethods.Http.Post;
                                httpWebRequest.Accept = "application/json";
                                httpWebRequest.UserAgent = ".NET Framework Client";
                                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                                {
                                    Json_CONNAISSEMENT obj = new Json_CONNAISSEMENT();
                                    obj.adr = con.AdresseBL; obj.client = con.IdClient.ToString(); obj.consignee = con.ConsigneeBL;
                                    obj.id = con.IdBL.ToString(); obj.notify = con.NotifyBL; obj.num = '_' + con.NumBL;
                                    obj.chassis = veh.NumChassis;
                                    MemoryStream strem = new MemoryStream();
                                    System.Runtime.Serialization.Json.DataContractJsonSerializer ser = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(Json_CONNAISSEMENT));
                                    ser.WriteObject(strem, obj);
                                    strem.Position = 0;
                                    StreamReader sr = new StreamReader(strem);
                                    streamWriter.Write(sr.ReadToEnd());
                                    streamWriter.Flush();
                                }
                                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                                {
                                    text.AppendLine(streamReader.ReadToEnd());
                                }
                                #endregion
                            }

                            Console.WriteLine(text.ToString());
                            //supprimer le bl  
                            List<RMT_CONNAISSEMENT> todel = (from m in lst where m.SysId == ind select m).ToList<RMT_CONNAISSEMENT>();
                            dcAcc.GetTable<RMT_CONNAISSEMENT>().DeleteAllOnSubmit(todel);
                            dcAcc.SubmitChanges();
                        }
                    }
                    catch (WebException ex1)
                    {
                        Console.WriteLine(ex1.Message + " " + ind);
                       // Console.ReadLine();
                        sb.AppendLine(ex1.Message + " " + ind);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message+" "+ind);
                        sb.AppendLine(ex.Message + " " + ind);
                        //Console.ReadLine();
                    }

                    nbrtraitement += 1;

                    if (nbrtraitement > 10)
                    { break; }

                }

                string path = System.Configuration.ConfigurationManager.AppSettings["path"];
                using (System.IO.StreamWriter sw = new System.IO.StreamWriter(path+"log_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt"))
                {
                    sw.WriteLine(sb.ToString()); 

                }
                
                dcAcc.SubmitChanges();
                transaction.Complete();
            }

            #endregion
        }
    }
}
