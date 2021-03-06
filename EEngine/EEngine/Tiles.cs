using System;
using System.Collections.Generic;
using System.Drawing;
using System.Xml;

namespace EEngine.EEngine
{
    public class Tiles
    {
        public Vector2 vScale = Vector2.Zero();
        private float fScale = 1f;
        public List<AnimatedSprite2D> Sprite = new List<AnimatedSprite2D>();
        public int Frame = 0;
        public string Tag = "";
        public string ShortTag = "";

        private enum Animations { Normal, Normal_Fog, Rain, Rain_Fog, Snow, Snow_Fog };
        public int AnimationSet { get; private set; } = (int)Animations.Normal;

        public Tiles(Vector2 Scale, List<AnimatedSprite2D> Sprite, string Tag, string ShortTag, bool Register)
        {
            this.vScale = Scale;
            this.Sprite = Sprite;
            this.Tag = Tag;
            this.ShortTag = ShortTag;

            if (Register)
            {
                Log.Info($"[TILES]({this.Tag}) - Has been registered!");
                EEngine.RegisterTile(this);
            }
        }
        public Tiles(float Scale, List<AnimatedSprite2D> Sprite, string Tag, string ShortTag, bool Register)
        {
            this.fScale = Scale;
            this.vScale = Sprite[0].Scale * this.fScale;
            this.Sprite = Sprite;
            this.Tag = Tag;
            this.ShortTag = ShortTag;

            if (Register)
            {
                Log.Info($"[TILES]({this.Tag}) - Has been registered!");
                EEngine.RegisterTile(this);
            }
        }
        /// <summary>
        /// For a single Animated Tile in a sprite sheet, using Sprite2D but zeroing out the Position and Scale
        /// </summary>
        public Tiles(AnimatedSprite2D AnimatedSprite, string Tag, string ShortTag, bool Register)
        {
            this.Sprite.Add(AnimatedSprite);
            this.Tag = Tag;
            this.ShortTag = ShortTag;

            if (Register)
            {
                Log.Info($"[TILES]({this.Tag}) - Has been registered!");
                EEngine.RegisterTile(this);
            }
        }


        /// <summary>
        /// For all Tiles in a sprite sheet, based on an xml document
        /// </summary>
        public Tiles(Image Image, XmlDocument Doc)
        {
            XmlNodeList XmlNode;
            List<Rectangle> Sections = new List<Rectangle>();
            List<string> Tags = new List<string>();

            List<List<Rectangle>> AllSections = new List<List<Rectangle>>();
            //List<bool> AllFlips = new List<bool>();
            List<List<string>> AllTags = new List<List<string>>();

            int j = 0;

            XmlNode = Doc.GetElementsByTagName("Tile");

            for (int i = 0; i <= XmlNode.Count - 1; i++)
            {
                try
                {
                    string[] StrPoint = XmlNode[i].ChildNodes.Item(0).InnerText.Trim().Replace("\t", "").Split(','); //Position on Image
                    string[] StrSize = XmlNode[i].ChildNodes.Item(1).InnerText.Trim().Replace("\t", "").Split(','); //Size of new Image
                    string Tag = XmlNode[i].ChildNodes.Item(2).InnerText.Trim(); //Tag or Name of the Image set
                    string GroupTag = XmlNode[i].ChildNodes.Item(3).InnerText.Trim(); //Tag or Name of individual image, incremented
                    string ShortTag = XmlNode[i].ChildNodes.Item(4).InnerText.Trim(); //Short Hand Tag or Name of the Image set
                    //bool Flip = bool.Parse(XmlNode[i].ChildNodes.Item(5).InnerText.Trim()); //If the new Image needs to be flipped
                    int Group = int.Parse(XmlNode[i].ChildNodes.Item(6).InnerText.Trim()); //Group of Image sets 
                    int SubGroup = int.Parse(XmlNode[i].ChildNodes.Item(7).InnerText.Trim()); //Groupings of an Image set (ie. Normal, Normal_Fog...)

                    int NextGroup = 0;
                    if (XmlNode[i].NextSibling != null) { NextGroup = int.Parse(XmlNode[i].NextSibling.ChildNodes.Item(6).InnerText.Trim()); }

                    int NextSubGroup = 0;
                    if (XmlNode[i].NextSibling != null) { NextSubGroup = int.Parse(XmlNode[i].NextSibling.ChildNodes.Item(7).InnerText.Trim()); }

                    Rectangle Section = new Rectangle(new Point(int.Parse(StrPoint[0]), int.Parse(StrPoint[1])), new Size(int.Parse(StrSize[0]), int.Parse(StrSize[0])));

                    Sections.Add(Section);
                    Tags.Add(Tag + "_" + j++);

                    if (SubGroup != NextSubGroup)
                    {

                        AllSections.Add(Sections);
                        //AllFlips.Add(Flip);
                        AllTags.Add(Tags);

                        if (Group != NextGroup)
                        {
                            new Tiles(AllSections, Image, AllTags, Tag, ShortTag);

                            AllSections = new List<List<Rectangle>>();
                            //AllFlips = new List<bool>();
                            AllTags = new List<List<string>>();
                        }

                        Sections = new List<Rectangle>();
                        Tags = new List<string>();

                        j = 0;
                    }

                }
                catch (NullReferenceException ex)
                {
                    Log.Error($"[TILES] - Unable to Register due to Null Reference: {ex.Message}");
                }
            }
        }
        public Tiles(List<Rectangle> Section, Image Image, List<string> SpriteTag, string Tag, string ShortTag)
        {
            List<Sprite2D> Sprites = new List<Sprite2D>();
            for (int i = 0; i < Section.Count; i++)
            {
                Sprites.Add(new Sprite2D(Section[i], Image, SpriteTag[i], false));
            }

            this.vScale = new Vector2(Section[0].Width, Section[0].Height);
            this.Sprite.Add(new AnimatedSprite2D(Sprites, Tag));
            this.ShortTag = ShortTag;

            Log.Info($"[TILES]({Tag}) - Has been registered!");
            EEngine.RegisterTile(this);
        }
        public Tiles(List<List<Rectangle>> Section, Image Image, List<List<string>> SpriteTag, string Tag, string ShortTag)
        {
            List<Sprite2D> Sprites = new List<Sprite2D>();

            for (int i = 0; i < Section.Count; i++)
            {
                for (int j = 0; j < Section[i].Count; j++)
                {
                    Sprites.Add(new Sprite2D(Section[i][j], Image, SpriteTag[i][j], false));
                }
                this.Sprite.Add(new AnimatedSprite2D(Sprites, Tag));
                Sprites = new List<Sprite2D>();
            }

            this.vScale = Sprite[0].Scale; //Tile_Sprite[0].Scale should be the same across all tiles
            this.Tag = Tag;
            this.ShortTag = ShortTag;

            EEngine.RegisterTile(this);
            Log.Info($"[TILES]({Tag}) - Has been registered!");
        }


        public void Normal() { AnimationSet = (int)Animations.Normal; }
        public void Normal_Fog() { AnimationSet = (int)Animations.Normal_Fog; }



        public void DestroySelf()
        {
            Log.Info($"[TILES]({Tag}) - Has been destroyed!");
            EEngine.UnRegisterTile(this);
        }
    }
}
