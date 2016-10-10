using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace WindowsFormsApplication1
{
    public partial class Xpilot : Form
    {
        List<Entity> elist;     // entities in this world
        Map map;                // Wall list (intended for temporary use to read from file)
        Entity myship;          // My ship
        Timer timer;
        Random rnd = new Random();

        /// <summary>
        /// Initialize this "Form", init elist, and start 10ms timer
        /// </summary>
        public Xpilot()
        {
            InitializeComponent();

            elist = new List<Entity>();
            System.Threading.Thread.Sleep(1000);
            elist.Add(new Entity());
            System.Threading.Thread.Sleep(1000);
            elist.Add(new Entity());
            System.Threading.Thread.Sleep(1000);
            elist.Add(new Entity());
            System.Threading.Thread.Sleep(1000);
            elist.Add(new Entity());
            System.Threading.Thread.Sleep(1000);
            elist.Add(new Entity());
            System.Threading.Thread.Sleep(1000);
            elist.Add(new Entity());

            // とりあえず自家製のマップを作成する。
            map = new Map();
            map.MapReadFile(); // <- リードしているっぽい名前だが、リードしていないです

            // マップから壁を取り出して、Form1 におけるすべてのモノリスト = elst に追加
            foreach (Entity e in map)
            {
                elist.Add(e);
            }
            map = null;                 // maplist を捨てる。

            // First ship is my ship
            myship = elist[0];

            // Tick count timer at 10ms
            timer = new Timer();
            timer.Interval = 10;
            timer.Tick += new EventHandler(myTick);
            timer.Start();
        }

        /// <summary>
        /// Display all the entities on the Form1
        /// </summary>
        private void show()
        {
            // おまじない
            Bitmap canvas = new Bitmap(pic.Width, pic.Height);
            Graphics g = Graphics.FromImage(canvas);

            // Use an arrow (→) for a ship
            Pen penShip = new Pen(Color.Black, 5);
            penShip.EndCap = System.Drawing.Drawing2D.LineCap.ArrowAnchor;
            // Rock(Wall) is Blue□, crash is red ○, bullet is black .
            Pen penRock =    new Pen(Color.Blue, 1);
            Pen penRockRec = new Pen(Color.Red, 1);  // Recognized rock
            Pen penCrash =   new Pen(Color.Red, 1);
            Pen penBullet = new Pen(Color.Red, 1);

#if false
            // 回転
            g.ResetTransform();
            g.TranslateTransform((float)myship.xpos, (float)myship.ypos);
            g.RotateTransform( (float) ((-myship.head_theta / Math.PI) * 180) - 90 /* look up */);
            g.TranslateTransform(-(float)myship.xpos, -(float)myship.ypos);
#endif

            // Draw all the entities
            foreach (Entity e in elist)
            {
                if (e is Wall)
                {
                    g.DrawRectangle(
                        e.isRecognized ? penRockRec : penRock, 
                        (int)e.xpos, (int)e.ypos, 5 /* w */, 5 /* h */);
                }
                else if (e is Bullet)
                {
                    g.DrawEllipse(penBullet, (int)e.xpos, (int)e.ypos, 2 /* w */, 2 /* h */);
                }
                else
                {
                    // REVISIT other entities are ship...
                    if (e.bang > 0) // bang!
                    {
                        int r = rnd.Next(10) + 5;   /* Random size ● */
                        g.DrawEllipse(penCrash, (int)e.xpos, (int)e.ypos, r, r);
                    }
                    else {          // cruising ship is "→"
                        g.DrawLine(penShip, 
                            (float)e.xpos,
                            (float)e.ypos,
                            (float)(e.xpos + 10 * Math.Cos(e.head_theta)),
                            (float)(e.ypos + 10 * Math.Sin(e.head_theta))
                            );
                    }
                }
            }

//          Font fnt = new Font("MS UI Gothic", 10);
//          fnt.Dispose();
            g.Dispose();

            // Display canvas on "pic"
            pic.Image = canvas;
        }

        /// <summary>
        /// Process User Input (Keyboard)
        /// </summary>
        private void processUserInput()
        {
            if (Keyboard.IsKeyDown(Key.A))
            {
                myship.turnLeft();                  // ←
            }
            if (Keyboard.IsKeyDown(Key.S))
            {
                myship.turnRight();                 // →
            }
            if (Keyboard.IsKeyDown(Key.Space))
            {
                myship.throttle();                  // Burst
            }
            if (Keyboard.IsKeyDown(Key.Z))
            {
                Bullet bullet = new Bullet(myship); // Create a bullet that position and verocity are the same as myship.
                elist.Add(bullet);
            }

        }

        /// <summary>
        /// Move entities in elist
        /// </summary>
        /// <param name="sender">Event sender (not used)</param>
        /// <param name="e">Event (not used)</param>
        private void myTick(object sender, EventArgs e)
        {
            // Clear recognitized bits
            foreach (Entity le in elist)
            {
                le.clear_isRecognized();
            }

            // Key input for myship
            processUserInput();

            // Automatic movement for non-myship
            foreach (Entity le in elist)
            {
                if (le != myship)           // REVISIT Ship class should be impremented.
                {
                    le.automove(elist);
                }
            }

            // Good-bye to entities that should vanish
            for (int i = elist.Count - 1; i >= 0; i--)
            {
                if (elist[i].vanish)
                {
                    elist.Remove(elist[i]);
                }
            }

            // Update velocity by gravity
            foreach (Entity le in elist)
            {
                le.attract(elist);
            }

            // Update velocity by emission
            foreach (Entity le in elist)
            {
                le.move();
            }

            // Update position
            foreach (Entity le in elist)
            {
                le.tick();
            }

            // Check if it crashes with other entity
            foreach (Entity le in elist)
            {
                le.hit(elist);
            }

            // Display entities in GUI
            show();
        }

        //
        // Button Events for this Form client
        //
        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
