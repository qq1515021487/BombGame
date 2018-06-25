using game.assemble;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombGame.assemble
{
    public enum Plat
    {
        WALL,
        LAWN,
        BRICK,
        BOMB_BUFF,
        POWER_BUFF,
        SPID_BUFF,
        BRICK_BREAK,
        BIG_WALL_SHOW,
        BIG_WALL_HIDE
    }

    public class Wall {
        // 墙的坐标
        public int ShowX { get; set; }
        public int ShowY { get; set; }
    }

    public class GameMap
    {
        // 放蛋的时间
        public static int bombTimeCount = 0;

        // 每个墙的宽
        public static int wallWidth = Configuration.WIDTH;

        // 每个墙的高
        public static int wallHeight = Configuration.HEIGHT;


        // 炸弹数量，炸弹能量，行走速度
        public static int[] buffCount = new int[3] { 10, 10, 10 };
        
        // 所有被放置的球的集合
        public static List<Bomb> bombs = new List<Bomb>();

        // 炸弹所在的位置
        public static Bomb[,] bombMap = new Bomb[Configuration.MAP_HEIGHT, Configuration.MAP_WIDTH];

        // 爆炸的时间
        public static int[,] explodeTime = new int[Configuration.MAP_HEIGHT, Configuration.MAP_WIDTH];

        // 游戏地图
        public static Plat[,] gameMap = new Plat[Configuration.MAP_HEIGHT, Configuration.MAP_WIDTH];


        /// <summary>
        /// 图片的加载
        /// </summary>
        private static Image[] brick_1 =
        {
            global::BombGame.Properties.Resources.brick_01_01,
            global::BombGame.Properties.Resources.brick_02_01,
            global::BombGame.Properties.Resources.brick_03_01,
            global::BombGame.Properties.Resources.brick_04_01,
            global::BombGame.Properties.Resources.brick_05_01,
            global::BombGame.Properties.Resources.brick_06_01,
            global::BombGame.Properties.Resources.brick_07_01,
            global::BombGame.Properties.Resources.brick_08_01,
        };

        private static Image[] brick_2 =
        {
            global::BombGame.Properties.Resources.brick_01_02,
            global::BombGame.Properties.Resources.brick_02_02,
            global::BombGame.Properties.Resources.brick_03_02,
            global::BombGame.Properties.Resources.brick_04_02,
            global::BombGame.Properties.Resources.brick_05_02,
            global::BombGame.Properties.Resources.brick_06_02,
            global::BombGame.Properties.Resources.brick_07_02,
            global::BombGame.Properties.Resources.brick_08_02,
        };
        private static Image[] wall_1 =
        {
            global::BombGame.Properties.Resources.wall_01_01,
            global::BombGame.Properties.Resources.wall_01_02,
            global::BombGame.Properties.Resources.wall_01_03,
            global::BombGame.Properties.Resources.wall_01_04,
            global::BombGame.Properties.Resources.wall_01_05,
            global::BombGame.Properties.Resources.wall_01_06,
            global::BombGame.Properties.Resources.wall_01_07,
            global::BombGame.Properties.Resources.wall_01_08,
            global::BombGame.Properties.Resources.wall_01_09,
            global::BombGame.Properties.Resources.wall_01_10,
            global::BombGame.Properties.Resources.lawn_08,
            global::BombGame.Properties.Resources.wall_01_11,
            global::BombGame.Properties.Resources.wall_01_12,
            global::BombGame.Properties.Resources.wall_01_13,
            global::BombGame.Properties.Resources.wall_01_14,
            global::BombGame.Properties.Resources.wall_01_15,
        };

        private static Image[] wall_2 =
        {
            global::BombGame.Properties.Resources.wall_02_01,
            global::BombGame.Properties.Resources.wall_02_02,
            global::BombGame.Properties.Resources.wall_02_03,
            global::BombGame.Properties.Resources.wall_02_04,
            global::BombGame.Properties.Resources.wall_02_05,
            global::BombGame.Properties.Resources.wall_02_06,
            global::BombGame.Properties.Resources.wall_02_07,
            global::BombGame.Properties.Resources.wall_02_08,
            global::BombGame.Properties.Resources.wall_02_09,
            global::BombGame.Properties.Resources.wall_02_10,
            global::BombGame.Properties.Resources.lawn_08,
            global::BombGame.Properties.Resources.wall_02_11,
            global::BombGame.Properties.Resources.wall_02_12,
            global::BombGame.Properties.Resources.wall_02_13,
            global::BombGame.Properties.Resources.wall_02_14,
            global::BombGame.Properties.Resources.wall_02_15,
        };

        private static Image[] big_wall =
        {
            global::BombGame.Properties.Resources.big_wall_01,
            global::BombGame.Properties.Resources.big_wall_02,
            global::BombGame.Properties.Resources.big_wall_03,
            global::BombGame.Properties.Resources.big_wall_04,
            global::BombGame.Properties.Resources.big_wall_05,
            global::BombGame.Properties.Resources.big_wall_06,
            global::BombGame.Properties.Resources.big_wall_07,
            global::BombGame.Properties.Resources.big_wall_08,
            global::BombGame.Properties.Resources.big_wall_09,
            global::BombGame.Properties.Resources.big_wall_10,
            global::BombGame.Properties.Resources.big_wall_11,
            global::BombGame.Properties.Resources.big_wall_12,
            global::BombGame.Properties.Resources.big_wall_13,
            global::BombGame.Properties.Resources.big_wall_14,
        };

        private static Image[] lawn =
        {
            global::BombGame.Properties.Resources.lawn_01,
            global::BombGame.Properties.Resources.lawn_02,
            global::BombGame.Properties.Resources.lawn_03,
            global::BombGame.Properties.Resources.lawn_04,
            global::BombGame.Properties.Resources.lawn_05,
            global::BombGame.Properties.Resources.lawn_06,
            global::BombGame.Properties.Resources.lawn_07,
            global::BombGame.Properties.Resources.lawn_08,
            global::BombGame.Properties.Resources.lawn_09,
        };

        private static Image[] buff =
        {
            global::BombGame.Properties.Resources.bombBuff,
            global::BombGame.Properties.Resources.powerBuff,
            global::BombGame.Properties.Resources.spidBuff,
        };


        static Rectangle destRect = new Rectangle(0, 0, 0, 0);
        static Rectangle destRect_2 = new Rectangle(0, 0, 0, 0);
        static Image drawImage = null;
        static Rectangle srcRect = new Rectangle(0, 0, wallWidth, wallHeight);


        // 地图图片编码
        public static int[,,] gameMapCode = 
        {
            {
                {10,10,10,10,10,10,10,10,10,10,10,10,10,10,10},
                {4,2,3,2,2,6,1,1,5,6,7,2,8,4,6},
                {4,6,3,8,4,9,5,1,1,9,3,1,4,4,4},
                {4,4,4,2,2,6,1,5,5,9,6,3,8,2,6},
                {2,7,1,6,1,9,5,1,1,9,2,2,2,3,2},
                {1,3,4,3,3,6,1,1,5,9,8,2,7,4,6},
                {3,6,2,8,2,9,5,5,1,6,2,1,3,1,2},
                {9,9,9,9,9,9,1,1,5,9,9,9,9,9,9},
                {1,3,2,3,2,6,5,1,1,9,4,7,2,6,2},
                {6,1,7,2,6,6,1,5,5,6,2,1,1,4,2},
                {2,3,2,1,2,9,5,1,1,9,3,8,3,7,2},
                {7,4,8,3,7,6,1,1,5,6,4,4,3,2,4},
                {4,4,2,4,2,9,5,5,1,9,2,8,4,6,4},
                {6,4,8,3,6,6,1,1,5,6,3,1,2,4,4},
            },
            {
                {10,10,10,10,10,10,10,10,10,10,10,10,10,10,10},
                {6,6,6,6,6,6,6,6,6,6,6,6,6,6,6},
                {6,6,6,6,6,6,6,6,6,6,6,6,6,6,6},
                {6,6,6,6,6,6,6,6,6,6,6,6,6,6,6},
                {6,6,6,6,5,6,7,6,6,6,7,6,6,6,6},
                {6,6,6,6,8,9,10,6,6,6,7,6,6,6,6},
                {6,6,6,6,11,12,13,7,7,6,7,6,6,6,6},
                {6,6,6,7,6,7,6,6,6,6,6,7,6,6,6},
                {12,11,12,13,6,6,6,6,0,1,6,6,6,6,6},
                {6,6,6,7,6,7,6,2,3,4,6,14,15,14,15},
                {6,6,6,7,6,6,6,6,6,6,6,6,6,6,6},
                {6,6,6,6,6,6,7,7,6,6,6,6,6,6,6},
                {6,6,6,6,6,6,6,6,6,6,6,6,6,6,6},
                {6,6,6,6,6,6,6,6,6,6,6,6,6,6,6},
            }

        };

        // 地图图片准确编码
        public static int[,,] gameMapCurrectCode =
        {
            {
                {10,10,10,10,10,10,10,10,10,10,10,10,10,10,10},
                {4,4,4,4,4,6,1,1,1,6,4,4,8,4,6},
                {4,6,4,8,4,9,1,1,1,9,4,4,4,4,4},
                {4,4,4,4,4,6,1,1,1,9,6,4,8,4,6},
                {4,7,4,6,4,9,1,1,1,9,4,4,4,4,4},
                {4,4,4,4,4,6,1,1,1,9,8,4,7,4,6},
                {4,6,4,8,4,9,1,1,1,6,4,4,4,4,4},
                {9,9,9,9,9,9,1,1,1,9,9,9,9,9,9},
                {4,4,4,4,4,6,1,1,1,9,4,7,4,6,4},
                {6,4,7,4,6,6,1,1,1,6,4,4,4,4,4},
                {4,4,4,4,4,9,1,1,1,9,4,8,4,7,4},
                {7,4,8,4,7,6,1,1,1,6,4,4,4,4,4},
                {4,4,4,4,4,9,1,1,1,9,4,8,4,6,4},
                {6,4,8,4,6,6,1,1,1,6,4,4,4,4,4},
            },
            {
                {10,10,10,10,10,10,10,10,10,10,10,10,10,10,10},
                {8,8,8,8,8,8,8,8,8,8,8,8,8,8,8},
                {8,8,8,8,8,8,8,8,8,8,8,8,8,8,8},
                {8,8,8,8,8,8,8,8,8,8,8,8,8,8,8},
                {8,8,8,8,8,8,8,8,8,8,8,8,8,8,8},
                {8,8,8,8,8,8,8,8,8,8,8,8,8,8,8},
                {8,8,8,8,8,8,8,8,8,8,8,8,8,8,8},
                {8,8,8,8,8,8,8,8,8,8,8,8,8,8,8},
                {8,8,8,8,8,8,8,8,8,8,8,8,8,8,8},
                {8,8,8,8,8,8,8,8,8,8,8,8,8,8,8},
                {8,8,8,8,8,8,8,8,8,8,8,8,8,8,8},
                {8,8,8,8,8,8,8,8,8,8,8,8,8,8,8},
                {8,8,8,8,8,8,8,8,8,8,8,8,8,8,8},
                {8,8,8,8,8,8,8,8,8,8,8,8,8,8,8},
            }
        };

        // 道具
        public static int[,] prop =
        {
            {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        };

        // 地图码，wall lawn
        private static int[,,] mapp =
        {
            {
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {1,2,2,2,2,1,1,1,2,1,0,2,0,1,0},
                {1,0,2,0,1,0,2,1,1,0,2,2,1,1,1},
                {1,1,2,2,2,1,1,2,2,0,0,2,0,2,0},
                {2,0,2,0,2,0,2,1,1,0,2,2,2,2,2},
                {2,2,2,2,2,1,1,1,2,0,0,2,0,2,0},
                {2,0,2,0,2,0,2,2,1,1,2,2,2,2,2},
                {0,0,0,0,0,0,1,1,2,0,0,0,0,0,0},
                {2,2,2,2,2,1,2,1,1,0,1,0,2,0,2},
                {0,2,0,2,0,1,1,2,2,1,2,2,2,1,2},
                {2,2,2,2,2,0,2,1,1,0,2,0,2,0,2},
                {0,1,0,2,0,1,1,1,2,1,2,2,2,2,1},
                {1,1,2,2,2,0,2,2,1,0,2,0,1,0,1},
                {0,1,0,2,0,1,1,1,2,1,2,2,2,1,1},

            },
            {
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {1,1,1,1,1,2,2,2,1,2,2,2,2,1,2},
                {1,2,1,2,2,2,1,2,2,2,2,2,2,2,1},
                {1,2,1,1,2,2,1,2,2,2,2,1,2,1,1},
                {2,2,2,1,7,7,7,2,1,2,2,1,1,2,1},
                {1,1,1,2,7,7,7,2,2,2,1,1,2,1,2},
                {1,1,2,1,7,7,7,2,1,1,2,1,2,1,1},
                {1,1,1,1,2,2,2,2,2,1,1,1,2,1,1},
                {0,0,0,0,1,2,2,1,7,7,1,1,1,1,1},
                {1,1,2,2,1,2,2,7,7,7,1,0,0,0,0},
                {2,2,2,1,1,2,2,1,1,2,1,2,2,1,1},
                {1,2,2,1,1,2,2,1,1,2,1,2,2,2,2},
                {1,1,2,1,2,1,1,2,1,1,1,2,1,1,1},
                {1,1,2,1,1,1,1,1,1,1,2,1,1,1,1},

            }
        };

        // 地图ID
        public static int idx;

        /// <summary>
        /// 加载地图
        /// </summary>
        public static void LoadMap(int _id)
        {
            for (int i = 0; i < buffCount.Length; i++)
            {
                buffCount[i] = 10;
            }
            bombs.Clear();
            Random random = new Random();
            idx = _id;
            for (int i = 0; i < Configuration.MAP_HEIGHT; i++)
            {
                for (int j = 0; j < Configuration.MAP_WIDTH; j++)
                {
                    // 初始化炸弹和道具
                    bombMap[i, j] = null;
                    prop[i, j] = 0;
                    if (mapp[idx, i, j] == 0)
                    {
                        gameMap[i, j] = Plat.WALL;
                    }
                    else if (mapp[idx, i, j] == 1)
                    {
                        gameMap[i, j] = Plat.LAWN;
                    }
                    else if (mapp[idx, i, j] == 7)
                    {
                        gameMap[i, j] = Plat.BIG_WALL_SHOW;
                    }
                    else
                    {
                        gameMap[i, j] = Plat.BRICK;
                    }
                }
            }
            ReloadProp();
        }

        /// <summary>
        /// 交换函数，交换两个数
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        private static void swap(ref int a, ref int b)
        {
            int c = a;
            a = b;
            b = c;
        }

        /// <summary>
        /// 随机化道具
        /// </summary>
        public static void ReloadProp()
        {
            int totBuff = 0, brickCount = 0;
            // 获取Buff数量
            for (int i = 0; i < buffCount.Length; i++)
            {
                totBuff += buffCount[i];
            }
            
            // 统计砖块数量
            for (int i = 0; i < Configuration.MAP_HEIGHT; i++)
            {
                for (int j = 0; j < Configuration.MAP_WIDTH; j++)
                {
                    if (gameMap[i, j] == Plat.BRICK)
                    {
                        brickCount++;
                    }
                }
            }
            // 临时变量
            int[] mark = new int[brickCount];
            int[] need = new int[totBuff];
            int cnt = 0;
            for (int i = 0; i < brickCount; i++)
            {
                mark[i] = i;
            }
            Random random = new Random();
            int maxlen = brickCount - 1;

            // 随机Buff位置
            for (int i = 0; i < totBuff; i++)
            {
                int j = random.Next(0, maxlen + 1);
                need[cnt++] = mark[j];
                swap(ref mark[j], ref mark[maxlen]);
                maxlen--;
            }
            Array.Sort(need);
            cnt = 0;
            int p = 0, propLen = buffCount.Length;
            // 将有Buff的位置给坐标赋值
            for (int i = 0; i < Configuration.MAP_HEIGHT; i++)
            {
                for (int j = 0; j < Configuration.MAP_WIDTH; j++)
                {
                    if (gameMap[i, j] == Plat.BRICK)
                    {
                        // 如果该坐标是有道具的坐标，赋值
                        if (p < totBuff && cnt == need[p])
                        {
                            System.Console.WriteLine("道具所在的坐标为：" + i + " " + j + "\n");
                            int type = random.Next(0, propLen);
                            buffCount[type]--;
                            prop[i, j] = type + 1;
                            // 如果buff数量不够了，那就舍弃
                            if (buffCount[type] == 0)
                            {
                                swap(ref buffCount[type], ref buffCount[propLen - 1]);
                                propLen--;
                            }
                            p++;
                        }
                        cnt++;
                    }
                }
            }
        }

        /// <summary>
        /// 减少总体炸弹爆炸时间
        /// </summary>
        public static void ReduceExplodeTime()
        {
            for (int i = 0; i < Configuration.MAP_HEIGHT; i++)
            {
                for (int j = 0; j < Configuration.MAP_WIDTH; j++)
                {
                    if (explodeTime[i, j] >= 20) 
                    {
                        explodeTime[i, j] -= 20;
                    }
                }
            }
        }
        /// <summary>
        /// 扫描一次整个场地，重新把原本是砖块的地方转成平地
        /// </summary>
        public static void ReloadBrickBreak()
        {
            for (int i = 0; i < Configuration.MAP_HEIGHT; i++)
            {
                for (int j = 0; j < Configuration.MAP_WIDTH; j++)
                {
                    if (explodeTime[i, j] == 0 && gameMap[i, j] == Plat.BRICK_BREAK)
                    {
                        gameMap[i, j] = Plat.LAWN;
                    }
                }
            }
        }
        public GameMap()
        {
        }
        
        /// <summary>
        /// 画整个地图
        /// </summary>
        /// <param name="g"></param>
        public static void Draw(Graphics g)
        {
            
            for (int i = 0; i < Configuration.MAP_HEIGHT; i++)
            {
                for (int j = 0; j < Configuration.MAP_WIDTH; j++)
                {
                    destRect.X = j * wallWidth;
                    destRect.Y = i * wallHeight;
                    destRect.Width = wallWidth;
                    destRect.Height = wallHeight;
                    // 画砖块
                    if (gameMap[i, j] == Plat.BRICK)
                    {
                        // 头顶还有一块东西
                        drawImage = brick_2[gameMapCode[idx, i, j]];
                        destRect_2.X = j * wallWidth;
                        destRect_2.Y = (i - 1) * wallHeight;
                        destRect_2.Width = wallWidth;
                        destRect_2.Height = wallHeight;
                        g.DrawImage(drawImage, destRect_2, srcRect, GraphicsUnit.Pixel);
                        drawImage = brick_1[gameMapCode[idx, i, j]];
                    }
                    // 画大型块
                    else if (gameMap[i, j] == Plat.BIG_WALL_SHOW)
                    {
                        drawImage = lawn[gameMapCurrectCode[idx, i, j]];
                        // 先画个草地给他
                        g.DrawImage(drawImage, destRect, srcRect, GraphicsUnit.Pixel);
                        drawImage = big_wall[gameMapCode[idx, i, j]];
                    }
                    // 画墙
                    else if (gameMap[i, j] == Plat.WALL)
                    {
                        // 头顶还有一块东西
                        drawImage = wall_2[gameMapCode[idx, i, j]];
                        destRect_2.X = j * wallWidth;
                        destRect_2.Y = (i - 1) * wallHeight;
                        destRect_2.Width = wallWidth;
                        destRect_2.Height = wallHeight;
                        g.DrawImage(drawImage, destRect_2, srcRect, GraphicsUnit.Pixel);
                        drawImage = wall_1[gameMapCode[idx, i, j]];
                    }
                    // 如果是陆地。按照gameMapCurrectCode来画
                    else if (gameMap[i, j] == Plat.LAWN)
                    {
                        drawImage = lawn[gameMapCurrectCode[idx, i, j]];
                        g.DrawImage(drawImage, destRect, srcRect, GraphicsUnit.Pixel);
                        // 如果有道具，就把道具画上
                        if (prop[i, j] != 0)
                        {
                            drawImage = buff[prop[i, j] - 1];
                            g.DrawImage(drawImage, destRect, srcRect, GraphicsUnit.Pixel);
                        }
                        else
                        {
                            continue;
                        }
                    }
                    // 如果是正在被炸开的地方，画个草地给他
                    else if (gameMap[i, j] == Plat.BRICK_BREAK)
                    {
                        drawImage = lawn[gameMapCurrectCode[idx, i, j]];
                    }
                    else
                    {
                        continue;
                    }
                    g.DrawImage(drawImage, destRect, srcRect, GraphicsUnit.Pixel);
                        
                }
            }
            
        }
    }
}
