/*
Copyright(c) 2016, piyonkitch <kazuo.horikawa.ko@gmail.com>
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:

* Redistributions of source code must retain the above copyright notice, this
 list of conditions and the following disclaimer.

* Redistributions in binary form must reproduce the above copyright notice,
 this list of conditions and the following disclaimer in the documentation
  and/or other materials provided with the distribution.

* Neither the name of roguelike nor the names of its
 contributors may be used to endorse or promote products derived from
  this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED.IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
OR TORT(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Input;

namespace Xpilot
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
            elist.Add(new Ship("自"));
            System.Threading.Thread.Sleep(1000);
            elist.Add(new Ship("イ", 70, 2.0));
            System.Threading.Thread.Sleep(1000);
            elist.Add(new Ship("ロ", 60, 1.8));
            System.Threading.Thread.Sleep(1000);
            elist.Add(new Ship("ハ", 70, 2.5));

            // homemade map
            map = new Map();
            map.MapReadFile();          // Map does not read from file; it returns a Wall to foreach.

            // Retrieve each Wall from map and add it to elist, entity list in this world.
            foreach (Entity e in map)
            {
                elist.Add(e);
            }
            map = null;                 // Discard map

            // First ship is my ship
            myship = elist[0];

            // Tick count timer at 10ms
            timer = new Timer();
            timer.Interval = 10;
            timer.Tick += new EventHandler(myTick);
            timer.Start();
        }

        /// <summary>
        /// Display all the entities on the Form1.pic
        /// </summary>
        private void show()
        {
            // Bitmap canvas where this application draws lines.
            Bitmap canvas = new Bitmap(pic.Width, pic.Height);
            Graphics g = Graphics.FromImage(canvas);

            // Use an arrow (→) for a ship
            Pen penShip = new Pen(Color.White, 5);
            penShip.EndCap = System.Drawing.Drawing2D.LineCap.ArrowAnchor;
            // Rock(Wall) is Blue□, crash is red ○, bullet is white.
            Pen penRock =    new Pen(Color.Blue, 1);
            Pen penRockRec = new Pen(Color.Red, 1);  // Recognized rock
            Pen penCrash =   new Pen(Color.Red, 1);
            Pen penBullet = new Pen(Color.White, 1);
            Font fnt = new Font("Arial", 8);
            Pen penLine = new Pen(Color.White, 1);

#if false
            // ぐりぐり回転はうまくいっていない
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
                        (int)e.xpos, (int)e.ypos, 4 /* w */, 4 /* h */);
                }
                else if (e is Bullet)
                {
                    g.DrawEllipse(penBullet, (int)e.xpos, (int)e.ypos, 3 /* w */, 3 /* h */);
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
                        g.DrawString(e.name, fnt, Brushes.White, 
                            (float)e.xpos+8, (float)e.ypos+8);
                    }
                }
            }

            // I heard that these Dispose decrease GC time ...
            penShip.Dispose();
            penRock.Dispose();
            penRockRec.Dispose();
            penCrash.Dispose();
            penBullet.Dispose();
            fnt.Dispose();
            penLine.Dispose();
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
                if (myship.gunTemp <= 0)
                {
                    Bullet bullet = new Bullet(myship); // Create a bullet that position and verocity are the same as myship.
                    elist.Add(bullet);
                    myship.gunTemp += myship.gunHeater;
                }
            }

        }

        /// <summary>
        /// Move entities in elist
        /// </summary>
        /// <param name="sender">Event sender (not used)</param>
        /// <param name="e">Event (not used)</param>
        private void myTick(object sender, EventArgs e)
        {
            List<Entity> out_elist = new List<Entity>(); // entities to be added (bullet)

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
                    le.automove(elist, out_elist);
                }
            }
            elist.AddRange(out_elist);

            // Good-bye to entities that should vanish (not for ship, but for bullet)
            for (int i = elist.Count - 1; i >= 0; i--)
            {
                if (elist[i].vanish)
                {
                    elist.Remove(elist[i]);
                }
            }

            //
            // Learning hook
            //
            foreach (Entity le in elist)
            {
                if (le is Ship)
                {
                    int valid_count = 0;
                    int ticks_survived_sum = 0;
                    if (le.freeze == 1) {
                        for (int i = 1; i < 10; i++)
                        {
                            if (le.survived_ticks[i] != 0)
                            {
                                ticks_survived_sum += le.survived_ticks[i];
                                valid_count++;
                            }
                        }

                        if (valid_count > 0)
                        {
                            Console.WriteLine(le.name + "survived ticks = " + (ticks_survived_sum / valid_count)); 
                        }
                    }
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

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
