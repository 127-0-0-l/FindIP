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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;

namespace FindIP
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void btnPing_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                IPAddress ipFrom = IPAddress.Parse(tbFrom.Text);
                IPAddress ipTo = IPAddress.Parse(tbTo.Text);

                List<IPAddress> ipList = GetIPList(ipFrom, ipTo);

                Ping ping = new Ping();
                ping.Send(IPAddress.Parse("192.168.100.33"));
                new Thread(() =>
                {
                    foreach (var item in ipList)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            rtbResult.AppendText(item + Environment.NewLine);
                            //rtbResult.AppendText(ping.Send(item).Status.ToString() + Environment.NewLine);
                            rtbResult.ScrollToEnd();
                        });
                        Thread.Sleep(10);
                    }
                }).Start();
            }
            catch(Exception ex)
            {
                rtbResult.AppendText(ex.Message + Environment.NewLine);
                rtbResult.ScrollToEnd();
            }
        }

        private void show(List<IPAddress> ipList)
        {
            foreach (var item in ipList)
            {
                Dispatcher.Invoke(() =>
                {
                    rtbResult.AppendText(item + Environment.NewLine);
                    rtbResult.ScrollToEnd();
                });
            }
        }

        private List<IPAddress> GetIPList(IPAddress ipFrom, IPAddress ipTo)
        {
            List<IPAddress> list = new List<IPAddress>();

            for (long i = GetLongFromIP(ipFrom); i <= GetLongFromIP(ipTo); i++)
            {
                list.Add(GetIPFromLong(i));
            }

            return list;
        }

        private long GetLongFromIP(IPAddress ip)
        {
            string[] strArr = ip.ToString().Split('.');
            int[] intArr = new int[4] { 
                int.Parse(strArr[0]),
                int.Parse(strArr[1]),
                int.Parse(strArr[2]),
                int.Parse(strArr[3]) };
            long result = (long)(intArr[0] * Math.Pow(256, 3) + intArr[1] * Math.Pow(256, 2) + intArr[2] * Math.Pow(256, 1) + intArr[3]);
            return result;
        }

        private IPAddress GetIPFromLong(long number)
        {
            string ip = "";

            for (int i = 3; i >= 0; i--)
            {
                ip += $"{number % 256}";
                if (i != 0)
                {
                    ip += ".";
                }
                number /= 256;
            }

            string[] ips = ip.Split('.');
            ip = "";

            for (int i = 3; i >= 0; i--)
            {
                ip += $"{ips[i]}";
                if (i != 0)
                {
                    ip += ".";
                }
            }

            return IPAddress.Parse(ip);
        }
    }
}
