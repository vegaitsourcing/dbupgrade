using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CsvHelper;
using Vega.DbUpgrade.Entities;

namespace Vega.DbUpgrade.Utilities
{
    public class ToolOperationsHelper
    {
        public string GetCsvImportReadySqlScript(string fileContent, string currentFolder)
        {
            var csvImports = new List<CsvImport>();
            foreach (var fileLine in fileContent.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None))
            {
                if (fileLine.Contains(Constants.ToolOperations.CsvImport))
                {
                    var startIndexOfOperation = fileLine.IndexOf(Constants.ToolOperations.CsvImport,
                        StringComparison.Ordinal);

                    var endIndexOfOperationDefinition = fileLine.Substring(startIndexOfOperation)
                        .IndexOf(">", StringComparison.Ordinal);
                    if (endIndexOfOperationDefinition > 0)
                    {
                        var operationDefinitionString = fileLine.Substring(startIndexOfOperation,
                            endIndexOfOperationDefinition + 1);
                        var tableIndex = operationDefinitionString.IndexOf(Constants.ToolOperations.Attributes.Table,
                            StringComparison.Ordinal);
                        var columnsIndex = operationDefinitionString.IndexOf(
                            Constants.ToolOperations.Attributes.Columns, StringComparison.Ordinal);
                        var csvFileIndex = operationDefinitionString.IndexOf(
                            Constants.ToolOperations.Attributes.CsvFile, StringComparison.Ordinal);
                        if (tableIndex < 0 || columnsIndex < 0 || csvFileIndex < 0)
                        {
                            throw new ArgumentException(
                                "The CSV import tool operation is not defined properly: There should be TABLE,COLUMNS and CSV_FILE attributes.");
                        }

                        var csvImport = new CsvImport
                        {
                            // if not defined, the default OperationId is empty.
                            OperationId = operationDefinitionString.IndexOf(
                                              Constants.ToolOperations.Attributes.UpgradeOperationId,
                                              StringComparison.Ordinal) > 0
                                ? GetAttributeValue(operationDefinitionString,
                                    Constants.ToolOperations.Attributes.UpgradeOperationId)
                                : string.Empty,
                            // if not defined, default delimiter is set to comma.
                            Delimiter = operationDefinitionString.IndexOf(
                                            Constants.ToolOperations.Attributes.Delimiter, StringComparison.Ordinal) > 0
                                ? GetAttributeValue(operationDefinitionString,
                                    Constants.ToolOperations.Attributes.Delimiter)
                                : ",",
                            Table =
                                GetAttributeValue(operationDefinitionString,
                                    Constants.ToolOperations.Attributes.Table),
                            Columns =
                                GetAttributeValue(operationDefinitionString,
                                    Constants.ToolOperations.Attributes.Columns).Split(',').ToList(),
                            CsvFiles = GetAttributeValue(operationDefinitionString,
                                Constants.ToolOperations.Attributes.CsvFile).Split(',').ToList()
                        };

                        csvImports.Add(csvImport);
                    }
                }
            }

            // handle all CSV Import operations and replace the placeholders with the proper INSERT script.
            foreach (var csvImport in csvImports)
            {
                fileContent = fileContent.Replace(
                    string.Format(Constants.DefaultPlaceholders.DbUpgradeOperationScriptsContent,
                        csvImport.OperationId),
                    string.Concat("\n", GetCsvImportSqlScript(currentFolder, csvImport)));
            }

            return fileContent;
        }

        private static string GetCsvImportSqlScript(string currentFolder, CsvImport csvImport)
        {
            var retVal = new StringBuilder();

            var insertFormat = string.Format("INSERT INTO {0} ({1})", csvImport.Table,
                string.Join(",", csvImport.Columns.ToArray()));

            foreach (var csvFile in csvImport.CsvFiles)
            {
                using (var streamReader = new StreamReader(string.Concat(currentFolder, @"\", csvFile)))
                {
                    var csv = new CsvReader(streamReader);
                    csv.Configuration.Delimiter = "\t";
                    csv.Configuration.HasHeaderRecord = true;

                    var index = 0;
                    while (csv.Read())
                    {
                        // skip the header line.
                        if (++index <= 0) continue;

                        var values = new string[csv.FieldHeaders.Length];
                        for (var i = 0; i < csv.FieldHeaders.Length; i++)
                        {
                            values[i] = csv.GetField<string>(i);
                        }

                        retVal.AppendLine(string.Concat(insertFormat,
                            string.Format(" VALUES({0});",
                                string.Join(",", values
                                    .Select(retItem => string.Concat("N'", retItem.Replace("'", "''"), "'"))
                                    .ToArray()))));
                    }
                }
            }

            return retVal.ToString();
        }

        private static string GetAttributeValue(string operationDefinitionString, string attribute)
        {
            var attributeIndexEnd = operationDefinitionString.IndexOf(attribute, StringComparison.Ordinal) +
                                    attribute.Length + 1;
            return operationDefinitionString.Substring(attributeIndexEnd,
                                                       operationDefinitionString.Substring(attributeIndexEnd)
                                                                                .IndexOf(";", StringComparison.Ordinal))
                                            .Trim();
        }
    }
}