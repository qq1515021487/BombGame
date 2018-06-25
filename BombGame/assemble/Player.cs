using game.assemble;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BombGame.assemble
{
    public enum State {
        WALK,
        STAND,
        UP,
        DOWN,
        LEFT,
        RIGHT
    }
    public class Player
    {
        // 人物ID，本来想做多个角色的，貌似不行就算了
        public int ID { get; set; }

        // 是否死亡
        public bool IsDie { get; set; }

        // 死亡动画时间
        public int dieTime = 0;

        // 是否在运动
        public State runState;

        // 面向的方向
        public State derState;

        // 上下左右
        public int[] direction = new int[4];

        // 人物坐标
        public int X { get; set; }
        public int Y { get; set; }

        // 炸弹数量
        public int BombCountBuff { get; set; }

        // 炸弹Buff
        public int BombPowerBuff { get; set; }

        // 速度Buff
        public int SpidBuff { get; set; }

        // 行走距离
        public readonly static int WALK_STEP = 10;

        // 人物死亡动画
        public static Image[] myDie =
        {
            global::BombGame.Properties.Resources.die_01_01,
            global::BombGame.Properties.Resources.die_01_02,
            global::BombGame.Properties.Resources.die_01_03,
            global::BombGame.Properties.Resources.die_01_04,
            global::BombGame.Properties.Resources.die_01_05,
            global::BombGame.Properties.Resources.die_01_06,
            global::BombGame.Properties.Resources.die_01_07,
            global::BombGame.Properties.Resources.die_01_08,
        };

        // 人物的图像，上下左右
        public static Image[,] myPlayer = 
        {
            {
                global::BombGame.Properties.Resources.character_1_up_01,
                global::BombGame.Properties.Resources.character_1_up_02,
                global::BombGame.Properties.Resources.character_1_up_03,
                global::BombGame.Properties.Resources.character_1_up_04,
            },
            {
                global::BombGame.Properties.Resources.character_1_down_01,
                global::BombGame.Properties.Resources.character_1_down_02,
                global::BombGame.Properties.Resources.character_1_down_03,
                global::BombGame.Properties.Resources.character_1_down_04,
            },
            {
                global::BombGame.Properties.Resources.character_1_left_01,
                global::BombGame.Properties.Resources.character_1_left_02,
                global::BombGame.Properties.Resources.character_1_left_03,
                global::BombGame.Properties.Resources.character_1_left_04,
            },
            {
                global::BombGame.Properties.Resources.character_1_right_01,
                global::BombGame.Properties.Resources.character_1_right_02,
                global::BombGame.Properties.Resources.character_1_right_03,
                global::BombGame.Properties.Resources.character_1_right_04,
            },
            {
                global::BombGame.Properties.Resources.character_stand_01,
                global::BombGame.Properties.Resources.character_stand_02,
                global::BombGame.Properties.Resources.character_stand_03,
                global::BombGame.Properties.Resources.character_stand_04,
            }
        };

        public Image[,] character;

        public Player(int x, int y, int ID)
        {
            character = myPlayer;
            runState = State.STAND;
            derState = State.DOWN;
            this.X = x;
            this.Y = y;
            this.ID = ID;
            BombCountBuff = BombPowerBuff = SpidBuff = 1;
            IsDie = false;
        }

        /// <summary>
        /// 是否可以移动到(x, y)
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool CanMove(int x, int y)
        {
            // 越界，失败
            if (x < 0 || x >= Configuration.MAP_WIDTH * Configuration.WIDTH)
            {
                return false;
            }
            if (y < 0 || y >= Configuration.MAP_HEIGHT * Configuration.HEIGHT)
            {
                return false;
            }
            // 化成坐标
            x = x / Configuration.WIDTH;
            y = y / Configuration.HEIGHT;

            if (GameMap.gameMap[y, x] == Plat.BIG_WALL_SHOW ||
                GameMap.gameMap[y, x] == Plat.BIG_WALL_HIDE)
            {
                return false;
            }
            // 碰壁不允许走
            if (GameMap.gameMap[y, x] == Plat.WALL)
            {
                return false;
            }
            if (GameMap.gameMap[y, x] == Plat.BRICK)
            {
                return false;
            }
            if (GameMap.bombMap[y, x] != null && GameMap.bombMap[y, x].IsExist)
            {
                return false;
            }
            return true;
        }

        Rectangle destRect;
        Rectangle srcRect = new Rectangle(0, 0, Configuration.WIDTH, Configuration.HEIGHT);

        /// <summary>
        /// 画人物
        /// </summary>
        /// <param name="g"></param>
        public void Draw(Graphics g)
        {
            destRect = new Rectangle(X, Y, Configuration.WIDTH, Configuration.HEIGHT);
            // 人物如果死亡
            if (IsDie)
            {
                // 在死亡动画里
                if (dieTime < 80)
                {
                    g.DrawImage(myDie[dieTime / 10], destRect, srcRect, GraphicsUnit.Pixel);
                }
            }
            else
            {
                // 如果在走动
                if (runState == State.WALK)
                {
                    if (derState == State.UP)
                    {
                        g.DrawImage(character[0, direction[0]], destRect, srcRect, GraphicsUnit.Pixel);
                    }
                    else if (derState == State.DOWN)
                    {
                        g.DrawImage(character[1, direction[1]], destRect, srcRect, GraphicsUnit.Pixel);
                    }
                    else if (derState == State.LEFT)
                    {
                        g.DrawImage(character[2, direction[2]], destRect, srcRect, GraphicsUnit.Pixel);
                    }
                    else if (derState == State.RIGHT)
                    {
                        g.DrawImage(character[3, direction[3]], destRect, srcRect, GraphicsUnit.Pixel);
                    }
                }
                // 否则就站着
                else
                {
                    if (derState == State.UP)
                    {
                        g.DrawImage(character[4, 0], destRect, srcRect, GraphicsUnit.Pixel);
                    }
                    else if (derState == State.DOWN)
                    {
                        g.DrawImage(character[4, 1], destRect, srcRect, GraphicsUnit.Pixel);
                    }
                    else if (derState == State.LEFT)
                    {
                        g.DrawImage(character[4, 2], destRect, srcRect, GraphicsUnit.Pixel);
                    }
                    else if (derState == State.RIGHT)
                    {
                        g.DrawImage(character[4, 3], destRect, srcRect, GraphicsUnit.Pixel);
                    }
                }
            }
            
            
        }

        // 用户按下键盘
        public void PlayerKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.W || e.KeyCode == Keys.S
                || e.KeyCode == Keys.A || e.KeyCode == Keys.D)
            {
                runState = State.WALK;
            }
            switch (e.KeyCode)
            {
                // 向上
                case Keys.W:
                    derState = State.UP;
                    direction[0] = (direction[0] + 1) % 4;
                    break;
                case Keys.S:
                    derState = State.DOWN;
                    direction[1] = (direction[1] + 1) % 4;
                    break;
                case Keys.A:
                    derState = State.LEFT;
                    direction[2] = (direction[2] + 1) % 4;
                    break;
                case Keys.D:
                    derState = State.RIGHT;
                    direction[3] = (direction[3] + 1) % 4;
                    break;
            }
        }

        /// <summary>
        /// 键盘抬起事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void PlayerKeyUP(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.W || e.KeyCode == Keys.S
                || e.KeyCode == Keys.A || e.KeyCode == Keys.D)
            {
                runState = State.STAND;
            }
        }

        /// <summary>
        /// 吃道具
        /// </summary>
        public void EatProp()
        {
            
            int x = X + Configuration.WIDTH / 2;
            int y = Y + Configuration.HEIGHT / 2;
            x = x / Configuration.WIDTH;
            y = y / Configuration.HEIGHT;
            if (GameMap.prop[y, x] == 1)
            {
                //SoundPlayer sp = new SoundPlayer(global::BombGame.Properties.Resources.get);
               // sp.PlaySync();
                // WavPlayer.PlayOnce(global::BombGame.Properties.Resources.get.Position);
                this.BombCountBuff++;
            }
            else if (GameMap.prop[y, x] == 2)
            {
                //SoundPlayer sp = new SoundPlayer(global::BombGame.Properties.Resources.get);
                //sp.PlaySync();
                this.BombPowerBuff++;
            }
            else if (GameMap.prop[y, x] == 3)
            {
                //SoundPlayer sp = new SoundPlayer(global::BombGame.Properties.Resources.get);
                //sp.PlaySync();
                this.SpidBuff++;
            }
            GameMap.prop[y, x] = 0;
        }

        /// <summary>
        /// 用户进行操作
        /// </summary>
        public void Move()
        {
            if (IsDie)
            {
                return;
            }
            runState = State.WALK;
            bool is1, is2;
            switch (derState)
            {
                // 向上
                case State.UP:
                    direction[0] = (direction[0] + 1) % 4;
                    is1 = CanMove(X, Y - WALK_STEP);
                    is2 = CanMove(X + Configuration.WIDTH - 1, Y - WALK_STEP);
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
                case State.DOWN:
                    direction[1] = (direction[1] + 1) % 4;
                    is1 = CanMove(X, Y + Configuration.HEIGHT + WALK_STEP - 1);
                    is2 = CanMove(X + Configuration.WIDTH - 1, Y + Configuration.HEIGHT + WALK_STEP - 2);
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
                case State.LEFT:
                    direction[2] = (direction[2] + 1) % 4;
                    is1 = CanMove(X - WALK_STEP, Y);
                    is2 = CanMove(X - WALK_STEP, Y + Configuration.HEIGHT - 1);
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
                case State.RIGHT:
                    direction[3] = (direction[3] + 1) % 4;
                    is1 = CanMove(X + Configuration.WIDTH + WALK_STEP - 2, Y);
                    is2 = CanMove(X + Configuration.WIDTH + WALK_STEP - 2, Y + Configuration.HEIGHT - 1);
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
            EatProp();
        }

        /// <summary>
        /// 放蛋蛋
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void SetBomb(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.J && BombCountBuff > 0 && !this.IsDie)
            {
                // 放蛋蛋
                int x = X + Configuration.WIDTH / 2;
                int y = Y + Configuration.HEIGHT / 2;
                if (GameMap.bombMap[y / Configuration.HEIGHT, x / Configuration.WIDTH] == null ||
                    !GameMap.bombMap[y / Configuration.HEIGHT, x / Configuration.WIDTH].IsExist)
                {
                    // 如果没冲突那就放个蛋蛋
                    Bomb b = new Bomb(x / Configuration.WIDTH, y / Configuration.HEIGHT, BombPowerBuff, 1, ID);
                    BombCountBuff--;
                    GameMap.bombs.Add(b);
                    GameMap.bombMap[y / Configuration.HEIGHT, x / Configuration.WIDTH] = b;
                }
            }
        }

        /// <summary>
        /// 检测人物是否死亡
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool checkDie(int x, int y)
        {
            int dx = (X + Configuration.WIDTH / 2) / Configuration.WIDTH;
            int dy = (Y + Configuration.HEIGHT / 2) / Configuration.HEIGHT;
            if (x == dx && y == dy)
            {
                IsDie = true;
                return true;
            }
            return false;
        }
    }
}
