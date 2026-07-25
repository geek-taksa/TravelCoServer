using System.Data;
using System.Data.SqlClient;
using TravelCoServer.Models;
using TravelCoServer.Repositories.Data;

namespace TravelCoServer.Repositories
{
    public class ListRepository
    {
        // CONSTRUCTOR
        private readonly DbHelper _db;
        public ListRepository(DbHelper db) { _db = db; }

        // METHODS
        public UserLists GetLists(int userId)
        {
            UserLists result = new UserLists();
            using (SqlConnection conn = _db.GetConnection())
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("TravelCo_sp_List_Get", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserId", userId);

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

                    string type = reader["ListType"].ToString();
                    if (type == "visited")
                        result.Visited.Add(c);
                    else
                        result.Wishlist.Add(c);
                }
            }
            return result;
        }

        public void Add(int userId, string countryCode, string listType)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("TravelCo_sp_List_Add", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@CountryCode", countryCode);
                cmd.Parameters.AddWithValue("@ListType", listType);
                cmd.ExecuteNonQuery();          
            }
        }

        public void Remove(int userId, string countryCode)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("TravelCo_sp_List_Remove", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@CountryCode", countryCode);
                cmd.ExecuteNonQuery();          
            }
        }

        public void Move(int userId, string countryCode, string newListType)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("TravelCo_sp_List_Move", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@CountryCode", countryCode);
                cmd.Parameters.AddWithValue("@ToType", newListType);
                cmd.ExecuteNonQuery();          
            }
        }
    }
}
