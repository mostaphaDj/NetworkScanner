using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using NetworkScanner.Models;
using NetworkScanner.Services;

namespace NetworkScanner
{
    public static class Global
    {
        public static Scanner Scanner { get; } = Scanner.GetScanner();

        /// <summary>
        /// يحتوي جميع IP Addresses
        /// و IPAdresses Range
        /// التي قمت تحديدها للبحث
        /// </summary>
        public static IPAddresses IPAddresses { get; } = IPAddresses.GetIPAddresses();

        /// <summary>
        /// الأجهزة التي تم العثور عليها
        /// </summary>
        public static ObservableCollection<Device> Devices { get; } = new ObservableCollection<Device>();
    }
}
