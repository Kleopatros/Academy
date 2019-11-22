using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Academy.Cs.Nonces
{
    /// <summary>
    /// Consumes the daily biometric files of a Google Fit export, extracts the average weight, and
    /// consolidates all records in a single output file.
    /// </summary>
    /// <remarks>
    /// Relevant columns:
    ///      [0] Start time
    ///      [1] End time
    ///     [12] Average weight (kg)
    ///     [13] Max weight (kg)
    ///     [14] Min weight (kg)
    /// </remarks>
    [TestClass]
    public class N20190901GoogleFit
    {
        [TestMethod]
        public void Main()
        {
            // Must contain the daily summaries (".\yyyy-MM-dd.csv")
            const string RootPath = @"";

            string[] files = Directory.GetFiles(RootPath, "*.csv");
            foreach (string file in files.Where(x => Regex.IsMatch(x, @"\d{4}-\d{2}-\d{2}\.csv")))
            {
                // Record date is the file name, sans extension
                string date = file.Split('\\').Last().Split('.')[0];
                using (StreamReader stream = File.OpenText(file))
                {
                    string line = stream.ReadLine();
                    string header = line;
                    while ((line = stream.ReadLine()) != null)
                    {
                        string[] columns = line.Split(',');
                        string timeAsString = columns[0]; // "Start time"
                        string valueAsString = columns[12]; // "Average weight (kg)"

                        if (!string.IsNullOrWhiteSpace(valueAsString))
                        {
                            // Start time column excludes the date, includes an invalid suffix, and is in the incorrect timezone
                            DateTime timestamp = DateTime.Parse($"{date} {timeAsString.Substring(0, 5)}").AddHours(-3);
                            string timestampAsString = timestamp.ToString("yyyy-MM-dd HH:mm");

                            // Convert to lbs
                            double value = double.Parse(valueAsString) * 2.20462;

                            File.AppendAllText(Path.Combine(RootPath, "results.csv"), $"{timestampAsString},{value}\r\n");
                        }
                    }
                }
            }
        }
    }
}