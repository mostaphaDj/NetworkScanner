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

namespace NetworkScanner.WPF.Views.MainView
{
    /// <summary>
    /// Interaction logic for PortsRangeDialog.xaml
    /// </summary>
    public partial class PortsRangeDialog : Window
    {
        public PortsRangeDialog()
        {
            InitializeComponent();
        }

        public ushort? Port { get; set; }

        private void ButtonOk_Click(object sender, RoutedEventArgs e)
        {
            if (ushort.TryParse(TextBoxPort.Text, out ushort port))
            {
                Port = port;
                DialogResult = true;
            }
            else
                TextBoxPort.Background = Brushes.Red;

        }

        private void TextBoxPort_TextChanged(object sender, TextChangedEventArgs e)
        {
            (sender as TextBox).Background = Brushes.White;
        }
    }
}
