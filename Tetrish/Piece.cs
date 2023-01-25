using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetrish
{
    internal abstract class Piece
    {
        protected abstract Position[][] Tiles { get; }
        protected abstract Position StartOffset { get; }
        public abstract int Id { get; }

        private int rotationState;
        private Position offset;
        public Piece()
        {
            offset = new Position(StartOffset.row, StartOffset.column);
        }

        public IEnumerable<Position> TilePositions()
        {
            foreach (Position p in Tiles[rotationState])
            {
                yield return new Position(p.row + offset.row, p.column + offset.column);
            }
        }

        public void RotateClockwise()
        {
            rotationState= ( rotationState + 1 ) % Tiles.Length;
        }

        public void RotateAntiClockwise()
        {
            if (rotationState == 0)
            {
                rotationState = Tiles.Length - 1;
            }
            else
            {
                rotationState--;
            }
        }

        public void Move(int rows, int columns)
        {
            offset.row+= rows;
            offset.column+= columns;
        }

        public void Reset()
        {
            rotationState = 0;
            offset.row = StartOffset.row;
            offset.column = StartOffset.column;
        }
    }
}
