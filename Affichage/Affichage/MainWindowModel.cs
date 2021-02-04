using LiveCharts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Affichage
{
    class MainWindowModel : INotifyPropertyChanged
    {

        public SeriesCollection ChartData { get; set; }

        public ObservableCollection<BattementCardiaque> listeBattement { get; set; }
        public void AddItemToList(int _temps, int _battement)
        {
            listeBattement.Add(new BattementCardiaque(_temps, _battement));
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
            listeBattement = new ObservableCollection<BattementCardiaque>();
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
