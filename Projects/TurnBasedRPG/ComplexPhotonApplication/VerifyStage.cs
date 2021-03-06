﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.TurnBasedRPG
{
    class VerifyStage : Regulus.Game.IStage<User>
    {
        Regulus.Project.TurnBasedRPG.Verify _Verify;
        private UserRoster _UserRoster;

        public VerifyStage(UserRoster user_roster)
        {
            // TODO: Complete member initialization
            this._UserRoster = user_roster;
        }
        Regulus.Game.StageLock Regulus.Game.IStage<User>.Enter(User obj)
        {
            _Verify = new Regulus.Project.TurnBasedRPG.Verify(_UserRoster);
            _Verify.LoginSuccess += obj.OnLoginSuccess;
            _Verify.QuitEvent += obj.Quit;
           
            obj.Provider.Bind<IVerify>(_Verify);

            return null;
        }

        void Regulus.Game.IStage<User>.Leave(User obj)
        {
            obj.Provider.Unbind<IVerify>(_Verify);
        }

        void Regulus.Game.IStage<User>.Update(User obj)
        {
            
        }
    }
}
