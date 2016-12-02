using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenBus.Common;

namespace OpenBus.WinForm
{
    public class MainFormStrings
    {
        public string Title { get; set; }
        public string StartGame { get; set; }
        public string VersionNumber { get; set; }

        public MainFormStrings()
        {
            Title = Constants.APPLICATION_NAME;
            StartGame = Constants.START_GAME;
            VersionNumber = Constants.VERSION_NUMBER;
        }
    }
}
