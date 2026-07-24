using System.Data.SqlClient;

namespace TravelCoServer.Repositories.Data   
{
    public class DbHelper
    {
        // a private field to hold the connection string
        private readonly string _connectionString;

        // a constructor that reads the connection string from the configuration and stores it in the private field
        public DbHelper(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("myProjDB");
        }

        // a public method that returns a new SqlConnection
        public SqlConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}
