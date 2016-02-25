using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Xml;

using Vega.DbUpgrade.Interfaces;
using Vega.DbUpgrade.Utilities;

namespace Vega.DbUpgrade
{
    /// <summary>
    /// This class contains methods for handling file system logic of DbUpgarader.
    /// </summary>
    public class DbUpgrader
    {
        #region [Members]
        /// <summary>
        /// Database provider.
        /// </summary>
        private readonly IDbProvider _provider;
        #endregion

        #region [Constructors]
        /// <summary>
        /// Initializes a new instance of the <see cref="DbUpgrader"/> class.
        /// </summary>
        public DbUpgrader()
        {
            _provider = DbProviderFactory.GetDbProvider();
        }
        #endregion
        
        #region [Public Methods]
        /// <summary>
        /// Updates database with scripts from <paramref name="scriptsFolder" /> folder.
        /// </summary>
        /// <param name="scriptsFolder">Path to the folder on file system which contains scripts for database update</param>
        /// <param name="fromVersion">From version. Set null if you'd like to perform the installation for all versions.</param>
        /// <param name="placeholdersKeyValuePairs">The placeholders key value pairs.</param>
        /// <returns>
        ///   <see cref="Vega.DbUpgrade.Utilities.DbUpgraderStatus" /> value.
        /// </returns>
        public DbUpgraderStatus Update(string scriptsFolder, string fromVersion, Dictionary<string, string> placeholdersKeyValuePairs)
        {
            var retVal = DbUpgraderStatus.Success;
            var isFromVersionFound = false;
            if (_provider != null)
            {
                var scriptsDirInfo = new DirectoryInfo(scriptsFolder);
                if (scriptsDirInfo.Exists)
                {
                    // Run sql scripts from Upgrades folder
                    var upgradeDir = FileHelper.GetSubDirectoryByName(scriptsDirInfo, Constants.FileFolderNames.UpgradesFolderName, SearchOption.TopDirectoryOnly);
                    if (upgradeDir != null && upgradeDir.Exists)
                    {
                        var versionDirs = new List<DirectoryInfo>();

                        var versionsXmlFile = FileHelper.GetFileByName(upgradeDir, Constants.FileFolderNames.VersionsFileName, SearchOption.TopDirectoryOnly);
                        if (versionsXmlFile != null && versionsXmlFile.Exists)
                        {
                            // Validate versions.xml file
                            XmlValidator.GetInstance().Validate(versionsXmlFile.FullName, Constants.XmlSchemas.VersionFile, Constants.XmlSchemaNames.Version);

                            var versionsXmlDoc = new XmlDocument();
                            versionsXmlDoc.Load(versionsXmlFile.FullName);

                            var versionNodes = versionsXmlDoc.SelectNodes("Versions/Version");
                            if (versionNodes != null)
                            {
                                foreach (XmlNode xmlNode in versionNodes)
                                {
                                    var versionDir =
                                        new DirectoryInfo(upgradeDir.FullName + Path.DirectorySeparatorChar +
                                                          xmlNode.InnerText);

                                    // if the version folder name has been passed, check if the folder is found.
                                    if (!isFromVersionFound && !string.IsNullOrEmpty(fromVersion) &&
                                        xmlNode.InnerText.ToLower() == fromVersion.ToLower())
                                    {
                                        isFromVersionFound = true;
                                    }

                                    // In case the from version value has been passed, and the version has been found,
                                    // set add the version folder to the list. all the following versions will be added as well.
                                    if (string.IsNullOrEmpty(fromVersion) || isFromVersionFound)
                                    {
                                        versionDirs.Add(versionDir);
                                    }
                                }
                            }
                        }
                        else
                        {
                            // Get versions by default order
                            var versions = upgradeDir.GetDirectories();
                            versionDirs.AddRange(versions);
                        }

                        if (!string.IsNullOrEmpty(fromVersion) && versionDirs.Count == 0)
                        {
                            retVal = DbUpgraderStatus.NonExistingVersionFolder;
                        }
                        else
                        {
                            // Execute SQL scripts
                            retVal = ExecuteScripts(versionDirs, true, placeholdersKeyValuePairs);
                        }
                    }

                    if (retVal == DbUpgraderStatus.Success)
                    {
                        // Run sql scripts from Common folder
                        var commonDir = FileHelper.GetSubDirectoryByName(scriptsDirInfo, Constants.FileFolderNames.CommonFolderName, SearchOption.TopDirectoryOnly);
                        if (commonDir != null && commonDir.Exists)
                        {
                            retVal = ExecuteScripts(commonDir, placeholdersKeyValuePairs);
                        }
                    }
                }
                else
                {
                    retVal = DbUpgraderStatus.NonExistingScriptsFolder;
                }
            }
            else
            {
                retVal = DbUpgraderStatus.UnknownDatabase;
            }

            return retVal;
        }
        #endregion

        #region [Private Methods]
        /// <summary>
        /// Executes SQL scripts in database from given directory.
        /// </summary>
        /// <param name="directory">Directory with SQL scripts.</param>
        /// <param name="placeholdersKeyValuePairs">The placeholders key value pairs.</param>
        /// <returns>
        ///   <see cref="Vega.DbUpgrade.Utilities.DbUpgraderStatus" /> value.
        /// </returns>
        private DbUpgraderStatus ExecuteScripts(DirectoryInfo directory, Dictionary<string, string> placeholdersKeyValuePairs)
        {
            var dirs = new List<DirectoryInfo> {directory};

            return ExecuteScripts(dirs, false, placeholdersKeyValuePairs);
        }

        /// <summary>
        /// Executes SQL scripts in database from given directories.
        /// </summary>
        /// <param name="dirs">Directories with SQL scripts.</param>
        /// <param name="updateChangeLog">if <c>true</c> ChangeLog table in database will be updated.</param>
        /// <param name="placeholdersKeyValuePairs">The placeholders key value pairs.</param>
        /// <returns>
        ///   <see cref="Vega.DbUpgrade.Utilities.DbUpgraderStatus" /> value.
        /// </returns>
        private DbUpgraderStatus ExecuteScripts(List<DirectoryInfo> dirs, bool updateChangeLog, Dictionary<string, string> placeholdersKeyValuePairs)
        {
            var retVal = DbUpgraderStatus.Success;
            if (updateChangeLog)
            {
                // Check definition.xml files. Each version should have a definition.xml file
                CheckDefinitionFiles(dirs);

                foreach (var version in dirs)
                {
                    // Execute scripts by order defined in xml definition file
                    var definitionFile = FileHelper.GetFileByName(version, Constants.FileFolderNames.DefinitionFileName, SearchOption.TopDirectoryOnly);
                    var defXmlDoc = new XmlDocument();
                    defXmlDoc.Load(definitionFile.FullName);

                    var fileNodes = defXmlDoc.SelectNodes("Files/File");
                    if (fileNodes != null)
                    {
                        foreach (XmlNode fileNode in fileNodes)
                        {
                            var selectSingleNode = fileNode.SelectSingleNode("Id");
                            var singleNode = fileNode.SelectSingleNode("Path");
                            if (selectSingleNode != null && singleNode != null)
                            {
                                var id = selectSingleNode.InnerText;
                                var path = singleNode.InnerText;

                                var fileId = new Guid(id);
                                var fileFullName = version.FullName + Path.DirectorySeparatorChar + path;

                                var scriptsFolder =
                                    ConfigurationManager.AppSettings[Constants.AppSettingKeys.ScriptsFolder];
                                var fileRelPath = FileHelper.GetRelativePath(fileFullName, scriptsFolder);
                                var fileContent = FileHelper.GetFileContent(fileFullName);

                                if (!_provider.IsScriptExecuted(fileId))
                                {
                                    Console.Write(String.Concat(Constants.Messages.ScriptExecutionFormatMessage,
                                                                fileFullName));

                                    // replace the placeholders in the scripts if exists.
                                    if (placeholdersKeyValuePairs != null && placeholdersKeyValuePairs.Count > 0)
                                    {
                                        fileContent =
                                            placeholdersKeyValuePairs.Where(
                                                placeholdersKeyValuePair =>
                                                !string.IsNullOrEmpty(placeholdersKeyValuePair.Value))
                                                                     .Aggregate(fileContent,
                                                                                (current, placeholdersKeyValuePair) =>
                                                                                current.Replace(
                                                                                    placeholdersKeyValuePair.Key,
                                                                                    placeholdersKeyValuePair.Value));
                                    }

                                    var res = _provider.ExecuteScript(fileId, fileContent, fileRelPath);

                                    var status = res
                                                     ? Constants.Messages.SuccessMessage
                                                     : Constants.Messages.FailureMessage;
                                    Console.WriteLine(status);

                                    if (!res)
                                    {
                                        retVal = DbUpgraderStatus.Error;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                foreach (var directory in dirs)
                {
                    var sqlFiles = directory.GetFiles(Constants.FileExtensions.Sql, SearchOption.AllDirectories);

                    foreach (var sqlFile in sqlFiles)
                    {
                        var fullName = sqlFile.FullName;

                        var fileContent = FileHelper.GetFileContent(fullName);
                        Console.WriteLine("Common script executed: " + fullName);

                        // replace the placeholders in the scripts if exists.
                        if (placeholdersKeyValuePairs != null && placeholdersKeyValuePairs.Count > 0)
                        {
                            fileContent =
                                placeholdersKeyValuePairs.Where(
                                    placeholdersKeyValuePair =>
                                    !string.IsNullOrEmpty(placeholdersKeyValuePair.Value))
                                                         .Aggregate(fileContent,
                                                                    (current, placeholdersKeyValuePair) =>
                                                                    current.Replace(
                                                                        placeholdersKeyValuePair.Key,
                                                                        placeholdersKeyValuePair.Value));
                        }

                        var res = _provider.ExecuteScript(fileContent);

                        if (res) continue;

                        Console.Write(String.Concat(Constants.Messages.ScriptExecutionFormatMessage, sqlFile.FullName));
                        Console.WriteLine(Constants.Messages.FailureMessage);

                        retVal = DbUpgraderStatus.Error;
                        break;
                    }
                }
            }

            return retVal;
        }

        /// <summary>
        /// Checks if definition files are present on file system and if they are in correct format.
        /// </summary>
        /// <param name="dirs">Directories which contains XML definition files.</param>
        private static void CheckDefinitionFiles(IReadOnlyCollection<DirectoryInfo> dirs)
        {
            if (dirs.Count > 0)
            {
                foreach (var versionDir in dirs)
                {
                    var definitionFile = FileHelper.GetFileByName(versionDir, Constants.FileFolderNames.DefinitionFileName, SearchOption.TopDirectoryOnly);
                    if (definitionFile != null && definitionFile.Exists)
                    {
                        XmlValidator.GetInstance().Validate(definitionFile.FullName, Constants.XmlSchemas.DefinitionFile, Constants.XmlSchemaNames.Definition);
                    }
                    else
                    {
                        throw new Exception(String.Format(Constants.Messages.DefinitionMissing, versionDir.FullName));
                    }
                }
            }
        }

        #endregion
    }
}