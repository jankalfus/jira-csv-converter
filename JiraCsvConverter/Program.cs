using System;
using System.IO;

namespace JiraCsvConverter
{
    class Program
    {
        static void Main()
        {
            var csv = new Csv();

            using (var standardInput = Console.OpenStandardInput())
            using (var inputReader = new StreamReader(standardInput))
            {
                csv.LoadCsvWithHeaders(inputReader);
            }

            csv.RenameColumn("Issue key", "Key");
            csv.RenameColumn("Outward issue link (Blocks)", "Blocks");
            csv.RenameColumn("Outward issue link (Issue split)", "Split");

            csv.MergeColumns("Blocks");
            csv.MergeColumns("Split");

            var exportedCsv = csv.ExportColumns(new[] { "Blocks", "Split", "Summary", "Key" });

            Console.Write(exportedCsv);
        }
    }
}