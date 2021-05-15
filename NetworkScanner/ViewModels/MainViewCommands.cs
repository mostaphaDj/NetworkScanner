using System;
using System.Net;
using System.Net.NetworkInformation;
using NetworkScanner.Models;
using NetworkScanner.Services;
using NetworkScanner.WPF;
using Prism.Commands;

namespace NetworkScanner.ViewModels
{
    public class MainViewCommands
    {
        private readonly MainViewModel viewModel;

        public MainViewCommands(MainViewModel mainPageViewModel)
        {
            viewModel = mainPageViewModel;
            #region Scan Control Commands
            StartCommand = new DelegateCommand(executeStart, canCxecuteStart);
            RestoreCommand = new DelegateCommand(executeRestore, canCxecuteRestore);
            StopCommand = new DelegateCommand(executeStop, canCxecuteStop);
            SuspendCommand = new DelegateCommand(executeSuspend, canCxecuteSuspend);
            ResumeCommand = new DelegateCommand(executeResume, canCxecuteResume);
            ClearResultCommand = new DelegateCommand(executeClearResult, canCxecuteClearResult);

            #endregion

            #region IPAdresses Range Commands
            AddIPCommand = new DelegateCommand<IPAdressesRange>(executeAddIPCommand);
            RemoveIPCommand = new DelegateCommand<IPAdressesRange>(executeRemoveIP, canExecuteIPRange);
            EditIPCommand = new DelegateCommand<IPAdressesRange>(executeEditIP, canExecuteIPRange);
            UpIPCommand = new DelegateCommand<IPAdressesRange>(executeUpIP, canExecuteIPRange);
            DownIPCommand = new DelegateCommand<IPAdressesRange>(executeDownIP, canExecuteIPRange);
            ClearIPCommand = new DelegateCommand(executeClearIP, canExecuteClearIP);
            #endregion

            #region Ports Range Commands
            AddPortCommand = new DelegateCommand<ushort?>(executeAddPort);
            RemovePortCommand = new DelegateCommand<ushort?>(executeRemovePort, canExecutePort);
            EditPortCommand = new DelegateCommand<ushort?>(executeEditPort, canExecutePort);
            UpPortCommand = new DelegateCommand<ushort?>(executeUpPort, canExecutePort);
            DownPortCommand = new DelegateCommand<ushort?>(executeDownPort, canExecutePort);
            ClearPortCommand = new DelegateCommand(executeClearPort, canExecuteClearPort);
            #endregion

        }

        //#region Ping Events
        //private void onProgress(IPAddress ipAddress, int number)
        //{
        //    //App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
        //    //{
               
        //    //});
        //}

        //private void onPingReply(PingReply pingReply, int number)
        //{

        //}
        //#endregion

        #region Scan Control Commands

        public DelegateCommand StartCommand { get; }
        public DelegateCommand RestoreCommand { get; }
        public DelegateCommand StopCommand { get; }
        public DelegateCommand SuspendCommand { get; }
        public DelegateCommand ResumeCommand { get; }
        public DelegateCommand ClearResultCommand { get; }


        private bool canCxecuteStart()
        {
            return viewModel.IPAddresses.Count > 0;
        }

        private bool canCxecuteClearResult()
        {
            return viewModel.Devices.Count > 0 && (Global.Scanner.State != ScanState.Running || Global.Scanner.State != ScanState.Suspended);
        }

        private bool canCxecuteResume()
        {
            return Global.Scanner.State == ScanState.Suspended;
        }

        private bool canCxecuteSuspend()
        {
            return Global.Scanner.State == ScanState.Running;
        }

        private bool canCxecuteStop()
        {
            return Global.Scanner.State == ScanState.Running || Global.Scanner.State == ScanState.Suspended;
        }

        private bool canCxecuteRestore()
        {
           return  Global.Scanner.State != ScanState.Running && Global.IPAddresses.Count > 0;
        }

        private void executeStart()
        {
            //Global.Scanner.OnPingReply += onPingReply;
            //Global.Scanner.OnProgress += onProgress;           
            Global.Scanner.Start();
            RaiseCanExecute();
        }

        private void executeClearResult()
        {
            Global.IPAddresses.Ranges.Clear();
            RaiseCanExecute();
            ClearResultCommand.RaiseCanExecuteChanged();
        }

        private void executeResume()
        {
            Global.Scanner.Resume();
            RaiseCanExecute();
        }

        private void executeSuspend()
        {
            Global.Scanner.Suspend();
            RaiseCanExecute();
        }

        private void executeStop()
        {
            Global.Scanner.Stop();
            RaiseCanExecute();
        }

        private void executeRestore()
        {
            Global.Scanner.Restore();
            RaiseCanExecute();
        }

        private void RaiseCanExecute()
        {
            StartCommand.RaiseCanExecuteChanged();
            RestoreCommand.RaiseCanExecuteChanged();
            StopCommand.RaiseCanExecuteChanged();
            SuspendCommand.RaiseCanExecuteChanged();
            ResumeCommand.RaiseCanExecuteChanged();
        }

        #endregion

        #region IPAdresses Range Commands
        public DelegateCommand<IPAdressesRange> AddIPCommand { get; }
        public DelegateCommand<IPAdressesRange> EditIPCommand { get; }
        public DelegateCommand<IPAdressesRange> UpIPCommand { get; }
        public DelegateCommand<IPAdressesRange> DownIPCommand { get; }
        public DelegateCommand<IPAdressesRange> RemoveIPCommand { get; }
        public DelegateCommand ClearIPCommand { get; }

        private bool canExecuteIPRange(IPAdressesRange ipAddressRange)
        {
            return ipAddressRange != null;
        }

        private bool canExecuteClearIP()
        {
           return  Global.IPAddresses.Count > 0;
        }

        private void executeUpIP(IPAdressesRange ipAddressRange)
        {
            throw new NotImplementedException();
        }

        private void executeDownIP(IPAdressesRange ipAddressRange)
        {
            throw new NotImplementedException();
        }

        private void executeEditIP(IPAdressesRange ipAddressRange)
        {
            //throw new NotImplementedException();
        }

        private void executeAddIPCommand(IPAdressesRange ipAddressRange)
        {
            if (ipAddressRange is null)
                return;
            viewModel.IPAddresses.Ranges.Add(ipAddressRange);
            RaiseCanExecute();
            ClearIPCommand.RaiseCanExecuteChanged();
        }

        private void executeRemoveIP(IPAdressesRange ipAddressRange)
        {
            viewModel.IPAddresses.Ranges.Remove(ipAddressRange);
            RaiseCanExecute();
            ClearIPCommand.RaiseCanExecuteChanged();
        }

        private void executeClearIP()
        {
            viewModel.IPAddresses.Ranges.Clear();
            RaiseCanExecute();
            ClearIPCommand.RaiseCanExecuteChanged();
        }
        #endregion

        #region Ports Range Commands

        public DelegateCommand<ushort?> AddPortCommand { get; }
        public DelegateCommand<ushort?> RemovePortCommand { get; }
        public DelegateCommand<ushort?> EditPortCommand { get; }
        public DelegateCommand<ushort?> UpPortCommand { get; }
        public DelegateCommand<ushort?> DownPortCommand { get; }
        public DelegateCommand ClearPortCommand { get; }

        private bool canExecutePort(ushort? port)
        {
            return port != null;
        }

        private bool canExecuteClearPort()
        {
           return viewModel.Ports.Count > 0;
        }

        private void executeDownPort(ushort? port)
        {
            throw new NotImplementedException();
        }

        private void executeUpPort(ushort? port)
        {
            throw new NotImplementedException();
        }

        private void executeEditPort(ushort? port)
        {
            throw new NotImplementedException();
        }

        private void executeAddPort(ushort? port)
        {
            if (port is null)
                return;
            viewModel.Ports.Add(port.Value);
            ClearPortCommand.RaiseCanExecuteChanged();
        }

        private void executeRemovePort(ushort? port)
        {
            viewModel.Ports.Remove(port.Value);
            ClearPortCommand.RaiseCanExecuteChanged();
        }

        private void executeClearPort()
        {
            viewModel.Ports.Clear();
            ClearPortCommand.RaiseCanExecuteChanged();
        }


        #endregion

    }
}
