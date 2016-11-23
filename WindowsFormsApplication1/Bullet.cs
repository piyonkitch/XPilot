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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xpilot
{
    [Serializable]
    class Bullet : Entity
    {
        private Random rnd = new Random();
        private double shoot_speed = 5;
        public int life     { get; private set; } = 30;
        public int life_max { get; private set; } = 30;

        // Bullet 発射時の処理（この世界に登場)
        public Bullet(Entity e)
        {
            this.xpos = e.xpos;
            this.ypos = e.ypos;
            this.head_theta = e.head_theta;
            this.xvel = e.xvel + shoot_speed * Math.Cos(head_theta);
            this.yvel = e.yvel + shoot_speed * Math.Sin(head_theta);
            this.m = 0.0;
            this.emit = 0.0;
            this.freeze = 0;    // .Net が０にしてくれるはずだが念のため初期化。
            this.bang = 0;      // .Net が０にしてくれるはずだが念のため初期化。
            this.vanish = false;  // .Net が０にしてくれるはずだが念のため初期化。
            this.name = "bullet";
        }

        // pos += vel
        public override void tick()
        {
            xpos += xvel;
            ypos += yvel;
        }

        // vel += emit 
        public override void move()
        {
            // if I hit somebody, ask game manager to remove me from this world.
            if (this.bang > 0)
            {
                this.vanish = true;
                this.bang = 0;
            }


            // count down my life
            if (this.life <= 0)
            {
                this.vanish = true;
                this.bang = 0;
            }
            this.life--;
        }

        // disable base class automove()
        public override void automove(List<Entity> elist, List<Entity> out_elist)
        {
            ;   // do nothing;
        }
    }
}
