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

        bool left;
        bool right;
        bool up;
        bool down;
        bool tab;

        bool num1;
        bool num2;
        bool num3;
        bool num4;
        bool num5;
        bool num6;

        readonly XmlDocument tileDoc = new XmlDocument();
        readonly XmlDocument unitDoc = new XmlDocument();
        readonly XmlDocument effectDoc = new XmlDocument();

        Vector2 lastPos = Vector2.Zero();
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
            new Units(playerSpriteSheet, unitDoc);
            new Effects(effectSpriteSheet, effectDoc);
            new Level(new Vector2(32, 32), Map);

            CreateArmyUnit(new Vector2(300, 200), new Vector2(48, 48), 0, new Vector2(0, 0));
            CreateArmyUnit(new Vector2(332, 200), new Vector2(48, 48), 1, new Vector2(0, 0));
            CreateArmyUnit(new Vector2(364, 200), new Vector2(48, 48), 2, new Vector2(0, 0));
            CreateArmyUnit(new Vector2(396, 200), new Vector2(48, 48), 3, new Vector2(0, 0));

            GetArmyUnit(0).Unit.Health = 49;
            GetArmyUnit(1).Unit.Health = 4;
            GetArmyUnit(3).Unit.Health = 98;
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
                CreateArmyUnit(new Vector2(x, y), new Vector2(48, 48), 4, new Vector2(0, 0));
                i++;
            }
            if(num2 && i == 0)
            {
                SetLevelTileFog(new Vector2(6, 7), 3);
                i++;
            }
            if(tab && i == 0)
            {
                //DeleteArmyUnit();
                i++;
            }


        }

        float speed = 2f;
        public override void OnUpdate()
        {
            GetArmyUnit(0).Unit.Idle();
            if (up)
            {
                GetArmyUnit(0).Unit.Up();
                GetArmyUnit(0).Position.Y -= speed;
                //Log.Normal(playerRun.Position);
            }

            if (down)
            {
                GetArmyUnit(0).Unit.Down();
                GetArmyUnit(0).Position.Y += speed;
                //Log.Normal(playerRun.Position);
            }
            if (left)
            {
                GetArmyUnit(0).Unit.Left();
                GetArmyUnit(0).Position.X -= speed;
                //Log.Normal(playerRun.Position);
            }
            if (right)
            {
                GetArmyUnit(0).Unit.Right();
                GetArmyUnit(0).Position.X += speed;
                //Log.Normal(playerRun.Position);
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
            if (i >= 120) { i = 0; } 
        }

        public override void GetKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.W) { up = true; }
            if (e.KeyCode == Keys.S) { down = true; }
            if (e.KeyCode == Keys.A) { left = true; }
            if (e.KeyCode == Keys.D) { right = true; }
            if (e.KeyCode == Keys.Tab) { tab = true; }

            if (e.KeyCode == Keys.NumPad1) { num1 = true; }
            if (e.KeyCode == Keys.NumPad2) { num2 = true; }
        }

        public override void GetKeyUp(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.W) { up = false; }
            if (e.KeyCode == Keys.S) { down = false; }
            if (e.KeyCode == Keys.A) { left = false; }
            if (e.KeyCode == Keys.D) { right = false; }
            if (e.KeyCode == Keys.Tab) { tab = false; }

            if (e.KeyCode == Keys.NumPad1) { num1 = false; }
            if (e.KeyCode == Keys.NumPad2) { num2 = false; }
        }

        public override void GetMouseDown(MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Left) { Log.Normal($"Mouse Left Down at {e.Location}"); }
        }
        public override void GetMouseUp(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) { Log.Normal("Mouse Left Up"); }
        }
        public override void GetMouseWheel(MouseEventArgs e)
        {

        }
    } 
}
