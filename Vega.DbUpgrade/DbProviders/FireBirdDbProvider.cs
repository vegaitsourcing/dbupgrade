using System;
using System.Collections.Generic;
using System.IO;
using FirebirdSql.Data.Isql;
using Vega.DbUpgrade.Interfaces;
using Vega.DbUpgrade.Utilities;

namespace Vega.DbUpgrade.DbProviders
{
    /// <summary>
    /// This class represents Firebird provider for DbUpgrade tool
    /// </summary>
    public class FireBirdDbProvider : IDbProvider
    {
        #region [Members]

        private readonly IDatabase _database;
        #endregion

        #region [Constructors]
        /// <summary>
        /// Initializes a new instance of the <see cref="FireBirdDbProvider" /> class.
        /// </summary>
        /// <param name="database">The database.</param>
        public FireBirdDbProvider(IDatabase database)
        {
            _database = database;
        }
        #endregion

        #region [IDbProvider Members]
        
        public string GetChangeLogTableScript()
        {
            return Constants.ChangeLogTableScripts.FireBirdChangeLogTable;
        }

        public bool ExecuteScript(string fileContent)
        {
            using (var connection = _database.GetDbConnection())
            {
                connection.Open();

                using (var command = _database.GetDbCommand())
                {
                    command.Connection = connection;
                    var commands = ParseSqlScript(fileContent);

                    foreach (string commandText in commands)
                    {
                        command.CommandText = commandText;
                        command.ExecuteNonQuery();
                    }
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

                    var result = (int)command.ExecuteScalar();

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

            var updateString = String.Format(Constants.ChangeLogTableScripts.FirebirdUpdateTableScript, fileId.ToString(), filePath);

            using (var connection = _database.GetDbConnection())
            {
                using (var command = _database.GetDbCommand(updateString, connection))
                {
                    connection.Open();

                    var result = command.ExecuteNonQuery();

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

        /// <summary>
        /// Parses SQL script using native Firebird .NET provider method
        /// </summary>
        /// <param name="sqlScript">Script to parse</param>
        /// <returns>Returns <code>List</code> of SQL commands</returns>
        private IEnumerable<string> ParseSqlScript(string sqlScript)
        {
            var retVal = new List<string>();

            using (TextReader textReader = new StringReader(sqlScript))
            {
                var script = new FbScript(textReader);
                script.Parse();
                retVal.AddRange(script.Results);
            }

            return retVal;
        }

        #endregion
    }
}
