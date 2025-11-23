using Microsoft.Data.SqlClient;

namespace NinhBinhStore.DAO
{
    public class DBContext
    {
        private readonly string _connectionString;

        // Lấy chuỗi kết nối từ appsettings.json thông qua constructor
        public DBContext(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public SqlConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}
