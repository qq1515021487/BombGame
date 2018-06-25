using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BombGame
{
    public delegate void ShowForm();
    public delegate void CloseForm();
    public partial class Start : Form
    {
        // 游戏窗体
        RunningGame rg = null;
        public Start()
        {
            InitializeComponent();
            FormClosing += Form1_FormClosing;
            this.MaximizeBox = false;
        }

        /// <summary>
        /// 简单模式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            rg = new RunningGame();
            rg.ShowStartForm += new ShowForm(ShowThisForm);
            rg.CloseAllForm += CloseThisForm;
            rg.LoadGame(0);
            rg.Show();
            this.Hide();
        }

        /// <summary>
        /// 困难模式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            rg = new RunningGame();
            rg.ShowStartForm += new ShowForm(ShowThisForm);
            rg.CloseAllForm += CloseThisForm;
            rg.LoadGame(1);
            rg.Show();
            this.Hide();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult dr = MessageBox.Show("是否退出?", "提示:", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);

            if (dr == DialogResult.OK)   //如果单击“是”按钮
            {
                // 超干净的退出系统

                // System.Environment.Exit(0);
                if (rg != null)
                {
                    rg.Dispose();
                    rg.Close();
                }
                this.Dispose();
                this.Close();
            }
            else if (dr == DialogResult.Cancel)
            {
                e.Cancel = true;                  //不执行操作
            }
        }

        private void Start_Load(object sender, EventArgs e)
        {
            
        }

        public void ShowThisForm()
        {
            this.Show();
        }

        public void CloseThisForm()
        {
            this.Dispose();
            this.Close();
        }
    }
}
