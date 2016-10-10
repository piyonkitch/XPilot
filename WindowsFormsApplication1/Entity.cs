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

        private Random rnd = new Random();

        // Init (xpos, ypos)
        public Entity()
        {
            xpos = rnd.Next(400);
            ypos = rnd.Next(400);
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

        // pos += vel
        public virtual void tick()
        {
            // If I am banging, do not move.
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

            // Freeze is count down to rebirth
            if (freeze > 0)
            {
                freeze--;
                return;
            }

            xpos += xvel;
            ypos += yvel;
        }

        // vel += emit
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
                if (r >= 1)     // 距離０では重力はないものとする
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
                if (e == this)  // 自分自身との衝突はしない
                {
                    continue;
                }

                // 今爆発中なら、再度ヒット判定しない（毎サイクルヒットと判定されてしまうので)
                if (this.bang > 0)
                {
                    continue;
                }

                if (distance(e, this) < 3)
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

        // 自動運転
        public virtual void automove(List<Entity> elist)
        {
            Entity w;

            // Find nearest wall and head against it.
            w = findNearestWall(elist);
            if (w != null)
            {
                double theta_tobe;
                double theta_diff;

                w.isRecognized = true;      // mark this wall is recognized by a ship

                theta_tobe = Math.Atan2((this.ypos - w.ypos), (this.xpos - w.xpos));
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

                // 逆向きになっていたら、噴出する。
                if ( ((theta_tobe - head_theta) > -(Math.PI / 4)) &&
                     ((theta_tobe - head_theta) <  (Math.PI / 4)) )
                {
                    if (distance(this, w) < 10)
                    {
                        emit += .1;
                    }
                    else if (distance(this, w) < 30)
                    {
                        emit += .05;
                    }

                    // Do not burst too much.
                    if (emit > 8)
                    {
                        emit = .8;
                    }
                }
            }
        }

        // 認識状態関連
        public void clear_isRecognized()
        {
            isRecognized = false;
        }

        //
        // 雑多な処理
        //
        private Entity findNearestWall(List<Entity> elist)
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

        private double distance(Entity e1, Entity e2)
        {
            return Math.Sqrt(Math.Pow((e1.xpos - e2.xpos), 2) 
                             +
                             Math.Pow((e1.ypos - e2.ypos), 2)
                   );
        }

    }
}
