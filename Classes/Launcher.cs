using BTD_Backend;
using BTD_Backend.Game;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace TD_Loader.Classes
{
    class Launcher
    {
        public static void Launch(GameType game)
        {
            var launcher = new Launcher();

            TempSettings.SaveSettings();

            //return;
            if (game == GameType.BTD6)
                BgThread.AddToQueue(() => launcher.LaunchBTD6());
            if (game == GameType.BTD5 || game == GameType.BTDB || game == GameType.BMC)
                BgThread.AddToQueue(() => launcher.LaunchJetGame());
        }

        public void LaunchBTD6()
        {
            int injectWaitTime = 12000;
            var btd6Info = GameInfo.GetGame(GameType.BTD6);

            if (!BTD_Backend.Natives.Windows.IsProgramRunning(btd6Info.ProcName, out Process proc))
                Process.Start("steam://rungameid/" + btd6Info.SteamID);
            else
                injectWaitTime = 0;

            while (!BTD_Backend.Natives.Windows.IsProgramRunning(btd6Info.ProcName, out proc))
                Thread.Sleep(1000);

            Thread.Sleep(injectWaitTime);

            foreach (var modPath in SessionData.LoadedMods)
            {
                if (!File.Exists(modPath))
                {
                    Log.Output("The BTD6 mod  \"" + modPath + "\"  could not be found. Failed to inject it");
                    continue;
                }
                BTD_Backend.Natives.Injector.InjectDll(modPath, proc);
            }
        }

        public void LaunchJetGame()
        {

        }
    }
}
