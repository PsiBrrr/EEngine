using System;
using System.Collections.Generic;
using System.Drawing;
using System.Xml;

namespace EEngine.EEngine
{
    public class Buildings
    {
        public Vector2 Building_Position = Vector2.Zero();
        public Vector2 Building_Scale = Vector2.Zero();
        public Tiles Building_Tile = null;
        public Vector2 Building_Array_Position { get; private set; } = Vector2.Zero();

        public Buildings(Vector2 Building_Position, Vector2 Building_Scale, Tiles Building_Tile, Vector2 Building_Array_Position)
        {
            this.Building_Position = Building_Position;
            this.Building_Scale = Building_Scale;
            this.Building_Tile = new Tiles(Building_Scale, Building_Tile.Tile_Sprite, Building_Tile.Tag, Building_Tile.ShortTag, false);
            this.Building_Array_Position = Building_Array_Position;

            if (Building_Tile != null) { Log.Info($"[BUILDING]({Building_Tile.Tag} Tile at Position {this.Building_Position.X.ToString()}:{this.Building_Position.Y.ToString()}) - Has been registered!"); }
            EEngine.RegisterBuilding(this);
        }

        public Buildings(Vector2 Scale, string[,] BuildingArray)
        {
            Vector2 StartPosition = Vector2.Zero();
            for (int x = 0; x < BuildingArray.GetLength(1); x++) { StartPosition.X += Scale.X / 2; }
            for (int y = 0; y < BuildingArray.GetLength(0); y++) { StartPosition.Y += Scale.Y / 2; }

            StartPosition.X = EEngine.GetScreenCenter().X - StartPosition.X - 10;
            StartPosition.Y = EEngine.GetScreenCenter().Y - StartPosition.Y - 20;

            EEngine.InitializeLevelArry(BuildingArray.GetLength(0));
            for (int i = 0; i < BuildingArray.GetLength(0); i++)
            {
                for (int j = 0; j < BuildingArray.GetLength(1); j++)
                {
                    new Levels(new Vector2(j * Scale.X + StartPosition.X, i * Scale.Y + StartPosition.Y), Scale, EEngine.GetTile(BuildingArray[i, j]), new Vector2(j, i));
                }
            }
            EEngine.Loaded();
        }

        public void DestroySelf()
        {
            Log.Info($"[BUILDINGS]({this.Building_Tile.Tag}) - Has been destroyed!");
            EEngine.UnRegisterBuilding(this);
        }
    }
}
