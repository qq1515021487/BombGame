using game.assemble;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombGame.assemble
{
    class Explode
    {
        public Explode()
        {

        }
        public Explode(Bomb b, int d)
        {
            this.B = b;
            this.Director = d;
        }
        public Bomb B { get; set; }
        // 上下左右 0123，4表示所有方向
        public int Director { get; set; }

    }
}
