using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Helpers;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Affichage
{
    class MainWindowModel : INotifyPropertyChanged
    {

        

        public SeriesCollection ChartData { get; set; }
        public SeriesCollection ChartDataVoltage { get; set; }
        public SeriesCollection ChartSpectre { get; set; }

        private ZoomingOptions _zoomingMode;
        public ZoomingOptions ZoomingMode
        {
            get { return _zoomingMode; }
            set
            {
                _zoomingMode = value;
                OnPropertyChanged();
            }
        }

        private ZoomingOptions _zoomingModeVoltage;
        public ZoomingOptions ZoomingModeVoltage
        {
            get { return _zoomingModeVoltage; }
            set
            {
                _zoomingModeVoltage = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<BattementCardiaque> ListeBattement { get; set; }

        public IList<OxyPlot.DataPoint> spectralAnalysis { get; set; }

        private int lastBattement;
        public int LastBattement
        {
            get
            {
                return lastBattement;
            }
            set
            {
                lastBattement = value;
                OnPropertyChanged("LastBattement");
            }
        }

        public MainWindowModel()
        {
            ListeBattement = new ObservableCollection<BattementCardiaque>();

            spectralAnalysis = new ObservableCollection<OxyPlot.DataPoint>();
            

            ChartData = new SeriesCollection()
            {
                new LineSeries()
                {
                Configuration = new CartesianMapper<BattementCardiaque>()
                .X(point => point.Temps) // Define a function that returns a value that should map to the x-axis
                .Y(point => point.Battement), // Define a function that returns a value that should map to the y-axis
                Title = "Battement Cardiaque à chaque seconde",
                Values = ListeBattement.AsChartValues(),
                PointGeometry = DefaultGeometries.Circle
                }
            };

            ZoomingMode = ZoomingOptions.Xy;

            ChartDataVoltage = new SeriesCollection()
            {
                new LineSeries()
                {
                Configuration = new CartesianMapper<Voltage>()
                .X(point => point.temps) // Define a function that returns a value that should map to the x-axis
                .Y(point => point.voltage), // Define a function that returns a value that should map to the y-axis
                Title = "Battement Cardiaque",
                PointGeometry = DefaultGeometries.Circle
                }
            };

            ChartDataVoltage = new SeriesCollection()
            {
                new LineSeries()
                {
                Configuration = new CartesianMapper<Voltage>()
                .X(point => point.temps) // Define a function that returns a value that should map to the x-axis
                .Y(point => point.voltage), // Define a function that returns a value that should map to the y-axis
                Title = "Réponse brute du radar",
                PointGeometry = null
                
                }
            };

            ChartSpectre = new SeriesCollection()
            {
                new LineSeries()
                {
                Configuration = new CartesianMapper<Spectre>()
                .X(point => point.frequence) // Define a function that returns a value that should map to the x-axis
                .Y(point => point.amplitude), // Define a function that returns a value that should map to the y-axis
                Title = "Spectre d'Amplitude",
                Fill = System.Windows.Media.Brushes.Transparent,
                PointGeometry = DefaultGeometries.Circle
                }
            };
           // ChartArea ca = ChartSpectre.ChartAreas[0];

          //  Axis ax = ca.AxisX;

            ZoomingModeVoltage = ZoomingOptions.Xy;

        }

        #region INotifyPropertyChanged Members  

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            if (PropertyChanged != null) PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion


    }


    class BattementCardiaque : INotifyPropertyChanged
    {

        private int battement;
        private int temps;
        public int Battement
        {
            get
            {
                return battement;
            }
            set
            {
                battement = value;
                OnPropertyChanged("Battement");
            }
        }
        public int Temps
        {
            get
            {
                return temps;
            }
            set
            {
                temps = value;
                OnPropertyChanged("Temps");
            }
        }

        public BattementCardiaque(int _temps, int _battement)
        {
            Temps = _temps;
            Battement = _battement;
        }



        #region INotifyPropertyChanged Members  

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }

    public class Voltage
    {
        public double voltage;
        public double temps;

        public Voltage(double _temps, double _voltage)
        {
            temps = _temps;
            voltage = _voltage;
        }
    }

    public class Spectre
    {
        public double amplitude;
        public double frequence;

        public Spectre(double _frequence, double _amplitude)
        {
            frequence = _frequence;
            amplitude = _amplitude;
        }
    }

}
