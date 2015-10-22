using System.Data;
using FirebirdSql.Data.FirebirdClient;

namespace Vega.DbUpgrade.Databases
{
    /// <summary>
    /// Concrete implementation for Firebird database.
    /// </summary>
    public class FirebirdDatabase : Database
    {
        #region [Public Methods]
        
        public override IDbConnection GetDbConnection()
        {
            return new FbConnection(DbConnectionString);
        }

        public override IDbCommand GetDbCommand()
        {
            return new FbCommand();
        }

        public override IDbCommand GetDbCommand(string commandText, IDbConnection connection)
        {
            return new FbCommand(commandText, (FbConnection)connection);
        }

        #endregion
    }
}