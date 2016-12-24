using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using OpenBus.Common;
using OpenBus.Game;
using OpenBus.Game.Objects;
using OpenBus.WPF.Model;

namespace OpenBus.WPF.ViewModel
{
    public class MainWindowModelStrings
    {
        public string Title { get; set; }
        public string StartGame { get; set; }
        public string Map { get; set; }
        public string VersionNumber { get; set; }

        public MainWindowModelStrings()
        {
            Title = Constants.APPLICATION_NAME;
            Map = "Map: ";
            StartGame = "Start Game";
            VersionNumber = Constants.VERSION_NUMBER;
        }
    }

    public class MainWindowSize
    {
        public int Width { get; set; }
        public int Height { get; set; }

        public MainWindowSize()
        {
            Width = Constants.DEFAULT_SCREEN_WIDTH;
            Height = Constants.DEFAULT_SCREEN_HEIGHT;
        }
    }

    public class MainWindowViewModel : WindowViewModel
    {
        private int selectedMapListIndex;
        private MainWindowModelStrings strings;
        private MainWindowSize size;
        private MainWindowModel model;

        private ICommand startGameCommand;
        private BitmapFrame icon;

        public MainWindowViewModel()
        {
            strings = new MainWindowModelStrings();
            size = new MainWindowSize();
            model = new MainWindowModel();
            icon = BitmapFrame.Create(new Uri(Constants.APPLICATION_ICON, UriKind.Relative));
            selectedMapListIndex = 0;
        }
        
        public MainWindowModelStrings Strings
        {
            get { return strings; }
        }
        
        public MainWindowSize ScreenSize
        {
            get { return size; }
        }

        public ObservableCollection<MainWindowModel.MapListItem> MapList
        {
            get
            {
                ObservableCollection<MainWindowModel.MapListItem> retList 
                    = new ObservableCollection<MainWindowModel.MapListItem>();
                foreach (MainWindowModel.MapListItem mapInfo in model.MapList)
                    retList.Add(mapInfo);
                return retList;
            }
        }

        public int SelectedMapListIndex
        {
            get { return selectedMapListIndex; }
            set { selectedMapListIndex = value; }
        }

        public BitmapFrame Icon
        {
            get { return icon; }
        }  

        public SolidColorBrush VersionLabelColor
        {
            get
            {
                if (strings.VersionNumber.Contains("Debug"))
                    return new SolidColorBrush(Color.FromRgb(230, 255, 60));
                else if (strings.VersionNumber.Contains("Development"))
                    return new SolidColorBrush(Color.FromRgb(255, 50, 50));
                else
                    return new SolidColorBrush(Color.FromRgb(200, 200, 200));
            }
        }

        public ICommand StartGameCommand
        {
            get 
            { 
                if(startGameCommand == null)
                {
                    startGameCommand = new ApplicationCommand(
                        () => StartGame(), true);
                }
                return startGameCommand;
            }
        }

        private void StartGame()
        {
            IsVisible = false;
            MainLoop.SetParameters(MapList[selectedMapListIndex].Path);
            MainLoop.Start();
            IsVisible = true;
        }
    }
}
