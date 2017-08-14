using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Data.Sql;
using System.Data.SqlClient;
namespace HashtUserPwd
{
    public class user
    {
        public int id; public string mpu; public string lu;

    }
    class Program
    {
        static void Main(string[] args)
        {
             
           SqlConnection con = new SqlConnection();
           con.ConnectionString = @"Data Source=192.168.0.28;Initial Catalog=VSOMBD2; User id=sa; Password=Passw0rd;";
           con.Open();
           SqlCommand cmd = new SqlCommand("select * from utilisateur ", con);
            SqlDataReader dr =null;
            dr= cmd.ExecuteReader();
           // int id=0; string lu; string mpu=string.Empty; 
            PwdHash h;
            string hash=string.Empty;
            SqlCommand cmdupd; user u; List<user> lst = new List<user>();
            
            try
            { 
                while (dr.Read())
                {
                    u = new user();
                    if (!(dr.IsDBNull(dr.GetOrdinal("idu"))))
                    {
                        u.id = dr.GetInt32(dr.GetOrdinal("idu"));
                    }

                    if (!(dr.IsDBNull(dr.GetOrdinal("nu"))))
                    {
                        u.lu = dr.GetString(dr.GetOrdinal("nu"));
                    }

                    if (!(dr.IsDBNull(dr.GetOrdinal("mpu"))))
                    {
                        u.mpu = dr.GetString(dr.GetOrdinal("mpu"));
                    }
                    lst.Add(u);
                    
                    /*h = new PwdHash(u.mpu);
                    hash=h.Decrypt();
                    Console.WriteLine(string.Format(" id :{0} ; mpu :{1}  ; hash :{2} ;  lu : {3}", u.id, u.mpu, hash,u.lu));
                    */
                }
                Console.WriteLine("fin création de la liste de utilisateur");
            }
            catch (Exception ex)
            {

                Console.WriteLine("une erreur lors de l'execution : " + ex.Message);
               // Console.ReadLine();
                 
            }
            finally
            {
                if (dr != null) dr.Close();
            }
             
            Console.ReadLine();
            if (lst.Count > 0)
            {
                Console.WriteLine("1-pour cryptage des pwd \n 2-pour decrypter les pwd");
                string ans = Console.ReadLine();
                if (ans=="1")
                {

                    #region update hashed pwd

                    SqlTransaction trans = con.BeginTransaction();
                    string sql = string.Empty;
                    try
                    {
                        foreach (user item in lst)
                        {
                            h = new PwdHash(item.mpu);
                            hash = h.Encrypt();
                            sql = string.Format(@"update utilisateur set mpu=@pwd where idu={1}", hash.ToString(), item.id);
                            Console.WriteLine(string.Format(" id :{0} ; mpu :{1}  ; hash :{2} ", item.id, item.mpu, hash));
                            cmdupd = new SqlCommand("update utilisateur set mpu=@pwd where idu=@id", con, trans);
                            cmdupd.Parameters.AddWithValue("@pwd", hash.ToString());
                            cmdupd.Parameters.AddWithValue("@id", item.id);
                            cmdupd.ExecuteNonQuery();

                        }
                        trans.Commit();
                        Console.WriteLine("cryptage ok");
                    }
                    catch (Exception ex)
                    {

                        trans.Rollback();
                        Console.WriteLine("erreur " + ex.Message);
                    }
                    finally
                    {
                        trans.Dispose();
                    }

                    #endregion

                }

                if (ans=="2")
                {
                    #region update hashed pwd

                    SqlTransaction trans = con.BeginTransaction();
                    string sql = string.Empty;
                    try
                    {
                        foreach (user item in lst)
                        {
                            h = new PwdHash(item.mpu);
                            hash = h.Decrypt();
                            sql = string.Format(@"update utilisateur set mpu='{0}' where idu={1}", hash, item.id);
                            Console.WriteLine(string.Format(" login :{0} ; mpu :{1}  ; hash :{2} nom : ", item.lu, item.mpu, hash));
                            cmdupd = new SqlCommand(sql, con, trans);
                            cmdupd.ExecuteNonQuery();

                        }
                        trans.Commit();
                        Console.WriteLine("decryptage ok pour ");
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        Console.WriteLine("erreur " + ex.Message);
                    }
                    finally
                    {
                        trans.Dispose();
                    }

                    #endregion
                }
                
            }

            con.Close();
            con.Dispose();

            Console.ReadLine();
            
        }
    }
}
