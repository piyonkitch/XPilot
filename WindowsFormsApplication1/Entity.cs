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

namespace Xpilot
{
    [Serializable]
    class Entity
    {
        // 質量 (setter は private でよさそうな気がする)
        public double m;
        // 位置 (setter は private でよさそうな気がする)
        public double xpos;
        public double ypos;
        // 速度 (setter は private でよさそうな気がする)
        public double xvel;
        public double yvel;

        // 頭の方向 (右が 0、左回り、１回転すると 2*PI)
        public double head_theta;
        // 噴射量 (setter は private でよさそうな気がする)
        protected double emit;

        // このモノに関する動作フラグ (setter は private でよさそうな気がする)
        public int freeze;     // ticks to rebirth
        public int bang;       // banging
        public bool vanish;    // please release from this world

        // mark that I am recognized by other entity
        public bool isRecognized;

        // parameters to learn
        public string name = "ship";
        public string nickname = "ship";
        const int num_hist = 10;
        public int[] survived_ticks = new int[num_hist]; // array to store how long this ship survived.
        public int gunTemp = 0;
        public int gunHeater = 20;
        
        private Random rnd = new Random();

        // Init (xpos, ypos)
        public Entity()
        {
            xpos = rnd.Next(XPilot.Constants.WorldSizeX - 50) + 25;
            ypos = rnd.Next(XPilot.Constants.WorldSizeX - 50) + 25;
            head_theta = 0;
            xvel = 0;
            yvel = 0;
            m = 0.1;
            emit = 0;
            freeze = 0;
            bang = 0;
            // survived (shift right)
            for (int i = num_hist-2; 0 <= i; i--)
            {
                survived_ticks[i+1] = survived_ticks[i];
            }
            survived_ticks[0] = 0;
        }

//        private void Relocate()
//        {
//            xpos = rnd.Next(XPilot.Constants.WorldSizeX - 50) + 25;
//            ypos = rnd.Next(XPilot.Constants.WorldSizeX - 50) + 25;
//            head_theta = 0;
//            xvel = 0;
//            yvel = 0;
//            emit = 0;
//        }

        // pos += vel
        public virtual void tick()
        {
            xpos += xvel;
            ypos += yvel;
        }

        // vel += emit
        public virtual void move()
        {
            xvel += Math.Cos(head_theta) * emit;
            yvel += Math.Sin(head_theta) * emit;
            emit -= 0.1;
            if (emit < 0)
            {
                emit = 0;
            }
        }

        // delta vel = sum of attraction by each entity
        public virtual void attract(List<Entity> elist)
        {
            foreach (Entity e in elist)
            {
                double r;       // range
                double a;       // absolute accel
                double theta;
                // a_x, a_y = a * theta
                double xacc = 0;
                double yacc = 0;

                if (e == this)
                {
                    continue;
                }

                r = distance(e, this);                
                if (r >= 1)     // ignore gravity if r < 1 as force becomes infinite.
                {
                    theta = Math.Atan2(e.ypos - this.ypos, e.xpos - this.xpos);
                    a = e.m / Math.Pow(r, 2);

                    xacc = a * Math.Cos(theta);
                    yacc = a * Math.Sin(theta);
                }
                // vel += acc
                xvel += xacc;
                yvel += yacc;
            }
        }

        public virtual bool hit(List<Entity> elist)
        {
            foreach (Entity e in elist)
            {
                // Entity does not hit to itself.
                if (e == this)
                {
                    continue;
                }

                // During banging, Entity does not start to bang again.
                if (this.bang > 0)
                {
                    continue;
                }

                if (distance(e, this) < 3)
                {
                    this.bang = 100;    // Bang for 100 ticks
                    return true;
                }
            }

            return false;   // no hit
        }

        // →
        public void turnRight()
        {
            head_theta += 0.1;
        }

        // ←
        public void turnLeft()
        {
            head_theta -= 0.1;
        }

        // Throttle engine
        public void throttle()
        {
            emit += .1;
            if (emit > 1)
            {
                emit = 1;
            }
        }

        public virtual void automove(List<Entity> elist, List<Entity> out_elist)
        {
            ;   // do nothing
        }

        // Rec related thing
        public void clear_isRecognized()
        {
            this.isRecognized = false;
        }

        //
        // Misc.
        //
        protected Entity findNearestWall(List<Entity> elist)
        {
            Entity near_wall = null;
            double dist_min = 99999;
            double dist = 0;

            foreach (Entity e in elist)
            {
                if (e is Wall)
                {
                    dist = distance(e, this);
                    if (dist < dist_min)
                    {
                        near_wall = e;
                        dist_min = dist;
                    }
                }
            }

            return near_wall;
        }

        protected double distance(Entity e1, Entity e2)
        {
            return Math.Sqrt(Math.Pow((e1.xpos - e2.xpos), 2) 
                             +
                             Math.Pow((e1.ypos - e2.ypos), 2)
                   );
        }

    }
}
