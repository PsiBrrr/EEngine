using System;
using System.Collections.Generic;
using System.Drawing;
using System.Xml;

namespace EEngine.EEngine
{
    public class Effects
    {
        public Vector2 Position = Vector2.Zero();
        public Vector2 Scale = Vector2.Zero();
        public List<AnimatedSprite2D> Effect_Sprite = new List<AnimatedSprite2D>();
        public string Tag = "";
        public string ShortTag = "";

        private enum Animations { Available, Unavailable };
        public int AnimationSet { get; private set; } = (int)Animations.Available;


        public Effects(Vector2 Position, Vector2 Scale, Effects Effect_Sprite, string Tag, string ShortTag, bool Register)
        {
            this.Position = Position;
            this.Scale = Scale;
            this.Effect_Sprite = Effect_Sprite.Effect_Sprite;
            this.Tag = Tag;
            this.ShortTag = ShortTag;

            if (Register)
            {
                EEngine.RegisterEffect(this);
                Log.Info($"[EFFECTS]({Tag}) - Has been registered!");
            }
        }


        public Effects(List<List<Rectangle>> Section, Image Image, List<List<string>> SpriteTag, List<bool> Flip, string Tag, string ShortTag)
        {
            List<Sprite2D> Sprites = new List<Sprite2D>();

            for (int i = 0; i < Section.Count; i++)
            {
                for (int j = 0; j < Section[i].Count; j++)
                {
                    if (Flip[i]) { Sprites.Add(new Sprite2D(Section[i][j], Image, RotateFlipType.RotateNoneFlipX, SpriteTag[i][j], true)); }
                    else { Sprites.Add(new Sprite2D(Section[i][j], Image, SpriteTag[i][j], true)); }
                }
                this.Effect_Sprite.Add(new AnimatedSprite2D(Sprites, Tag));
                Sprites = new List<Sprite2D>();
            }

            this.Scale = new Vector2(Section[0][0].Width, Section[0][0].Height); //All Sprites should be the same size
            this.Tag = Tag;
            this.ShortTag = ShortTag;

            Log.Info($"[EFFECTS]({Tag}) - Has been registered!");
            EEngine.RegisterEffect(this);
        }
        public Effects(Image Image, XmlDocument Doc)
        {
            XmlNodeList Node;
            List<Rectangle> Sections = new List<Rectangle>();
            List<string> Tags = new List<string>();

            List<List<Rectangle>> AllSections = new List<List<Rectangle>>();
            List<bool> AllFlips = new List<bool>();
            List<List<string>> AllTags = new List<List<string>>();

            int j = 0;

            Node = Doc.GetElementsByTagName("Effects");

            foreach (XmlNode node in Node)
            {
                try
                {
                    string Effect_Tag = node.Attributes.GetNamedItem("Tag").InnerText;
                    foreach (XmlNode child in node.ChildNodes)
                    {
                        if (child.Name == "Effect")
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
                                    new Effects(AllSections, Image, AllTags, AllFlips, Effect_Tag, ShortTag);

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


        public void Available() { AnimationSet = (int)Animations.Available; }
        public void Unavailable() { AnimationSet = (int)Animations.Unavailable; }

        public void DestroySelf()
        {
            Log.Info($"[EFFECTS]({Tag}) - Has been destroyed!");
            EEngine.UnRegisterEffect(this);
        }

    }
}
