using System.Collections.Generic;

namespace Vega.DbUpgrade.Entities
{
    public class CsvImport
    {
        public string Table { get; set; }

        public List<string> Columns { get; set; }

        public string CsvFile { get; set; }
    }
}