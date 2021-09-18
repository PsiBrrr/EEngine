using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EEngine.EEngine
{
    public class Armies
    {
        public Vector2 Position = Vector2.Zero();
        public Vector2 Scale = Vector2.Zero();
        public Units Unit = null;
        public Vector2 Army_Level_Array_Position { get; private set; } = Vector2.Zero();

        public Armies(Vector2 Position, Vector2 Scale, Units Unit, int Frame, Effects HealthEffect, Effects SupplyEffect, Vector2 Army_Level_Array_Position)
        {
            try
            {
                this.Scale = Scale;
                this.Unit = new Units(Position, Scale, Unit, Frame, HealthEffect, SupplyEffect, false);
                this.Position = this.Unit.Position;
                this.Army_Level_Array_Position = Army_Level_Array_Position;

                EEngine.RegisterArmy(this);
                Log.Info($"[ARMIES]({this.Unit.Tag}) - Has been Registered!");
            }
            catch
            {
                Log.Error("[ARMIES] - Unable to Register!");
            }
        }
        public Armies(Vector2 Position, float Scale, Units Unit, int Frame, Effects HealthEffect, Effects SupplyEffect, Vector2 Army_Level_Array_Position)
        {
            try
            {
                this.Scale = Unit.Unit_Sprite[0].Scale * Scale; //Array position 0 scale should be consistant across all units
                this.Unit = new Units(Position, Scale, Unit, Frame, HealthEffect, SupplyEffect, false);
                this.Position = this.Unit.Position;
                this.Army_Level_Array_Position = Army_Level_Array_Position;

                EEngine.RegisterArmy(this);
                Log.Info($"[ARMIES]({this.Unit.Tag}) - Has been Registered!");
            }
            catch
            {
                Log.Error("[ARMIES] - Unable to Register!");
            }
        }

        public bool ArmyUnitMoveX(Vector2 TargetPosition, float Speed)
        {
            if (TargetPosition.X < Position.X)
            {
                Unit.Left();
                Position.X -= Speed;

                return true;
            }
            else if (TargetPosition.X > Position.X)
            {
                Unit.Right();
                Position.X += Speed;

                return true;
            }
            else
            {
                Unit.Idle();

                return false;
            }
        }

        public bool ArmyUnitMoveY(Vector2 TargetPosition, float Speed)
        {
            if (TargetPosition.Y < Position.Y)
            {
                Unit.Up();
                Position.Y -= Speed;

                return true;
            }
            else if (TargetPosition.Y > Position.Y)
            {
                Unit.Down();
                Position.Y += Speed;

                return true;
            }
            else
            {
                Unit.Idle();

                return false;
            }
        }

        public void DestroySelf()
        {
            Log.Info($"[ARMIES]({this.Unit.Tag}) - Has been destroyed!");
            EEngine.UnRegisterArmy(this);
        }

 

    }
}
