using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication1
{
    [Serializable]
    class Entity
    {
        public double xpos;
        public double ypos;
        public double m;            // 質量
        public double head_theta;   // 頭の向き 
        protected double emit;
        public int  freeze;         // 死んでから復活するまでの tick 数
        public int  bang;           // 爆発中
        public bool vanish;         // この世界から解放要

        // 速度
        public double xvel;
        public double yvel;
        private Random rnd = new Random();

        // 座標を初期化する
        public Entity()
        {
            xpos = rnd.Next(100);
            ypos = rnd.Next(100);
            head_theta = 0;
            xvel = 0;
            yvel = 0;
            m = 0.1;
            emit = 0;
            freeze = 0;
            bang = 0;
        }

        private void Relocate()
        {
            xpos = rnd.Next(100) + 50;
            ypos = rnd.Next(100) + 50;
            head_theta = 0;
            xvel = 0;
            yvel = 0;
            emit = 0;
        }

        // 速度分を座標に追加
        public virtual void tick()
        {
            // 爆発中
            if (bang > 0)
            {
                bang--;
                if (bang <= 0)
                {
                    freeze = 100;
                    Relocate();
                }
                return;
            }

            // 復活までのカウントダウン中は、フリーズ
            if (freeze > 0)
            {
                freeze--;
                return;
            }

            xpos += xvel;
            ypos += yvel;
        }

        // emit を速度に反映
        public virtual void move()
        {
            // スロットル
            xvel += Math.Cos(head_theta) * emit;
            yvel += Math.Sin(head_theta) * emit;
            emit -= 0.1;
            if (emit < 0)
            {
                emit = 0;
            }
        }

        // 各物体間の引力から速度を更新
        public virtual void attract(List<Entity> elist)
        {
            foreach (Entity e in elist)
            {
                double r;       // 距離
                double a;       // 加速度（絶対値）
                double theta;   // 方向
                // 加速度 (x,y) を、a と theta から求めたもの
                double xacc = 0;
                double yacc = 0;

                if (e == this)
                {
                    continue;
                }

                r = Math.Sqrt(Math.Pow(e.xpos - this.xpos, 2) + Math.Pow(e.ypos - this.ypos, 2));
                if (r >= 1)     // 距離０では重力はないものとする
                {
                    theta = Math.Atan2(e.ypos - this.ypos, e.xpos - this.xpos); // 方向
                    a = e.m / Math.Pow(r, 2);                                   // 加速度

                    xacc = a * Math.Cos(theta);
                    yacc = a * Math.Sin(theta);
                }

                xvel += xacc;   // 加速度を速度に加える
                yvel += yacc;   // 加速度を速度に加える
            }
        }

        public virtual bool hit(List<Entity> elist)
        {
            foreach (Entity e in elist)
            {
                if (e == this)  // 自分自身との衝突はしない
                {
                    continue;
                }

                // 今爆発中なら、再度ヒット判定しない（毎サイクルヒットと判定されてしまうので)
                if (this.bang > 0)
                {
                    continue;
                }

                if (Math.Sqrt(Math.Pow((e.xpos - this.xpos), 2) + Math.Pow((e.ypos - this.ypos), 2)) < 3)
                {
                    // 100 サイクル、爆発表示
                    this.bang = 100;
                    return true;
                }
            }

            return false;   // no hit
        }

        // 右に旋回
        public void turnRight()
        {
            head_theta += 0.1;
        }

        // 左に旋回
        public void turnLeft()
        {
            head_theta -= 0.1;
        }

        // スロットル
        public void throttle()
        {
            emit += .1;
            if (emit > 1)
            {
                emit = 1;
            }
        }
    }
}
