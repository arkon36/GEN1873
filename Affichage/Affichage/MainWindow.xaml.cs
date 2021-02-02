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
        public MainWindow()
        {
            InitializeComponent();
            MainWindowModel model = new MainWindowModel();
            this.DataContext = model;
            model.listeBattement.Insert(0, new BattementCardiaque("5:35", 70));
            model.listeBattement.Insert(0, new BattementCardiaque("5:36", 73));
            model.listeBattement.Insert(0, new BattementCardiaque("5:37", 65));
            model.listeBattement.Insert(0, new BattementCardiaque("5:38", 68));
            model.LastBattement = model.listeBattement.First().Battement;
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
