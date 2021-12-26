using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Threading;
using System.Threading.Tasks;

namespace EEngine.EEngine
{
    class Canvas : Form
    {
        public Canvas()
        {
            this.DoubleBuffered = true;
        }
    }

    public abstract class EEngine
    {
        private static Vector2 ScreenSize = new Vector2(480, 640);
        private string Title = "New Game";
        private Canvas Window = null;
        private Thread GameLoopThread = null;
        private System.Timers.Timer AnimationTimer = null;
        //private Task GameLoopTask = null;

        private static Map map; //temporary until list of levels implemented

        private static List<Units> AllUnits = new List<Units>();
        private static List<Armies> Armies = new List<Armies>();
        private static List<Tiles> AllTiles = new List<Tiles>();
        private static List<Buildings> AllBuildings = new List<Buildings>();
        private static List<Buildings> ABuilding = new List<Buildings>();
        private static List<List<Map>> Map = new List<List<Map>>();
        private static List<Effects> AllEffects = new List<Effects>();

        private static int Timers = 0;
        private static int TimerLimits = 12; //Sets how fast the movement and animations play. Can be affected by processor speed.
        private static int Unit_Frame = 0; //Used to sync up all the Unit animations when creating Army
        private static bool Load = false;

        public static int TimerLimit { set { TimerLimits = value; } }

        public Color BackgroundColor = Color.Beige;

        private static Vector2 CameraPosition = Vector2.Zero();
        public float CameraAngle = 0f;

        public EEngine(Vector2 ScreenSize, string Title)
        {
            Log.Success("Game is Starting...");
            EEngine.ScreenSize = ScreenSize;
            this.Title = Title;

            Window = new Canvas();
            Window.Size = new Size((int)EEngine.ScreenSize.X, (int)EEngine.ScreenSize.Y);
            Window.Text = this.Title;
            Window.Paint += Renderer;
            Window.KeyDown += Window_KeyDown;
            Window.KeyUp += Window_KeyUp;
            Window.MouseDown += Window_MouseDown;
            Window.MouseUp += Window_MouseUp;
            Window.MouseWheel += Window_MouseWheel;
            Window.MouseMove += Window_MouseMove;
            Window.FormClosed += Window_FormClosed;

            //GameLoopTask = Task.Factory.StartNew(GameLoop);
            GameLoopThread = new Thread(GameLoop)
            {
                IsBackground = true,
                Priority = ThreadPriority.Highest
            };
            GameLoopThread.Start();

            AnimationTimer = new System.Timers.Timer(200);
            AnimationTimer.Elapsed += OnTimedEvent;
            AnimationTimer.AutoReset = true;

            Application.Run(Window);
        }



        private void Window_FormClosed(object sender, FormClosedEventArgs e)
        {
            GameLoopThread.Abort();

            AnimationTimer.Stop();
            AnimationTimer.Dispose();
        }
        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            GetKeyUp(e);
        }
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            GetKeyDown(e);
        }
        private void Window_MouseUp(object sender, MouseEventArgs e)
        {
            GetMouseUp(e);
        }
        private void Window_MouseDown(object sender, MouseEventArgs e)
        {
            GetMouseDown(e);
        }
        private void Window_MouseWheel(object sender, MouseEventArgs e)
        {
            GetMouseWheel(e);
        }
        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            GetMouseMove(e);
        }


        public static Vector2 GetScreenCenter()
        {
            return new Vector2((ScreenSize.X / 2), (ScreenSize.Y / 2));
        }
        public static Vector2 GetCameraPosition()
        {
            return CameraPosition;
        }
        public static void SetCameraPositionY(float Change)
        {
            CameraPosition.Y += Change;
        }
        public static void SetCameraPositionX(float Change)
        {
            CameraPosition.X += Change;
        }


        public static void RegisterUnit(Units Unit)
        {
            AllUnits.Add(Unit);
        }
        public static void UnRegisterUnit(Units Unit)
        {
            AllUnits.Remove(Unit);
        }
        public static Units GetUnit(int Index)
        {
            if (AllUnits.ElementAtOrDefault(Index) != null) { return AllUnits[Index]; }
            else { return null; }
        }
        public static void SetArmyUnit(Units Unit, bool Active)
        {
            int Index = Armies.FindIndex(x => x.Unit.ID == Unit.ID);
            Armies[Index].Unit.Available = Active;
            Armies[Index].Unit.UnavailableIdle();
            Armies[Index].Unit.Health_Effect.Unavailable();
        }




        public static void RegisterArmy(Armies Army)
        {
            Armies.Add(Army);
        }
        public static void UnRegisterArmy(Armies Army)
        {
            Armies.Remove(Army);
        }
        public static Armies GetArmyUnit(int Index)
        {
            if (AllUnits.ElementAtOrDefault(Index) != null) { return Armies[Index]; }
            else { return null; }
        }
        public static Armies GetArmyUnit(Vector2 Position)
        {
            foreach (Armies ArmyUnit in Armies.ToList())
            {
                if (Position.X.IsBetween(ArmyUnit.Unit.GetUnitUpperBoundingPosition().X, ArmyUnit.Unit.GetUnitLowerBoundingPosition().X)
                    && Position.Y.IsBetween(ArmyUnit.Unit.GetUnitUpperBoundingPosition().Y, ArmyUnit.Unit.GetUnitLowerBoundingPosition().Y))
                { return ArmyUnit; }
            }

            return null;
        }
        /// <summary>
        /// Returns the Tag/Name of the Unit based on the position on the game screen
        /// </summary>
        /// <param name="Position">The Position on the game screen</param>
        /// <returns></returns>
        public static String GetArmyUnitTag(Vector2 Position)
        {
            String Army_Unit_Array_Tag = "";

            foreach (Armies ArmyUnit in Armies.ToList())
            {
                if (Position.X.IsBetween(ArmyUnit.Unit.GetUnitUpperBoundingPosition().X, ArmyUnit.Unit.GetUnitLowerBoundingPosition().X)
                    && Position.Y.IsBetween(ArmyUnit.Unit.GetUnitUpperBoundingPosition().Y, ArmyUnit.Unit.GetUnitLowerBoundingPosition().Y))
                { Army_Unit_Array_Tag = ArmyUnit.Unit.Tag; }
            }

            return Army_Unit_Array_Tag;
        }

        public static void CreateArmyUnit(Vector2 Position, Vector2 Scale, int Index, Vector2 Level_Array_Position)
        {
            new Armies(Position, Scale, GetUnit(Index), Unit_Frame, EEngine.GetEffect(0), EEngine.GetEffect(1), Level_Array_Position);
        }
        public static void CreateArmyUnit(Vector2 Position, float Scale, int Index, Vector2 Level_Array_Position)
        {
            new Armies(Position, Scale, GetUnit(Index), Unit_Frame, EEngine.GetEffect(0), EEngine.GetEffect(1), Level_Array_Position);
        }
        /*public static void DeleteArmyUnit()
        {
            Armies.RemoveAt(0);
            Log.Normal("Unit Removed");
        }*/


        public static void RegisterEffect(Effects Effect)
        {
            AllEffects.Add(Effect);
        }
        public static void UnRegisterEffect(Effects Effect)
        {
            AllEffects.Remove(Effect);
        }
        public static Effects GetEffect(int Index)
        {
            if (AllUnits.ElementAtOrDefault(Index) != null) { return AllEffects[Index]; }
            else { return null; }
        }


        public static void RegisterBuilding(Buildings Building)
        {
            AllBuildings.Add(Building);
        }
        public static void RegisterABuilding(Buildings Building)
        {
            ABuilding.Add(Building);
        }
        public static void UnRegisterBuilding(Buildings Building)
        {
            AllBuildings.Remove(Building);
        }
        public static Buildings GetBuilding(int Index)
        {
            return AllBuildings[Index];
        }
        public static void CreateBuilding(Vector2 Position, float Scale, int Index)
        {
            new Buildings(Position, Scale, GetBuilding(Index), 0, true);
        }


        public static void RegisterTile(Tiles Tile)
        {
            AllTiles.Add(Tile);
        }
        public static void UnRegisterTile(Tiles Tile)
        {
            AllTiles.Remove(Tile);
        }
        public static Tiles GetTile(int Index)
        {
            return AllTiles[Index];
        }
        public static Tiles GetTile(string ShortTag)
        {
            foreach (Tiles tile in AllTiles)
            {
                if (tile.ShortTag == ShortTag) { return tile; }
            }
            return null;
        }
        public static Tiles GetTile(Tiles Tile)
        {
            foreach(Tiles tile in AllTiles)
            {
                if (tile == Tile) { return tile; }
            }
            return null;
        }


        /// <summary>
        /// Initializes the internal lists of the multidimensional list "Map"
        /// </summary>
        /// <param name="Row_Y">Number of internal lists. Based on the MapArray height</param>
        public static void InitializeMapArrayY(int Row_Y)
        {
            for(int y = 0; y < Row_Y; y++)
            {
                Map.Add(new List<Map>());
            }
        }
        public static void RegisterMap(Map map)
        {
            Map[(int)map.Map_Array_Index.Y].Add(map);
        }
        public static void UnRegisterMap(Map map)
        {
            Map[(int)map.Map_Array_Index.Y].Remove(map);
        }
        public static void CreateMap(float Scale, string[,] Map)
        {
            //GetTile(0).Tile_Scale should be the same for all tiles
           map = new Map(Scale, GetTile(0).vScale, Map);
        }
        public static Map GetMapItem(Vector2 World_Position)
        {
            Vector2 Map_Array_Index = map.GetMapTileArrayIndex(World_Position);

            if (Map_Array_Index != Vector2.Negative()) { return Map[(int)Map_Array_Index.Y][(int)Map_Array_Index.X]; }
            else { return null; }
        }


        public static void SetMapTileFog()
        {
            for (int i = 0; i < Map.Count; i++)
            {
                for (int j = 0; j < Map[i].Count; j++)
                {
                    Map[i][j].Tile.Normal();
                }
            } 
        }
        public static void SetMapTileFogVision(Vector2 Map_Array_Position, int Vision)
        {
            Map_Array_Position.X -= Vision - 1; //Start Position along X axis
            int VisionSize = ((Vision * 2) - 1); //Vision Width

            for (int i = 0; i < Vision; i++)
            {
                for (int j = (i * -1); j <= i; j++)
                {
                    //if (!((int)Level_Array_Position.Y + j).IsBetween(0, (Levels.Count - 1))
                    //    || !((int)Level_Array_Position.X + i).IsBetween(0, (Levels[(int)Level_Array_Position.Y + j].Count - 1)))
                    //{ continue; }
                    //Levels[(int)Level_Array_Position.Y + j][(int)Level_Array_Position.X + i].Level_Tile.Normal();

                    if (((int)Map_Array_Position.Y + j).IsBetween(0, (Map.Count - 1))
                        && ((int)Map_Array_Position.X + i).IsBetween(0, (Map[(int)Map_Array_Position.Y + j].Count - 1)))
                    { Map[(int)Map_Array_Position.Y + j][(int)Map_Array_Position.X + i].Tile.Normal(); }

                    //if (!((int)Level_Array_Position.X + ((VisionSize - 1) - i)).IsBetween(0, (Levels[(int)Level_Array_Position.Y + j].Count - 1)))
                    //{ continue; }
                    //Levels[(int)Level_Array_Position.Y + j][(int)Level_Array_Position.X + ((VisionSize - 1) - i)].Level_Tile.Normal();

                    if (((int)Map_Array_Position.Y + j).IsBetween(0, (Map.Count - 1))
                        && ((int)Map_Array_Position.X + ((VisionSize - 1) - i)).IsBetween(0, (Map[(int)Map_Array_Position.Y + j].Count - 1)))
                    { Map[(int)Map_Array_Position.Y + j][(int)Map_Array_Position.X + ((VisionSize - 1) - i)].Tile.Normal(); }

                }
            }




            //for (int y = ((Vision - 1) * -1); y < Vision; y++)
            //{
            //    if (!((int)Level_Array_Position.Y + y).IsBetween(0, Levels.Count)) { continue; }

            //    for (int x = ((Vision - 1) * -1); x < Vision; x++)
            //    {
            //        if (!((int)Level_Array_Position.X + x).IsBetween(0, Levels[(int)Level_Array_Position.Y + y].Count)) { continue; }
            //        Levels[(int)Level_Array_Position.Y + y][(int)Level_Array_Position.X + x].Level_Tile.Normal();
            //    }
            //}

        }

        public static void Loaded()
        {
            Load = true;
        }

        private void FrameTile()
        {
            foreach (List<Map> levelY in Map.ToList())
            {
                foreach (Map levelX in levelY)
                {
                    if (levelX.Tile.Sprite[levelX.Tile.AnimationSet].Sprite.Count > 1) { levelX.Tile.Frame++; }
                    if (levelX.Tile.Frame == levelX.Tile.Sprite[levelX.Tile.AnimationSet].Sprite.Count) { levelX.Tile.Frame = 0; }
                }
            }
        }
        private void FrameBuilding()
        {
            foreach (Buildings building in ABuilding)
            {
                if (building.Building_Sprite[building.AnimationSet].Sprite.Count > 1) { building.Frame++; }
                if (building.Frame == building.Building_Sprite[building.AnimationSet].Sprite.Count) { building.Frame = 0; }

            }
        }
        private void FrameUnit()
        {
            Unit_Frame++;
            foreach (Armies army in Armies)
            {
                if (Unit_Frame == army.Unit.Unit_Sprite[army.Unit.AnimationSet].Sprite.Count)
                {
                    Unit_Frame = 0;
                }

                army.Unit.Frame = Unit_Frame;
            }
        }



        void GameLoop()
        {
            OnLoad();
            AnimationTimer.Start();
            while (/*GameLoopTask.Status.Equals(TaskStatus.Running)*/GameLoopThread.IsAlive)
            {
                try
                {
                    OnDraw();
                    Window.BeginInvoke((MethodInvoker) delegate { Window.Refresh(); });
                    OnUpdate();
                    Thread.Sleep(10);
                    //GameLoopTask.Wait(2);
                }
                catch
                {
                    Log.Error("Game has not been found...");
                }
            }
        }
        private void Renderer(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            g.Clear(BackgroundColor);

            g.CompositingMode = CompositingMode.SourceOver;
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.InterpolationMode = InterpolationMode.NearestNeighbor;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;

            g.TranslateTransform(CameraPosition.X, CameraPosition.Y);
            g.RotateTransform(CameraAngle);

            if (Load)
            {
                foreach (List<Map> levelY in Map.ToList())
                {
                    foreach (Map levelX in levelY.ToList())
                    {
                        g.DrawImage(levelX.Tile.Sprite[levelX.Tile.AnimationSet].Sprite[levelX.Tile.Frame].Sprite,
                            levelX.Position.X,
                            levelX.Position.Y,
                            levelX.Scale.X,
                            levelX.Scale.Y);
                    }
                }

                foreach (Buildings building in ABuilding.ToList())
                {
                    g.DrawImage(building.Building_Sprite[building.AnimationSet].Sprite[building.Frame].Sprite,
                    building.Position.X,
                    building.Position.Y,
                    building.vScale.X,
                    building.vScale.Y);
                }

                foreach (Armies army in Armies.ToList())
                {
                    //Unit
                    g.DrawImage(army.Unit.Unit_Sprite[army.Unit.AnimationSet].Sprite[army.Unit.Frame].Sprite,
                        army.Unit.CalculateUnitOffsetOrigin().X,
                        army.Unit.CalculateUnitOffsetOrigin().Y,
                        army.Unit.vScale.X,
                        army.Unit.vScale.Y);
                    //Unit Effect (Health, Ammo, Fuel)
                    g.DrawImage(army.Unit.Health_Effect.Effect_Sprite[army.Unit.Health_Effect.AnimationSet].Sprite[army.Unit.GetUnitHeath()].Sprite,
                        army.Unit.GetUnitHealthEffect(army.Unit.CalculateUnitOffsetOrigin()).X,
                        army.Unit.GetUnitHealthEffect(army.Unit.CalculateUnitOffsetOrigin()).Y,
                        army.Unit.Health_Effect.Scale.X,
                        army.Unit.Health_Effect.Scale.Y);
                    g.DrawImage(army.Unit.Supply_Effect.Effect_Sprite[army.Unit.Supply_Effect.AnimationSet].Sprite[army.Unit.GetUnitSupply()].Sprite,
                        army.Unit.GetUnitSupplyEffect(army.Unit.CalculateUnitOffsetOrigin()).X,
                        army.Unit.GetUnitSupplyEffect(army.Unit.CalculateUnitOffsetOrigin()).Y,
                        army.Unit.Supply_Effect.Scale.X,
                        army.Unit.Supply_Effect.Scale.Y);
                }
                g.DrawString("Test Text", new Font("Arial", 16), new SolidBrush(Color.White), new PointF(0f - GetCameraPosition().X, 0f - GetCameraPosition().Y));

            }



            Timers++;
            if (Timers > TimerLimits) { Timers = 0; }
        }
        private void OnTimedEvent(object sender, System.Timers.ElapsedEventArgs e)
        {
            FrameTile();
            FrameUnit();
            FrameBuilding();
            Log.Success(e.SignalTime.ToString());
        }



        public abstract void OnLoad();
        public abstract void OnUpdate();
        public abstract void OnDraw();
        public abstract void GetKeyDown(KeyEventArgs e);
        public abstract void GetKeyUp(KeyEventArgs e);
        public abstract void GetMouseDown(MouseEventArgs e);
        public abstract void GetMouseUp(MouseEventArgs e);
        public abstract void GetMouseWheel(MouseEventArgs e);
        public abstract void GetMouseMove(MouseEventArgs e);
    }
}
