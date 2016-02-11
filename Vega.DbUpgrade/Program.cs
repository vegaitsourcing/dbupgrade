using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using Vega.DbUpgrade.Utilities;   

namespace Vega.DbUpgrade
{
    /// <summary>
    /// DbUpgrade console application.
    /// </summary>
    public class Program
    {
        #region [Public Methods]

        private const int Success = 0;
        private const int ErrorOccured = 160;
        private const int ScriptDirectoryDoesntExist = 270;
        private const int UnknownDatabase = 490;
        private const int VersionFolderDoesntExist = 520;

        /// <summary>
        /// Starting point of DbUpgrade console application.
        /// </summary>
        /// <param name="args">Input arguments.</param>
        public static void Main(string[] args)
        {
            //quick fix
            //TODO: check if there is a better solution
            ///////////////////////////////////////////////////
            var usCulture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentCulture = usCulture;
            ///////////////////////////////////////////////////

            var scriptsFolderPath = string.Empty;
            var fromVersion = string.Empty;
            var placeholdersWithValues = new Dictionary<string, string>();
            try
            {
                // Set current directory
                var exeFile = new FileInfo(Assembly.GetExecutingAssembly().Location);
                if (exeFile.DirectoryName != null)
                {
                    Environment.CurrentDirectory = exeFile.DirectoryName;
                }

                if (args.Length == 1 && args[0] == Constants.CommandLineOptions.Help)
                {
                    DisplayHelpMessage();
                    return;
                }

                if (args.Length > 0)
                {
                    var initialDirectory = string.Empty;

                    for (var i = 0; i < args.Length; i += 2)
                    {
                        var action = args[i].ToLower();

                        switch (action)
                        {
                            case Constants.CommandLineOptions.FromVersion:
                                fromVersion = args[i + 1];
                                break;
                            case Constants.CommandLineOptions.ScriptsFolderPath:

                                scriptsFolderPath = args[i + 1];
                                break;

                            case Constants.CommandLineOptions.PlaceholdersWithValues:
                                {
                                    if (args.Length > i + 1)
                                    {
                                        var argValue = args[i + 1];
                                        foreach (
                                            var keyValuePairSplitted in
                                                argValue.Split(Constants.Delimiters.KeyValuePairDelimiter)
                                                        .Select(
                                                            keyValuePair =>
                                                            keyValuePair.Split(Constants.Delimiters.KeyValueDelimiter)))
                                        {
                                            placeholdersWithValues.Add(keyValuePairSplitted[0],
                                                                       keyValuePairSplitted.Length > 1
                                                                           ? keyValuePairSplitted[1]
                                                                           : string.Empty);
                                        }
                                    }

                                    break;
                                }

                            case Constants.CommandLineOptions.Generate:
                                if (args.Length > i + 1)
                                {
                                    initialDirectory = args[i + 1].ToLower();
                                }

                                CreateDemoDirectories(initialDirectory);

                                return;
                        }
                    }
                }

                // check if the Scripts has been passed via argument.
                if (string.IsNullOrEmpty(scriptsFolderPath))
                {
                    scriptsFolderPath = ConfigurationManager.AppSettings[Constants.AppSettingKeys.ScriptsFolder];
                }

                DbUpgraderStatus res;
                if (Directory.Exists(scriptsFolderPath))
                {
                    var upgrader = new DbUpgrader();
                    res = upgrader.Update(scriptsFolderPath, fromVersion, placeholdersWithValues);
                }
                else
                {
                    res = DbUpgraderStatus.NonExistingScriptsFolder;
                }
                 
                WriteMessage(res);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                Environment.Exit(ErrorOccured);
            }
        }

        #endregion

        #region [Private Methods]

        /// <summary>
        /// Creates demo directory structure
        /// </summary>
        /// <param name="initialDirectory">Initial directory in which to create demo directories</param>
        private static void CreateDemoDirectories(string initialDirectory)
        {
            if (!string.IsNullOrEmpty(initialDirectory))
            {
                if (!Directory.Exists(initialDirectory))
                {
                    throw new DirectoryNotFoundException();
                }

                initialDirectory += Path.DirectorySeparatorChar;
            }

            // create directory structure
            Directory.CreateDirectory(initialDirectory + Constants.FileFolderNames.ScriptsFolderName +
                                      Path.DirectorySeparatorChar + Constants.FileFolderNames.CommonFolderName +
                                      Path.DirectorySeparatorChar + Constants.FileFolderNames.FunctionsFolderName);
            Directory.CreateDirectory(initialDirectory + Constants.FileFolderNames.ScriptsFolderName +
                                      Path.DirectorySeparatorChar + Constants.FileFolderNames.CommonFolderName +
                                      Path.DirectorySeparatorChar + Constants.FileFolderNames.ProcedureFolderName);
            Directory.CreateDirectory(initialDirectory + Constants.FileFolderNames.ScriptsFolderName +
                                      Path.DirectorySeparatorChar + Constants.FileFolderNames.UpgradesFolderName +
                                      Path.DirectorySeparatorChar + "V1.0.0");

            // create versions file
            File.WriteAllText(
                initialDirectory + Constants.FileFolderNames.ScriptsFolderName + Path.DirectorySeparatorChar +
                Constants.FileFolderNames.UpgradesFolderName + Path.DirectorySeparatorChar +
                Constants.FileFolderNames.VersionsFileName, Constants.DemoFileContent.VersionFileContent);

            // create definition file
            File.WriteAllText(
                initialDirectory + Constants.FileFolderNames.ScriptsFolderName + Path.DirectorySeparatorChar +
                Constants.FileFolderNames.UpgradesFolderName + Path.DirectorySeparatorChar + "V1.0.0" +
                Path.DirectorySeparatorChar + Constants.FileFolderNames.DefinitionFileName,
                Constants.DemoFileContent.DefinitionFileContent);
        }

        /// <summary>
        /// Displays usage message
        /// </summary>
        private static void DisplayHelpMessage()
        {
            Console.WriteLine(Assembly.GetExecutingAssembly().ManifestModule.Name + @" -scriptsFolderPath [<root scripts folder path>]");
            Console.WriteLine(string.Empty);
            Console.WriteLine(Assembly.GetExecutingAssembly().ManifestModule.Name + @" -generate [<output folder>]");
            Console.WriteLine(string.Empty);
            Console.WriteLine("Example:" + Environment.NewLine);
            Console.WriteLine(Assembly.GetExecutingAssembly().ManifestModule.Name + @" -generate C:\OutputFolder");
            Console.ReadKey();
        }
        
        /// <summary>
        /// Writes the message to console.
        /// </summary>
        /// <param name="status">The database upgrader status.</param>
        private static void WriteMessage(DbUpgraderStatus status)
        {
            switch (status)
            {
                case DbUpgraderStatus.Success:
                    Console.WriteLine(Constants.Messages.DbSuccUpdated);
                    Environment.Exit(Success);
                    break;
                case DbUpgraderStatus.Error:
                    Console.WriteLine(Constants.Messages.DbNotUpdated);
                    Environment.Exit(ErrorOccured);
                    break;
                case DbUpgraderStatus.NonExistingScriptsFolder:
                    Console.WriteLine(Constants.Messages.NonExistingScriptsFolder);
                    Environment.Exit(ScriptDirectoryDoesntExist);
                    break;
                case DbUpgraderStatus.UnknownDatabase:
                    Console.WriteLine(Constants.Messages.UnknownDatabase);
                    Environment.Exit(UnknownDatabase);
                    break;
                case DbUpgraderStatus.NonExistingVersionFolder:
                    Console.WriteLine(Constants.Messages.NonExistingVersionFolder);
                    Environment.Exit(VersionFolderDoesntExist);
                    break;
            }
        }

        #endregion
    }
}
