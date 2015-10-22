using System.Data;
using MySql.Data.MySqlClient;
using Vega.DbUpgrade.Interfaces;

namespace Vega.DbUpgrade.Databases
{
    /// <summary>
    /// Concrete implementation for MySQL database.
    /// </summary>
    public class MySqlDatabase : IDatabase
    {
        public string DbConnectionString { get; set; }

        #region [Public Methods]
        
        public IDbConnection GetDbConnection()
        {
            return new MySqlConnection(DbConnectionString);
        }

        public IDbCommand GetDbCommand()
        {
            return new MySqlCommand();
        }

        public IDbCommand GetDbCommand(string commandText, IDbConnection connection)
        {
            return new MySqlCommand(commandText, (MySqlConnection)connection);
        }

        #endregion
    }
}
