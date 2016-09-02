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

        // 速度分を座標に追加
        public override void tick()
        {
            xpos += xvel;
            ypos += yvel;
        }

        // emit を速度に反映
        public override void move()
        {
            // 誰かに当たったら消滅する
            if (this.bang > 0)
            {
                this.vanish = true; // このオブジェクトを管理している人に、この世界から開放してもらう。
                this.bang = 0;
            }

            // スロットル
            xvel += Math.Cos(head_theta) * emit;
            yvel += Math.Sin(head_theta) * emit;
            emit -= 0.1;
            if (emit < 0)
            {
                emit = 0;
            }
        }
    }
}
