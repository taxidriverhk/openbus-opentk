using System;
using System.Windows.Input;
using OpenBus.Common;
using OpenBus.Game;
using OpenBus.Game.Objects;
using OpenBus.WPF.Model;
using System.Collections.ObjectModel;

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
            StartGame = Constants.START_GAME;
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

        public MainWindowViewModel()
        {
            strings = new MainWindowModelStrings();
            size = new MainWindowSize();
            model = new MainWindowModel();
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

        private ICommand startGameCommand;
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
