using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;
using System.Drawing.Drawing2D;
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
        //private Task GameLoopTask = null;

        private static List<Units> AllUnits = new List<Units>();
        private static List<Armies> Armies = new List<Armies>();
        private static List<Tiles> AllTiles = new List<Tiles>();
<<<<<<< Updated upstream
        private static List<List<Level>> Levels = new List<List<Level>>();
=======
        private static List<Buildings> AllBuildings = new List<Buildings>();
        private static List<List<Levels>> Levels = new List<List<Levels>>();
>>>>>>> Stashed changes
        private static List<Effects> AllEffects = new List<Effects>();

        private static int Timers = 0;
        private static int TimerLimits = 12; //Sets how fast the movement and animations play. Can be affected by processor speed.
        private static bool Load = false;

        public static int TimerLimit { set { TimerLimits = value; } }

        public Color BackgroundColor = Color.Beige;

        public Vector2 CameraPosition = Vector2.Zero();
        public float CameraAngle = 0f;

        public EEngine(Vector2 ScreenSize, string Title)
        {
            Log.Success("Game is Starting");
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
            Window.FormClosed += Window_FormClosed;

            //GameLoopTask = Task.Factory.StartNew(GameLoop);
            GameLoopThread = new Thread(GameLoop)
            {
                IsBackground = true,
                Priority = ThreadPriority.Highest
            };
            GameLoopThread.Start();

            Application.Run(Window);
        }

        private void Window_FormClosed(object sender, FormClosedEventArgs e)
        {
            GameLoopThread.Abort();
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

        public static Vector2 GetScreenCenter()
        {
            return new Vector2((ScreenSize.X / 2), (ScreenSize.Y / 2));
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
                if (Position.X.IsBetween(ArmyUnit.Unit.CalculateUnitUpperBoundingPosition().X, ArmyUnit.Unit.CalculateUnitLowerBoundingPosition().X)
                    && Position.Y.IsBetween(ArmyUnit.Unit.CalculateUnitUpperBoundingPosition().Y, ArmyUnit.Unit.CalculateUnitLowerBoundingPosition().Y))
                { Army_Unit_Array_Tag = ArmyUnit.Unit.Tag; }
            }

            return Army_Unit_Array_Tag;
        }

        public static void CreateArmyUnit(Vector2 Position, Vector2 Scale, int Index, Vector2 Level_Array_Position)
        {
            new Armies(Position, Scale, GetUnit(Index), EEngine.GetEffect(0), EEngine.GetEffect(1), Level_Array_Position);
        }
        public static void CreateArmyUnit(Vector2 Position, float Scale, int Index, Vector2 Level_Array_Position)
        {
            new Armies(Position, Scale, GetUnit(Index), EEngine.GetEffect(0), EEngine.GetEffect(1), Level_Array_Position);
        }
        /*public static void DeleteArmyUnit()
        {
            Armies.RemoveAt(0);
            Log.Normal("Unit Removed");
        }*/

<<<<<<< Updated upstream
=======

        public static void RegisterBuilding(Buildings Building)
        {
            AllBuildings.Add(Building);
        }
        public static void UnRegisterBuilding(Buildings Building)
        {
            AllBuildings.Remove(Building);
        }
>>>>>>> Stashed changes

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
        /// Initializes the internal lists of the multidimensional list "Levels"
        /// </summary>
        /// <param name="Row_Y">Number of internal lists. Based on the MapArray height</param>
        public static void InitializeLevelArry(int Row_Y)
        {
            for(int y = 0; y < Row_Y; y++)
            {
                Levels.Add(new List<Levels>());
            }
        }
        public static void RegisterLevel(Levels Level)
        {
            Levels[(int)Level.Level_Array_Position.Y].Add(Level);
        }
        public static void UnRegisterLevel(Levels Level)
        {
            Levels[(int)Level.Level_Array_Position.Y].Remove(Level);
        }
        public static Vector2 GetLevelTilePosition(Vector2 Position)
        {
            Vector2 Level_Array_Position = Vector2.Zero();

            foreach (List<Levels> levelY in Levels.ToList())
            {
                foreach (Levels levelX in levelY.ToList())
                {
                    if (Position.X.IsBetween(levelX.Level_Position.X, (levelX.Level_Position.X + levelX.Level_Scale.X)) 
                        && Position.Y.IsBetween(levelX.Level_Position.Y, (levelX.Level_Position.Y + levelX.Level_Scale.Y)))
                    { Level_Array_Position = levelX.Level_Position; }
                }
            }

            return Level_Array_Position;
        }
        public static String GetLevelTileTag(Vector2 Position)
        {
            String Level_Array_Tag = "";

            foreach (List<Levels> levelY in Levels.ToList())
            {
                foreach (Levels levelX in levelY.ToList())
                {
                    if (Position.X.IsBetween(levelX.Level_Position.X, (levelX.Level_Position.X + levelX.Level_Scale.X))
                        && Position.Y.IsBetween(levelX.Level_Position.Y, (levelX.Level_Position.Y + levelX.Level_Scale.Y)))
                    { Level_Array_Tag = levelX.Level_Tile.Tag; }
                }
            }

            return Level_Array_Tag;
        }
        public static void CreateLevel(float Scale, string[,] Map)
        {
            //GetTile(0).Tile_Scale should be the same for all tiles
            new Levels(Scale, GetTile(0).Tile_Scale, Map);
        }

        public static void SetLevelTileFog()
        {
            for (int i = 0; i < Levels.Count; i++)
            {
                for (int j = 0; j < Levels[i].Count; j++)
                {
                    Levels[i][j].Level_Tile.Normal();
                }
            } 
        }
        public static void SetLevelTileFogVision(Vector2 Level_Array_Position, int Vision)
        {
            Level_Array_Position.X -= Vision - 1; //Start Position along X axis
            int VisionSize = ((Vision * 2) - 1); //Vision Width

            for (int i = 0; i < Vision; i++)
            {
                for (int j = (i * -1); j <= i; j++)
                {
                    //if (!((int)Level_Array_Position.Y + j).IsBetween(0, (Levels.Count - 1))
                    //    || !((int)Level_Array_Position.X + i).IsBetween(0, (Levels[(int)Level_Array_Position.Y + j].Count - 1)))
                    //{ continue; }
                    //Levels[(int)Level_Array_Position.Y + j][(int)Level_Array_Position.X + i].Level_Tile.Normal();

                    if (((int)Level_Array_Position.Y + j).IsBetween(0, (Levels.Count - 1))
                        && ((int)Level_Array_Position.X + i).IsBetween(0, (Levels[(int)Level_Array_Position.Y + j].Count - 1)))
                    { Levels[(int)Level_Array_Position.Y + j][(int)Level_Array_Position.X + i].Level_Tile.Normal(); }

                    //if (!((int)Level_Array_Position.X + ((VisionSize - 1) - i)).IsBetween(0, (Levels[(int)Level_Array_Position.Y + j].Count - 1)))
                    //{ continue; }
                    //Levels[(int)Level_Array_Position.Y + j][(int)Level_Array_Position.X + ((VisionSize - 1) - i)].Level_Tile.Normal();

                    if (((int)Level_Array_Position.Y + j).IsBetween(0, (Levels.Count - 1))
                        && ((int)Level_Array_Position.X + ((VisionSize - 1) - i)).IsBetween(0, (Levels[(int)Level_Array_Position.Y + j].Count - 1)))
                    { Levels[(int)Level_Array_Position.Y + j][(int)Level_Array_Position.X + ((VisionSize - 1) - i)].Level_Tile.Normal(); }

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


        private void FrameTile(int Timer)
        {
            if (Timer == TimerLimits)
            {
                foreach (List<Levels> levelY in Levels.ToList())
                {
                    foreach (Levels levelX in levelY)
                    {
                        if (levelX.Level_Tile.Tile_Sprite[levelX.Level_Tile.AnimationSet].Sprite.Count > 1) { levelX.Level_Tile.Tile_Frame++; }
                        if (levelX.Level_Tile.Tile_Frame == levelX.Level_Tile.Tile_Sprite[levelX.Level_Tile.AnimationSet].Sprite.Count) { levelX.Level_Tile.Tile_Frame = 0; }
                    }
                }
            }
        }
        private void FrameBuilding(int Timer)
        {
            if (Timer == TimerLimits)
            {
                foreach (Buildings building in AllBuildings)
                {
                    //if (building.Building_Sprite[building.AnimationSet].Sprite.Count > 1) { building.Building_Frame++; }
                    //if (building.Building_Frame == building.Building_Sprite[building.AnimationSet].Sprite.Count) { building.Building_Frame = 0; }

                }
            }
        }
        private void FrameUnit(int Timer)
        {
            if (Timer == TimerLimits)
            {
                foreach (Armies army in Armies)
                {
                    if (army.Unit.Unit_Sprite[army.Unit.AnimationSet].Sprite.Count > 1) { army.Unit.Unit_Frame++; }
                    if (army.Unit.Unit_Frame == army.Unit.Unit_Sprite[army.Unit.AnimationSet].Sprite.Count) { army.Unit.Unit_Frame = 0; }

                }
            }
        }


        void GameLoop()
        {
            OnLoad();
            while(/*GameLoopTask.Status.Equals(TaskStatus.Running)*/GameLoopThread.IsAlive)
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
            //TimeSpan ms1 = DateTime.Now.TimeOfDay;

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
                FrameTile(Timers);
                foreach (List<Levels> levelY in Levels.ToList())
                {
                    foreach (Levels levelX in levelY.ToList())
                    {
                        g.DrawImage(levelX.Level_Tile.Tile_Sprite[levelX.Level_Tile.AnimationSet].Sprite[levelX.Level_Tile.Tile_Frame].Sprite,
                            levelX.Level_Position.X,
                            levelX.Level_Position.Y,
                            levelX.Level_Scale.X,
                            levelX.Level_Scale.Y);
                    }
                }

                FrameBuilding(Timers);
                //foreach (Buildings building in AllBuildings.ToList())
                //{
                //    g.DrawImage(building.Building_Sprite[building.AnimationSet].Sprite[building.Building_Frame].Sprite,
                //    building.Building_Position.X,
                //    building.Building_Position.Y,
                //    building.Building_Scale.X,
                //    building.Building_Scale.Y);
                //}

                FrameUnit(Timers);
                foreach (Armies army in Armies.ToList())
                {
                    //Unit
                    g.DrawImage(army.Unit.Unit_Sprite[army.Unit.AnimationSet].Sprite[army.Unit.Unit_Frame].Sprite,
                        army.Unit.Position.X,
                        army.Unit.Position.Y,
                        army.Unit.vScale.X,
                        army.Unit.vScale.Y);
                    //Unit Effect (Health, Ammo, Fuel)
                    g.DrawImage(army.Unit.Health_Effect.EffectElement.Sprite[army.Unit.GetUnitHeath()].Sprite,
                        army.Unit.GetUnitHealthEffect(army.Unit.Position).X,
                        army.Unit.GetUnitHealthEffect(army.Unit.Position).Y,
                        army.Unit.Health_Effect.Scale.X,
                        army.Unit.Health_Effect.Scale.Y);
                    g.DrawImage(army.Unit.Supply_Effect.EffectElement.Sprite[army.Unit.GetUnitSupply()].Sprite,
                        army.Unit.GetUnitSupplyEffect(army.Unit.Position).X,
                        army.Unit.GetUnitSupplyEffect(army.Unit.Position).Y,
                        army.Unit.Supply_Effect.Scale.X,
                        army.Unit.Supply_Effect.Scale.Y);
                }
            }

            Timers++;
            if (Timers > TimerLimits) { Timers = 0; }

            //TimeSpan ms2 = DateTime.Now.TimeOfDay;
            //long ticks = ms2.Ticks - ms1.Ticks;

            //g.DrawString(ticks.ToString(), new Font("Arial", 16), new SolidBrush(Color.White), new PointF(0f, 0f));
        }



        public abstract void OnLoad();
        public abstract void OnUpdate();
        public abstract void OnDraw();
        public abstract void GetKeyDown(KeyEventArgs e);
        public abstract void GetKeyUp(KeyEventArgs e);
        public abstract void GetMouseDown(MouseEventArgs e);
        public abstract void GetMouseUp(MouseEventArgs e);
        public abstract void GetMouseWheel(MouseEventArgs e);
    }
}
