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
    [Serializable()]
    class Map
    {
        List<Entity> wlist;

        // wlist holds Walls.
        public Map()
        {
            wlist = new List<Entity>();
        }

        // IEnumerator makes this object enumerable.
        public IEnumerator<Entity> GetEnumerator()
        {
            foreach (Entity e in wlist)
            {
                yield return e;
            }
        }

        // REVISIT dummy map
        public void MapReadFile()
        {
            // Upper walls
            for (int x = 0; x <= XPilot.Constants.WorldSizeX; x += 5)
            {
                wlist.Add(new Wall(x, 0));
            }
            // Lower walls
            for (int x = 0; x <= XPilot.Constants.WorldSizeX; x += 5)
            {
                wlist.Add(new Wall(x, XPilot.Constants.WorldSizeY));
            }
            // Leftmost walls
            for (int y = 0 + 5; y < XPilot.Constants.WorldSizeY; y += 5)
            {
                wlist.Add(new Wall(0, y));
            }
            // Righmost walls
            for (int y = 0 + 5; y < XPilot.Constants.WorldSizeY; y += 5)
            {
                wlist.Add(new Wall(XPilot.Constants.WorldSizeX, y));
            }

            // Maybe we want to have random walls, natural walls or ...
            for (int i = 160; i < 200; i += 5)
            {
                wlist.Add(new Wall(i, 50));
            }
            for (int i = 90; i < 120; i += 5)
            {
                wlist.Add(new Wall(200, i));
            }
        }
    }
}
