using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FileSplit
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            if(fileDialog.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = fileDialog.FileName;
            }
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(textBox1.Text != string.Empty)
            {

                using (StreamReader sr = new StreamReader(textBox1.Text))
                {
                    string line;
                    string saveText = "";
                    //skip first two line
                    sr.ReadLine();
                    sr.ReadLine();

                    int compt = 0;
                    int fileCompt = 0;
                    while ((line = sr.ReadLine()) != null)
                    {
                        compt++;
                        saveText = saveText + line + "\n";
                        if(compt == 40)
                        {
                            File.WriteAllText(Path.GetDirectoryName(textBox1.Text) + "\\" + fileCompt + ".csv", saveText);
                            fileCompt++;
                            compt = 0;
                            saveText = "";
                        }
                    }
                }


            }
        }
    }
}
