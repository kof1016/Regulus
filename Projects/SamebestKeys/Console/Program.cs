﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var view = new Regulus.Utility.ConsoleViewer();
            var input = new Regulus.Utility.ConsoleInput(view);
            
            var application = new Regulus.Project.SamebestKeys.Console(view, input);

            BotSet bots = new BotSet();


            application.SetLogMessage(Regulus.Utility.Console.LogFilter.All);

            Regulus.Utility.Updater<Regulus.Utility.IUpdatable> updater = new Regulus.Utility.Updater<Regulus.Utility.IUpdatable>();
            
            updater.Add(application);
            updater.Add(input);
            updater.Add(bots);

            bool exit = false;
            application.Command.Register("quit", () => { exit = true; });
            application.Command.Register<int>("Bot", (count) => 
            {
                bots.Create(count);
            });
            application.SelectSystemEvent += (selector) =>
            {
                var value = selector.Use("remoting");
                value.OnValue += _SpawnController;
                value.OnValue += (requester) => 
                {
                    bots.Requester = requester;
                };
            };

            Regulus.Utility.TimeCounter fps = new Regulus.Utility.TimeCounter(); ;
            while (exit == false)
            {
                if (fps.Second > 1.0 / 60)
                {
                    updater.Update();
                    fps.Reset();
                }
            }
            application.Command.Unregister("Bot");
            application.Command.Unregister("quit");
            application.Stop();

            
        }


        

        private static void _Spawn(Regulus.Remoting.Value<Regulus.Game.ConsoleFramework<Regulus.Project.SamebestKeys.IUser>.IUserRequester> value)
        {
            value.OnValue += _SpawnController;
        }

        static void _SpawnController(Regulus.Game.ConsoleFramework<Regulus.Project.SamebestKeys.IUser>.IUserRequester obj)
        {
            obj.Spawn("jc", true);
        }

        
    }
}
