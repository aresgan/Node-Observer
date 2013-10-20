using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.SqlServerCe;
using System.Net;
using System.Net.Sockets;
using System.Data;

namespace NodeObserver
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        string startip;
        string stopip;
        int IDb4Scan = 0;
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            listBox1.Dispatcher.BeginInvoke(new Action(delegate()
            {
                listBox1.Items.Clear();
            }));
           
            #region getLargestIDfromDB
            Database mydb = new Database();
            mydb.openDBConnection();
            IDb4Scan = mydb.GetLargestID();
            mydb.closeDBConnection();
            #endregion 

            textBox2.Dispatcher.BeginInvoke(new Action(delegate()
            {
                startip = textBox2.Text.ToString();
            }));


            //stopip = textBox3.Text.ToString();
            textBox3.Dispatcher.BeginInvoke(new Action(delegate()
            {
                stopip = textBox3.Text.ToString();
            }));

            System.Threading.Thread scanner = new System.Threading.Thread(new System.Threading.ThreadStart(Scan));
            scanner.IsBackground = true;
            scanner.Start();


        }

        public void displayToDataGrid()
        {

            //display to database.
            SqlCeConnection conn = new SqlCeConnection("Data Source = subject101.sdf; Password =''");

            SqlCeDataAdapter Adapter = null;
            //unknown

            if (comboBox1.Text.Length != 0)
            {
                Adapter = new SqlCeDataAdapter(comboBox1.Text.ToString() + " AND ID > " + IDb4Scan, conn);

            }
            else
            {
                Adapter = new SqlCeDataAdapter("Select * from Record where status <> 'UNKNOWN' AND ID > " + IDb4Scan, conn);
            }

            // Bind Data to DataSet
            DataSet ds = new DataSet();
            Adapter.Fill(ds);

            // Bind Data to DataGrid
            dataGrid1.ItemsSource = ds.Tables[0].AsDataView();
            //dataGrid1.DataContext = Bind.Tables[0];

            label9.Content = dataGrid1.Items.Count;

            // Close the Database Connection
            Adapter.Dispose();
            ds.Dispose();
            conn.Close();

        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {

            #region getLargestIDfromDB
            Database mydb = new Database();
            mydb.openDBConnection();
            int ldb = mydb.GetLargestID();
            mydb.closeDBConnection();
            #endregion 

            ldb = ldb - int.Parse(textBox4.Text);

            //display to database.
            SqlCeConnection conn = new SqlCeConnection("Data Source = subject101.sdf; Password =''");
            SqlCeDataAdapter Adapter = null;
            //unknown

            if (comboBox1.Text.Length != 0)
            {
                System.Console.WriteLine(comboBox1.Text.ToString() + " AND ID > " + ldb);
                Adapter = new SqlCeDataAdapter(comboBox1.Text.ToString() + " AND ID > " + ldb, conn);
            }
            else
            {
                Adapter = new SqlCeDataAdapter("Select * from Record where status <> 'UNKNOWN' AND ID > " + ldb, conn);
            }
            // Bind Data to DataSet
            DataSet ds = new DataSet();
            Adapter.Fill(ds);

            // Bind Data to DataGrid
            dataGrid1.ItemsSource = ds.Tables[0].AsDataView();
            //dataGrid1.DataContext = Bind.Tables[0];
            label9.Content = dataGrid1.Items.Count;
            // Close the Database Connection
            Adapter.Dispose();
            ds.Dispose();
            conn.Close();
            
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            IPHostEntry host;
            string localIP = "";
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork && System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable() == true)
                {
                    localIP = ip.ToString();
                }
            }
            textBox1.Text = localIP;
        }


        private void button4_Click(object sender, RoutedEventArgs e)
        {
            if (textBox1.Text.Length == 0)
            {
                MessageBox.Show("Enter an IP");
            }
            else
            {

                int i = 0;
                foreach (string x in textBox1.Text.Split('.'))
                {
                    i++;
                    if (i == 1)
                    {
                        textBox2.Text = x;
                        textBox3.Text = x;
                    }
                    else if (i > 1 && i < 4)
                    {
                        textBox2.Text = textBox2.Text + "." + x;
                        textBox3.Text = textBox3.Text + "." + x;
                    }
                    else if (i == 4)
                    {
                        textBox2.Text = textBox2.Text + ".1";
                        textBox3.Text = textBox3.Text + ".254";
                    }
                }
            }
        }

        public void Scan()
        {

            string netAddr = null;
            int start = 0;
            int stop = 0;

            int i = 0;
            foreach (string x in startip.Split('.'))
            {
                i++;
                if (i == 1)
                {
                    netAddr = x;
                }
                else if (i > 1 && i < 4)
                {
                    netAddr = netAddr + "." + x;

                }
                else if (i == 4)
                {
                    netAddr = netAddr + ".";
                    start = int.Parse(x);
                }
            }

            i = 0;
            foreach (string x in stopip.Split('.'))
            {
                i++;
                if (i == 4)
                {
                    stop = int.Parse(x);
                }
            }

            //System.Console.WriteLine(netAddr);
            //System.Console.WriteLine(start);
            //System.Console.WriteLine(stop);
            //System.Console.WriteLine("Staging IPs for testing.");
            listBox1.Dispatcher.BeginInvoke(new Action(delegate()
            {
                listBox1.Items.Add("Staging IPs for testing : " + DateTime.Now.ToString("HH:mm:ss tt"));
            }));

            //System.Console.WriteLine("Launching network sensor.");
            listBox1.Dispatcher.BeginInvoke(new Action(delegate()
            {
                listBox1.Items.Add("Launching network sensor : " + DateTime.Now.ToString("HH:mm:ss tt"));
            }));

            NetworkSensor[] ns = new NetworkSensor[256];
            for (int j = start; j <= stop; j++)
            {
                ns[j] = new NetworkSensor(netAddr + j.ToString());
            }

            listBox1.Dispatcher.BeginInvoke(new Action(delegate()
            {
                listBox1.Items.Add("Waiting for sensor reply : " + DateTime.Now.ToString("HH:mm:ss tt"));
            }));

            for (int j = start; j <= stop; j++)
            {
                ns[j].sense.Join();
            }
            //System.Console.WriteLine("Sensing completed.");
            listBox1.Dispatcher.BeginInvoke(new Action(delegate()
            {
                listBox1.Items.Add("Scanning completed : " + DateTime.Now.ToString("HH:mm:ss tt"));
            }));


            listBox1.Dispatcher.BeginInvoke(new Action(delegate()
            {
                listBox1.Items.Add("Storing to database : " + DateTime.Now.ToString("HH:mm:ss tt"));
            }));

            #region write2DB
            Database mydb = new Database();
            mydb.openDBConnection();
            for (int j = start; j <= stop; j++)
            {
                mydb.insertRecord(ns[j].day, ns[j].date, ns[j].time, ns[j].ip, ns[j].mac, ns[j].hostname, ns[j].status);
            }
            mydb.closeDBConnection();
            #endregion 

            listBox1.Dispatcher.BeginInvoke(new Action(delegate()
            {
                listBox1.Items.Add("Process completed : " + DateTime.Now.ToString("HH:mm:ss tt"));
                displayToDataGrid();
            }));



        }


        private bool cont = true; 
        private void button5_Click(object sender, RoutedEventArgs e)
        {
            label7.Content = "ON";
            cont = true;
            int slpduration = int.Parse(textBox5.Text.ToString()); 
            System.Threading.Thread timer = new System.Threading.Thread(new System.Threading.ThreadStart(delegate()
            {
                while (cont)
                {
                    button1_Click(sender, e);
                    System.Threading.Thread.Sleep(slpduration * 1000);
                    //System.Console.WriteLine("Testing" + DateTime.Now.ToString("HH:mm:ss tt")) ;
                }
            }));
            timer.IsBackground = true;
            timer.Start();
        }

        private void button6_Click(object sender, RoutedEventArgs e)
        {
            label7.Content = "OFF";
            cont = false; 
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
            comboBox1.Items.Add("Select * from Record where status <> 'UNKNOWN'");
            comboBox1.Items.Add("Select * from Record where 1=1");
            comboBox1.Items.Add("Select * from Record where status = 'UP'");
            comboBox1.Items.Add("Select * from Record where status = 'DOWN'");
            comboBox1.SelectedIndex = 0;
            //System.Console.WriteLine(comboBox1.Text.ToString());
        }

    }
}
