using MySql.Data.MySqlClient;
using System.Configuration;
using System.Data.SqlClient;

namespace ScrumMvp.Data
{
    // Fábrica de conexiones a la MySQL/MariaDB de Plesk.
    // La cadena "ScrumDb" se define en Web.config.
    public static class Db
    {
        public static MySqlConnection GetConnection()
        {
            string cs = ConfigurationManager.ConnectionStrings["ScrumDb"].ConnectionString;
            return new MySqlConnection(cs);
        }
    }
}