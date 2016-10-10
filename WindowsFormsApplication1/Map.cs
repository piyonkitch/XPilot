using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication1
{
    [Serializable()]
    class Map
    {
        List<Entity> wlist;

        // Entity のコレクタ
        public Map()
        {
            wlist = new List<Entity>();
        }

        // foreach で数えられるようにする
        public IEnumerator<Entity> GetEnumerator()
        {
            foreach (Entity e in wlist)
            {
                yield return e;
            }
        }

        // ダミーのマップ
        public void MapReadFile()   // ファイルからのリードは今後の課題にする
        {
            // 上
            for (int x = 0; x <= 400; x += 10)
            {
                wlist.Add(new Wall(x, 0));
            }
            // 下
            for (int x = 0; x <= 400; x += 10)
            {
                wlist.Add(new Wall(x, 400));
            }
            // 左
            for (int y = 0+10; y < 400; y += 10)
            {
                wlist.Add(new Wall(0, y));
            }
            // 右
            for (int y = 0+10; y < 400; y += 10)
            {
                wlist.Add(new Wall(400, y));
            }


            for (int i = 160; i < 200; i += 10)
            {
                wlist.Add(new Wall(i, 50));
            }
            for (int i = 90; i < 120; i += 10)
            {
                wlist.Add(new Wall(200, i));
            }
        }
    }
}
