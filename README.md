# How to bind the SQL Database in UWP Chart?

This example illustrates how to establish the SQL connection and bind the retrieving data from database in a step by step process:

**Step 1:**  Create a ViewModel class that establishes a connection to your SQLite database, executes a query, and retrieves the data into an ObservableCollection<ChartDataItem>. The database file is created and seeded in the app’s local folder on first run in this example.
```csharp
public class ViewModel
{
    public ObservableCollection<ChartDataItem> DataTable { get; } = new ObservableCollection<ChartDataItem>();

    . . .

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

                // Seed a few sample points
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
```

**Step 2:** Set up the [SfChart](https://help.syncfusion.com/cr/uwp/Syncfusion.UI.Xaml.Charts.SfChart.html) control with appropriate axes and bind the ItemsSource of a chart series to the DataTable property from your ViewModel. Specify the XBindingPath and YBindingPath to map to the respective columns in your database table.
```xml
<Grid>
    <Grid.DataContext>
        <local:ViewModel/>
    </Grid.DataContext>

    <chart:SfChart Margin="10">

        <chart:SfChart.PrimaryAxis>
            <chart:NumericalAxis RangePadding="Additional" />
        </chart:SfChart.PrimaryAxis>

        <chart:SfChart.SecondaryAxis>
            <chart:NumericalAxis RangePadding="Additional" />
        </chart:SfChart.SecondaryAxis>

        <chart:ScatterSeries ItemsSource="{Binding DataTable}"
                             XBindingPath="XValue"
                             YBindingPath="YValue" />
    </chart:SfChart>
</Grid>
```

## Output
<img width="1139" height="682" alt="image" src="https://github.com/user-attachments/assets/150d7535-ff50-436a-8fcb-0d9f4ec39bdf" />

## Troubleshooting

### Path Too Long Exception

If you are facing a "Path too long" exception when building this example project, close Visual Studio and rename the repository to a shorter name before building the project.

Refer to the knowledge base article [How to bind the SQL Database in UWP Chart](https://support.syncfusion.com/kb/article/10133/how-to-bind-the-sql-database-in-uwp-chart).
