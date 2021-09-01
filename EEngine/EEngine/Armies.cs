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

        public Armies(Vector2 Position, Vector2 Scale, Units Unit, Effects HealthEffect, Effects SupplyEffect, Vector2 Army_Level_Array_Position)
        {
            try
            {
                this.Scale = Scale;
                this.Unit = new Units(Position, Scale, Unit.Unit_Sprite, HealthEffect, SupplyEffect, Unit.Tag, Unit.ShortTag, false);
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
        public Armies(Vector2 Position, float Scale, Units Unit, Effects HealthEffect, Effects SupplyEffect, Vector2 Army_Level_Array_Position)
        {
            try
            {
                this.Scale = Unit.Unit_Sprite[0].Scale * Scale; //Array position 0 scale should be consistant across all units
                this.Unit = new Units(Position, Scale, Unit.Unit_Sprite, HealthEffect, SupplyEffect, Unit.Tag, Unit.ShortTag, false);
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


        public void DestroySelf()
        {
            Log.Info($"[ARMIES]({this.Unit.Tag}) - Has been destroyed!");
            EEngine.UnRegisterArmy(this);
        }

    }
}
