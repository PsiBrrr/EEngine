using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using EEngine.EEngine;
using System.Windows.Forms;
using System.Xml;
using System.IO;

namespace EEngine
{
    class DemoGame : EEngine.EEngine
    {
        Image tileSpriteSheet;
        Image playerSpriteSheet;
        Image effectSpriteSheet;
        readonly float Scale = 4f;
        readonly float Speed = 4f;

        bool Up;
        bool Down;
        bool Left;
        bool Right;

        bool tab;
        bool click;
        bool num1;
        bool num2;
        bool num3;
        //bool num4;
        //bool num5;
        //bool num6;

        readonly XmlDocument tileDoc = new XmlDocument();
        readonly XmlDocument buildingDoc = new XmlDocument();
        readonly XmlDocument unitDoc = new XmlDocument();
        readonly XmlDocument effectDoc = new XmlDocument();

        Armies SelectedUnit;
        Vector2 TargetPosition;

        //Vector2 lastPos = Vector2.Zero();
        string[,] Map =
        {
            {"tb", "tb", "tb", "tb", "tb", "tb", "tb" },
            {"tb", "gls", "gl", "gl", "gl", "gl", "gl" },
            {"tb", "gls", "gl", "gl", "gl", "gl", "gl" },
            {"tb", "gls", "gl", "gl", "gl", "gl", "gl" },
            {"tb", "gls", "gl", "gl", "gl", "gl", "gl" },
            {"tb", "gls", "gl", "gl", "ocluc", "ocu", "ocu" },
            {"tb", "gls", "gl", "gl", "ocl", "o", "o" },
            {"tb", "tb", "tb", "tr", "ocl", "o", "o" }
        };
        public DemoGame() : base(new Vector2(800, 600), "Engine Stream Demo") { }

        public override void OnLoad()
        {
            BackgroundColor = Color.Black;

            try
            {
                tileSpriteSheet = Image.FromFile("Assets/Sprites/Tiles/Advance Wars 2 - Modified Tiles.png");
                playerSpriteSheet = Image.FromFile("Assets/Sprites/Units/Advance Wars 2 - Modified.png");
                effectSpriteSheet = Image.FromFile("Assets/Sprites/Effects/Advance Wars 2 - Modified Effects.png");
                tileDoc.Load("Assets/Sprites/Tiles/Tiles.xml");
                buildingDoc.Load("Assets/Sprites/Tiles/Structures.xml");
                unitDoc.Load("Assets/Sprites/Units/Units.xml"); 
                effectDoc.Load("Assets/Sprites/Effects/Effects.xml");
            }
            catch (ArgumentException ex)
            {
                Log.Error(ex.Message);
            }
            catch (FileNotFoundException ex)
            {
                Log.Error(ex.Message);
            }
            catch (XmlException ex)
            {
                Log.Error(ex.Message);
            }

            new Tiles(tileSpriteSheet, tileDoc);
            new Buildings(tileSpriteSheet, buildingDoc);
            new Units(playerSpriteSheet, unitDoc);
            new Effects(effectSpriteSheet, effectDoc);


            CreateMap(Scale, Map);
            //SetLevelTileFog();

            CreateBuilding(new Vector2(230, 344), Scale, 0);

            //CreateArmyUnit(new Vector2(300, 200), new Vector2(48, 48), 1, new Vector2(0, 0)); //Manual scale set of army unit
            CreateArmyUnit(new Vector2(294, 216), Scale, 0, new Vector2(0, 0));
            CreateArmyUnit(new Vector2(358, 216), Scale, 1, new Vector2(0, 0));
            CreateArmyUnit(new Vector2(422, 216), Scale, 2, new Vector2(0, 0));
            CreateArmyUnit(new Vector2(486, 216), Scale, 3, new Vector2(0, 0));

            GetArmyUnit(0).Unit.SetUnitHealth(49);
            SelectedUnit = null;
            TargetPosition = Vector2.Zero();
        }

        int i = 0;
        float x = 0;
        float y = 0;
        public override void OnDraw()
        {
            if (num1 && i == 0)
            {
                Random random = new Random();
                x = random.Next(0, 800);
                y = random.Next(0, 600);
                CreateArmyUnit(new Vector2(x, y), Scale, 5, new Vector2(0, 0));
                i++;
            }
            if(num2 && i == 0)
            {
                SetMapTileFogVision(new Vector2(3, 3), 3);

                i++;
            }
            if(num3 && i ==0)
            {
                if(SelectedUnit != null)
                {
                    SetArmyUnit(SelectedUnit.Unit, false);
                }

                i++;
            }
            if(tab && i == 0)
            {
                //DeleteArmyUnit();
                i++;
            }


        }

        public override void OnUpdate()
        {
            try
            {
                if (click) { click = SelectedUnit.ArmyUnitMoveY(TargetPosition, Speed); }

                if (Up)
                {
                    SetCameraPositionY(-Speed);
                }
                if (Down)
                {
                    SetCameraPositionY(Speed);
                }
                if (Left)
                {
                    SetCameraPositionX(-Speed);
                }
                if (Right)
                {
                    SetCameraPositionX(Speed);
                }

                //if(player.IsColliding("Wall"))
                //{
                //    //Log.Info($"Colliding! {lastPos.X}");
                //    //times++;
                //    player.Position.X = lastPos.X;
                //    player.Position.Y = lastPos.Y;
                //}
                //else
                //{ 
                //    lastPos.X = player.Position.X;
                //    lastPos.Y = player.Position.Y;
                //}

                if (i != 0) { i++; Log.Normal(i); }
                if (i >= 60) { i = 0; }
            }
            catch(NullReferenceException ex)
            {
                Log.Error(ex.Message);
            }
        }

        public override void GetKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.W) { Up = true; }
            if (e.KeyCode == Keys.S) { Down = true; }
            if (e.KeyCode == Keys.A) { Left = true; }
            if (e.KeyCode == Keys.D) { Right = true; }

            if (e.KeyCode == Keys.Tab) { tab = true; }
            if (e.KeyCode == Keys.NumPad1) { num1 = true; }
            if (e.KeyCode == Keys.NumPad2) { num2 = true; }
            if (e.KeyCode == Keys.NumPad3) { num3 = true; }
        }
        public override void GetKeyUp(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.W) { Up = false; }
            if (e.KeyCode == Keys.S) { Down = false; }
            if (e.KeyCode == Keys.A) { Left = false; }
            if (e.KeyCode == Keys.D) { Right = false; }

            if (e.KeyCode == Keys.Tab) { tab = false; }
            if (e.KeyCode == Keys.NumPad1) { num1 = false; }
            if (e.KeyCode == Keys.NumPad2) { num2 = false; }
            if (e.KeyCode == Keys.NumPad3) { num3 = false; }
        }

        public override void GetMouseDown(MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Left)
            {
                Log.Normal($"Mouse Left Down at {e.Location}");

                Armies TempUnit = GetArmyUnit(new Vector2(e.Location.X, e.Location.Y));
                Map TempMap = GetMapItem(new Vector2(e.Location.X, e.Location.Y));
                
                if(TempMap != null)
                {
                    TargetPosition = TempMap.Position;
                    Log.Info2(TargetPosition);
                    if (SelectedUnit != null) { if (SelectedUnit.Unit.Active) { click = true; } }
                }

                if (TempUnit != null && TempUnit.Unit.Active)
                {
                    SelectedUnit = TempUnit;
                    Log.Info2(SelectedUnit.Unit.Tag);
                }
            }
        }
        public override void GetMouseUp(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Log.Normal("Mouse Left Up");

            }
        }
        public override void GetMouseWheel(MouseEventArgs e)
        {

        }
        public override void GetMouseMove(MouseEventArgs e)
        {

        }
    } 
}
