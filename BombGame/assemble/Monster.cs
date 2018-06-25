using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombGame.assemble
{
    public enum MonsterState
    {
        UP,
        LEFT,
        DOWN,
        RIGHT
    }
    public delegate void Check();
    public class Monster
    {
        // 检查胜利事件
        public event Check CheckWin;
        // 4个方向行走
        static readonly int[,] run = new int[4,2]{ { 0, -1 }, { -1, 0 }, { 0, 1 }, { 1, 0 } };
        // 怪物的ID
        public int ID { get; set; }
        // 怪物是否死亡
        public bool IsDie { get; set; }
        // 死亡时间，最多30s
        public int dieTime = 0;
        // 怪物的HP
        public int HP { get; set; }
        // 怪物面向的方向
        public MonsterState derState;
        // 上下左右
        public int[] direction = new int[4];

        // 怪物当前坐标
        public int X { get; set; }
        public int Y { get; set; }

        // 怪物转移方向的计时器
        public int Timer { get; set; }

        // 怪物走的步数
        public static readonly int WALK_STEP = 5;

        /// <summary>
        /// 初始化怪物
        /// </summary>
        /// <param name="x">横坐标</param>
        /// <param name="y">纵坐标</param>
        /// <param name="hp">血量</param>
        /// <param name="id">怪物ID</param>
        public Monster(int x, int y, int hp, int id)
        {
            ID = id;
            X = x;
            Y = y;
            if (id == 0)
            {
                hp = 100;
            }
            else if (id >= 1)
            {
                hp = 500;
            }
            dieTime = 0;
            HP = hp;
            derState = MonsterState.RIGHT;
            Timer = 0;
        }

        // 怪物死亡动画图片
        public static Image[,] die =
        {
            {
                global::BombGame.Properties.Resources.monster_01_09,
                global::BombGame.Properties.Resources.monster_01_10,
                global::BombGame.Properties.Resources.monster_01_11,
            },
            {
                global::BombGame.Properties.Resources.monster_02_09,
                global::BombGame.Properties.Resources.monster_02_10,
                global::BombGame.Properties.Resources.monster_02_11,
            },
            {
                global::BombGame.Properties.Resources.monster_03_09,
                global::BombGame.Properties.Resources.monster_03_10,
                global::BombGame.Properties.Resources.monster_03_11,
            }

        };

        // 怪兽方向资源
        public static Image[,,] monster =
        {
            {
                {
                    global::BombGame.Properties.Resources.monster_01_01,
                    global::BombGame.Properties.Resources.monster_01_02,
                },
                {
                    global::BombGame.Properties.Resources.monster_01_03,
                    global::BombGame.Properties.Resources.monster_01_04,
                },
                {
                    global::BombGame.Properties.Resources.monster_01_05,
                    global::BombGame.Properties.Resources.monster_01_06,
                },
                {
                    global::BombGame.Properties.Resources.monster_01_07,
                    global::BombGame.Properties.Resources.monster_01_08,
                },
            },
            {
                {
                    global::BombGame.Properties.Resources.monster_02_01,
                    global::BombGame.Properties.Resources.monster_02_02,
                },
                {
                    global::BombGame.Properties.Resources.monster_02_03,
                    global::BombGame.Properties.Resources.monster_02_04,
                },
                {
                    global::BombGame.Properties.Resources.monster_02_05,
                    global::BombGame.Properties.Resources.monster_02_06,
                },
                {
                    global::BombGame.Properties.Resources.monster_02_07,
                    global::BombGame.Properties.Resources.monster_02_08,
                },
            },
            {
                {
                    global::BombGame.Properties.Resources.monster_03_01,
                    global::BombGame.Properties.Resources.monster_03_02,
                },
                {
                    global::BombGame.Properties.Resources.monster_03_03,
                    global::BombGame.Properties.Resources.monster_03_04,
                },
                {
                    global::BombGame.Properties.Resources.monster_03_05,
                    global::BombGame.Properties.Resources.monster_03_06,
                },
                {
                    global::BombGame.Properties.Resources.monster_03_07,
                    global::BombGame.Properties.Resources.monster_03_08,
                },
            },

        };

        // 画图必备
        Rectangle destRect;
        Rectangle srcRect = new Rectangle(0, 0, Configuration.WIDTH, Configuration.HEIGHT);
        
        /// <summary>
        /// 画怪兽
        /// </summary>
        /// <param name="g"></param>
        public void Draw(Graphics g)
        {
            destRect = new Rectangle(X, Y, Configuration.WIDTH, Configuration.HEIGHT);
            // 如果怪物死亡
            if (IsDie)
            {
                // 如果怪物还需要继续画
                if (dieTime < 30)
                {
                    System.Console.WriteLine(dieTime);
                    g.DrawImage(die[ID, dieTime / 10], destRect, srcRect, GraphicsUnit.Pixel);
                }
            }
            else
            {
                // 怪物的方向
                if (derState == MonsterState.UP)
                {
                    g.DrawImage(monster[ID, 0, direction[0]], destRect, srcRect, GraphicsUnit.Pixel);
                }
                else if (derState == MonsterState.DOWN)
                {
                    g.DrawImage(monster[ID, 1, direction[1]], destRect, srcRect, GraphicsUnit.Pixel);
                }
                else if (derState == MonsterState.LEFT)
                {
                    g.DrawImage(monster[ID, 2, direction[2]], destRect, srcRect, GraphicsUnit.Pixel);
                }
                else if (derState == MonsterState.RIGHT)
                {
                    g.DrawImage(monster[ID, 3, direction[3]], destRect, srcRect, GraphicsUnit.Pixel);
                }
            }
        }

        Random random = new Random();
        // 用于算法标记的数组
        bool[,] mark = new bool[Configuration.MAP_HEIGHT, Configuration.MAP_WIDTH];
        // 算法所用的队列
        Queue<PointPair> q = new Queue<PointPair>();

        /// <summary>
        /// 初始化mark数组
        /// </summary>
        void InitMark()
        {
            for (int i = 0; i < Configuration.MAP_HEIGHT; i++)
            {
                for (int j = 0; j < Configuration.MAP_WIDTH; j++)
                {
                    mark[i, j] = false;
                }
            }
        }
        /// <summary>
        /// 通过BFS算法判断怪物是否可以碰到人物
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public bool CanTouchPlayer(Player player, out MonsterState runState)
        {
            // 初始化
            InitMark();
            q.Clear();
            PointPair pointPair = new PointPair((MonsterState) 0, 
                new Point(X / Configuration.WIDTH, Y / Configuration.HEIGHT), 0);
            q.Enqueue(pointPair);
            mark[Y / Configuration.HEIGHT, X / Configuration.WIDTH] = true;
            PointPair tmp;
            int x, y, tx, ty;
            while (q.Count > 0)
            {
                tmp = q.Dequeue();
                x = tmp.Point.X;
                y = tmp.Point.Y;
                // 如果能碰到那个人，那直接走那个方向
                if (x == player.X / Configuration.WIDTH && 
                    y == player.Y / Configuration.HEIGHT)
                {
                    runState = tmp.FirstState;
                    q.Clear();
                    return true;
                }
                // 四个方向去判定
                for (int i = 0; i < 4; i++)
                {
                    tx = x + run[i, 0];
                    ty = y + run[i, 1];
                    // 如果越界
                    if (tx < 0 || tx >= Configuration.MAP_WIDTH ||
                        ty < 0 || ty >= Configuration.MAP_HEIGHT)
                    {
                        continue;
                    }
                    // 如果存在炸弹
                    if (GameMap.bombMap[ty, tx] != null && GameMap.bombMap[ty, tx].IsExist)
                    {
                        continue;
                    }
                    // 如果没走过且可走
                    if (GameMap.gameMap[ty, tx] == Plat.LAWN && !mark[ty, tx])
                    {
                        if (tmp.Time == 0)
                        {
                            pointPair = new PointPair((MonsterState) i,
                                new Point(tx, ty), tmp.Time + 1);
                        }
                        else
                        {
                            pointPair = new PointPair(tmp.FirstState,
                                new Point(tx, ty), tmp.Time + 1);
                        }
                        mark[ty, tx] = true;
                        q.Enqueue(pointPair);
                    }
                }
            }
            runState = MonsterState.DOWN;
            return false;
        }

        /// <summary>
        /// 随机走动
        /// </summary>
        /// <param name="player"></param>
        public void Wander(Player player)
        {
            Timer++;
            int dir, tx, ty;
            // 每8次刷新面板换一次方向，因为步数是5，恰好走一格
            if (Timer % 8 == 0)
            {
                MonsterState runState;
                // 如果可以碰到人，则追人
                if (CanTouchPlayer(player, out runState))
                {
                    derState = runState;
                }
                // 不行的话，则随机走动
                else
                {
                    // 随机1和-1，表示他会专向的方向
                    dir = random.Next(0, 2);
                    if (dir == 0)
                    {
                        dir = -1;
                    }
                    MonsterState tmp = derState;
                    derState = (MonsterState)(((int)tmp + 4) % 4);
                    // 一个个方向转过去，转到可以动为止。
                    for (int i = 0; i < 4; i++)
                    {
                        
                        tx = (X + Configuration.WIDTH / 2) / Configuration.WIDTH + run[(int) tmp, 0];
                        ty = (Y + Configuration.HEIGHT / 2) / Configuration.HEIGHT + run[(int)tmp, 1];
                        if (tx >= 0 && tx < Configuration.MAP_WIDTH 
                            && ty >= 0 && ty < Configuration.MAP_HEIGHT
                            && GameMap.gameMap[ty, tx] == Plat.LAWN)
                        {
                            if (GameMap.bombMap[ty, tx] == null || !GameMap.bombMap[ty, tx].IsExist)
                            {
                                derState = tmp;
                                break;
                            }
                        }
                        tmp = (MonsterState)(((int)tmp + dir + 4) % 4);
                    }
                }
            }
        }

        /// <summary>
        /// 怪物减少HP，
        /// 这里存在一个BUG，因为一个炸弹会存在很久，所以怪物不一定是只减少10滴血
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>        
        public void ReduceHp(int x, int y)
        {
            int dx = (X + Configuration.WIDTH / 2) / Configuration.WIDTH;
            int dy = (Y + Configuration.HEIGHT / 2) / Configuration.HEIGHT;
            if (x == dx && y == dy)
            {
                HP -= 10;
            }
            if (HP <= 0)
            {
                IsDie = true;
                CheckWin();
            }
        }
        /// <summary>
        /// 怪物移动
        /// </summary>
        public void Move()
        {
            if (IsDie)
            {
                return;
            }
            bool is1, is2;
            switch (derState)
            {
                // 向上
                case MonsterState.UP:
                    direction[0] = (direction[0] + 1) % 2;
                    is1 = Player.CanMove(X, Y - WALK_STEP);
                    is2 = Player.CanMove(X + Configuration.WIDTH - 1, Y - WALK_STEP);
                    if (is1 && is2)
                    {
                        Y -= WALK_STEP;
                    }
                    else if (is1)
                    {
                        X -= WALK_STEP;
                    }
                    else if (is2)
                    {
                        X += WALK_STEP;
                    }
                    break;
                case MonsterState.DOWN:
                    direction[1] = (direction[1] + 1) % 2;
                    is1 = Player.CanMove(X, Y + Configuration.HEIGHT + WALK_STEP);
                    is2 = Player.CanMove(X + Configuration.WIDTH - 1, Y + Configuration.HEIGHT + WALK_STEP - 1);
                    if (is1 && is2)
                    {
                        Y += WALK_STEP;
                    }
                    else if (is1)
                    {
                        X -= WALK_STEP;
                    }
                    else if (is2)
                    {
                        X += WALK_STEP;
                    }
                    break;
                case MonsterState.LEFT:
                    direction[2] = (direction[2] + 1) % 2;
                    is1 = Player.CanMove(X - WALK_STEP, Y);
                    is2 = Player.CanMove(X - WALK_STEP, Y + Configuration.HEIGHT - 1);
                    if (is1 && is2)
                    {
                        X -= WALK_STEP;
                    }
                    else if (is1)
                    {
                        Y -= WALK_STEP;
                    }
                    else if (is2)
                    {
                        Y += WALK_STEP;
                    }
                    break;
                case MonsterState.RIGHT:
                    direction[3] = (direction[3] + 1) % 2;
                    is1 = Player.CanMove(X + Configuration.WIDTH + WALK_STEP - 1, Y);
                    is2 = Player.CanMove(X + Configuration.WIDTH + WALK_STEP - 1, Y + Configuration.HEIGHT - 1);
                    if (is1 && is2)
                    {
                        X += WALK_STEP;
                    }
                    else if (is1)
                    {
                        Y -= WALK_STEP;
                    }
                    else if (is2)
                    {
                        Y += WALK_STEP;
                    }
                    break;
            }
        }
    }

    /// <summary>
    /// 用于算法的数据结构
    /// </summary>
    public class PointPair
    {
        public MonsterState FirstState { get; set; }
        public Point Point { get; set; }

        public int Time { get; set; }
        public PointPair(MonsterState f, Point p, int t)
        {
            FirstState = f;
            Point = p;
            Time = t;
        }
    }
}
