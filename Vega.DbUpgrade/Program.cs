using System;
using System.Configuration;
using System.Globalization;
using System.IO;
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

        /// <summary>
        /// Starting point of DbUpgrade console application.
        /// </summary>
        /// <param name="args">Input arguments.</param>
        public static void Main(string[] args)
        {
                var showHelp = false;

                //quick fix
                //TODO: check if there is a better solution
                ///////////////////////////////////////////////////
                var usCulture = new CultureInfo("en-US");
                Thread.CurrentThread.CurrentCulture = usCulture;
                ///////////////////////////////////////////////////

                try
                {
                    // Set current directory
                    var exeFile = new FileInfo(Assembly.GetExecutingAssembly().Location);
                    if (exeFile.DirectoryName != null)
                    {
                        Environment.CurrentDirectory = exeFile.DirectoryName;
                    }

                    if (args.Length > 0)
                    {
                        var initialDirectory = string.Empty;
                        var action = args[0].ToLower();

                        switch (action)
                        {
                            case Constants.CommandLineOptions.Generate:
                                if (args.Length > 1)
                                {
                                    initialDirectory = args[1].ToLower();
                                }

                                CreateDemoDirectories(initialDirectory);
                                break;
                            case Constants.CommandLineOptions.Help:
                                showHelp = true;
                                break;
                            default: 
                                showHelp = true;
                                break;
                        }

                        if (showHelp)
                        {
                            DisplayHelpMessage();
                        }
                    }
                    else
                    {
                        DbUpgraderStatus res;
                        var scriptsFolderPath = ConfigurationManager.AppSettings[Constants.AppSettingKeys.ScriptsFolder];
                        if (Directory.Exists(scriptsFolderPath))
                        {
                            var upgrader = new DbUpgrader();
                            res = upgrader.Update(scriptsFolderPath);
                        }
                        else
                        {
                            res = DbUpgraderStatus.NonExistingScriptsFolder;
                        }

                        WriteMessage(res);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);
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
                    break;
                case DbUpgraderStatus.Error:
                    Console.WriteLine(Constants.Messages.DbNotUpdated);
                    break;
                case DbUpgraderStatus.NonExistingScriptsFolder:
                    Console.WriteLine(Constants.Messages.NonExistingScriptsFolder);
                    break;
                case DbUpgraderStatus.UnknownDatabase:
                    Console.WriteLine(Constants.Messages.UnknownDatabase);
                    break;
            }
        }

        #endregion
    }
}
