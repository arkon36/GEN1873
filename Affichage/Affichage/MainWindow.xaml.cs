using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Helpers;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace Affichage
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainWindowModel model;
        public MainWindow()
        {
            InitializeComponent();
            model = new MainWindowModel();
            this.DataContext = model;
            model.ListeBattement.Insert(0, new BattementCardiaque(0, 70));
            model.ListeBattement.Insert(0, new BattementCardiaque(1, 73));
            model.ListeBattement.Insert(0, new BattementCardiaque(4, 65));
            model.ListeBattement.Insert(0, new BattementCardiaque(9, 68));
            model.LastBattement = model.ListeBattement.First().Battement;


            model.ChartData[0].Values = model.ListeBattement.AsChartValues();

            model.ListeBattement.Insert(0, new BattementCardiaque(9, 15));
            model.ListeBattement.Insert(0, new BattementCardiaque(-4, 28));
            model.ListeBattement.Insert(0, new BattementCardiaque(15, 108));
            model.ListeBattement.Insert(0, new BattementCardiaque(100, 2000));
            model.ChartData[0].Values = model.ListeBattement.AsChartValues();
            AddBattementCardiaque(new BattementCardiaque(-100, -2000)); 
        }

        private void AddBattementCardiaque(BattementCardiaque battement)
        {
            model.ListeBattement.Insert(0, battement);
            model.ChartData[0].Values.Add(battement);
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
    }
}
