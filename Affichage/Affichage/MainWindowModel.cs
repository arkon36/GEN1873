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

        public ObservableCollection<BattementCardiaque> ListeBattement { get; set; }
        
        public void AddItemToList(int _temps, int _battement)
        {
            ListeBattement.Add(new BattementCardiaque(_temps, _battement));
        }

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

    
}
