using Microsoft.Data.Sqlite;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;

namespace SQLLiteChartBinding
{
    public class ViewModel
    {
        public ObservableCollection<ChartDataItem> DataTable { get; } = new ObservableCollection<ChartDataItem>();

        public ViewModel()
        {
            _ = InitializeAsync();
        }

        private async Task InitializeAsync()
        {
            try
            {
                string dbPath = await EnsureAndSeedDatabaseAsync("DataSource.sqlite");

                using (var connection = new SqliteConnection($"Data Source={dbPath}"))
                {
                    await connection.OpenAsync();

                    using (var cmd = connection.CreateCommand())
                    {
                        cmd.CommandText = "SELECT xValue, yValue FROM ChartData";
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                DataTable.Add(new ChartDataItem
                                {
                                    XValue = reader.GetDouble(0),
                                    YValue = reader.GetDouble(1)
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("SQLite init/load error: " + ex.Message);
            }
        }

        // Creates a SQLite DB in LocalFolder on first run and seeds simple rows
        private static async Task<string> EnsureAndSeedDatabaseAsync(string dbFileName)
        {
            var localFolder = ApplicationData.Current.LocalFolder;
            var existing = await localFolder.TryGetItemAsync(dbFileName);
            var dbPath = Path.Combine(localFolder.Path, dbFileName);

            if (existing == null)
            {
                using (var connection = new SqliteConnection($"Data Source={dbPath}"))
                {
                    await connection.OpenAsync();

                    // Create table
                    using (var create = connection.CreateCommand())
                    {
                        create.CommandText = @"
                            CREATE TABLE IF NOT EXISTS ChartData (
                                xValue REAL NOT NULL,
                                yValue REAL NOT NULL
                            );";
                        await create.ExecuteNonQueryAsync();
                    }

                    // Seed a few sample points (you can change these)
                    using (var tx = connection.BeginTransaction())
                    using (var insert = connection.CreateCommand())
                    {
                        insert.CommandText = "INSERT INTO ChartData (xValue, yValue) VALUES ($x, $y)";
                        var px = insert.CreateParameter(); px.ParameterName = "$x"; insert.Parameters.Add(px);
                        var py = insert.CreateParameter(); py.ParameterName = "$y"; insert.Parameters.Add(py);

                        var points = new (double x, double y)[]
                        {
                            (1, 10), (2, 14), (3, 9), (4, 18), (5, 22), (6, 17), (7, 25)
                        };

                        foreach (var (x, y) in points)
                        {
                            px.Value = x;
                            py.Value = y;
                            await insert.ExecuteNonQueryAsync();
                        }

                        tx.Commit();
                    }
                }
            }

            return dbPath;
        }
    }

    public class ChartDataItem
    {
        public double XValue { get; set; }

        public double YValue { get; set; }
    }
}
