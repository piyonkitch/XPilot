/*
Copyright(c) 2016, piyonkitch<kazuo.horikawa.ko@gmail.com>
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
    class Wall : Entity
    {
        Random rnd = new Random();

        public Wall()
        {
            xpos = rnd.Next(100);   // REVISIT SIZE
            ypos = rnd.Next(100);   // REVISIT SIZE
            m = 0.01;
            name = "wall";
        }

        // Constructor with (x, y)
        public Wall(double x, double y)
        {
            xpos = x;
            ypos = y;
            head_theta = 0;
            m = 1;
        }

        // Wall will never move.
        public override void tick()
        {
            ;
        }

        // Wall will never move.
        public override void move()
        {
            ;
        }

        // Wall will never move.
        public override void attract(List<Entity> elist)
        {
            ;
        }

        // Wall will not think anything
        public override void automove(List<Entity> elist, List<Entity> out_elist)
        {
            ;
        }
    }
}