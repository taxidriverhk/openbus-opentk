using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenBusDrivingSimulator.GUI
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            App game = new App();
            game.InitializeComponent();
            game.Run();
        }
    }
}
