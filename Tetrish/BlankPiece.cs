using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetrish
{
    internal class BlankPiece : Piece
    {
        private readonly Position[][] tiles = new Position[][]
        {
            new Position[] { new(0,0) }
        };

        public override int Id => 0;
        protected override Position StartOffset => new Position(0, 0);
        protected override Position[][] Tiles => tiles;
    }
}

