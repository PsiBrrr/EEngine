using System;
using System.Collections.Generic;
using System.Drawing;
using System.Xml;

namespace EEngine.EEngine
{
    public class Units
    {
        public Guid ID { get; private set; }
        public Vector2 Position = Vector2.Zero();
        public Vector2 vScale = Vector2.Zero();
        private float fScale = 1f;
        public List<AnimatedSprite2D> Unit_Sprite = new List<AnimatedSprite2D>();
        public Effects Health_Effect = null;
        public Effects Supply_Effect = null;
        public int Frame = 0;
        public bool Available = true;
        public string Tag = "";
        public string ShortTag = "";

        //Hard set buffer area around Unit Sprite
        private readonly float SpriteTopBuffer = 8f;
        private readonly float SpriteLeftBuffer = 4f;
        private readonly float SpriteRightBuffer = 4f;
        private readonly float SpriteBottomBuffer = 0f;


        private float Ammo = 0f;
        public float Cost { get; private set; } = 0f;
        private float MaxFuel = 0f;
        private float Fuel = 0f;
        private readonly int MaxHealth = 100;
        private int Health = 100;
        private float Movement = 0f;
        private float Vision = 0f;



        private enum Animations { Idle, Available_Up, Available_Down, Available_Left, Available_Right, UnAvailable_Idle };
        public int AnimationSet { get; private set; } = (int)Animations.Idle;


        public Units(Vector2 Position, Vector2 vScale, Units Unit, int Frame, Effects Health_Effect, Effects Supply_Effect, bool Register)
        {
            ID = Guid.NewGuid();
            this.Position = Position;
            //this.Bounding_Position = CalculateUnitBoundingPosition(Position, Scale);
            this.vScale = vScale;
            this.Unit_Sprite = Unit.Unit_Sprite;
            this.Frame = Frame;
            this.Tag = Unit.Tag;
            this.Cost = Unit.Cost;
            this.Fuel = Unit.Fuel;
            this.ShortTag = Unit.ShortTag;

            this.Health_Effect = new Effects(GetUnitHealthEffect(Position), Health_Effect.Scale * fScale, Health_Effect, Health_Effect.Tag, Health_Effect.ShortTag, false);
            this.Supply_Effect = new Effects(GetUnitSupplyEffect(Position), Supply_Effect.Scale * fScale, Supply_Effect, Supply_Effect.Tag, Supply_Effect.ShortTag, false);

            if (Register)
            {
                EEngine.RegisterUnit(this);
                Log.Info($"[UNITS]({Tag}) - Has been registered!");
            }
        }
        public Units(Vector2 Position, float fScale, Units Unit, int Frame, Effects Health_Effect, Effects Supply_Effect, bool Register)
        {
            ID = Guid.NewGuid();
            this.Position = Position;
            this.fScale = fScale;
            this.vScale = Unit.Unit_Sprite[0].Scale * this.fScale;
            this.Unit_Sprite = Unit.Unit_Sprite;
            this.Frame = Frame;
            this.Tag = Unit.Tag;
            this.Cost = Unit.Cost;
            this.Fuel = Unit.Fuel;
            this.ShortTag = Unit.ShortTag;

            this.Health_Effect = new Effects(GetUnitHealthEffect(Position), Health_Effect.Scale * fScale, Health_Effect, Health_Effect.Tag, Health_Effect.ShortTag, false);
            this.Supply_Effect = new Effects(GetUnitSupplyEffect(Position), Supply_Effect.Scale * fScale, Supply_Effect, Supply_Effect.Tag, Supply_Effect.ShortTag, false);

            if (Register)
            {
                EEngine.RegisterUnit(this);
                Log.Info($"[UNITS]({Tag}) - Has been registered!");
            }
        }


        /// Create a new Unit from multiple AnimatedSprite2Ds
        /// </summary>
        /// <param name="Section">  List of a List of Rectangles. Each inner list is used to create an AnimatedSprite2D from multiple Sprite2Ds</param>
        /// <param name="Image">    Sprite Sheet</param>
        /// <param name="SpriteTag">List of a List of Strings. Each inner list is a unique Tag assigned to a Sprite2D</param>
        /// <param name="Flip">     bool. Should these images be flipped</param>
        /// <param name="Tag">      string. Unique Tag</param>
        /// <param name="ShortTag"> string. Short hand Unique Tag</param>
        public Units(List<List<Rectangle>> Section, Image Image, List<List<string>> SpriteTag, List<bool> Flip, string Tag, float Ammo, float Cost, float Fuel, string ShortTag)
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
            this.Ammo = Ammo;
            this.Cost = Cost;
            this.Fuel = Fuel;
            this.ShortTag = ShortTag;

            EEngine.RegisterUnit(this);
            Log.Info($"[UNITS]({Tag}) - Has been registered!");
        }
        public Units(Image Image, XmlDocument Doc)
        {
            XmlNodeList Node;
            List<Rectangle> Sections = new List<Rectangle>();
            List<string> Tags = new List<string>();

            List<List<Rectangle>> AllSections = new List<List<Rectangle>>();
            List<bool> AllFlips = new List<bool>();
            List<List<string>> AllTags = new List<List<string>>();

            int j = 0;

            Node = Doc.GetElementsByTagName("Units");
            
            foreach(XmlNode node in Node)
            {
                try
                {
                    string Unit_Tag = node.Attributes.GetNamedItem("Tag").InnerText;
                    float Unit_Ammo = 0f;
                    float Unit_Cost = float.Parse(node.Attributes.GetNamedItem("Unit_Cost").InnerText);
                    float Unit_Fuel = float.Parse(node.Attributes.GetNamedItem("Unit_Fuel").InnerText);
                    foreach (XmlNode child in node.ChildNodes)
                    {
                        if (child.Name == "Unit")
                        {
                            string[] StrPoint = child.ChildNodes.Item(0).InnerText.Trim().Replace("\t", "").Split(',');
                            string[] StrSize = child.ChildNodes.Item(1).InnerText.Trim().Replace("\t", "").Split(',');
                            string AnimationTag = child.ChildNodes.Item(2).InnerText.Trim();
                            string ShortTag = child.ChildNodes.Item(3).InnerText.Trim();
                            bool Flip = bool.Parse(child.ChildNodes.Item(4).InnerText.Trim());
                            int SubGroup = int.Parse(child.ChildNodes.Item(5).InnerText.Trim());

                            int NextSubGroup = 0;
                            if (child.NextSibling != null) { NextSubGroup = int.Parse(child.NextSibling.ChildNodes.Item(5).InnerText.Trim()); }

                            Rectangle Section = new Rectangle(new Point(int.Parse(StrPoint[0]), int.Parse(StrPoint[1])), new Size(int.Parse(StrSize[0]), int.Parse(StrSize[1])));

                            Sections.Add(Section);
                            Tags.Add(AnimationTag + "_" + j++);

                            if (SubGroup != NextSubGroup)
                            {

                                AllSections.Add(Sections);
                                AllFlips.Add(Flip);
                                AllTags.Add(Tags);

                                if (child.NextSibling == null)
                                {
                                    new Units(AllSections, Image, AllTags, AllFlips, Unit_Tag, Unit_Ammo, Unit_Cost, Unit_Fuel, ShortTag);

                                    AllSections = new List<List<Rectangle>>();
                                    AllFlips = new List<bool>();
                                    AllTags = new List<List<string>>();
                                }

                                Sections = new List<Rectangle>();
                                Tags = new List<string>();

                                j = 0;
                            }
                        }
                    }
                }
                catch (NullReferenceException ex)
                {
                    Log.Error($"[UNITS] - Unable to Register: {ex.Message}");
                }
            }
        }


        public void Idle() { AnimationSet = (int)Animations.Idle; }
        public void Up() { AnimationSet = (int)Animations.Available_Up; }
        public void Down() { AnimationSet = (int)Animations.Available_Down; }
        public void Left() { AnimationSet = (int)Animations.Available_Left; }
        public void Right() { AnimationSet = (int)Animations.Available_Right; }
        public void UnavailableIdle() { AnimationSet = (int)Animations.UnAvailable_Idle; }


        /// <summary>
        /// Finds upper corner of Unit. Unit may not actually start where the corner of the Sprite is due to sprite size being larger.
        /// </summary>
        /// <returns></returns>
        public Vector2 GetUnitUpperBoundingPosition()
        {
            Vector2 Bounding_Position = Vector2.Zero();
            Bounding_Position.X += CalculateUnitOffsetOrigin().X + (SpriteLeftBuffer * fScale);
            Bounding_Position.Y += CalculateUnitOffsetOrigin().Y + (SpriteTopBuffer * fScale);

            return Bounding_Position;
        }
        /// <summary>
        /// Finds lower corner of Unit. Unit may not actually end where the corner of the Sprite is due to sprite size being larger.
        /// </summary>
        /// <returns></returns>
        public Vector2 GetUnitLowerBoundingPosition()
        {
            Vector2 Bounding_Position = Vector2.Zero();
            Bounding_Position.X += CalculateUnitOffsetOrigin().X + vScale.X - (SpriteRightBuffer * fScale);
            Bounding_Position.Y += CalculateUnitOffsetOrigin().Y + vScale.Y - (SpriteBottomBuffer * fScale);

            return Bounding_Position;
        }
        /// <summary>
        /// Used to find the Origin X & Y of a Unit Sprite. This is due to the fact that the Sprites are not consistant with the other Sprites.
        /// 
        /// </summary>
        /// <returns>Vector2</returns>
        public Vector2 CalculateUnitOffsetOrigin()
        {
            Vector2 Offset_Position = Vector2.Zero();
            Offset_Position.X += Position.X - (SpriteLeftBuffer * fScale);
            Offset_Position.Y += Position.Y - (SpriteTopBuffer * fScale);

            return Offset_Position;
        }














        public Vector2 GetUnitHealthEffect(Vector2 Position)
        {
            float Health_Effect_Scale_Y = 0f;
            if (Health_Effect != null) { Health_Effect_Scale_Y = Health_Effect.Scale.Y; }

            return new Vector2(Position.X + vScale.X / 2, Position.Y + (vScale.Y - Health_Effect_Scale_Y));
        }
        public void AddUnitHealth(int Health)
        {
            this.Health += Health;
            if (this.Health > MaxHealth) { this.Health = MaxHealth; }
        }
        public void SetUnitHealth(int Health)
        {
            this.Health = Health;
        }
        public int GetUnitHeath()
        {
            return (Health / 10);
        }


        public Vector2 GetUnitStatusEffect(Vector2 Position)
        {
            return null;
        }
        public Vector2 GetUnitSupplyEffect(Vector2 Position)
        {
            //The size of each sprite is 24 pixel, however, the idle sprite is only 16 pixels. This is due to some sprites animations using the full 24 pixels (ie. Bomber movement)
            return new Vector2(Position.X + vScale.X / 2, Position.Y + (SpriteTopBuffer * fScale)); 
        }
        public int GetUnitSupply()
        {
            decimal FuelRounded = Math.Ceiling((decimal)MaxFuel / 20);

            if((decimal)Fuel <= (Math.Round(FuelRounded)))
            {
                return 0;
            }
            return 2;
        }
        public void AddUnitFuel()
        {
            if (this.Fuel < this.MaxFuel) { this.Fuel = this.MaxFuel; }
        }
        public void SubUnitFuel()
        {
            this.Fuel--;
        }
        public void SetUnitFuel(float Fuel)
        {
            this.Fuel = Fuel;
            if (this.Fuel > MaxFuel) { this.Fuel = MaxFuel; }
        }


        public void DestroySelf()
        {
            EEngine.UnRegisterUnit(this);
            Log.Info($"[UNITS]({this}) - Has been destroyed!");
        }
    }
}
