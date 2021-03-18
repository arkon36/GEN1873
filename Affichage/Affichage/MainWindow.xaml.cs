using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Helpers;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Globalization;
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
            
            //AddBattementCardiaque(new BattementCardiaque(0, 70));
            //AddBattementCardiaque(new BattementCardiaque(1, 73));
            //AddBattementCardiaque(new BattementCardiaque(4, 65));
            //AddBattementCardiaque(new BattementCardiaque(9, 68));


            //AddBattementCardiaque(new BattementCardiaque(10, 15));
            //AddBattementCardiaque(new BattementCardiaque(14, 28));
            //AddBattementCardiaque(new BattementCardiaque(15, 108));
            //AddBattementCardiaque(new BattementCardiaque(16, 2000));
            //AddBattementCardiaque(new BattementCardiaque(17, -2000));
            //model.LastBattement = model.ListeBattement.First().Battement;
            LoadCsvFile();

            ReceiveInfo = new Thread(new ThreadStart(listen));
            ReceiveInfo.Start();

            Thread addinfo = new Thread(new ThreadStart(tempo));
            addinfo.Start();

            //Thread t = new Thread(new ThreadStart(AddInfoToGraph));
            //t.Start();

        }

        private void AddBattementCardiaque(BattementCardiaque battement)
        {
            model.ListeBattement.Insert(0, battement);
            if (model.ChartData[0].Values != null)
            {
                if (model.ListeBattement.Count > 50)
                {
                    model.ListeBattement.RemoveAt(model.ListeBattement.Count - 1);
                    model.ChartData[0].Values.RemoveAt(0);
                }
                model.ChartData[0].Values.Add(battement);
            }
            else
            {
                model.ChartData[0].Values = model.ListeBattement.AsChartValues();
            }
            model.LastBattement = model.ListeBattement.First().Battement;

        }

        private void LoadCsvFile()
        {
            using (StreamReader sr = new StreamReader("U:\\GEN1873\\Fichier csv\\Original Clean.csv"))
            {
                //Ignore les deux première ligne
                sr.ReadLine();
                sr.ReadLine();

                string currentLine;
                while ((currentLine = sr.ReadLine()) != null)
                {
                    string[] info = currentLine.Split(',');

                    if (Double.TryParse(info[0], NumberStyles.Any, CultureInfo.InvariantCulture, out double temps) && Double.TryParse(info[1], NumberStyles.Any, CultureInfo.InvariantCulture, out double voltage))
                    {
                        temp = temps;
                        if (model.ChartDataVoltage[0].Values != null)
                        {
                            if (model.ChartDataVoltage[0].Values.Count > 150)
                                model.ChartDataVoltage[0].Values.RemoveAt(0);

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
        /*
        static void LoadMatlab(List<int> data, double time)
        {


            
            // Create the MATLAB instance
            MLApp.MLApp matlab = new MLApp.MLApp();

            // Change to the directory where the function is located
            matlab.Execute(@"cd U:\GEN1873\Affichage");

            // Define the output
            object result = null;

            int[] array = data.ToArray<int>();

            // Call the MATLAB function myfunc
            matlab.Feval("Traitement", 2, out result, array.Length, time, array);
            array = null;

            matlab.GetFullMatrix()
            // Display result
            object[] res = result as object[];

            if (res != null)
            {
                res[0].
                for(int i = 0; i< res[0]; i++)
            }

            
        }*/

        public void AddInfoToGraph()
        {
            while (true)
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    if (voltTemp < 0)
                    {
                        ascend = true;
                    }
                    else if (voltTemp > 15)
                    {
                        ascend = false;
                    }
                    if (ascend)
                        voltTemp = voltTemp + 0.1;
                    else
                        voltTemp = voltTemp - 0.1;

                    temp += 0.1;
                    if (model.ChartDataVoltage[0].Values.Count > 150)
                        model.ChartDataVoltage[0].Values.RemoveAt(0);

                    model.ChartDataVoltage[0].Values.Add(new Voltage(temp, voltTemp));

                }), DispatcherPriority.Background);
                Thread.Sleep(1000);
            }

        }


        public void tempo()
        {
            int tempo = 0;
            Random random = new Random();
            while (true)
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    AddBattementCardiaque(new BattementCardiaque(tempo, random.Next(50, 100)));
                    tempo = tempo + 1;
                }), DispatcherPriority.Background);
                Thread.Sleep(3000);
            }

        }
        
        public static void fft()
        {
            
        }


        public void listen()
        {
            TcpClient client = null;
            try
            {
                List<int> data = new List<int>();
                int volt;
                double time = 0;
                int PORT_NO = 8888;
                string SERVER_IP = "192.168.0.153";

                //---create a TCPClient object at the IP and port no.---
                client = new TcpClient(SERVER_IP, PORT_NO);

                while (true)
                {
                    Console.WriteLine("Connection Open");
                    NetworkStream nwStream = client.GetStream();

                    //---read back the text---
                    byte[] bytesToRead = new byte[client.ReceiveBufferSize];
                    int bytesRead = nwStream.Read(bytesToRead, 0, client.ReceiveBufferSize);
                    volt = BitConverter.ToInt32(bytesToRead, 0);
                    time += 0.0005;
                    data.Add(volt);
                    Console.WriteLine(volt);
                    if(time >= 1)
                    {
                        //var traitement = new Thread(() => LoadMatlab(data, time));
                        //traitement.Start();
                        time = 0;
                        data.Clear();
                    }
                    
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("SocketException: {0}", e);
            }
            finally
            {
                // Stop listening for new clients.
                client.Close();
            }
        }


    }
}
