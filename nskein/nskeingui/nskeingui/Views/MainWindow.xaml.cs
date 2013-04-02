using System.Diagnostics;
using System.Windows;
using System.Windows.Navigation;
using nskein;

namespace nskeingui.Views {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
            this.SeedString = "";
            this.BestHashString = "";
        }

        public string SeedString { get; set; }
        public string BestHashString { get; set; }

        private string Skeinify(SimpleSkeinManaged skein, string message) {
            message = message.Replace(" ", "").Replace("\r\n", "");
            byte[] input = Util.ByteArrayfromHex(message);
            byte[] output = skein.ComputeHash(input);

            string outputString = Util.HexStringFromByteArray(output);
            return outputString;
        }

        public void GoButton_Click(object sender, RoutedEventArgs e) {
            Skein1024Managed skein1024 = new Skein1024Managed();
            this.SeedString = this.seedtxt.Text;
            this.BestHashString = Skeinify(skein1024, this.SeedString);
            this.BestHashString = Skeinify(skein1024, BestHashString);
            this.outputtxt.Text = this.BestHashString;

        }
    }
}
