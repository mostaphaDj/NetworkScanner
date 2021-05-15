using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using NetworkScanner.Models;
using NetworkScanner.WPF;

namespace NetworkScanner.Services
{
    /// <summary>
    /// متفحص الشبكة
    /// </summary>
    public class Scanner
    {
        private static Scanner _scanner;

        public delegate void PingReplyDelegate(PingReply pingReply, int Number);
        public delegate void ProgressDelegate(IPAddress ipAddress, int Number);
        public delegate void CompleteDelegate();

        public event PingReplyDelegate OnPingReply;
        public event ProgressDelegate OnProgress;
        public event CompleteDelegate OnComplete;

        public Thread Thread { get; private set; }

        /// <summary>
        /// حالة التفحص حاليا
        /// </summary>
        public ScanState State
        {
            get
            {
                if (Thread is null)
                    return ScanState.Stopped;

                switch (Thread.ThreadState)
                {
                    case ThreadState.Running:
                        return ScanState.Running;
                    case ThreadState.Suspended:
                    case ThreadState.SuspendRequested:
                        return ScanState.Suspended;
                    case ThreadState.Stopped:
                    case ThreadState.Unstarted:
                    case ThreadState.StopRequested:
                    case ThreadState.Aborted:
                    case ThreadState.AbortRequested:
                        return ScanState.Stopped;

                    case ThreadState.Background:
                        throw new NotImplementedException();

                    // or Thread Sleep
                    // or waiting on a thread synchronization
                    // or lock
                    // Thread Join
                    case ThreadState.WaitSleepJoin:
                        throw new NotImplementedException();
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        /// <summary>
        /// عدد الثريدات التي يجب انشائها
        /// </summary>
        public int PingerCount { get; set; } = 300;

        /// <summary>
        /// 
        /// </summary>
        public int Timeout { get; set; } = 2000;

        private readonly object _lockObject = new object();

        private void CreatePingers()
        {
            for (int i = 0; i < PingerCount; i++)
            {
                if (Global.IPAddresses.isCompleted)
                    return;

                Ping ping = new Ping();
                ping.PingCompleted += pingCompleted;

                ping.SendPingAsync(Global.IPAddresses.Current, Timeout);

                lock (_lockObject)
                {
                    Global.IPAddresses.Increment();
                }
            }
        }

        private void pingCompleted(object sender, PingCompletedEventArgs e)
        {
            if (!Global.IPAddresses.isCompleted)
            {
                lock (_lockObject)
                {
                    Global.IPAddresses.Increment();

                    Ping ping = (Ping)sender;
                    ping.SendPingAsync(Global.IPAddresses.Current, Timeout);
                }
            }
            else
            {
                OnComplete?.Invoke();
                (sender as Ping).Dispose();
            }


            if (e.Reply.Status == IPStatus.Success)
            {
                Insert(e.Reply);
                lock (_lockObject)
                {
                    Global.IPAddresses.RepliedCount++;
                }

                OnPingReply?.Invoke(e.Reply, Global.IPAddresses.RepliedCount);
            }

            OnProgress?.Invoke(Global.IPAddresses.Current, Global.IPAddresses.PingedCount);
        }

        private void Insert(PingReply pingReply)
        {
            Task.Run(() =>
            {
                for (int i = 0; i < Global.Devices.Count; i++)
                {
                    if (pingReply.Address.Compare(Global.Devices[i].IPAddress) == -1)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            Device device = new Device(pingReply.Address) { Timeout = pingReply.RoundtripTime };
                            Global.Devices.Insert(i, device);
                            device.GetInfo();
                        });

                        return;
                    }
                }

                Application.Current.Dispatcher.Invoke(() =>
                {
                    Device device = new Device(pingReply.Address) { Timeout = pingReply.RoundtripTime };
                    Global.Devices.Add(device);
                    device.GetInfo();
                });
            });
        }


        public static Scanner GetScanner()
        {
            if (_scanner is null)
                _scanner = new Scanner();

            return _scanner;
        }

        public void Start()
        {
            if (State != ScanState.Running || State != ScanState.Suspended)
            {
                Global.Devices.Clear();
                Global.IPAddresses.GoToFirst();
                Thread?.Abort();

                Thread = new Thread(CreatePingers);
                Thread.Start();
            }
        }

        public void Restore()
        {
            Start();
        }

        public void Stop()
        {
            Thread.Abort();
        }

        /// <summary>
        /// Pause
        /// </summary>
        public void Suspend()
        {
            Thread.Suspend();
        }

        public void Resume()
        {
            Thread.Resume();
        }
    }

    public enum ScanState
    {
        Running = 0,
        Stopped = 1,
        Suspended = 2
    }
}