using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using OpenBus.Common;

namespace OpenBus.WinForm
{
    public partial class MainForm : Form
    {
        private MainFormStrings mainFormStrings;
        private MainFormInterface mainFormInterface;

        public MainForm()
        {
            InitializeComponent();

            mainFormInterface = new MainFormInterface(this);
            mainFormStrings = new MainFormStrings();

            // Load customized UI settings
            FormBorderStyle = FormBorderStyle.FixedSingle;
            StartPosition = FormStartPosition.CenterScreen;
            Icon = new Icon(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
                + Constants.PATH_DELIM + Constants.APPLICATION_ICON);
            Text = mainFormStrings.Title;
            startGameButton.Text = mainFormStrings.StartGame;
        }

        private void startGameButton_Click(object sender, EventArgs e)
        {
            mainFormInterface.StartGame();
        }
    }
}
