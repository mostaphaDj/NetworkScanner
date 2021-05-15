using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Open.Nat;
using Prism.Mvvm;


namespace NetworkScanner.Models
{
    /// <summary>
    /// يحتوي جميع IP Addresses
    /// و IPAdresses Range
    /// التي قمت تحديدها للبحث
    /// </summary>
    public class IPAddresses : BindableBase
    {
        // private IPAddress _current; // ip البحث الحالي
        private int _rangeIndex; // مجال البحث الحالي
        private bool _isCompleted; // هل تم اكتمال البحث أم لا

        private IPAddresses()
        {
            // تحميل جميع IPAddress و IPAddress Ranges المخزنة
            //if (WPF.Properties.Settings.Default.IPAddressRanges != null)
            //{
            //    foreach (string range in WPF.Properties.Settings.Default.IPAddressRanges)
            //    {
            //        string[] s = range.Split(new[] { " -> " }, StringSplitOptions.RemoveEmptyEntries);
            //        Ranges.Add(new IPAdressesRange { From = IPAddress.Parse(s[0]), To = IPAddress.Parse(s[1]) });
            //    }
            //}
            Ranges.CollectionChanged += Ranges_CollectionChanged;
        }

        private static IPAddresses _ipAddresses;

        public static IPAddresses GetIPAddresses()
        {
            if (_ipAddresses is null)
                _ipAddresses = new IPAddresses();

            return _ipAddresses;
        }

        private void Ranges_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {

            if (e.Action == NotifyCollectionChangedAction.Remove || e.Action == NotifyCollectionChangedAction.Add || e.Action == NotifyCollectionChangedAction.Reset)
            {
                if (WPF.Properties.Settings.Default.IPAddressRanges is null)
                    WPF.Properties.Settings.Default.IPAddressRanges = new StringCollection();

                WPF.Properties.Settings.Default.IPAddressRanges.Clear();
                WPF.Properties.Settings.Default.IPAddressRanges.AddRange(Ranges.ToList().ConvertAll(range => range.ToString()).ToArray());
                WPF.Properties.Settings.Default.Save();

                calculCount();
            }
        }

        private void calculCount()
        {
            long count = 1;
            foreach (IPAdressesRange range in Ranges)
            {
                int Address1 = BitConverter.ToInt32(range.From.GetAddressBytes().Reverse().ToArray(), 0);
                int Address2 = BitConverter.ToInt32(range.To.GetAddressBytes().Reverse().ToArray(), 0);
                count += Address2 - Address1;
            }
            Count = Convert.ToInt32(count);
        }

        public ObservableCollection<IPAdressesRange> Ranges { get; } = new ObservableCollection<IPAdressesRange>();

        public IPAddress Current = new IPAddress(0);
        //{
        //    get => _current;
        //    private set
        //    {
        //        SetProperty(ref _current, value);
        //        RaisePropertyChanged();
        //    }
        //}

        public int Count { set; get; }

        private int _pingedCount;
        public int PingedCount
        {
            get => _pingedCount;
            private set
            {
                PercentComplete = _pingedCount * 100 / Count;
                SetProperty(ref _pingedCount, value);
                RaisePropertyChanged();
            }
        }

        private int _percentComplete;
        public int PercentComplete
        {
            get => _percentComplete;
            private set
            {
                SetProperty(ref _percentComplete, value);
                RaisePropertyChanged();
            }
        }

        private int _repliedCount;
        public int RepliedCount
        {
            get => _repliedCount;
            set
            {
                SetProperty(ref _repliedCount, value);
                RaisePropertyChanged();
            }
        }

        public bool isCompleted
        {
            get => _isCompleted;
            private set
            {
                SetProperty(ref _isCompleted, value);
                RaisePropertyChanged();
            }
        }

        public void Increment()
        {
            Current.Increment();
            RaisePropertyChanged("Current");

            if (Current.Compare(Ranges[_rangeIndex].To) == 1)
            {
                if (_rangeIndex < Ranges.Count - 1)
                {
                    _rangeIndex++;
                    Current.Address = Ranges[_rangeIndex].From.Address;
                }
                else
                    isCompleted = true;
            }

            Global.IPAddresses.PingedCount++;
        }

        public void GoToFirst()
        {
            if (Ranges.Count < 1)
                return;

            Current.Address = Ranges[0].From.Address;
            _rangeIndex = 0;
            PingedCount = 1;
            RepliedCount = 0;
            isCompleted = false;

        }

        public IPAddress Last()
        {
            if (Ranges.Count < 1)
                return null;
            return Ranges[Ranges.Count - 1].To;
        }
    }

    public class IPAdressesRange
    {
        public IPAddress From { get; set; }
        public IPAddress To { get; set; }

        public override string ToString()
        {
            if (From.Address == To.Address)
                return From.ToString();

            byte[] byteFrom = From.GetAddressBytes();
            byte[] byteTo = To.GetAddressBytes();

            if (byteFrom[3] == byteTo[3])
                return $"{From}/24";

            if (byteFrom[2] == byteTo[2] && byteFrom[3] == byteTo[3])
                return $"{From}/16";

            if (byteFrom[1] == byteTo[1] && byteFrom[2] == byteTo[2] && byteFrom[3] == byteTo[3])
                return $"{From}/8";

            return From + " -> " + To;
        }
    }

    public static class IPAdressesExtension
    {
        public static void Increment(this IPAddress ipAddress)
        {
            //byte[] ipAddressBytes = ipAddress.GetAddressBytes();

            //int i = ipAddressBytes.Length - 1;
            //while (true)
            //{
            //    ipAddressBytes[i]++;
            //    if (ipAddressBytes[i] == 0)
            //        i--;
            //    else
            //        break;

            //    if (i < 0)
            //        break;
            //}

            //ipAddress.Address = BitConverter.ToInt32(ipAddressBytes, 0);

            int ip = BitConverter.ToInt32(ipAddress.GetAddressBytes().Reverse().ToArray(), 0);
            ip++;
            ipAddress.Address = BitConverter.ToInt32(BitConverter.GetBytes(ip).Reverse().ToArray(), 0);

        }

        public static int Compare(this IPAddress ipAddress1, IPAddress ipAddress2)
        {
            byte[] ipAddressBytes = ipAddress1.GetAddressBytes();
            byte[] ipAddressBytes1 = ipAddress2.GetAddressBytes();

            for (int i = 0; i < 4; i++)
            {
                if (ipAddressBytes[i] > ipAddressBytes1[i])
                    return 1;
                if (ipAddressBytes[i] < ipAddressBytes1[i])
                    return -1;
            }

            //ipAddress = new IPAddress(ipAddress.GetAddressBytes().Reverse().ToArray());
            //ipAddress1 = new IPAddress(ipAddress1.GetAddressBytes().Reverse().ToArray());

            //if (ipAddress1.Address < ipAddress.Address)
            //    return 1;
            //if (ipAddress1.Address > ipAddress.Address)
            //    return -1;

            return 0;
        }

        public static async Task<IPAddress> GetExternalIPAddressAsync()
        {
            IPAddress ipAddress = null;
            try
            {
                NatDiscoverer natDiscoverer = new NatDiscoverer();
                var device = await natDiscoverer.DiscoverDeviceAsync();
                var ip = await device.GetExternalIPAsync();
                //Console.WriteLine("The external IP Address is: {0} ", ip);

                NatDiscoverer discoverer = new NatDiscoverer();

                NatDevice natDevice = await Task.Run(() => discoverer.DiscoverDeviceAsync());
                ipAddress = await Task.Run(() => natDevice.GetExternalIPAsync());
            }
            catch
            {
                // ignored
            }
            return ipAddress;
        }

        public static IPAddress[] GetInternalIPAddress()
        {
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            return host.AddressList.Where((ip) => ip.AddressFamily == AddressFamily.InterNetwork).ToArray();
        }
    }
}