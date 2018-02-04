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
        public string Map { get; set; }
        public string Title { get; set; }
        public string StartGame { get; set; }
        public string VersionNumber { get; set; }

        public MainFormStrings()
        {
            Map = "Map: ";
            Title = Constants.APPLICATION_NAME;
            StartGame = "Start Game";
            VersionNumber = Constants.VERSION_NUMBER;
        }
    }
}
