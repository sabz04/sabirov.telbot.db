using sabirov.telbot.db.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace sabirov.telbot.db
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string receiveStarus = "";
        public MainWindow()
        {
            InitializeComponent();
            new TelApi();
            new Thread(x =>
            {
                while(true)
                    receiveStarus = TelApi.IsReceiving().ToString();
            }).Start();
            isreceiveLB.Content = receiveStarus;
        }

        private void startBTN_Click(object sender, RoutedEventArgs e)
        {
            TelApi.BotActive(true);
            isreceiveLB.Content = receiveStarus;
        }

        private void stopBTN_Click(object sender, RoutedEventArgs e)
        {
            TelApi.BotActive(false);
            isreceiveLB.Content = receiveStarus;
        }
    }
}
