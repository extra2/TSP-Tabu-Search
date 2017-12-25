using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TSP_Tabu_Search
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string fileName = "";
        public MainWindow()
        {
            InitializeComponent();
        }

        private void buttonSelectFile_Click(object sender, RoutedEventArgs e)
        {
            FileDialog fileDialog = new OpenFileDialog();
            if (fileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                fileName = fileDialog.FileName;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int time = Int32.Parse(textBoxMaxTime.Text);
            if (fileName != "")
            {
                string status = new TSP().StartTsp(time, fileName, false);
                labelStatus.TextWrapping = TextWrapping.Wrap;
                labelStatus.Text = status;
            }
        }
        private void ButtonGenetic_Click(object sender, RoutedEventArgs e)
        {
            int time = Int32.Parse(textBoxMaxTime.Text);
        }
    }
}
