using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Helpers;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Affichage
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainWindowModel model;
        private double temp;
        private double voltTemp = 0;
        private bool ascend = true;
        private Thread ReceiveInfo;

        public MainWindow()
        {
            InitializeComponent();
            model = new MainWindowModel();
            this.DataContext = model;
            AddBattementCardiaque(new BattementCardiaque(0, 70));
            AddBattementCardiaque(new BattementCardiaque(1, 73));
            AddBattementCardiaque(new BattementCardiaque(4, 65));
            AddBattementCardiaque(new BattementCardiaque(9, 68));
            

            AddBattementCardiaque(new BattementCardiaque(10, 15));
            AddBattementCardiaque(new BattementCardiaque(14, 28));
            AddBattementCardiaque(new BattementCardiaque(15, 108));
            AddBattementCardiaque(new BattementCardiaque(25, 2000));
            AddBattementCardiaque(new BattementCardiaque(50, -2000));
            model.LastBattement = model.ListeBattement.First().Battement;
            LoadCsvFile();

            ReceiveInfo = new Thread(new ThreadStart(listen));
            ReceiveInfo.Start();

        }

        private void AddBattementCardiaque(BattementCardiaque battement)
        {
            model.ListeBattement.Insert(0, battement);
            if(model.ChartData[0].Values != null)
                model.ChartData[0].Values.Add(battement);
            else
                model.ChartData[0].Values = model.ListeBattement.AsChartValues();

        }

        private void LoadCsvFile()
        {
            using (StreamReader sr = new StreamReader("E:\\Alec\\Ecole\\GEN1873 Git Folder\\GEN1873\\Fichier csv\\Original Clean.csv"))
            {
                //Ignore les deux première ligne
                sr.ReadLine();
                sr.ReadLine();

                string currentLine;
                while ((currentLine = sr.ReadLine()) != null)
                {
                    string[] info = currentLine.Split(',');

                    if (Double.TryParse(info[0], out double temps) && Double.TryParse(info[1], out double voltage))
                    {
                        temp = temps;
                        if (model.ChartDataVoltage[0].Values != null)
                        {
                            if (model.ChartDataVoltage[0].Values.Count > 150)
                                model.ChartDataVoltage[0].Values.RemoveAt(0);
                            else
                                model.ChartDataVoltage[0].Values.Add(new Voltage(temps, voltage));
                        }
                        else
                        {
                            model.ChartDataVoltage[0].Values = new ChartValues<Voltage> { new Voltage(temps, voltage) };
                        }

                    }
                }
            }

        }

        /*********
         * Trouver une facon d'ajouter matlab
         * https://www.mathworks.com/matlabcentral/answers/705763-why-am-i-getting-the-error-the-type-or-namespace-mlapp-could-not-be-found-when-building-c-appli
         * */
        static void LoadMatlab(string[] args)
        {


            /*
            // Create the MATLAB instance 
            MLApp.MLApp matlab = new MLApp.MLApp();

            // Change to the directory where the function is located 
            matlab.Execute(@"cd c:\temp\example");

            // Define the output 
            object result = null;

            // Call the MATLAB function myfunc
            matlab.Feval("myfunc", 2, out result, 3.14, 42.0, "world");

            // Display result 
            object[] res = result as object[];

            */
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            Thread t = new Thread(new ThreadStart(AddInfoToGraph));
            t.Start();

        }

        public void AddInfoToGraph()
        {
            for (int i = 0; i < 160; i++)
            {
                Dispatcher.BeginInvoke(new Action(() =>
                            {
                                if(voltTemp < 0)
                                {
                                    ascend = true;
                                }
                                else if(voltTemp > 15)
                                {
                                    ascend = false;
                                }
                                if(ascend)
                                    voltTemp = voltTemp + 0.1;
                                else
                                    voltTemp = voltTemp - 0.1;

                                temp += 0.1;
                                if (model.ChartDataVoltage[0].Values.Count > 150)
                                    model.ChartDataVoltage[0].Values.RemoveAt(0);
                                else
                                    model.ChartDataVoltage[0].Values.Add(new Voltage(temp, voltTemp));

                            }), DispatcherPriority.Background);
                Thread.Sleep(30);
            }

        }

        public void listen()
        {
            TcpListener server = null;
            try
            {
                // Set the TcpListener on port 13000.
                Int32 port = 13000;
                IPAddress localAddr = IPAddress.Parse("192.168.0.162");

                // TcpListener server = new TcpListener(port);
                server = new TcpListener(localAddr, port);

                // Start listening for client requests.
                server.Start();

                // Buffer for reading data
                Byte[] bytes = new Byte[256];
                String data = null;

                // Enter the listening loop.
                while (true)
                {
                    Console.Write("Waiting for a connection... ");

                    // Perform a blocking call to accept requests.
                    // You could also use server.AcceptSocket() here.
                    TcpClient client = server.AcceptTcpClient();
                    Console.WriteLine("Connected!");

                    data = null;

                    // Get a stream object for reading and writing
                    NetworkStream stream = client.GetStream();

                    int i;

                    // Loop to receive all the data sent by the client.
                    while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        // Translate data bytes to a ASCII string.
                        data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                        Console.WriteLine("Received: {0}", data);

                        // Process the data sent by the client.
                        data = data.ToUpper();

                        byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);

                        // Send back a response.
                        stream.Write(msg, 0, msg.Length);
                        Console.WriteLine("Sent: {0}", data);
                    }

                    // Shutdown and end connection
                    client.Close();
                }
            }
            catch (System.Net.Sockets.SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            finally
            {
                // Stop listening for new clients.
                server.Stop();
            }

            Console.WriteLine("\nHit enter to continue...");
            Console.Read();
        }


    }
}
