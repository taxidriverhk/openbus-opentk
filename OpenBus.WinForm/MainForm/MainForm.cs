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
        private MainFormModel mainFormModel;
        private MainFormStrings mainFormStrings;
        private MainFormInterface mainFormInterface;

        public MainForm()
        {
            InitializeComponent();

            mainFormModel = new MainFormModel();
            mainFormInterface = new MainFormInterface(this);
            mainFormStrings = new MainFormStrings();

            // Load customized UI settings
            MaximizeBox = false;
            MaximumSize = Size;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            StartPosition = FormStartPosition.CenterScreen;
            Icon = new Icon(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
                + Constants.PATH_DELIM + Constants.APPLICATION_ICON);
            Text = mainFormStrings.Title;

            mapLabel.Text = mainFormStrings.Map;
            mapList.Items.AddRange(mainFormModel.MapNameList);
            mapList.SelectedIndex = 0;

            startGameButton.Text = mainFormStrings.StartGame;
        }

        private void startGameButton_Click(object sender, EventArgs e)
        {
            if (mapList.SelectedIndex < 0)
                return;
            string mapToLoad = mainFormModel.GetMapPath(
                mapList.Items[mapList.SelectedIndex].ToString());
            mainFormInterface.StartGame(mapToLoad);
        }
    }
}
