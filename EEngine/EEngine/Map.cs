using System;
using System.Collections.Generic;
using System.Xml;

namespace EEngine.EEngine
{
    public class Map
    {
        public Vector2 StartPosition = Vector2.Zero();
        public Vector2 Position { get; private set; } = Vector2.Zero();
        public Vector2 Scale = Vector2.Zero();
        public Vector2 Size = Vector2.Zero();
        public Tiles Tile = null;
        public Vector2 Map_Array_Index { get; private set; } = Vector2.Zero();

        public Map(Vector2 Position, Vector2 Scale, Tiles Tile, Vector2 Map_Array_Index)
        {
            this.Position = Position;
            this.Scale = Scale;
            this.Tile = new Tiles(Scale, Tile.Sprite, Tile.Tag, Tile.ShortTag, false);
            this.Map_Array_Index = Map_Array_Index;

            if (Tile != null) { Log.Info($"[LEVEL]({Tile.Tag} Tile at Position {this.Position.X.ToString()}:{this.Position.Y.ToString()}) - Has been registered!"); }
            EEngine.RegisterMap(this);
        }

        public Map(Vector2 Scale, string[,] MapArray)
        {
            this.Scale = Scale;
            this.Size = new Vector2(MapArray.GetLength(1), MapArray.GetLength(0));
            this.StartPosition = (Size * Scale) / 2;

            StartPosition.X = EEngine.GetScreenCenter().X - StartPosition.X - 10;
            StartPosition.Y = EEngine.GetScreenCenter().Y - StartPosition.Y - 20;

            EEngine.InitializeMapArrayY(MapArray.GetLength(0));
            for (int i = 0; i < MapArray.GetLength(0); i++)
            {
                for (int j = 0; j < MapArray.GetLength(1); j++)
                {
                    new Map(new Vector2(j * Scale.X + StartPosition.X, i * Scale.Y + StartPosition.Y), Scale, EEngine.GetTile(MapArray[i,j]), new Vector2(j, i));
                }
            }
            EEngine.Loaded();
        }
        public Map(float fScale, Vector2 Scale, string[,] MapArray)
        {
            Vector2 Scale_2 = Scale * fScale;
            this.Scale = Scale_2;
            this.Size = new Vector2(MapArray.GetLength(1), MapArray.GetLength(0));
            this.StartPosition = (Size * Scale_2) / 2;


            StartPosition.X = EEngine.GetScreenCenter().X - StartPosition.X - 10;
            StartPosition.Y = EEngine.GetScreenCenter().Y - StartPosition.Y - 20;

            EEngine.InitializeMapArrayY(MapArray.GetLength(0));
            for (int i = 0; i < MapArray.GetLength(0); i++)
            {
                for (int j = 0; j < MapArray.GetLength(1); j++)
                {
                    new Map(new Vector2(j * Scale_2.X + StartPosition.X, i * Scale_2.Y + StartPosition.Y), Scale_2, EEngine.GetTile(MapArray[i, j]), new Vector2(j, i));
                }
            }

            EEngine.Loaded();
        }


        public Vector2 GetMapTileArrayIndex(Vector2 WorldPosition)
        {
            if (WorldPosition.X < (StartPosition.X + EEngine.GetCameraPosition().X) 
                || WorldPosition.Y < (StartPosition.Y + EEngine.GetCameraPosition().Y) 
                || WorldPosition.X > ((StartPosition.X + EEngine.GetCameraPosition().X) + (Scale.X * Size.X)) 
                || WorldPosition.Y > ((StartPosition.Y + EEngine.GetCameraPosition().Y) + (Scale.Y * Size.Y)))
            { return Vector2.Negative(); }
            else
            { return new Vector2((float)Math.Floor((WorldPosition.X - (StartPosition.X + EEngine.GetCameraPosition().X)) / Scale.X), (float)Math.Floor((WorldPosition.Y - (StartPosition.Y + EEngine.GetCameraPosition().Y)) / Scale.Y)); }
        }


        public Map(XmlDocument Doc)
        {
            XmlNodeList XmlNode;

            XmlNode = Doc.GetElementsByTagName("Tile");

            for (int i = 0; i <= XmlNode.Count - 1; i++)
            {
                try
                {
                    //string[] StrPoint = XmlNode[i].ChildNodes.Item(0).InnerText.Trim().Split(',');
                    //string[] StrSize = XmlNode[i].ChildNodes.Item(1).InnerText.Trim().Split(',');
                    //string Tag = XmlNode[i].ChildNodes.Item(2).InnerText.Trim();
                    //string ShortTag = XmlNode[i].ChildNodes.Item(3).InnerText.Trim();

                    //Rectangle Section = new Rectangle(new Point(int.Parse(StrPoint[0]), int.Parse(StrPoint[1])), new Size(int.Parse(StrSize[0]), int.Parse(StrSize[0])));

                    //new Tiles(Section, Image, Tag, ShortTag);
                }
                catch (NullReferenceException ex)
                {
                    Log.Error($"[TILES] - Unable to Register due to Null Reference: {ex.Message}");
                }
            }
        }
    }
}
