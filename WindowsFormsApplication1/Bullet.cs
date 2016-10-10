using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication1
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
        public override void automove(List<Entity> elist)
        {
            ;   // do nothing;
        }
    }
}
