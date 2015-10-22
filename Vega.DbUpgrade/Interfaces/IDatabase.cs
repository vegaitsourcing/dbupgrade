using System.Data;

namespace Vega.DbUpgrade.Interfaces
{
    public interface IDatabase
    {
        /// <summary>
        /// Gets the db connection string.
        /// </summary>
        /// <value>The db connection string.</value>
        string DbConnectionString { get; }

        /// <summary>
        /// Gets the db connection.
        /// </summary>
        /// <returns>Database connection.</returns>
        IDbConnection GetDbConnection();

        /// <summary>
        /// Gets the db command.
        /// </summary>
        /// <returns>Database command.</returns>
        IDbCommand GetDbCommand();

        /// <summary>
        /// Gets the db command.
        /// </summary>
        /// <param name="commandText">The command text.</param>
        /// <param name="connection">The connection.</param>
        /// <returns>Database command.</returns>
        IDbCommand GetDbCommand(string commandText, IDbConnection connection);
    }
}