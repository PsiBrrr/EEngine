using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace EEngine.EEngine
{
    public class Levels
    {
        public Vector2 Position = Vector2.Zero();
        public Vector2 Scale = Vector2.Zero();
        public Tiles Tile = null;
        public Vector2 Level_Array_Position { get; private set; } = Vector2.Zero();

        public Levels(Vector2 Position, Vector2 Scale, Tiles Tile, Vector2 Level_Array_Position)
        {
            this.Position = Position;
            this.Scale = Scale;
            this.Tile = new Tiles(Scale, Tile.Sprite, Tile.Tag, Tile.ShortTag, false);
            this.Level_Array_Position = Level_Array_Position;

            if (Tile != null) { Log.Info($"[LEVEL]({Tile.Tag} Tile at Position {this.Position.X.ToString()}:{this.Position.Y.ToString()}) - Has been registered!"); }
            EEngine.RegisterLevel(this);
        }

        public Levels(Vector2 Tile_Scale, string[,] MapArray)
        {
            Vector2 StartPosition = Vector2.Zero();
            for (int x = 0; x < MapArray.GetLength(1); x++) { StartPosition.X += Tile_Scale.X / 2; }
            for (int y = 0; y < MapArray.GetLength(0); y++) { StartPosition.Y += Tile_Scale.Y / 2; }

            StartPosition.X = EEngine.GetScreenCenter().X - StartPosition.X - 10;
            StartPosition.Y = EEngine.GetScreenCenter().Y - StartPosition.Y - 20;

            EEngine.InitializeLevelArry(MapArray.GetLength(0));
            for (int i = 0; i < MapArray.GetLength(0); i++)
            {
                for (int j = 0; j < MapArray.GetLength(1); j++)
                {
                    new Levels(new Vector2(j * Tile_Scale.X + StartPosition.X, i * Tile_Scale.Y + StartPosition.Y), Tile_Scale, EEngine.GetTile(MapArray[i,j]), new Vector2(j, i));
                }
            }
            EEngine.Loaded();
        }
        public Levels(float Scale, Vector2 Tile_Scale, string[,] MapArray)
        {
            Vector2 StartPosition = Vector2.Zero();
            Vector2 Tile_Scale_2 = Tile_Scale * Scale;
            for (int x = 0; x < MapArray.GetLength(1); x++) { StartPosition.X += Tile_Scale_2.X / 2; }
            for (int y = 0; y < MapArray.GetLength(0); y++) { StartPosition.Y += Tile_Scale_2.Y / 2; }

            StartPosition.X = EEngine.GetScreenCenter().X - StartPosition.X - 10;
            StartPosition.Y = EEngine.GetScreenCenter().Y - StartPosition.Y - 20;

            EEngine.InitializeLevelArry(MapArray.GetLength(0));
            for (int i = 0; i < MapArray.GetLength(0); i++)
            {
                for (int j = 0; j < MapArray.GetLength(1); j++)
                {
                    new Levels(new Vector2(j * Tile_Scale_2.X + StartPosition.X, i * Tile_Scale_2.Y + StartPosition.Y), Tile_Scale_2, EEngine.GetTile(MapArray[i, j]), new Vector2(j, i));
                }
            }
            EEngine.Loaded();
        }


        public Levels(XmlDocument Doc)
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
