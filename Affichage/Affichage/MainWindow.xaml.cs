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
using CenterSpace.NMath.Core;

namespace Affichage
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainWindowModel model;
        private double temp;
        private int time;
        private bool appRunning;
        private double voltTemp = 0;
        private bool ascend = true;
        private Thread ReceiveInfo;
        

        public MainWindow()
        {
            InitializeComponent();
            model = new MainWindowModel();
            this.DataContext = model;
            appRunning = true;

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
            //LoadCsvFile();

            //ReceiveInfo = new Thread(new ThreadStart(listen));
            //ReceiveInfo.Start();

            Thread addinfo = new Thread(new ThreadStart(tempo));
            addinfo.Start();

            Thread t = new Thread(new ThreadStart(AddInfoToGraph));
            t.Start();

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
            using (StreamReader sr = new StreamReader("E:\\Alec\\Ecole\\GEN1873 Git Folder\\GEN1873\\Fichier csv\\Original Clean.csv"))
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
                            if (model.ChartDataVoltage[0].Values.Count > 12000)
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
            List<double> data = new List<double>();
            int volt;
            time = 0;
            Random rnd = new Random();
            Console.WriteLine("Connection Open");
            while (appRunning)
            {
                
                
                time += 10;
                volt = rnd.Next(1000, 3000);
                data.Add(volt);
                //Process data after 1 seconde
                if (time % 1000== 0)
                {
                    Double[] tempList = data.ToArray();
                    var traitement = new Thread(() => fft(tempList, time));
                    traitement.Start();
                    data.Clear();
                }
                Thread.Sleep(10);

            }

            /*
            while (appRunning)
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
                    if (model.ChartDataVoltage[0].Values.Count > 140)
                        model.ChartDataVoltage[0].Values.RemoveAt(0);

                    model.ChartDataVoltage[0].Values.Add(new Voltage(temp, voltTemp));

                }), DispatcherPriority.Background);
                Thread.Sleep(1000);
            }
            */
        }


        public void tempo()
        {
            int tempo = 0;
            Random random = new Random();
            while (appRunning)
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    AddBattementCardiaque(new BattementCardiaque(tempo, random.Next(50, 100)));
                    tempo = tempo + 1;
                }), DispatcherPriority.Background);
                Thread.Sleep(3000);
            }

        }
        
        public void fft(Double[] data, int sampleTime)
        {
            //
            // Simple example to compute a forward 1D real 1024 point FFT
            //

            // Create some random signal data.

            var dataVector = new DoubleVector(data);

            // Compute the FFT
            // This will create a complex conjugate symmetric packed result.
            var fft = new DoubleForward1DFFT(dataVector.Count());
            DoubleVector fftresult = fft.FFT(dataVector);
            double timeInDouble;
            sampleTime = sampleTime - 1000;
            
            for (int j = 0; j < fftresult.Length-1; j++)
            {
                timeInDouble = sampleTime / 1000;

                Dispatcher.BeginInvoke(new Action(() =>
                {

                    if (model.ChartDataVoltage[0].Values != null)
                    {
                        if (model.ChartDataVoltage[0].Values.Count > 6000)
                            model.ChartDataVoltage[0].Values.RemoveAt(0);

                        model.ChartDataVoltage[0].Values.Add(new Voltage(timeInDouble, fftresult[j]));
                    }
                    else
                    {
                        model.ChartDataVoltage[0].Values = new ChartValues<Voltage> { new Voltage(timeInDouble, fftresult[j]) };
                    }

                }), DispatcherPriority.Background);
                sampleTime += 10;
            }

        }


        public void listen()
        {
            TcpClient client = null;
            try
            {
                List<double> data = new List<double>();
                int volt;
                time = 0;
                int PORT_NO = 8888;
                string SERVER_IP = "192.168.0.153";

                //---create a TCPClient object at the IP and port no.---
                client = new TcpClient(SERVER_IP, PORT_NO);

                while (appRunning)
                {
                    Console.WriteLine("Connection Open");
                    NetworkStream nwStream = client.GetStream();

                    //---read back the text---
                    byte[] bytesToRead = new byte[client.ReceiveBufferSize];
                    int bytesRead = nwStream.Read(bytesToRead, 0, client.ReceiveBufferSize);
                    volt = BitConverter.ToInt32(bytesToRead, 0);
                    time += 5;
                    data.Add(volt);
                    Console.WriteLine(volt);
                    //Process data after 1 seconde
                    if(time % 1000 == 0)
                    {
                        Double[] tempList = data.ToArray();
                        var traitement = new Thread(() => fft(tempList, time));
                        traitement.Start();
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

        private void Window_Closed(object sender, EventArgs e)
        {
            appRunning = false;
        }
    }
}
