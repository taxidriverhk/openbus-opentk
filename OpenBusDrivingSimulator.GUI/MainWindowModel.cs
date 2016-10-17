using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using OpenBusDrivingSimulator.Core;
using OpenBusDrivingSimulator.Game;

namespace OpenBusDrivingSimulator.GUI
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

    public class MainWindowModel : WindowModel
    {
        public MainWindowModel()
        {
            strings = new MainWindowModelStrings();
        }

        private MainWindowModelStrings strings;
        public MainWindowModelStrings Strings
        {
            get { return strings; }
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
