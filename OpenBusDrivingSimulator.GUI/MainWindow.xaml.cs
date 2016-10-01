using System.Windows;
using OpenBusDrivingSimulator.Game;

namespace OpenBusDrivingSimulator.GUI
{
    /// <summary>
    /// 
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
