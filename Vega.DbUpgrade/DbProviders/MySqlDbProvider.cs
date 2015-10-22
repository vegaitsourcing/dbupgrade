using System;
using Vega.DbUpgrade.Interfaces;
using Vega.DbUpgrade.Utilities;

namespace Vega.DbUpgrade.DbProviders
{
    /// <summary>
    /// This class represents MySQL provider for DbUpgrade tool.
    /// </summary>
    public class MySqlDbProvider : IDbProvider
    {
        #region [Members]

        /// <summary>
        /// Used for creating DbConnection and DbCommand.
        /// </summary>
        private readonly IDatabase _database;

        #endregion

        #region [Constructors]
        /// <summary>
        /// Initializes a new instance of the <see cref="MySqlDbProvider" /> class.
        /// </summary>
        /// <param name="database">The database.</param>
        public MySqlDbProvider(IDatabase database)
        {
            _database = database;
        }
        #endregion

        #region [IDbProvider Members]
        
        public string GetChangeLogTableScript()
        {
            return Constants.ChangeLogTableScripts.MySqlChangeLogTable;
        }
        
        public bool ExecuteScript(string fileContent)
        {
            using (var connection = _database.GetDbConnection())
            {
                using (var command = _database.GetDbCommand(fileContent, connection))
                {
                    connection.Open();

                    command.ExecuteNonQuery();
                }
            }

            return true;
        }

        public bool ExecuteScript(Guid fileId, string fileContent, string filePath)
        {
            var retval = false;

            CreateChangeTable();

            if (!IsScriptExecuted(fileId))
            {
                if (ExecuteScript(fileContent))
                {
                    retval = UpdateChangeTable(fileId, filePath);
                }
            }
            else
            {
                retval = true;
            }

            return retval;
        }

        public bool IsScriptExecuted(Guid fileId)
        {
            var retval = false;

            var sqlScript = String.Format(Constants.ChangeLogTableScripts.CheckIfScriptIsExecutedScript, fileId.ToString());

            using (var connection = _database.GetDbConnection())
            {
                using (var command = _database.GetDbCommand(sqlScript, connection))
                {
                    connection.Open();

                    var result = (long)command.ExecuteScalar();

                    if (result > 0)
                    {
                        retval = true;
                    }
                }
            }

            return retval;
        }

        #endregion

        #region [Private Methods]
        /// <summary>
        /// Updates the change table.
        /// </summary>
        /// <param name="fileId">The file id.</param>
        /// <param name="filePath">The file path.</param>
        /// <returns>Returns <code>true</code> if script is successfully executed, otherwise <code>false</code>.</returns>
        private bool UpdateChangeTable(Guid fileId, string filePath)
        {
            var retval = false;

            var updateString = String.Format(Constants.ChangeLogTableScripts.MySqlUpdateTableScript, fileId, filePath);

            using (var connection = _database.GetDbConnection())
            {
                using (var command = _database.GetDbCommand(updateString, connection))
                {
                    connection.Open();

                    int result = command.ExecuteNonQuery();

                    if (result > 0)
                    {
                        retval = true;
                    }
                }
            }

            return retval;
        }

        /// <summary>
        /// Creates the change table.
        /// </summary>
        /// <returns>Returns <code>true</code> if script is successfully executed, otherwise <code>false</code>.</returns>
        private void CreateChangeTable()
        {
            using (var connection = _database.GetDbConnection())
            {
                using (var command = _database.GetDbCommand(GetChangeLogTableScript(), connection))
                {
                    connection.Open();

                    command.ExecuteNonQuery();
                }
            }
        }

        #endregion
    }
}
