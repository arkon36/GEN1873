using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Helpers;
using LiveCharts.Wpf;
using OxyPlot;
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
        private double timeInterval = 0.05;
        private double time = 0;
        private bool appRunning;
        private double voltTemp = 0;
        private bool ascend = true;
        private Thread ReceiveInfo;
        private List<double> voltage = new List<double>();


        public MainWindow()
        {
            InitializeComponent();
            model = new MainWindowModel();
            this.DataContext = model;
            appRunning = true;

            

            //LoadCsvFile();

            ReceiveInfo = new Thread(new ThreadStart(listen));
            ReceiveInfo.Start();

            //Thread addinfo = new Thread(new ThreadStart(tempo));
            //addinfo.Start();

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

        public void AddInfoToGraph(double[] data)
        {


            for(int i = 0; i<data.Length-1; i++)
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    
                    if (model.ChartDataVoltage[0].Values != null)
                    {
                        if (model.ChartDataVoltage[0].Values.Count > 6000)
                            model.ChartDataVoltage[0].Values.RemoveAt(0);

                        model.ChartDataVoltage[0].Values.Add(new Voltage(time, data[i]));
                    }
                    else
                    {
                        model.ChartDataVoltage[0].Values = new ChartValues<Voltage> { new Voltage(time, data[i]) };
                    }

                }), DispatcherPriority.Background);
                time += 0.250;
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

        /*  public void fft(Double[] data, int sampleTime)
          {

              for(int i = 0; i<)




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

          */
        public void FFT(double[] value)
        {

            if (model.ChartSpectre[0].Values != null)
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    model.ChartSpectre[0].Values.Clear();
                }), DispatcherPriority.Normal);

            }

           // double[] fundamental = MathNet.Numerics.Generate.Sinusoidal(200, 20, 1, 1);

            System.Numerics.Complex[] tmp = new System.Numerics.Complex[value.Length];
            double sampleRate = 20;
            double hzPerSample = sampleRate / value.Length;
            for (int i = 0; i < value.Length; i++)
            {
                tmp[i] = new System.Numerics.Complex(value[i], 0);

            }
            MathNet.Numerics.IntegralTransforms.Fourier.Forward(tmp, MathNet.Numerics.IntegralTransforms.FourierOptions.Matlab);
            for (int i = 0; i < tmp.Length/2+1; i++)
            {
                //Console.WriteLine(Math.Abs(Math.Sqrt(Math.Pow(tmp[i].Real, 2) + Math.Pow(tmp[i].Imaginary, 2))) + "         " + (Math.Pow(tmp[i].Real, 2) + Math.Pow(tmp[i].Imaginary, 2)) + "         " + Math.Sqrt(Math.Pow(tmp[i].Real, 2) + Math.Pow(tmp[i].Imaginary, 2)));
                double mag = (2.0 / tmp.Length) * (Math.Abs(Math.Sqrt(Math.Pow(tmp[i].Real, 2) + Math.Pow(tmp[i].Imaginary, 2))));

                if (model.ChartSpectre[0].Values != null)
                {
                    Dispatcher.Invoke(new Action(() =>
                    {
                       model.ChartSpectre[0].Values.Add(new Spectre(hzPerSample * i, mag));
                    }), DispatcherPriority.Normal);
                }
                else
                {
                    Dispatcher.Invoke(new Action(() =>
                    {
                        model.ChartSpectre[0].Values = new ChartValues<Spectre> { new Spectre(hzPerSample * i, mag) };
                    }), DispatcherPriority.Normal);
                }
            }
        }


        public void processData(string info, int index)
        {
            info = info.Trim('\r', '\n');
            String[] data = info.Split(new string[] { "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            //if (index > 0)
            //{
            //    while(threads[index - 1].IsAlive)
            //    {
            //        Thread.Sleep(1);
            //    }
            //}
            for (int i = 0; i < data.Length; i++)
            {
                if (double.TryParse(data[i], NumberStyles.Any, CultureInfo.InvariantCulture, out double value))
                {
                    //Il faut un buffer sur les donné lu, parce que les valeur recu sur c# ne sont pas entiere (ex: 1 suivi de 930 au lieu de 1930)
                    //donc il faut eviter que si une de ces valeur se retrouve a la fin du string representant toutes les valeur lu dans l'intervalle
                    //de temps, il ne faut pas que cette valeur coupé en deux soit considérer dans l'affichage
                    if (value >= 0 && value <= 10000)
                    {
                        value = value * 3.3 / 4096;
                        
                        Dispatcher.BeginInvoke(new Action(() =>
                        {

                            if (model.ChartDataVoltage[0].Values != null)
                            {
                                if (model.ChartDataVoltage[0].Values.Count > 200)
                                    model.ChartDataVoltage[0].Values.RemoveAt(0);

                                model.ChartDataVoltage[0].Values.Add(new Voltage(time, value));
                                time = time + timeInterval;
                            }
                            else
                            {
                                model.ChartDataVoltage[0].Values = new ChartValues<Voltage> { new Voltage(time, value) };
                                time = time + timeInterval;
                            }

                        }), DispatcherPriority.Normal);
                        voltage.Add(value);
                        if(voltage.Count >= 200)
                        {
                            double[] array = voltage.ToArray();
                            voltage.Clear();
                            FFT(array);
                            
                        }
                    }

                }
            }
        }

        public void listen()
        {
            TcpClient client = null;
            
            try
            {
                List<double> data = new List<double>();
                int PORT_NO = 8888;
                string SERVER_IP = "192.168.0.179";

                //---create a TCPClient object at the IP and port no.---
                client = new TcpClient(SERVER_IP, PORT_NO);
                Console.WriteLine("Connection Open");
                while (appRunning)
                {
                    NetworkStream nwStream = client.GetStream();


                    Byte[] dataReceive = new Byte[4096];
                    string responseData = string.Empty;
                    Int32 bytes;
                    while ((bytes = nwStream.Read(dataReceive, 0, dataReceive.Length)) != 0)
                    {
                        
                        responseData = Encoding.ASCII.GetString(dataReceive, 0, bytes);
                        Console.Write(responseData);
                        processData(responseData, 0);
                        Thread.Sleep(1000);
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
