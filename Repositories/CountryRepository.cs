using System.Data.SqlClient;
using System.Data;                
using TravelCoServer.Models;      
using TravelCoServer.Repositories.Data;  

namespace TravelCoServer.Repositories
{
    public class CountryRepository
    {
        private readonly DbHelper _db;
        public CountryRepository(DbHelper db)   
        {
            _db = db;
        }

        public List<Country> GetAll()
        {
            List<Country> countries = new List<Country>();

            using (SqlConnection conn = _db.GetConnection())
            {
                conn.Open();
               
                SqlCommand cmd = new SqlCommand("TravelCo_sp_Country_GetAll", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Country c = new Country();
                    c.Code = reader["Code"].ToString();
                    c.Name = reader["Name"].ToString();
                    c.Capital = reader["Capital"] as string;
                    c.Region = reader["Region"] as string;
                    c.Population = Convert.ToInt64(reader["Population"]);
                    c.Area = Convert.ToDouble(reader["Area"]);
                    c.Flag = reader["Flag"] as string;

                    countries.Add(c);
                }
            }
            return countries;

        }

        public Country? GetByCode(string code)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("TravelCo_sp_Country_GetByCode", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Code", code);

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())            // one row expected
                {
                    Country c = new Country();
                    c.Code = reader["Code"].ToString();
                    c.Name = reader["Name"].ToString();
                    c.Capital = reader["Capital"] as string;
                    c.Region = reader["Region"] as string;
                    c.Population = Convert.ToInt64(reader["Population"]);
                    c.Area = Convert.ToDouble(reader["Area"]);
                    c.Flag = reader["Flag"] as string;
                    return c;
                }
                return null;                  // no country found
            }
        }
    }
}
