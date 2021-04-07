using LiveCharts;
using LiveCharts.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using System.Xml;

namespace Affichage
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainWindowModel model;
        private double temp;
        private double timeInterval;
        private double time = 0;
        private int timeBattementCardiaque = 0;
        private bool appRunning;
        private Thread ReceiveInfo;
        private List<double> voltage = new List<double>();
        private int PORT_NO;
        private string SERVER_IP;
        private double minValue = 0;
        private double maxValue = 0;
        private int signalRange;


        public MainWindow()
        {
            InitializeComponent();
            model = new MainWindowModel();
            LoadConfigFile();
            this.DataContext = model;
            appRunning = true;

            //LoadCsvFile();

            ReceiveInfo = new Thread(new ThreadStart(listen));
            ReceiveInfo.Start();
        }


        /// <summary>
        /// Add a heartbeat to the graph
        /// </summary>
        /// <param name="battement">the heartbeat</param>
        private void AddBattementCardiaque(BattementCardiaque battement)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                model.ListeBattement.Insert(0, battement);
            }), DispatcherPriority.Normal);
            
            if (model.ChartData[0].Values != null)
            {
                if (model.ListeBattement.Count > 50)
                {
                    Dispatcher.Invoke(new Action(() =>
                    {
                        model.ListeBattement.RemoveAt(model.ListeBattement.Count - 1);
                        model.ChartData[0].Values.RemoveAt(0);
                    }), DispatcherPriority.Normal);
                }
                Dispatcher.Invoke(new Action(() =>
                {
                    model.ChartData[0].Values.Add(battement);
                }), DispatcherPriority.Normal);
                
            }
            else
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    model.ChartData[0].Values = model.ListeBattement.AsChartValues();
                }), DispatcherPriority.Normal); 
            }
            Dispatcher.Invoke(new Action(() =>
            {
                model.LastBattement = model.ListeBattement.First().Battement;
            }), DispatcherPriority.Normal);

            

        }

        /// <summary>
        /// Load the Config File
        /// </summary>
        private void LoadConfigFile()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(Directory.GetCurrentDirectory() + "\\ConfigFile.xml");
            XmlNode root = doc.DocumentElement.SelectSingleNode("/Affichage");

            XmlNode address = root["address"];
            SERVER_IP = address["Ip"].InnerText;
            Int32.TryParse(address["Port"].InnerText, NumberStyles.Any, CultureInfo.InvariantCulture, out PORT_NO);

            Double.TryParse(root["SamplingRate"].InnerText, NumberStyles.Any, CultureInfo.InvariantCulture, out timeInterval);

            XmlNode data = root["Data"];
            Int32.TryParse(data["Range"].InnerText, NumberStyles.Any, CultureInfo.InvariantCulture, out signalRange);
            Double.TryParse(data["Min"].InnerText, NumberStyles.Any, CultureInfo.InvariantCulture, out minValue);
            Double.TryParse(data["Max"].InnerText, NumberStyles.Any, CultureInfo.InvariantCulture, out maxValue);



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

        /// <summary>
        /// Fast fourier transform on the data
        /// </summary>
        /// <param name="value">200 real number value with the specified interval</param>
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

            //Add value to a complex
            System.Numerics.Complex[] tmp = new System.Numerics.Complex[value.Length];
            double sampleRate = 1/timeInterval;
            double hzPerSample = sampleRate / value.Length;
            for (int i = 0; i < value.Length; i++)
            {
                tmp[i] = new System.Numerics.Complex(value[i], 0);

            }

            //Process complex with Matlab FFT
            MathNet.Numerics.IntegralTransforms.Fourier.Forward(tmp, MathNet.Numerics.IntegralTransforms.FourierOptions.Matlab);
            double maxValue = 0;
            double freqMaxValue = 0;
            for (int i = 1; i < tmp.Length/2; i++)
            {
                double mag = (Math.Abs(Math.Sqrt(Math.Pow(tmp[i].Real, 2) + Math.Pow(tmp[i].Imaginary, 2))));
                if (mag > maxValue)
                {
                    maxValue = mag;
                    freqMaxValue = hzPerSample * i;
                }
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
            timeBattementCardiaque = timeBattementCardiaque + Convert.ToInt32(signalRange * timeInterval);
            AddBattementCardiaque(new BattementCardiaque(timeBattementCardiaque, Convert.ToInt32(freqMaxValue * 60)));

        }


        /// <summary>
        /// Take a string containing the data and split it with /r /n and add it to voltage graph
        /// </summary>
        /// <param name="info">string with the data unprocessed</param>
        /// <returns></returns>
        public string processData(string info)
        {
            string lastInfo = "";
            info = info.Trim('\r', '\n');
            String[] data = info.Split(new string[] { "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < data.Length; i++)
            {
                if (double.TryParse(data[i], NumberStyles.Any, CultureInfo.InvariantCulture, out double value))
                {
                    //Il faut un buffer sur les donné lu, parce que les valeur recu sur c# ne sont pas entiere (ex: 1 suivi de 930 au lieu de 1930)
                    //donc il faut eviter que si une de ces valeur se retrouve a la fin du string representant toutes les valeur lu dans l'intervalle
                    //de temps, il ne faut pas que cette valeur coupé en deux soit considérer dans l'affichage
                    if (value >= minValue && value <= maxValue)
                    {
                        //Voltage resolution
                        value = value * 3.3 / 4096;
                        
                        Dispatcher.BeginInvoke(new Action(() =>
                        {

                            if (model.ChartDataVoltage[0].Values != null)
                            {
                                if (model.ChartDataVoltage[0].Values.Count > signalRange)
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
                        //FFT on signalRange value
                        if(voltage.Count >= signalRange)
                        {
                            double[] array = voltage.ToArray();
                            voltage.Clear();
                            FFT(array);
                            
                        }
                    }
                    //If it's the last value and it's not valid keep it for the next processing
                    else if(i == data.Length - 1)
                    {
                        lastInfo = data[i];
                    }


                }
            }
            return lastInfo;
        }

        /// <summary>
        /// Listen from incomming data from the Radar
        /// </summary>
        public void listen()
        {
            TcpClient client = null;
            
            try
            {
                List<double> data = new List<double>();

                //create a TCPClient object at the IP and port no
                client = new TcpClient(SERVER_IP, PORT_NO);
                Console.WriteLine("Connection Open");
                while (appRunning)
                {
                    NetworkStream nwStream = client.GetStream();


                    Byte[] dataReceive = new Byte[4096];
                    string responseData = string.Empty;
                    Int32 bytes;
                    //Keep receiving and processing data while the radar is running
                    while ((bytes = nwStream.Read(dataReceive, 0, dataReceive.Length)) != 0)
                    {
                        
                        responseData = responseData + Encoding.ASCII.GetString(dataReceive, 0, bytes);
                        //Console.Write(responseData);
                        responseData = processData(responseData);
                        //Give Time to get a range of data
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
                try
                {
                    client.Close();
                }
                catch { }

            }

        }

        private void Window_Closed(object sender, EventArgs e)
        {
            appRunning = false;
        }
    }
}
