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
    class Ship : Entity
    {
        // parameters to learn
        const int num_hist = 10;
        public double distWallDanger = 70;   // default
        public double speedTooFast = 2.0;    // default
        public double home_xpos;
        public double home_ypos;
        private Ship shipLeader = null;

        private Random rnd = new Random();

        // Init (xpos, ypos)
        public Ship()
        {
            home_xpos = rnd.Next(XPilot.Constants.WorldSizeX - 50) + 25;
            home_ypos = rnd.Next(XPilot.Constants.WorldSizeY - 50) + 25;
            xpos = home_xpos;
            ypos = home_ypos;
            head_theta = 0;

            xvel = 0;
            yvel = 0;

            m = 0.1;
            emit = 0;
            freeze = 0;
            bang = 0;
            gunTemp = 0;
            //
            // Learning bits
            //
            for (int i = num_hist-2; 0 <= i; i--)
            {
                survived_ticks[i+1] = survived_ticks[i];
            }
            survived_ticks[0] = 0;
        }

        public Ship(string nickName) : this()
        {
            // constructor w/o args is called
            name = nickName;
        }

        public Ship(string nickName, double dist, double speed) : this()
        {
            // constructor w/o args is called
            name = nickName;
            distWallDanger = dist;
            speedTooFast = speed;
        }

        public Ship(string nickName, double dist, double speed, Ship leader) : this(nickName, dist, speed)
        {
            shipLeader = leader;
        }

        private void Home()
        {
            xpos = home_xpos;
            ypos = home_ypos;
            Console.WriteLine("xpos:" + home_xpos + ", ypos:" + home_ypos);
            head_theta = 0;

            xvel = 0;
            yvel = 0;

            emit = 0;

            // survived (shift right)
            for (int i = num_hist - 2; 0 <= i; i--)
            {
                survived_ticks[i + 1] = survived_ticks[i];
            }
            survived_ticks[0] = 0;
        }

        // pos += vel
        public override void tick()
        {
            // If I am banging, do not move.
            if (bang > 0)
            {
                bang--;
                if (bang <= 0)
                {
                    freeze = 100;
                    Home();
                }
                return;
            }

            // Freeze is count down to rebirth
            if (freeze > 0)
            {
                freeze--;
                return;
            }

            xpos += xvel;
            ypos += yvel;

            survived_ticks[0]++;
            if (gunTemp > 0) {
                gunTemp--;
            }
        }

        enum Strategy
        {
            NO_PLAN,
            REDUCE_SPEED,
            AVOID_WALL,
            CHASE_SHIP,
            FOLLOW_LEADER,
            AVOID_LEADER,
        }

        // 自動運転
        public override void automove(List<Entity> elist, List<Entity> out_elist)
        {
            double theta_tobe = 0.0;
            double theta_diff;
            Entity w = null;
            Strategy strategy = Strategy.NO_PLAN;

            if (Math.Sqrt(Math.Pow(this.xvel, 2) + Math.Pow(this.yvel, 2))
                > speedTooFast)
            {
                // If I am flying too fast, reduce vel.
                theta_tobe = Math.Atan2(this.yvel, this.xvel) + Math.PI;
                if (theta_tobe > Math.PI)
                {
                    theta_tobe -= (2 * Math.PI);
                }
                strategy = Strategy.REDUCE_SPEED;
            }
            else
            {
                // Otherwise, avoid nearest wall.

                // Find nearest wall and head against it.
                w = findNearestWall(elist);
                if ((w != null) && (distance(this, w) < distWallDanger))
                {
                    // mark this wall is recognized by a ship
                    w.isRecognized = true;      

                    theta_tobe = Math.Atan2((this.ypos - w.ypos), (this.xpos - w.xpos));
                    strategy = Strategy.AVOID_WALL;
                }
                else if (shipLeader == null) 
                {
                    /* Chase myship */
                    theta_tobe = Math.Atan2((elist[0].ypos - this.ypos), (elist[0].xpos - this.xpos));
                    strategy = Strategy.CHASE_SHIP;
                }
                else
                {
                    if (distance(this, shipLeader) < 30)
                    {
                        theta_tobe = Math.Atan2((this.ypos - shipLeader.ypos), (this.xpos - shipLeader.xpos));
                        strategy = Strategy.AVOID_LEADER;
                    }
                    else if (distance(this, elist[0]) < 100)
                    {
                        /* Chase myship */
                        theta_tobe = Math.Atan2((elist[0].ypos - this.ypos), (elist[0].xpos - this.xpos));
                        strategy = Strategy.CHASE_SHIP;
                    }
                    else
                    {
                        /* Follow leader */
                        theta_tobe = Math.Atan2((shipLeader.ypos - this.ypos), (shipLeader.xpos - this.xpos));
                        strategy = Strategy.FOLLOW_LEADER;
                    }
                }
            }

            // Calculate theta_diff by (theta_tobe - head_theta)
            theta_diff = theta_tobe - head_theta;
            // be within -PI - PI
            if (theta_diff > Math.PI)
            {
                theta_diff -= (2 * Math.PI);
            }
            if (theta_diff < -Math.PI)
            {
                theta_diff += (2 * Math.PI);
            }
            if ( ! ((-Math.PI <= theta_diff) && (theta_diff <= Math.PI)) )
            {
                Console.WriteLine(theta_diff);
            }

            if ( 0.1 < theta_diff)
            {
                head_theta += 0.1;
            }
            if (theta_diff < -0.1)
            {
                head_theta -= 0.1;
            }
            // want to be within -PI - PI
            if (head_theta > Math.PI)
            {
                head_theta -= (2 * Math.PI);
            }
            if (head_theta < -Math.PI)
            {
                head_theta += (2 * Math.PI);
            }
            if (!((-Math.PI <= head_theta) && (head_theta <= Math.PI)))
            {
                Console.WriteLine(head_theta);
            }

            // emit if my head is CORRECT direction
            if ( ((theta_tobe - head_theta) > -(Math.PI / 4)) &&
                    ((theta_tobe - head_theta) <  (Math.PI / 4)) )
            {
                switch (strategy)
                {
                    case Strategy.REDUCE_SPEED:
                    case Strategy.FOLLOW_LEADER:
                        emit += .05;
                        break;
                    case Strategy.CHASE_SHIP:
                        emit += .05;
                        if (gunTemp <= 0)
                        {
                            // Create a bullet that position and verocity are the same as myship.
                            Bullet bullet = new Bullet(this);
                            // As elist cannot be modifed, out_elist will be added later. 
                            out_elist.Add(bullet);
                            // gun temperature goes up after shooting. 
                            gunTemp += gunHeater;              
                            Console.WriteLine(gunTemp);
                        }
                        break;
                    case Strategy.AVOID_WALL:
                        if (distance(this, w) < 30)
                        {
                            emit += .15;
                        }
                        else if (distance(this, w) < 50)
                        {
                            emit += .1;
                        }
                        break;
                    case Strategy.AVOID_LEADER:
                        if (distance(this, shipLeader) < 15)
                        {
                            emit += .15;
                        }
                        else if (distance(this, shipLeader) < 30)
                        {
                            emit += .1;
                        }
                        break;
                    default:
                        break;
                }

                // Do not burst too much.
                if (emit > 6)
                {
                    emit = 6;
                }
            }
            else
            {
                emit = 0;
            }
        }

        //
        // NOTE: findNearestWall() and distance() methods in Entity are used.
        //
    }
}
