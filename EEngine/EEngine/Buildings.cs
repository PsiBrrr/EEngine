using System;
using System.Collections.Generic;
using System.Drawing;
using System.Xml;

namespace EEngine.EEngine
{
    public class Buildings
    {
        public Vector2 Building_Scale = Vector2.Zero();
        public List<AnimatedSprite2D> Building_Sprite = new List<AnimatedSprite2D>();
        public int Building_Frame = 0;
        public string Tag = "";
        public string ShortTag = "";

        private enum Animations { Normal, Normal_Fog, Rain, Rain_Fog, Snow, Snow_Fog };

        public int AnimationSet { get; private set; } = (int)Animations.Normal_Fog;

        public Buildings(Vector2 Building_Scale, List<AnimatedSprite2D> Building, string Tag, string ShortTag, bool Register)
        {
            this.Building_Scale = Building_Scale;
            this.Building_Sprite = Building;
            this.Tag = Tag;
            this.ShortTag = ShortTag;

            if (Register)
            {
                Log.Info($"[BUILDING]({this.Tag}) - Has been registered!");
                EEngine.RegisterBuilding(this);
            }
        }

        public void DestroySelf()
        {
            Log.Info($"[BUILDING]({Tag}) - Has been destroyed!");
            EEngine.UnRegisterBuilding(this);
        }
    }
}
