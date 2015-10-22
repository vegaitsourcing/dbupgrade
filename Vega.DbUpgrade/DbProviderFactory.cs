using System;
using System.Configuration;
using Vega.DbUpgrade.Databases;
using Vega.DbUpgrade.DbProviders;
using Vega.DbUpgrade.Interfaces;
using Vega.DbUpgrade.Utilities;

namespace Vega.DbUpgrade
{
    /// <summary>
    /// Database provider factory.
    /// </summary>
    internal static class DbProviderFactory
    {
        /// <summary>
        /// Gets database provider.
        /// </summary>
        /// <returns><see cref="Vega.DbUpgrade.Interfaces.IDbProvider"/> object.</returns>
        public static IDbProvider GetDbProvider()
        {
            IDbProvider retVal = null;

            string providerName = ConfigurationManager.AppSettings[Constants.AppSettingKeys.DatabaseProvider];
            if (!String.IsNullOrEmpty(providerName))
            {
                if (Enum.IsDefined(typeof(DBProviders), providerName.ToLower()))
                {
                    var provider = (DBProviders)Enum.Parse(typeof(DBProviders), providerName, true);
                    switch (provider)
                    {
                        case DBProviders.mssql:
                            retVal = new MsSqlDbProvider(new MsSqlDatabase());
                            break;
                        case DBProviders.mysql:
                            retVal = new MySqlDbProvider(new MySqlDatabase());
                            break;
                        case DBProviders.firebird:
                            retVal = new FireBirdDbProvider(new FirebirdDatabase());
                            break;
                    }
                }
            }

            return retVal;
        }
    }
}