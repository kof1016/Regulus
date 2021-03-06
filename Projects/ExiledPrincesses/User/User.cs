﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Regulus.Project.ExiledPrincesses
{
	public interface IUser : Regulus.Utility.IUpdatable
	{
        Regulus.Remoting.Ghost.IProviderNotice<Regulus.Remoting.ITime> TimeProvider { get; }
		Regulus.Remoting.Ghost.IProviderNotice<IVerify> VerifyProvider { get ; }
        Regulus.Remoting.Ghost.IProviderNotice<IUserStatus> StatusProvider { get; }
        Regulus.Remoting.Ghost.IProviderNotice<ITown> TownProvider { get; }	
        Regulus.Remoting.Ghost.IProviderNotice<IAdventure> AdventureProvider { get; }

        Regulus.Remoting.Ghost.IProviderNotice<IAdventureIdle> AdventureIdleProvider { get; }
        Regulus.Remoting.Ghost.IProviderNotice<IAdventureGo> AdventureGoProvider { get; }
        Regulus.Remoting.Ghost.IProviderNotice<IAdventureChoice> AdventureChoiceProvider { get; }

        Regulus.Remoting.Ghost.IProviderNotice<IActor> ActorProvider { get; }
        Regulus.Remoting.Ghost.IProviderNotice<ITeam> TeamProvider { get; }
        Regulus.Remoting.Ghost.IProviderNotice<ICombatController> CombatControllerProvider { get; }


        Regulus.Remoting.Ghost.IProviderNotice<T> QueryProivder<T>();
    }

    
}
