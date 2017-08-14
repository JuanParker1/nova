using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Sql;
using System.Data.SqlClient;

namespace VSOM_TOOLS
{
    class CTR
    {
        public string NumCtr { get; set; }
        public int IdCtr { get; set; }
        public string SensCtr { get; set; }
        public int IdExport { get; set; }

    }
    class Program
    {
        static void Main(string[] args)
        {
            SqlConnection con = new SqlConnection();
            con.ConnectionString = @"Data Source=192.168.0.28;Initial Catalog=VSOMBD2; User id=sa; Password=Passw0rd;";
            con.Open();
            SqlCommand cmd;
            

            Console.WriteLine("1-Correction routine \n 2-Changement statut booking \n ");
            string ans = Console.ReadLine();
            if (ans == "1")
            {
                Console.WriteLine("Saisir le numéro de conteneur:");
                string ctr = Console.ReadLine();

                #region recupération des  conteneur export
                 cmd = new SqlCommand(string.Format("select NumCtr, IdCtr, SensCtr from CONTENEUR where NumCtr like '{0}' and SensCtr='E' ", ctr), con);
                SqlDataReader dr = null;
                dr = cmd.ExecuteReader();
                CTR _ctr; List<CTR> lst1 = new List<CTR>(); ;
                try
                {
                    while (dr.Read())
                    {
                        _ctr = new CTR();
                        if (!(dr.IsDBNull(dr.GetOrdinal("IdCtr"))))
                        {
                            _ctr.IdCtr = dr.GetInt32(dr.GetOrdinal("IdCtr"));
                        }

                        if (!(dr.IsDBNull(dr.GetOrdinal("NumCtr"))))
                        {
                            _ctr.NumCtr = dr.GetString(dr.GetOrdinal("NumCtr"));
                        }

                        if (!(dr.IsDBNull(dr.GetOrdinal("SensCtr"))))
                        {
                            _ctr.SensCtr = dr.GetString(dr.GetOrdinal("SensCtr"));
                        }
                        lst1.Add(_ctr);

                    }
                    Console.WriteLine("DONE: Récupération conteneur");
                }
                catch (Exception ex)
                {

                    Console.WriteLine("une erreur lors de la recupération des conteneur : " + ex.Message);
                    // Console.ReadLine();

                }
                finally
                {
                    if (dr != null) dr.Close();
                } 
                #endregion

                #region recupération conteneurTC
                 cmd = new SqlCommand(string.Format("select NumTC, IdTC, IdCtrExport from CONTENEUR_TC where NumTC like '{0}' ", ctr), con);
                SqlDataReader dr2 = null;
                dr2 = cmd.ExecuteReader();
                CTR _ctr2; List<CTR> lst2 = new List<CTR>(); ;
                try
                {
                    while (dr2.Read())
                    {
                        _ctr2 = new CTR();
                        if (!(dr.IsDBNull(dr2.GetOrdinal("IdTC"))))
                        {
                            _ctr2.IdCtr = dr2.GetInt32(dr.GetOrdinal("IdTC"));
                        }

                        if (!(dr.IsDBNull(dr2.GetOrdinal("NumTC"))))
                        {
                            _ctr2.NumCtr = dr2.GetString(dr.GetOrdinal("NumTC"));
                        }

                        if (!(dr.IsDBNull(dr2.GetOrdinal("IdCtrExport"))))
                        {
                            _ctr2.IdExport = dr2.GetInt32(dr.GetOrdinal("IdCtrExport"));
                        }
                        lst2.Add(_ctr2);

                    }
                    Console.WriteLine("DONE : Récupération conteneur_TC");
                }
                catch (Exception ex)
                {

                    Console.WriteLine("une erreur lors de la recupération des conteneur_tc : " + ex.Message);
                    // Console.ReadLine();

                }
                finally
                {
                    if (dr != null) dr.Close();
                }
                #endregion

                if (lst1.Count > 0 && lst2.Count > 0)
                {
                    //recupéraion du conteneur export 
                    CTR _exportCTR = lst1.OrderByDescending(p => p.IdCtr).First();
                    //conteneur TC
                    CTR _tc = lst2.OrderByDescending(p => p.IdCtr).First();

                    //MAJ colonne idexport
                    cmd = new SqlCommand(string.Format("update conteneur_tc set idctrexport={0} where idtc={1}", _exportCTR.IdCtr, _tc.IdCtr));
                    int nbr = cmd.ExecuteNonQuery();
                    Console.WriteLine("Correction effectuée"); 
                }
                else
                {
                    Console.WriteLine("opération impossible");
                }
            }

            if (ans == "2")
            {
                //Console.WriteLine("Entrer le numero du bl export");
                //string ctr2 = Console.ReadLine();
                //cmd = new SqlCommand(string.Format("select NumTC, IdTC, IdCtrExport from CONTENEUR_TC where NumTC like '{0}' ", ctr), con);
                //SqlDataReader dr3 = null;
                //dr3 = cmd.ExecuteReader();

            }


        }
    }
}
