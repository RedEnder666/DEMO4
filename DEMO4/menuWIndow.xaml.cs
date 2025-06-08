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
using System.Windows.Shapes;

namespace DEMO4
{
    /// <summary>
    /// Interaction logic for menuWIndow.xaml
    /// </summary>
    public partial class menuWIndow : Window
    {
        public menuWIndow()
        {
            InitializeComponent();
        }

        private void exitButton_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void partnersButton_Click(object sender, RoutedEventArgs e)
        {
            var pw = new partnersWindow();
            pw.Show();
            this.Close();
        }
    }
}
