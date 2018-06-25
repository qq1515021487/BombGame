using BombGame;
using BombGame.assemble;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace game.assemble
{
    public class Bomb
    {

        // 泡泡的坐标
        public int ShowX { get; set; }
        public int ShowY { get; set; }

        // 泡泡倒计时
        public int EndTime { get; set; }

        // 泡泡的长度
        public int BombLen { get; set; }

        // 泡泡当前的类型
        public int BombType { get; set; }

        // 最大类型数
        public int BombTypeRound { get; set; }

        // 泡泡图形的长度和宽度
        public static int DrawHeight = Configuration.HEIGHT;
        public static int DrawWidth = Configuration.WIDTH;

        // 方向，上下左右
        private int[, ] direction = new int[4, 2] { { 0, -1}, { 0, 1}, { -1, 0}, { 1, 0} };

        // 炸弹是否存在
        public bool IsExist;

        // 炸弹存在的时间
        private int time;

        // 炸弹样式ID
        public int ComfromId;

        public Bomb()
        {
            IsExist = false;
        }

        /// <summary>
        /// 初始化炸弹
        /// </summary>
        /// <param name="showX"></param>
        /// <param name="showY"></param>
        /// <param name="bombLen"></param>
        /// <param name="bombType"></param>
        /// <param name="comfromId"></param>
        public Bomb(int showX, int showY, int bombLen, int bombType, int comfromId)
        {
            IsExist = true;
            this.ShowX = showX;
            this.ShowY = showY;
            this.BombLen = bombLen;
            this.BombType = bombType;
            this.BombTypeRound = 3;
            this.EndTime = 0;
            this.ComfromId = comfromId;
        }

        Rectangle destRect = new Rectangle(0, 0, 0, 0);
        Rectangle srcRect = new Rectangle(0, 0, DrawWidth, DrawHeight);

        // 炸弹图片
        Image[] bombImage = new Image[3]
        {
            global::BombGame.Properties.Resources.bomb_1_01,
            global::BombGame.Properties.Resources.bomb_1_02,
            global::BombGame.Properties.Resources.bomb_1_03,
        };

        // 爆炸时的图片
        Image[,] explodeImage =
        {
            {
                global::BombGame.Properties.Resources.explode_01_01_01,
                global::BombGame.Properties.Resources.explode_01_01_02,
                global::BombGame.Properties.Resources.explode_01_01_03,
                global::BombGame.Properties.Resources.explode_01_01_04,
            },
            {
                global::BombGame.Properties.Resources.explode_01_02_01,
                global::BombGame.Properties.Resources.explode_01_02_02,
                global::BombGame.Properties.Resources.explode_01_02_03,
                global::BombGame.Properties.Resources.explode_01_02_04,
            },
            {
                global::BombGame.Properties.Resources.explode_01_03_01,
                global::BombGame.Properties.Resources.explode_01_03_02,
                global::BombGame.Properties.Resources.explode_01_03_03,
                global::BombGame.Properties.Resources.explode_01_03_04,
            },
            {
                global::BombGame.Properties.Resources.explode_01_04_01,
                global::BombGame.Properties.Resources.explode_01_04_02,
                global::BombGame.Properties.Resources.explode_01_04_03,
                global::BombGame.Properties.Resources.explode_01_04_04,
            },
            {
                global::BombGame.Properties.Resources.explode_01_00_01,
                global::BombGame.Properties.Resources.explode_01_00_01,
                global::BombGame.Properties.Resources.explode_01_00_02,
                global::BombGame.Properties.Resources.explode_01_00_03,
            },
            {
                global::BombGame.Properties.Resources.explode_10_01,
                global::BombGame.Properties.Resources.explode_10_02,
                global::BombGame.Properties.Resources.explode_10_03,
                global::BombGame.Properties.Resources.explode_10_04,
            },
        };

        // 画蛋蛋
        public void Draw(Graphics g)
        {
            // 设置画图的位置
            destRect.X = ShowX * DrawWidth;
            destRect.Y = ShowY * DrawHeight;
            destRect.Width = DrawWidth;
            destRect.Height = DrawHeight;
            
            g.DrawImage(bombImage[this.BombType], destRect, srcRect, GraphicsUnit.Pixel);
            this.time++;
            // 10次一轮回，换样式
            if (this.time % 10 == 0)
            {
                this.BombType++;
                this.BombType %= this.BombTypeRound;
            }
            
            this.EndTime += 10;
        }

        public void Swap(ref int a, ref int b)
        {
            int tmp = a;
            a = b;
            b = tmp;
        }
        
        /// <summary>
        /// 炸弹爆炸瞬间画图。type为动画的第几次
        /// </summary>
        /// <param name="type"></param>
        public void DrawExplode(Graphics g, int type, int time, Player player)
        {
            int leftlen = Math.Max(ShowX - BombLen, 0);
            int rightlen = Math.Min(ShowX + BombLen, Configuration.MAP_WIDTH - 1);
            int uplen = Math.Max(ShowY - BombLen, 0);
            int downlen = Math.Min(ShowY + BombLen, Configuration.MAP_HEIGHT - 1);
            destRect.X = ShowX * DrawWidth;
            destRect.Y = ShowY * DrawHeight;
            destRect.Width = DrawWidth;
            destRect.Height = DrawHeight;
            g.DrawImage(explodeImage[4, type], destRect, srcRect, GraphicsUnit.Pixel);
            player.checkDie(ShowX, ShowY);
            RunningGame.ReduceMonsterHp(ShowX, ShowY);

            // 上
            for (int i = ShowY - 1; i >= uplen; i--)
            {
                if (GameMap.gameMap[i, ShowX] == Plat.WALL)
                {
                    break;
                }
                if (!player.IsDie)
                {
                    player.checkDie(ShowX, i);
                }
                RunningGame.ReduceMonsterHp(ShowX, i);

                destRect.X = ShowX * DrawWidth;
                destRect.Y = i * DrawHeight;
                destRect.Width = DrawWidth;
                destRect.Height = DrawHeight;

                // 如果碰到砖
                if (GameMap.gameMap[i, ShowX] == Plat.BRICK)
                {
                    GameMap.gameMap[i, ShowX] = Plat.BRICK_BREAK;
                    g.DrawImage(explodeImage[5, type], destRect, srcRect, GraphicsUnit.Pixel);
                    GameMap.explodeTime[i, ShowX] = Math.Max(time, GameMap.explodeTime[i, ShowX]);
                    break;
                }
                if (GameMap.gameMap[i, ShowX] == Plat.BRICK_BREAK
                    || GameMap.gameMap[i, ShowX] == Plat.BIG_WALL_SHOW
                    || GameMap.gameMap[i, ShowX] == Plat.BIG_WALL_HIDE)
                {
                    g.DrawImage(explodeImage[5, type], destRect, srcRect, GraphicsUnit.Pixel);
                    GameMap.explodeTime[i, ShowX] = Math.Max(time, GameMap.explodeTime[i, ShowX]);
                    break;
                }


                if (i - 1 >= 0 && GameMap.gameMap[i - 1, ShowX] == Plat.WALL)
                {
                    g.DrawImage(explodeImage[0, type], destRect, srcRect, GraphicsUnit.Pixel);
                    break;
                }
                if (i != uplen)
                {
                    g.DrawImage(explodeImage[0, 0], destRect, srcRect, GraphicsUnit.Pixel);
                }
                else
                {
                    g.DrawImage(explodeImage[0, type], destRect, srcRect, GraphicsUnit.Pixel);
                }
            }

            // 下
            for (int i = ShowY + 1; i <= downlen; i++)
            {
                if (GameMap.gameMap[i, ShowX] == Plat.WALL)
                {
                    break;
                }
                if (!player.IsDie)
                {
                    player.checkDie(ShowX, i);
                }
                RunningGame.ReduceMonsterHp(ShowX, i);
                destRect.X = ShowX * DrawWidth;
                destRect.Y = i * DrawHeight;
                destRect.Width = DrawWidth;
                destRect.Height = DrawHeight;
                // 如果碰到砖
                if (GameMap.gameMap[i, ShowX] == Plat.BRICK)
                {
                    GameMap.gameMap[i, ShowX] = Plat.BRICK_BREAK;
                    g.DrawImage(explodeImage[5, type], destRect, srcRect, GraphicsUnit.Pixel);
                    GameMap.explodeTime[i, ShowX] = Math.Max(time, GameMap.explodeTime[i, ShowX]);
                    break;
                }
                if (GameMap.gameMap[i, ShowX] == Plat.BRICK_BREAK
                    || GameMap.gameMap[i, ShowX] == Plat.BIG_WALL_SHOW
                    || GameMap.gameMap[i, ShowX] == Plat.BIG_WALL_HIDE)
                {
                    g.DrawImage(explodeImage[5, type], destRect, srcRect, GraphicsUnit.Pixel);
                    GameMap.explodeTime[i, ShowX] = Math.Max(time, GameMap.explodeTime[i, ShowX]);
                    break;
                }

                // 如果下一格是墙
                if (i + 1 < Configuration.MAP_HEIGHT && GameMap.gameMap[i + 1, ShowX] == Plat.WALL)
                {
                    g.DrawImage(explodeImage[1, type], destRect, srcRect, GraphicsUnit.Pixel);
                    break;
                }
                if (i != downlen)
                {
                    g.DrawImage(explodeImage[1, 0], destRect, srcRect, GraphicsUnit.Pixel);
                }
                else
                {
                    g.DrawImage(explodeImage[1, type], destRect, srcRect, GraphicsUnit.Pixel);
                }
            }

            // 左
            for (int i = ShowX - 1; i >= leftlen; i--)
            {
                // 如果第一格是墙
                if (GameMap.gameMap[ShowY, i] == Plat.WALL)
                {
                    break;
                }
                if (!player.IsDie)
                {
                    player.checkDie(i, ShowY);
                }
                RunningGame.ReduceMonsterHp(i, ShowY);
                destRect.X = i * DrawWidth;
                destRect.Y = ShowY * DrawHeight;
                destRect.Width = DrawWidth;
                destRect.Height = DrawHeight;
                // 如果碰到砖
                if (GameMap.gameMap[ShowY, i] == Plat.BRICK)
                {
                    GameMap.gameMap[ShowY, i] = Plat.BRICK_BREAK;
                    g.DrawImage(explodeImage[5, type], destRect, srcRect, GraphicsUnit.Pixel);
                    GameMap.explodeTime[ShowY, i] = Math.Max(time, GameMap.explodeTime[ShowY, i]);
                    break;
                }
                if (GameMap.gameMap[ShowY, i] == Plat.BRICK_BREAK
                    || GameMap.gameMap[ShowY, i] == Plat.BIG_WALL_SHOW
                    || GameMap.gameMap[ShowY, i] == Plat.BIG_WALL_HIDE)
                {
                    g.DrawImage(explodeImage[5, type], destRect, srcRect, GraphicsUnit.Pixel);
                    GameMap.explodeTime[ShowY, i] = Math.Max(time, GameMap.explodeTime[ShowY, i]);
                    break;
                }
                
                // 如果下一格是墙
                if (i - 1 >= 0 && GameMap.gameMap[ShowY, i - 1] == Plat.WALL)
                {
                    g.DrawImage(explodeImage[2, type], destRect, srcRect, GraphicsUnit.Pixel);
                    break;
                }

                // 判断是否到结束
                if (i != leftlen)
                {
                    g.DrawImage(explodeImage[2, 0], destRect, srcRect, GraphicsUnit.Pixel);
                }
                else
                {
                    g.DrawImage(explodeImage[2, type], destRect, srcRect, GraphicsUnit.Pixel);
                }
            }

            // 右
            for (int i = ShowX + 1; i <= rightlen; i++)
            {
                if (GameMap.gameMap[ShowY, i] == Plat.WALL)
                {
                    break;
                }
                if (player.IsDie)
                {
                    player.checkDie(i, ShowY);
                }
                RunningGame.ReduceMonsterHp(i, ShowY);
                destRect.X = i * DrawWidth;
                destRect.Y = ShowY * DrawHeight;
                destRect.Width = DrawWidth;
                destRect.Height = DrawHeight;

                // 如果碰到砖
                if (GameMap.gameMap[ShowY, i] == Plat.BRICK)
                {
                    GameMap.gameMap[ShowY, i] = Plat.BRICK_BREAK;
                    g.DrawImage(explodeImage[5, type], destRect, srcRect, GraphicsUnit.Pixel);
                    GameMap.explodeTime[ShowY, i] = Math.Max(time, GameMap.explodeTime[ShowY, i]);
                    break;
                }
                if (GameMap.gameMap[ShowY, i] == Plat.BRICK_BREAK
                    || GameMap.gameMap[ShowY, i] == Plat.BIG_WALL_SHOW
                    || GameMap.gameMap[ShowY, i] == Plat.BIG_WALL_HIDE)
                {
                    g.DrawImage(explodeImage[5, type], destRect, srcRect, GraphicsUnit.Pixel);
                    GameMap.explodeTime[ShowY, i] = Math.Max(time, GameMap.explodeTime[ShowY, i]);
                    break;
                }

                // 下一格是墙
                if (i + 1 < Configuration.MAP_WIDTH && GameMap.gameMap[ShowY, i + 1] == Plat.WALL)
                {
                    g.DrawImage(explodeImage[3, type], destRect, srcRect, GraphicsUnit.Pixel);
                    break;
                }

                // 是否到结尾
                if (i != rightlen)
                {
                    g.DrawImage(explodeImage[3, 0], destRect, srcRect, GraphicsUnit.Pixel);
                }
                else
                {
                    g.DrawImage(explodeImage[3, type], destRect, srcRect, GraphicsUnit.Pixel);
                }
            }
           
        }

        /// <summary>
        /// 检测可以炸到的炸弹。将他们的结束时间直接设为爆炸状态
        /// </summary>
        public void CheckBomb()
        {
            int leftlen = Math.Max(ShowX - BombLen, 0);
            int rightlen = Math.Min(ShowX + BombLen, Configuration.MAP_WIDTH - 1);
            int uplen = Math.Max(ShowY - BombLen, 0);
            int downlen = Math.Min(ShowY + BombLen, Configuration.MAP_HEIGHT - 1);
            for (int i = ShowX - 1; i >= leftlen && i >= 0; i--)
            {
                if (GameMap.gameMap[ShowY, i] == Plat.WALL)
                {
                    break;
                }
                if (GameMap.gameMap[ShowY, i] == Plat.BRICK)
                {
                    break;
                }
                if (GameMap.gameMap[ShowY, i] == Plat.BRICK_BREAK)
                {
                    break;
                }
                if (GameMap.bombMap[ShowY, i] != null && 
                    GameMap.bombMap[ShowY, i].IsExist &&
                    GameMap.bombMap[ShowY, i].EndTime < 1000)
                {
                    GameMap.bombMap[ShowY, i].EndTime = 1000;
                }
            }
            for (int i = ShowY - 1; i >= uplen && i >= 0; i--)
            {
                if (GameMap.gameMap[i, ShowX] == Plat.WALL)
                {
                    break;
                }
                if (GameMap.gameMap[i, ShowX] == Plat.BRICK)
                {
                    break;
                }
                if (GameMap.gameMap[i, ShowX] == Plat.BRICK_BREAK)
                {
                    break;
                }
                if (GameMap.bombMap[i, ShowX] != null &&
                    GameMap.bombMap[i, ShowX].IsExist &&
                    GameMap.bombMap[i, ShowX].EndTime < 1000)
                {
                    GameMap.bombMap[i, ShowX].EndTime = 1000;
                }
            }
            for (int i = ShowX + 1; i <= rightlen; i++)
            {
                if (GameMap.gameMap[ShowY, i] == Plat.WALL)
                {
                    break;
                }
                if (GameMap.gameMap[ShowY, i] == Plat.BRICK)
                {
                    break;
                }
                if (GameMap.gameMap[ShowY, i] == Plat.BRICK_BREAK)
                {
                    break;
                }
                if (GameMap.bombMap[ShowY, i] != null &&
                    GameMap.bombMap[ShowY, i].IsExist &&
                    GameMap.bombMap[ShowY, i].EndTime < 1000)
                {
                    GameMap.bombMap[ShowY, i].EndTime = 1000;
                }
            }
            for (int i = ShowY + 1; i <= downlen; i++)
            {
                if (GameMap.gameMap[i, ShowX] == Plat.WALL)
                {
                    break;
                }
                if (GameMap.gameMap[i, ShowX] == Plat.BRICK)
                {
                    break;
                }
                if (GameMap.gameMap[i, ShowX] == Plat.BRICK_BREAK)
                {
                    break;
                }
                if (GameMap.bombMap[i, ShowX] != null &&
                    GameMap.bombMap[i, ShowX].IsExist &&
                    GameMap.bombMap[i, ShowX].EndTime < 1000)
                {
                    GameMap.bombMap[i, ShowX].EndTime = 1000;
                }
            }
        }

        /// <summary>
        /// 画蛋蛋
        /// </summary>
        /// <param name="g"></param>
        /// <param name="player"></param>
        public static void Draw(Graphics g, Player player)
        {
            int len = GameMap.bombs.Count;
            for (int i = len - 1; i >= 0; i--)
            {
                // 炸弹已经爆炸
                if (GameMap.bombs[i].EndTime >= 1000)
                {
                    // 炸弹爆炸，归还炸弹
                    if (GameMap.bombs[i].EndTime == 1000)
                    {
                        // 这里想播放音效的，但是不能异步。。暂时无解
                        //SoundPlayer sp = new SoundPlayer(global::BombGame.Properties.Resources.explode);
                        //sp.Play();
                        if (GameMap.bombs[i].ComfromId == 1)
                        {
                            player.BombCountBuff++;
                        }
                    }
                    // 炸弹缓慢爆炸
                    GameMap.bombs[i].EndTime += 10;
                    // 检查是否还有炸弹可以爆炸
                    

                    // 消除炸弹
                    if (GameMap.bombs[i].EndTime >= 1140)
                    {
                        GameMap.bombMap[GameMap.bombs[i].ShowY, GameMap.bombs[i].ShowX] = null;
                        GameMap.bombs.Remove(GameMap.bombs[i]);
                    }
                    else
                    {
                        // 画炸弹爆炸的动画
                        GameMap.bombs[i].DrawExplode(g, (GameMap.bombs[i].EndTime - 1000) / 50 + 1,
                            1150 - GameMap.bombs[i].EndTime, player);
                        GameMap.bombs[i].CheckBomb();
                    }
                }
                else
                {
                    // 炸弹在颤抖
                    GameMap.bombs[i].Draw(g);
                }
            }
        }
    }
}
