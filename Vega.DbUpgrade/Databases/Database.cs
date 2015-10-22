using System.Configuration;
using System.Data;
using Vega.DbUpgrade.Interfaces;
using Vega.DbUpgrade.Utilities;

namespace Vega.DbUpgrade.Databases
{
    /// <summary>
    /// Presents abstract class for database.
    /// </summary>
    public abstract class Database : IDatabase
    {
        #region [Members]
        
        private readonly string _connectionString = ConfigurationManager.AppSettings[Constants.AppSettingKeys.ConnectionString];

        #endregion

        #region [Properties]
        
        public string DbConnectionString
        {
            get { return _connectionString; }
        }
        #endregion

        #region [Abstract Methods]
        
        public abstract IDbConnection GetDbConnection();

        public abstract IDbCommand GetDbCommand();

        public abstract IDbCommand GetDbCommand(string commandText, IDbConnection connection);

        #endregion
    }
}
