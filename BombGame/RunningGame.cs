using BombGame.assemble;
using game.assemble;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.DirectX.DirectSound;

namespace BombGame
{
    public partial class RunningGame : Form
    {
        public static bool isClose = false;
        Random random = new Random();
        // 玩家1
        Player player = null;
        // 怪物
        public static Monster[] monster = new Monster[10];
        // 怪物数量
        public static int monsterCount = 3;
        // 当前关卡
        //private int Type = 0;
        // 显示主页窗体事件
        public event ShowForm ShowStartForm;
        // 关闭主页窗体事件
        public event CloseForm CloseAllForm;
        // 声音
        SoundPlayer sp = null;

        /// <summary>
        /// 初始化窗体
        /// </summary>
        /// <param name="type"></param>
        public RunningGame()
        {
            InitializeComponent();
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            KeyPreview = true;
            FormClosing += Form1_FormClosing;
            this.MaximizeBox = false;
            isShowDetailMessage = false;
            isLose = false;
        }
        
        /// <summary>
        /// 加载游戏
        /// </summary>
        public void LoadGame(int type)
        {
            GameMap.LoadMap(type);
            sp = new SoundPlayer(Properties.Resources.bg1);
            sp.PlayLooping();
            player = new Player(0, Configuration.HEIGHT, 1);

            // 加载三个怪兽，这里困难模式要不要多加几个Emmm
            //monster[0] = new Monster(0,  Configuration.HEIGHT, 100, random.Next(0, 3));
            //monster[1] = new Monster(0,  Configuration.HEIGHT, 100, random.Next(0, 3));
            //monster[2] = new Monster(0,  Configuration.HEIGHT, 100, random.Next(0, 3));

            if (type == 0)
            {
                monster[0] = new Monster(0, (Configuration.MAP_HEIGHT - 1) * Configuration.HEIGHT, 100, random.Next(0, 3));
                monster[1] = new Monster((Configuration.MAP_WIDTH - 1) * Configuration.WIDTH, (Configuration.MAP_HEIGHT - 3) * Configuration.HEIGHT, 100, random.Next(0, 3));
                monster[2] = new Monster((Configuration.MAP_WIDTH - 1) * Configuration.WIDTH, 2 * Configuration.HEIGHT, 100, random.Next(0, 3));
                monsterCount = 3;
            }
            else if (type == 1)
            {
                monster[0] = new Monster(0, (Configuration.MAP_HEIGHT - 1) * Configuration.HEIGHT, 100, random.Next(1, 3));
                monster[1] = new Monster((Configuration.MAP_WIDTH - 1) * Configuration.WIDTH, (Configuration.MAP_HEIGHT - 3) * Configuration.HEIGHT, 100, random.Next(1, 3));
                monster[2] = new Monster((Configuration.MAP_WIDTH - 1) * Configuration.WIDTH, 2 * Configuration.HEIGHT, 100, random.Next(1, 3));
                monster[3] = new Monster((Configuration.MAP_WIDTH - 1) * Configuration.WIDTH, (Configuration.MAP_HEIGHT - 7) * Configuration.HEIGHT, 100, random.Next(1, 3));
                monster[4] = new Monster(0, (Configuration.MAP_HEIGHT - 7) * Configuration.HEIGHT, 100, random.Next(1, 3));

                monsterCount = 5;
            }
            for (int i = 0; i < monsterCount; i++)
            {
                monster[i].CheckWin += CheckWin;
            }
        }
        /// <summary>
        /// 面板画图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            // 扫描所有的怪物
            for (int i = 0; i < monsterCount; i++)
            {
                // 怪物游走
                monster[i].Wander(player);
                // 怪物移动
                monster[i].Move();
                // 如果人物碰到怪物
                if (!player.IsDie && !monster[i].IsDie && 
                    player.checkDie(monster[i].X / Configuration.WIDTH, monster[i].Y / Configuration.HEIGHT))
                {
                    player.dieTime = 50;
                }
            }
            
            // 画地图
            GameMap.Draw(e.Graphics);
            // 检查地图上的炸弹，并画他们
            Bomb.Draw(e.Graphics, player);
            // 减少总体炸弹的时间
            GameMap.ReduceExplodeTime();
           
            // 画怪兽，并检测怪兽
            for (int i = 0; i < monsterCount; i++)
            {
                if (monster[i].IsDie && monster[i].dieTime < 30)
                {
                    monster[i].dieTime++;
                }
                monster[i].Draw(e.Graphics);
            }
            // 人物如果死亡
            if (player.IsDie)
            {
                player.dieTime++;
            }
            // 画人
            player.Draw(e.Graphics);
            // 刷新砖块，查看是否给破坏
            GameMap.ReloadBrickBreak();
            if (isWin && !isShowDetailMessage)
            {
                isShowDetailMessage = true;
                // 这里应该返回首页
                MessageBox.Show("游戏胜利！");
                ShowStartForm();
                sp.Dispose();
                sp.Stop();
                this.Dispose();
                this.Close();
            }
            if (player.IsDie && player.dieTime >= 80 && !isShowDetailMessage)
            {
                isShowDetailMessage = true;
                isLose = true;
                MessageBox.Show("你已经死亡，游戏结束，任务失败");
                ShowStartForm();
                sp.Dispose();
                sp.Stop();
                this.Dispose();
                this.Close();
            }
        }
        
        /// <summary>
        /// 人物移动事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer1_Tick(object sender, EventArgs e)
        {
            // 刷新面板
            this.pictureBox1.Refresh();
            if (player != null)
            {
                if (player.runState == State.WALK)
                {
                    player.Move();
                }
            }
        }

        /// <summary>
        /// 怪物掉血
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public static void ReduceMonsterHp(int x, int y)
        {
            // 遍历每个怪物，判断他们是否会受伤
            for (int i = 0; i < monsterCount; i++)
            {
                monster[i].ReduceHp(x, y);
            }
        }

        // 计时器开始时间
        int mint = 4;
        int scss = 59;
        bool isLose;

        private void timer3_Tick(object sender, EventArgs e)
        {
            if (!isLose)
            {
                if (mint >= 0)
                {
                    scss--;
                    if (scss == 0)
                    {
                        // 游戏结束，游戏失败
                        if (mint == 0 && scss == 0)
                        {
                            isLose = true;
                            MessageBox.Show("时间到，游戏结束，任务失败");
                            ShowStartForm();
                            sp.Dispose();
                            sp.Stop();
                            this.Dispose();
                            this.Close();
                            // 这里弹个窗？
                            return;
                        }
                        mint--;
                        scss = 59;
                    }
                    label1.Text = mint.ToString() + ":" + scss.ToString();
                }
            }
            
        }

        bool isShowDetailMessage;
        bool isWin;
        /// <summary>
        ///  检查游戏胜利
        /// </summary>
        public void CheckWin()
        {
            isWin = true;
            for (int i = 0; i < monsterCount; i++)
            {
                if (!monster[i].IsDie)
                {
                    isWin = false;
                }
            }
            
        }

        /// <summary>
        /// form关闭事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {   
            DialogResult dr = MessageBox.Show("是否退出?", "提示:", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
           
            if (dr == DialogResult.OK)   //如果单击“是”按钮
            {
                // 超干净的退出系统
                // System.Environment.Exit(0);
                this.CloseAllForm();
                this.Dispose();
                this.Close();
            }
            else if (dr == DialogResult.Cancel)
            {
                e.Cancel = true;                  //不执行操作
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
        }

        // 页面加载
        private void RunningGame_Load(object sender, EventArgs e)
        {
            this.KeyDown += HideSpace;
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(player.PlayerKeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(player.PlayerKeyUP);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(player.SetBomb);
        }

        // 隐藏空格触发的Button事件
        private void HideSpace(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space || e.KeyCode == Keys.Enter)
            {
                e.Handled = true;//表示已经处理了键盘消息
            }
        }

        /// <summary>
        /// 退出当前游戏，放回主菜单按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click_1(object sender, EventArgs e)
        {
            ShowStartForm();
            sp.Dispose();
            sp.Stop();
            this.Dispose();
            this.Close();
        }
    }
}
