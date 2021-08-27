using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using factorial;
using System.Threading;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace WpfForm
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static int i = 0;
        public static int fact1 = 0;
        public static int fact2 = 0;
        public static int fact3 = 0;
        public static System.Windows.Forms.WebBrowser mtbDate = new System.Windows.Forms.WebBrowser();
        string connectionString;
        SqlDataAdapter adapter;
        DataTable DataGridTable;
        public MainWindow()
        {
            InitializeComponent();
            connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Page1();
            Page3();
        }

        private void Page1()
        {
            System.Windows.Forms.Integration.WindowsFormsHost hostBrowser =
                new System.Windows.Forms.Integration.WindowsFormsHost();
            System.Windows.Forms.Integration.WindowsFormsHost hostButton =
                new System.Windows.Forms.Integration.WindowsFormsHost();

            string address = "https://yandex.ru/";

            mtbDate.Navigate(new Uri(address));
            hostBrowser.Child = mtbDate;
            this.gridBrowser.Children.Add(hostBrowser);

            System.Windows.Forms.Button refr = new System.Windows.Forms.Button();
            refr.Text = "Обновить страницу";
            refr.Size = new System.Drawing.Size(60, 20);
            refr.Location = new System.Drawing.Point(50, 50);
            refr.Click += new EventHandler(but_Click);
            hostButton.Child = refr;
            this.gridButton.Children.Add(hostButton);
        }

        private void but_Click(object sender, EventArgs e)
        {
            mtbDate.Refresh();
            i++;
            this.Title = "Страница обновлена " + i + " раз";
        }

        private static void ThreeTasks()
        {
            Task<int>[] tasks3 = new Task<int>[3]
            {
                new Task<int>(() => fact1 = factorial.factorial.fact(10, true)),
                new Task<int>(() => fact2 = factorial.factorial.fact(10, true)),
                new Task<int>(() => fact3 = factorial.factorial.fact(10, true))
            };

            foreach (var t in tasks3)
                t.Start();
            Task.WaitAll(tasks3);

        }

        private void SummFactorial()
        {
            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, (ThreadStart)delegate () 
            {
                int f1 = Convert.ToInt32(fact1lab.Content);
                int f2 = Convert.ToInt32(fact1lab.Content);
                int f3 = Convert.ToInt32(fact1lab.Content);
                int sum = f1+f2+f3;
                factSumlab.Content = sum;
            });
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Thread thread = new Thread(new ThreadStart(ThreeTasks));
            thread.Start();
            thread.Join();
            fact1lab.Content = fact1;
            fact2lab.Content = fact2;
            fact3lab.Content = fact3;
            Thread disp = new Thread(SummFactorial);
            disp.Start();
        }
        private void Page3()
        {
            UpdateDB();
        }

        private void DelString_Click(object sender, RoutedEventArgs e)
        {
            if (datagrid1.SelectedItems != null)
            {
                for (int i = 0; i < datagrid1.SelectedItems.Count; i++)
                {
                    DataRowView datarowView = datagrid1.SelectedItems[i] as DataRowView;
                    if (datarowView != null)
                    {
                        string sql = "delete from Users where Number = '"+ datarowView.Row.ItemArray[0] + "'";
                        DataGridTable = new DataTable();
                        SqlConnection connection = null;
                        try
                        {
                            connection = new SqlConnection(connectionString);
                            SqlCommand command = new SqlCommand(sql, connection);
                            adapter = new SqlDataAdapter(command);

                            connection.Open();
                            adapter.Fill(DataGridTable);
                            datagrid1.ItemsSource = DataGridTable.DefaultView;
                        }
                        catch (Exception ex)
                        {
                            System.Windows.MessageBox.Show(ex.Message);
                        }
                        finally
                        {
                            if (connection != null)
                                connection.Close();
                            UpdateDB();
                        }
                    }
                }
            }
        }

        private void UpdateDB()
        {
            string sql = "select * from Users order by Number";
            DataGridTable = new DataTable();
            SqlConnection connection = null;
            try
            {
                connection = new SqlConnection(connectionString);
                SqlCommand command = new SqlCommand(sql, connection);
                adapter = new SqlDataAdapter(command);

                adapter.InsertCommand = new SqlCommand("", connection);
                adapter.InsertCommand.CommandType = CommandType.StoredProcedure;
                adapter.InsertCommand.Parameters.Add(new SqlParameter("@Number", SqlDbType.NVarChar, 50, "Number"));
                adapter.InsertCommand.Parameters.Add(new SqlParameter("@FIO", SqlDbType.NVarChar, 50, "FIO"));
                adapter.InsertCommand.Parameters.Add(new SqlParameter("@Login", SqlDbType.Int, 0, "Login"));

                connection.Open();
                adapter.Fill(DataGridTable);
                datagrid1.ItemsSource = DataGridTable.DefaultView;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
            finally
            {
                if (connection != null)
                    connection.Close();
            }
        }

        private void AddString_Click(object sender, RoutedEventArgs e)
        {
            if (textboxFIO.Text != "" && textboxLogin.Text != "")
            {
                string sql = "insert into Users values('"+textboxFIO.Text+"', '"+textboxLogin.Text+"')";
                DataGridTable = new DataTable();
                SqlConnection connection = null;
                try
                {
                    connection = new SqlConnection(connectionString);
                    SqlCommand command = new SqlCommand(sql, connection);
                    adapter = new SqlDataAdapter(command);
                    adapter.InsertCommand = new SqlCommand("", connection);
                    adapter.InsertCommand.CommandType = CommandType.StoredProcedure;
                    adapter.InsertCommand.Parameters.Add(new SqlParameter("@FIO", SqlDbType.NChar, 100, "textboxFIO"));
                    adapter.InsertCommand.Parameters.Add(new SqlParameter("@Login", SqlDbType.NChar, 100, "textboxLogin"));
                    connection.Open();
                    adapter.Fill(DataGridTable);
                    datagrid1.ItemsSource = DataGridTable.DefaultView;
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message);
                }
                finally
                {
                    if (connection != null)
                        connection.Close();
                    UpdateDB();
                    textboxFIO.Clear();
                    textboxLogin.Clear();
                }
            }
        }
    }
}
