﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Regulus.Project.SamebestKeysUserConsole
{
    using Regulus.Project.SamebestKeys;
    using Regulus.Project.SamebestKeys.Serializable;
    using Regulus.Remoting;    
    class CommandBinder
    {
        CommandHandler _CommandHandler;
        SamebestKeys.User _User;
        public CommandBinder(CommandHandler command_handler, SamebestKeys.User user)
        {
            _CommandHandler = command_handler;
            _User = user; 
        }
        internal void Setup()
        {
            var fw = _User as Regulus.Utility.IUpdatable;            

            _Bind(_User.VerifyProvider);
            _Bind(_User.ParkingProvider);
            _Bind(_User.PlayerProvider);
            _Bind(_User.ObservedAbilityProvider);
            _Bind(_User.MapInfomationProvider);
            _Bind(_User.TimeProvider);            

            _ObservedWatcher = new ObservedWatcher();
            _ObservedWatcher.Initial();

            Func<int, Value<string>> ping = (idx) =>
            {
                var pingVal = _User.GetPing(idx);                
                return "Ping : " + new System.TimeSpan(pingVal).TotalMilliseconds.ToString() + "ms";                
            };

            Action<Value<string>> pingResponse = res =>
            {
                string msg;
                if (res.TryGetValue(out msg))
                    Console.WriteLine(msg);
            };
            _CommandHandler.Set("Ping", _Build<int, string>(ping, pingResponse), "ping ex. Ping 0~?");
        }

        private void _Bind(Regulus.Remoting.Ghost.IProviderNotice<ITime> providerNotice)
        {
            providerNotice.Supply += _TimeSupply;
            providerNotice.Unsupply += _TimeUnsupply;
        }
        Time _Time = new Time();
        private void _TimeUnsupply(ITime obj)
        {
            _Time = null;
            _CommandHandler.Rise("QueryTime");
        }

        private void _TimeSupply(ITime obj)
        {
            _Time = new Time(obj);

            Action<Value<long>> queryTimeResult = (value) =>
            {
                value.OnValue += (res) =>
                {
                    Console.WriteLine("現在時間 server = " + new System.TimeSpan(res));
                    Console.WriteLine("現在時間 client = " + new System.TimeSpan( _Time.Ticks ));                    
                };
            };
            _CommandHandler.Set("QueryTime", _Build<long>(obj.GetTick , queryTimeResult), "查詢時間 ex. QueryTime");
            
        }

        

        private void _Bind(Regulus.Remoting.Ghost.IProviderNotice<IMapInfomation> providerNotice)
        {
            providerNotice.Supply += _MapSupply;
            providerNotice.Unsupply += _MapUnsupply;
        }

        private void _MapUnsupply(IMapInfomation obj)
        {
            
        }

        private void _MapSupply(IMapInfomation obj)
        {
            
        }

        private void _Bind(Regulus.Remoting.Ghost.IProviderNotice<IObservedAbility> providerNotice)
        {
            
            providerNotice.Supply += _ObservedSupply;
            providerNotice.Unsupply += _ObservedUnsupply;
        }
        class ObservedWatcher
        {
            List<IObservedAbility> _Unknowns = new List<IObservedAbility>();
            List<IObservedAbility> _Observers = new List<IObservedAbility>();
            public void Add(IObservedAbility obs)
            {
                _Unknowns.Add(obs);

                
            }

            internal void Remove(IObservedAbility obj)
            {
                _Unknowns.Remove(obj);
                if (_Observers.Remove(obj))
                {
                    Console.WriteLine("entiry離開" + obj.Id);
                }
            }

            
            internal void Initial()
            {
                
                
            }
            System.DateTime _UpdateInterval;
            public void Update()
            {

                if ((System.DateTime.Now - _UpdateInterval).TotalSeconds > 0.2)
                {
                    var newobss = (from unknown in _Unknowns where unknown.Id != Guid.Empty && unknown.Position != null select unknown).ToArray();
                    foreach (var newobs in newobss)
                    {
                        Console.WriteLine(String.Format("entiry進入{0}:{1},{2}", newobs.Id.ToString(), newobs.Position.X, newobs.Position.Y));
                        _Observers.Add(newobs);
                        newobs.ShowActionEvent += (mi) => { newobs_ShowActionEvent(newobs.Id, mi); };
                        newobs.SayEvent += (message) =>
                        {
                            Console.WriteLine(newobs.Id + ":" + message);
                        };
                        _Unknowns.Remove(newobs);
                    } 

                    _UpdateInterval = System.DateTime.Now;
                }
                
            }

            void newobs_ShowActionEvent(Guid id,MoveInfomation obj)
            {

                Console.WriteLine(String.Format("entiry行動{0}:{1}", id, obj.ActionStatue ));
            }

            internal void Release()
            {
                
            }
        }

         
        ObservedWatcher _ObservedWatcher;
        public void _ObservedSupply(IObservedAbility obj)
        {
            _ObservedWatcher.Add(obj);
            
        }

        void _ObservedUnsupply(IObservedAbility obj)
        {
            _ObservedWatcher.Remove(obj);
            
        }

        private void _Bind(Regulus.Remoting.Ghost.IProviderNotice<IPlayer> providerNotice)
        {
            providerNotice.Supply += _PlayerSupply;
            providerNotice.Unsupply += _PlayerUnsupply;
        }

        private void _PlayerUnsupply(IPlayer obj)
        {
            _CommandHandler.Rise("ExitWorld");
            _CommandHandler.Rise("Logout");
            _CommandHandler.Rise("SetPosition");            
            _CommandHandler.Rise("Ready");
			_CommandHandler.Rise("SetVision");
            _CommandHandler.Rise("Stop");
            _CommandHandler.Rise("Run");
            _CommandHandler.Rise("Act");
            _CommandHandler.Rise("Say");
			

            
        }

        private void _PlayerSupply(IPlayer obj)
        {
			_CommandHandler.Set("SetVision", _Build<int>(obj.SetVision), "設定視野 ex. Vision 100 100");
            _CommandHandler.Set("SetPosition", _Build<float, float>(obj.SetPosition), "設定位置 ex. SetPosition 100 100");
            _CommandHandler.Set("Ready", _Build(obj.Ready), "準備完畢 ex. Ready");
            _CommandHandler.Set("ExitWorld", _Build(obj.ExitWorld), "返回選角 ex. ExitWorld");
            _CommandHandler.Set("Logout", _Build(obj.Logout), "離開遊戲 ex. Logout");
            _CommandHandler.Set("Stop", _Build<float>(obj.Stop), "停止移動 ex. Stop 0~360");

            
            Action<float> run = (d) =>
            {
                obj.Walk(d);
            };
            _CommandHandler.Set("Run", _Build<float>(run), "移動 ex. Run 0~360");

            Action<int> act = (d) =>
            {
                if (d < Enum.GetValues(typeof(ActionStatue)).Length)
                    obj.BodyMovements((ActionStatue)d);
            };
            _CommandHandler.Set("Act", _Build<int>(act), "做動作 ex. 0 ~" + Enum.GetValues(typeof(ActionStatue)).Length.ToString());

            _CommandHandler.Set("Say", _Build<string>(obj.Say), "講話 ex. 你好");

        } 

        

        private void _Bind(Regulus.Remoting.Ghost.IProviderNotice<IParking> providerNotice)
        {
            providerNotice.Supply += _ParkingSupply;
            providerNotice.Unsupply += _ParkingUnupply;
        }

        private void _ParkingUnupply(IParking obj)
        {
            _CommandHandler.Rise("CheckActorName");
            _CommandHandler.Rise("CreateActor");
            _CommandHandler.Rise("DestroyActor");
            _CommandHandler.Rise("Back");
            _CommandHandler.Rise("QueryActors");
            _CommandHandler.Rise("Select");
        }

        private void _ParkingSupply(IParking obj)
        {
            Action<Value<bool>> checkActorNameResult = (value)=>
            {
                value.OnValue += (res) => 
                {
                    if (res)
                    {
                        Console.WriteLine("角色名稱有重複.");
                    }
                    else
                        Console.WriteLine("角色名稱沒有重複.");
                };
            };
            _CommandHandler.Set("CheckActorName", _Build<string, bool>(obj.CheckActorName, checkActorNameResult), "檢查重複名稱 ex. CheckActorName [名稱]");

            Func<string , Value<bool>> createActor = (name)=>
            {
                var ai = new EntityLookInfomation();
                ai.Name = name;
                return obj.CreateActor(ai);
            };

            Action<Value<bool>> createActorResult = (value) =>
            {
                value.OnValue += (res) =>
                {
                    if (res)
                    {
                        Console.WriteLine("角色建立成功.");
                    }
                    else
                        Console.WriteLine("角色建立失敗.");
                };
            };

            _CommandHandler.Set("CreateActor", _Build<string, bool>(createActor, createActorResult), "建立新角色 ex. CreateActor [名稱]");


            Action<Value<EntityLookInfomation[]>> destroyActorResult = (value) =>
            {
                value.OnValue += (res) =>
                {
                    _ShowActors(res);
                };
            };
            _CommandHandler.Set("DestroyActor", _Build<string, EntityLookInfomation[]>(obj.DestroyActor, destroyActorResult), "刪除角色 ex. DestroyActor [名稱]");


            _CommandHandler.Set("Back", _Build(obj.Back), "返回登入 ex. Back ");


            Action<Value<EntityLookInfomation[]>> queryActorsResult = (value) =>
            {
                value.OnValue += (res) =>
                {
                    _ShowActors(res);
                };
            };
            _CommandHandler.Set("QueryActors", _Build<EntityLookInfomation[]>(obj.QueryActors, queryActorsResult), "查詢角色 ex. QueryActors");



            Action<Value<bool>> selectResult = (value) =>
            {
                value.OnValue += (res) =>
                {
                    if (res)
                        Console.WriteLine("角色選擇正確.");
                    else
                        Console.WriteLine("角色選擇錯誤.");
                };
            };
            _CommandHandler.Set("Select", _Build<string, bool>(obj.Select, selectResult), "選擇角色 ex. select [名稱]");
        }

        private void _ShowActors(EntityLookInfomation[] ais)
        {
            foreach (var ai in ais)
            {
                Console.WriteLine(ai.Name);
            }
        }
        private void _Bind(Regulus.Remoting.Ghost.IProviderNotice<IVerify> providerNotice)
        {
            providerNotice.Supply += _VeriftSupply;
            providerNotice.Unsupply += _VeriftUnsupply;
        }

        private void _VeriftUnsupply(IVerify obj)
        {
            _CommandHandler.Rise("CreateAccount");
            _CommandHandler.Rise("Login");
        }

        private void _VeriftSupply(IVerify obj)
        {
            _CommandHandler.Set("CreateAccount", _Build<string, string , bool>(obj.CreateAccount, _CreateAccountResult), "建立帳號 ex. createaccount [帳號] [密碼]");
            _CommandHandler.Set("Login", _Build<string, string, LoginResult>(obj.Login, _LoginAccountResult), "登入 ex. login [帳號] [密碼]");
        }

        private void _LoginAccountResult(Value<LoginResult> obj)
        {
            obj.OnValue += (res) =>
            {
                if (res == LoginResult.Success)
                    Console.WriteLine("登入成功.");
                else if(res == LoginResult.Error)
                    Console.WriteLine("登入失敗.");
                else if(res == LoginResult.RepeatLogin)
                {
                    Console.WriteLine("重複登入.");
                }
            };
        }

        private void _CreateAccountResult(Value<bool> obj)
        {
            obj.OnValue += (res) => 
            {
                if (res)
                    Console.WriteLine("帳號建立成功.");
                else
                    Console.WriteLine("帳號建立失敗.");
            };
        }


        private Action<string[]> _Build(Action func)
        {
            return (args) =>
            {
                if (args.Length == 0)
                {                    
                    func.Invoke();
                }
            };
        }

        private Action<string[]> _Build<T1>(Action<T1> func)
        {
            return (args) =>
            {
                if (args.Length == 1)
                {
                    object arg0;
                    ClosuresHelper.Cnv(args[0], out arg0 , typeof(T1));
                    func.Invoke((T1)arg0);
                }
            };
        }


        private Action<string[]> _Build<T1, T2>(Action<T1, T2> func)
        {
            return (args) =>
            {
                if (args.Length == 2)
                {
                    object arg0;
                    ClosuresHelper.Cnv(args[0], out arg0, typeof(T1));
                    object arg1;
                    ClosuresHelper.Cnv(args[1], out arg1, typeof(T2));

                    func.Invoke((T1)arg0, (T2)arg1);
                }
            };
        }


        private Action<string[]> _Build<T1, T2, T3>(Action<T1, T2, T3> func)
        {
            return (args) =>
            {
                if (args.Length == 3)
                {
                    object arg0;
                    ClosuresHelper.Cnv(args[0], out arg0, typeof(T1));
                    object arg1;
                    ClosuresHelper.Cnv(args[1], out arg1, typeof(T2));
                    object arg2;
                    ClosuresHelper.Cnv(args[2], out arg2, typeof(T3));
                    

                    func.Invoke((T1)arg0, (T2)arg1, (T3)arg2 );
                }
            };
        }

        private Action<string[]> _Build<T1, T2, T3, T4>(Action<T1, T2, T3, T4> func)
        {
            return (args) =>
            {
                if (args.Length == 4)
                {
                    object arg0;
                    ClosuresHelper.Cnv(args[0], out arg0, typeof(T1));
                    object arg1;
                    ClosuresHelper.Cnv(args[1], out arg1, typeof(T2));
                    object arg2;
                    ClosuresHelper.Cnv(args[2], out arg2, typeof(T3));
                    object arg3;
                    ClosuresHelper.Cnv(args[3], out arg3, typeof(T4));
                    
                    func.Invoke((T1)arg0, (T2)arg1, (T3)arg2, (T4)arg3 );
                }
            };
        }

        delegate void Action5<T1, T2, T3, T4, T5>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);
        private Action<string[]> _Build<T1, T2, T3, T4, T5>(Action5<T1, T2, T3, T4, T5> func)
        {
            return (args) =>
            {
                if (args.Length == 5)
                {
                    object arg0;
                    ClosuresHelper.Cnv(args[0], out arg0, typeof(T1));
                    object arg1;
                    ClosuresHelper.Cnv(args[1], out arg1, typeof(T2));
                    object arg2;
                    ClosuresHelper.Cnv(args[2], out arg2, typeof(T3));
                    object arg3;
                    ClosuresHelper.Cnv(args[3], out arg3, typeof(T4));
                    object arg4;
                    ClosuresHelper.Cnv(args[4], out arg4, typeof(T5));
                    func.Invoke((T1)arg0, (T2)arg1, (T3)arg2, (T4)arg3, (T5)arg4);                    
                }
            };
        }

        private Action<string[]> _Build<TR>(Func<Value<TR>> func, Action<Value<TR>> value)
        {
            return (args) =>
            {
                if (args.Length == 0)
                {                    
                    var ret = func.Invoke();
                    if (ret != null && value != null)
                    {
                        value((Value<TR>)ret);
                    }
                }
            };
        }

        private Action<string[]> _Build<T1, TR>(Func<T1, Value<TR>> func, Action<Value<TR>> value)
        {
            return (args) =>
            {
                if (args.Length == 1)
                {
                    object arg0;

                    ClosuresHelper.Cnv(args[0], out arg0, typeof(T1));                    
                    var ret = func.Invoke((T1)arg0);
                    if (ret != null && value != null)
                    {
                        value((Value<TR>)ret);
                    }
                }
            };
        }

        private Action<string[]> _Build<T1, T2, TR>(Func<T1, T2, Value<TR>> func, Action<Value<TR>> value) 
        {
            return (args) =>
            {
                if (args.Length == 2)
                {
                    object arg0;
                    object arg1;
                    ClosuresHelper.Cnv(args[0], out arg0, typeof(T1));
                    ClosuresHelper.Cnv(args[1], out arg1, typeof(T2));
                    var ret = func.Invoke((T1)arg0, (T2)arg1);
                    if (ret != null && value != null)
                    {
                        value( (Value <TR>) ret);
                    }
                }
            };
        }

        private Action<string[]> _Build<T1, T2, T3, TR>(Func<T1, T2, T3, Value<TR>> func, Action<Value<TR>> value)
        {
            return (args) =>
            {
                if (args.Length == 3)
                {
                    object arg0;
                    ClosuresHelper.Cnv(args[0], out arg0, typeof(T1));
                    object arg1;
                    ClosuresHelper.Cnv(args[1], out arg1, typeof(T2));
                    object arg2;
                    ClosuresHelper.Cnv(args[2], out arg2, typeof(T3));                    

                    var ret = func.Invoke((T1)arg0, (T2)arg1, (T3)arg2 );
                    if (ret != null && value != null)
                    {
                        value((Value<TR>)ret);
                    }
                }
            };
        }

        private Action<string[]> _Build<T1, T2, T3, T4, TR>(Func<T1, T2, T3, T4, Value<TR>> func, Action<Value<TR>> value)
        {
            return (args) =>
            {
                if (args.Length == 4)
                {
                    object arg0;
                    ClosuresHelper.Cnv(args[0], out arg0, typeof(T1));
                    object arg1;
                    ClosuresHelper.Cnv(args[1], out arg1, typeof(T2));
                    object arg2;
                    ClosuresHelper.Cnv(args[2], out arg2, typeof(T3));
                    object arg3;
                    ClosuresHelper.Cnv(args[3], out arg3, typeof(T4));
                    
                    var ret = func.Invoke((T1)arg0, (T2)arg1, (T3)arg2, (T4)arg3);
                    if (ret != null && value != null)
                    {
                        value((Value<TR>)ret);
                    }
                }
            };
        }
        delegate TR Func5<T1, T2, T3, T4, T5,TR>(T1 arg0, T2 arg1, T3 arg2, T4 arg3, T5 arg4);
        private Action<string[]> _Build<T1, T2, T3, T4, T5, TR>(Func5<T1, T2, T3, T4, T5, Value<TR>> func, Action<Value<TR>> value)
        {
            return (args) =>
            {
                if (args.Length == 5)
                {
                    object arg0;
                    ClosuresHelper.Cnv(args[0], out arg0, typeof(T1));
                    object arg1;
                    ClosuresHelper.Cnv(args[1], out arg1, typeof(T2));
                    object arg2;
                    ClosuresHelper.Cnv(args[2], out arg2, typeof(T3));
                    object arg3;
                    ClosuresHelper.Cnv(args[3], out arg3, typeof(T4));
                    object arg4;
                    ClosuresHelper.Cnv(args[4], out arg4, typeof(T5));
                    var ret = func.Invoke((T1)arg0, (T2)arg1, (T3)arg2, (T4)arg3, (T5)arg4);
                    if (ret != null && value != null)
                    {
                        value((Value<TR>)ret);
                    }
                }
            };
        }

        internal void TearDown()
        {
            _CommandHandler.Rise("Ping");
            _ObservedWatcher.Release();
        }

        internal void Update()
        {
            if (_Time != null)
                _Time.Update();
            _ObservedWatcher.Update();
        }
    }
}
