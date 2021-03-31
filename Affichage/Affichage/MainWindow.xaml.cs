﻿using LiveCharts;
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
        private int timeBattementCardiaque = 0;
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
        }

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
            double maxValue = 0;
            double freqMaxValue = 0;
            for (int i = 1; i < tmp.Length/2; i++)
            {
                //Console.WriteLine(Math.Abs(Math.Sqrt(Math.Pow(tmp[i].Real, 2) + Math.Pow(tmp[i].Imaginary, 2))) + "         " + (Math.Pow(tmp[i].Real, 2) + Math.Pow(tmp[i].Imaginary, 2)) + "         " + Math.Sqrt(Math.Pow(tmp[i].Real, 2) + Math.Pow(tmp[i].Imaginary, 2)));
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
            timeBattementCardiaque = timeBattementCardiaque + 10;
            AddBattementCardiaque(new BattementCardiaque(timeBattementCardiaque, Convert.ToInt32(freqMaxValue * 60)));

        }


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
                    if (value >= 1000 && value <= 10000)
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
                    else if(i == data.Length - 1)
                    {
                        lastInfo = data[i];
                    }


                }
            }
            return lastInfo;
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
                        
                        responseData = responseData + Encoding.ASCII.GetString(dataReceive, 0, bytes);
                        //Console.Write(responseData);
                        responseData = processData(responseData);
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
