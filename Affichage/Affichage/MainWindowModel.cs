using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Affichage
{
    class MainWindowModel : INotifyPropertyChanged
    {


        public BindingList<BattementCardiaque> listeBattement { get; set; }
        public void AddItemToList(string _temps, int _battement)
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
            listeBattement = new BindingList<BattementCardiaque>();
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
        private string temps;
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
        public string Temps
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

        public BattementCardiaque(string _temps, int _battement)
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
