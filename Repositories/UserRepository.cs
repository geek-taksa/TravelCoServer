using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography.X509Certificates;
using TravelCoServer.Models;
using TravelCoServer.Repositories.Data;

namespace TravelCoServer.Repositories
{
    public class UserRepository
    {
        private readonly DbHelper _db;
        public UserRepository(DbHelper db) { _db = db; }

        public int Create(User user)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("TravelCo_sp_User_Create", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Username", user.Username);
                cmd.Parameters.AddWithValue("@Email", user.Email);
                cmd.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
                cmd.Parameters.AddWithValue("@PasswordSalt", user.PasswordSalt);

                object result = cmd.ExecuteScalar();   // the SCOPE_IDENTITY value
                return Convert.ToInt32(result);        // the new user's Id
            }
        }

        public User? GetByEmail(string email)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("TravelCo_sp_User_GetByEmail", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Email", email);

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    User u = new User();
                    // map all 9 columns — see the conversions below
                    u.Id = Convert.ToInt32(reader["Id"]);
                    u.Username = reader["Username"].ToString();
                    u.Email = reader["Email"].ToString();
                    u.PasswordHash = reader["PasswordHash"].ToString();
                    u.PasswordSalt = reader["PasswordSalt"].ToString();
                    u.Role = reader["Role"].ToString();
                    u.IsLocked = Convert.ToBoolean(reader["IsLocked"]);
                    u.CanShare = Convert.ToBoolean(reader["CanShare"]);
                    u.CreatedAt = Convert.ToDateTime(reader["CreatedAt"]);

                    return u;
                }
                return null;
            }
        }

        public User? GetById(int id)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("TravelCo_sp_User_GetById", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", id);

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    User u = new User();
                    // map all 9 columns — see the conversions below
                    u.Id = Convert.ToInt32(reader["Id"]);
                    u.Username = reader["Username"].ToString();
                    u.Email = reader["Email"].ToString();
                    u.PasswordHash = reader["PasswordHash"].ToString();
                    u.PasswordSalt = reader["PasswordSalt"].ToString();
                    u.Role = reader["Role"].ToString();
                    u.IsLocked = Convert.ToBoolean(reader["IsLocked"]);
                    u.CanShare = Convert.ToBoolean(reader["CanShare"]);
                    u.CreatedAt = Convert.ToDateTime(reader["CreatedAt"]);

                    return u;
                }
                return null;
            }
        }

        public void AddLoginEvent(int userId)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("TravelCo_sp_LoginEvent_Add", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.ExecuteNonQuery();
            }
        }

        // METHODS for profile & preferences management
        public void SetContinents(int userId, List<string> continents)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                conn.Open();

                SqlCommand clear = new SqlCommand("TravelCo_sp_UserContinents_Clear", conn);
                clear.CommandType = CommandType.StoredProcedure;
                clear.Parameters.AddWithValue("@UserId", userId);
                clear.ExecuteNonQuery();

                if (continents != null)
                {
                    foreach (string cont in continents)
                    {
                        SqlCommand add = new SqlCommand("TravelCo_sp_UserContinent_Add", conn);
                        add.CommandType = CommandType.StoredProcedure;
                        add.Parameters.AddWithValue("@UserId", userId);
                        add.Parameters.AddWithValue("@Continent", cont);
                        add.ExecuteNonQuery();
                    }
                }
            }
        }

        public List<LanguagePref> GetLanguages(int userId)
        {
            List<LanguagePref> list = new List<LanguagePref>();
            using (SqlConnection conn = _db.GetConnection())
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("TravelCo_sp_UserLanguages_Get", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserId", userId);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(new LanguagePref
                    {
                        Name = reader["LanguageName"].ToString(),
                        Level = reader["Level"].ToString()
                    });
                }
            }
            return list;
        }

        public void SetLanguages(int userId, List<LanguagePref> languages)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                conn.Open();
                SqlCommand clear = new SqlCommand("TravelCo_sp_UserLanguages_Clear", conn);
                clear.CommandType = CommandType.StoredProcedure;
                clear.Parameters.AddWithValue("@UserId", userId);
                clear.ExecuteNonQuery();
                if (languages != null)
                {
                    foreach (var lang in languages)
                    {
                        SqlCommand add = new SqlCommand("TravelCo_sp_UserLanguage_Add", conn);
                        add.CommandType = CommandType.StoredProcedure;
                        add.Parameters.AddWithValue("@UserId", userId);
                        add.Parameters.AddWithValue("@LanguageName", lang.Name);
                        add.Parameters.AddWithValue("@Level", lang.Level);
                        add.ExecuteNonQuery();
                    }
                }
            }
        }

        public List<string> GetContinents(int userId)
        {
            List<string> list = new List<string>();
            using (SqlConnection conn = _db.GetConnection())
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("TravelCo_sp_UserContinents_Get", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserId", userId);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(reader["Continent"].ToString());
                }
            }
            return list;
        }

        public void UpdateProfile(int userId, string username, string email)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("TravelCo_sp_User_Update", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", userId);
                cmd.Parameters.AddWithValue("@Username", username);
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.ExecuteNonQuery();
            }
        }
    }
}
