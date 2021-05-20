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
        public AnimatedSprite2D EffectElement = null;
        public string Tag = "";
        public string ShortTag = "";

        public Effects(Vector2 Position, Vector2 Scale, AnimatedSprite2D EffectElement, string Tag, string ShortTag, bool Register)
        {
            this.Position = Position;
            this.Scale = Scale;
            this.EffectElement = EffectElement;
            this.Tag = Tag;
            this.ShortTag = ShortTag;

            if (Register)
            {
                EEngine.RegisterEffect(this);
                Log.Info($"[EFFECTS]({Tag}) - Has been registered!");
            }
        }
        public Effects(List<Rectangle> Section, Image Image, List<string> SpriteTag, string Tag, string ShortTag)
        {
            List<Sprite2D> Sprites = new List<Sprite2D>();
            for (int i = 0; i < Section.Count; i++)
            {
                Sprites.Add(new Sprite2D(Section[i], Image, SpriteTag[i], true));
            }

            this.Scale = new Vector2(Section[0].Width, Section[0].Height);
            this.EffectElement = new AnimatedSprite2D(Sprites, Tag);
            this.ShortTag = ShortTag;

            Log.Info($"[EFFECTS]({Tag}) - Has been registered!");
            EEngine.RegisterEffect(this);
        }

        public Effects(Image Image, XmlDocument Doc)
        {
            XmlNodeList XmlNode;
            List<Rectangle> Sections = new List<Rectangle>();
            List<string> Tags = new List<string>();

            int j = 0;

            XmlNode = Doc.GetElementsByTagName("Effect");

            for (int i = 0; i <= XmlNode.Count - 1; i++)
            {
                try
                {
                    string[] StrPoint = XmlNode[i].ChildNodes.Item(0).InnerText.Trim().Replace("\t", "").Split(',');
                    string[] StrSize = XmlNode[i].ChildNodes.Item(1).InnerText.Trim().Replace("\t", "").Split(',');
                    string Tag = XmlNode[i].ChildNodes.Item(2).InnerText.Trim();
                    string ShortTag = XmlNode[i].ChildNodes.Item(3).InnerText.Trim();
                    int Group = int.Parse(XmlNode[i].ChildNodes.Item(4).InnerText.Trim());

                    int NextGroup = 0;
                    if (XmlNode[i].NextSibling != null) { NextGroup = int.Parse(XmlNode[i].NextSibling.ChildNodes.Item(4).InnerText.Trim()); }

                    Rectangle Section = new Rectangle(new Point(int.Parse(StrPoint[0]), int.Parse(StrPoint[1])), new Size(int.Parse(StrSize[0]), int.Parse(StrSize[0])));

                    Sections.Add(Section);
                    Tags.Add(Tag + "_" + j++); ;

                    if (Group == 0)
                    {
                        new Effects(Sections, Image, Tags, Tag, ShortTag);

                        Sections = new List<Rectangle>();
                        Tags = new List<string>();

                        j = 0;
                    }
                    else if (Group != 0 && Group != NextGroup)
                    {
                        new Effects(Sections, Image, Tags, Tag, ShortTag);

                        Sections = new List<Rectangle>();
                        Tags = new List<string>();

                        j = 0;
                    }


                }
                catch (NullReferenceException ex)
                {
                    Log.Error($"[EFFECTS] - Unable to Register: {ex.Message}");
                }
            }
        }

        public void DestroySelf()
        {
            Log.Info($"[EFFECTS]({Tag}) - Has been destroyed!");
            EEngine.UnRegisterEffect(this);
        }

    }
}
