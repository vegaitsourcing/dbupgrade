using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Vega.DbUpgrade.Interfaces;
using Vega.DbUpgrade.Utilities;

namespace Vega.DbUpgrade.DbProviders
{
    /// <summary>
    /// This class represents MS SQL Server provider for DbUpgrade tool
    /// </summary>
    public class MsSqlDbProvider : IDbProvider
    {
        #region [Constants and Fields]

        /// <summary>
        ///  Used for creating DbConnection and DbCommand.
        /// </summary>
        private readonly IDatabase _database;

        #endregion

        #region [Constructors]
        /// <summary>
        /// Initializes a new instance of the <see cref="MsSqlDbProvider" /> class.
        /// </summary>
        /// <param name="database">The database.</param>
        public MsSqlDbProvider(IDatabase database)
        {
            _database = database;
        }
        #endregion
        
        #region [IDbProvider Members]
        
        public string GetChangeLogTableScript()
        {
            return Constants.ChangeLogTableScripts.MsSqlChangeLogTable;
        }

        public bool ExecuteScript(string fileContent)
        {
            using (var connection = _database.GetDbConnection())
            {
                using (var command = _database.GetDbCommand())
                {
                    command.Connection = connection;
                    command.CommandTimeout = 0;
                    connection.Open();

                    var commands = ParseSqlScript(fileContent);

                    foreach (var commandText in commands)
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

            if (!IsScriptExecuted(fileId))
            {
                Console.WriteLine("Executing script: " + filePath);
                if (ExecuteScript(fileContent))
                {
                    Console.WriteLine("Success executing");
                    retval = UpdateChangeTable(fileId, filePath);
                }
                else
                {
                    Console.WriteLine("Fail executing");
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

            if (!CheckIfTableExists())
            {
                CreateChangeTable();
            }
                
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

        #region [Private methods]

        /// <summary>
        /// Updates the change table.
        /// </summary>
        /// <param name="fileId">The file id.</param>
        /// <param name="filePath">The file path.</param>
        /// <returns>Returns <code>true</code> if table is successfully updated, otherwise <code>false</code></returns>
        private bool UpdateChangeTable(Guid fileId, string filePath)
        {
            var retval = false;

            var updateString = String.Format(Constants.ChangeLogTableScripts.MsSqlUpdateTableScript, fileId, filePath);

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
        /// <returns>Returns <code>true</code> if table is successfully created, otherwise <code>false</code></returns>
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
        /// Checks if table exists.
        /// </summary>
        /// <returns>Returns <code>true</code> if table exists, otherwise <code>false</code></returns>
        private bool CheckIfTableExists()
        {
            var retval = false;

            using (var connection = _database.GetDbConnection())
            {
                using (var command = _database.GetDbCommand(Constants.ChangeLogTableScripts.MsSqlCheckIfChangeTableExists, connection))
                {
                    connection.Open();

                    var tableCount = (int)command.ExecuteScalar();

                    if (tableCount > 0)
                    {
                        retval = true;
                    }
                }
            }

            return retval;
        }

        /// <summary>
        /// This method parses sql string and searches for "GO" statements. It returns list of batch sql commands without "GO" statemens
        /// </summary>
        /// <param name="sqlScript">The SQL script.</param>
        /// <returns>Returns list of sql batch commands.</returns>
        private IEnumerable<string> ParseSqlScript(string sqlScript)
        {
            var retval = new List<string>();
            const string regexPattern = @"\s*GO(;\s*|\s*) ($ | \-\- .*$)";

            var commands = Regex.Split(sqlScript + "\n", regexPattern,
                                       RegexOptions.IgnoreCase | RegexOptions.Multiline |
                                       RegexOptions.IgnorePatternWhitespace | RegexOptions.ExplicitCapture);

            for (var i = 0; i < commands.Length; i++)
            {
                var trimmedCommand = commands[i].Trim();
                if (!String.IsNullOrEmpty(trimmedCommand))
                {
                    retval.Add(trimmedCommand);
                }
            }

            return retval;
        }

        #endregion
    }
}
