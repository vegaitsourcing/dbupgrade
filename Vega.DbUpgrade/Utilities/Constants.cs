namespace Vega.DbUpgrade.Utilities
{
    /// <summary>
    /// Vega DbUpgrade constants
    /// </summary>
    public class Constants
    {
        /// <summary>
        /// This class contains ChangeLog table definitions for different databases.
        /// ChangeLog database table is used to keep information about performed database updates.
        /// </summary>
        public class ChangeLogTableScripts
        {
            /// <summary>
            /// Script for creation of ChangeLog table in SQL database
            /// </summary>
            public const string MsSqlChangeLogTable = @"create table DBChangeLog (
                                                        DBChangeLogID        UNIQUEIDENTIFIER     not null,
                                                        ExecutionStartTime   DATETIME             not null,
                                                        ScriptFilePath       NVARCHAR(500)        not null,
                                                        constraint PK_DBCHANGELOG primary key (DBChangeLogID)
                                                    )";

            /// <summary>
            /// Script that checks if DBChangeLog table exist on SQL server
            /// </summary>
            public const string MsSqlCheckIfChangeTableExists = @"SELECT COUNT(*) FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DBChangeLog]') AND type in (N'U')";

            /// <summary>
            /// Script that inserts row into DBChangeLog table
            /// </summary>
            public const string MsSqlUpdateTableScript = @"INSERT INTO DBChangeLog(DBChangeLogID, ExecutionStartTime, ScriptFilePath)
                                                                        VALUES('{0}', GETDATE(), '{1}')";

            /// <summary>
            /// Script that inserts row into DBChangeLog table for MySql DB
            /// </summary>
            public const string MySqlUpdateTableScript = @"INSERT INTO DBChangeLog(DBChangeLogID, ExecutionStartTime, ScriptFilePath)
                                                                        VALUES('{0}', NOW(), '{1}')";

            /// <summary>
            /// Script that inserts row into DBChangeLog table for Firebird DB
            /// </summary>
            public const string FirebirdUpdateTableScript = @"INSERT INTO DBChangeLog(DBChangeLogID, ExecutionStartTime, ScriptFilePath)
                                                                        VALUES('{0}',CURRENT_TIMESTAMP, '{1}')";

            /// <summary>
            /// Script that checks if some sql file is already executed
            /// </summary>
            public const string CheckIfScriptIsExecutedScript = @"SELECT COUNT(DBChangeLogID) FROM DBChangeLog WHERE DBChangeLogID = '{0}'";

            /// <summary>
            /// Script for creation of ChangeLog table in MySQL database
            /// </summary>
            public const string MySqlChangeLogTable = @"create table if not exists DBCHANGELOG
                                                        (
                                                            DBChangeLogID        CHAR(38) NOT NULL,
                                                            ExecutionStartTime   DATETIME NOT NULL,
                                                            ScriptFilePath       VARCHAR(500) NOT NULL,
                                                            PRIMARY KEY (DBChangeLogID)
                                                        );";
            
            /// <summary>
            /// Script for creation of ChangeLog table in FireBird database
            /// </summary>
            public const string FireBirdChangeLogTable = @"
                                                            
                                                            EXECUTE BLOCK AS BEGIN
                                                            if (NOT EXISTS(SELECT 1 FROM rdb$relations WHERE rdb$relation_name = 'DBCHANGELOG')) THEN
                                                                                                                        EXECUTE STATEMENT '
                                                                                                                                                CREATE TABLE DBChangeLog
                                                                                                                                                (
                                                                                                                                                    DBChangeLogID        CHAR(38) NOT NULL,
                                                                                                                                                    ExecutionStartTime   TIMESTAMP NOT NULL,
                                                                                                                                                    ScriptFilePath       VARCHAR(500) NOT NULL,
                                                                                                                                                    PRIMARY KEY(DBChangeLogID)
                                                                                                                                                );
                                                                                                                                            ';
                                                            END;
                                                            ";
        }

        /// <summary>
        /// App settings key names
        /// </summary>
        public class AppSettingKeys
        {
            /// <summary>
            /// Connection string settings key.
            /// </summary>
            public const string ConnectionString = "ConnectionString";

            /// <summary>
            /// Scripts folder path key.
            /// </summary>
            public const string ScriptsFolder = "ScriptsFolder";

            /// <summary>
            /// Database provider settings key.
            /// </summary>
            public const string DatabaseProvider = "DbProvider";
        }

        /// <summary>
        /// Contains constants of folder names as well as definition XML file names
        /// </summary>
        public class FileFolderNames
        {
            /// <summary>
            /// Common folder name.
            /// </summary>
            public const string CommonFolderName = "Common";

            /// <summary>
            /// Folder name with SQL updates.
            /// </summary>
            public const string UpgradesFolderName = "Upgrades";

            /// <summary>
            /// Scripts folder name
            /// </summary>
            public const string ScriptsFolderName = "Scripts";

            /// <summary>
            /// Functions folder name
            /// </summary>
            public const string FunctionsFolderName = "Functions";

            /// <summary>
            /// Procedures folder name
            /// </summary>
            public const string ProcedureFolderName = "Procedures";

            /// <summary>
            /// XML definition file name.
            /// </summary>
            public const string DefinitionFileName = "definition.xml";

            /// <summary>
            /// XML version file name.
            /// </summary>
            public const string VersionsFileName = "versions.xml";
        }

        /// <summary>
        /// Describes file extensions
        /// </summary>
        public class FileExtensions
        {
            /// <summary>
            /// Extension of SQL script file.
            /// </summary>
            public const string Sql = "*.sql";
        }

        /// <summary>
        /// Defines XSD definitions
        /// </summary>
        public class XmlSchemas
        {
            /// <summary>
            /// XML schema of version XML file.
            /// </summary>
            public const string VersionFile = @"<xsd:schema targetNamespace=""urn:version-schema""
                                                          elementFormDefault=""qualified""
                                                          xmlns=""urn:version-schema""    
                                                          xmlns:xsd=""http://www.w3.org/2001/XMLSchema""
                                                >
                                                  <xsd:element name=""Versions"">
                                                    <xsd:complexType>
                                                      <xsd:sequence>
                                                        <xsd:element maxOccurs=""unbounded"" name=""Version"" type=""xsd:string""/>
                                                      </xsd:sequence>
                                                    </xsd:complexType>
                                                  </xsd:element>
                                                </xsd:schema>";

            /// <summary>
            /// XML schema of definition file.
            /// </summary>
            public const string DefinitionFile = @"<xsd:schema targetNamespace=""urn:definition-schema""
                                                          elementFormDefault=""qualified""
                                                          xmlns=""urn:definition-schema""    
                                                          xmlns:xsd=""http://www.w3.org/2001/XMLSchema""
                                                >
                                                  <xsd:element name=""Files"">
                                                    <xsd:complexType>
                                                      <xsd:sequence>
                                                        <xsd:element maxOccurs=""unbounded"" name=""File"">    
                                                            <xsd:complexType>
                                                                <xsd:sequence>
                                                                    <xsd:element name=""Id"" type=""GUID""/>
                                                                    <xsd:element name=""Path"" type=""xsd:string""/>
                                                                </xsd:sequence>
                                                            </xsd:complexType>
                                                        </xsd:element>
                                                      </xsd:sequence>
                                                    </xsd:complexType>
                                                  </xsd:element>

                                                  <xsd:simpleType name=""GUID"">
                                                    <xsd:annotation>
                                                      <xsd:documentation xml:lang=""en"">
                                                         The representation of a GUID, generally the id of an element.
                                                      </xsd:documentation>
                                                    </xsd:annotation>
                                                    <xsd:restriction base=""xsd:string"">
                                                      <xsd:pattern value=""\{[a-fA-F0-9]{8}-[a-fA-F0-9]{4}-[a-fA-F0-9]{4}-[a-fA-F0-9]{4}-[a-fA-F0-9]{12}\}""/>
                                                    </xsd:restriction>
                                                  </xsd:simpleType>
                                                </xsd:schema>";
        }

        /// <summary>
        /// Schema names.
        /// </summary>
        public class XmlSchemaNames
        {
            /// <summary>
            /// XML schema name of version XML file.
            /// </summary>
            public const string Version = "urn:version-schema";

            /// <summary>
            /// XML schema name of definition XML file.
            /// </summary>
            public const string Definition = "urn:definition-schema";
        }

        /// <summary>
        /// Messages that are displayed in console.
        /// </summary>
        public class Messages
        {
            /// <summary>
            /// Database has been successfully updated.
            /// </summary>
            public const string DbSuccUpdated = "Database has been successfully updated.";
            
            /// <summary>
            /// Database wan not updated.
            /// </summary>
            public const string DbNotUpdated = "Database was not updated.";

            /// <summary>
            /// Folder with sql scripts doesn't exist.
            /// </summary>
            public const string NonExistingScriptsFolder = "Error: Folder with sql scripts doesn't exist.";

            /// <summary>
            /// Folder doesn't exist.
            /// </summary>
            public const string NonExistingFolder = "Error: Folder '{0}' doesn't exist.";

            /// <summary>
            /// Database is unknown.
            /// </summary>
            public const string UnknownDatabase = "Error: Unknown database.";

            /// <summary>
            /// Incorrect format of XML file.
            /// </summary>
            public const string IncorrectFormatOfXmlFile = "Error: XML file {0} is not in correct format:\n{1}";

            /// <summary>
            /// Missing definition file.
            /// </summary>
            public const string DefinitionMissing = "Error: Missing definition XML file for version {0}";

            /// <summary>
            /// Message on successful script execution
            /// </summary>
            public const string SuccessMessage = "Done!";

            /// <summary>
            /// Message on failed script execution
            /// </summary>
            public const string FailureMessage = "Failed!";

            /// <summary>
            /// Format for script execution message
            /// </summary>
            public const string ScriptExecutionFormatMessage = "Executing script: {0}; ";

            /// <summary>
            /// Error in script message string format.
            /// </summary>
            public const string ErrorInScriptMessage = "Encounter error in script file: {0}; ";
        }

        /// <summary>
        /// Content of demo files
        /// </summary>
        public class DemoFileContent
        {
            /// <summary>
            /// Content of demo file versions.xml
            /// </summary>
            public const string VersionFileContent = @"<?xml version=""1.0"" encoding=""utf-8""?>
                                                        <Versions>
                                                          <!-- This is only sample file
                                                                Versions would be named here
                                                                Each version should have unique entry
                                                            -->
                                                          <Version>V1.0.0</Version>
                                                        </Versions>";

            /// <summary>
            /// Content of demo file definitions.xml
            /// </summary>
            public const string DefinitionFileContent = @"<?xml version=""1.0"" encoding=""utf-8""?>
                                                            <Files>
                                                                <!-- This is only sample file
                                                                     Each file should have unique GUID
                                                                     Also file paths are relative to parent directory 
                                                                  <File>
                                                                    <Id>{DEA469E4-0D2E-4b00-B833-3B550F0A0732}</Id>
                                                                    <Path>Tables\BackendUser.sql</Path>
                                                                  </File>  
                                                                -->
                                                            </Files>
                                                            ";
        }

        /// <summary>
        /// Contains command line switch options
        /// </summary>
        public class CommandLineOptions
        {
            /// <summary>
            /// Generate switch
            /// </summary>
            public const string Generate = "-generate";

            /// <summary>
            /// The scripts folder path
            /// </summary>
            public const string ScriptsFolderPath = "-scriptsFolderPath";

            /// <summary>
            /// Help switch
            /// </summary>
            public const string Help = "-help";
        }
    }
}