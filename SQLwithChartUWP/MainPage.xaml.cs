using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;


// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SQLLiteChartBinding
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        public MainPage()
        {
            this.InitializeComponent();

        }       
    }

    public class ViewModel
    {
        public ViewModel()
        {
            try
            {
                SqlConnection thisConnection = new SqlConnection(ConnectionString);
                thisConnection.Open();
                string Get_Data = "SELECT * FROM ChartData";
                SqlCommand cmd = thisConnection.CreateCommand();
                cmd.CommandText = Get_Data;
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                sda.Fill(ds);
                var table = ds.Tables[0];
                this.DataTable = table;
            }
            catch
            {
                return;
            }
        }

        public object DataTable { get; set; }

        public static string ConnectionString
        {
            get
            {
                string currentDir = System.Environment.CurrentDirectory;
                currentDir = currentDir.Substring(0, currentDir.Length - 10) + "\\LocalDataBase";
                return @"Data Source=Data Source=ata Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\RachelA\source\repos\SQLLiteChartBinding\LocalDataBase\SeriesItemsSource.mdf;Integrated Security=True;Connect Timeout=30";
            }
        }
    }
}
