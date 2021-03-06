﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.SamebestKeys
{
    class VerifyStage : Regulus.Game.IStage<User>
    {
        Regulus.Project.SamebestKeys.Verify _Verify;
        private UserRoster _UserRoster;
        IStorage _Storage;
        public VerifyStage(UserRoster user_roster, IStorage storage)
        {
            _Storage = storage;
            this._UserRoster = user_roster;
        }
        Regulus.Game.StageLock Regulus.Game.IStage<User>.Enter(User obj)
        {
            _Verify = new Regulus.Project.SamebestKeys.Verify(_UserRoster, _Storage);
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
