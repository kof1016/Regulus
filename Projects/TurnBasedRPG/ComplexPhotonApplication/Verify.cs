﻿using Regulus.Project.TurnBasedRPG.Serializable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.TurnBasedRPG
{
    class Verify : IVerify
    {
        
        Regulus.Remoting.Value<bool> IVerify.CreateAccount(string name, string password)
        {

            if (Regulus.Utility.Singleton<Storage>.Instance.FindAccountInfomation(name) == null)
            {
                AccountInfomation ai = new AccountInfomation();
                ai.Name = name;
                ai.Password = password;
                ai.Id = Guid.NewGuid();
                Regulus.Utility.Singleton<Storage>.Instance.Add(ai);
                return true;
            }
            return false;
        }

        public event Action<Serializable.AccountInfomation> LoginSuccess;
        private UserRoster _UserRoster;

        public Verify(UserRoster user_roster)
        {
            // TODO: Complete member initialization
            this._UserRoster = user_roster;

            
        }
        Regulus.Remoting.Value<LoginResult> IVerify.Login(string name, string password)
        {
            var user = _UserRoster.Find(name);
            if (user == null)
            {
                var ai = Regulus.Utility.Singleton<Storage>.Instance.FindAccountInfomation(name);
                if (ai != null && ai.Password == password)
                {
                    LoginSuccess(ai);                    
                    return LoginResult.Success;
                }
            }
            else
            {
                
                user.Logout();
                return LoginResult.RepeatLogin;
            }
            return LoginResult.Error;
            
        }


        public event Action QuitEvent;
        void IVerify.Quit()
        {
            if (QuitEvent != null)
            {
                QuitEvent();
                QuitEvent = null;
            }

            
        }

        
        
    }
}
