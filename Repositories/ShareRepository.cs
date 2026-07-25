using System.Data;
using System.Data.SqlClient;
using TravelCoServer.Models;
using TravelCoServer.Repositories.Data;

namespace TravelCoServer.Repositories
{
    public class ShareRepository
    {
        private readonly DbHelper _db;
        public ShareRepository(DbHelper db) { _db = db; }

        public List<Share> GetAll(string? countryCode)
        {
            List<Share> shares = new List<Share>();
            using (SqlConnection conn = _db.GetConnection())
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("TravelCo_sp_Share_GetAll", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                // pass the value, or DBNull if it's null (that triggers the proc's "= NULL" default behavior)
                cmd.Parameters.AddWithValue("@CountryCode", (object?)countryCode ?? DBNull.Value);

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Share s = new Share();
                    s.Id = Convert.ToInt32(reader["Id"]);
                    s.UserId = Convert.ToInt32(reader["UserId"]);
                    s.CountryCode = reader["CountryCode"].ToString();
                    s.CountryName = reader["CountryName"].ToString();
                    s.Type = reader["Type"].ToString();
                    s.Title = reader["Title"].ToString();
                    s.Body = reader["Body"].ToString();
                    s.Author = reader["Author"].ToString();
                    s.CreatedAt = Convert.ToDateTime(reader["CreatedAt"]);
                    shares.Add(s);
                }
            }
            return shares;
        }

        // create / update / delete below
        public int Create(int userId, ShareRequest req)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("TravelCo_sp_Share_Create", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@CountryCode", req.CountryCode);
                cmd.Parameters.AddWithValue("@Type", req.Type);
                cmd.Parameters.AddWithValue("@Title", req.Title);
                cmd.Parameters.AddWithValue("@Body", req.Body);
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        public int Update(int id, int userId, ShareRequest req)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("TravelCo_sp_Share_Update", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@Type", req.Type);
                cmd.Parameters.AddWithValue("@Title", req.Title);
                cmd.Parameters.AddWithValue("@Body", req.Body);
                return cmd.ExecuteNonQuery();   // rows affected
            }
        }

        public int Delete(int id, int userId)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("TravelCo_sp_Share_Delete", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.Parameters.AddWithValue("@UserId", userId);
                return cmd.ExecuteNonQuery();   // rows affected
            }
        }
    }
}
