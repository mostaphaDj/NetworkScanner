using System.Collections.ObjectModel;
using NetworkScanner.Models;
using NetworkScanner.Services;
using Prism.Mvvm;

namespace NetworkScanner.ViewModels
{
    public class MainViewModel : BindableBase
    {
        public MainViewModel()
        {
            Commands = new MainViewCommands(this);
            
        }

        public IPAddresses IPAddresses { get; } = Global.IPAddresses;
        public Scanner Scanner { get; } = Global.Scanner;

        public ObservableCollection<ushort> Ports { get; } = new ObservableCollection<ushort>();
        public ObservableCollection<Device> Devices { get; } = Global.Devices;
        public MainViewCommands Commands { get; }
    }
}
