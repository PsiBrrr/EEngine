using System.Collections.Generic;
using System.Drawing;

namespace EEngine.EEngine
{
    public class AnimatedSprite2D
    {
        public Vector2 Position = Vector2.Zero();
        public Vector2 Scale = Vector2.Zero();
        public List<Sprite2D> Sprite = null;
        public string Tag = "";
        public int AnimatedSprite_Frame = 0;

        public AnimatedSprite2D(Vector2 Position, Vector2 Scale, List<Sprite2D> Sprite, string Tag)
        {
            try
            {
                if (Sprite.Count == 0) { Log.Error($"[ANIMATEDSPRITE2D]({Tag}) - Sprite is Empty!"); } else { this.Sprite = Sprite; }
                if (string.IsNullOrEmpty(Tag)) { Log.Warning($"[ANIMATEDSPRITE2D]({Sprite[0].Tag}) - Tag is Null or Empty!"); }

                this.Position = Position;
                this.Scale = Scale;
                this.Tag = Tag;
            }
            catch
            {
                Log.Error($"[ANIMATEDSPRITE2D]({Tag}) - Unable to Create Animated Sprite!");
            }

        }

        public AnimatedSprite2D(List<Sprite2D> Sprite, string Tag)
        {
            try
            {
                if (Sprite.Count == 0) { Log.Error($"[ANIMATEDSPRITE2D]({Tag}) - Sprite is Empty!"); } else { this.Sprite = Sprite; }
                if (string.IsNullOrEmpty(Tag)) { Log.Warning($"[ANIMATEDSPRITE2D]({Sprite[0].Tag}) - Tag is Null or Empty!"); }

                this.Tag = Tag;
            }
            catch
            {
                Log.Error($"[ANIMATEDSPRITE2D]({Tag}) - Unable to Create Animated Sprite!");
            }
        }


        //public bool IsColliding(Sprite2D a, Sprite2D b)
        //{
        //    if (a.Position.X < b.Position.X + b.Scale.X &&
        //        a.Position.X + a.Scale.X > b.Position.X &&
        //        a.Position.Y < b.Position.Y + b.Scale.Y &&
        //        a.Position.Y + a.Scale.Y > b.Position.Y)
        //    {
        //        return true;
        //    }

        //    return false;
        //}
    }
}
