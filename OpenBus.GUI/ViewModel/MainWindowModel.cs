using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using OpenBus.Core;
using OpenBus.Game;
using OpenBus.GUI.View;

namespace OpenBus.GUI.ViewModel
{
    public class MainWindowModelStrings
    {
        public string Title { get; set; }
        public string StartGame { get; set; }
        public string VersionNumber { get; set; }

        public MainWindowModelStrings()
        {
            Title = Constants.APPLICATION_NAME;
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

    public class MainWindowModel : WindowModel
    {
        public MainWindowModel()
        {
            strings = new MainWindowModelStrings();
            size = new MainWindowSize();
        }

        private MainWindowModelStrings strings;
        public MainWindowModelStrings Strings
        {
            get { return strings; }
        }

        private MainWindowSize size;
        public MainWindowSize ScreenSize
        {
            get { return size; }
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
            MainLoop.Start();
            IsVisible = true;
        }
    }
}
