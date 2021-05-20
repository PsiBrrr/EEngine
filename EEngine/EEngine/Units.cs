using System;
using System.Collections.Generic;
using System.Drawing;
using System.Xml;

namespace EEngine.EEngine
{
    public class Units
    {
        public Vector2 Unit_Position = Vector2.Zero();
        public Vector2 Unit_Scale = Vector2.Zero();
        public List<AnimatedSprite2D> Unit_Sprite = new List<AnimatedSprite2D>();
        public Effects Health_Effect = null;
        public int Unit_Frame = 0;

        public int Ammo = 0;
        public int Cost = 0;
        public int Fuel = 0;
        private readonly int MaxHealth = 99;
        private readonly int MinHealth = 0;
        public int Health = 0;
        public int Movement = 0;
        public int Vision = 0;

        public string Tag = "";
        public string ShortTag = "";

        private enum Animations { Idle_Left, Active_Up, Active_Down, Active_Left, Active_Right, Unavailable_Left, Unavailable_Right };
        public int AnimationSet { get; private set; } = (int)Animations.Idle_Left;


        public Units(Vector2 Unit_Position, Vector2 Unit_Scale, List<AnimatedSprite2D> Unit, Effects HealthEffect, string Tag, string ShortTag, bool Register)
        {
            this.Unit_Position = Unit_Position;
            this.Unit_Scale = Unit_Scale;
            this.Unit_Sprite = Unit;
            this.Tag = Tag;
            this.ShortTag = ShortTag;

            this.Health_Effect = new Effects(GetUnitHealthEffect(Unit_Position),
                HealthEffect.Scale * 2,
                HealthEffect.EffectElement,
                HealthEffect.Tag,
                HealthEffect.ShortTag,
                false);

            if (Register)
            {
                EEngine.RegisterUnit(this);
                Log.Info($"[UNITS]({Tag}) - Has been registered!");
            }
        }
        /// <summary>
        /// Create a new Unit from a AnimatedSprite2Ds 
        /// </summary>
        /// <param name="Section">List of Rectangles. Is used to create an AnimatedSprite2D from multiple Sprite2Ds</param>
        /// <param name="Image">Sprite Sheet</param>
        /// <param name="SpriteTag">List of Strings. Is a unique Tag assigned to a Sprite2D</param>
        /// <param name="Tag">Unique Tag</param>
        /// <param name="ShortTag">Short hand unique Tag</param>
        public Units(List<Rectangle> Section, Image Image, List<string> SpriteTag, bool Flip, string Tag, string ShortTag)
        {
            List<Sprite2D> Sprites = new List<Sprite2D>();

            for (int i = 0; i < Section.Count; i++)
            {
                if (Flip) { Sprites.Add(new Sprite2D(Section[i], Image, RotateFlipType.RotateNoneFlipX, SpriteTag[i], true)); }
                else { Sprites.Add(new Sprite2D(Section[i], Image, SpriteTag[i], true)); }
            }

            this.Unit_Sprite.Add(new AnimatedSprite2D(Sprites, Tag));
            this.Tag = Tag;
            this.ShortTag = ShortTag;

            EEngine.RegisterUnit(this);
            Log.Info($"[UNITS]({Tag}) - Has been registered!");
        }
        /// Create a new Unit from multiple AnimatedSprite2Ds
        /// </summary>
        /// <param name="Section">List of a List of Rectangles. Each inner list is used to create an AnimatedSprite2D from multiple Sprite2Ds</param>
        /// <param name="Image">Sprite Sheet</param>
        /// <param name="SpriteTag">List of a List of Strings. Each inner list is a unique Tag assigned to a Sprite2D</param>
        /// <param name="Flip">Boolean. Should these images be flipped</param>
        /// <param name="Tag">Unique Tag</param>
        /// <param name="ShortTag">Short hand unique Tag</param>
        public Units(List<List<Rectangle>> Section, Image Image, List<List<string>> SpriteTag, List<bool> Flip, string Tag, string ShortTag)
        {
            List<Sprite2D> Sprites = new List<Sprite2D>();

            for (int i = 0; i < Section.Count; i++)
            {
                for (int j = 0; j < Section[i].Count; j++)
                {
                    if (Flip[i]) { Sprites.Add(new Sprite2D(Section[i][j], Image, RotateFlipType.RotateNoneFlipX, SpriteTag[i][j], true)); }
                    else { Sprites.Add(new Sprite2D(Section[i][j], Image, SpriteTag[i][j], true)); }
                }
                this.Unit_Sprite.Add(new AnimatedSprite2D(Sprites, Tag));
                Sprites = new List<Sprite2D>();
            }

            this.Tag = Tag;
            this.ShortTag = ShortTag;

            EEngine.RegisterUnit(this);
            Log.Info($"[UNITS]({Tag}) - Has been registered!");
        }
        public Units(Image Image, XmlDocument Doc)
        {
            XmlNodeList XmlNode;
            List<Rectangle> Sections = new List<Rectangle>();
            List<string> Tags = new List<string>();

            List<List<Rectangle>> AllSections = new List<List<Rectangle>>();
            List<bool> AllFlips = new List<bool>();
            List<List<string>> AllTags = new List<List<string>>();

            int j = 0;

            XmlNode = Doc.GetElementsByTagName("Unit");

            for (int i = 0; i <= XmlNode.Count - 1; i++)
            {
                try
                {
                    string[] StrPoint = XmlNode[i].ChildNodes.Item(0).InnerText.Trim().Replace("\t", "").Split(',');
                    string[] StrSize = XmlNode[i].ChildNodes.Item(1).InnerText.Trim().Replace("\t", "").Split(',');
                    string Tag = XmlNode[i].ChildNodes.Item(2).InnerText.Trim();
                    string ShortTag = XmlNode[i].ChildNodes.Item(3).InnerText.Trim();
                    bool Flip = bool.Parse(XmlNode[i].ChildNodes.Item(4).InnerText.Trim());
                    int Group = int.Parse(XmlNode[i].ChildNodes.Item(5).InnerText.Trim());
                    int SubGroup = int.Parse(XmlNode[i].ChildNodes.Item(6).InnerText.Trim());

                    int NextGroup = 0;
                    if (XmlNode[i].NextSibling != null) { NextGroup = int.Parse(XmlNode[i].NextSibling.ChildNodes.Item(5).InnerText.Trim()); }

                    int NextSubGroup = 0;
                    if (XmlNode[i].NextSibling != null) { NextSubGroup = int.Parse(XmlNode[i].NextSibling.ChildNodes.Item(6).InnerText.Trim()); }

                    Rectangle Section = new Rectangle(new Point(int.Parse(StrPoint[0]), int.Parse(StrPoint[1])), new Size(int.Parse(StrSize[0]), int.Parse(StrSize[0])));

                    Sections.Add(Section);
                    Tags.Add(Tag + "_" + j++);

                    if (SubGroup != NextSubGroup)
                    {

                        AllSections.Add(Sections);
                        AllFlips.Add(Flip);
                        AllTags.Add(Tags);

                        if (Group != NextGroup)
                        {
                            new Units(AllSections, Image, AllTags, AllFlips, Tag, ShortTag);

                            AllSections = new List<List<Rectangle>>();
                            AllFlips = new List<bool>();
                            AllTags = new List<List<string>>();
                        }

                        Sections = new List<Rectangle>();
                        Tags = new List<string>();

                        j = 0;
                    }
                }
                catch (NullReferenceException ex)
                {
                    Log.Error($"[UNITS] - Unable to Register: {ex.Message}");
                }
            }
        }


        public void Idle() { AnimationSet = (int)Animations.Idle_Left; }
        public void Up() { AnimationSet = (int)Animations.Active_Up; }
        public void Down() { AnimationSet = (int)Animations.Active_Down; }
        public void Left() { AnimationSet = (int)Animations.Active_Left; }
        public void Right() { AnimationSet = (int)Animations.Active_Right; }
        public void UnavailableLeft() { AnimationSet = (int)Animations.Unavailable_Left; }
        public void UnavailableRight() { AnimationSet = (int)Animations.Unavailable_Right; }


        public Vector2 GetUnitHealthEffect(Vector2 Position)
        {
            return new Vector2(Position.X + Unit_Scale.X / 2, Position.Y + (Unit_Scale.Y - 16));
        }
        public void AddUnitHealth(int Health)
        {
            this.Health += Health;
            if (this.Health > MaxHealth) { this.Health = MaxHealth; }
        }
        public int GetUnitHeath_Rounded()
        {
            return (Health / 10);
        }


        public void DestroySelf()
        {
            EEngine.UnRegisterUnit(this);
            Log.Info($"[UNITS]({this}) - Has been destroyed!");
        }
    }
}
