using System;

namespace Vega.DbUpgrade.Interfaces
{
    /// <summary>
    /// Presents contract for database providers.
    /// </summary>
    public interface IDbProvider
    {
        /// <summary>
        /// Gets sql script for creation of ChangeLog database table.
        /// </summary>
        /// <returns>The sql script that represents the ChangeLog table.</returns>
        string GetChangeLogTableScript();

        /// <summary>
        /// Executes script on database.
        /// </summary>
        /// <param name="fileContent">SQL script.</param>
        /// <returns>Returns <code>true</code> if script is successfully executed, otherwise <code>false</code>.</returns>
        bool ExecuteScript(string fileContent);

        /// <summary>
        /// Executes script on database and updates ChangeLog table.
        /// </summary>
        /// <param name="fileId">Unique identifier of script file.</param>
        /// <param name="fileContent">SQL script.</param>
        /// <param name="filePath">Script's file path on file system.</param>
        /// <returns>Returns <code>true</code> if script is successfully executed, otherwise <code>false</code>.</returns>
        bool ExecuteScript(Guid fileId, string fileContent, string filePath);

        /// <summary>
        /// Checks if sql script is already executed.
        /// </summary>
        /// <param name="fileId">ID of a change script.</param>
        /// <returns>Returns <code>true</code> if script is successfully executed, otherwise <code>false</code>.</returns>
        bool IsScriptExecuted(Guid fileId);
    }
}
