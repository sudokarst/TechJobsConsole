﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace TechJobsConsole
{
    using Job = Dictionary<string, string>;
    class JobData
    {
        static List<Job> AllJobs = new List<Job>();
        static bool IsDataLoaded = false;

        public static List<Job> FindAll()
        {
            LoadData();
            return AllJobs;
        }

        /*
         * Returns a list of all values contained in a given column,
         * without duplicates. 
         */
        public static List<string> FindAll(string column)
        {
            LoadData();

            List<string> values = new List<string>();

            foreach (Job job in AllJobs)
            {
                string aValue = job[column];

                if (!values.Contains(aValue))
                {
                    values.Add(aValue);
                }
            }
            return values;
        }

        /// <summary>
        /// Search across all columns, without duplicates </summary>
        /// <returns>
        /// A list of jobs where the value is found in any column </returns>
        public static List<Job> FindByValue(string searchFor)
        {
            // load data, if not already loaded
            LoadData();

            List<Job> results = new List<Job>();

            foreach (Job job in AllJobs)
            {
                foreach (string column in job.Values)
                {
                    if (column.IndexOf(searchFor, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        results.Add(job);
                        break; // drop back to outer loop
                    }
                }
            }
            return results;
        }

        public static List<Job> FindByColumnAndValue(string column, string searchFor)
        {
            // load data, if not already loaded
            LoadData();

            List<Job> results = new List<Job>();

            foreach (Job row in AllJobs)
            {
                string value = row[column];

                if (value.IndexOf(searchFor, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    results.Add(row);
                }
            }

            return results;
        }

        /*
         * Load and parse data from job_data.csv
         */
        private static void LoadData()
        {

            if (IsDataLoaded)
            {
                return;
            }

            List<string[]> rows = new List<string[]>();

            using (StreamReader reader = File.OpenText("job_data.csv"))
            {
                while (reader.Peek() >= 0)
                {
                    string line = reader.ReadLine();
                    string[] rowArrray = CSVRowToStringArray(line);
                    if (rowArrray.Length > 0)
                    {
                        rows.Add(rowArrray);
                    }
                }
            }

            string[] headers = rows[0];
            rows.Remove(headers);

            // Parse each row array into a more friendly Dictionary
            foreach (string[] row in rows)
            {
                Job rowDict = new Job();

                for (int i = 0; i < headers.Length; i++)
                {
                    rowDict.Add(headers[i], row[i]);
                }
                AllJobs.Add(rowDict);
            }

            IsDataLoaded = true;
        }

        /*
         * Parse a single line of a CSV file into a string array
         */
        private static string[] CSVRowToStringArray(string row, char fieldSeparator = ',', char stringSeparator = '\"')
        {
            bool isBetweenQuotes = false;
            StringBuilder valueBuilder = new StringBuilder();
            List<string> rowValues = new List<string>();

            // Loop through the row string one char at a time
            foreach (char c in row.ToCharArray())
            {
                if ((c == fieldSeparator && !isBetweenQuotes))
                {
                    rowValues.Add(valueBuilder.ToString());
                    valueBuilder.Clear();
                }
                else
                {
                    if (c == stringSeparator)
                    {
                        isBetweenQuotes = !isBetweenQuotes;
                    }
                    else
                    {
                        valueBuilder.Append(c);
                    }
                }
            }

            // Add the final value
            rowValues.Add(valueBuilder.ToString());
            valueBuilder.Clear();

            return rowValues.ToArray();
        }
    }
}
