using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Sql;
using System.Data.SqlClient;
namespace SageInvoiceUpd
{
    class Program
    {
        static void Main(string[] args)
        {
            SqlConnection con = new SqlConnection();
            con.ConnectionString = @"Data Source=192.168.0.28\SVR2012;Initial Catalog=SOCOMARTESTING; User id=sa; Password=Passw0rd2012;";
            con.Open();
            SqlCommand cmd = new SqlCommand("select * from F_ECRITUREC where ", con);
            SqlDataReader dr = null;
            dr = cmd.ExecuteReader();

        }
    }
}
