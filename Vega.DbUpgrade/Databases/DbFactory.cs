using Vega.DbUpgrade.DbProviders;
using Vega.DbUpgrade.Interfaces;

namespace Vega.DbUpgrade.Databases
{
    /// <summary>
    /// Presents factory provider for databases.
    /// </summary>
    public static class DbFactory
    {
        #region [Public Methods]
        /// <summary>
        /// Gets the database.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <returns>Returns concrete database.</returns>
        public static IDatabase GetDatabase(IDbProvider provider)
        {
            IDatabase retVal = null;

            if (provider is MsSqlDbProvider)
            {
                retVal = new MsSqlDatabase();
            }
            else if (provider is MySqlDbProvider)
            {
                retVal = new MySqlDatabase();
            }
            else if (provider is FireBirdDbProvider)
            {
                retVal = new FirebirdDatabase();
            }

            return retVal;
        }
        #endregion
    }
}