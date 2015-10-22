using System.Data;
using System.Data.SqlClient;

namespace Vega.DbUpgrade.Databases
{   
    /// <summary>
    /// Concrete implementation for MSSQL database.
    /// </summary>
    public class MsSqlDatabase : Database
    {
        #region [Public Methods]
        
        public override IDbConnection GetDbConnection()
        {
            return new SqlConnection(DbConnectionString);
        }
        
        public override IDbCommand GetDbCommand()
        {
            return new SqlCommand();
        }

        public override IDbCommand GetDbCommand(string commandText, IDbConnection connection)
        {
            return new SqlCommand(commandText, (SqlConnection) connection);
        }

        #endregion
    }
}