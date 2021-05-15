using System.Windows;
using System.Windows.Controls;
using NetworkScanner.Models;
using Prism.Commands;


namespace NetworkScanner.WPF.Views.MainView
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : Window
    {
        public MainView()
        {
            InitializeComponent();

            //DataContext = new ViewModels.MainViewModel();
        }

        private void buttonAddIP_Click(object sender, RoutedEventArgs e)
        {
            IPRangeDialog ipRangeDialog = new IPRangeDialog { Owner = this };
            buttonAddIP.CommandParameter = (ipRangeDialog.ShowDialog() == true) ? ipRangeDialog.IPAddressRange : null;
        }

        private void buttonEditIP_Click(object sender, RoutedEventArgs e)
        {
            IPRangeDialog ipRangeDialog = new IPRangeDialog { Owner = this };
            ipRangeDialog.IPAddressRange = (IPAdressesRange) listBoxIPRange.SelectedItem;
            buttonEditIP.CommandParameter = (ipRangeDialog.ShowDialog() == true) ? ipRangeDialog.IPAddressRange : null;
        }

        private void listBoxIPRange_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            (buttonRemoveIP.Command as DelegateCommand<IPAdressesRange>).RaiseCanExecuteChanged();
            (buttonEditIP.Command as DelegateCommand<IPAdressesRange>).RaiseCanExecuteChanged();
            (buttonUpIP.Command as DelegateCommand<IPAdressesRange>).RaiseCanExecuteChanged();
            (buttonDownIP.Command as DelegateCommand<IPAdressesRange>).RaiseCanExecuteChanged();
        }

        private void buttonAddPort_Click(object sender, RoutedEventArgs e)
        {
            PortsRangeDialog portsRangeDialog = new PortsRangeDialog {Owner = this};
            buttonAddPort.CommandParameter = (portsRangeDialog.ShowDialog() == true) ? portsRangeDialog.Port : null;

        }

        private void buttonEditPort_Click(object sender, RoutedEventArgs e)
        {
            PortsRangeDialog portsRangeDialog = new PortsRangeDialog { Owner = this };
            portsRangeDialog.Port = (ushort)listBoxIPRange.SelectedItem;
            buttonEditPort.CommandParameter = (portsRangeDialog.ShowDialog() == true) ? portsRangeDialog.Port : null;

        }

        private void listBoxPorts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            (buttonRemovePort.Command as DelegateCommand<ushort?>).RaiseCanExecuteChanged();
            (buttonEditPort.Command as DelegateCommand<ushort?>).RaiseCanExecuteChanged();
            (buttonUpPort.Command as DelegateCommand<ushort?>).RaiseCanExecuteChanged();
            (buttonDownPort.Command as DelegateCommand<ushort?>).RaiseCanExecuteChanged();

        }

        private void ListBoxItem_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
                buttonEditIP_Click(buttonEditIP, null);
                (buttonEditIP.Command as DelegateCommand<IPAdressesRange>).Execute((IPAdressesRange)(sender as ListBoxItem).Content);

        }
    }
}
