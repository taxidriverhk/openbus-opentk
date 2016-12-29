using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenBus.Game;

namespace OpenBus.WPF
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            MainLoop.ReadProgramArgs(args);
            App game = new App();
            game.InitializeComponent();
            game.Run();
        }
    }
}
