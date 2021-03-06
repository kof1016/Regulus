﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Regulus.Project.TurnBasedRPG
{
   

    class PortalEntity : Entity, IObserveAbility
    {
        string _TargetMap;
        Types.Vector2 _TargetPosition;
        Types.Vector2 _Position;
        Regulus.Types.Rect _Vision;

        public PortalEntity(Guid id, Regulus.Types.Rect vision,string target_map, Types.Vector2 target_position)
            : base(id)
        {
            _TargetPosition = new Types.Vector2();
            _TargetPosition.X = target_position.X;
            _TargetPosition.Y = target_position.Y;
            _TargetMap = target_map;
            _Vision = vision;

            _Position = new Types.Vector2();
            _Position.X = (float)(_Vision.X + _Vision.Width /2);
            _Position.X = (float)(_Vision.Y + _Vision.Height / 2);
        }
        
        protected override void _SetAbility(Entity.AbilitySet abilitys)
        {
            abilitys.AttechAbility<IObserveAbility>(this);
        }

        protected override void _RiseAbility(Entity.AbilitySet abilitys)
        {
            abilitys.DetechAbility<IObserveAbility>();
        }

        void IObserveAbility.Update(Regulus.Project.TurnBasedRPG.Map.EntityInfomation[] observeds, List<IObservedAbility> lefts)
        {
            foreach (var observed in observeds)
            {
                if (observed.Cross != null)
                {
                    observed.Cross.Move(_TargetMap, Regulus.Utility.ValueHelper.DeepCopy(_TargetPosition) );
                }
            }
        }

        Types.Vector2 IObserveAbility.Position
        {
            get { return _Position; }
        }

        Regulus.Types.Rect IObserveAbility.Vision
        {
            get { return _Vision; }
        }

        event Action<IObservedAbility> IObserveAbility.IntoEvent
        {
            add { }
            remove { }
        }

        event Action<IObservedAbility> IObserveAbility.LeftEvent
        {
            add { }
            remove { }
        }
    }
}
