using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetrish
{
    internal class PiecePicker
    {
        private readonly Piece[] pieces = new Piece[] 
        { 
            new LinePiece(),
            new RBend_Piece(),
            new LBend_Piece(),
            new SquarePiece(),
            new Plus_Piece(),
            new Zig_Piece(),
            new Zag_Piece(),
        };

        private readonly Random random= new Random();

        public Piece NextPiece { get; private set; }

        public PiecePicker()
        {
            NextPiece = RandomPiece();
        }

        private Piece RandomPiece()
        {
            return pieces[random.Next(pieces.Length)];
        }

        public Piece NewPiece()
        {
            Piece piece = NextPiece;

            do
            {
                NextPiece = RandomPiece();
            } 
            while( piece.Id == NextPiece.Id );

            return piece;
        }
    }
}
