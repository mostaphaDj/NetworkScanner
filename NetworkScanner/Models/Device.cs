using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;

namespace NetworkScanner.Models
{
    /// <summary>
    /// الأجهزة التي تم العثور عليها
    /// </summary>
    public class Device:BindableBase
    {
        public Device(IPAddress ipAddress)
        {
            IPAddress = ipAddress;
        }

        private IPAddress _ipAddress;
        private PhysicalAddress _macAddress;
        private string _hostname;
        private string _messageResponse;
        private string _os;
        private string _username;
        private string _password;
        private string _replyStatus;
        private string _lockState;
        private long _timeout;

        public void GetInfo()
        {
           GetHostnameAsync(_ipAddress);
        }

        public IPAddress IPAddress
        {
            get => _ipAddress;
            set
            {
                SetProperty(ref _ipAddress, value);
                RaisePropertyChanged();
            }
        }

        public PhysicalAddress MACAddress
        {
            get => _macAddress;
            set
            {
                SetProperty(ref _macAddress, value);
                RaisePropertyChanged();
            }
        }

        public string Hostname
        {
            get => _hostname;
            set
            {
                SetProperty(ref _hostname, value);
                RaisePropertyChanged();
            }
        }
        public string MessageResponse
        {
            get => _messageResponse;
            set
            {
                SetProperty(ref _messageResponse, value);
                RaisePropertyChanged();
            }
        }

        public string Username
        {
            get => _username;
            set
            {
                SetProperty(ref _username, value);
                RaisePropertyChanged();
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                SetProperty(ref _password, value);
                RaisePropertyChanged();
            }
        }

        public string OS
        {
            get => _os;
            set
            {
                SetProperty(ref _os, value);
                RaisePropertyChanged();
            }
        }

        public long Timeout
        {
            get => _timeout;
            set
            {
                SetProperty(ref _timeout, value);
                RaisePropertyChanged();
            }
        }

        public string ReplyStatus
        {
            get => _replyStatus;
            set
            {
                SetProperty(ref _replyStatus, value);
                RaisePropertyChanged();
            }
        }

        public string LockState
        {
            get => _lockState;
            set
            {
                SetProperty(ref _lockState, value);
                RaisePropertyChanged();
            }
        }


        public override string ToString()
        {
            return IPAddress+" " + Hostname + " " + Timeout ;
        }

        private void GetHostnameAsync(IPAddress ipAddress)
        {
            Task.Run(() =>
            {
                try
                {
                    IPHostEntry ipHostEntry = Dns.GetHostEntry(ipAddress);
                    if (ipHostEntry.HostName != null)
                        Hostname = ipHostEntry.HostName;
                }
                catch
                {
                    // ignored
                }
            });

        }
    }

    //public class Router : Device
    //{
    //    public string AdslUsername { get; set; }
    //    public string AdslPassword { get; set; }
    //    public string WifiName { get; set; }
    //    public string WifiUsername { get; set; }
    //    public string WifiPassword { get; set; }
    //}

    //public class PC : Device
    //{

    //}
}
