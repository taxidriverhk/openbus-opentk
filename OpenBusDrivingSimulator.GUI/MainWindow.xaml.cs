using System.Windows;
using OpenBusDrivingSimulator.Game;

namespace OpenBusDrivingSimulator.GUI
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void StartGameButton_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            GameLoop.Start();
            this.Show();
        }
    }
}
