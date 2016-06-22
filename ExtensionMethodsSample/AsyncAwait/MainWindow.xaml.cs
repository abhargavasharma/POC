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
using System.Net.Http;


namespace AsyncAwait
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }


        // . . .
        private async void startButton_Click(object sender, RoutedEventArgs e)
        {
            // ONE
            Task<int> getLengthTask = AccessTheWebAsync();

            // FOUR
            int contentLength = await getLengthTask;

            // SIX
            resultsTextBox.Text +=
                String.Format("\r\nLength of the downloaded string: {0}.\r\n", contentLength);
        }


        async Task<int> AccessTheWebAsync()
        {
            // TWO
            HttpClient client = new HttpClient();
            Task<string> getStringTask =
                client.GetStringAsync("http://msdn.microsoft.com");

            // THREE                 
            string urlContents = await getStringTask;

            // FIVE
            return urlContents.Length;
        }


    }
}
