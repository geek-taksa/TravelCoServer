using System.Data;
using System.Data.SqlClient;
using TravelCoServer.Models;
using TravelCoServer.Repositories.Data;

namespace TravelCoServer.Repositories
{
    public class AdminRepository
    {
        private readonly DbHelper _db;
        public AdminRepository(DbHelper db) { _db = db; }

        public List<AdminUserDto> GetUsers()
        {
            List<AdminUserDto> users = new List<AdminUserDto>();
            using (SqlConnection conn = _db.GetConnection())
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("TravelCo_sp_Admin_Users", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    AdminUserDto u = new AdminUserDto();
                    u.Id = Convert.ToInt32(reader["Id"]);
                    u.Username = reader["Username"].ToString();
                    u.Email = reader["Email"].ToString();
                    u.Role = reader["Role"].ToString();
                    u.IsLocked = Convert.ToBoolean(reader["IsLocked"]);
                    u.CanShare = Convert.ToBoolean(reader["CanShare"]);
                    u.CreatedAt = Convert.ToDateTime(reader["CreatedAt"]);
                    users.Add(u);
                }
            }
            return users;
        }

        public AdminStats GetStats()
        {
            AdminStats stats = new AdminStats();
            using (SqlConnection conn = _db.GetConnection())
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("TravelCo_sp_Admin_Stats", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    stats.DailyLogins = Convert.ToInt32(reader["DailyLogins"]);
                    stats.CountriesImported = Convert.ToInt32(reader["CountriesImported"]);
                    stats.CountriesSaved = Convert.ToInt32(reader["CountriesSaved"]);
                    stats.SharesCreated = Convert.ToInt32(reader["SharesCreated"]);
                }
            }
            return stats;
        }

        public void SetUserFlags(int id, bool isLocked, bool canShare)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("TravelCo_sp_Admin_SetUserFlags", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.Parameters.AddWithValue("@IsLocked", isLocked);
                cmd.Parameters.AddWithValue("@CanShare", canShare);
                cmd.ExecuteNonQuery();
            }
        }
    }
}
