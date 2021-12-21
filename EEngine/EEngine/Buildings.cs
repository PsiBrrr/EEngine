using System;
using System.Collections.Generic;
using System.Drawing;
using System.Xml;

namespace EEngine.EEngine
{
    public class Buildings
    {
        public Vector2 Position = Vector2.Zero();
        public Vector2 vScale = Vector2.Zero();
        private float fScale = 1f;
        public List<AnimatedSprite2D> Building_Sprite = new List<AnimatedSprite2D>();
        public int Frame = 0;
        public string Tag = "";
        public string ShortTag = "";

        private enum Animations { Normal, Normal_Fog, Rain, Rain_Fog, Snow, Snow_Fog };
        public int AnimationSet { get; private set; } = (int)Animations.Normal;


        public Buildings(Vector2 Position, float fScale, Buildings Building, int Frame, bool Register)
        {
            this.Position = Position;
            this.fScale = fScale;
            this.vScale = Building.Building_Sprite[0].Scale * this.fScale;
            this.Building_Sprite = Building.Building_Sprite;
            this.Frame = Frame;
            this.Tag = Building.Tag;
            this.ShortTag = Building.ShortTag;

            if (Register)
            {
                EEngine.RegisterABuilding(this);
                Log.Info($"[ABUILDINGS]({Tag}) - Has been registered!");
            }
        }


        /// <summary>
        /// For all Buildings in a sprite sheet, based on an xml document
        /// </summary>
        public Buildings(Image Image, XmlDocument Doc)
        {
            XmlNodeList Node;
            List<Rectangle> Sections = new List<Rectangle>();
            List<string> Tags = new List<string>();

            List<List<Rectangle>> AllSections = new List<List<Rectangle>>();
            List<bool> AllFlips = new List<bool>();
            List<List<string>> AllTags = new List<List<string>>();

            int j = 0;

            Node = Doc.GetElementsByTagName("Buildings");

            foreach (XmlNode node in Node)
            {
                try
                {
                    string Building_Tag = node.Attributes.GetNamedItem("Building_Tag").InnerText;
                    //float Unit_Ammo = 0f;
                    //float Unit_Cost = float.Parse(node.Attributes.GetNamedItem("Unit_Cost").InnerText);
                    //float Unit_Fuel = float.Parse(node.Attributes.GetNamedItem("Unit_Fuel").InnerText);
                    foreach (XmlNode child in node.ChildNodes)
                    {
                        if (child.Name == "Building")
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
                                    new Buildings(AllSections, Image, AllTags, Building_Tag, ShortTag);

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
                    Log.Error($"[BUILDINGS] - Unable to Register: {ex.Message}");
                }
            }
        }
        public Buildings(List<List<Rectangle>> Section, Image Image, List<List<string>> SpriteTag, string Tag, string ShortTag)
        {
            List<Sprite2D> Sprites = new List<Sprite2D>();

            for (int i = 0; i < Section.Count; i++)
            {
                for (int j = 0; j < Section[i].Count; j++)
                {
                    Sprites.Add(new Sprite2D(Section[i][j], Image, SpriteTag[i][j], true));
                }
                this.Building_Sprite.Add(new AnimatedSprite2D(Sprites, Tag));
                Sprites = new List<Sprite2D>();
            }

            //this.vScale = Building_Sprite[0].Scale; //Building_Sprite[0].Scale should be the same across all tiles
            this.Tag = Tag;
            this.ShortTag = ShortTag;

            EEngine.RegisterBuilding(this);
            Log.Info($"[BUILDINGS]({Tag}) - Has been registered!");
        }


        public void Normal() { AnimationSet = (int)Animations.Normal; }
        public void Normal_Fog() { AnimationSet = (int)Animations.Normal_Fog; }

        public void DestroySelf()
        {
            Log.Info($"[BUILDINGS]({this.Tag}) - Has been destroyed!");
            EEngine.UnRegisterBuilding(this);
        }
    }
}
