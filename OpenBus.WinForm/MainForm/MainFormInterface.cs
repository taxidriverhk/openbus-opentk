﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenBus.Game;

namespace OpenBus.WinForm
{
    public class MainFormInterface
    {
        private MainForm form;

        public MainFormInterface(MainForm mainForm)
        {
            form = mainForm;
        }

        public void StartGame(string mapPath)
        {
            form.Hide();

            MainLoopStartParameter param = new MainLoopStartParameter();
            param.MapPath = mapPath;
            MainLoop.SetParameters(param);
            MainLoop.Start();
            form.Show();
        }
    }
}
