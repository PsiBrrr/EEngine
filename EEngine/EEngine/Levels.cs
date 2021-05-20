using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace EEngine.EEngine
{
    public class Level
    {
        public Vector2 Level_Position = Vector2.Zero();
        public Vector2 Level_Scale = Vector2.Zero();
        public Tiles Level_Tile = null;
        public Vector2 Level_Array_Position { get; private set; } = Vector2.Zero();

        public Level(Vector2 Level_Position, Vector2 Level_Scale, Tiles Level_Tile, Vector2 Level_Array_Position)
        {
            this.Level_Position = Level_Position;
            this.Level_Scale = Level_Scale;
            this.Level_Tile = new Tiles(Level_Scale, Level_Tile.Tile_Sprite, Level_Tile.Tag, Level_Tile.ShortTag, false);
            this.Level_Array_Position = Level_Array_Position;

            if (Level_Tile != null) { Log.Info($"[LEVEL]({Level_Tile.Tag} Tile at Position {this.Level_Position.X.ToString()}:{this.Level_Position.Y.ToString()}) - Has been registered!"); }
            EEngine.RegisterLevel(this);
        }

        public Level(Vector2 Scale, string[,] MapArray)
        {
            Vector2 StartPosition = Vector2.Zero();
            for (int x = 0; x < MapArray.GetLength(1); x++) { StartPosition.X += Scale.X / 2; }
            for (int y = 0; y < MapArray.GetLength(0); y++) { StartPosition.Y += Scale.Y / 2; }

            StartPosition.X = EEngine.GetScreenCenter().X - StartPosition.X - 10;
            StartPosition.Y = EEngine.GetScreenCenter().Y - StartPosition.Y - 20;

            EEngine.InitializeLevel(MapArray.GetLength(0));
            for (int i = 0; i < MapArray.GetLength(0); i++)
            {
                for (int j = 0; j < MapArray.GetLength(1); j++)
                {
                    new Level(new Vector2(j * Scale.X + StartPosition.X, i * Scale.Y + StartPosition.Y), Scale, EEngine.GetTile(MapArray[i,j]), new Vector2(j, i));
                }
            }
            EEngine.Loaded();
        }


        public Level(XmlDocument Doc)
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
