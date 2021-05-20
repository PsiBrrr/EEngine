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
        private static List<List<Level>> Levels = new List<List<Level>>();
        private static List<Effects> AllEffects = new List<Effects>();

        private static int Timers = 0;
        private static int TimerLimits = 20;
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
        public static void CreateArmyUnit(Vector2 Position, Vector2 Scale, int Index, Vector2 Level_Array_Position)
        {
            new Armies(Position, Scale, GetUnit(Index), EEngine.GetEffect(0), Level_Array_Position);
        }
        /*public static void DeleteArmyUnit()
        {
            Armies.RemoveAt(0);
            Log.Normal("Unit Removed");
        }*/


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
        public static void InitializeLevel(int Row_Y)
        {
            for(int y = 0; y < Row_Y; y++)
            {
                Levels.Add(new List<Level>());
            }
        }
        public static void RegisterLevel(Level Level)
        {
            Levels[(int)Level.Level_Array_Position.Y].Add(Level);
        }
        public static void UnRegisterLevel(Level Level)
        {
            Levels[(int)Level.Level_Array_Position.Y].Remove(Level);
        }
        public static Vector2 GetLevelTile(Vector2 Position)
        {
            Vector2 Level_Array_Position = Vector2.Zero();

            foreach (List<Level> levelY in Levels.ToList())
            {
                foreach (Level levelX in levelY.ToList())
                {
                    if (Position.X.IsBetween(levelX.Level_Position.X, (levelX.Level_Position.X + levelX.Level_Scale.X)) 
                        && Position.Y.IsBetween(levelX.Level_Position.Y, (levelX.Level_Position.Y + levelX.Level_Scale.Y)))
                    { Level_Array_Position = levelX.Level_Array_Position; }
                }
            }

            return Level_Array_Position;
        }
        //public static void SetLevelTileFog(Vector2 Position, int Vision)
        //{
        //    Vector2 Level_Array_Position = new Vector2(0, 4);//GetLevelTile(Position);
        //    int VisionSize = ((Vision * 2) - 1);

        //    for (int i = 0; i < Vision; i++)
        //    {
        //        for (int j = (i * -1); j <= i; j++)
        //        {
        //            if (!((int)Level_Array_Position.Y + j).IsBetween(0, (Levels.Count - 1))
        //                || !((int)Level_Array_Position.X + i).IsBetween(0, (Levels[(int)Level_Array_Position.Y + j].Count - 1)))
        //            { continue; }
        //            Levels[(int)Level_Array_Position.Y + j][(int)Level_Array_Position.X + i].Level_Tile.Normal();

        //            if (!((int)Level_Array_Position.X + ((VisionSize - 1) - i)).IsBetween(0, (Levels[(int)Level_Array_Position.Y + j].Count - 1))) { continue; }
        //            Levels[(int)Level_Array_Position.Y + j][(int)Level_Array_Position.X + ((VisionSize - 1) - i)].Level_Tile.Normal();
        //        }
        //    }
        //}
        public static void SetLevelTileFog(Vector2 Level_Array_Position, int Vision)
        {
            Level_Array_Position.X -= Vision - 1;
            int VisionSize = ((Vision * 2) - 1);

            for (int i = 0; i < Vision; i++)
            {
                for (int j = (i * -1); j <= i; j++)
                {
                    if (!((int)Level_Array_Position.Y + j).IsBetween(0, (Levels.Count - 1))
                        || !((int)Level_Array_Position.X + i).IsBetween(0, (Levels[(int)Level_Array_Position.Y + j].Count - 1)))
                    { continue; }
                    Levels[(int)Level_Array_Position.Y + j][(int)Level_Array_Position.X + i].Level_Tile.Normal();

                    if (!((int)Level_Array_Position.X + ((VisionSize - 1) - i)).IsBetween(0, (Levels[(int)Level_Array_Position.Y + j].Count - 1)))
                    { continue; }
                    Levels[(int)Level_Array_Position.Y + j][(int)Level_Array_Position.X + ((VisionSize - 1) - i)].Level_Tile.Normal();
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
                foreach (List<Level> levelY in Levels.ToList())
                {
                    foreach (Level levelX in levelY)
                    {
                        if (levelX.Level_Tile.Tile_Sprite[levelX.Level_Tile.AnimationSet].Sprite.Count > 1) { levelX.Level_Tile.Tile_Frame++; }
                        if (levelX.Level_Tile.Tile_Frame == levelX.Level_Tile.Tile_Sprite[levelX.Level_Tile.AnimationSet].Sprite.Count) { levelX.Level_Tile.Tile_Frame = 0; }
                    }
                }
                //foreach (Level level in Levels)
                //{
                //    if (level.Tile.Tile_Sprite[0].Sprite.Count > 1) { level.Tile.Tile_Frame++; }
                //    if (level.Tile.Tile_Frame == level.Tile.Tile_Sprite[0].Sprite.Count) { level.Tile.Tile_Frame = 0; }
                //}
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
                foreach(List<Level> levelY in Levels.ToList())
                {
                    foreach(Level levelX in levelY.ToList())
                    {
                        g.DrawImage(levelX.Level_Tile.Tile_Sprite[levelX.Level_Tile.AnimationSet].Sprite[levelX.Level_Tile.Tile_Frame].Sprite,
                            levelX.Level_Position.X,
                            levelX.Level_Position.Y,
                            levelX.Level_Scale.X,
                            levelX.Level_Scale.Y);
                    }
                }
                //foreach (Level level in Levels)
                //{
                //    g.DrawImage(level.Tile.Tile_Sprite[level.Tile.AnimationSet].Sprite[level.Tile.Tile_Frame].Sprite,
                //        level.Position.X,
                //        level.Position.Y,
                //        level.Scale.X,
                //        level.Scale.Y);
                //}

                FrameUnit(Timers);
                foreach (Armies army in Armies.ToList())
                {
                    g.DrawImage(army.Unit.Unit_Sprite[army.Unit.AnimationSet].Sprite[army.Unit.Unit_Frame].Sprite,
                        army.Unit.Unit_Position.X,
                        army.Unit.Unit_Position.Y,
                        army.Unit.Unit_Scale.X,
                        army.Unit.Unit_Scale.Y);
                    g.DrawImage(army.Unit.Health_Effect.EffectElement.Sprite[army.Unit.GetUnitHeath_Rounded()].Sprite,
                        army.Unit.GetUnitHealthEffect(army.Unit.Unit_Position).X,
                        army.Unit.GetUnitHealthEffect(army.Unit.Unit_Position).Y,
                        army.Unit.Health_Effect.Scale.X,
                        army.Unit.Health_Effect.Scale.Y);
                }
            }

            Timers++;
            if (Timers > TimerLimits) { Timers = 0; }
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
