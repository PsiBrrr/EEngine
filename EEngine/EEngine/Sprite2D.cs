using System;
using System.Drawing;

namespace EEngine.EEngine
{
    public class Sprite2D
    {
        public Vector2 Position = Vector2.Zero();
        public Vector2 Scale = Vector2.Zero();
        public Image Image = null;
        public string Tag = "";
        public Bitmap Sprite = null;

        /// <summary>
        /// Creates then Registers a Bitmap from a single sprite from image
        /// </summary>
        public Sprite2D(Vector2 Position, Vector2 Scale, Image Image, string Tag, bool Transparent)
        {
            this.Position = Position;
            this.Scale = Scale;
            this.Image = Image;
            this.Tag = Tag;

            Image tmpImage = Image;
            Bitmap tmpSprite = new Bitmap(tmpImage);

            Sprite = tmpSprite;
            if (Transparent) { Sprite.MakeTransparent(); }
        }
        /// <summary>
        /// Creates then Registers a Bitmap from a sprite sheet from image
        /// </summary>
        public Sprite2D(Vector2 Position, Vector2 Scale, Rectangle Section, Image Image, string Tag, bool Transparent)
        {
            this.Position = Position;
            this.Scale = Scale;
            this.Image = Image;
            this.Tag = Tag;

            Image tmpImage = Image;
            Bitmap tmpSprite = new Bitmap(tmpImage);
            Bitmap sprite = tmpSprite.Clone(Section, tmpSprite.PixelFormat);

            Sprite = sprite;
            if (Transparent) { Sprite.MakeTransparent(); }

            tmpSprite.Dispose();
        }
        /// <summary>
        /// Creates then Registers a Bitmap from a single sprite from image. 
        /// Rotated and/or Flipped
        /// </summary>
        public Sprite2D(Vector2 Position, Vector2 Scale, Image Image, RotateFlipType RotateFlip, string Tag, bool Transparent)
        {
            this.Position = Position;
            this.Scale = Scale;
            this.Image = Image;
            this.Tag = Tag;

            Image tmpImage = Image;
            Bitmap tmpSprite = new Bitmap(tmpImage);
            tmpSprite.RotateFlip(RotateFlip);

            Sprite = tmpSprite;
            if (Transparent) { Sprite.MakeTransparent(); }
        }
        /// <summary>
        /// Creates then Registers a Bitmap from a sprite sheet from image.
        /// Rotated and/or Flipped
        /// </summary>
        public Sprite2D(Vector2 Position, Vector2 Scale, Rectangle Section, Image Image, RotateFlipType RotateFlip, string Tag, bool Transparent)
        {
            this.Position = Position;
            this.Scale = Scale;
            this.Image = Image;
            this.Tag = Tag;

            Image tmpImage = Image;
            Bitmap tmpSprite = new Bitmap(tmpImage);
            Bitmap sprite = tmpSprite.Clone(Section, tmpSprite.PixelFormat);
            sprite.RotateFlip(RotateFlip);

            Sprite = sprite;
            if (Transparent) { Sprite.MakeTransparent(); }

            tmpSprite.Dispose();
        }


        /// <summary>
        /// Creates then Registers a Bitmap from a single sprite from image
        /// No Position or Scale
        /// </summary>
        public Sprite2D(Image Image, string Tag, bool Transparent)
        {
            this.Image = Image;
            this.Tag = Tag;

            Image tmpImage = Image;
            Bitmap tmpSprite = new Bitmap(tmpImage);

            Sprite = tmpSprite;
            if (Transparent) { Sprite.MakeTransparent(); }
        }
        /// <summary>
        /// Creates a Bitmap from a sprite sheet from image
        /// No Location or Scale
        /// </summary>
        public Sprite2D(Rectangle Section, Image Image, string Tag, bool Transparent)
        {
            try
            {
                this.Scale = new Vector2(Section.Width, Section.Height);
                this.Image = Image;
                this.Tag = Tag;

                Image tmpImage = Image;
                Bitmap tmpSprite = new Bitmap(tmpImage);
                Bitmap sprite = tmpSprite.Clone(Section, tmpSprite.PixelFormat);

                Sprite = sprite;
                if (Transparent) { Sprite.MakeTransparent(); }

                tmpSprite.Dispose();
            }
            catch (OutOfMemoryException ex)
            {
                Log.Error($"[SPRITE2D] - {Tag}: {ex.Message}");
            }
        }
        /// <summary>
        /// Creates a Bitmap from a single sprite from image. 
        /// No Position and Scale
        /// Rotated and/or Flipped
        /// </summary>
        public Sprite2D(Image Image, RotateFlipType RotateFlip, string Tag, bool Transparent)
        {
            this.Image = Image;
            this.Tag = Tag;

            Image tmpImage = Image;
            Bitmap tmpSprite = new Bitmap(tmpImage);
            tmpSprite.RotateFlip(RotateFlip);

            Sprite = tmpSprite;
            if (Transparent) { Sprite.MakeTransparent(); }
        }
        /// <summary>
        /// Creates a Bitmap from a sprite sheet from image.
        /// No Position and Scale
        /// Rotated and/or Flipped
        /// </summary>
        public Sprite2D(Rectangle Section, Image Image, RotateFlipType RotateFlip, string Tag, bool Transparent)
        {
            try
            {
                this.Scale = new Vector2(Section.Width, Section.Height);
                this.Image = Image;
                this.Tag = Tag;

                Image tmpImage = Image;
                Bitmap tmpSprite = new Bitmap(tmpImage);
                Bitmap sprite = tmpSprite.Clone(Section, tmpSprite.PixelFormat);
                sprite.RotateFlip(RotateFlip);

                Sprite = sprite;
                if (Transparent) { Sprite.MakeTransparent(); }

                tmpSprite.Dispose();
            }
            catch (OutOfMemoryException ex)
            {
                Log.Error($"[SPRITE2D] - {Tag}: {ex.Message}");
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

        //public bool IsColliding(string tag)
        //{
        //    foreach(Sprite2D b in EEngine.AllSprites)
        //    {
        //        if(b.Tag == tag)
        //        {
        //            if (Position.X < b.Position.X + b.Scale.X &&
        //                Position.X + Scale.X > b.Position.X &&
        //                Position.Y < b.Position.Y + b.Scale.Y &&
        //                Position.Y + Scale.Y > b.Position.Y)
        //            {
        //                return true;
        //            }
        //        }
        //    }

        //    return false;
        //}

        //public void DestroySelf()
        //{
        //    Log.Info($"[SPRITE2D]({Tag}) - Has been destroyed!");
        //    EEngine.UnRegisterSprite(this);
        //}
    }
}
