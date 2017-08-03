using System.Collections.Generic;

namespace Vega.DbUpgrade.Entities
{
    public class CsvImport
    {
        public string OperationId { get; set; }

        public string Table { get; set; }

        public string Delimiter { get; set; }

        public List<string> Columns { get; set; }

        public List<string> CsvFiles { get; set; }
    }
}