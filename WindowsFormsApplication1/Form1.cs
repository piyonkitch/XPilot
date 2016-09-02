using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        List<Entity> elist;     // Entity のリスト
        Map map;                // Wall が入っている。
        Entity myship;          // My ship
        Timer timer;
        Random rnd = new Random();

        /// <summary>
        /// フォームの初期化＋物体の初期化＋タイマ開始
        /// </summary>
        public Form1()
        {
            InitializeComponent();

            elist = new List<Entity>();
            System.Threading.Thread.Sleep(100);
            elist.Add(new Entity());
            System.Threading.Thread.Sleep(100);
            elist.Add(new Entity());

            // とりあえず自家製のマップを作成する。
            map = new Map();
            map.MapReadFile();

//          map = (Map)LoadFromBinaryFile("c:\\temp\\test.obj");    // ファイルからのマップロードは別課題にしてみる

            // マップから壁を取り出して、Form1 におけるすべてのモノリストに追加する。
            foreach (Entity e in map)
            {
                elist.Add(e);
            }
            map = null;                 // maplist を捨てる。

            // 最初の船を自分の船にする
            myship = elist[0];

            // とりあえずタイマー
            timer = new Timer();
            timer.Interval = 10;
            timer.Tick += new EventHandler(myTick);
            timer.Start();
        }

        /// <summary>
        /// メイン画面に、elist 上のすべての物体を描画する
        /// </summary>
        private void show()
        {
            //描画先とするImageオブジェクトを作成する
            Bitmap canvas = new Bitmap(pic.Width, pic.Height);
            //ImageオブジェクトのGraphicsオブジェクトを作成する
            Graphics g = Graphics.FromImage(canvas);

            // 矢印を使ってみる
            Pen penShip = new Pen(Color.Black, 5);
            penShip.EndCap = System.Drawing.Drawing2D.LineCap.ArrowAnchor;

            Pen penRock = new Pen(Color.Blue, 1);            // Wall は青□
            Pen penCrash = new Pen(Color.Red, 1);            // crash は赤●
            Pen penBullet = new Pen(Color.Black, 1);            // crash は赤●

            foreach (Entity e in elist)
            {
                if (e is Wall)
                {
                    g.DrawRectangle(penRock, (int)e.xpos, (int)e.ypos, 5 /* 幅 */, 5 /* 高さ */);
                }
                else if (e is Bullet)
                {
                    g.DrawEllipse(penCrash, (int)e.xpos, (int)e.ypos, 2 /* 幅 */, 2 /* 幅 */);
                }
                else
                {
                    // REVISIT 船に決め打ち
                    if (e.bang > 0)
                    {
                        int r = rnd.Next(10) + 5;   /* 5～15 ドットの● */
                        g.DrawEllipse(penCrash, (int)e.xpos, (int)e.ypos, r, r);
                    }
                    else {
                        g.DrawLine(penShip, 
                            (float)e.xpos,
                            (float)e.ypos,
                            (float)(e.xpos + 10 * Math.Cos(e.head_theta)),
                            (float)(e.ypos + 10 * Math.Sin(e.head_theta))
                            );
                    }
                }
            }

            Font fnt = new Font("MS UI Gothic", 10);

            //リソースを解放する
            fnt.Dispose();
            g.Dispose();

            //PictureBox1に表示する
            pic.Image = canvas;
        }

        /// <summary>
        /// Process User Input (Keyboard)
        /// </summary>
        private void processUserInput()
        {
            if (Keyboard.IsKeyDown(Key.A))
            {
                myship.turnLeft();
            }
            if (Keyboard.IsKeyDown(Key.S))
            {
                myship.turnRight();
            }
            if (Keyboard.IsKeyDown(Key.Space))
            {
                myship.throttle();
            }
            if (Keyboard.IsKeyDown(Key.Z))
            {
                Bullet bullet = new Bullet(myship);     // bullet を作る。速度などは myship と同じ。
                elist.Add(bullet);
            }

        }

        /// <summary>
        /// 定期的に呼ばれて、elist を動かす。
        /// </summary>
        /// <param name="sender">イベント送信者</param>
        /// <param name="e">イベント？</param>
        private void myTick(object sender, EventArgs e)
        {
            // キー入力
            processUserInput();

            // 削除することにしていた要素を削る。
            for (int i = elist.Count - 1; i >= 0; i--)
            {
                if (elist[i].vanish)
                {
                    elist.Remove(elist[i]);
                }
            }

            foreach (Entity le in elist)
            {
                le.attract(elist);          // update velocity by gravity
            }
            foreach (Entity le in elist)
            {
                le.move();                   // update velocity by emission
            }
            foreach (Entity le in elist)
            {
                le.tick();                   // update position
            }

            // 各自衝突判定をする。
            foreach (Entity le in elist)
            {
                le.hit(elist);
            }

            show();
        }

        //
        // 以下、ボタンイベント
        //
        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
