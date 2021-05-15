using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using NetworkScanner.Models;
using NetworkScanner.WPF.Annotations;


namespace NetworkScanner.WPF.Views.MainView
{
    /// <summary>
    /// Interaction logic for IPRangeDialog.xaml
    /// </summary>
    public partial class IPRangeDialog : Window, INotifyPropertyChanged
    {
        public IPAdressesRange IPAddressRange { get; set; }

        private IPAddress _ipAddressSelected;
        public IPAddress IPAddressSelected
        {
            get => _ipAddressSelected;
            private set
            {
                _ipAddressSelected = value;
                ComboBoxClass_SelectionChanged(ComboBoxClass, null);
            }
        }

        public Dictionary<IPAddress, string> MyIPAddresses { get; } = new Dictionary<IPAddress, string>();

        public IPRangeDialog()
        {
            InitializeComponent();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            IPAddress externalIPAddress = await IPAdressesExtension.GetExternalIPAddressAsync();
            if (externalIPAddress != null)
                MyIPAddresses.Add(externalIPAddress, externalIPAddress + " (external)");

            foreach (var ipAddress in IPAdressesExtension.GetInternalIPAddress().Reverse())
                MyIPAddresses.Add(ipAddress, ipAddress + " (local)");

            if (IPAddressRange is null)
            {
                ComboBoxMyIp.SelectedItem = MyIPAddresses.First();
                IPAddressSelected = MyIPAddresses.First().Key;
                TextBoxForm.Text = IPAddressSelected.ToString();
                TextBoxTo.Text = IPAddressSelected.ToString();
                RadioButtonIP.IsChecked = true;
            }
            else
            {
                IPAddressSelected = new IPAddress(IPAddressRange.From.GetAddressBytes());
                if (IPAddressRange.From.Equals(IPAddressRange.To))
                    RadioButtonIP.IsChecked = true;
                else
                    RadioButtonRange.IsChecked = true;

                ComboBoxMyIp.Text = IPAddressSelected.ToString();

            }
        }

        private void ButtonOk_Click(object sender, RoutedEventArgs e)
        {
            if (isValid)
            {
                if (IPAddressRange is null)
                    IPAddressRange = new IPAdressesRange();

                IPAddressRange.From = _from;
                IPAddressRange.To = _to;

                DialogResult = true;
            }
        }

        private IPAddress _from;
        private IPAddress _to;

        private bool _isValid = true;
        public bool isValid
        {
            get => _isValid;
            private set
            {
                _isValid = value;
                OnPropertyChanged();
            }
        }


        private bool isIPAdrress(string input)
        {
            return Regex.IsMatch(input,
                @"^(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])$");
        }

        private void TextBoxRange_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (isIPAdrress(TextBoxForm.Text))
            {
                _from = IPAddress.Parse(TextBoxForm.Text);

                TextBoxForm.Foreground = Brushes.Black;
                TextBoxForm.Tag = "valid";
                isValid = true;
            }
            else
            {
                TextBoxForm.Foreground = Brushes.Red;
                isValid = false;
                TextBoxForm.Tag = null;
                return;
            }

            if (isIPAdrress(TextBoxTo.Text))
            {
                _to = IPAddress.Parse(TextBoxTo.Text );
                TextBoxTo.Foreground = Brushes.Black;
                TextBoxForm.Tag = "valid";
                isValid = isValid && true;
            }
            else
            {
                TextBoxTo.Foreground = Brushes.Red;
                isValid = false;
                TextBoxTo.Tag = null;
                return;
            }

            if (_from.Compare(_to) > 0)
            {
                TextBoxForm.Foreground = Brushes.Red;
                TextBoxTo.Foreground = Brushes.Red;
            }
            else
            {
                TextBoxTo.Foreground = Brushes.Black;
                TextBoxForm.Foreground = Brushes.Black;
                TextBoxTo.Tag = "valid";
                TextBoxForm.Tag = "valid";

                isValid = isValid && true;
            }
        }

        private void ComboBoxMyIp_Changed(object sender, RoutedEventArgs e)
        {
            if (!isIPAdrress(ComboBoxMyIp.Text))
            {
                ComboBoxMyIp.Foreground = Brushes.Red;

                isValid = false;
                ComboBoxMyIp.Tag = null;
                return;
            }

            IPAddressSelected = IPAddress.Parse(ComboBoxMyIp.Text);

            ComboBoxMyIp.Foreground = Brushes.Black;

            ComboBoxMyIp.Tag = "valid";
            isValid = isValid && true;
        }

        private void ComboBoxClass_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            byte[] ipAddressFromBytes = IPAddressSelected.GetAddressBytes();
            byte[] ipAddressToBytes = (byte[])ipAddressFromBytes.Clone();

            int j = Convert.ToInt32((sender as ComboBox).SelectedIndex);
            for (int i = 3; i > j; i--)
            {
                ipAddressFromBytes[i] = 0;
                ipAddressToBytes[i] = 255;
            }

            TextBoxForm.Text = string.Join(".", ipAddressFromBytes);
            TextBoxTo.Text = string.Join(".", ipAddressToBytes);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            ComboBoxClass.SelectedIndex = -1;
        }

        private void RadioButtonRange_Checked(object sender, RoutedEventArgs e)
        {
            ComboBoxClass.SelectedIndex = 2;
        }
    }
}
