using System.Data.SqlClient;
using System.Data;                
using TravelCoServer.Models;      
using TravelCoServer.Repositories.Data;  

namespace TravelCoServer.Repositories
{
    public class CountryRepository
    {
        private readonly DbHelper _db;
        public CountryRepository(DbHelper db)   // DI gives us DbHelper
        {
            _db = db;
        }

        public List<Country> GetAll()
        {
            // 1. make an empty List<Country>
            List<Country> countries = new List<Country>();

            // 2. get a connection from _db and open it
            using (SqlConnection conn = _db.GetConnection())
            {
                conn.Open();
                // steps 3-4 go inside here
                // 3. make a SqlCommand for "TravelCo_sp_Country_GetAll",
                //    set CommandType = CommandType.StoredProcedure
                SqlCommand cmd = new SqlCommand("TravelCo_sp_Country_GetAll", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                // 4. ExecuteReader; while (reader.Read()) { map one row -> Country; add to list }
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
    }
}
