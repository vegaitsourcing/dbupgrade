using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Vega.DbUpgrade.Entities;

namespace Vega.DbUpgrade.Utilities
{
    public class ToolOperationsHelper
    {
        public string GetCsvImportReadySqlScript(string fileContent, string currentFolder)
        {
            if (fileContent.Contains(Constants.DefaultPlaceholders.DbUpgradeOperationScriptsContent))
            {
                var startIndexOfOperation = fileContent.IndexOf(Constants.ToolOperations.CsvImport,
                                                                StringComparison.Ordinal);
                if (startIndexOfOperation > 0)
                {
                    var endIndexOfOperationDefinition = fileContent.Substring(startIndexOfOperation)
                                                                   .IndexOf(">", StringComparison.Ordinal);
                    if (endIndexOfOperationDefinition > 0)
                    {
                        var operationDefinitionString = fileContent.Substring(startIndexOfOperation,
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
                                Table =
                                    GetAttributeValue(operationDefinitionString,
                                                      Constants.ToolOperations.Attributes.Table),
                                Columns =
                                    GetAttributeValue(operationDefinitionString,
                                                      Constants.ToolOperations.Attributes.Columns).Split(',').ToList(),
                                CsvFile =
                                    GetAttributeValue(operationDefinitionString,
                                                      Constants.ToolOperations.Attributes.CsvFile)
                            };

                        fileContent = fileContent.Replace(
                            Constants.DefaultPlaceholders.DbUpgradeOperationScriptsContent,
                            string.Concat("\n", GetCsvImportSqlScript(currentFolder, csvImport)));
                    }
                }
            }

            return fileContent;
        }

        private static string GetCsvImportSqlScript(string currentFolder, CsvImport csvImport)
        {
            var retVal = new StringBuilder();
            var csvContent = File.ReadAllLines(string.Concat(currentFolder, @"\", csvImport.CsvFile));
            var insertFormat = string.Format("INSERT INTO {0} ({1})", csvImport.Table,
                                             string.Join(",", csvImport.Columns.ToArray()));
            var i = 0;
            foreach (var csvLine in csvContent)
            {
                // skip the header line.
                if (i++ <= 0) continue;

                var values = CsvRowToStringArray(csvLine);
                if (csvImport.Columns.Count() < values.Count())
                {
                    // fix the case when the excel add the empty column to the end of the csv rows.
                    if (csvImport.Columns.Count() + 1 == values.Count())
                    {
                        values = values.Take(values.Count() - 1).ToArray();
                    }
                    else
                    {
                        throw new ApplicationException(
                            string.Format(
                                "The header column number: {0} and row column number: {1} doesn't match. Column values are: {2}.",
                                csvImport.Columns.Count(), values.Count(), string.Join(",", values)));
                    }
                }
                
                retVal.AppendLine(string.Concat(insertFormat,
                                                string.Format(" VALUES({0});",
                                                              string.Join(",", values))));
            }

            return retVal.ToString();
        }

        private static string[] CsvRowToStringArray(string row, char fieldSep = '\t', char stringSep = '\"')
        {
            var hasQuote = false;
            var stringBuilder = new StringBuilder();
            var values = new List<string>();

            foreach (var character in row)
            {
                if ((character == fieldSep && !hasQuote))
                {
                    values.Add(stringBuilder.ToString());
                    stringBuilder.Clear();
                }
                else if (character == stringSep)
                {
                    hasQuote = !hasQuote;
                }
                else
                {
                    stringBuilder.Append(character);
                }
            }

            values.Add(stringBuilder.ToString());

            return values.Select(retItem => string.Concat("N'", retItem.Replace("'", "''"), "'")).ToArray();
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