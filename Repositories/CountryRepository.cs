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

                // Result set 1: the country
                Country? c = null;
                if (reader.Read())
                {
                    c = new Country();
                    c.Code = reader["Code"].ToString();
                    c.Name = reader["Name"].ToString();
                    c.Capital = reader["Capital"] as string;
                    c.Region = reader["Region"] as string;
                    c.Population = Convert.ToInt64(reader["Population"]);
                    c.Area = Convert.ToDouble(reader["Area"]);
                    c.Flag = reader["Flag"] as string;
                }
                if (c == null) return null;

                // Result set 2: languages
                reader.NextResult();
                while (reader.Read())
                    c.Languages.Add(reader["Language"].ToString());

                // Result set 3: currencies
                reader.NextResult();
                while (reader.Read())
                    c.Currencies.Add(reader["Currency"].ToString());

                return c;
            }
        }

        //CRUD:
        public void Create(Country c)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("TravelCo_sp_Country_Create", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Code", c.Code);
                cmd.Parameters.AddWithValue("@Name", c.Name);
                cmd.Parameters.AddWithValue("@Capital", (object?)c.Capital ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Region", (object?)c.Region ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Population", c.Population);
                cmd.Parameters.AddWithValue("@Area", c.Area);
                cmd.Parameters.AddWithValue("@Flag", (object?)c.Flag ?? DBNull.Value);
                cmd.ExecuteNonQuery();
            }
        }

        public int Update(Country c)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("TravelCo_sp_Country_Update", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Code", c.Code);
                cmd.Parameters.AddWithValue("@Name", c.Name);
                cmd.Parameters.AddWithValue("@Capital", (object?)c.Capital ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Region", (object?)c.Region ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Population", c.Population);
                cmd.Parameters.AddWithValue("@Area", c.Area);
                cmd.Parameters.AddWithValue("@Flag", (object?)c.Flag ?? DBNull.Value);
                return cmd.ExecuteNonQuery();   // rows affected
            }
        }

        public int Delete(string code)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("TravelCo_sp_Country_Delete", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Code", code);
                return cmd.ExecuteNonQuery();
            }
        }

        // METHODS for countries.dev
        public void Upsert(Country c)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("TravelCo_sp_Country_Upsert", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Code", c.Code);
                cmd.Parameters.AddWithValue("@Name", c.Name);
                cmd.Parameters.AddWithValue("@Capital", (object?)c.Capital ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Region", (object?)c.Region ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Population", c.Population);
                cmd.Parameters.AddWithValue("@Area", c.Area);
                cmd.Parameters.AddWithValue("@Flag", (object?)c.Flag ?? DBNull.Value);
                cmd.ExecuteNonQuery();
            }
        }

        public void ClearLanguages(string code)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("TravelCo_sp_CountryLanguages_Clear", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CountryCode", code);
                cmd.ExecuteNonQuery();
            }
        }

        public void AddLanguage(string code, string language)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("TravelCo_sp_CountryLanguage_Add", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CountryCode", code);
                cmd.Parameters.AddWithValue("@Language", language);
                cmd.ExecuteNonQuery();
            }
        }

        public void ClearCurrencies(string code)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("TravelCo_sp_CountryCurrencies_Clear", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CountryCode", code);
                cmd.ExecuteNonQuery();
            }
        }

        public void AddCurrency(string code, string currency)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("TravelCo_sp_CountryCurrency_Add", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CountryCode", code);
                cmd.Parameters.AddWithValue("@Currency", currency);
                cmd.ExecuteNonQuery();
            }
        }

        public Dictionary<string, int> GetRegionCounts()
        {
            Dictionary<string, int> counts = new Dictionary<string, int>();
            using (SqlConnection conn = _db.GetConnection())
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("TravelCo_sp_Country_RegionCounts", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    counts[reader["Region"].ToString()] = Convert.ToInt32(reader["Count"]);
                }
            }
            return counts;
        }

        public List<Country> GetCountries(string? search, string? region, string? language,
                                      string? currency, string? sort, string? order)
        {
            List<Country> countries = new List<Country>();
            using (SqlConnection conn = _db.GetConnection())
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("TravelCo_sp_Country_List", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Search", (object?)search ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Region", (object?)region ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Language", (object?)language ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Currency", (object?)currency ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Sort", string.IsNullOrEmpty(sort) ? "name" : sort);
                cmd.Parameters.AddWithValue("@Order", string.IsNullOrEmpty(order) ? "asc" : order);

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
