#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace JiraCsvConverter
{
    public class Csv
    {
        private List<List<string>> data = new List<List<string>>();

        private List<string> GetHeaders() => data.First();

        public void LoadCsvWithHeaders(StreamReader reader, string delimiter = ",")
        {
            string? line;

            while ((line = reader.ReadLine()) != null)
            {
                var values = line.Split(delimiter);
                data.Add(new List<string>(values));
            }

            if (data.Count < 2)
            {
                throw new ArgumentException("There are less than 2 lines in the CSV input, you might want to check it's correct!");
            }
        }

        public void RenameColumn(string from, string to)
        {
            var headers = GetHeaders();
            for (var i = 0; i < headers.Count; i++)
            {
                if (headers[i] == from)
                {
                    headers[i] = to;
                }
            }
        }

        public void MergeColumns(string header, string delimiter = ":")
        {
            var headers = GetHeaders();
            var originalIndex = -1;
            var copyIndexes = new List<int>();
            for (var i = 0; i < headers.Count; i++)
            {
                if (headers[i] == header)
                {
                    if (originalIndex == -1)
                    {
                        originalIndex = i;
                    }
                    else
                    {
                        copyIndexes.Add(i);
                    }
                }
            }

            if (originalIndex == -1)
            {
                throw new InvalidOperationException("Header not found");
            }

            for (var i = 0; i < data.Count; i++)
            {
                var line = data[i];

                if (i != 0) // skip headers
                {
                    var original = line[originalIndex];
                    var copies = copyIndexes.Select(copyIndex => line[copyIndex]).Where(copy => copy.Trim() != "");
                    line[originalIndex] = string.Join(delimiter, new List<string> { original }.Concat(copies));
                }

                // remove copies
                for (var j = copyIndexes.Count - 1; j >= 0; j--)
                {
                    line.RemoveAt(copyIndexes[j]);
                }
            }
        }

        public string ExportColumns(string[] columnHeaders, string delimiter = ",")
        {
            var headers = GetHeaders();

            var columnIndexesToExport = new List<int>();
            for (var i = 0; i < headers.Count; i++)
            {
                if (columnHeaders.Any(columnHeader => headers[i] == columnHeader))
                {
                    columnIndexesToExport.Add(i);
                }
            }

            var csv = new StringBuilder();

            foreach (var line in data)
            {
                var lineSelection = columnIndexesToExport.Select(i => line[i]);
                csv.AppendLine(string.Join(delimiter, lineSelection));
            }

            return csv.ToString();
        }
    }
}