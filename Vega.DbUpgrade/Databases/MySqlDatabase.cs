using System.Data;
using MySql.Data.MySqlClient;

namespace Vega.DbUpgrade.Databases
{
    /// <summary>
    /// Concrete implementation for MySQL database.
    /// </summary>
    public class MySqlDatabase : Database
    {
        #region [Public Methods]
        
        public override IDbConnection GetDbConnection()
        {
            return new MySqlConnection(DbConnectionString);
        }

        public override IDbCommand GetDbCommand()
        {
            return new MySqlCommand();
        }

        public override IDbCommand GetDbCommand(string commandText, IDbConnection connection)
        {
            return new MySqlCommand(commandText, (MySqlConnection)connection);
        }

        #endregion
    }
}
