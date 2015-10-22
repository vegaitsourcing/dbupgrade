namespace Vega.DbUpgrade.Utilities
{
    /// <summary>
    /// Provides database provider that DbUpgrade supports.
    /// </summary>
    public enum DBProviders
    {
        /// <summary>
        /// Microsoft SQL Database.
        /// </summary>
        mssql,

        /// <summary>
        /// My SQL Database.
        /// </summary>
        mysql,

        /// <summary>
        /// FireBird Database.
        /// </summary>
        firebird
    }

    /// <summary>
    /// Return statuses of DbUpgrader application.
    /// </summary>
    public enum DbUpgraderStatus
    {
        /// <summary>
        /// Database successfully updated.
        /// </summary>
        Success,
        
        /// <summary>
        /// Error during database update.
        /// </summary>
        Error,

        /// <summary>
        /// Folder with SQL scripts doesn't exist.
        /// </summary>
        NonExistingScriptsFolder,

        /// <summary>
        /// Database is unknown.
        /// </summary>
        UnknownDatabase
    }
}
