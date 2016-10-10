using System;
using System.Collections.Generic;

namespace WindowsFormsApplication1
{
    class Wall : Entity
    {
        Random rnd = new Random();

        // 座標を初期化する
        public Wall()
        {
            xpos = rnd.Next(100);
            ypos = rnd.Next(100);
            head_theta = 0;
            m = 0.01;
        }

        // 座標を初期化する
        public Wall(double x, double y)
        {
            xpos = x;
            ypos = y;
            head_theta = 0;
            m = 1;
        }

        // 速度分を座標に追加→動かない
        public override void tick()
        {
            ;
        }

        // emit を速度に反映→動かない
        public override void move()
        {
            ;
        }

        // 各物体間の引力から速度を更新→動かない
        public override void attract(List<Entity> elist)
        {
            ;
        }

        // 自動運転→動かない
        public override void automove(List<Entity> elist)
        {
            ;
        }
    }
}