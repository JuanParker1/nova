using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Data.SqlClient;
using System.Text;
namespace VSOM_WS
{
    /// <summary>
    /// Description résumée de Service1
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // Pour autoriser l'appel de ce service Web depuis un script à l'aide d'ASP.NET AJAX, supprimez les marques de commentaire de la ligne suivante. 
    // [System.Web.Script.Services.ScriptService]
    public class Service1 : System.Web.Services.WebService
    {

        [WebMethod]
        public string SetScan(string text) 
        {
            try
            {
                //traitement du fichier
                string[] _t = text.Split(new string[] {"\r\n"},StringSplitOptions.None);
                
                string strcnn = @"Data Source=192.168.0.28\SVR2012;Initial Catalog=NAVY;User ID=sa;Password=P@ssw0rd2012";
                SqlConnection scon = new SqlConnection(strcnn);
                scon.Open(); SqlCommand scmd; string[] c_str; 
                for (int i = 0; i < _t.Length; i++)
                {
                    c_str = _t[i].Split('|');
                    string[] _str = c_str[0].Split('-');
                    scmd = new SqlCommand(string.Format("insert into PSION_SORTIES(CHASSIS,IDBS,SCAN,FULLDATA) values('{0}','{1}','{2}','{3}')", string.Empty, _str[1], c_str[2], _t[i]), scon);
                    scmd.ExecuteNonQuery();
                } 
                scon.Close();
                return "ok";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }


        [WebMethod]
        public string SetSortie(string idbs)
        {
            string strcnn = @"Data Source=192.168.0.28\SVR2012;Initial Catalog=NAVY;User ID=sa;Password=P@ssw0rd2012";
            SqlConnection scon = new SqlConnection(strcnn);
            try
            { 
                scon.Open(); SqlCommand scmd;
                scmd = new SqlCommand(string.Format("update VEHICULE set DSRVeh=GETDATE() , StatVeh = 'Livré' where  VEHICULE.IdBS={0} and VEHICULE.DSRVeh is null", idbs), scon);
                scmd.ExecuteNonQuery();
                return "ok";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            finally {
                scon.Close();
            }
        }

        [WebMethod]
        public string CheckSortie(string idbs)
        {
            try
            { 
                string strcnn = @"Data Source=192.168.0.28\SVR2012;Initial Catalog=NAVY;User ID=sa;Password=P@ssw0rd2012";
                SqlConnection scon = new SqlConnection(strcnn);
                scon.Open(); SqlCommand scmd;
                scmd = new SqlCommand(string.Format("select VEHICULE.NumChassis, VEHICULE.DescVeh , CONNAISSEMENT.NumBL from BON_SORTIE inner join CONNAISSEMENT on BON_SORTIE.idbl=CONNAISSEMENT.IdBL inner join VEHICULE on VEHICULE.IdBS=BON_SORTIE.IdBS where BON_SORTIE.IdBS={0}", idbs), scon);
                SqlDataReader dr = null;
                dr = scmd.ExecuteReader();
                string chassi = string.Empty; string descveh = string.Empty; string numbl=string.Empty;
                StringBuilder sb = new StringBuilder();
                try
                {
                    while (dr.Read())
                    {
                         
                        if (!(dr.IsDBNull(dr.GetOrdinal("NumChassis"))))
                        {
                            chassi = dr.GetString(dr.GetOrdinal("NumChassis"));
                        }

                        if (!(dr.IsDBNull(dr.GetOrdinal("DescVeh"))))
                        {
                            descveh = dr.GetString(dr.GetOrdinal("DescVeh"));
                        }

                        if (!(dr.IsDBNull(dr.GetOrdinal("NumBL"))))
                        {
                           numbl = dr.GetString(dr.GetOrdinal("NumBL"));
                        }
                        sb.Append(string.Format("Chassis : {0} \n Vehicule : {1} \n", chassi, descveh));
                    }
                    //
                    return "BL: "+numbl+";\n"+sb.ToString() ;
                }
                catch(Exception ex)
                {
                    return "0 : "+ex.Message;
                }
                finally{
                    if (dr != null) dr.Close();
                    scon.Close();
                }
                
            }
            catch (Exception ex)
            {
                return "1 : "+ex.Message;
            }
        }

    }

}